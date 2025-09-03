using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void AssignRegions() {
			AssignStartRegion();
			AssignMountainRegion();
			AssignDesertRegion();
			AssignRiverRegions();
			AssignGraveyardRegion();
			AssignKakarikoRegion();
			AssignCoastRegions();
			AssignForestRegions();
			AssignTunnelBiomes();
			AssignRemainingScreens();
		}

		private static void AssignStartRegion() {
			Region startRegion = new Region {
				Biome = Biome.StartZone,
				EnvironmentColor = EnvironmentColor.Green,
				Priority = 5
			};

			Direction startRegionBorderSnap = new List<Direction> {
				Direction.Down, Direction.Left, Direction.Right
			}[Utilities.GetRandomInt(0, 2)];

			if (startRegionBorderSnap == Direction.Down) {
				int startCol = Utilities.GetRandomInt(0, Game.ScreensWide - 3);
				int startRow = Game.ScreensHigh - 2;

				PaintScreenRegion(startRegion, startCol, startRow, 3, 2);
			}

			if (startRegionBorderSnap == Direction.Left || startRegionBorderSnap == Direction.Right) {
				int startRow = Utilities.GetRandomInt(4, Game.ScreensHigh - 2);
				int startCol = startRegionBorderSnap == Direction.Left ? 0 : Game.ScreensWide - 3;

				PaintScreenRegion(startRegion, startCol, startRow, 3, 2);
			}

			AssignLinkStartScreen();
		}

		private static void AssignRiverRegions() {
			if (!FrontEnd.MainWindow.IncludeRiver) {
				return;
			}

			List<Screen> validStartScreens = new List<Screen>();
			foreach (Screen screen in Game.Screens) {
				if (screen.Row == 0 &&
				    screen.Column > 0 &&
				    screen.Column < Game.LastScreenColumn &&
				    screen.Region == null &&
				    screen.GetScreenDown().Region == null &&
				    !screen.IsLake &&
				    !screen.GetScreenDown().IsLake &&
				    !screen.IsOverworldItemScreen &&
				    !screen.GetScreenDown().IsOverworldItemScreen
				   ) {
					validStartScreens.Add(screen);
				}
			}

			if (validStartScreens.Count > 0) {
				Screen startScreen = validStartScreens[Utilities.GetRandomInt(0, validStartScreens.Count - 1)];

				Region riverRegion = new Region {
					Biome = Biome.River,
					EnvironmentColor = EnvironmentColor.Green,
					Priority = 8
				};

				riverRegion.AddScreen(startScreen);
				startScreen.ExitsSouth = true;
				startScreen.GetScreenDown().ExitsNorth = true;
				GrowRiverRegion(riverRegion, startScreen.GetScreenDown());

				int totalRiverScreens = riverRegion.Screens.Count;

				for (int i = 1; i < totalRiverScreens; i += 3) {
					if (i < riverRegion.Screens.Count - 1) {
						riverRegion.Screens[i].HasRiverBridge = true;
					}
				}
			}
		}

		private static void GrowRiverRegion(Region riverRegion, Screen screen) {
			riverRegion.AddScreen(screen);

			List<Screen> nextScreenOptions = new List<Screen>();

			Screen screenLeft = screen.GetScreenLeft();
			Screen screenRight = screen.GetScreenRight();
			Screen screenDown = screen.GetScreenDown();

			if (screenLeft != null &&
			    screenLeft.Region == null &&
			    screenLeft.LakeSpot != NineSliceSpot.Bottom &&
			    screenLeft.LakeSpot != NineSliceSpot.Top &&
			    screenLeft.LakeSpot != NineSliceSpot.Left &&
			    screenLeft.LakeSpot != NineSliceSpot.Right &&
			    !(screenLeft.IsLake && screen.Row < 5) &&
			    !(screenLeft.Column == 0 && screen.Row < 5) &&
			    !(screenLeft.Column == Game.LastScreenColumn && screen.Row < 5)
			   ) {
				nextScreenOptions.Add(screenLeft);
			}

			if (screenRight != null &&
			    screenRight.Region == null &&
			    screenRight.LakeSpot != NineSliceSpot.Bottom &&
			    screenRight.LakeSpot != NineSliceSpot.Top &&
			    screenRight.LakeSpot != NineSliceSpot.Left &&
			    screenRight.LakeSpot != NineSliceSpot.Right &&
			    !(screenRight.IsLake && screen.Row < 5) &&
			    !(screenRight.Column == 0 && screen.Row < 5) &&
			    !(screenRight.Column == Game.LastScreenColumn && screen.Row < 5)
			   ) {
				nextScreenOptions.Add(screenRight);
			}

			if (screenDown != null &&
			    screenDown.Region == null &&
			    screenDown.LakeSpot != NineSliceSpot.Bottom &&
			    screenDown.LakeSpot != NineSliceSpot.Top &&
			    screenDown.LakeSpot != NineSliceSpot.Left &&
			    screenDown.LakeSpot != NineSliceSpot.Right &&
			    !(screenDown.IsLake && screen.Row < 5) &&
			    !(screenDown.Column == 0 && screen.Row < 5) &&
			    !(screenDown.Column == Game.LastScreenColumn && screen.Row < 5)
			   ) {
				nextScreenOptions.Add(screenDown);
			}

			if (nextScreenOptions.Count == 0) {
				if (screenLeft != null &&
				    screenLeft.LakeSpot != NineSliceSpot.Bottom &&
				    screenLeft.LakeSpot != NineSliceSpot.Top &&
				    screenLeft.LakeSpot != NineSliceSpot.Left &&
				    screenLeft.LakeSpot != NineSliceSpot.Right &&
				    screenLeft.Region == null
				   ) {
					nextScreenOptions.Add(screenLeft);
				}

				if (screenRight != null &&
				    screenRight.LakeSpot != NineSliceSpot.Bottom &&
				    screenRight.LakeSpot != NineSliceSpot.Top &&
				    screenRight.LakeSpot != NineSliceSpot.Left &&
				    screenRight.LakeSpot != NineSliceSpot.Right &&
				    screenRight.Region == null
				   ) {
					nextScreenOptions.Add(screenRight);
				}

				if (screenDown != null &&
				    screenDown.LakeSpot != NineSliceSpot.Bottom &&
				    screenDown.LakeSpot != NineSliceSpot.Top &&
				    screenDown.LakeSpot != NineSliceSpot.Left &&
				    screenDown.LakeSpot != NineSliceSpot.Right &&
				    screenDown.Region == null
				   ) {
					nextScreenOptions.Add(screenDown);
				}
			}

			if (!screen.IsLake &&
			    screen.Row != Game.LastScreenRow &&
			    screen.Column != 0 &&
			    screen.Column != Game.LastScreenColumn &&
			    nextScreenOptions.Count > 0
			   ) {
				Screen nextScreen = nextScreenOptions[Utilities.GetRandomInt(0, nextScreenOptions.Count - 1)];

				if (nextScreen != null) {
					if (nextScreen == screenLeft) {
						screen.ExitsWest = true;
						nextScreen.ExitsEast = true;
					}

					if (nextScreen == screenRight) {
						screen.ExitsEast = true;
						nextScreen.ExitsWest = true;
					}

					if (nextScreen == screenDown) {
						screen.ExitsSouth = true;
						nextScreen.ExitsNorth = true;
					}

					GrowRiverRegion(riverRegion, nextScreen);
				}
			}
		}

		private static void AssignGraveyardRegion() {
			const int width = 2;
			const int height = 2;

			List<List<bool>> terrainMap = GenerateBlockTerrainMap(width, height);

			List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap);

			if (validPositions.Count > 0) {
				int startScreenIndex = validPositions[Utilities.GetRandomInt(0, validPositions.Count - 1)];
				List<Screen> screens =
					ApplyTerrainMapToScreens(terrainMap, startScreenIndex, SettableTerrainType.Graveyard);

				Region graveyardRegion = new Region {
					Biome = Biome.Graveyard,
					EnvironmentColor = EnvironmentColor.Grey,
					Priority = 7
				};

				foreach (Screen screen in screens) {
					graveyardRegion.AddScreen(screen);
				}
			}
		}

		private static void AssignKakarikoRegion() {
			if (!FrontEnd.MainWindow.IncludeKakariko) {
				return;
			}

			int width = 2 + Utilities.GetRandomInt(0, 1);
			int height = width == 2 ? 3 : 2;

			List<List<bool>> terrainMap = GenerateBlockTerrainMap(width, height);

			List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap);

			if (validPositions.Count > 0) {
				int startScreenIndex = validPositions[Utilities.GetRandomInt(0, validPositions.Count - 1)];
				List<Screen> screens = ApplyTerrainMapToScreens(terrainMap, startScreenIndex);

				Region kakarikoBiome = new Region {
					Biome = Biome.Kakariko,
					EnvironmentColor = Utilities.GetRandomInt(0, 100) >= 50
						? EnvironmentColor.Green
						: EnvironmentColor.Brown,
					Priority = 7
				};

				foreach (Screen screen in screens) {
					kakarikoBiome.AddScreen(screen);
				}
			}
		}

		private static void AssignDesertRegion() {
			const int width = 4;
			const int height = 2;

			List<List<bool>> terrainMap = GenerateBlockTerrainMap(width, height);

			if (Utilities.GetRandomInt(0, 1) == 0) {
				int roll = Utilities.GetRandomInt(0, 2);
				if (roll == 0) {
					terrainMap[0][0] = false;
					terrainMap[0][width - 1] = false;
				} else if (roll == 1) {
					terrainMap[0][0] = false;
					terrainMap[0][1] = false;
				} else {
					terrainMap[0][width - 2] = false;
					terrainMap[0][width - 1] = false;
				}
			} else {
				int roll = Utilities.GetRandomInt(0, 2);
				if (roll == 0) {
					terrainMap[height - 1][0] = false;
					terrainMap[height - 1][width - 1] = false;
				} else if (roll == 1) {
					terrainMap[height - 1][0] = false;
					terrainMap[height - 1][1] = false;
				} else {
					terrainMap[height - 1][width - 2] = false;
					terrainMap[height - 1][width - 1] = false;
				}
			}

			List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap, SettableTerrainType.Desert);

			Region desertRegion = new Region {
				Biome = Biome.Desert,
				EnvironmentColor = EnvironmentColor.Brown,
				Priority = 7
			};

			if (validPositions.Count > 0) {
				int startScreenIndex = validPositions[Utilities.GetRandomInt(0, validPositions.Count - 1)];
				List<Screen> screens =
					ApplyTerrainMapToScreens(terrainMap, startScreenIndex, SettableTerrainType.Desert);

				foreach (Screen screen in screens) {
					desertRegion.AddScreen(screen);
				}
			}

			if (desertRegion.Screens.Count > 0) {
				List<Screen> validOasisScreens = desertRegion.Screens.Where(screen => !screen.IsDeadEnd).ToList();
				validOasisScreens[Utilities.GetRandomInt(0, validOasisScreens.Count - 1)].IsOasis = true;
			}
		}

		private static void AssignCoastRegions() {
			foreach (Screen screen in Game.Screens) {
				if (screen.IsCoast && screen.Region == null) {
					Region coastRegion = new Region {
						Biome = Biome.RockyBeach,
						EnvironmentColor = EnvironmentColor.Brown,
						Priority = 6
					};

					CollectCoasts(coastRegion, screen);
				}
			}
		}

		private static void CollectCoasts(Region region, Screen screen) {
			Screen screenRight = screen.GetScreenRight();
			Screen screenDown = screen.GetScreenDown();
			Screen screenLeft = screen.GetScreenLeft();
			Screen screenUp = screen.GetScreenUp();

			region.AddScreen(screen);

			if (region.Screens.Count < 8 && screenRight != null && screenRight.IsCoast && screenRight.Region == null) {
				CollectCoasts(region, screenRight);
			}

			if (region.Screens.Count < 8 && screenDown != null && screenDown.IsCoast && screenDown.Region == null) {
				CollectCoasts(region, screenDown);
			}

			if (region.Screens.Count < 8 && screenLeft != null && screenLeft.IsCoast && screenLeft.Region == null) {
				CollectCoasts(region, screenLeft);
			}

			if (region.Screens.Count < 8 && screenUp != null && screenUp.IsCoast && screenUp.Region == null) {
				CollectCoasts(region, screenUp);
			}
		}

		private static void AssignMountainRegion() {
			bool hasBeenBuilt = false;
			Region mountainRegion = new Region {
				Biome = Biome.MountainRange,
				EnvironmentColor = EnvironmentColor.Brown,
				Priority = 4
			};

			while (!hasBeenBuilt) {
				int width = Utilities.GetRandomInt(8, 10);
				const int height = 3;

				List<List<bool>> terrainMap = GenerateBlockTerrainMap(width, height);
				List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap, SettableTerrainType.Mountain);
				List<int> farEnoughAwayPositions = new List<int>();

				foreach (int validPosition in validPositions) {
					Screen startScreen = Game.Screens[Game.LinkStartScreen];

					if (startScreen.Column <= 5) {
						if (validPosition >= 5) {
							farEnoughAwayPositions.Add(validPosition);
						}
					} else if (startScreen.Column >= Game.ScreensWide - 5) {
						if (validPosition <= 2) {
							farEnoughAwayPositions.Add(validPosition);
						}
					} else {
						farEnoughAwayPositions.Add(validPosition);
					}
				}

				int startScreenIndex = farEnoughAwayPositions[
					Utilities.GetRandomInt(0, farEnoughAwayPositions.Count - 1)
				];

				List<Screen> screens = ApplyTerrainMapToScreens(terrainMap, startScreenIndex);

				foreach (Screen screen in screens) {
					mountainRegion.AddScreen(screen);
				}

				hasBeenBuilt = true;
			}

			ExpandRegion(mountainRegion, 3);
		}

		private static void AssignForestRegions() {
			int fails = 0;
			const int maxFails = 50;
			const int totalAllowedDenseForests = 2;
			int totalAllowedGhostForests = FrontEnd.MainWindow.IncludeGhostForest ? 1 : 0;
			int totalDenseForests = 0;
			int totalGhostForests = 0;

			for (int i = 0; i < 10; i++) {
				Biome selectedBiome = totalGhostForests < totalAllowedGhostForests
					? Biome.GhostForest
					: totalDenseForests < totalAllowedDenseForests
						? Biome.DenseForest
						: Biome.LightForest;

				int width;
				int height;
				int priority;

				switch (selectedBiome) {
					case Biome.GhostForest:
						totalGhostForests++;
						if (Utilities.GetRandomInt(0, 1) == 0) {
							width = 3;
							height = 1;
						} else {
							height = 3;
							width = 1;
						}

						priority = 3;
						break;
					case Biome.DenseForest:
						totalDenseForests++;
						width = Utilities.GetRandomInt(2, 4);
						height = Utilities.GetRandomInt(2, 4);
						while (width * height >= 12) {
							if (width > 2 && Utilities.GetRandomInt(0, 1) == 0) {
								width--;
							} else {
								height--;
							}
						}

						priority = 2;
						break;
					case Biome.LightForest:
						if (Utilities.GetRandomInt(0, 1) == 0) {
							width = Utilities.GetRandomInt(2, 4);
							height = 1;
						} else {
							height = Utilities.GetRandomInt(2, 4);
							width = 1;
						}

						priority = 3;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				bool vertical = Utilities.GetRandomInt(0, 1) == 1;

				if (vertical) {
					int temp = width;
					width = height;
					height = temp;
				}

				List<List<bool>> terrainMap = GenerateBlockTerrainMap(width, height);
				List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap);

				if (validPositions.Count > 0) {
					int startScreenIndex = validPositions[Utilities.GetRandomInt(0, validPositions.Count - 1)];
					List<Screen> screens = ApplyTerrainMapToScreens(terrainMap, startScreenIndex);

					Region newRegion = new Region {
						Biome = selectedBiome,
						EnvironmentColor = Utilities.GetRandomInt(0, 2) == 0
							? EnvironmentColor.Brown
							: EnvironmentColor.Green,
						Priority = priority
					};

					foreach (Screen screen in screens) {
						newRegion.AddScreen(screen);
					}
				} else {
					i--;
					fails++;
					if (fails > maxFails) {
						break;
					}
				}
			}
		}

		private static void AssignTunnelBiomes() {
			if (Utilities.GetRandomInt(0, 1) == 0) {
				AssignVerticalTunnels();
				AssignHorizontalTunnels();
			} else {
				AssignHorizontalTunnels();
				AssignVerticalTunnels();
			}
		}

		private static void AssignVerticalTunnels() {
			const int maxLength = 3;
			const int minLength = 3;

			for (int i = maxLength; i >= minLength; i--) {
				foreach (Screen screen in Game.Screens) {
					if (screen.Region == null) {
						Screen sourceScreen = screen;
						bool isStretch = true;

						for (int j = 1; j < i; j++) {
							Screen screenDown = sourceScreen.GetScreenDown();
							if (screenDown != null && screenDown.Region == null) {
								sourceScreen = screenDown;
							} else {
								isStretch = false;
								break;
							}
						}

						if (isStretch && Utilities.GetRandomInt(0, 2) == 0) {
							Region tunnelRegion = new Region {
								Biome = Biome.Tunnel,
								EnvironmentColor = EnvironmentColor.Brown,
								Priority = 6
							};

							tunnelRegion.AddScreen(screen);
							Screen thisScreen = screen;
							for (int j = 1; j < i; j++) {
								thisScreen = thisScreen.GetScreenDown();
								tunnelRegion.AddScreen(thisScreen);
							}
						}
					}
				}
			}
		}

		private static void AssignHorizontalTunnels() {
			const int maxLength = 3;
			const int minLength = 3;

			for (int i = maxLength; i >= minLength; i--) {
				foreach (Screen screen in Game.Screens) {
					if (screen.Region == null) {
						Screen sourceScreen = screen;
						bool isStretch = true;

						for (int j = 1; j < i; j++) {
							Screen screenRight = sourceScreen.GetScreenRight();
							if (screenRight != null && screenRight.Region == null) {
								sourceScreen = screenRight;
							} else {
								isStretch = false;
								break;
							}
						}

						if (isStretch && Utilities.GetRandomInt(0, 2) == 0) {
							Region tunnelRegion = new Region {
								Biome = Biome.Tunnel,
								EnvironmentColor = EnvironmentColor.Brown,
								Priority = 6
							};

							tunnelRegion.AddScreen(screen);
							Screen thisScreen = screen;
							for (int j = 1; j < i; j++) {
								thisScreen = thisScreen.GetScreenRight();
								tunnelRegion.AddScreen(thisScreen);
							}
						}
					}
				}
			}
		}

		private static void AssignRemainingScreens() {
			int fails = 0;
			const int maxFails = 128;

			while (!AllScreensAssignedToRegion()) {
				foreach (Screen screen in Game.Screens) {
					if (screen.Region == null) {
						Screen screenUp = screen.GetScreenUp();
						Screen screenDown = screen.GetScreenDown();
						Screen screenLeft = screen.GetScreenLeft();
						Screen screenRight = screen.GetScreenRight();

						List<Region> validRegions = new List<Region>();
						List<Biome> unacceptableBiomes = new List<Biome> {
							Biome.Tunnel,
							Biome.Desert,
							Biome.Graveyard,
							Biome.RockyBeach,
							Biome.StartZone,
							Biome.MountainRange,
							Biome.River
						};

						if (screenUp?.Region != null && !unacceptableBiomes.Contains(screenUp.Region.Biome)) {
							validRegions.Add(screenUp.Region);
						}

						if (screenDown?.Region != null && !unacceptableBiomes.Contains(screenDown.Region.Biome)) {
							validRegions.Add(screenDown.Region);
						}

						if (screenLeft?.Region != null && !unacceptableBiomes.Contains(screenLeft.Region.Biome)) {
							validRegions.Add(screenLeft.Region);
						}

						if (screenRight?.Region != null && !unacceptableBiomes.Contains(screenRight.Region.Biome)) {
							validRegions.Add(screenRight.Region);
						}

						if (validRegions.Count > 0) {
							fails = 0;

							Region smallestRegion = validRegions[0];

							foreach (Region region in validRegions) {
								if (region.Screens.Count < smallestRegion.Screens.Count) {
									smallestRegion = region;
								}
							}

							smallestRegion.AddScreen(screen);
						} else {
							fails++;

							if (fails > maxFails) {
								Region newRegion = new Region {
									Biome = Biome.LightForest,
									EnvironmentColor = EnvironmentColor.Green,
									Priority = 3
								};

								newRegion.AddScreen(screen);
								fails = 0;
							}
						}
					}
				}
			}
		}

		private static bool AllScreensAssignedToRegion() {
			bool allScreensAssigned = true;

			foreach (Screen screen in Game.Screens) {
				if (screen.Region == null) {
					allScreensAssigned = false;
					break;
				}
			}

			return allScreensAssigned;
		}

		private static void PaintScreenRegion(Region region, int x, int y, int width, int height) {
			if (x < 0) {
				x = 0;
			}

			if (y < 0) {
				y = 0;
			}

			while (x + width > Game.ScreensWide) {
				x -= 1;
			}

			while (y + height > Game.ScreensHigh) {
				y -= 1;
			}

			for (int col = x; col < x + width; col++) {
				for (int row = y; row < y + height; row++) {
					Screen screen = Game.Screens[Utilities.GetScreenIndexFromColAndRow(col, row)];
					if (screen.Region == null) {
						screen.Region = region;
						region.AddScreen(screen);
					}
				}
			}
		}

		private static void ExpandRegion(Region region, int expansionSize = 2) {
			List<Screen> validScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				if (screen.Region != null) {
					continue;
				}

				Screen screenUp = screen.GetScreenUp();
				Screen screenDown = screen.GetScreenDown();
				Screen screenLeft = screen.GetScreenLeft();
				Screen screenRight = screen.GetScreenRight();

				bool screenUpIsRegion = screenUp != null && screenUp.Region == region;
				bool screenDownIsRegion = screenDown != null && screenDown.Region == region;
				bool screenLeftIsRegion = screenLeft != null && screenLeft.Region == region;
				bool screenRightIsRegion = screenRight != null && screenRight.Region == region;

				if (screenUpIsRegion || screenDownIsRegion || screenLeftIsRegion || screenRightIsRegion) {
					validScreens.Add(screen);
				}
			}

			for (int i = 0; i < expansionSize; i++) {
				if (validScreens.Count > 0) {
					Screen validScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					validScreen.Region = region;
					region.AddScreen(validScreen);
					validScreens.Remove(validScreen);
				}
			}
		}

		private static int GetDistanceFromHomeToBiome(Region region, Region homeRegion) {
			if (region.Screens.Count == 0) {
				return 0;
			}

			int distanceInCols = Math.Abs(GetRegionCenterCol(region) - GetRegionCenterCol(homeRegion));
			int distanceInRows = Math.Abs(GetRegionCenterRow(region) - GetRegionCenterRow(homeRegion));

			return Math.Abs(distanceInCols + distanceInRows);
		}

		private static int GetRegionCenterCol(Region region) {
			List<int> allCols = new List<int>();

			foreach (Screen screen in region.Screens) {
				allCols.Add(screen.Column);
			}

			return (int) Math.Floor(allCols.Average());
		}

		private static int GetRegionCenterRow(Region region) {
			List<int> allRows = new List<int>();

			foreach (Screen screen in region.Screens) {
				allRows.Add(screen.Row);
			}

			return (int) Math.Floor(allRows.Average());
		}

		private static void SetRegionBiome(Region region, EnvironmentColor color, TerrainType terrainType) {
			foreach (Screen screen in region.Screens) {
				screen.EnvironmentColor = color;
			}
		}

		private static void AssignSubRegions() {
			AssignIslandRegions();
			AssignDeadEndsOfForests();
		}

		private static void AssignIslandRegions() {
			foreach (Screen screen in Game.Screens) {
				if (screen.IsDockIsland) {
					Region newRegion = new Region {
						Biome = Biome.Island,
						Priority = 1
					};

					screen.Region.RemoveScreen(screen);
					newRegion.AddScreen(screen);
				} else if (screen.IsLake && screen.LakeSpot == NineSliceSpot.Middle) {
					Region newRegion = new Region {
						Biome = Biome.Island,
						Priority = 1
					};

					screen.Region.RemoveScreen(screen);
					newRegion.AddScreen(screen);
				}
			}
		}

		private static void AssignDeadEndsOfForests() {
			foreach (Screen screen in Game.Screens) {
				if (screen.Region.Biome == Biome.DenseForest || screen.Region.Biome == Biome.LightForest) {
					if (screen.IsDeadEnd) {
						Region newRegion = new Region {
							Biome = Biome.MountainRange,
							EnvironmentColor = EnvironmentColor.Green,
							Priority = 4
						};

						screen.Region.RemoveScreen(screen);
						newRegion.AddScreen(screen);
					}
				}
			}
		}
	}
}