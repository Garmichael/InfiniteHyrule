using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class TunnelBuilder : ScreenBuilder {
		public TunnelBuilder(Screen screen) : base(screen) { }

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			DoubleTopAndBottomEdges();

			BuildInterior();

			TileDrawingTemplates.BuildLake(Screen);

			CleanSoloRocks();

			if (Screen.IsAnyRoad) {
				TileDrawingTemplates.BuildAnyRoadFormation(Screen);
			} else if (Screen.IsOpenDungeon) {
				TileDrawingTemplates.BuildDungeonEntrance(Screen);
			} else if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			bool isVerticalEdge = edge == Direction.Down || edge == Direction.Up;

			List<bool> edgeProfile = GetSolidFilledEdgeTiles(isVerticalEdge);

			if (isVerticalEdge) {
				int start;
				int length;

				if (
					edge == Direction.Up && Screen.GetScreenUp().Region.Biome == Screen.Region.Biome ||
					edge == Direction.Down && Screen.GetScreenDown().Region.Biome == Screen.Region.Biome
				) {
					start = 7;
					length = 1;
				} else {
					start = Utilities.GetRandomInt(3, 6);
					length = Utilities.GetRandomInt(5, 8);
				}

				if (
					edge == Direction.Up && Screen.HasWaterTopRight ||
					edge == Direction.Down && Screen.HasWaterBottomRight
				) {
					start = 6;
				} else if (
					edge == Direction.Up && Screen.HasWaterTopLeft ||
					edge == Direction.Down && Screen.HasWaterBottomLeft
				) {
					start = 9 - length + 1;
				}

				for (int i = 0; i < edgeProfile.Count; i++) {
					edgeProfile[i] = i < start || i >= start + length;
				}
			} else {
				int start;
				int length;

				if (
					edge == Direction.Right && Screen.GetScreenRight().Region.Biome == Screen.Region.Biome ||
					edge == Direction.Left && Screen.GetScreenLeft().Region.Biome == Screen.Region.Biome
				) {
					start = 5;
					length = 1;
				} else {
					start = Utilities.GetRandomInt(3, 6);
					length = Utilities.GetRandomInt(2, 4);
				}

				if (
					edge == Direction.Right && Screen.HasWaterTopRight ||
					edge == Direction.Left && Screen.HasWaterTopLeft
				) {
					start = 5;
				} else if (
					edge == Direction.Right && Screen.HasWaterBottomRight ||
					edge == Direction.Left && Screen.HasWaterBottomLeft
				) {
					start = 5 - length + 1;
				}

				for (int i = 0; i < edgeProfile.Count; i++) {
					edgeProfile[i] = i < start || i >= start + length;
				}
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

		private void BuildInterior() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 1, 2, Game.LastTileColumn - 1, Game.LastTileRow - 2);

			for (int index = 0; index < Screen.EdgeNorth.Count; index++) {
				int westEnd = Screen.EdgeWest.LastIndexOf(false);
				int eastEnd = Screen.EdgeEast.LastIndexOf(false);

				int end = westEnd > eastEnd
					? westEnd
					: eastEnd;

				if (end < 0) {
					end = Game.TilesHigh - 3;
				}

				if (!Screen.EdgeNorth[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, index, 0, index, end);
				}
			}

			for (int index = 0; index < Screen.EdgeSouth.Count; index++) {
				int westEnd = Screen.EdgeWest.IndexOf(false);
				int eastEnd = Screen.EdgeEast.IndexOf(false);

				if (westEnd == -1) {
					westEnd = Game.TilesWide;
				}

				if (eastEnd == -1) {
					eastEnd = Game.TilesWide;
				}

				int end = westEnd < eastEnd
					? westEnd
					: eastEnd;

				if (end > Game.TilesWide - 1) {
					end = 2;
				}

				if (!Screen.EdgeSouth[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, index, end, index, Game.TilesHigh - 1);
				}
			}

			if (Screen.ExitsNorth && Screen.ExitsSouth && !Screen.ExitsWest && !Screen.ExitsEast) {
				int firstIndex = Screen.EdgeNorth.IndexOf(false) < Screen.EdgeSouth.IndexOf(false)
					? Screen.EdgeNorth.IndexOf(false)
					: Screen.EdgeSouth.IndexOf(false);

				int lastIndex = Screen.EdgeNorth.LastIndexOf(false) > Screen.EdgeSouth.LastIndexOf(false)
					? Screen.EdgeNorth.LastIndexOf(false)
					: Screen.EdgeSouth.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, firstIndex, 2, lastIndex, 2);
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground,
					firstIndex, Game.TilesHigh - 3, lastIndex, Game.TilesHigh - 3
				);
			}

			for (int index = 0; index < Screen.EdgeWest.Count; index++) {
				int northEnd = Screen.EdgeNorth.LastIndexOf(false);
				int southEnd = Screen.EdgeSouth.LastIndexOf(false);

				int end = northEnd > southEnd
					? northEnd
					: southEnd;

				if (end < 0) {
					end = Game.TilesWide - 2;
				}

				if (!Screen.EdgeWest[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 0, index, end, index);
				}
			}

			for (int index = 0; index < Screen.EdgeEast.Count; index++) {
				int northEnd = Screen.EdgeNorth.IndexOf(false);
				int southEnd = Screen.EdgeSouth.IndexOf(false);

				if (northEnd == -1) {
					northEnd = Game.TilesWide;
				}

				if (southEnd == -1) {
					southEnd = Game.TilesWide;
				}

				int end = northEnd < southEnd
					? northEnd
					: southEnd;

				if (end > Game.TilesWide - 1) {
					end = 1;
				}

				if (!Screen.EdgeEast[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, end, index, Game.TilesWide - 1, index);
				}
			}

			if (Screen.ExitsWest && Screen.ExitsEast && !Screen.ExitsNorth && !Screen.ExitsSouth) {
				int firstIndex = Screen.EdgeWest.IndexOf(false) < Screen.EdgeEast.IndexOf(false)
					? Screen.EdgeWest.IndexOf(false)
					: Screen.EdgeEast.IndexOf(false);

				int lastIndex = Screen.EdgeWest.LastIndexOf(false) > Screen.EdgeEast.LastIndexOf(false)
					? Screen.EdgeWest.LastIndexOf(false)
					: Screen.EdgeEast.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, firstIndex, 1, lastIndex);
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground,
					Game.TilesWide - 2, firstIndex, Game.TilesWide - 2, lastIndex
				);
			}

			const int erosionAmount = 4;
			const int erosionIntensity = 25;

			for (int i = 0; i < erosionAmount; i++) {
				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					if (!Utilities.IsBorderTile(tileIndex)) {
						int tileRow = Utilities.GetRowFromTileIndex(tileIndex);
						int tileCol = Utilities.GetColFromTileIndex(tileIndex);

						TileType thisTileType = Game.TileLookupById[Screen.Tiles[tileIndex]];
						TileType tileNorth = tileRow == 0
							? TileType.CaveAlt
							: Game.TileLookupById[Screen.Tiles[tileIndex - Game.TilesWide]];

						TileType tileSouth = tileRow == Game.TilesHigh - 1
							? TileType.CaveAlt
							: Game.TileLookupById[Screen.Tiles[tileIndex + Game.TilesWide]];

						TileType tileWest = tileCol == 0
							? TileType.CaveAlt
							: Game.TileLookupById[Screen.Tiles[tileIndex - 1]];

						TileType tileEast = tileCol == Game.TilesWide - 1
							? TileType.CaveAlt
							: Game.TileLookupById[Screen.Tiles[tileIndex + 1]];

						if (thisTileType == TileType.Rock) {
							int totalSurroundingGround = 0;
							if (tileNorth == TileType.Ground) {
								totalSurroundingGround++;
							}

							if (tileSouth == TileType.Ground) {
								totalSurroundingGround++;
							}

							if (tileWest == TileType.Ground) {
								totalSurroundingGround++;
							}

							if (tileEast == TileType.Ground) {
								totalSurroundingGround++;
							}

							if (totalSurroundingGround == 1) {
								int roll = Utilities.GetRandomInt(0, 100);
								if (roll < erosionIntensity) {
									Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ground];
								}
							}
						}
					}
				}
			}

			AddCorners();
		}

		private void AddCorners() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (!Utilities.IsBorderTile(tileIndex)) {
					bool topLeftIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row - 1)] ==
					                     Game.TileLookup[TileType.Rock];

					bool topIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col, row - 1)] ==
					                 Game.TileLookup[TileType.Rock];

					bool topRightIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row - 1)] ==
					                      Game.TileLookup[TileType.Rock];

					bool rightIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)] ==
					                   Game.TileLookup[TileType.Rock];

					bool bottomRightIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row + 1)] ==
					                         Game.TileLookup[TileType.Rock];

					bool leftIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)] ==
					                  Game.TileLookup[TileType.Rock];

					bool bottomLeftIsRock = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row + 1)] ==
					                        Game.TileLookup[TileType.Rock];

					if (topLeftIsRock && topIsRock && topRightIsRock && rightIsRock && bottomRightIsRock) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Rock];
					}

					if (topLeftIsRock && topIsRock && topRightIsRock && leftIsRock && bottomLeftIsRock) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Rock];
					}
				}
			}
		}

		public override void AssignEnemies() {
			if (Screen.JourneyDistanceToStartScreen <= 9) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteRed];
				Screen.UsesMixedEnemies = false;
			} else {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteBlue];
				Screen.UsesMixedEnemies = false;
			}

			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);
		}
	}
}