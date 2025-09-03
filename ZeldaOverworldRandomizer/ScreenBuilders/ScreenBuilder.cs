using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.MapBuilder;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class ScreenBuilder {
		protected readonly Screen Screen;

		public ScreenBuilder(Screen screen) {
			Screen = screen;
		}

		public virtual void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Desert);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Desert);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Desert);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Desert);
			DoubleTopAndBottomEdges();
		}

		public virtual void AssignEnemies() {
			Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.One];
			Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.None];
			Screen.EnemiesEnterFromSides = false;
			Screen.UsesMixedEnemies = false;
		}

		protected void AssignEdges() {
			if (Screen.ExitsNorth) {
				Screen screenNorth = Screen.GetScreenUp();
				if (screenNorth != null && screenNorth.IsAlreadyGenerated) {
					AssignCloneEdge(screenNorth, Direction.Up);
				} else {
					AssignConstructedEdge(Direction.Up);
				}
			} else {
				AssignSolidEdge(Direction.Up);
			}

			if (Screen.ExitsSouth) {
				Screen screenSouth = Screen.GetScreenDown();
				if (screenSouth != null && screenSouth.IsAlreadyGenerated) {
					AssignCloneEdge(screenSouth, Direction.Down);
				} else {
					AssignConstructedEdge(Direction.Down);
				}
			} else {
				AssignSolidEdge(Direction.Down);
			}

			if (Screen.ExitsEast) {
				Screen screenEast = Screen.GetScreenRight();
				if (screenEast != null && screenEast.IsAlreadyGenerated) {
					AssignCloneEdge(screenEast, Direction.Right);
				} else {
					AssignConstructedEdge(Direction.Right);
				}
			} else {
				AssignSolidEdge(Direction.Right);
			}

			if (Screen.ExitsWest) {
				Screen screenWest = Screen.GetScreenLeft();
				if (screenWest != null && screenWest.IsAlreadyGenerated) {
					AssignCloneEdge(screenWest, Direction.Left);
				} else {
					AssignConstructedEdge(Direction.Left);
				}
			} else {
				AssignSolidEdge(Direction.Left);
			}
		}

		protected void AssignCloneEdge(Screen otherScreen, Direction edge) {
			if (edge == Direction.Up) {
				Screen.EdgeNorth = new List<bool>(otherScreen.EdgeSouth);
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = new List<bool>(otherScreen.EdgeNorth);
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = new List<bool>(otherScreen.EdgeWest);
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = new List<bool>(otherScreen.EdgeEast);
			}
		}

		protected void AssignSolidEdge(Direction edge) {
			if (edge == Direction.Up) {
				Screen.EdgeNorth = GetSolidFilledEdgeTiles(true);
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = GetSolidFilledEdgeTiles(true);
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = GetSolidFilledEdgeTiles(false);
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = GetSolidFilledEdgeTiles(false);
			}
		}

		protected static List<bool> GetSolidFilledEdgeTiles(bool isVerticalEdge) {
			List<bool> tiles = new List<bool>();
			int length = isVerticalEdge ? Game.TilesWide : Game.TilesHigh;

			for (int i = 0; i < length; i++) {
				tiles.Add(true);
			}

			return tiles;
		}

		protected virtual void AssignConstructedEdge(Direction edge) {
			if (edge == Direction.Up) {
				Screen.EdgeNorth = new List<bool> {
					true, true, true, true, true, false, false, false,
					false, false, false, true, true, true, true, true,
				};
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = new List<bool> {
					true, true, true, true, true, false, false, false,
					false, false, false, true, true, true, true, true,
				};
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = new List<bool>
					{ true, true, true, true, false, false, false, true, true, true, true };
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = new List<bool>
					{ true, true, true, true, false, false, false, true, true, true, true };
			}
		}

		protected void WriteEdgeTilesToScreen(Direction edge, TileType tileType) {
			List<bool> edgeTiles = new List<bool>();
			if (edge == Direction.Up) {
				edgeTiles = Screen.EdgeNorth;
			}

			if (edge == Direction.Down) {
				edgeTiles = Screen.EdgeSouth;
			}

			if (edge == Direction.Left) {
				edgeTiles = Screen.EdgeWest;
			}

			if (edge == Direction.Right) {
				edgeTiles = Screen.EdgeEast;
			}

			for (int index = 0; index < edgeTiles.Count; index++) {
				if (edgeTiles[index]) {
					int tileIndex = 0;

					if (edge == Direction.Up) {
						tileIndex = Utilities.GetTileByColAndRow(index, 0);
					} else if (edge == Direction.Down) {
						tileIndex = Utilities.GetTileByColAndRow(index, Game.TilesHigh - 1);
					} else if (edge == Direction.Right) {
						tileIndex = Utilities.GetTileByColAndRow(Game.TilesWide - 1, index);
					} else if (edge == Direction.Left) {
						tileIndex = Utilities.GetTileByColAndRow(0, index);
					}

					Screen.Tiles[tileIndex] = Game.TileLookup[tileType];
				}
			}
		}

		protected void DoubleTopAndBottomEdges() {
			TileDrawing.CloneTileRow(Screen, 0, 1);
			TileDrawing.CloneTileRow(Screen, 10, 9);
		}

		protected void CleanSoloRocks() {
			List<TileType> actsLikeGround = new List<TileType> {
				TileType.Ground, TileType.Water, TileType.Bridge, TileType.BridgeVertical, TileType.Desert
			};

			bool shouldStillClean = true;

			while (shouldStillClean) {
				shouldStillClean = false;

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					if (!Utilities.IsBorderTile(tileIndex)) {
						TileType tileThis = Utilities.GetTile(Screen, tileIndex);
						TileType tileUp = Utilities.GetTileUp(Screen, tileIndex);
						TileType tileDown = Utilities.GetTileDown(Screen, tileIndex);
						TileType tileLeft = Utilities.GetTileLeft(Screen, tileIndex);
						TileType tileRight = Utilities.GetTileRight(Screen, tileIndex);

						bool isHorizontalLoner = tileThis == TileType.Rock &&
						                         actsLikeGround.Contains(tileLeft) &&
						                         actsLikeGround.Contains(tileRight);

						bool isVerticalLoner = tileThis == TileType.Rock &&
						                       actsLikeGround.Contains(tileUp) &&
						                       actsLikeGround.Contains(tileDown);

						if (isHorizontalLoner || isVerticalLoner) {
							Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ground];
							shouldStillClean = true;
						}
					}
				}
			}
		}

		protected void AddCaveEntrance() {
			List<int> existingCaves = GetExistingCaveIndexes();

			if (existingCaves.Count > 0) {
				return;
			}

			if (!Screen.CaveIsHidden && !Screen.IsOpenDungeon) {
				List<int> rockCaveIndexes = GetRockCaveIndexes();
				if (rockCaveIndexes.Count > 0) {
					AddRockCaveEntrance(rockCaveIndexes);
				} else {
					Screen.IsRebuildable = true;
					Debug.Print("Rebuild Reason: not enough available cave indexes on a screen");
					OverworldBuilder.ShouldRebuild = true;
				}
			} else {
				if (Screen.IsWhistleLake && Game.OverworldWhistleSecretScreens.IndexOf(128) >= 0) {
					Screen.PushedStairsPositionId = 1;
					Screen.ExitCavePositionX = 7;
					Screen.ExitCavePositionY = 1;

					int index = Game.OverworldWhistleSecretScreens.IndexOf(128);
					Game.OverworldWhistleSecretScreens[index] = Screen.ScreenId;
				} else {
					Screen.IsWhistleLake = false;
					List<int> bombableWallIndexes = GetRockCaveIndexes();
					List<int> burnableTreeIndexes = GetBurnableTreeIndexes();
					List<int> pushableBoulderIndexes = GetPushableBoulderIndexes();

					List<TileType> caveOptions = new List<TileType>();

					if (bombableWallIndexes.Count > 0) {
						caveOptions.Add(TileType.RockBombWall);
					}

					if (burnableTreeIndexes.Count > 0) {
						caveOptions.Add(TileType.TreeBurnable);
					}

					if (!Game.DungeonCaves.Contains(Game.CaveLookupById[Screen.CaveDestination])) {
						if (pushableBoulderIndexes.Count > 0) {
							caveOptions.Add(TileType.BoulderPushable);
						}
					}

					if (caveOptions.Count > 0) {
						TileType caveOption = caveOptions[Utilities.GetRandomInt(0, caveOptions.Count - 1)];

						if (caveOption == TileType.RockBombWall) {
							AddRockCaveEntrance(bombableWallIndexes);
						} else if (caveOption == TileType.TreeBurnable) {
							AddBurnableTree(burnableTreeIndexes);
						} else {
							AddPushableBoulder(pushableBoulderIndexes);
						}
					} else {
						Screen.IsRebuildable = true;
						Debug.Print("Rebuild Reason: Not enough cave spots");
						OverworldBuilder.ShouldRebuild = true;
					}
				}
			}
		}

		private List<int> GetExistingCaveIndexes() {
			List<int> validTileIndexes = new List<int>();

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				if (
					Utilities.GetTile(Screen, tileIndex) == TileType.BoulderPushable ||
					Utilities.GetTile(Screen, tileIndex) == TileType.TreeBurnable ||
					Utilities.GetTile(Screen, tileIndex) == TileType.RockBombWall ||
					Utilities.GetTile(Screen, tileIndex) == TileType.Cave ||
					Utilities.GetTile(Screen, tileIndex) == TileType.StaircaseDown
				) {
					validTileIndexes.Add(tileIndex);
				}
			}

			return validTileIndexes;
		}

		private List<int> GetRockCaveIndexes() {
			List<int> validTileIndexes = new List<int>();

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (row > 0 && col > 0 && row < Game.TilesHigh - 2 && col < Game.TilesWide - 1) {
					TileType tileUp = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(col, row - 1)]];
					TileType tileDown = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(col, row + 1)]];
					TileType tileLeft = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)]];
					TileType tileRight = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)]];

					if (Screen.Tiles[tileIndex] == Game.TileLookup[TileType.Rock] &&
					    tileUp == TileType.Rock &&
					    tileDown == TileType.Ground &&
					    tileLeft == TileType.Rock &&
					    tileRight == TileType.Rock
					) {
						if (Game.DungeonCaves.Contains(Game.CaveLookupById[Screen.CaveDestination])) {
							if (col >= 5 && col <= 10) {
								validTileIndexes.Add(tileIndex);
							}
						} else {
							validTileIndexes.Add(tileIndex);
						}
					}
				}
			}

			return validTileIndexes;
		}

		private List<int> GetBurnableTreeIndexes() {
			List<int> validTileIndexes = new List<int>();

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (row > 1 && row < Game.LastTileRow - 1 && col > 1 && col < Game.LastTileColumn) {
					List<TileType> groundTiles = new List<TileType> { TileType.Ground, TileType.Desert };

					bool bothUp = row > 2 &&
					              groundTiles.Contains(Utilities.GetTileUp(Screen, tileIndex)) &&
					              groundTiles.Contains(Utilities.GetTileUp(Screen, tileIndex - Game.TilesWide));

					bool bothDown = row < Game.LastTileRow - 2 &&
					                groundTiles.Contains(Utilities.GetTileDown(Screen, tileIndex)) &&
					                groundTiles.Contains(Utilities.GetTileDown(Screen, tileIndex + Game.TilesWide));

					bool bothLeft = col > 2 &&
					                groundTiles.Contains(Utilities.GetTile(Screen, tileIndex - 1)) &&
					                groundTiles.Contains(Utilities.GetTile(Screen, tileIndex - 2));

					bool bothRight = col < Game.LastTileColumn - 2 &&
					                 groundTiles.Contains(Utilities.GetTile(Screen, tileIndex + 1)) &&
					                 groundTiles.Contains(Utilities.GetTile(Screen, tileIndex + 2));

					if (Game.TileLookupById[Screen.Tiles[tileIndex]] == TileType.Tree &&
					    (bothUp || bothDown || bothLeft || bothRight)
					) {
						if (Game.DungeonCaves.Contains(Game.CaveLookupById[Screen.CaveDestination])) {
							if (col >= 5 && col <= 10) {
								validTileIndexes.Add(tileIndex);
							}
						} else {
							validTileIndexes.Add(tileIndex);
						}
					}
				}
			}

			return validTileIndexes;
		}

		private List<int> GetPushableBoulderIndexes() {
			List<TileType> groundTiles = new List<TileType> { TileType.Debug, TileType.Desert, TileType.Ground };

			List<int> validTileIndexes = new List<int>();
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (!Utilities.IsThickBorderTile(tileIndex) &&
				    Utilities.GetTile(Screen, tileIndex) == TileType.Boulder &&
				    groundTiles.Contains(Utilities.GetTileUp(Screen, tileIndex)) &&
				    groundTiles.Contains(Utilities.GetTileDown(Screen, tileIndex))
				) {
					if (Game.DungeonCaves.Contains(Game.CaveLookupById[Screen.CaveDestination])) {
						if (col >= 5 && col <= 10) {
							validTileIndexes.Add(tileIndex);
						}
					} else {
						validTileIndexes.Add(tileIndex);
					}
				}
			}

			return validTileIndexes;
		}

		private void AddRockCaveEntrance(List<int> validTileIndexes) {
			if (validTileIndexes.Count > 0) {
				int indexOfCave = validTileIndexes[Utilities.GetRandomInt(0, validTileIndexes.Count - 1)];

				if (Screen.CaveIsHidden) {
					Screen.Tiles[indexOfCave] = Game.TileLookup[TileType.RockBombWall];
				} else {
					Screen.Tiles[indexOfCave] = Game.TileLookup[TileType.Cave];
				}

				int row = Utilities.GetRowFromTileIndex(indexOfCave);
				int col = Utilities.GetColFromTileIndex(indexOfCave);
				Screen.ExitCavePositionX = col;
				Screen.ExitCavePositionY = row - 1;
			}
		}

		private void AddBurnableTree(List<int> validTileIndexes) {
			int chosenIndex = validTileIndexes[Utilities.GetRandomInt(0, validTileIndexes.Count - 1)];
			int col = Utilities.GetColFromTileIndex(chosenIndex);
			int row = Utilities.GetRowFromTileIndex(chosenIndex);

			Screen.Tiles[chosenIndex] = Game.TileLookup[TileType.TreeBurnable];

			List<TileType> groundTiles = new List<TileType> { TileType.Ground, TileType.Desert };
			List<Direction> validExitDirections = new List<Direction>();

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(col, row - 1)))) {
				validExitDirections.Add(Direction.Up);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(col, row + 1)))) {
				validExitDirections.Add(Direction.Down);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(col - 1, row)))) {
				validExitDirections.Add(Direction.Left);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(col + 1, row)))) {
				validExitDirections.Add(Direction.Right);
			}

			Direction exitDirection = validExitDirections[Utilities.GetRandomInt(0, validExitDirections.Count - 1)];

			if (exitDirection == Direction.Up) {
				Screen.ExitCavePositionX = col;
				Screen.ExitCavePositionY = row - 2;
			}

			if (exitDirection == Direction.Down) {
				Screen.ExitCavePositionX = col;
				Screen.ExitCavePositionY = row;
			}

			if (exitDirection == Direction.Right) {
				Screen.ExitCavePositionX = col + 1;
				Screen.ExitCavePositionY = row - 1;
			}

			if (exitDirection == Direction.Left) {
				Screen.ExitCavePositionX = col - 1;
				Screen.ExitCavePositionY = row - 1;
			}
		}

		private void AddPushableBoulder(List<int> validTileIndexes) {
			List<TileType> groundTiles = new List<TileType> { TileType.Debug, TileType.Desert, TileType.Ground };
			int pushableBoulderIndex = validTileIndexes[Utilities.GetRandomInt(0, validTileIndexes.Count - 1)];
			Screen.Tiles[pushableBoulderIndex] = Game.TileLookup[TileType.BoulderPushable];

			List<int> validStairsEntrances = new List<int>();

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(5, 3)))) {
				validStairsEntrances.Add(0);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(4, 5)))) {
				validStairsEntrances.Add(1);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(9, 5)))) {
				validStairsEntrances.Add(2);
			}

			if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(6, 5)))) {
				validStairsEntrances.Add(3);
			}

			if (validStairsEntrances.Count > 0) {
				Screen.PushedStairsPositionId =
					validStairsEntrances[Utilities.GetRandomInt(0, validStairsEntrances.Count - 1)];

				int x = 0;
				int y = 0;

				if (Screen.PushedStairsPositionId == 0) {
					x = 5;
					y = 3;
				} else if (Screen.PushedStairsPositionId == 1) {
					x = 4;
					y = 5;
				} else if (Screen.PushedStairsPositionId == 2) {
					x = 9;
					y = 5;
				} else if (Screen.PushedStairsPositionId == 3) {
					x = 6;
					y = 5;
				}

				List<int> validExitSpots = new List<int>();

				if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(x - 1, y)))) {
					validExitSpots.Add(Utilities.GetTileByColAndRow(x - 1, y - 1));
				}

				if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(x + 1, y)))) {
					validExitSpots.Add(Utilities.GetTileByColAndRow(x + 1, y - 1));
				}

				if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(x, y - 1)))) {
					validExitSpots.Add(Utilities.GetTileByColAndRow(x, y - 2));
				}

				if (groundTiles.Contains(Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(x, y + 1)))) {
					validExitSpots.Add(Utilities.GetTileByColAndRow(x, y));
				}

				if (validExitSpots.Count > 0) {
					int validExitSpot = validExitSpots[Utilities.GetRandomInt(0, validExitSpots.Count - 1)];
					Screen.ExitCavePositionX = Utilities.GetColFromTileIndex(validExitSpot);
					Screen.ExitCavePositionY = Utilities.GetRowFromTileIndex(validExitSpot);
				}
			}
		}

		protected void AddCorners(TileType tileType) {
			List<TileType> countsAsGround = new List<TileType> {
				TileType.Debug, TileType.Desert, TileType.Ground, TileType.BlackFloor
			};
			List<TileType> protectedFronts = new List<TileType> {
				TileType.Bridge, TileType.BridgeVertical, TileType.Cave, TileType.WaterfallCave
			};

			bool goAgain = true;
			while (goAgain) {
				goAgain = false;

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					int row = Utilities.GetRowFromTileIndex(tileIndex);
					int col = Utilities.GetColFromTileIndex(tileIndex);

					if (!Utilities.IsBorderTile(tileIndex)) {
						bool tileIsGround = countsAsGround.Contains(Game.TileLookupById[Screen.Tiles[tileIndex]]);

						bool topIsProtected = protectedFronts.Contains(
							Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(col, row - 1)]]
						);

						bool topLeftIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row - 1)] ==
						                     Game.TileLookup[tileType];

						bool topIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col, row - 1)] ==
						                 Game.TileLookup[tileType];

						bool topRightIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row - 1)] ==
						                      Game.TileLookup[tileType];

						bool rightIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)] ==
						                   Game.TileLookup[tileType];

						bool bottomRightIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row + 1)] ==
						                         Game.TileLookup[tileType];

						bool leftIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)] ==
						                  Game.TileLookup[tileType];

						bool bottomLeftIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row + 1)] ==
						                        Game.TileLookup[tileType];

						bool bottomIsTile = Screen.Tiles[Utilities.GetTileByColAndRow(col, row + 1)] ==
						                    Game.TileLookup[tileType];

						if (tileIsGround) {
							if (topLeftIsTile && topIsTile && topRightIsTile && rightIsTile && bottomRightIsTile) {
								Screen.Tiles[tileIndex] = Game.TileLookup[tileType];
								goAgain = true;
							}

							if (topLeftIsTile && topIsTile && topRightIsTile && leftIsTile && bottomLeftIsTile) {
								Screen.Tiles[tileIndex] = Game.TileLookup[tileType];
								goAgain = true;
							}

							if (leftIsTile && bottomLeftIsTile && bottomIsTile && bottomRightIsTile &&
							    topLeftIsTile && !topIsProtected
							) {
								Screen.Tiles[tileIndex] = Game.TileLookup[tileType];
								goAgain = true;
							}

							if (bottomLeftIsTile && bottomIsTile && bottomRightIsTile && rightIsTile &&
							    topRightIsTile && !topIsProtected
							) {
								Screen.Tiles[tileIndex] = Game.TileLookup[tileType];
								goAgain = true;
							}
						}
					}
				}
			}
		}

		protected void AddWatersideGrass(int lengthOfGrass = 2) {
			for (int i = 0; i < lengthOfGrass; i++) {
				List<int> tilesToTurnToGrass = new List<int>();

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					TileType tile = Utilities.GetTile(Screen, tileIndex);
					List<TileType> borderingTiles = new List<TileType> {
						Utilities.GetTileRight(Screen, tileIndex),
						Utilities.GetTileLeft(Screen, tileIndex),
						Utilities.GetTileUp(Screen, tileIndex),
						Utilities.GetTileDown(Screen, tileIndex),
					};

					if (tile == TileType.Ground &&
					    (borderingTiles.Contains(TileType.Water) || borderingTiles.Contains(TileType.Desert))
					) {
						tilesToTurnToGrass.Add(tileIndex);
					}
				}

				foreach (int tileIndex in tilesToTurnToGrass) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Desert];
				}
			}

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				TileType tile = Utilities.GetTile(Screen, tileIndex);
				List<TileType> borderingTiles = new List<TileType> {
					Utilities.GetTileRight(Screen, tileIndex),
					Utilities.GetTileLeft(Screen, tileIndex),
					Utilities.GetTileUp(Screen, tileIndex),
					Utilities.GetTileDown(Screen, tileIndex),
				};

				if (tile == TileType.Ground &&
				    borderingTiles.Contains(TileType.Desert) &&
				    Utilities.GetRandomInt(0, 2) == 0
				) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Desert];
				}
			}
		}
	}
}