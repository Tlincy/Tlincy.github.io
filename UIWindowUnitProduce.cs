using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Triniti.AW.UI;
using UnityEngine.UI;
using Triniti.AW.UI.Game.Core;

public class UIWindowUnitProduce : UIWindowBuilding
{
	public UIUnitList unitList;
	public UIReplaceUnitList replaceUnitList;
	public Transform unitListItemAnchor;
	public Transform replaceUnitListAnchor;
	public Button construction;
	private Barrack barrack;
	private UICampItem campItem;
	public GameObject campObj;
	public Transform campAnchor;
	private List<int> campLists;
	private UICopy m_campCopy;
	public GameObject processObj;
	public GameObject troopsItem;
	public Transform troopsInfoAnchor;
	protected override void Awake()
	{
		base.Awake();
		unitList.gameObject.AddComponent<UICopy>();
		replaceUnitList.gameObject.AddComponent<UICopy>();
		m_campCopy = campAnchor.gameObject.AddComponent<UICopy>();
		m_campCopy.sample = campObj;
		campLists = new List<int>();
	}
	public override void Open(Building building)
	{
		barrack = (Barrack)building;
		base.Open(building);
        CloseUnitList();
		InitOrUpdateUnitList();
	}
	public override void Close()
	{
		base.Close();
		UIWindowHome home = UIWindowWrapper.GetWindow<UIWindowHome>();
		UIWindowPlayerInfo playerInfo = UIWindowWrapper.GetWindow<UIWindowPlayerInfo>();
		home.Open();
		playerInfo.Open();
        CloseUnitList();
	}
    private void CloseUnitList()
    {
        unitList.Close();
        replaceUnitList.Close();
    }
	public void InitOrUpdateUnitList()
	{
		InitProduce();
		Barrack barrack = m_building as Barrack;
		Dictionary<int, Camp> produceTroops = barrack.GetProduceTroopDic();
		//AW.GetSingleton<IDataAdapter>().GetBuildingConfItem(barrack).GetCampVolume();
		List<int> produceUnitLists = AW.GetSingleton<IDataAdapter>().GetProduceUnitList(m_building.GetBuildingType());
		unitList.InitOrUpdateUnitList(produceUnitLists, m_building, unitListItemAnchor);
		if (campLists != null)
		{
			campLists.Clear();
		}
		campLists.AddRange(produceTroops.Keys);
        campLists.Sort();
		itemList.Clear();
		m_campCopy.UpdateCopy(campLists.Count, UpdateCampCopy);
	}
	private void UpdateCampCopy(int index, GameObject copy)
	{
        UpdateCampCreate(campLists[index], copy);
	}
	//! 进度条
	protected AWBuildingProcess m_buildingProcess;
	private void UpdateCampCreate(int camp, GameObject copy)
	{
		AWBuildingBarrack buildingBarrack = AWCore.Instance.GetGame().GetScene<AWSceneHome>().GetAWBuilding<AWBuildingBarrack>(m_building);
        IBuildingConfItem buildingConfItem = AW.GetSingleton<IDataAdapter>().GetBuildingConfItem(m_building);
		List<int> produceUnitLists = AW.GetSingleton<IDataAdapter>().GetProduceUnitList(m_building.GetBuildingType());
		Dictionary<int, Camp> produceTroops = barrack.GetProduceTroopDic();
		copy.transform.SetParent(campAnchor, false);
		copy.GetComponent<UISetAnchor>().SetAnchor(buildingBarrack.GetModelAnchor(camp), UIAnchorType.World3DToUI2D);
		UICampItem campItem = copy.GetComponent<UICampItem>();
        campItem.InitCamp(produceTroops, camp, buildingConfItem);
        campItem.SetUnitName();
		campItem.SetSelect(false);
		itemList.Add(campItem);
        Button button = copy.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(CloseUnitList);
		button.onClick.AddListener(ResetCampItem);
        button.onClick.AddListener(new OnClickCampEvent(construction, troopsInfoAnchor, produceTroops[camp], replaceUnitList, produceUnitLists, m_building, replaceUnitListAnchor,campItem).OnClickCreateTroopsInfo);
        m_buildingProcess = copy.ExtGetMissingComponent<AWBuildingProcess>();
		m_buildingProcess.InitProcess(produceTroops[camp], copy.transform, new Vector3(-20, 80, 0), UIAnchorType.UI2DToUI2D);
		m_buildingProcess.SetShow(true);
	}
	List<UICampItem> itemList = new List<UICampItem>();
	private void ResetCampItem()
	{
		foreach(UICampItem item in itemList)
		{
			item.SetSelect(false);
		}
	}
	public void InitProduce()
	{
		construction.onClick.RemoveAllListeners();
		construction.onClick.AddListener(OnClickConstruction);
		if (AW.GetSingleton<IDataAdapter>().GetIdleCamp(barrack))
		{
			construction.gameObject.SetActive(true);
		}
		else
		{
			construction.gameObject.SetActive(false);
		}
	}
	public void OnClickConstruction()
	{
        unitList.Open();
	}
	class OnClickCampEvent
	{
		Button m_construction;
		Transform m_troopsAnchor;
		Transform m_unitListAnchor;
		Camp m_camp;
		Building m_building;
		List<int> m_produceUnitList;
		UIReplaceUnitList m_unitList;
        UICampItem m_campItem;
		int m_count;
		public OnClickCampEvent(Button construction, Transform troopsAnchor, Camp camp, UIReplaceUnitList unitList, List<int> produceUnitList, Building building, Transform listAnchor,UICampItem campItem)
		{
			m_construction = construction;
			m_troopsAnchor = troopsAnchor;
			m_unitListAnchor = listAnchor;
			m_camp = camp;
			m_produceUnitList = produceUnitList;
			m_unitList = unitList;
			m_building = building;
            m_campItem = campItem;
		}

		public void OnClickCreateTroopsInfo()
		{
			m_campItem.SetSelect(true);
			UIWindowTroopsInfo info = UIWindowWrapper.GetWindowAddMask<UIWindowTroopsInfo>();
			info.transform.SetParent(m_troopsAnchor, false);
			info.Open(m_camp);
			IUnitConfItem unitConfItem = AW.GetSingleton<IDataAdapter>().GetOwnedUnitConfItem(m_camp.ProduceItem);
			IBuildingConfItem buildingConfItem = AW.GetSingleton<IDataAdapter>().GetBuildingConfItem(m_building);
			m_count = buildingConfItem.GetTroopCostVolume() / unitConfItem.GetCapacityCost();
			Building barrck = (Building)m_camp;
            if (barrck.GetState() == AWEnum.BuildingStateType.Produce)
            {
                info.replace.gameObject.SetActive(false);
                info.SetCompleteButtonStyle(OnClickComplete);
                info.SetCancelButtonStle(OnClickCancel);
                if (m_camp.GetProducedCount() < m_count)
                {
                    info.SetSupplementButtonStyle(OnClickSupplement);
                }
                else
                {
                    info.supplement.gameObject.SetActive(false);
                }
            }
            else
            {
                info.complete.gameObject.SetActive(false);
                info.cancel.gameObject.SetActive(false);
                if (m_camp.GetProducedCount() < m_count)
                {
                    info.SetSupplementButtonStyle(OnClickSupplement);
                    info.SetReplaceButtonStyle(OnClickReplace);
                }
                else
                {
                    info.supplement.gameObject.SetActive(false);
                    info.SetReplaceButtonStyle(OnClickReplace);
                }
            }
            info.SetRename(OnClickRename);
		}
		private void OnClickComplete()
		{
			Debug.Log("OnClickComplete");
			AWCore.Instance.GetControl().PostEvent(new BoostUnitEvent(m_camp, AW.GetSingleton<IDataAdapter>().GetServerTime(), CallBack));
		}
		private void OnClickCancel()
		{
			Debug.Log("OnClickCancel");
			AWCore.Instance.GetControl().PostEvent(new CancelUnitEvent(m_camp, AW.GetSingleton<IDataAdapter>().GetServerTime(), CallBack));
		}
		public void OnClickReplace()
		{
			Debug.Log("OnClickReplace");
            m_construction.gameObject.SetActive(false);
			if (m_produceUnitList.Contains(m_camp.ProduceItem))
			{
				m_produceUnitList.Remove(m_camp.ProduceItem);
			}
			m_unitList.InitReplaceUnitList(m_produceUnitList, m_building, m_camp, m_unitListAnchor);
            m_unitList.Open();
		}
		private void OnClickSupplement()
		{
			Debug.Log("OnClickSupplement");
			AWCore.Instance.GetControl().PostEvent(new SupplyUnitEvent(m_camp, m_count - m_camp.GetProducedCount(), 1, CallBack));
		}
        private void OnClickRename()
        {
            Debug.Log("OnClickRename");
            UIWindowEnterName name = UIWindowWrapper.GetWindowAddMask<UIWindowEnterName>();
            name.Open();
            name.SetStyle(OnNameVerify);
           
        }
        private void OnNameVerify(string name)
        {
            AWCore.Instance.GetControl().PostEvent(new ChangeUnitNameEvent(m_camp, name, RenameCallBack));
            
        }
		private void CallBack(bool result)
		{
			m_troopsAnchor.GetComponentInChildren<UIWindowTroopsInfo>().Close();
			UIWindowWrapper.GetWindowAddMask<UIWindowUnitProduce>().InitProduce();
		}
        private void RenameCallBack(bool result)
        {
            UIWindowWrapper.GetWindowAddMask<UIWindowEnterName>().Close();
            m_campItem.SetUnitName();
        }
	}
}
