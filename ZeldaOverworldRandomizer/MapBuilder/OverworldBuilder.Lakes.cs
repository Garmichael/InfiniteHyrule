using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void AssignLakes() {
			AssignLake(3);
			AssignLake(3);
			AssignLake(2);
			AssignLake(2);
		}

		private static void AssignLake(int lakeSize) {
			List<List<bool>> terrainMap = GenerateBlockTerrainMap(lakeSize, lakeSize);
			List<int> validPositions = GetValidTerrainMapScreenPositions(terrainMap, SettableTerrainType.Lake);

			if (validPositions.Count > 0) {
				int startScreenIndex = validPositions[Utilities.GetRandomInt(0, validPositions.Count - 1)];
				ApplyTerrainMapToScreens(terrainMap, startScreenIndex, SettableTerrainType.Lake);
			}
		}

		private static void AssignDocks() {
			SetDockAppropriateScreens();
			List<Screen> dockableScreens = GetDockableScreens();

			if (dockableScreens.Count < 2) {
				Debug.Print("Rebuild Reason: Not enough valid Dock Screens");
				ShouldRebuild = true;
			} else {
				Screen firstDockableScreen = dockableScreens[Utilities.GetRandomInt(0, dockableScreens.Count - 1)];
				firstDockableScreen.IsDock = true;
				firstDockableScreen.GetScreenUp().IsDockIsland = true;
				dockableScreens.Remove(firstDockableScreen);

				Screen farthestScreen = null;
				int farthestDistance = -1;

				foreach (Screen screen in dockableScreens) {
					int distance = Math.Abs(screen.Column - firstDockableScreen.Column);
					if (distance > farthestDistance) {
						farthestDistance = distance;
						farthestScreen = screen;
					}
				}

				if (farthestScreen != null) {
					farthestScreen.IsDock = true;
					farthestScreen.GetScreenUp().IsDockIsland = true;
				}
			}
		}

		private static void SetDockAppropriateScreens() {
			List<Biome> acceptableBiomes = new List<Biome> {
				Biome.Graveyard, Biome.Island, Biome.DenseForest, Biome.LightForest, Biome.MountainRange
			};
			
			foreach (Screen screen in Game.Screens) {
				Screen northScreen = screen.GetScreenUp();

				if (northScreen != null &&
				    northScreen.Region.Biome == Biome.Island &&
				    acceptableBiomes.Contains(screen.Region.Biome)
				) {
					screen.IsDockable = true;
				}
			}

			if (GetDockableScreens().Count < 2) {
				SetDockPotentialScreens();
			}
		}

		private static void SetDockPotentialScreens() {
			List<Biome> acceptableBiomes = new List<Biome> {
				Biome.Graveyard, Biome.Island, Biome.DenseForest, Biome.LightForest, Biome.MountainRange
			};
			
			foreach (Screen screen in Game.Screens) {
				Screen northScreen = screen.GetScreenUp();

				if (
					northScreen != null &&
					northScreen.ExitsSouth &&
					!northScreen.ExitsNorth &&
					!northScreen.ExitsWest &&
					!northScreen.ExitsEast &&
					northScreen.Region.Biome != Biome.River &&
					screen.Region.Biome != Biome.River &&
					acceptableBiomes.Contains(screen.Region.Biome)
				) {
					screen.IsDockable = true;
				}
			}
		}

		private static List<Screen> GetDockableScreens() {
			List<Screen> dockableScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				if (screen.IsDockable) {
					dockableScreens.Add(screen);
				}
			}

			return dockableScreens;
		}
	}
}