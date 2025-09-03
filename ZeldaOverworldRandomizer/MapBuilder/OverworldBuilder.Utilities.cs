using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static List<List<bool>> GenerateBlockTerrainMap(int width, int height) {
			List<List<bool>> tempMap = BuildTerrainMap(width, height, true);
			return tempMap;
		}

		private static List<List<bool>> BuildTerrainMap(int width, int height, bool initialValue = false) {
			List<List<bool>> map = new List<List<bool>>();

			if (width > Game.ScreensWide) {
				width = Game.ScreensWide;
			}

			if (height > Game.ScreensHigh) {
				height = Game.ScreensHigh;
			}

			for (int i = 0; i < height; i++) {
				List<bool> row = new List<bool>();
				for (int j = 0; j < width; j++) {
					row.Add(initialValue);
				}

				map.Add(row);
			}

			return map;
		}

		private static List<int> GetValidTerrainMapScreenPositions(
			List<List<bool>> terrainMap,
			SettableTerrainType terrainType = SettableTerrainType.None
		) {
			List<int> validPositions = new List<int>();

			for (int screenIndex = 0; screenIndex < Game.Screens.Count; screenIndex++) {
				Screen screen = Game.Screens[screenIndex];

				int terrainMapHeight = terrainMap.Count;
				int terrainMapWidth = terrainMap[0].Count;
				bool isValidColumn = screen.Column <= Game.ScreensWide - terrainMapWidth;
				bool isValidRow = screen.Row <= Game.ScreensHigh - terrainMapHeight;
				bool fitsHere = true;

				if (isValidColumn && isValidRow) {
					for (int i = 0; i < terrainMapWidth; i++) {
						for (int j = 0; j < terrainMapHeight; j++) {
							int matchingScreenIndex = Utilities.GetScreenIndexFromColAndRow(
								screen.Column + i,
								screen.Row + j
							);

							Screen matchingScreen = Game.Screens[matchingScreenIndex];

							if (matchingScreen.Region != null) {
								fitsHere = false;
							}

							if (terrainType == SettableTerrainType.Mountain) {
								if (screen.Row > 0) {
									fitsHere = false;
								}
							} else if (terrainType == SettableTerrainType.Desert) {
								if (matchingScreen.IsCoast ||
								    matchingScreen.IsLake
								) {
									fitsHere = false;
								}
							} else if (terrainType == SettableTerrainType.Lake) {
								if (matchingScreen.IsCoast ||
								    matchingScreen.IsLake
								) {
									fitsHere = false;
								}
							}
						}
					}
				} else {
					fitsHere = false;
				}

				if (fitsHere) {
					validPositions.Add(screenIndex);
				}
			}

			return validPositions;
		}

		private static List<Screen> ApplyTerrainMapToScreens(
			List<List<bool>> terrainMap,
			int startScreenIndex,
			SettableTerrainType terrainType = SettableTerrainType.Forest
		) {
			List<Screen> addedScreens = new List<Screen>();
			int terrainMapHeight = terrainMap.Count;
			int terrainMapWidth = terrainMap[0].Count;

			for (int i = 0; i < terrainMapWidth; i++) {
				for (int j = 0; j < terrainMapHeight; j++) {
					int matchingScreenIndex = Utilities.GetScreenIndexFromColAndRow(
						Game.Screens[startScreenIndex].Column + i,
						Game.Screens[startScreenIndex].Row + j
					);

					if (terrainMap[j][i]) {
						Screen screen = Game.Screens[matchingScreenIndex];
						addedScreens.Add(screen);

						if (terrainType == SettableTerrainType.Graveyard) {
							screen.IsGraveyard = true;
						} else if (terrainType == SettableTerrainType.Lake) {
							screen.IsLake = true;

							if (i == 0) {
								if (j == 0) {
									screen.LakeSpot = NineSliceSpot.TopLeft;
								} else if (j == terrainMapHeight - 1) {
									screen.LakeSpot = NineSliceSpot.BottomLeft;
								} else {
									screen.LakeSpot = NineSliceSpot.Left;
								}
							} else if (i == terrainMapWidth - 1) {
								if (j == 0) {
									screen.LakeSpot = NineSliceSpot.TopRight;
								} else if (j == terrainMapHeight - 1) {
									screen.LakeSpot = NineSliceSpot.BottomRight;
								} else {
									screen.LakeSpot = NineSliceSpot.Right;
								}
							} else {
								if (j == 0) {
									screen.LakeSpot = NineSliceSpot.Top;
								} else if (j == terrainMapHeight - 1) {
									screen.LakeSpot = NineSliceSpot.Bottom;
								} else {
									screen.LakeSpot = NineSliceSpot.Middle;
								}
							}
						}
					}
				}
			}

			return addedScreens;
		}

		private static void FillScreenWithBlankTiles(Screen screen) {
			screen.Tiles.Clear();

			for (int i = 0; i < Game.TilesHigh * Game.TilesWide; i++) {
				screen.Tiles.Add(Game.TileLookup[TileType.Ground]);
			}
		}
	}
}