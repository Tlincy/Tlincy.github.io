using System.Collections;
using System.Collections.Generic;
using Triniti.AW.UI.Pattern;

//! 数据适配器接口
public interface IDataAdapter
{
	//! Version3.0
	#region Story
	//! 获取当前任务故事项
	List<UIStoryItem> GetTaskUIStoryItem();
	//! 获取当前故事项
	List<UIStoryItem> GetStoryUIStoryItem(int progress, int progressSub);
	#endregion

	#region IAP
	//! 获取IAPMedal
	List<IAPMedalItem> GetIAPMedal();
	//! 获取IAPMoney
	List<IAPMoneyItem> GetIAPMoney();
	#endregion

	#region Resource
	//! 获取当前拥有升星经验
	int GetStarExp(int unitId);
	//! 获取最大星级
	int GetStarMaxLevel();
	#endregion

	#region Troop
	//! 获取兵种项
	IUnit2ConfItem GetUnit2ConfItem(string troopId);
	//! 获取当前兵种序列
	List<string> GetCurTroopList();
	//! 获取部队空地ID
	int GetTroopSlot(string troopId);
	//! 获取维修时间
	int GetTroopRepairTime(string troopId);
	//! 是否需要维修
	bool CanTroopRepair(string troopId);
	//! 获取部队名字
	string GetTroopName(string troopId);
	//! 获取部队统计属性索引
	Dictionary<string, string> GetTroopExploit(string troopId);
	#endregion

	#region Defence
	//! 获取当前防御工事序列
	List<int> GetCurDefenceList();
	//! 获取防御工事项
	IDefenceConfItem GetDefenceConfItem(int defenceId, int reform = 0);
	//! 获取当前防御工事项
	IDefenceConfItem GetCurDefenceConfItem(int defenceId);
	#endregion

	#region Cost
	//! 获取当前使用人口
	int GetUsedTroopCost();
	//! 获取剩余人口
	int GetEmptyTroopCost();
	#endregion

	#region Slot
	//! 获取是否还有空地使用
	bool IsHaveEmptySlot(int slotType);
	//! 获取当前可用空地
	int GetFirstEmptySlot(int slotType);
	//! 获取使用空地列表
	List<int> GetUsedSlotList(int slotType);
	//! 获取新增空地列表
	List<int> GetNewSlot(int slotType);
	#endregion

	#region Factory
	//! 获取当前工厂
	List<int> GetCurFactory();
	//! 获取当前可以购买的工厂
	List<int> GetCanBuyFactory();
	//! 获取工厂冷却
	int GetFactoryCD(int factoryId);
	//! 是否工厂可以维修
	bool CanFactoryRepair();

	#endregion

	#region Command
	//! 获取司令部项
	ICommandConfItem GetCommandConfItem(int lv);
	//! 获取当前司令部项
	ICommandConfItem GetCurCommandConfItem();
	//! 获取当前司令部等级
	int GetCurCommandLevel();
	//! 获取当前司令部经验
	int GetCurCommandExp();
	//! 获取司令部是否正在升级
	bool IsCurCommandUpgrading();
	//! 获取司令部升级所需时间
	int GetCurCommandUpgradeRemainTime();
	//! 获取司令部升级剩余时间
	int GetCurCommandUpgradeRemainTimeNoClamp();
	#endregion

	#region Supply
	//! 获取补给站项
	ISupplyConfItem GetSupplyConfItem(int lv);
	//! 获取补给站项by司令部等级
	ISupplyConfItem GetSupplyConfItemByCommandLv(int lv);
	//! 获取当前补给站项
	ISupplyConfItem GetCurSupplyConfItem();
	//! 获取当前补给站等级
	int GetCurSupplyLevel();
	#endregion

	#region UnitTree
	//! 获取所有蓝图
	List<IUnit2ConfItem> GetAllBlueprint();
	//! 获取所有科技树节点
	Dictionary<int, Dictionary<int, IUnit2ConfItem>> GetAllUnitTreeNode();
	//! 获取科技树分页
	List<int> GetUnitTreePage();
	//! 获取所有科技树节点
	Dictionary<int, List<IUnit2ConfItem>> GetUnitTreeNode(int unitTreeId);
	//! 获取科技树分列
	List<int> GetUnitTreeColumn(int unitTreeId);
	//! 获取科技树分行
	List<int> GetUnitTreeLine(int unitTreeId);
	//! 获取科技树分页名称
	string GetUnitTreePageTag(int unitTreeId);
	//! 获取科技树节点位置
	KeyValuePair<int, int> GetUnitTreePosition(int unitTreeId, IUnit2ConfItem confItem);
	#endregion

	#region Unit2
	//! 获取单位项
	IUnit2ConfItem GetUnit2ConfItem(int unitId, int grade, int reform = 0, int star = 0);
	//! 获取前阶单位项
	IUnit2ConfItem GetPreGradeConfItem(IUnit2ConfItem item);
	//! 获取后阶单位项
	IUnit2ConfItem GetNextGradeConfItem(IUnit2ConfItem item);
	//! 获取拥有的强化点数
	int GetOwnedReformPoint(int unitId);
	//! 获取升级所需资源
	IResource GetUpgradeCost(int unitId, int startLv, int endLv);
	//! 获取升级所需蓝图
	List<IUnit2ConfItem> GetUpgradeBlueprint(int unitId, int startLv, int endLv);
	#endregion

	//! Version2.0

	#region Box
	//! 获取箱子项
	IBoxConfItem GetBoxConfItem(int boxId);
	//! 获取箱子列表
	List<int> GetBoxList(int level);
	//! 获取当前箱子列表
	List<int> GetCurBoxList();
	
	#endregion

	#region Player
	string GetPlayerName();
	int GetPlayerCurLevel();
	int GetPlayerMaxLevel();
	int GetPlayerCurExp();
	int GetPlayerCost();
	int GetPlayerCostMax();
	int GetPlayerBuyCostPrice();
	IPlayerConfItem GetPlayerConfItem(int playerLevel);
	IPlayerConfItem GetCurPlayerConfItem();
	#endregion

	#region Mail
	IMailConfItem GetMailConfItem(int count);
	int GetMailCount();
	#endregion

	#region Achievement
	List<IAchievementConfItem> GetAchievementList();
	#endregion

	#region Leaderboard
	List<ILeaderboardConfItem> GetLeaderboardList();
	#endregion

	//! Version1.0

	#region Server
	//! 获取服务器时间
	int GetServerTime();
	#endregion

	#region Building
	//! 加载建筑列表
	List<Building> LoadBuildings();
	//! 加载建筑
	Building LoadBuilding(string uid);
	//! 获取建筑项
	IBuildingConfItem GetBuildingConfItem(Building building);
	//! 获取建筑项
	IBuildingConfItem GetBuildingMaxLevelConfItem(int buildingtype);
	//! 获取建造建筑项
	IBuildingConfItem GetCreateBuildingConfItem(int buildingtype);
	//! 获取是否还有空闲工人
	bool GetIdleWorker();
	//! 获取当前正在工作的建筑
	List<string> GetCurWorkingBuildingID(); 
	//! 获取是否还有空闲兵营
	bool GetIdleCamp(Barrack barrack);
	//! 同步建筑
	void SyncBuilding(Building building);
	#endregion

	#region Tree
	////! 获取树木列表
	//List<Tree> LoadTrees();
	////! 获取树木项
	//ITreeConfItem GetTreeConfItem(string id);
	#endregion

	#region Resource
	//! 获取资源
	IResource GetResource();
	//! 获取资源总量
	IResource GetResourceVolume();
	//! 获取资源转换
	IResource GetResource(IResource resource, int time, int timeType = 0);
	#endregion

	#region Unit
	//! 获取拥有兵种
	List<int> GetOwnedUnitList();
	//! 获取训练兵种
	List<int> GetProduceUnitList(int buildingType);
	//! 获取兵种型号列表
	List<string> GetUnitModelList(int unitId);
	//! 获取单位项
	IUnitConfItem GetUnitConfItem(int unitId, int lv);
	//! 获取拥有单位项
	IUnitConfItem GetOwnedUnitConfItem(int unitId);
	//! 获取拥有单位下一级单位项
	IUnitConfItem GetOwnedUnitNextLevelConfItem(int unitId);
     //!获取当前拥有图纸
    int GetUnitBlueprintCount(int unitId);
	//! 获取当前单位等级
	int GetUnitLevel(int unitId);
	//! 获取单位下一级
	int GetUnitNextLevel(int unitId);
	#endregion

	#region Lab
	//!  获取研究所兵种种类项
	ILabUnitTypeConfItem GetLabUnitTypeConfItem(string unitTypeId);
	//! 获取模型项
	IUnitModelConfItem GetUnitModelConfItem(int unitId, string model);
	//! 获取当前图纸数量
	int GetCurBlueprintCount(string model);
	////! 是否是步兵
	//bool IsInfantry(int unitId);

	#endregion

	#region Gear
	//! 获取持有零件列表
	List<int> GetKeepGearList();
	//! 获取生产零件列表
	List<int> GetCreateGearList();
	//! 获取零件项
	IGearConfItem GetGearConfItem(int gearId);
	//! 获取兵种可装备零件列表
	List<int> GetEquipableGearList(int unitType);
	#endregion

	#region Spy
	//! 获取免费刷新间谍剩余时间
	int GetFreeSpyRefreshRemainTime();
	//! 获取间谍刷新需要资源
	IResource GetSpyRefreshNeedResource(int refreshType);
	//! 获取持有间谍列表
	List<string> GetKeepSpyList();
	//! 获取可雇佣间谍列表
	List<string> GetRecruitSpyList();
	//! 获取间谍项
	ISpyConfItem GetSpyConfItem(string spyId);
	//! 获取当前间谍栏位
	int GetCurSpySolt(Building building);
	//! 获取是否可以购买间谍栏位
	bool GetCanBuySpySolt();
	//! 获取购买间谍槽位价格
	IResource GetCurSpySoltPrice();
	#endregion

	#region Task
	//! 获取防御任务列表
	List<int> GetShelterTaskList();
	//! 获取当前防御任务
	int GetCurShelterTask();
	//! 获取防御任务项
	IShelterConfItem GetShelterConfItem(int taskId);
	#endregion

	#region Report
	//! 获取战报列表
	List<string> GetReportList();
	//! 获取战报项
	IReportConfItem GetReportConfItem(string id);
	#endregion
}
