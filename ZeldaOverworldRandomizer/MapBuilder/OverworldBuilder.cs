using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuilders;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		public static bool ShouldRebuild;
		private const int MaxTimesAllowedToRebuild = 1000;

		public static void BuildOverworld() {
			Game.ResetProperties();
			ResetScreenData();
			AssignCoasts();
			AssignLakes();
			AssignRegions();
			AssignPathing();
			AssignDocks();
			AssignSubRegions();
			AssignFairyPonds();
			AssignWhistleLakes();
			AssignOverworldLadderItem();
			AssignSecretScreen();
			AssignCaves();
			AssignArmos();

			if (ShouldRebuild) {
				Rebuild();
			} else {
				BuildScreens();

				if (ShouldRebuild) {
					Rebuild();
					return;
				}

				Game.TimesRebuilt = 0;
			}
		}

		private static void Rebuild() {
			Game.TimesRebuilt++;
			Debug.Print("Rebuild Attempt # " + Game.TimesRebuilt);
			ShouldRebuild = false;
			
			if (Game.TimesRebuilt > MaxTimesAllowedToRebuild) {
				Game.TimesRebuilt = 0;
				MessageBox.Show(
					"This Seed does not produce a viable overworld, even after trying like, really hard. " +
					"Please try another."
				);
			} else {
				BuildOverworld();
			}
		}

		private static void ResetScreenData() {
			Game.Screens.Clear();

			for (int i = 0; i < Game.ScreensHigh * Game.ScreensWide; i++) {
				Game.Screens.Add(new Screen {
					ScreenId = i
				});
			}
		}

		private static void AssignFairyPonds() {
			const int fairyPondCount = 2;
			Screen previousFairyPond = null;

			for (int fairyPondIndex = 0; fairyPondIndex < fairyPondCount; fairyPondIndex++) {
				List<Screen> validScreens = new List<Screen>();

				foreach (Screen screen in Game.Screens) {
					int distanceFromOtherFairyPond = 16;

					if (previousFairyPond != null) {
						int previousFairyPondCol = previousFairyPond.Column;
						int thisScreenCol = screen.Column;
						distanceFromOtherFairyPond = Math.Abs(previousFairyPondCol - thisScreenCol);
					}

					if (screen.CanBeFairyPond && distanceFromOtherFairyPond > 5) {
						validScreens.Add(screen);
					}
				}

				if (validScreens.Count > 0) {
					Screen fairyScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					fairyScreen.IsFairyPond = true;
					previousFairyPond = fairyScreen;
				}
			}
		}

		private static void AssignWhistleLakes() {
			const int minWhistleLakeCount = 3;
			const int maxWhistleLakeCount = 5;
			int whistleLakeCount = Utilities.GetRandomInt(minWhistleLakeCount, maxWhistleLakeCount);

			for (int whistleLakeIndex = 0; whistleLakeIndex < whistleLakeCount; whistleLakeIndex++) {
				List<Screen> validScreens = new List<Screen>();

				foreach (Screen screen in Game.Screens) {
					if (screen.CanBeWhistleLake) {
						validScreens.Add(screen);
					}
				}

				if (validScreens.Count > 0) {
					Screen whistleLakeScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					whistleLakeScreen.IsWhistleLake = true;
				}
			}
		}

		private static void AssignLinkStartScreen() {
			List<Screen> validScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				bool isCorrectBiome = screen.Region != null && screen.Region.Biome == Biome.StartZone;
				bool isBorderScreen = screen.Row == 0 || screen.Row == Game.ScreensHigh - 1 ||
				                      screen.Column == 0 || screen.Column == Game.ScreensWide - 1;

				if (isCorrectBiome && isBorderScreen) {
					validScreens.Add(screen);
				}
			}

			Screen linkScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
			Game.LinkStartScreen = linkScreen.ScreenId;
		}

		private static void AssignOverworldLadderItem() {
			List<Screen> validScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				Screen screenSouth = screen.GetScreenDown();
				bool isDockReceiver = screenSouth != null && screenSouth.IsDock;

				if (
					(screen.IsLake || screen.IsCoast) &&
					screen.LakeSpot != NineSliceSpot.Middle &&
					!isDockReceiver &&
					!screen.IsDock &&
					screen.ScreenId != Game.LinkStartScreen &&
					screen.Region.Biome != Biome.River &&
					screen.Region.Biome != Biome.Kakariko &&
					!(screen.IsFirstRow && screen.IsFirstColumn) &&
					!(screen.IsFirstRow && screen.IsLastColumn) &&
					!(screen.IsLastRow && screen.IsFirstColumn) &&
					!(screen.IsLastRow && screen.IsLastColumn)
				) {
					validScreens.Add(screen);
				}
			}

			if (validScreens.Count == 0) {
				Debug.Print("Rebuild Reason: Not enough valid Ladder Item screens");
				ShouldRebuild = true;
			} else {
				Screen overworldItemScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];

				Game.OverworldItemScreenId = overworldItemScreen.ScreenId;
				Game.OverworldLadderScreens.Add(overworldItemScreen.ScreenId);
				overworldItemScreen.IsOverworldItemScreen = true;
			}
		}

		private static void AssignSecretScreen() {
			List<Screen> validSecretScreens = Game.Screens.Where(screen => screen.CanBeSecretScreen).ToList();

			if (validSecretScreens.Count == 0) {
				Debug.Print("Rebuild Reason: Not enough valid secret screens");
				ShouldRebuild = true;
			} else {
				validSecretScreens = validSecretScreens.OrderBy(screen => screen.Row).ToList();
				Screen secretScreen = validSecretScreens.Last();
				secretScreen.IsSecretScreen = true;
			}
		}

		private static void AssignArmos() {
			AssignArmosWithStairs();
			AssignArmosWithItem();
			AssignArmosWithNothing();
		}

		private static void AssignArmosWithStairs() {
			List<Screen> validScreens = new List<Screen>();

			List<int> validCaves = new List<int> {
				Game.CaveLookup[CaveType.Letter],
				Game.CaveLookup[CaveType.HeartContainer],
				Game.CaveLookup[CaveType.MoneyGame],
				Game.CaveLookup[CaveType.PotionShop],
				Game.CaveLookup[CaveType.RupeesHundred],
				Game.CaveLookup[CaveType.RupeesTen],
				Game.CaveLookup[CaveType.RupeesThirty],
				Game.CaveLookup[CaveType.ShopA],
				Game.CaveLookup[CaveType.ShopB],
				Game.CaveLookup[CaveType.ShopC],
				Game.CaveLookup[CaveType.BlueRingShop]
			};

			foreach (Screen screen in Game.Screens) {
				if (screen.CanBeArmosStairs && validCaves.Contains(screen.CaveDestination)) {
					validScreens.Add(screen);
				}
			}

			for (int i = 0; i < 6; i++) {
				if (validScreens.Count > 0) {
					Screen screen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					validScreens.Remove(screen);
					screen.IsArmosCave = true;
				}
			}
		}

		private static void AssignArmosWithItem() {
			List<Screen> validScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				if (screen.CanBeArmosHiddenItem) {
					validScreens.Add(screen);
				}
			}

			Screen hiddenItemScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
			hiddenItemScreen.IsArmosHiddenItem = true;
		}

		private static void AssignArmosWithNothing() {
			List<Screen> validScreens = new List<Screen>();
			foreach (Screen screen in Game.Screens) {
				if (screen.CanBeArmosWithNothing) {
					validScreens.Add(screen);
				}
			}

			for (int i = 0; i < 4; i++) {
				if (validScreens.Count > 0) {
					Screen screen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					validScreens.Remove(screen);
					screen.IsArmosDecor = true;
				}
			}
		}

		private static void BuildScreens() {
			int maxRegionPriority = 0;
			foreach (Region region in Game.Regions) {
				if (region.Priority > maxRegionPriority) {
					maxRegionPriority = region.Priority;
				}
			}

			for (int i = 0; i <= maxRegionPriority; i++) {
				foreach (Region region in Game.Regions) {
					if (region.Priority == i) {
						foreach (Screen screen in region.Screens) {
							BuildScreen(screen);
							BuildTrim(screen);
							screen.IsAlreadyGenerated = true;
						}
					}
				}
			}
		}

		private static void BuildScreen(Screen screen) {
			FillScreenWithBlankTiles(screen);

			ScreenBuilder builder;

			if (screen.Region.Biome == Biome.MountainRange || screen.Region.Biome == Biome.StartZone) {
				builder = new MountainRangeBuilder(screen);
			} else if (screen.Region.Biome == Biome.Tunnel) {
				builder = new TunnelBuilder(screen);
			} else if (screen.Region.Biome == Biome.Graveyard) {
				builder = new GraveyardBuilder(screen);
			} else if (screen.Region.Biome == Biome.Kakariko) {
				builder = new KakarikoBuilder(screen);
			} else if (screen.Region.Biome == Biome.Desert) {
				builder = new DesertBuilder(screen);
			} else if (screen.Region.Biome == Biome.RockyBeach) {
				builder = new RockyBeachBuilder(screen);
			} else if (screen.Region.Biome == Biome.DenseForest) {
				builder = new DenseForestBuilder(screen);
			} else if (screen.Region.Biome == Biome.LightForest) {
				builder = new LightForestBuilder(screen);
			} else if (screen.Region.Biome == Biome.GhostForest) {
				builder = new GhostForestBuilder(screen);
			} else if (screen.Region.Biome == Biome.Island) {
				builder = new IslandBuilder(screen);
			} else if (screen.Region.Biome == Biome.River) {
				builder = new RiverBuilder(screen);
			} else if (screen.Region.Biome == Biome.Debug) {
				builder = new DebugBuilder(screen);
			} else {
				builder = new ScreenBuilder(screen);
			}

			builder.BuildScreen();
			builder.AssignEnemies();
		}
	}
}