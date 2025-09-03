using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.MapBuilder;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class KakarikoBuilder : ScreenBuilder {
		public KakarikoBuilder(Screen screen) : base(screen) { }

		private const TileType RoadTile = TileType.Ground;
		private const int AbsoluteMinHouseWidth = 3;
		private const int StartingMinHouseWidth = 5;

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			FillGroundWithGrass();
			BuildEdgeTiles();
			DoubleTopAndBottomEdges();

			BuildGenericScreen();

			if (ShouldRebuild()) {
				Debug.Print("Rebuild Reason: Kakariko Fail");
				OverworldBuilder.ShouldRebuild = true;
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			bool isVerticalEdge = edge == Direction.Down || edge == Direction.Up;

			int edgeSize = isVerticalEdge
				? Game.TilesWide
				: Game.TilesHigh;

			if (
				isVerticalEdge && (Screen.HasWaterLeft || Screen.HasWaterRight) ||
				!isVerticalEdge && (Screen.HasWaterTop || Screen.HasWaterBottom)
			) {
				AssignConstructedEdgeForWater(edge);
				return;
			}

			int gapsCount = Utilities.GetRandomInt(1, 2);
			int gapWidth = gapsCount == 0
				? Utilities.GetRandomInt(3, 6)
				: Utilities.GetRandomInt(2, 3);

			while (gapWidth * gapsCount + 6 > edgeSize) {
				gapWidth--;
			}

			List<bool> edgeProfile = new List<bool>();

			for (int gap = 0; gap < gapsCount; gap++) {
				edgeProfile.Add(true);
				edgeProfile.Add(true);
				for (int i = 0; i < gapWidth; i++) {
					edgeProfile.Add(false);
				}
			}

			edgeProfile.Add(true);

			if (!isVerticalEdge) {
				edgeProfile.Add(true);
				edgeProfile.Insert(0, true);
			}

			while (edgeProfile.Count < edgeSize) {
				List<int> solidEdges = Enumerable.Range(0, edgeProfile.Count)
					.Where(i => edgeProfile[i])
					.ToList();

				edgeProfile.Insert(solidEdges[Utilities.GetRandomInt(0, solidEdges.Count - 1)], true);
			}

			if (edge == Direction.Up) {
				Screen.EdgeNorth = edgeProfile;
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = edgeProfile;
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = edgeProfile;
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = edgeProfile;
			}
		}

		private void AssignConstructedEdgeForWater(Direction edge) {
			if (edge == Direction.Up || edge == Direction.Down) {
				List<bool> edgeProfile = new List<bool>();

				if (Screen.HasWaterLeft) {
					for (int i = 0; i < Game.TilesWide; i++) {
						edgeProfile.Add(i < 9 || i > 11);
					}
				} else if (Screen.HasWaterRight) {
					for (int i = 0; i < Game.TilesWide; i++) {
						edgeProfile.Add(i < 4 || i > 6);
					}
				}

				if (edge == Direction.Up) {
					Screen.EdgeNorth = edgeProfile;
				} else {
					Screen.EdgeSouth = edgeProfile;
				}
			} else if (edge == Direction.Left || edge == Direction.Right) {
				List<bool> edgeProfile = new List<bool>();

				if (Screen.HasWaterTop) {
					for (int i = 0; i < Game.TilesHigh; i++) {
						edgeProfile.Add(i < 5 || i > 7);
					}
				} else if (Screen.HasWaterBottom) {
					for (int i = 0; i < Game.TilesHigh; i++) {
						edgeProfile.Add(i < 3 || i > 5);
					}
				}

				if (edge == Direction.Left) {
					Screen.EdgeWest = edgeProfile;
				} else {
					Screen.EdgeEast = edgeProfile;
				}
			}
		}

		private void FillGroundWithGrass() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Desert, 0, 0, 15, 10);
		}

		private void BuildEdgeTiles() {
			List<Biome> treeTypeBiomes = new List<Biome> {
				Biome.River, Biome.DenseForest, Biome.LightForest, Biome.GhostForest, Biome.Kakariko
			};

			WriteEdgeTilesToScreen(
				Direction.Up,
				Screen.GetScreenUp() != null && treeTypeBiomes.Contains(Screen.GetScreenUp().Region.Biome)
					? TileType.Tree
					: TileType.Rock
			);

			WriteEdgeTilesToScreen(
				Direction.Down,
				Screen.GetScreenDown() != null && treeTypeBiomes.Contains(Screen.GetScreenDown().Region.Biome)
					? TileType.Tree
					: TileType.Rock
			);

			WriteEdgeTilesToScreen(
				Direction.Left,
				Screen.GetScreenLeft() != null && treeTypeBiomes.Contains(Screen.GetScreenLeft().Region.Biome)
					? TileType.Tree
					: TileType.Rock
			);

			WriteEdgeTilesToScreen(
				Direction.Right,
				Screen.GetScreenRight() != null && treeTypeBiomes.Contains(Screen.GetScreenRight().Region.Biome)
					? TileType.Tree
					: TileType.Rock
			);
		}

		private void BuildGenericScreen() {
			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			if (Screen.IsLake && Screen.LakeSpot == NineSliceSpot.Middle) {
				TileDrawingTemplates.BuildCenterLakeRing(Screen);
			}

			if (Screen.IsDock) {
				TileDrawingTemplates.BuildDock(Screen);
			}

			BuildRoads();
			PlaceHouses();
			ConnectHouseDoorsToPath();
			BuildCaves();
			ReplaceDebugTiles();
		}

		private void BuildRoads() {
			List<int> roadStartTileWest = GetRoadStartIndexes(Screen.EdgeWest);
			List<int> roadStartTileEast = GetRoadStartIndexes(Screen.EdgeEast);
			List<int> roadStartTileNorth = GetRoadStartIndexes(Screen.EdgeNorth);
			List<int> roadStartTileSouth = GetRoadStartIndexes(Screen.EdgeSouth);

			if (Screen.TotalExits >= 3) {
				List<List<Direction>> adjacentCorners = new List<List<Direction>>();

				if (Screen.ExitsNorth) {
					if (Screen.ExitsWest) {
						adjacentCorners.Add(new List<Direction> { Direction.Up, Direction.Left });
					}

					if (Screen.ExitsEast) {
						adjacentCorners.Add(new List<Direction> { Direction.Up, Direction.Right });
					}
				}

				if (Screen.ExitsSouth) {
					if (Screen.ExitsWest) {
						adjacentCorners.Add(new List<Direction> { Direction.Down, Direction.Left });
					}

					if (Screen.ExitsEast) {
						adjacentCorners.Add(new List<Direction> { Direction.Down, Direction.Right });
					}
				}

				List<Direction> chosenCorners = adjacentCorners[Utilities.GetRandomInt(0, adjacentCorners.Count - 1)];

				BuildRoadBetweenAdjacentEdges(
					chosenCorners,
					roadStartTileNorth,
					roadStartTileSouth,
					roadStartTileWest,
					roadStartTileEast
				);

				BuildConnectingRoads(roadStartTileNorth, roadStartTileSouth, roadStartTileWest, roadStartTileEast);
			} else if (Screen.TotalExits == 2) {
				if (Screen.ExitsNorth && Screen.ExitsWest) {
					BuildRoadBetweenAdjacentEdges(
						new List<Direction> { Direction.Up, Direction.Left },
						roadStartTileNorth,
						roadStartTileSouth,
						roadStartTileWest,
						roadStartTileEast
					);
					BuildConnectingRoads(roadStartTileNorth, roadStartTileSouth, roadStartTileWest, roadStartTileEast);
				} else if (Screen.ExitsNorth && Screen.ExitsEast) {
					BuildRoadBetweenAdjacentEdges(
						new List<Direction> { Direction.Up, Direction.Right },
						roadStartTileNorth,
						roadStartTileSouth,
						roadStartTileWest,
						roadStartTileEast
					);
					BuildConnectingRoads(roadStartTileNorth, roadStartTileSouth, roadStartTileWest, roadStartTileEast);
				} else if (Screen.ExitsSouth && Screen.ExitsWest) {
					BuildRoadBetweenAdjacentEdges(
						new List<Direction> { Direction.Down, Direction.Left },
						roadStartTileNorth,
						roadStartTileSouth,
						roadStartTileWest,
						roadStartTileEast
					);
					BuildConnectingRoads(roadStartTileNorth, roadStartTileSouth, roadStartTileWest, roadStartTileEast);
				} else if (Screen.ExitsSouth && Screen.ExitsEast) {
					BuildRoadBetweenAdjacentEdges(
						new List<Direction> { Direction.Down, Direction.Right },
						roadStartTileNorth,
						roadStartTileSouth,
						roadStartTileWest,
						roadStartTileEast
					);
					BuildConnectingRoads(roadStartTileNorth, roadStartTileSouth, roadStartTileWest, roadStartTileEast);
				} else if (Screen.ExitsNorth && Screen.ExitsSouth) {
					int rowIndex = Utilities.GetRandomInt(5, 8);
					foreach (int colIndex in roadStartTileNorth) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, colIndex, 0, colIndex, rowIndex, TileType.Desert
						);
					}

					foreach (int colIndex in roadStartTileSouth) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, colIndex, rowIndex, colIndex, Game.TilesHigh - 1, TileType.Desert
						);
					}

					int firstNorth = roadStartTileNorth.First();
					int firstSouth = roadStartTileSouth.First();
					int lastNorth = roadStartTileNorth.Last();
					int lastSouth = roadStartTileSouth.Last();

					int left = firstNorth < firstSouth
						? firstNorth
						: firstSouth;

					int right = lastNorth > lastSouth
						? lastNorth
						: lastSouth;

					TileDrawing.FillRectWithTiles(
						Screen, RoadTile, left, rowIndex, right, rowIndex, TileType.Desert
					);
				} else if (Screen.ExitsWest && Screen.ExitsEast) {
					int colIndex = Utilities.GetRandomInt(5, 12);

					foreach (int rowIndex in roadStartTileWest) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, 0, rowIndex, colIndex, rowIndex, TileType.Desert
						);
					}

					foreach (int rowIndex in roadStartTileEast) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, colIndex, rowIndex, Game.TilesWide - 1, rowIndex, TileType.Desert
						);
					}

					int firstWest = roadStartTileWest.First();
					int firstEast = roadStartTileEast.First();
					int lastWest = roadStartTileWest.Last();
					int lastEast = roadStartTileEast.Last();

					int top = firstWest < firstEast
						? firstWest
						: firstEast;

					int bottom = lastWest > lastEast
						? lastWest
						: lastEast;

					TileDrawing.FillRectWithTiles(
						Screen, RoadTile, colIndex, top, colIndex, bottom, TileType.Desert
					);
				}
			} else if (Screen.TotalExits == 1) {
				if (Screen.ExitsNorth) {
					int crossRow = Utilities.GetRandomInt(4, 6);

					foreach (int colIndex in roadStartTileNorth) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, colIndex, 0, colIndex, crossRow, TileType.Desert
						);
					}

					if (roadStartTileNorth.Count > 1) {
						TileDrawing.FillRectWithTiles(
							Screen,
							RoadTile,
							roadStartTileNorth.First(),
							crossRow,
							roadStartTileNorth.Last(),
							crossRow,
							TileType.Desert
						);
					}
				} else if (Screen.ExitsSouth) {
					int crossRow = Utilities.GetRandomInt(4, 6);

					foreach (int colIndex in roadStartTileSouth) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, colIndex, Game.TilesHigh - 1, colIndex, crossRow, TileType.Desert
						);
					}

					if (roadStartTileSouth.Count > 1) {
						TileDrawing.FillRectWithTiles(
							Screen,
							RoadTile,
							roadStartTileSouth.First(),
							crossRow,
							roadStartTileSouth.Last(),
							crossRow,
							TileType.Desert
						);
					}
				} else if (Screen.ExitsWest) {
					int crossCol = Utilities.GetRandomInt(8, 12);

					foreach (int rowIndex in roadStartTileWest) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, 0, rowIndex, crossCol, rowIndex, TileType.Desert
						);
					}

					if (roadStartTileWest.Count > 1) {
						TileDrawing.FillRectWithTiles(
							Screen,
							RoadTile,
							crossCol,
							roadStartTileWest.First(),
							crossCol,
							roadStartTileWest.Last(),
							TileType.Desert
						);
					}
				} else if (Screen.ExitsEast) {
					int crossCol = Utilities.GetRandomInt(8, 12);

					foreach (int rowIndex in roadStartTileEast) {
						TileDrawing.FillRectWithTiles(
							Screen, RoadTile, Game.TilesWide - 1, rowIndex, crossCol, rowIndex, TileType.Desert
						);
					}

					if (roadStartTileEast.Count > 1) {
						TileDrawing.FillRectWithTiles(
							Screen,
							RoadTile,
							crossCol,
							roadStartTileEast.First(),
							crossCol,
							roadStartTileEast.Last(),
							TileType.Desert
						);
					}
				}
			}
		}

		private List<int> GetRoadStartIndexes(List<bool> edgeTiles) {
			List<int> roadStartTiles = new List<int>();
			for (int tileIndex = 0; tileIndex < edgeTiles.Count; tileIndex++) {
				bool isSolid = edgeTiles[tileIndex];

				if (!isSolid) {
					int gapStartIndex = tileIndex;
					int gapEndIndex = edgeTiles.Count - 1;

					for (int gapIndex = tileIndex; gapIndex < edgeTiles.Count - 1; gapIndex++) {
						bool nextIsSolid = edgeTiles[gapIndex + 1];
						if (nextIsSolid) {
							gapEndIndex = gapIndex;
							tileIndex = gapIndex + 1;
							break;
						}
					}

					roadStartTiles.Add((int) Math.Ceiling((gapEndIndex - gapStartIndex) / 2f) + gapStartIndex);
				}
			}

			return roadStartTiles;
		}

		private void BuildRoadBetweenAdjacentEdges(
			List<Direction> chosenCorners,
			List<int> roadStartTileNorth,
			List<int> roadStartTileSouth,
			List<int> roadStartTileWest,
			List<int> roadStartTileEast
		) {
			if (chosenCorners[0] == Direction.Up) {
				if (chosenCorners[1] == Direction.Left) {
					if (roadStartTileNorth.Count > 0 && roadStartTileWest.Count > 0) {
						int col = roadStartTileNorth.Last();
						int row = roadStartTileWest.Last();

						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, 0, col, row, TileType.Desert);
						TileDrawing.FillRectWithTiles(Screen, RoadTile, 0, row, col, row, TileType.Desert);
					}
				} else {
					if (roadStartTileNorth.Count > 0 && roadStartTileEast.Count > 0) {
						int col = roadStartTileNorth.First();
						int row = roadStartTileEast.Last();

						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, 0, col, row, TileType.Desert);
						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, row, Game.TilesWide - 1, row, TileType.Desert);
					}
				}
			} else if (chosenCorners[0] == Direction.Down) {
				if (chosenCorners[1] == Direction.Left) {
					if (roadStartTileSouth.Count > 0 && roadStartTileWest.Count > 0) {
						int col = roadStartTileSouth.Last();
						int row = roadStartTileWest.First();

						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, row, col, Game.TilesHigh - 1, TileType.Desert);
						TileDrawing.FillRectWithTiles(Screen, RoadTile, 0, row, col, row, TileType.Desert);
					}
				} else {
					if (roadStartTileSouth.Count > 0 && roadStartTileEast.Count > 0) {
						int col = roadStartTileSouth.First();
						int row = roadStartTileEast.First();

						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, row, col, Game.TilesHigh - 1,
							TileType.Desert);
						TileDrawing.FillRectWithTiles(Screen, RoadTile, col, row, Game.TilesWide - 1, row,
							TileType.Desert);
					}
				}
			}
		}

		private void BuildConnectingRoads(
			List<int> roadStartTileNorth,
			List<int> roadStartTileSouth,
			List<int> roadStartTileWest,
			List<int> roadStartTileEast
		) {
			for (int i = 0; i < 2; i++) {
				foreach (int colIndex in roadStartTileNorth) {
					int tileIndex = Utilities.GetTileByColAndRow(colIndex, 0);
					int endRow = Game.TilesHigh - 1;

					if (Screen.Tiles[tileIndex] != Game.TileLookup[RoadTile]) {
						for (int rowIndex = 0; rowIndex < Game.TilesHigh; rowIndex++) {
							tileIndex = Utilities.GetTileByColAndRow(colIndex, rowIndex);
							if (Screen.Tiles[tileIndex] == Game.TileLookup[RoadTile]) {
								endRow = rowIndex;
								break;
							}
						}

						if (endRow < Game.TilesHigh - 1) {
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, colIndex, 0, colIndex, endRow, TileType.Desert
							);
						} else {
							int closestWest = Game.TilesHigh - 1;
							int closestEast = Game.TilesHigh - 1;

							if (roadStartTileWest.Count > 0) {
								closestWest = roadStartTileWest.First();
							}

							if (roadStartTileEast.Count > 0) {
								closestEast = roadStartTileEast.First();
							}

							endRow = closestWest < closestEast
								? closestWest
								: closestEast;

							int endCol = closestWest < closestEast
								? 0
								: Game.TilesWide - 1;

							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, colIndex, 0, colIndex, endRow, TileType.Desert
							);
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, colIndex, endRow, endCol, endRow, TileType.Desert
							);
						}
					}
				}

				foreach (int colIndex in roadStartTileSouth) {
					int tileIndex = Utilities.GetTileByColAndRow(colIndex, Game.TilesHigh - 1);
					int endRow = 0;

					if (Screen.Tiles[tileIndex] != Game.TileLookup[RoadTile]) {
						for (int rowIndex = Game.TilesHigh - 1; rowIndex >= 0; rowIndex--) {
							tileIndex = Utilities.GetTileByColAndRow(colIndex, rowIndex);
							if (Screen.Tiles[tileIndex] == Game.TileLookup[RoadTile]) {
								endRow = rowIndex;
								break;
							}
						}

						if (endRow > 0) {
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, colIndex, endRow, colIndex, Game.TilesHigh - 1, TileType.Desert
							);
						} else {
							int closestWest = 0;
							int closestEast = 0;

							if (roadStartTileWest.Count > 0) {
								closestWest = roadStartTileWest.Last();
							}

							if (roadStartTileEast.Count > 0) {
								closestEast = roadStartTileEast.Last();
							}

							endRow = closestWest > closestEast
								? closestWest
								: closestEast;

							int endCol = closestWest > closestEast
								? 0
								: Game.TilesWide - 1;

							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, colIndex, endRow, colIndex, Game.TilesHigh - 1, TileType.Desert
							);
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, endCol, endRow, colIndex, endRow, TileType.Desert
							);
						}
					}
				}

				foreach (int rowIndex in roadStartTileWest) {
					int tileIndex = Utilities.GetTileByColAndRow(0, rowIndex);
					int endCol = Game.TilesWide - 1;

					if (Screen.Tiles[tileIndex] != Game.TileLookup[RoadTile]) {
						for (int colIndex = 0; colIndex < Game.TilesWide; colIndex++) {
							tileIndex = Utilities.GetTileByColAndRow(colIndex, rowIndex);
							if (Screen.Tiles[tileIndex] == Game.TileLookup[RoadTile]) {
								endCol = colIndex;
								break;
							}
						}

						if (endCol < Game.TilesWide - 1) {
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, 0, rowIndex, endCol, rowIndex, TileType.Desert
							);
						} else {
							int closestNorth = Game.TilesWide - 1;
							int closestSouth = Game.TilesWide - 1;

							if (roadStartTileNorth.Count > 0) {
								closestNorth = roadStartTileNorth.First();
							}

							if (roadStartTileSouth.Count > 0) {
								closestSouth = roadStartTileSouth.First();
							}

							endCol = closestNorth < closestSouth
								? closestNorth
								: closestSouth;

							int endRow = closestNorth < closestSouth
								? 0
								: Game.TilesHigh - 1;

							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, 0, rowIndex, endCol, rowIndex, TileType.Desert
							);
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, endCol, rowIndex, endCol, endRow, TileType.Desert
							);
						}
					}
				}

				foreach (int rowIndex in roadStartTileEast) {
					int tileIndex = Utilities.GetTileByColAndRow(Game.TilesWide - 1, rowIndex);
					int endCol = 0;

					if (Screen.Tiles[tileIndex] != Game.TileLookup[RoadTile]) {
						for (int colIndex = Game.TilesWide - 1; colIndex >= 0; colIndex--) {
							tileIndex = Utilities.GetTileByColAndRow(colIndex, rowIndex);
							if (Screen.Tiles[tileIndex] == Game.TileLookup[RoadTile]) {
								endCol = colIndex;
								break;
							}
						}

						if (endCol > 0) {
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, endCol, rowIndex, Game.TilesWide - 1, rowIndex, TileType.Desert
							);
						} else {
							int closestNorth = 0;
							int closestSouth = 0;

							if (roadStartTileNorth.Count > 0) {
								closestNorth = roadStartTileNorth.Last();
							}

							if (roadStartTileSouth.Count > 0) {
								closestSouth = roadStartTileSouth.Last();
							}

							endCol = closestNorth > closestSouth
								? closestNorth
								: closestSouth;

							int endRow = closestNorth > closestSouth
								? 0
								: Game.TilesHigh - 1;

							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, endCol, rowIndex, Game.TilesWide - 1, rowIndex, TileType.Desert
							);
							TileDrawing.FillRectWithTiles(
								Screen, RoadTile, endCol, rowIndex, endCol, endRow, TileType.Desert
							);
						}
					}
				}
			}
		}

		private struct HousePlacement {
			public int TileIndex;
			public int Width;
		}

		private void PlaceHouses() {
			List<TileType> lawnTiles = new List<TileType> {
				TileType.Desert, TileType.Ground
			};

			List<TileType> houseTiles = new List<TileType> {
				TileType.KakarikoHouseDoor, TileType.KakarikoHouseFront,
				TileType.KakarikoHouseTopLeft, TileType.KakarikoHouseTopMiddle, TileType.KakarikoHouseTopRight,
				TileType.KakarikoHouseBottomLeft, TileType.KakarikoHouseBottomMiddle, TileType.KakarikoHouseBottomRight
			};

			List<TileType> placeOverTiles = new List<TileType> { TileType.Desert, TileType.Tree, TileType.Rock };
			bool canStillFitHouses = true;

			int minHouseWidth = StartingMinHouseWidth;

			while (canStillFitHouses) {
				List<HousePlacement> housePlacements = new List<HousePlacement>();

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					int startCol = Utilities.GetColFromTileIndex(tileIndex);
					int startRow = Utilities.GetRowFromTileIndex(tileIndex);

					bool isValidStartingSpot = startCol > (Screen.ExitsWest ? 1 : 0) &&
					                           startCol < Game.TilesWide - 4 &&
					                           startRow > 0 &&
					                           startRow < Game.TilesHigh - 5;

					if (isValidStartingSpot) {
						if (placeOverTiles.Contains(Utilities.GetTile(Screen, tileIndex))) {
							int maxWidth = 0;

							for (int checkedCol = startCol; checkedCol <= Game.LastTileColumn; checkedCol++) {
								int checkedTileRowUp = Utilities.GetTileByColAndRow(checkedCol, startRow - 1);
								int checkedTileRow0 = Utilities.GetTileByColAndRow(checkedCol, startRow);
								int checkedTileRow1 = Utilities.GetTileByColAndRow(checkedCol, startRow + 1);
								int checkedTileRow2 = Utilities.GetTileByColAndRow(checkedCol, startRow + 2);
								int checkedTileRow3 = Utilities.GetTileByColAndRow(checkedCol, startRow + 3);

								bool columnIsValid =
									!(startRow == 1 && Screen.Tiles[checkedTileRowUp] ==
										Game.TileLookup[TileType.Desert]) &&
									!houseTiles.Contains(Game.TileLookupById[Screen.Tiles[checkedTileRowUp]]) &&
									placeOverTiles.Contains(Utilities.GetTile(Screen, checkedTileRow0)) &&
									placeOverTiles.Contains(Utilities.GetTile(Screen, checkedTileRow1)) &&
									placeOverTiles.Contains(Utilities.GetTile(Screen, checkedTileRow2)) &&
									lawnTiles.Contains(Game.TileLookupById[Screen.Tiles[checkedTileRow3]]);

								int lastAcceptableColumn = Screen.ExitsEast
									? Game.LastTileColumn - 1
									: Game.LastTileColumn;

								if (!columnIsValid || checkedCol == lastAcceptableColumn) {
									maxWidth = checkedCol - startCol;
									break;
								}
							}

							if (maxWidth >= minHouseWidth) {
								housePlacements.Add(new HousePlacement {
									TileIndex = tileIndex,
									Width = maxWidth
								});
							}
						}
					}
				}

				if (housePlacements.Count > 0) {
					int selectedHousePlacementIndex = Utilities.GetRandomInt(0, housePlacements.Count - 1);
					HousePlacement housePlacement = housePlacements[selectedHousePlacementIndex];
					housePlacement.Width = Utilities.GetRandomInt(minHouseWidth, housePlacement.Width);

					TileDrawingTemplates.PlaceHouse(
						Screen,
						Utilities.GetColFromTileIndex(housePlacement.TileIndex),
						Utilities.GetRowFromTileIndex(housePlacement.TileIndex),
						housePlacement.Width
					);

					BuildYard(housePlacement);
				} else {
					if (minHouseWidth == AbsoluteMinHouseWidth) {
						canStillFitHouses = false;
					} else {
						minHouseWidth--;
					}
				}
			}
		}

		private void BuildYard(HousePlacement housePlacement) {
			int startCol = Utilities.GetColFromTileIndex(housePlacement.TileIndex);
			int startRow = Utilities.GetRowFromTileIndex(housePlacement.TileIndex) + 3;

			List<int> heights = new List<int>();
			bool shouldPullBackYardByOne = false;

			for (int currentCol = startCol; currentCol < startCol + housePlacement.Width; currentCol++) {
				int height = 0;

				while (true) {
					if (startRow + height > Game.LastTileRow) {
						heights.Add(height);
						break;
					}

					TileType tileType =
						Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(currentCol, startRow + height));

					if (tileType == TileType.Desert) {
						height++;
					} else {
						if (tileType != RoadTile) {
							shouldPullBackYardByOne = true;
						}

						heights.Add(height);
						break;
					}
				}
			}

			int doorColumn = 0;
			for (int columnIndex = startCol; columnIndex < startCol + housePlacement.Width; columnIndex++) {
				if (Utilities.GetTileUp(Screen, Utilities.GetTileByColAndRow(columnIndex, startRow)) ==
				    TileType.KakarikoHouseDoor) {
					doorColumn = columnIndex;
				}
			}

			heights.Sort();

			if (heights.Count > 0 && heights.First() > 0) {
				int finalHeight = heights.First();
				if (shouldPullBackYardByOne) {
					finalHeight--;
				}

				while (startRow + finalHeight > Game.TilesHigh - 2) {
					finalHeight--;
				}

				if (finalHeight > 3) {
					finalHeight = 3;
				}

				for (int colIndex = startCol; colIndex < startCol + housePlacement.Width; colIndex++) {
					for (int rowIndex = startRow; rowIndex < startRow + finalHeight; rowIndex++) {
						if (colIndex == startCol || colIndex == startCol + housePlacement.Width - 1) {
							TileDrawing.DrawTile(Screen, TileType.KakarikoPost, colIndex, rowIndex);
						} else if (
							rowIndex == startRow &&
							Utilities.GetTileUp(Screen, Utilities.GetTileByColAndRow(colIndex, rowIndex)) ==
							TileType.KakarikoHouseFront
						) {
							TileDrawing.DrawTile(Screen, TileType.Tree, colIndex, rowIndex);
						} else if (colIndex != doorColumn) {
							TileDrawing.DrawTile(Screen, TileType.Debug, colIndex, rowIndex);
						}
					}
				}
			}
		}

		private void ConnectHouseDoorsToPath() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				TileType tileType = Utilities.GetTile(Screen, tileIndex);
				TileType doorstepType = Utilities.GetTileDown(Screen, tileIndex);

				if (tileType == TileType.KakarikoHouseDoor && doorstepType == TileType.Desert) {
					List<int> route = Utilities.GetShortestPathToTileType(
						Screen,
						tileIndex + 16,
						new List<TileType> { TileType.Desert },
						new List<TileType> { TileType.Ground }
					);

					foreach (int routeTileIndex in route) {
						Screen.Tiles[routeTileIndex] = Game.TileLookup[RoadTile];
					}
				}
			}
		}

		private void BuildCaves() {
			if (Screen.CaveDestination > 0) {
				List<int> doorIndexes = new List<int>();

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					if (Screen.Tiles[tileIndex] == Game.TileLookup[TileType.KakarikoHouseDoor]) {
						doorIndexes.Add(tileIndex);
					}
				}

				if (doorIndexes.Count == 0) {
					Debug.Print("Rebuild Reason: Kakariko door fail");
					OverworldBuilder.ShouldRebuild = true;
				} else {
					int caveIndex = doorIndexes[Utilities.GetRandomInt(0, doorIndexes.Count - 1)];

					Screen.Tiles[caveIndex] = Game.TileLookup[Screen.CaveIsHidden ? TileType.RockBombWall : TileType.Cave];
					int row = Utilities.GetRowFromTileIndex(caveIndex);
					int col = Utilities.GetColFromTileIndex(caveIndex);
					Screen.ExitCavePositionX = col;
					Screen.ExitCavePositionY = row - 1;
				}
			}
		}

		private void ReplaceDebugTiles() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				if (Utilities.GetTile(Screen, tileIndex) == TileType.Debug) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Desert];
				}
			}
		}

		private bool ShouldRebuild() {
			if (!Utilities.EveryGroundTileAccessible(Screen)) {
				return true;
			}

			int totalHouses = 0;
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				if (Utilities.GetTile(Screen, tileIndex) == TileType.KakarikoHouseTopLeft) {
					totalHouses++;
				}
			}

			return totalHouses == 0;
		}

		public override void AssignEnemies() {
			int enemySet = Utilities.GetRandomInt(0, 2);

			if (enemySet == 0) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 1) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinBlue];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_MoblinBlue
				];
				Screen.UsesMixedEnemies = true;
			}

			Screen.EnemiesEnterFromSides = false;
		}
	}
}