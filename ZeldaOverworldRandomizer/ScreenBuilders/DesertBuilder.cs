using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class DesertBuilder : ScreenBuilder {
		public DesertBuilder(Screen screen) : base(screen) { }

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			TileDrawing.CloneTileRow(Screen, 0, 1);
			TileDrawing.CloneTileRow(Screen, Game.LastTileRow, Game.LastTileRow - 1);

			if (Screen.IsOpenDungeon) {
				TileDrawingTemplates.BuildDungeonEntrance(Screen);
			} else if (Screen.IsOasis && FrontEnd.MainWindow.GenerateEnhancedDeserts) {
				BuildOasis();
			} else if (FrontEnd.MainWindow.GenerateEnhancedDeserts) {
				BuildGenericScreen();
			}

			if (!Screen.IsOpenDungeon && Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}

			TileDrawing.ReplaceTile(Screen, TileType.Ground, TileType.Desert);
		}

		protected override void AssignConstructedEdge(Direction edge) {
			if (edge == Direction.Up) {
				Screen.EdgeNorth = GetSolidFilledEdgeTiles(true);
				for (int i = 0; i < Screen.EdgeNorth.Count; i++) {
					Screen.EdgeNorth[i] = i <= 0 || i >= Game.LastTileColumn;
				}
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = GetSolidFilledEdgeTiles(true);
				for (int i = 0; i < Screen.EdgeSouth.Count; i++) {
					Screen.EdgeSouth[i] = i <= 0 || i >= Game.LastTileColumn;
				}
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = GetSolidFilledEdgeTiles(false);
				for (int i = 0; i < Screen.EdgeWest.Count; i++) {
					Screen.EdgeWest[i] = i <= 1 || i >= Game.LastTileRow - 1;
				}
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = GetSolidFilledEdgeTiles(false);
				for (int i = 0; i < Screen.EdgeEast.Count; i++) {
					Screen.EdgeEast[i] = i <= 1 || i >= Game.LastTileRow - 1;
				}
			}
		}

		private void BuildGenericScreen() {
			BuildLargeRocks();
			BuildBoulders();
		}

		private void BuildLargeRocks() {
			const int totalRocks = 2;

			for (int i = 0; i < totalRocks; i++) {
				int width = Utilities.GetRandomInt(2, 3);
				const int height = 2;
				int left = Utilities.GetRandomInt(2, 5) + i * 6;
				int top = Utilities.GetRandomInt(3, 6);

				TileDrawing.FillRectWithTiles(Screen, TileType.Rock, left, top, left + width - 1, top + height - 1);
			}
		}

		private void BuildBoulders() {
			int totalBoulders = Utilities.GetRandomInt(2, 5);

			for (int i = 0; i < totalBoulders; i++) {
				List<int> validTileIndexes = new List<int>();

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					TileType tileThis = Utilities.GetTile(Screen, tileIndex);
					TileType tileUp = Utilities.GetTileUp(Screen, tileIndex);
					TileType tileDown = Utilities.GetTileDown(Screen, tileIndex);
					TileType tileLeft = Utilities.GetTileLeft(Screen, tileIndex);
					TileType tileRight = Utilities.GetTileRight(Screen, tileIndex);

					if (
						!Utilities.IsThickBorderTile(tileIndex) &&
						tileThis == TileType.Ground &&
						tileUp == TileType.Ground &&
						tileDown == TileType.Ground &&
						tileLeft == TileType.Ground &&
						tileRight == TileType.Ground
					) {
						validTileIndexes.Add(tileIndex);
					}
				}

				if (validTileIndexes.Count > 0) {
					int boulderTileIndex = validTileIndexes[Utilities.GetRandomInt(0, validTileIndexes.Count - 1)];
					Screen.Tiles[boulderTileIndex] = Game.TileLookup[TileType.Boulder];
				}
			}
		}

		private void BuildOasis() {
			Screen.PaletteInterior = 2;

			for (int i = 0; i < 3; i++) {
				int left = Utilities.GetRandomInt(5, 8);
				int top = Utilities.GetRandomInt(4, 5);
				TileDrawing.FillRectWithTiles(Screen, TileType.Water, left, top, left + 2, top + 2);
			}

			int totalTrees = Utilities.GetRandomInt(2, 5);
			for (int i = 0; i < totalTrees; i++) {
				List<int> validTileIndexes = new List<int>();

				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					TileType tileThis = Utilities.GetTile(Screen, tileIndex);
					TileType tileUp = Utilities.GetTileUp(Screen, tileIndex);
					TileType tileDown = Utilities.GetTileDown(Screen, tileIndex);
					TileType tileLeft = Utilities.GetTileLeft(Screen, tileIndex);
					TileType tileRight = Utilities.GetTileRight(Screen, tileIndex);

					bool isWaterAdjacent = tileUp == TileType.Water ||
					                       tileDown == TileType.Water ||
					                       tileLeft == TileType.Water ||
					                       tileRight == TileType.Water;

					bool isTreeAdjacent = tileUp == TileType.PalmTree ||
					                      tileDown == TileType.PalmTree ||
					                      tileLeft == TileType.PalmTree ||
					                      tileRight == TileType.PalmTree;

					bool isRockAdjacent = tileUp == TileType.Rock ||
					                      tileDown == TileType.Rock ||
					                      tileLeft == TileType.Rock ||
					                      tileRight == TileType.Rock;

					if (!Utilities.IsThickBorderTile(tileIndex) &&
					    tileThis == TileType.Ground &&
					    isWaterAdjacent &&
					    !isTreeAdjacent &&
					    !isRockAdjacent
					   ) {
						validTileIndexes.Add(tileIndex);
					}
				}

				if (validTileIndexes.Count > 0) {
					int treeTileIndex = validTileIndexes[Utilities.GetRandomInt(0, validTileIndexes.Count - 1)];
					Screen.Tiles[treeTileIndex] = Game.TileLookup[TileType.PalmTree];
				}
			}
		}

		public override void AssignEnemies() {
			if (Screen.IsFairyPond) {
				return;
			}

			if (Screen.IsOasis) {
				int enemySet = Utilities.GetRandomInt(0, 3);

				if (enemySet == 0 && !Screen.IsOpenDungeon) {
					Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.One];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[MixedEnemyTypes.TwinMoldorm];
					Screen.UsesMixedEnemies = true;
				} else {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat
					];
					Screen.UsesMixedEnemies = true;
				}
			} else {
				int enemySet = Utilities.GetRandomInt(0, 6);

				if (enemySet == 0 && !Screen.IsOpenDungeon) {
					Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.One];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[MixedEnemyTypes.TwinMoldorm];
					Screen.UsesMixedEnemies = true;
				} else if (enemySet <= 3) {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
					Screen.UsesMixedEnemies = false;
				} else if (enemySet <= 6) {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverBlue];
					Screen.UsesMixedEnemies = false;
				}

				Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);
			}
		}
	}
}