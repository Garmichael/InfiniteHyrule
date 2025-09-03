using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void AssignCaves() {
			if (ShouldRebuild) {
				return;
			}

			AssignDungeons();
			AssignSpecialCaves();
			AssignNormalOpenCaves();
			AssignNormalHiddenCaves();
			AssignAnyRoadCaves();
		}

		private static void AssignDungeons() {
			List<CaveType> dungeons = new List<CaveType> {
				CaveType.Dungeon1, CaveType.Dungeon2, CaveType.Dungeon3, CaveType.Dungeon4, CaveType.Dungeon5,
				CaveType.Dungeon6, CaveType.Dungeon7, CaveType.Dungeon8, CaveType.Dungeon9
			};

			List<Screen> finalDungeonScreens = new List<Screen> {
				null, null, null,
				null, null, null,
				null, null, null
			};

			List<Screen> validDungeonScreens = Game.Screens.Where(screen => screen.CanBeDungeon).ToList();

			if (FrontEnd.MainWindow.HideNormalDungeons) {
				validDungeonScreens = Game.Screens.Where(
					screen => screen.CanBeNormalHiddenCave &&
					          !screen.IsDockIsland &&
					          !screen.IsWhistleLake
				).ToList();
			}

			int dungeonCount = 9;

			if (validDungeonScreens.Count < dungeonCount) {
				Debug.Print("Rebuild Reason: Not enough valid dungeon screens");
				ShouldRebuild = true;
				return;
			}

			validDungeonScreens = validDungeonScreens.OrderBy(screen => screen.JourneyDistanceToStartScreen).ToList();

			while (validDungeonScreens.Count > dungeonCount * 2) {
				validDungeonScreens.RemoveAt(Utilities.GetRandomInt(0, validDungeonScreens.Count - 1));
			}

			while (validDungeonScreens.Count > dungeonCount) {
				bool removedOne = false;
				for (int a = 0; a < validDungeonScreens.Count - 1; a++) {
					for (int b = a + 1; b < validDungeonScreens.Count; b++) {
						Screen screenAScreen = validDungeonScreens[a];
						Screen screenBScreen = validDungeonScreens[b];

						if (screenAScreen.JourneyDistanceToScreen(screenBScreen, false) <= 5) {
							validDungeonScreens.Remove(screenBScreen);
							removedOne = true;
							break;
						}
					}

					if (removedOne) {
						break;
					}
				}

				if (!removedOne) {
					validDungeonScreens.RemoveAt(Utilities.GetRandomInt(3, validDungeonScreens.Count - 1));
				}
			}

			for (int i = 0; i < dungeonCount; i++) {
				finalDungeonScreens[i] = validDungeonScreens[i];
			}

			if (FrontEnd.MainWindow.HideNormalDungeons) {
				foreach (Screen dungeonScreen in validDungeonScreens) {
					dungeonScreen.CaveIsHidden = true;
				}
			}

			if (FrontEnd.MainWindow.HideDungeon4) {
				List<Screen> validIslandDungeons = Game.Screens.Where(screen => screen.IsDockIsland).ToList();
				validIslandDungeons = validIslandDungeons.OrderBy(screen => Utilities.GetRandomInt(0, 100)).ToList();

				if (validIslandDungeons.Count > 1) {
					finalDungeonScreens[4 - 1] = validIslandDungeons[0];
				} else {
					Debug.Print("Rebuild Reason: No valid Dock Island for Dungeon 4");
					ShouldRebuild = true;
				}
			}

			if (FrontEnd.MainWindow.HideDungeon7) {
				List<Screen> validLakeDungeons = Game.Screens.Where(screen => screen.IsWhistleLake).ToList();
				validLakeDungeons = validLakeDungeons.OrderBy(screen => Utilities.GetRandomInt(0, 100)).ToList();

				if (validLakeDungeons.Count > 1) {
					finalDungeonScreens[7 - 1] = validLakeDungeons[0];
				} else {
					Debug.Print("Rebuild Reason: No valid Whistle Lake for Dungeon 7");
				}
			}

			if (FrontEnd.MainWindow.HideDungeon8) {
				Screen secretScreen = Game.Screens.Where(screen => screen.IsSecretScreen).ToList().First();
				finalDungeonScreens[8 - 1] = secretScreen;
				finalDungeonScreens[8 - 1].CaveIsHidden = true;
			}

			if (FrontEnd.MainWindow.HideDungeon9) {
				List<Biome> dungeon9Biomes = new List<Biome> {
					Biome.Graveyard, Biome.LightForest, Biome.MountainRange,
					Biome.Desert, Biome.River, Biome.GhostForest, Biome.RockyBeach
				};

				List<Screen> validDungeon9Screens = Game.Screens.Where(
					screen => dungeon9Biomes.Contains(screen.Region.Biome) &&
					          screen.ScreenId > 0 &&
					          screen.CaveDestination == 0 &&
					          !screen.IsDeadEnd &&
					          !screen.IsDock &&
					          !screen.IsDockIsland &&
					          !screen.IsFairyPond &&
					          !screen.IsWhistleLake &&
					          !screen.IsOverworldItemScreen &&
					          !screen.IsSecretScreen &&
					          screen.DistanceToStartScreen > 4
				).ToList();

				Screen dungeon9Screen = validDungeon9Screens[Utilities.GetRandomInt(0, validDungeon9Screens.Count - 1)];
				dungeon9Screen.CaveDestination = Game.CaveLookup[CaveType.Dungeon9];
				dungeon9Screen.CaveIsHidden = true;
				finalDungeonScreens[9 - 1] = dungeon9Screen;
			}

			for (int dungeon = 0; dungeon < dungeons.Count; dungeon++) {
				finalDungeonScreens[dungeon].CaveDestination = Game.CaveLookup[dungeons[dungeon]];

				if (dungeon < 8) {
					Game.WarpWhistleDestinations[dungeon] = finalDungeonScreens[dungeon].ScreenId - 1;
				}
			}
		}

		private static void AssignSpecialCaves() {
			AssignCaveIntoScreens(new List<Screen> { Game.Screens[Game.LinkStartScreen] }, CaveType.WoodSword);

			List<Screen> validSpecialItemScreens = Game.Screens.Where(screen => screen.CanBeSpecialCave).ToList();

			if (validSpecialItemScreens.Count < 4) {
				Debug.Print("Rebuild Reason: Not enough valid special item screens");
				ShouldRebuild = true;
				return;
			}

			List<CaveType> caves = new List<CaveType> {
				CaveType.BlueRingShop, CaveType.Letter, CaveType.WhiteSword, CaveType.MasterSword
			};

			caves = caves.OrderBy(cave => Utilities.GetRandomInt(0, 100)).ToList();
			List<Screen> emptyDockIslands = validSpecialItemScreens.Where(screen => screen.IsDockIsland).ToList();

			AssignCavesIntoScreens(caves, emptyDockIslands, validSpecialItemScreens, false);
		}

		private static void AssignAnyRoadCaves() {
			List<Screen> anyRoad1Screens = new List<Screen>();
			List<Screen> anyRoad2Screens = new List<Screen>();
			List<Screen> anyRoad3Screens = new List<Screen>();
			List<Screen> anyRoad4Screens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				if (screen.CanBeAnyRoad) {
					const float centerColumn = Game.ScreensWide / 2f;
					const float centerRow = Game.ScreensHigh / 2f;

					bool isInTopLeft = screen.Column <= centerColumn - 2 && screen.Row <= centerRow - 2;
					bool isInTopRight = screen.Column >= centerColumn + 2 && screen.Row <= centerRow - 2;
					bool isInBottomLeft = screen.Column <= centerColumn - 2 && screen.Row >= centerRow + 2;
					bool isInBottomRight = screen.Column >= centerColumn + 2 && screen.Row >= centerRow + 2;

					if (isInTopLeft) {
						anyRoad1Screens.Add(screen);
					} else if (isInTopRight) {
						anyRoad2Screens.Add(screen);
					} else if (isInBottomLeft) {
						anyRoad3Screens.Add(screen);
					} else if (isInBottomRight) {
						anyRoad4Screens.Add(screen);
					}
				}
			}

			if (anyRoad1Screens.Count == 0 ||
			    anyRoad2Screens.Count == 0 ||
			    anyRoad3Screens.Count == 0 ||
			    anyRoad4Screens.Count == 0) {
				Debug.Print("Rebuild Reason: Not enough Any Road screens");
				ShouldRebuild = true;
				return;
			}

			AssignCaveIntoScreens(anyRoad1Screens, CaveType.AnyRoad);
			AssignCaveIntoScreens(anyRoad2Screens, CaveType.AnyRoad);
			AssignCaveIntoScreens(anyRoad3Screens, CaveType.AnyRoad);
			AssignCaveIntoScreens(anyRoad4Screens, CaveType.AnyRoad);
		}

		private static void AssignNormalOpenCaves() {
			List<CaveType> allOpenCaves = GetAllNormalOpenCaves();

			allOpenCaves = allOpenCaves.OrderBy(cave => Utilities.GetRandomInt(0, 100)).ToList();

			List<Screen> openCaveScreens = Game.Screens.Where(screen => screen.CanBeNormalOpenCave).ToList();
			openCaveScreens = openCaveScreens.OrderBy(screen => Utilities.GetRandomInt(0, 100)).ToList();

			List<Screen> deadEndScreens = openCaveScreens.Where(screen => screen.IsDeadEnd).ToList();

			AssignCavesIntoScreens(allOpenCaves, deadEndScreens, openCaveScreens, false);
		}

		private static void AssignNormalHiddenCaves() {
			List<CaveType> allHiddenCaves = GetAllNormalHiddenCaves();

			allHiddenCaves = allHiddenCaves.OrderBy(cave => Utilities.GetRandomInt(0, 100)).ToList();

			if (!FrontEnd.MainWindow.HideDungeon8) {
				int indexOfFirstHundredRupeeCave = allHiddenCaves.IndexOf(CaveType.RupeesHundred);
				allHiddenCaves.RemoveAt(indexOfFirstHundredRupeeCave);
				Screen secretScreen = Game.Screens.Where(screen => screen.IsSecretScreen).ToList().First();
				secretScreen.CaveDestination = Game.CaveLookup[CaveType.RupeesHundred];
				secretScreen.CaveIsHidden = false;
			}

			List<Screen> hiddenCaveScreens = Game.Screens.Where(screen => screen.CanBeNormalHiddenCave).ToList();
			hiddenCaveScreens = hiddenCaveScreens.OrderBy(screen => Utilities.GetRandomInt(0, 100)).ToList();

			List<Screen> firstPriorityScreens =
				hiddenCaveScreens.Where(screen => screen.IsDeadEnd || screen.IsWhistleLake).ToList();

			AssignCavesIntoScreens(allHiddenCaves, firstPriorityScreens, hiddenCaveScreens, true);
		}

		private static List<CaveType> GetAllNormalOpenCaves() {
			List<CaveType> allOpenCaves = new List<CaveType>();

			for (int i = 0; i < Game.TotalCavesShopAOpen; i++) {
				allOpenCaves.Add(CaveType.ShopA);
			}

			for (int i = 0; i < Game.TotalCavesShopBOpen; i++) {
				allOpenCaves.Add(CaveType.ShopB);
			}

			for (int i = 0; i < Game.TotalCavesShopCOpen; i++) {
				allOpenCaves.Add(CaveType.ShopC);
			}

			for (int i = 0; i < Game.TotalCavesPotionShopsOpen; i++) {
				allOpenCaves.Add(CaveType.PotionShop);
			}

			for (int i = 0; i < Game.TotalCavesRupeesTenOpen; i++) {
				allOpenCaves.Add(CaveType.RupeesTen);
			}

			for (int i = 0; i < Game.TotalCavesRupeesThirtyOpen; i++) {
				allOpenCaves.Add(CaveType.RupeesThirty);
			}

			for (int i = 0; i < Game.TotalCavesRupeesHundredOpen; i++) {
				allOpenCaves.Add(CaveType.RupeesHundred);
			}

			for (int i = 0; i < Game.TotalCavesMoneyGameOpen; i++) {
				allOpenCaves.Add(CaveType.MoneyGame);
			}

			for (int i = 0; i < Game.TotalCavesTaxOpen; i++) {
				allOpenCaves.Add(CaveType.Tax);
			}

			for (int i = 0; i < Game.TotalCavesHeartsOpen; i++) {
				allOpenCaves.Add(CaveType.HeartContainer);
			}

			if (FrontEnd.MainWindow.Dungeon9Hint) {
				allOpenCaves.Add(CaveType.HintSecretInTreeAtDeadEnd);
			}

			return allOpenCaves;
		}

		private static List<CaveType> GetAllNormalHiddenCaves() {
			List<CaveType> allHiddenCaves = new List<CaveType>();

			for (int i = 0; i < Game.TotalCavesShopAHidden; i++) {
				allHiddenCaves.Add(CaveType.ShopA);
			}

			for (int i = 0; i < Game.TotalCavesShopBHidden; i++) {
				allHiddenCaves.Add(CaveType.ShopB);
			}

			for (int i = 0; i < Game.TotalCavesShopCHidden; i++) {
				allHiddenCaves.Add(CaveType.ShopC);
			}

			for (int i = 0; i < Game.TotalCavesPotionShopsHidden; i++) {
				allHiddenCaves.Add(CaveType.PotionShop);
			}

			for (int i = 0; i < Game.TotalCavesRupeesTenHidden; i++) {
				allHiddenCaves.Add(CaveType.RupeesTen);
			}

			for (int i = 0; i < Game.TotalCavesRupeesThirtyHidden; i++) {
				allHiddenCaves.Add(CaveType.RupeesThirty);
			}

			for (int i = 0; i < Game.TotalCavesRupeesHundredHidden; i++) {
				allHiddenCaves.Add(CaveType.RupeesHundred);
			}

			for (int i = 0; i < Game.TotalCavesMoneyGameHidden; i++) {
				allHiddenCaves.Add(CaveType.MoneyGame);
			}

			for (int i = 0; i < Game.TotalCavesTaxHidden; i++) {
				allHiddenCaves.Add(CaveType.Tax);
			}

			for (int i = 0; i < Game.TotalCavesHeartsHidden; i++) {
				allHiddenCaves.Add(CaveType.HeartContainer);
			}

			return allHiddenCaves;
		}

		private static void AssignCaveIntoScreens(List<Screen> screens, CaveType caveType) {
			if (screens.Count == 0) {
				return;
			}

			Screen caveScreen = screens[Utilities.GetRandomInt(0, screens.Count - 1)];
			caveScreen.CaveDestination = Game.CaveLookup[caveType];

			if (caveType == CaveType.AnyRoad) {
				Game.AnyRoadCaveScreens.Add(caveScreen);
			}

			screens.Remove(caveScreen);
		}

		private static void AssignCavesIntoScreens(
			List<CaveType> allCaveTypes,
			List<Screen> firstPriorityScreens,
			List<Screen> allScreens,
			bool isHidden
		) {
			foreach (Screen screen in firstPriorityScreens) {
				if (allCaveTypes.Count > 0) {
					screen.CaveDestination = Game.CaveLookup[allCaveTypes[0]];
					screen.CaveIsHidden = isHidden;
					allCaveTypes.RemoveAt(0);
					allScreens.Remove(screen);
				} else {
					break;
				}
			}

			foreach (Screen screen in allScreens) {
				if (allCaveTypes.Count > 0) {
					screen.CaveDestination = Game.CaveLookup[allCaveTypes[0]];
					screen.CaveIsHidden = isHidden;
					allCaveTypes.RemoveAt(0);
				} else {
					break;
				}
			}
		}
	}
}