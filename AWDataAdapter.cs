using System.Collections;
using System.Collections.Generic;
using Triniti.AW.UI.Pattern;
using Triniti.AW.UI.Common;
using UnityEngine;
using Triniti.AW.UI;

//! 数据适配器
public class AWDataAdapter : IDataAdapter
{
	//! Version3.0
	#region Version3.0
	//! 获取当前任务故事项
	public List<UIStoryItem> GetTaskUIStoryItem()
	{
		List<UIStoryItem> itemList = new List<UIStoryItem>();
		List<Newbie.Dialogue> taskList = Newbie.GetDialogueTask();
		foreach (Newbie.Dialogue task in taskList)
		{
			itemList.Add(new UIStoryItem(task));
		}
		return itemList;
	}
	//! 获取当前故事项
	public List<UIStoryItem> GetStoryUIStoryItem(int progress, int progressSub)
	{
		List<UIStoryItem> itemList = new List<UIStoryItem>();
		List<Newbie.Dialogue> storyList = Newbie.GetDialogueStory(progress, progressSub);
		foreach (Newbie.Dialogue story in storyList)
		{
			itemList.Add(new UIStoryItem(story));
		}
		return itemList;
	}
	//! 获取IAPMedal
	public List<IAPMedalItem> GetIAPMedal()
	{
		return Conf.GetInstance().IAPMedal.GetItems();
	}
	//! 获取IAPMoney
	public List<IAPMoneyItem> GetIAPMoney()
	{
		return Conf.GetInstance().IAPMoney.GetItems();
	}
	//! 获取升星经验
	public int GetStarExp(int unitId)
	{
		int exp = 0;
		if(DataCenter.GetInstance().GetDataSave().elitePoints.TryGetValue(unitId, out exp))
		{
			return exp;
		}
		return 0;
	}
	//! 获取最大星级
	public int GetStarMaxLevel()
	{
		return GetCommandConfItem(GetCurCommandConfItem().GetMaxLevel()).GetUnlockStarLevel();
	}
	//! 获取兵种项
	public IUnit2ConfItem GetUnit2ConfItem(string troopId)
	{
		DataSave.Troop troop = DataCenter.GetInstance().GetDataSave().troops[troopId];
		return new Unit2ConfItem(troop.unitId, troop.grade, troop.reform, troop.star);
	}
	//! 获取当前兵种序列
	public List<string> GetCurTroopList()
	{
		return new List<string>(DataCenter.GetInstance().GetDataSave().troops.Keys);
	}
	//! 获取部队空地ID
	public int GetTroopSlot(string troopId)
	{
		return DataCenter.GetInstance().GetDataSave().troops[troopId].slot;
	}
	//! 获取维修时间
	public int GetTroopRepairTime(string troopId)
	{
		if (Newbie.GetNewbieStatus() == 20 && Newbie.GetNewbieStatusSub() == 1)
		{
			return 3600 * 24;
		}
		return Mathf.Clamp(DataCenter.GetInstance().GetDataSave().troops[troopId].cd - GetServerTime(), 0, int.MaxValue);
	}
	//! 是否需要维修
	public bool CanTroopRepair(string troopId)
	{
		return GetTroopRepairTime(troopId) > 0;
	}
	//! 获取部队名字
	public string GetTroopName(string troopId)
	{
		return DataCenter.GetInstance().GetDataSave().troops[troopId].troopName;
	}
	//! 获取部队统计数据
	public Dictionary<string, string> GetTroopExploit(string troopId)
	{
		Dictionary<string, string> exploit = new Dictionary<string, string>();
		exploit.Add("COMBAT", "929");
		exploit.Add("SURVIVE", "999");
		exploit.Add("DESTORY", "30%");
		exploit.Add("POWER", "12958");
		return exploit;
	}
	//! 获取当前防御工事序列
	public List<int> GetCurDefenceList()
	{
		return new List<int>(DataCenter.GetInstance().GetDataSave().defences.Keys);
	}
	//! 获取防御工事项
	public IDefenceConfItem GetDefenceConfItem(int defenceId, int reform = 0)
	{
		return new DefenceConfItem(defenceId, reform);
	}
	//! 获取当前防御工事项
	public IDefenceConfItem GetCurDefenceConfItem(int defenceId)
	{
		if (DataCenter.GetInstance().GetDataSave().defences.ContainsKey(defenceId))
		{
			return GetDefenceConfItem(DataCenter.GetInstance().GetDataSave().defences[defenceId].unitId, DataCenter.GetInstance().GetDataSave().defences[defenceId].reform);
		}
		return null;
	}

	//! 获取当前使用人口
	public int GetUsedTroopCost()
	{
		int cost = 0;
		foreach (DataSave.Troop troop in DataCenter.GetInstance().GetDataSave().troops.Values)
		{
			cost += GetUnit2ConfItem(troop.troopId).GetTroopCost();
		}
		return cost;
	}
	//! 获取剩余人口
	public int GetEmptyTroopCost()
	{
		return GetCurSupplyConfItem().GetTroopCost() - GetUsedTroopCost();
	}
	//! 获取是否还有空地使用
	public bool IsHaveEmptySlot(int slotType)
	{
		return GetUsedSlotList(slotType).Count < GetCurCommandConfItem().GetSlotList(slotType).Count;
	}
	//!获取可用空地序列
	private List<int> GetEmptySlotList(int slotType)
	{
		List<int> slotList = GetCurCommandConfItem().GetSlotList(slotType);
		List<int> usedList = GetUsedSlotList(slotType);
		foreach (int slot in usedList)
		{
			slotList.Remove(slot);
		}
		slotList.Sort();
		return slotList;
	}
	//! 获取当前可用空地
	public int GetFirstEmptySlot(int slotType)
	{
		List<int> emptySlotList = GetEmptySlotList(slotType);
		if (emptySlotList.Count > 0)
		{
			return emptySlotList[0];
		}
		return -1;
	}
	//! 获取使用空地序列
	public List<int> GetUsedSlotList(int slotType)
	{
		List<int> slotList = new List<int>();
		foreach (DataSave.Troop troop in DataCenter.GetInstance().GetDataSave().troops.Values)
		{
			if (GetUnit2ConfItem(troop.troopId).GetSlotType() == slotType)
			{
				slotList.Add(troop.slot);
			}
		}
		slotList.Sort();
		return slotList;
	}
	//! 获取新增空地列表
	public List<int> GetNewSlot(int slotType)
	{
		List<int> slotList = GetCurCommandConfItem().GetSlotList(slotType);
		if (GetCurCommandLevel() > 1)
		{
			List<int> lastSlotList = GetCommandConfItem(GetCurCommandLevel() - 1).GetSlotList(slotType);
			foreach (int i in lastSlotList)
			{
				slotList.Remove(i);
			}
		}
		return slotList;
	}
	//! 获取当前工厂
	public List<int> GetCurFactory()
	{
		List<int> factory = new List<int>();
		if (DataCenter.GetInstance().GetDataSave().building.factory0 != 0)
		{
			factory.Add(0);
		}
		if (DataCenter.GetInstance().GetDataSave().building.factory1 != 0)
		{
			factory.Add(1);
		}
		if (DataCenter.GetInstance().GetDataSave().building.factory2 != 0)
		{
			factory.Add(2);
		}
		return factory;
	}
	public List<int> GetCanBuyFactory()
	{
		List<int> factory = new List<int>();
		if (DataCenter.GetInstance().GetDataSave().building.factory1 == 0)
		{
			factory.Add(1);
		}
		if (DataCenter.GetInstance().GetDataSave().building.factory2 == 0)
		{
			factory.Add(2);
		}
		return factory;
	}
	//! 获取工厂冷却
	public int GetFactoryCD(int factoryId)
	{
		if (factoryId == 0)
		{
			return Mathf.Clamp(DataCenter.GetInstance().GetDataSave().building.factory0cd - GetServerTime(), 0, int.MaxValue);
		}
		if (factoryId == 1)
		{
			return Mathf.Clamp(DataCenter.GetInstance().GetDataSave().building.factory1cd - GetServerTime(), 0, int.MaxValue);
		}
		else if (factoryId == 2)
		{
			return Mathf.Clamp(DataCenter.GetInstance().GetDataSave().building.factory2cd - GetServerTime(), 0, int.MaxValue);
		}
		return 0;
	}
	//! 是否工厂可以维修
	public bool CanFactoryRepair()
	{
		foreach (string troopId in GetCurTroopList())
		{
			if (CanTroopRepair(troopId))
			{
				return true;
			}
		}
		return false;
	}
	//! 获取司令部项
	public ICommandConfItem GetCommandConfItem(int lv)
	{
		return new CommandConfItem(lv);
	}
	//! 获取当前司令部项
	public ICommandConfItem GetCurCommandConfItem()
	{
		return new CommandConfItem(GetCurCommandLevel());
	}
	//! 获取当前司令部等级
	public int GetCurCommandLevel()
	{
		return DataCenter.GetInstance().GetDataSave().GetBuildingLevelCommand();
	}
	//! 获取当前司令部经验
	public int GetCurCommandExp()
	{
		return 0;
		//return DataCenter.GetInstance().GetDataSave().building.commandExp;
	}
	//! 获取司令部是否正在升级
	public bool IsCurCommandUpgrading()
	{
		return DataCenter.GetInstance().GetDataSave().building.commandUpgradeStatus == 1;
	}
	//! 获取司令部升级所需时间
	public int GetCurCommandUpgradeRemainTime()
	{
		return Mathf.Clamp(GetCurCommandUpgradeRemainTimeNoClamp(), 0, int.MaxValue);
	}

	public int GetCurCommandUpgradeRemainTimeNoClamp()
	{
		return DataCenter.GetInstance().GetDataSave().building.commandUpgradeTime - GetServerTime();
	}

	//! 获取补给站项
	public ISupplyConfItem GetSupplyConfItem(int lv)
	{
		return new SupplyConfItem(lv);
	}
	//! 获取补给站项
	public ISupplyConfItem GetSupplyConfItemByCommandLv(int lv)
	{
		ISupplyConfItem confItem = new SupplyConfItem(1);
		while (confItem.GetUpgradeItem().GetCommandLevel() <= lv)
		{
			confItem = confItem.GetUpgradeItem();
			if (confItem.GetLevel() == confItem.GetMaxLevel()|| confItem == confItem.GetUpgradeItem())
			{
				break;
			}
		}
		return confItem;
	}
	//! 获取当前补给站项
	public ISupplyConfItem GetCurSupplyConfItem()
	{
		
		return GetSupplyConfItem(GetCurCommandLevel());
	}
	//! 获取当前补给站等级
	public int GetCurSupplyLevel()
	{
		return GetSupplyConfItemByCommandLv(GetCurCommandLevel()).GetLevel();
		//return DataCenter.GetInstance().GetDataSave().building.supplyLevel;
	}

	//! 获取所有蓝图
	private List<IUnit2ConfItem> m_allBlueprint;
	public List<IUnit2ConfItem> GetAllBlueprint()
	{
		if (m_allBlueprint == null)
		{
			m_allBlueprint = new List<IUnit2ConfItem>();
			List<UnitModelItem> items = Conf.GetInstance().UnitModel.GetItems();
			foreach (UnitModelItem item in items)
			{
				//! Level超过UnitMaxLevel的数据字段并且品质小于0的字段不需要显示
				if (item.Level <= Conf.GetInstance().UnitInfo.FindById(item.UnitId).UnitMaxLevel && Conf.GetInstance().UnitInfo.FindById(item.UnitId).Quality >= 0&& item.Reform == 0&& item.Grade >= 1)
				{
					m_allBlueprint.Add(GetUnit2ConfItem(item.UnitId, item.Grade));
				}
			}
		}
		return m_allBlueprint;
	}

	//! 获取所有科技树节点
	private Dictionary<int, Dictionary<int, IUnit2ConfItem>> m_unitTreeNode;
	public Dictionary<int, Dictionary<int, IUnit2ConfItem>> GetAllUnitTreeNode()
	{
		if (m_unitTreeNode == null)
		{
			m_unitTreeNode = new Dictionary<int, Dictionary<int, IUnit2ConfItem>>();
			List<IUnit2ConfItem> items = GetAllBlueprint();
			foreach (IUnit2ConfItem item in items)
			{
				if (!m_unitTreeNode.ContainsKey(item.GetUnitId()))
				{
					m_unitTreeNode.Add(item.GetUnitId(), new Dictionary<int, IUnit2ConfItem>());
				}
				if (!m_unitTreeNode[item.GetUnitId()].ContainsKey(item.GetGrade()))
				{
					m_unitTreeNode[item.GetUnitId()].Add(item.GetGrade(), item);
				}
			}
		}
		return m_unitTreeNode;
	}


	//! 获取科技树分页
	public List<int> GetUnitTreePage()
	{
		List<int> page = new List<int>();
		List<UnitUITreeItem> items = Conf.GetInstance().UnitUITree.GetItems();
		foreach (UnitUITreeItem item in items)
		{
			page.Add(item.PageId);
		}
		return page;
	}

	//! 获取所有科技树节点
	private Dictionary<int, Dictionary<int, List<IUnit2ConfItem>>> m_unitTreeNodeIndex = new Dictionary<int, Dictionary<int, List<IUnit2ConfItem>>>();
	public Dictionary<int, List<IUnit2ConfItem>> GetUnitTreeNode(int unitTreeId)
	{
		if (!m_unitTreeNodeIndex.ContainsKey(unitTreeId))
		{
			List<int> lines = GetUnitTreeLine(unitTreeId);
			Dictionary<int, List<IUnit2ConfItem>> unitTreeLineIndex = new Dictionary<int, List<IUnit2ConfItem>>();
			foreach (int unitId in lines)
			{
				List<IUnit2ConfItem> items = new List<IUnit2ConfItem>();
				IUnit2ConfItem item = AW.GetSingleton<IDataAdapter>().GetUnit2ConfItem(unitId, 1);
				do
				{
					items.Add(item);
					item = item.GetUpLevelItem();
				}
				while (item != null);
				unitTreeLineIndex.Add(unitId, items);
			}
			m_unitTreeNodeIndex.Add(unitTreeId, unitTreeLineIndex);
		}
		return m_unitTreeNodeIndex[unitTreeId];
	}

	//! 获取科技树分列
	public List<int> GetUnitTreeColumn(int unitTreeId)
	{
		List<int> column = new List<int>();
		for (int i = GetUnitTreeStartLv(unitTreeId); i <= AW.GetSingleton<IDataAdapter>().GetCurCommandConfItem().GetMaxLevel(); i++)
		{
			column.Add(i);
		}
		return column;
	}

	//! 获取科技树分行
	private Dictionary<int, List<int>> m_lineListIndex = new Dictionary<int, List<int>>();
	public List<int> GetUnitTreeLine(int unitTreeId)
	{
		if (!m_lineListIndex.ContainsKey(unitTreeId))
		{
			UnitUITreeItem item = Conf.GetInstance().UnitUITree.IndexPageId.Find(unitTreeId);
			List<int> line = new List<int>();
			foreach(string sign in item.UnitSign)
			{
				line.AddRange(item.GetFieldIntArray(sign));
			}
			m_lineListIndex.Add(unitTreeId, line);
		}
		return m_lineListIndex[unitTreeId];
	}

	//! 获取科技树分页初始等级
	private Dictionary<int, int> m_startLvIndex = new Dictionary<int, int>();
	private int GetUnitTreeStartLv(int unitTreeId)
	{
		if (!m_startLvIndex.ContainsKey(unitTreeId))
		{
			int startlv = int.MaxValue;
			foreach (int unitId in GetUnitTreeLine(unitTreeId))
			{
				IUnit2ConfItem confItem = AW.GetSingleton<IDataAdapter>().GetUnit2ConfItem(unitId, 1);
				startlv = Mathf.Min(startlv, confItem.GetUnlockCommandLevel());
			}
			m_startLvIndex.Add(unitTreeId, startlv);
		}
		return m_startLvIndex[unitTreeId];
	}

	//! 获取科技树分页名称
	public string GetUnitTreePageTag(int unitTreeId)
	{
		return Conf.GetInstance().UnitUITree.IndexPageId.Find(unitTreeId).UITag;
	}

	//! 获取科技树节点位置
	private Dictionary<int, Dictionary<KeyValuePair<int, int>, KeyValuePair<int, int>>> m_unitTreePositionIndex = new Dictionary<int, Dictionary<KeyValuePair<int, int>, KeyValuePair<int, int>>>();
	public KeyValuePair<int, int> GetUnitTreePosition(int unitTreeId, IUnit2ConfItem confitem)
	{
		KeyValuePair<int, int> confitemNode = new KeyValuePair<int, int>(confitem.GetUnitId(), confitem.GetUnlockCommandLevel());
		if (!m_unitTreePositionIndex.ContainsKey(unitTreeId))
		{
			m_unitTreePositionIndex.Add(unitTreeId, new Dictionary<KeyValuePair<int, int>, KeyValuePair<int, int>>());
		}
		if (!m_unitTreePositionIndex[unitTreeId].ContainsKey(confitemNode))
		{
			List<int> columnList = GetUnitTreeColumn(unitTreeId);
			List<int> lineList = GetUnitTreeLine(unitTreeId);
			int key = lineList.FindIndex(delegate(int id) { return confitemNode.Key == id; }) + 1;
			int value = columnList.FindIndex(delegate(int level) { return confitemNode.Value == level; }) + 1;
			m_unitTreePositionIndex[unitTreeId].Add(confitemNode, new KeyValuePair<int, int>(key, value));
		}
		return m_unitTreePositionIndex[unitTreeId][confitemNode];
	}

	//! 获取单位项
	public IUnit2ConfItem GetUnit2ConfItem(int unitId, int grade, int reform = 0, int star = 0)
	{
		return new Unit2ConfItem(unitId, grade, reform, star);
	}
	public IUnit2ConfItem GetPreGradeConfItem(IUnit2ConfItem item)
	{
		Dictionary<int, Dictionary<int, IUnit2ConfItem>> node = GetAllUnitTreeNode();
		if (node.ContainsKey(item.GetUnitId()) && node[item.GetUnitId()] != null && node[item.GetUnitId()].ContainsKey(item.GetGrade() - 1))
		{
			return node[item.GetUnitId()][item.GetGrade() - 1];
		}
		return null;
	}
	public IUnit2ConfItem GetNextGradeConfItem(IUnit2ConfItem item)
	{
		Dictionary<int, Dictionary<int, IUnit2ConfItem>> node = GetAllUnitTreeNode();
		if (node.ContainsKey(item.GetUnitId()) && node[item.GetUnitId()] != null && node[item.GetUnitId()].ContainsKey(item.GetGrade() + 1))
		{
			return node[item.GetUnitId()][item.GetGrade() + 1];
		}
		return null;
	}
	//! 获取强化点数
	public int GetOwnedReformPoint(int unitId)
	{
		return 0;
	}

	//! 获取升级所需资源
	private TriDictionary<int, int, int, IResource> m_upgradeCostIndex = new TriDictionary<int, int, int, IResource>();
	public IResource GetUpgradeCost(int unitId, int startLv, int endLv)
	{
		if (!m_upgradeCostIndex.ContainsKey(unitId, startLv, endLv))
		{
			int cost = 0;
			for (int i = startLv; i <= endLv; i++)
			{
				cost += Conf.GetInstance().UnitCost.Find(unitId, i).UpgradeMoney;
			}
			m_upgradeCostIndex[unitId, startLv, endLv] = new Resource(AWEnum.ResourceType.Steel, cost);
		}
		return m_upgradeCostIndex[unitId, startLv, endLv];
	}

	//! 获取升级所需蓝图
	private TriDictionary<int, int, int, List<IUnit2ConfItem>> m_upgradeBlueprintIndex = new TriDictionary<int, int, int, List<IUnit2ConfItem>>(); 
	public List<IUnit2ConfItem> GetUpgradeBlueprint(int unitId, int startLv, int endLv)
	{
		if (!m_upgradeBlueprintIndex.ContainsKey(unitId, startLv, endLv))
		{
			List<IUnit2ConfItem> confItemList = new List<IUnit2ConfItem>();
			for (int i = startLv; i <= endLv; i++)
			{
					if(Conf.GetInstance().UnitCost.Find(unitId, i).UpgradeBlueprint > 0)
					{
						int grade = Conf.GetInstance().UnitModel.Find(unitId, i).Grade;
						confItemList.Add(GetUnit2ConfItem(unitId, grade));
					}
			}
			m_upgradeBlueprintIndex[unitId, startLv, endLv] = confItemList;
		}
		return m_upgradeBlueprintIndex[unitId, startLv, endLv];
	}
	#endregion

	//! Version2.0
	#region Version2.0
	public IBoxConfItem GetBoxConfItem(int boxId)
	{
		return new BoxConfItem(boxId);
	}

	public List<int> GetBoxList(int level)
	{
		return new List<int>(Conf.GetInstance().BuildingInfoCommand.IndexLevel.Find(level).BoxList);
	}

	public List<int> GetCurBoxList()
	{
		return GetBoxList(DataCenter.GetInstance().GetDataSave().building.commandLevel);
	}

	public string GetPlayerName()
	{
		return DataCenter.GetInstance().GetDataSave().player.name;
	}
	public int GetPlayerCurLevel()
	{
		return DataCenter.GetInstance().GetDataSave().player.level;
	}
	public int GetPlayerMaxLevel()
	{
		return Conf.GetInstance().GlobalValueInt("MaxPlayerLevel");
	}
	public int GetPlayerCurExp()
	{
		return DataCenter.GetInstance().GetDataSave().player.exp;
	}
	public int GetPlayerCost()
	{
		return DataCenter.GetInstance().GetDataSave().GetPlayerCost();
	}
	public int GetPlayerCostMax()
	{
		return DataCenter.GetInstance().GetDataSave().GetPlayerCostMax();
	}
	public int GetPlayerBuyCostPrice()
	{
		return DataCenter.GetInstance().GetDataSave().GetPlayerCostPrice();
	}

	public IPlayerConfItem GetPlayerConfItem(int playerLevel)
	{
		return new PlayerConfItem(playerLevel);
	}
	public IPlayerConfItem GetCurPlayerConfItem()
	{
		return GetPlayerConfItem(GetPlayerCurLevel());
	}

	public IMailConfItem GetMailConfItem(int curCount)
	{
		return new MailConfItem(curCount);
	}

	public int GetMailCount()
	{
		return DataCenter.GetInstance().GetDataMail().Count();
	}


	public List<IAchievementConfItem> GetAchievementList()
	{
		int count = UnityEngine.Random.Range(0, 10);
		List<IAchievementConfItem> items = new List<IAchievementConfItem>();
		for (int i = 0; i < count; i++)
		{
			items.Add(new AchievementConfItem());
		}
		return items;
	}


	public List<ILeaderboardConfItem> GetLeaderboardList()
	{
		List<DataRank.Rank> ranks = DataCenter.GetInstance().GetDataRank().Get();
		List<ILeaderboardConfItem> items = new List<ILeaderboardConfItem>();
		foreach (DataRank.Rank rank in ranks)
		{
			items.Add(new LeaderboardConfItem(rank.name, rank.level, rank.combatPower));
		}
		return items;
	}
	#endregion

	//! Version 1.0
	#region Version1.0
	//! 获取服务器时间
	public int GetServerTime()
	{
		return DataCenter.GetInstance().GetServerTime();
	}
	//! 加载建筑列表
	public List<Building> LoadBuildings()
	{
		//List<Building> buildings = new List<Building>();

		//Dictionary<string, DataSave.Building> buildingIndex = DataCenter.GetInstance().GetDataSave().buildings;
		//foreach (DataSave.Building building in buildingIndex.Values)
		//{
		//	buildings.Add(BuildingFactory.CreateBuilding(building));
		//}
		//return buildings;
		return null;
	}

	//! 加载建筑
	public Building LoadBuilding(string uid)
	{
		//Dictionary<string, DataSave.Building> buildingIndex = DataCenter.GetInstance().GetDataSave().buildings;
		//if (buildingIndex.ContainsKey(uid))
		//{
		//	return BuildingFactory.CreateBuilding(buildingIndex[uid]);
		//}
		//else
		//{
		//	AWOutput.Log("Building uid:" + uid + " not found!");
		//	return null;
		//}

		return null;
	}

	//! 获取建筑项
	public IBuildingConfItem GetBuildingConfItem(Building building)
	{
		return new BuildingConfItem(building.GetBuildingType(), building.ModifyLevel);
	}
	//! 获取建筑项
	public IBuildingConfItem GetBuildingMaxLevelConfItem(int buildingtype)
	{
		return new BuildingConfItem(buildingtype, Conf.GetInstance().Building.IndexBuildingType.Find(buildingtype).MaxLevel);
	}
	//! 获取建造建筑项
	public IBuildingConfItem GetCreateBuildingConfItem(int buildingtype)
	{
		return new BuildingConfItem(buildingtype, 0);
	}

	//! 获取是否还有空闲工人
	public bool GetIdleWorker()
	{
		//List<DataSave.BuildingWorker> workers = DataCenter.GetInstance().GetDataSave().buildingWorkders;
		//foreach (DataSave.BuildingWorker worker in workers)
		//{
		//	if (worker.building == "")
		//	{
		//		return true;
		//	}
		//}
		//return false;

		return false;
	}

	//! 获取当前正在工作建筑
	public List<string> GetCurWorkingBuildingID()
	{
		//List<string> works = new List<string>();
		//List<DataSave.BuildingWorker> workers = DataCenter.GetInstance().GetDataSave().buildingWorkders;
		//foreach (DataSave.BuildingWorker worker in workers)
		//{
		//	if (worker.building != "")
		//	{
		//		works.Add(worker.building);
		//	}
		//}
		//return works;

		return null;
	}

	//! 获取是否还有空闲兵营
	public bool GetIdleCamp(Barrack barrack)
	{
		return barrack.GetProduceTroopDic().Count < GetBuildingConfItem(barrack).GetCampVolume();
	}

	//! 同步建筑
	public void SyncBuilding(Building building)
	{
		//if (DataCenter.GetInstance().GetDataSave().buildings.ContainsKey(building.GetUniqueID()))
		//{
		//	building.Sync(new AWBuildingReader(DataCenter.GetInstance().GetDataSave().buildings[building.GetUniqueID()]));
		//}
		//else if (building is Camp && DataCenter.GetInstance().GetDataSave().troops.ContainsKey(building.GetUniqueID()))
		//{
		//	building.Sync(new TroopBuildingReader(DataCenter.GetInstance().GetDataSave().troops[building.GetUniqueID()]));
		//}
	}

	////! 加载树木列表
	//public List<Tree> LoadTrees()
	//{
	//	List<Tree> trees = new List<Tree>();
	//	HashSet<int> treeSet = DataCenter.GetInstance().GetDataSave().trees;
	//	foreach(int treeID in treeSet)
	//	{
	//		trees.Add(BuildingFactory.CreateTree(treeID));
	//	}
	//	return trees;
	//}

	////! 获取树木项 
	//public ITreeConfItem GetTreeConfItem(string id)
	//{
	//	return new TreeConfItem(id);
	//}

	//! 获取资源
	public IResource GetResource()
	{
		DataSave.Player player = DataCenter.GetInstance().GetDataSave().player;
		return new ResourceGroup(0, player.money, 0, player.medal);
	}
	//! 获取资源容量
	public IResource GetResourceVolume()
	{
		//DataSave.BuildingStorage storage = DataCenter.GetInstance().GetDataSave().buildingStorage;
		//return new ResourceGroup(storage.moneyCapacity, storage.steelCapacity, storage.oilCapacity);
		return null;
	}


	//! 计算水晶公式
	private int CalculateMedalFormula(int x, double a, double b, double k)
	{
		if (x <= 0)
		{
			return 0;
		}
		else
		{
			return (int)System.Math.Floor(System.Math.Max(1, x / (a * System.Math.Floor(System.Math.Pow(x, k)) + b)));
		}
	}
	//! 资源类型参数名映射
	private static Dictionary<int, string> resourceTypeToParamName = new Dictionary<int, string>(){{AWEnum.ResourceType.Money,"Money"},
																																						{AWEnum.ResourceType.Steel,"Steel"},
																																						{AWEnum.ResourceType.Oil,"Oil"}};
	//! 时间类型参数名映射
	private static Dictionary<int, string> timeTypeToParamName = new Dictionary<int, string>(){{AWEnum.ImmediateTimeType.Produce,"UnitTime"},
																																					{AWEnum.ImmediateTimeType.Research,"UnlockTime"},
																																					{AWEnum.ImmediateTimeType.Upgrade,"BuildingTime"}};

	//! 获取资源转换
	//public IResource GetResource(IResource resource, int time, int timeType)
	//{
	//	int total = 0;
	//	if (resource != null)
	//	{
	//		foreach (int type in resource.GetTypeSet())
	//		{
	//			switch (type)
	//			{
	//				case AWEnum.ResourceType.Medal:
	//					total += resource.Get(type);
	//					break;
	//				default:
	//					string resourceParamName = resourceTypeToParamName[type];
	//					double resourceA = Conf.GetInstance().GlobalValueDouble(resourceParamName + "ParamA");
	//					double resourceB = Conf.GetInstance().GlobalValueDouble(resourceParamName + "ParamB");
	//					double resourceK = Conf.GetInstance().GlobalValueDouble(resourceParamName + "ParamK");
	//					total += CalculateMedalFormula(resource.Get(type), resourceA, resourceB, resourceK);
	//					break;
	//			}
	//		}
	//	}

	//	string timeParamName = timeTypeToParamName[timeType];
	//	double timeA = Conf.GetInstance().GlobalValueDouble(timeParamName + "ParamA");
	//	double timeB = Conf.GetInstance().GlobalValueDouble(timeParamName + "ParamB");
	//	double timeK = Conf.GetInstance().GlobalValueDouble(timeParamName + "ParamK");
	//	total += CalculateMedalFormula(UnityEngine.Mathf.CeilToInt(time / 60.0f), timeA, timeB, timeK);

	//	return new Resource(AWEnum.ResourceType.Medal, total);
	//}	public IResource GetResource(IResource resource, int time, int timeType)
	//! 获取资源转换
	public IResource GetResource(IResource resource, int time, int timeType = 0)
	{
		//if (Newbie.GetNewbieStatus() == 14)
		//{
		//	return new Resource(AWEnum.ResourceType.Medal, 0);
		//}
		int total = 0;
		if (resource != null)
		{
			foreach (int type in resource.GetTypeSet())
			{
				int count = resource.Get(type);
				switch (type)
				{
					case AWEnum.ResourceType.Medal:
						total += count;
						break;
					case AWEnum.ResourceType.Steel:
						if (count > 0)
						{
							total += DataSave.CalcMoney2Medal(count);
						}
						break;
					default:
						break;
				}
			}
		}

		if (time > 0)
		{
			switch (timeType)
			{ 
				case AWEnum.ImmediateTimeType.Repair:
					total += DataSave.CalcUnitTime2Medal(time);
 					break;
				case AWEnum.ImmediateTimeType.Upgrade:
					total += DataSave.CalcBuildingTime2Medal(time);
					break;
				default:
					break;
			}
		}

		return new Resource(AWEnum.ResourceType.Medal, total);
	}

	//! 获取拥有兵种
	public List<int> GetOwnedUnitList()
	{
		return null;
	}

	//! 获取训练兵种
	public List<int> GetProduceUnitList(int buildingType)
	{
		//List<int> produceUnitList = new List<int>();
		//int labType = Barrack.GetUnitLabType(buildingType);
		//Dictionary<int, DataSave.Unit> units = DataCenter.GetInstance().GetDataSave().units;
		//foreach (int unlockUnit in units.Keys)
		//{
		//	if (units[unlockUnit].level > 0)
		//	{
		//		IUnitConfItem item = GetUnitConfItem(unlockUnit, units[unlockUnit].level);
		//		if (item.GetLabType() == labType)
		//		{
		//			produceUnitList.Add(unlockUnit);
		//		}
		//	}
		//}
		//return produceUnitList;
		return null;
	}

	//! 获取兵种型号列表
	public List<string> GetUnitModelList(int unitId)
	{
		List<string> modelList = new List<string>();
		IUnitConfItem item = GetOwnedUnitConfItem(unitId);
		for (int i = 1; i <= item.GetMaxLevel(); i++)
		{

			string model = GetUnitConfItem(unitId, i).GetModel();
			if (!modelList.Contains(model))
			{
				modelList.Add(model);
			}
		}
		return modelList;
	}

	//! 获取单位项
	public IUnitConfItem GetUnitConfItem(int unitId, int lv)
	{
		return new UnitConfItem(unitId, lv);
	}

	//! 获取拥有单位项
	public IUnitConfItem GetOwnedUnitConfItem(int unitId)
	{
		return GetUnitConfItem(unitId, GetUnitLevel(unitId));
	}
	//! 获取拥有单位下一级单位项
	public IUnitConfItem GetOwnedUnitNextLevelConfItem(int unitId)
	{
		return GetUnitConfItem(unitId, GetUnitNextLevel(unitId));
	}

	//! 获取当前单位等级
	public int GetUnitLevel(int unitId)
	{
		//if (DataCenter.GetInstance().GetDataSave().units.ContainsKey(unitId))
		//{
		//	return DataCenter.GetInstance().GetDataSave().units[unitId].level;
		//}
		return 0;
	}
	//!获取当前拥有图纸
	public int GetUnitBlueprintCount(int unitId)
	{
		//if (DataCenter.GetInstance().GetDataSave().units.ContainsKey(unitId))
		//{
		//	return DataCenter.GetInstance().GetDataSave().units[unitId].blueprintCount;
		//}
		return 0;
	}
	//! 获取当前单位下一级
	public int GetUnitNextLevel(int unitId)
	{
		return GetUnitLevel(unitId) + 1;
	}

	//!  获取研究所兵种种类项
	public ILabUnitTypeConfItem GetLabUnitTypeConfItem(string unitTypeId)
	{
		return new LabUnitTypeConfItem(unitTypeId);
	}

	//! 获取模型项
	public IUnitModelConfItem GetUnitModelConfItem(int unitId, string model)
	{
		return new UnitModelConfItem(unitId, model);
	}

	//! 获取当前图纸数量
	public int GetCurBlueprintCount(string model)
	{
		//if(DataCenter.GetInstance().GetDataSave().blueprints.ContainsKey(model))
		//{
		//	return DataCenter.GetInstance().GetDataSave().blueprints[model];
		//}
		//else
		//{
		//	return 0;
		//}

		return 0;
	}

	////! 2是步兵 14是掷弹兵
	//public bool IsInfantry(int unitId)
	//{
	//	return unitId == 2 || unitId == 14;
	//}


	//! 获取持有零件列表
	public List<int> GetKeepGearList()
	{
		return null;
		//return new List<int>(Global.FilterSameElement<int>(DataCenter.GetInstance().GetDataSave().gears).Keys);
	}

	//! 获取生产零件列表
	public List<int> GetCreateGearList()
	{
		List<int> gearList = new List<int>();
		List<GearItem> itemList = Conf.GetInstance().Gear.GetItems();
		foreach (GearItem item in itemList)
		{
			gearList.Add(item.GearId);
		}
		return gearList;
	}

	//! 获取零件项
	public IGearConfItem GetGearConfItem(int gearId)
	{
		return new GearConfItem(gearId);
	}

	//! 获取兵种可装备零件列表
	public List<int> GetEquipableGearList(int unitType)
	{
		List<int> equipableGearList = new List<int>();
		List<int> keepList = GetKeepGearList();
		foreach (int keep in keepList)
		{
			if (this.GetGearConfItem(keep).GetNeedUnitType() == unitType)
			{
				equipableGearList.Add(keep);
			}
		}
		return equipableGearList;
	}

	//! 获取免费刷新间谍剩余时间
	public int GetFreeSpyRefreshRemainTime()
	{
		int hourTime = 60 * 60;
		int dayTime = 24 * hourTime;
		int now = GetServerTime();
		int offset = Conf.GetInstance().GlobalValueInt("RefreshOffsetSpy") * hourTime;
		int timezone = DataCenter.GetInstance().GetDataSave().player.timezone * hourTime;
		int lastSpyRefreshTime = DataCenter.GetInstance().GetDataSave().player.spyRefreshTime;
		int n = ((now - timezone) - (lastSpyRefreshTime + offset)) / dayTime;
		if (n >= 1)
		{
			return 0;
		}
		else
		{
			return (lastSpyRefreshTime + offset) - (now - timezone) + dayTime;
		}
	}
	//! 获取间谍刷新需要资源
	public IResource GetSpyRefreshNeedResource(int refreshType)
	{
		switch (refreshType)
		{
			case AWEnum.SpyRefreshType.Free:
				return new Resource(AWEnum.ResourceType.Medal, 0);
			case AWEnum.SpyRefreshType.Normal:
				return new Resource(AWEnum.ResourceType.Medal, Conf.GetInstance().GlobalValueInt("CostMedalRefreshSpyA"));
			case AWEnum.SpyRefreshType.Advance:
				return new Resource(AWEnum.ResourceType.Medal, Conf.GetInstance().GlobalValueInt("CostMedalRefreshSpyB"));
			default:
				return null;
		}
	}
	//! 获取持有间谍列表
	public List<string> GetKeepSpyList()
	{
		return null;
		//return new List<string>(DataCenter.GetInstance().GetDataSave().spies.Keys);
	}
	//! 获取可雇佣间谍列表
	public List<string> GetRecruitSpyList()
	{
		return null;
		//return new List<string>(DataCenter.GetInstance().GetDataSave().spiesStore.Keys);
	}

	//! 获取间谍项
	public ISpyConfItem GetSpyConfItem(string spyId)
	{
		return new SpyConfItem(spyId);
	}

	//! 获取当前间谍栏位
	public int GetCurSpySolt(Building building)
	{
		return GetBuildingConfItem(building).GetSpyVolume() + DataCenter.GetInstance().GetDataSave().player.spySlot;
	}

	//! 获取是否可以购买间谍栏位
	public bool GetCanBuySpySolt()
	{
		SpySlotItem item = Conf.GetInstance().SpySlot.IndexSlot.Find(DataCenter.GetInstance().GetDataSave().player.spySlot + 1);
		return (item != null);
	}
	//! 获取购买间谍槽位价格
	public IResource GetCurSpySoltPrice()
	{
		SpySlotItem item = Conf.GetInstance().SpySlot.IndexSlot.Find(DataCenter.GetInstance().GetDataSave().player.spySlot + 1);
		return new Resource(AWEnum.ResourceType.Medal, item.CostMedal);
	}

	//! 获取防御任务列表
	public List<int> GetShelterTaskList()
	{
		return new List<int>(DataCenter.GetInstance().GetDataSave().shelterTasks.Keys);
	}

	//! 获取当前防御任务
	public int GetCurShelterTask()
	{
		return DataCenter.GetInstance().GetDataSave().shelterTaskCurrent;
	}

	//! 获取防御任务项
	public IShelterConfItem GetShelterConfItem(int taskId)
	{
		return new ShelterConfItem(taskId);
	}

	//! 获取战报列表
	public List<string> GetReportList()
	{
		List<string> reportList = new List<string>();
		foreach (DataSave.PvPLog log in DataCenter.GetInstance().GetDataSave().pvpLogs)
		{
			reportList.Add(log.logId);
		}
		return reportList;
	}

	//! 获取战报项
	public IReportConfItem GetReportConfItem(string id)
	{
		return new ReportConfItem(id);
	}
	#endregion
}
