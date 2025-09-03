using System.Collections.Generic;
using ZeldaOverworldRandomizer.MapBuilder;

namespace ZeldaOverworldRandomizer.GameData {
	public static class Game {
		public static int TimesRebuilt;
		public static readonly List<Region> Regions = new List<Region>();
		public static readonly List<Screen> Screens = new List<Screen>();
		public static readonly List<List<Column>> ScreenLayouts = new List<List<Column>>();

		public const int ScreensWide = 16;
		public const int ScreensHigh = 8;
		public const int TilesWide = 16;
		public const int TilesHigh = 11;

		public static int LastScreenRow => ScreensHigh - 1;
		public static int LastScreenColumn => ScreensWide - 1;
		public static int LastTileRow => TilesHigh - 1;
		public static int LastTileColumn => TilesWide - 1;

		public static int LinkVerticalPosition = 7;
		public static int LinkStartScreen = 77;

		public static Screen NormalCaveScreen;
		public static Screen AnyRoadCaveInteriorScreen;
		public static readonly List<Screen> AnyRoadCaveScreens = new List<Screen>();

		public static int OverworldItemPositionX = 0;
		public static int OverworldItemPositionY = 0;
		public static int OverworldItemScreenId = 0;
		public static readonly List<int> OverworldLadderScreens = new List<int>();

		public static List<int> OverworldWhistleSecretScreens = new List<int> {
			128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128
		};

		public static List<int> WarpWhistleDestinations = new List<int> {0, 0, 0, 0, 0, 0, 0, 0};

		public static readonly List<int> SpecialDockScreens = new List<int> {85};
		public const int LostScreenA = 128;
		public const int LostScreenB = 128;

		public static readonly List<int> ArmosCaveScreens = new List<int>();
		public static readonly List<int> ArmosCaveScreenPositionXs = new List<int>();
		public static int ArmosSecretPositionY = 4;
		public static int ArmosHiddenItemScreen = 0;
		public static int ArmosHiddenItemPositionX = 0;

		public const int TotalCavesShopAOpen = 4;
		public const int TotalCavesShopAHidden = 0;

		public const int TotalCavesShopBOpen = 3;
		public const int TotalCavesShopBHidden = 0;

		public const int TotalCavesShopCOpen = 0;
		public const int TotalCavesShopCHidden = 4;

		public const int TotalCavesPotionShopsOpen = 2;
		public const int TotalCavesPotionShopsHidden = 4;

		public const int TotalCavesRupeesTenOpen = 0;
		public const int TotalCavesRupeesTenHidden = 5;

		public const int TotalCavesRupeesThirtyOpen = 0;
		public const int TotalCavesRupeesThirtyHidden = 7;

		public const int TotalCavesRupeesHundredOpen = 0;
		public const int TotalCavesRupeesHundredHidden = 3;

		public const int TotalCavesMoneyGameOpen = 1;
		public const int TotalCavesMoneyGameHidden = 4;

		public const int TotalCavesTaxOpen = 0;
		public const int TotalCavesTaxHidden = 9;

		public const int TotalCavesHeartsOpen = 0;
		public const int TotalCavesHeartsHidden = 4;

		public static readonly Dictionary<CaveType, int> CaveLookup = new Dictionary<CaveType, int> {
			{CaveType.None, 0},
			{CaveType.Dungeon1, 1},
			{CaveType.Dungeon2, 2},
			{CaveType.Dungeon3, 3},
			{CaveType.Dungeon4, 4},
			{CaveType.Dungeon5, 5},
			{CaveType.Dungeon6, 6},
			{CaveType.Dungeon7, 7},
			{CaveType.Dungeon8, 8},
			{CaveType.Dungeon9, 9},
			{CaveType.WoodSword, 16},
			{CaveType.HeartContainer, 17},
			{CaveType.WhiteSword, 18},
			{CaveType.MasterSword, 19},
			{CaveType.AnyRoad, 20},
			{CaveType.HintSecretInTreeAtDeadEnd, 21},
			{CaveType.MoneyGame, 22},
			{CaveType.Tax, 23},
			{CaveType.Letter, 24},
			{CaveType.HintMeetOldManAtGrave, 25},
			{CaveType.PotionShop, 26},
			{CaveType.PayHint1, 27},
			{CaveType.PayHint2, 28},
			{CaveType.ShopA, 29},
			{CaveType.ShopB, 30},
			{CaveType.ShopC, 31},
			{CaveType.BlueRingShop, 32},
			{CaveType.RupeesThirty, 33},
			{CaveType.RupeesHundred, 34},
			{CaveType.RupeesTen, 35}
		};

		public static readonly Dictionary<int, CaveType> CaveLookupById = new Dictionary<int, CaveType> {
			{0, CaveType.None},
			{1, CaveType.Dungeon1},
			{2, CaveType.Dungeon2},
			{3, CaveType.Dungeon3},
			{4, CaveType.Dungeon4},
			{5, CaveType.Dungeon5},
			{6, CaveType.Dungeon6},
			{7, CaveType.Dungeon7},
			{8, CaveType.Dungeon8},
			{9, CaveType.Dungeon9},
			{16, CaveType.WoodSword},
			{17, CaveType.HeartContainer},
			{18, CaveType.WhiteSword},
			{19, CaveType.MasterSword},
			{20, CaveType.AnyRoad},
			{21, CaveType.HintSecretInTreeAtDeadEnd},
			{22, CaveType.MoneyGame},
			{23, CaveType.Tax},
			{24, CaveType.Letter},
			{25, CaveType.HintMeetOldManAtGrave},
			{26, CaveType.PotionShop},
			{27, CaveType.PayHint1},
			{28, CaveType.PayHint2},
			{29, CaveType.ShopA},
			{30, CaveType.ShopB},
			{31, CaveType.ShopC},
			{32, CaveType.BlueRingShop},
			{33, CaveType.RupeesThirty},
			{34, CaveType.RupeesHundred},
			{35, CaveType.RupeesTen}
		};

		public static readonly List<CaveType> DungeonCaves = new List<CaveType> {
			CaveType.Dungeon1,
			CaveType.Dungeon2,
			CaveType.Dungeon3,
			CaveType.Dungeon4,
			CaveType.Dungeon5,
			CaveType.Dungeon6,
			CaveType.Dungeon7,
			CaveType.Dungeon8,
			CaveType.Dungeon9
		};

		public static readonly Dictionary<TileType, int> TileLookup = new Dictionary<TileType, int> {
			{TileType.CaveAlt, 0},
			{TileType.KakarikoHouseTopMiddle, 3},
			{TileType.KakarikoHouseBottomMiddle, 4},
			{TileType.Water, 5},
			{TileType.ShoreTop, 6},
			{TileType.ShoreBottom, 7},
			{TileType.ShoreLeft, 8},
			{TileType.ShoreRight, 9},
			{TileType.Ladder, 10},
			{TileType.Bridge, 11},
			{TileType.Cave, 12},
			{TileType.BlackFloor, 13},
			{TileType.Ground, 14},
			{TileType.WaterfallCave, 15},
			{TileType.KakarikoHouseFront, 16},
			{TileType.KakarikoPost, 17},
			{TileType.StaircaseDown, 18},
			{TileType.Boulder, 19},
			{TileType.Grave, 20},
			{TileType.ShoreTopLeft, 21},
			{TileType.ShoreTopRight, 22},
			{TileType.ShoreBottomLeft, 23},
			{TileType.ShoreBottomRight, 24},
			{TileType.Tree, 25},
			{TileType.RockTop, 26},
			{TileType.Rock, 27},
			{TileType.BigTreeTopLeft, 28},
			{TileType.BigTreeTopMiddle, 29},
			{TileType.BigTreeTopRight, 30},
			{TileType.BigTreeBottomLeft, 31},
			{TileType.BigTreeBottomRight, 32},
			{TileType.DungeonMouthTopLeft, 33},
			{TileType.DungeonMouthTopMiddle, 34},
			{TileType.DungeonMouthTopRight, 35},
			{TileType.DungeonMouthBottomLeft, 36},
			{TileType.DungeonMouthBottomRight, 37},
			{TileType.BoulderPushable, 38},
			{TileType.RockBombWall, 39},
			{TileType.TreeBurnable, 40},
			{TileType.GravePushable, 41},
			{TileType.KakarikoHouseTopLeft, 42},
			{TileType.KakarikoHouseTopRight, 43},
			{TileType.Armos, 44},
			{TileType.DungeonMouthTopMiddleAlt, 45},
			{TileType.WaterTopLeftInsideCorner, 46},
			{TileType.WaterTopRightInsideCorner, 47},
			{TileType.WaterBottomLeftInsideCorner, 48},
			{TileType.WaterBottomRightInsideCorner, 49},
			{TileType.RockTopLeft, 50},
			{TileType.RockTopRight, 51},
			{TileType.RockBottomLeft, 52},
			{TileType.RockBottomRight, 53},
			{TileType.Waterfall, 54},
			{TileType.Desert, 55},
			{TileType.KakarikoHouseBottomLeft, 57},
			{TileType.KakarikoHouseBottomRight, 59},
			{TileType.KakarikoHouseDoor, 60},
			{TileType.BridgeVertical, 61},
			{TileType.PalmTree, 62},
			{TileType.Debug, 56}
		};

		public static readonly Dictionary<int, TileType> TileLookupById = new Dictionary<int, TileType> {
			{0, TileType.CaveAlt},
			{1, TileType.Debug},
			{2, TileType.Debug},
			{3, TileType.KakarikoHouseTopMiddle},
			{4, TileType.KakarikoHouseBottomMiddle},
			{5, TileType.Water},
			{6, TileType.ShoreTop},
			{7, TileType.ShoreBottom},
			{8, TileType.ShoreLeft},
			{9, TileType.ShoreRight},
			{10, TileType.Ladder},
			{11, TileType.Bridge},
			{12, TileType.Cave},
			{13, TileType.BlackFloor},
			{14, TileType.Ground},
			{15, TileType.WaterfallCave},
			{16, TileType.KakarikoHouseFront},
			{17, TileType.KakarikoPost},
			{18, TileType.StaircaseDown},
			{19, TileType.Boulder},
			{20, TileType.Grave},
			{21, TileType.ShoreTopLeft},
			{22, TileType.ShoreTopRight},
			{23, TileType.ShoreBottomLeft},
			{24, TileType.ShoreBottomRight},
			{25, TileType.Tree},
			{26, TileType.RockTop},
			{27, TileType.Rock},
			{28, TileType.BigTreeTopLeft},
			{29, TileType.BigTreeTopMiddle},
			{30, TileType.BigTreeTopRight},
			{31, TileType.BigTreeBottomLeft},
			{32, TileType.BigTreeBottomRight},
			{33, TileType.DungeonMouthTopLeft},
			{34, TileType.DungeonMouthTopMiddle},
			{35, TileType.DungeonMouthTopRight},
			{36, TileType.DungeonMouthBottomLeft},
			{37, TileType.DungeonMouthBottomRight},
			{38, TileType.BoulderPushable},
			{39, TileType.RockBombWall},
			{40, TileType.TreeBurnable},
			{41, TileType.GravePushable},
			{42, TileType.KakarikoHouseTopLeft},
			{43, TileType.KakarikoHouseTopRight},
			{44, TileType.Armos},
			{45, TileType.DungeonMouthTopMiddleAlt},
			{46, TileType.WaterTopLeftInsideCorner},
			{47, TileType.WaterTopRightInsideCorner},
			{48, TileType.WaterBottomLeftInsideCorner},
			{49, TileType.WaterBottomRightInsideCorner},
			{50, TileType.RockTopLeft},
			{51, TileType.RockTopRight},
			{52, TileType.RockBottomLeft},
			{53, TileType.RockBottomRight},
			{54, TileType.Waterfall},
			{55, TileType.Desert},
			{56, TileType.Debug},
			{57, TileType.KakarikoHouseBottomLeft},
			{58, TileType.Debug},
			{59, TileType.KakarikoHouseBottomRight},
			{60, TileType.KakarikoHouseDoor},
			{61, TileType.BridgeVertical},
			{62, TileType.PalmTree},
			{63, TileType.Debug}
		};

		public static readonly Dictionary<EnemyCount, int> EnemyCountLookup = new Dictionary<EnemyCount, int> {
			{EnemyCount.One, 0},
			{EnemyCount.Four, 1},
			{EnemyCount.Five, 2},
			{EnemyCount.Six, 3}
		};

		public static readonly Dictionary<int, EnemyCount> EnemyCountLookupByValue = new Dictionary<int, EnemyCount> {
			{0, EnemyCount.One},
			{1, EnemyCount.Four},
			{2, EnemyCount.Five},
			{3, EnemyCount.Six}
		};

		public static readonly Dictionary<SingleEnemyTypes, int> SingleEnemyTypeLookup =
			new Dictionary<SingleEnemyTypes, int> {
				{SingleEnemyTypes.None, 0},
				{SingleEnemyTypes.LynelBlue, 1},
				{SingleEnemyTypes.LynelRed, 2},
				{SingleEnemyTypes.MoblinBlue, 3},
				{SingleEnemyTypes.MoblinRed, 4},
				{SingleEnemyTypes.OctorokRed, 7},
				{SingleEnemyTypes.OctorokRedFast, 8},
				{SingleEnemyTypes.OctorokBlue, 9},
				{SingleEnemyTypes.OctorokBlueFast, 10},
				{SingleEnemyTypes.TektiteBlue, 13},
				{SingleEnemyTypes.TektiteRed, 14},
				{SingleEnemyTypes.LeeverBlue, 15},
				{SingleEnemyTypes.LeeverRed, 16},
				{SingleEnemyTypes.Peahat, 26},
				{SingleEnemyTypes.FallingRocksGenerator, 31},
				{SingleEnemyTypes.GhiniMaster, 33},
				{SingleEnemyTypes.GhiniMinion, 34}
			};

		public static readonly Dictionary<int, SingleEnemyTypes> SingleEnemyTypeLookupById =
			new Dictionary<int, SingleEnemyTypes> {
				{0, SingleEnemyTypes.None},
				{1, SingleEnemyTypes.LynelBlue},
				{2, SingleEnemyTypes.LynelRed},
				{3, SingleEnemyTypes.MoblinBlue},
				{4, SingleEnemyTypes.MoblinRed},
				{7, SingleEnemyTypes.OctorokRed},
				{8, SingleEnemyTypes.OctorokRedFast},
				{9, SingleEnemyTypes.OctorokBlue},
				{10, SingleEnemyTypes.OctorokBlueFast},
				{13, SingleEnemyTypes.TektiteBlue},
				{14, SingleEnemyTypes.TektiteRed},
				{15, SingleEnemyTypes.LeeverBlue},
				{16, SingleEnemyTypes.LeeverRed},
				{26, SingleEnemyTypes.Peahat},
				{31, SingleEnemyTypes.FallingRocksGenerator},
				{33, SingleEnemyTypes.GhiniMaster},
				{34, SingleEnemyTypes.GhiniMinion}
			};

		public static readonly Dictionary<MixedEnemyTypes, int> MixedEnemyTypeLookup =
			new Dictionary<MixedEnemyTypes, int> {
				{MixedEnemyTypes.TwinMoldorm, 1},
				{MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_MoblinBlue, 34},
				{MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_Peahat_Peahat, 35},
				{MixedEnemyTypes.Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue, 36},
				{MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LynelRed_LynelBlue_LeeverBlue, 37},
				{MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None, 38},
				{MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat, 39},
				{MixedEnemyTypes.OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast, 40},
				{MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue, 41},
				{MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoBlue_OctoBlue_OctoRedFast_OctoBlueFast, 42},
				{MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue, 43}
			};

		public static readonly Dictionary<int, MixedEnemyTypes> MixedEnemyTypeLookupById =
			new Dictionary<int, MixedEnemyTypes> {
				{1, MixedEnemyTypes.TwinMoldorm},
				{34, MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_MoblinBlue},
				{35, MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_Peahat_Peahat},
				{36, MixedEnemyTypes.Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue},
				{37, MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LynelRed_LynelBlue_LeeverBlue},
				{38, MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None},
				{39, MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat},
				{40, MixedEnemyTypes.OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast},
				{41, MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue},
				{42, MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoBlue_OctoBlue_OctoRedFast_OctoBlueFast},
				{43, MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue}
			};

		public static void ResetProperties() {
			Regions.Clear();
			OverworldLadderScreens.Clear();
			ArmosCaveScreens.Clear();
			ArmosCaveScreenPositionXs.Clear();
			AnyRoadCaveScreens.Clear();
			OverworldWhistleSecretScreens = new List<int> {128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128};
			WarpWhistleDestinations = new List<int> {0, 0, 0, 0, 0, 0, 0, 0};
		}
	}
}