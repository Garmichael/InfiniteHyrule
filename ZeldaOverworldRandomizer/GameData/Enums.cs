namespace ZeldaOverworldRandomizer.GameData {
	public enum TerrainType {
		Forest,
		Mountain
	}

	public enum EnvironmentColor {
		Green,
		Brown,
		Grey,
	}

	public enum NineSliceSpot {
		TopLeft,
		Left,
		BottomLeft,
		Top,
		Middle,
		Bottom,
		TopRight,
		Right,
		BottomRight,
		None
	}

	public enum Direction {
		Left,
		Up,
		Right,
		Down
	}

	public enum TileType {
		CaveAlt,
		Water,
		ShoreTop,
		ShoreBottom,
		ShoreLeft,
		ShoreRight,
		Ladder,
		Bridge,
		BridgeVertical,
		Cave,
		BlackFloor,
		Ground,
		WaterfallCave,
		StaircaseDown,
		Boulder,
		Grave,
		ShoreTopLeft,
		ShoreTopRight,
		ShoreBottomLeft,
		ShoreBottomRight,
		Tree,
		RockTop,
		Rock,
		BigTreeTopLeft,
		BigTreeTopMiddle,
		BigTreeBottomRight,
		BigTreeBottomLeft,
		BigTreeTopRight,
		DungeonMouthTopLeft,
		DungeonMouthTopMiddle,
		DungeonMouthTopRight,
		DungeonMouthBottomLeft,
		DungeonMouthBottomRight,
		BoulderPushable,
		RockBombWall,
		TreeBurnable,
		GravePushable,
		Armos,
		DungeonMouthTopMiddleAlt,
		WaterTopLeftInsideCorner,
		WaterTopRightInsideCorner,
		WaterBottomLeftInsideCorner,
		WaterBottomRightInsideCorner,
		RockTopLeft,
		RockTopRight,
		RockBottomLeft,
		RockBottomRight,
		Waterfall,
		Desert,
		KakarikoHouseFront,
		KakarikoPost,
		KakarikoHouseTopLeft,
		KakarikoHouseTopMiddle,
		KakarikoHouseTopRight,
		KakarikoHouseBottomLeft,
		KakarikoHouseBottomMiddle,
		KakarikoHouseBottomRight,
		KakarikoHouseDoor,
		PalmTree,
		Debug
	}

	public enum Biome {
		StartZone,
		RockyBeach,
		Tunnel,
		DenseForest,
		LightForest,
		GhostForest,
		MountainRange,
		Graveyard,
		Kakariko,
		Desert,
		Island,
		River,
		Debug
	}

	public enum CaveType {
		None,
		Dungeon1,
		Dungeon2,
		Dungeon3,
		Dungeon4,
		Dungeon5,
		Dungeon6,
		Dungeon7,
		Dungeon8,
		Dungeon9,
		WoodSword,
		HeartContainer,
		WhiteSword,
		MasterSword,
		AnyRoad,
		HintSecretInTreeAtDeadEnd,
		MoneyGame,
		Tax,
		Letter,
		HintMeetOldManAtGrave,
		PotionShop,
		PayHint1,
		PayHint2,
		ShopA,
		ShopB,
		ShopC,
		BlueRingShop,
		RupeesThirty,
		RupeesHundred,
		RupeesTen
	}

	public enum EnemyCount {
		One,
		Four,
		Five,
		Six
	}

	public enum SingleEnemyTypes {
		None,
		LynelBlue,
		LynelRed,
		MoblinBlue,
		MoblinRed,
		OctorokRed,
		OctorokRedFast,
		OctorokBlue,
		OctorokBlueFast,
		TektiteBlue,
		TektiteRed,
		LeeverBlue,
		LeeverRed,
		Peahat,
		FallingRocksGenerator,
		GhiniMaster,
		GhiniMinion
	}

	public enum MixedEnemyTypes {
		TwinMoldorm,
		MoblinBlue_MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_MoblinBlue,
		MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_Peahat_Peahat,
		Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue,
		LynelBlue_LynelBlue_LynelRed_LynelRed_LynelBlue_LeeverBlue,
		LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None,
		LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat,
		OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast,
		OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue,
		OctoRedFast_OctoRedFast_OctoBlue_OctoBlue_OctoRedFast_OctoBlueFast,
		OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
	}
}