using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.Common {
	public static class Utilities {
		private static Random _random;

		static Utilities() {
			SetSeed(GenerateRandomSeed());
			_random = new Random(FrontEnd.MainWindow.Seed);
		}

		public static int GenerateRandomSeed() {
			Random random = new Random();
			string runner = "";

			for (int i = 0; i < 8; i++) {
				if (i == 0) {
					runner += random.Next(1, 9).ToString();
				} else {
					runner += random.Next(0, 9).ToString();
				}
			}

			return int.Parse(runner);
		}

		public static void SetSeed(int seed) {
			FrontEnd.MainWindow.Seed = seed;
			_random = new Random(FrontEnd.MainWindow.Seed);
		}

		public static int GetRandomInt(int min, int max) {
			return _random.Next(min, max + 1);
		}

		public static bool ValidateStringIsHex(string value) {
			try {
				int.Parse(value, System.Globalization.NumberStyles.HexNumber);
			} catch {
				return false;
			}

			return true;
		}
		
		public static int GetIntFromHex(string value) {
			return int.Parse(value, System.Globalization.NumberStyles.HexNumber);
		}

		public static int GetIntFromBinary(string value) {
			return Convert.ToInt32(value, 2);
		}

		public static string GetBinaryFromInt(int value, int expandSize = 8) {
			string binaryValue = Convert.ToString(value, 2);

			while (binaryValue.Length < expandSize) {
				binaryValue = "0" + binaryValue;
			}

			return binaryValue;
		}

		public static string GetBinaryFromHex(string value, int expandSize = 8) {
			return GetBinaryFromInt(GetIntFromHex(value), expandSize);
		}

		public static string GetHexFromBinary(string value) {
			string hex = Convert.ToInt32(value, 2).ToString("X");
			if (hex.Length == 1) {
				hex = "0" + hex;
			}

			return hex;
		}

		public static string GetHexFromInt(int value) {
			return GetHexFromBinary(GetBinaryFromInt(value));
		}

		public static int GetRandomFromOddsTable(List<int> oddsTable) {
			int roll = GetRandomInt(0, 100);

			int runningTotal = 0;

			foreach (int selection in oddsTable) {
				if (roll <= runningTotal + selection) {
					return selection;
				}

				runningTotal += selection;
			}

			return oddsTable.Count - 1;
		}

		public static int GetScreenIndexFromColAndRow(int col, int row) {
			return Game.ScreensWide * row + col;
		}

		public static int GetTileByColAndRow(int col, int row) {
			return Game.TilesWide * row + col;
		}

		public static int GetColFromTileIndex(int index) {
			return index % Game.TilesWide;
		}

		public static int GetRowFromTileIndex(int index) {
			return (int) Math.Floor(index / (float) Game.TilesWide);
		}

		public static TileType GetTile(Screen screen, int tileIndex) {
			return Game.TileLookupById[screen.Tiles[tileIndex]];
		}

		public static TileType GetTileUp(Screen screen, int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);

			if (row == 0) {
				return TileType.CaveAlt;
			}

			int tileIndexOfUp = GetTileByColAndRow(col, row - 1);
			return Game.TileLookupById[screen.Tiles[tileIndexOfUp]];
		}

		public static TileType GetTileDown(Screen screen, int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);

			if (row == Game.LastTileRow) {
				return TileType.CaveAlt;
			}

			int tileIndexOfDown = GetTileByColAndRow(col, row + 1);
			return Game.TileLookupById[screen.Tiles[tileIndexOfDown]];
		}

		public static TileType GetTileRight(Screen screen, int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);

			if (col == Game.LastTileColumn) {
				return TileType.CaveAlt;
			}

			int tileIndexOfRight = GetTileByColAndRow(col + 1, row);
			return Game.TileLookupById[screen.Tiles[tileIndexOfRight]];
		}

		public static TileType GetTileLeft(Screen screen, int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);

			if (col == 0) {
				return TileType.CaveAlt;
			}

			int tileIndexOfLeft = GetTileByColAndRow(col - 1, row);
			return Game.TileLookupById[screen.Tiles[tileIndexOfLeft]];
		}

		public static bool IsBorderTile(int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);
			return row <= 1 || row >= Game.TilesHigh - 2 || col == 0 || col == Game.TilesWide - 1;
		}

		public static bool IsThickBorderTile(int tileIndex) {
			int row = GetRowFromTileIndex(tileIndex);
			int col = GetColFromTileIndex(tileIndex);
			return row <= 1 || row >= Game.TilesHigh - 2 || col <= 1 || col >= Game.TilesWide - 2;
		}

		public static bool EnemiesCanSpawnOnScreen(Screen screen) {
			List<TileType> groundTiles = new List<TileType> { TileType.Ground, TileType.Desert };

			bool crashesFromTop = true;
			List<Vector> fromTopSideSpawnSpots = new List<Vector> {
				new Vector(5, 4), new Vector(5, 8), new Vector(6, 6),
				new Vector(7, 5), new Vector(9, 6), new Vector(10, 4),
				new Vector(3, 7), new Vector(10, 8), new Vector(12, 7)
			};

			foreach (Vector spot in fromTopSideSpawnSpots) {
				int tileId = screen.Tiles[GetTileByColAndRow((int) spot.X, (int) spot.Y)];

				if (groundTiles.Contains(Game.TileLookupById[tileId])) {
					crashesFromTop = false;
					break;
				}
			}

			bool crashesFromLeft = true;
			List<Vector> fromLeftSideSpawnSpots = new List<Vector> {
				new Vector(5, 8), new Vector(8, 4), new Vector(8, 6),
				new Vector(10, 4), new Vector(10, 6), new Vector(12, 3),
				new Vector(5, 2), new Vector(12, 7), new Vector(13, 5)
			};

			foreach (Vector spot in fromLeftSideSpawnSpots) {
				int tileId = screen.Tiles[GetTileByColAndRow((int) spot.X, (int) spot.Y)];

				if (groundTiles.Contains(Game.TileLookupById[tileId])) {
					crashesFromLeft = false;
					break;
				}
			}

			bool crashesFromRight = true;
			List<Vector> fromRightSideSpawnSpots = new List<Vector> {
				new Vector(2, 5), new Vector(3, 3), new Vector(3, 7),
				new Vector(5, 4), new Vector(5, 6), new Vector(7, 4),
				new Vector(7, 6), new Vector(10, 2), new Vector(10, 8)
			};

			foreach (Vector spot in fromRightSideSpawnSpots) {
				int tileId = screen.Tiles[GetTileByColAndRow((int) spot.X, (int) spot.Y)];

				if (groundTiles.Contains(Game.TileLookupById[tileId])) {
					crashesFromRight = false;
					break;
				}
			}

			bool crashesFromBottom = true;
			List<Vector> fromBottomSideSpawnSpots = new List<Vector> {
				new Vector(6, 4), new Vector(5, 2), new Vector(3, 3),
				new Vector(5, 6), new Vector(8, 5), new Vector(9, 4),
				new Vector(10, 2), new Vector(10, 6), new Vector(12, 3)
			};

			foreach (Vector spot in fromBottomSideSpawnSpots) {
				int tileId = screen.Tiles[GetTileByColAndRow((int) spot.X, (int) spot.Y)];

				if (groundTiles.Contains(Game.TileLookupById[tileId])) {
					crashesFromBottom = false;
					break;
				}
			}

			return !crashesFromTop && !crashesFromLeft && !crashesFromRight && !crashesFromBottom;
		}

		public static int GetMin(int value, int min) {
			return value < min
				? min
				: value;
		}

		public static int GetMax(int value, int max) {
			return value > max
				? max
				: value;
		}

		public static List<int> GetShortestPathToTileType(
			Screen screen,
			int startTileIndex,
			List<TileType> walkableTiles,
			List<TileType> endTiles
		) {
			List<List<int>> routes = new List<List<int>>();

			for (int tileIndex = 0; tileIndex < screen.Tiles.Count; tileIndex++) {
				int row = GetRowFromTileIndex(tileIndex);
				int col = GetColFromTileIndex(tileIndex);

				if (!endTiles.Contains(GetTile(screen, tileIndex))) {
					continue;
				}

				List<PathingNode> nodes = new List<PathingNode>();

				for (int i = 0; i < screen.Tiles.Count; i++) {
					nodes.Add(new PathingNode { Id = i });
				}

				nodes[tileIndex].Status = PathingNodeStatus.Open;
				PathingNode targetNode = nodes[startTileIndex];

				while (true) {
					List<PathingNode> openNodes = nodes.Where(node => node.Status == PathingNodeStatus.Open).ToList();

					foreach (PathingNode node in openNodes) {
						node.Status = PathingNodeStatus.Closed;

						List<int> neighbors = new List<int>();

						if (row > 0 && walkableTiles.Contains(GetTileUp(screen, node.Id))) {
							neighbors.Add(node.Id - 16);
						}

						if (row < Game.LastTileRow && walkableTiles.Contains(GetTileDown(screen, node.Id))) {
							neighbors.Add(node.Id + 16);
						}

						if (col > 0 && walkableTiles.Contains(GetTileRight(screen, node.Id))) {
							neighbors.Add(node.Id + 1);
						}

						if (col < Game.LastTileColumn && walkableTiles.Contains(GetTileLeft(screen, node.Id))) {
							neighbors.Add(node.Id - 1);
						}

						foreach (int neighbor in neighbors) {
							PathingNode matchingNode = nodes[neighbor];
							if (matchingNode.Status == PathingNodeStatus.Unchecked) {
								matchingNode.Status = PathingNodeStatus.Open;
								matchingNode.ParentNode = node;
							}
						}
					}

					if (openNodes.Contains(targetNode) || openNodes.Count == 0) {
						break;
					}
				}

				if (targetNode.ParentNode != null) {
					PathingNode currentOnPath = targetNode;
					List<int> route = new List<int> { currentOnPath.Id };
					while (currentOnPath.ParentNode != null) {
						currentOnPath = currentOnPath.ParentNode;
						route.Add(currentOnPath.Id);
					}

					routes.Add(route);
				}
			}

			if (routes.Count > 0) {
				List<int> optimalRoute = routes[0];

				if (routes.Count > 1) {
					for (int routeIndex = 1; routeIndex < routes.Count; routeIndex++) {
						List<int> route = routes[routeIndex];
						if (route.Count < optimalRoute.Count) {
							optimalRoute = route;
						}
					}
				}

				return optimalRoute;
			}

			return new List<int>();
		}

		public static bool EveryGroundTileAccessible(Screen screen) {
			if (screen.ScreenId == 25) {
				int v = 4321;
				v++;
			}
			List<TileType> groundTileTypes = new List<TileType> {
				TileType.Ground, TileType.Desert, TileType.Bridge, TileType.BridgeVertical, TileType.Ladder
			};

			List<int> allGroundTiles = new List<int>();

			for (int tileIndex = 0; tileIndex < screen.Tiles.Count; tileIndex++) {
				TileType tile = Game.TileLookupById[screen.Tiles[tileIndex]];
				if (groundTileTypes.Contains(tile)) {
					allGroundTiles.Add(tileIndex);
				}
			}

			List<int> groundTilesTouching = new List<int>();
			List<int> toCheck = new List<int> { allGroundTiles[0] };

			while (toCheck.Count > 0) {
				List<int> newToCheck = new List<int>();

				foreach (int tileIndex in toCheck) {
					if (tileIndex == 138) {
						int jfdkj = 22;
						jfdkj++;
					}
					groundTilesTouching.Add(tileIndex);

					int col = GetColFromTileIndex(tileIndex);
					int row = GetRowFromTileIndex(tileIndex);
					bool isFirstCol = col == 0;
					bool isLastCol = col == Game.LastTileColumn;
					bool isFirstRow = row == 0;
					bool isLastRow = row == Game.LastTileRow;

					if (
						!isFirstCol &&
						allGroundTiles.Contains(tileIndex - 1) &&
						!groundTilesTouching.Contains(tileIndex - 1) &&
						!newToCheck.Contains(tileIndex - 1)
					) {
						newToCheck.Add(tileIndex - 1);
					}

					if (
						!isLastCol &&
						allGroundTiles.Contains(tileIndex + 1) &&
						!groundTilesTouching.Contains(tileIndex + 1) &&
						!newToCheck.Contains(tileIndex + 1)
					) {
						newToCheck.Add(tileIndex + 1);
					}

					if (
						!isFirstRow &&
						allGroundTiles.Contains(tileIndex - 16) &&
						!groundTilesTouching.Contains(tileIndex - 16) &&
						!newToCheck.Contains(tileIndex - 16)
					) {
						newToCheck.Add(tileIndex - 16);
					}

					if (
						!isLastRow &&
						allGroundTiles.Contains(tileIndex + 16) &&
						!groundTilesTouching.Contains(tileIndex + 16) &&
						!newToCheck.Contains(tileIndex + 16)
					) {
						newToCheck.Add(tileIndex + 16);
					}
				}

				toCheck = newToCheck;
			}

			groundTilesTouching.Sort();

			int expectedInaccessibleCount = 0;
			
			if (screen.IsAnyRoad) {
				expectedInaccessibleCount++;
			}
			
			return allGroundTiles.Count == groundTilesTouching.Count + expectedInaccessibleCount;
		}
	}
}