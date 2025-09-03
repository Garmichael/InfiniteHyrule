using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class RockyBeachBuilder : ScreenBuilder {
		public RockyBeachBuilder(Screen screen) : base(screen) { }

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			DoubleTopAndBottomEdges();

			Screen.PaletteInterior = 3;
			Screen.EnvironmentColor = EnvironmentColor.Brown;

			if (Screen.Row == Game.LastScreenRow) {
				ExtendLips();
			}

			if (!Screen.ExitsEast &&
			    Screen.Column == 0 &&
			    Screen.Row != 0 &&
			    Screen.Row != Game.LastScreenRow
			) {
				BuildEastWall();
			}

			if (!Screen.ExitsWest &&
			    Screen.Column == Game.LastScreenColumn &&
			    Screen.Row != 0 &&
			    Screen.Row != Game.LastScreenRow
			) {
				BuildWestWall();
			}

			CarveSpaceForExits();

			TileDrawingTemplates.BuildCoast(Screen);

			if (Screen.Row == Game.LastScreenRow &&
			    Screen.Column != 0 &&
			    Screen.Column != Game.LastScreenColumn &&
			    (Screen.GetScreenLeft() == null || Screen.GetScreenLeft().IsCoast) &&
			    (Screen.GetScreenRight() == null || Screen.GetScreenRight().IsCoast) &&
			    Utilities.GetRandomInt(0, 2) == 0 && FrontEnd.MainWindow.GenerateEnhancedBeaches
			) {
				int coastCutLeft = Utilities.GetRandomInt(3, 6);
				int coastCutRight = Utilities.GetRandomInt(10, 13);
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, coastCutLeft, 6, coastCutRight, 6);

				coastCutLeft += 2;
				coastCutRight -= 2;
				if (coastCutRight - coastCutLeft > 2) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, coastCutLeft, 7, coastCutRight, 7);
				}
			}

			if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}

			if (Screen.CaveDestination == Game.CaveLookup[CaveType.AnyRoad]) {
				TileDrawingTemplates.BuildAnyRoadFormation(Screen);
			}

			if (Screen.IsDock) {
				TileDrawingTemplates.BuildDock(Screen);
			}

			AddCorners(TileType.Rock);
			CleanSoloRocks();
			
			if (FrontEnd.MainWindow.GenerateEnhancedBeaches) {
				AddWatersideGrass();
				AddPalmTrees();
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			if (edge == Direction.Right) {
				if (Screen.GetScreenRight().Region == Screen.Region) {
					if (Utilities.GetRandomInt(0, 1) == 0) {
						Screen.EdgeEast = new List<bool>
							{ true, true, false, false, false, false, false, false, false, true, true };
					} else {
						if (Screen.Row == 0) {
							Screen.EdgeEast = new List<bool>
								{ true, true, false, false, false, false, false, true, true, true, true };
						} else {
							Screen.EdgeEast = new List<bool>
								{ true, true, true, true, false, false, false, false, false, true, true };
						}
					}
				} else {
					Screen.EdgeEast = new List<bool>
						{ true, true, false, false, false, false, false, false, false, true, true };
				}
			}

			if (edge == Direction.Left) {
				if (Screen.GetScreenLeft().Region == Screen.Region) {
					if (Utilities.GetRandomInt(0, 1) == 0) {
						Screen.EdgeWest = new List<bool>
							{ true, true, false, false, false, false, false, false, false, true, true };
					} else {
						if (Screen.Row == 0) {
							Screen.EdgeWest = new List<bool>
								{ true, true, false, false, false, false, false, true, true, true, true };
						} else {
							Screen.EdgeWest = new List<bool>
								{ true, true, true, true, false, false, false, false, false, true, true };
						}
					}
				} else {
					Screen.EdgeWest = new List<bool>
						{ true, true, false, false, false, false, false, false, false, true, true };
				}
			}

			if (edge == Direction.Down) {
				if (Screen.Column == 0) {
					Screen.EdgeSouth = new List<bool> {
						true, true, true,
						false, false, false, false, false, false, false, false,
						true, true, true, true, true
					};
				} else if (Screen.Column == Game.LastScreenColumn) {
					Screen.EdgeSouth = new List<bool> {
						true, true, true, true, true, true,
						false, false, false, false, false, false, false,
						true, true, true
					};
				} else {
					Screen.EdgeSouth = new List<bool> {
						true, true,
						false, false, false,
						true, true
					};

					for (int i = 0; i < 9; i++) {
						if (Utilities.GetRandomInt(0, 1) == 1) {
							Screen.EdgeSouth.Insert(0, true);
						} else {
							Screen.EdgeSouth.Add(true);
						}
					}
				}
			}

			if (edge == Direction.Up) {
				if (Screen.Column == 0) {
					Screen.EdgeNorth = new List<bool> {
						true, true, true,
						false, false, false, false, false, false, false, false,
						true, true, true, true, true
					};
				} else if (Screen.Column == Game.LastScreenColumn) {
					Screen.EdgeNorth = new List<bool> {
						true, true, true, true, true, true,
						false, false, false, false, false, false, false,
						true, true, true
					};
				} else {
					Screen.EdgeNorth = new List<bool> {
						true, true,
						false, false, false,
						true, true
					};

					for (int i = 0; i < 9; i++) {
						if (Utilities.GetRandomInt(0, 1) == 1) {
							Screen.EdgeNorth.Insert(0, true);
						} else {
							Screen.EdgeNorth.Add(true);
						}
					}
				}
			}
		}

		private void ExtendLips() {
			if (Screen.ExitsWest && Screen.EdgeWest[2]) {
				if (Screen.ExitsEast && Screen.EdgeEast[2]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 1, 2, Game.TilesWide - 2, 3);
				} else {
					TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 1, 2, Utilities.GetRandomInt(3, 6), 3);
				}
			} else if (Screen.ExitsEast && Screen.EdgeEast[2]) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Rock, Utilities.GetRandomInt(9, 12), 2,
					Game.TilesWide - 2, 3);
			}

			if (Screen.ExitsNorth) {
				int indexOfFirstHole = Screen.EdgeNorth.IndexOf(false);
				int indexOfLastHole = Screen.EdgeNorth.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, indexOfFirstHole, 2, indexOfLastHole, 3);
			}
		}

		private void BuildEastWall() {
			int start = 2;
			int bottom = 8;

			for (int index = 0; index < 11; index++) {
				bool solid = !(index >= start && index <= bottom);
				TileDrawing.DrawTile(Screen, solid ? TileType.Rock : TileType.Ground, 11, index);
			}

			int columnWithSameStart = Utilities.GetRandomInt(12, 13);

			for (int i = 12; i <= 14; i++) {
				if (columnWithSameStart != i) {
					start += Utilities.GetRandomInt(0, 2);
				}

				bottom -= Utilities.GetRandomInt(0, 2);

				for (int index = 0; index < 11; index++) {
					bool solid = !(index >= start && index <= bottom);
					TileDrawing.DrawTile(Screen, solid ? TileType.Rock : TileType.Ground, i, index);
				}
			}

			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 14, 0, 15, 10);

			if (Screen.ExitsSouth) {
				int firstIndex = Screen.EdgeSouth.IndexOf(false);
				int lastIndex = Screen.EdgeSouth.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, firstIndex, 8, lastIndex, 10);
			}

			if (Screen.ExitsNorth) {
				int firstIndex = Screen.EdgeNorth.IndexOf(false);
				int lastIndex = Screen.EdgeNorth.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, firstIndex, 0, lastIndex, 2);
			}

			if (Screen.ExitsEast) {
				int indexOfFirstHole = Screen.EdgeEast.IndexOf(false);
				int indexOfLastHole = Screen.EdgeEast.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground,
					11, indexOfFirstHole, 15, indexOfLastHole);
			}
		}

		private void BuildWestWall() {
			int start = 2;
			int bottom = 8;

			for (int index = 0; index < 11; index++) {
				bool solid = !(index >= start && index <= bottom);
				TileDrawing.DrawTile(Screen, solid ? TileType.Rock : TileType.Ground, 5, index);
			}

			int columnWithSameStart = Utilities.GetRandomInt(3, 4);

			for (int i = 4; i >= 2; i--) {
				if (columnWithSameStart != i) {
					start += Utilities.GetRandomInt(0, 2);
				}

				bottom -= Utilities.GetRandomInt(0, 2);

				for (int index = 0; index < 11; index++) {
					bool solid = !(index >= start && index <= bottom);
					TileDrawing.DrawTile(Screen, solid ? TileType.Rock : TileType.Ground, i, index);
				}
			}

			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 0, 0, 2, 10);

			if (Screen.ExitsWest) {
				int indexOfFirstHole = Screen.EdgeWest.IndexOf(false);
				int indexOfLastHole = Screen.EdgeWest.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 0, indexOfFirstHole, 5, indexOfLastHole);
			}
		}

		private void CarveSpaceForExits() {
			if (Screen.Column == 0) {
				for (int columnIndex = 11; columnIndex < Game.TilesWide; columnIndex++) {
					if (!Screen.EdgeNorth[columnIndex]) {
						TileDrawing.FillRectWithTiles(Screen, TileType.Ground, columnIndex, 0, columnIndex, 6);
					}
				}
			}

			if (Screen.Column == Game.LastScreenColumn) {
				for (int columnIndex = 2; columnIndex < 6; columnIndex++) {
					if (!Screen.EdgeNorth[columnIndex]) {
						TileDrawing.FillRectWithTiles(Screen, TileType.Ground, columnIndex, 0, columnIndex, 6);
					}
				}
			}
		}

		private void AddPalmTrees() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				bool isEdgeTile = Utilities.IsThickBorderTile(tileIndex);

				if (isEdgeTile) {
					continue;
				}
				
				bool isDesert = Utilities.GetTile(Screen, tileIndex) == TileType.Desert;
				bool isCoastAdjacent = Utilities.GetTileDown(Screen, tileIndex) == TileType.Water ||
				                       Utilities.GetTileRight(Screen, tileIndex) == TileType.Water ||
				                       Utilities.GetTileLeft(Screen, tileIndex) == TileType.Water;

				bool isRockAdjacent = Utilities.GetTileRight(Screen, tileIndex) == TileType.Rock ||
				                      Utilities.GetTileLeft(Screen, tileIndex) == TileType.Rock ||
				                      Utilities.GetTileUp(Screen, tileIndex) == TileType.Rock ||
				                      Utilities.GetTileDown(Screen, tileIndex) == TileType.Rock ||
				                      Utilities.GetTileUp(Screen, tileIndex + 1) == TileType.Rock ||
				                      Utilities.GetTileUp(Screen, tileIndex - 1) == TileType.Rock ||
				                      Utilities.GetTileDown(Screen, tileIndex + 1) == TileType.Rock ||
				                      Utilities.GetTileDown(Screen, tileIndex - 1) == TileType.Rock;

				bool blocksItemDock = Game.OverworldItemScreenId == Screen.ScreenId && 
				                      (
					                      Utilities.GetTileDown(Screen, tileIndex + 16) == TileType.Bridge ||
					                      Utilities.GetTileRight(Screen, tileIndex + 1) == TileType.Bridge ||
					                      Utilities.GetTileLeft(Screen, tileIndex - 1) == TileType.Bridge
				                      );

				bool isRandomlyATree = Utilities.GetRandomInt(0, 5) <= 0;

				if (!blocksItemDock && !isRockAdjacent && isDesert && isCoastAdjacent && isRandomlyATree) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.PalmTree];
				}
			}
		}

		public override void AssignEnemies() {
			int enemySet = Utilities.GetRandomInt(0, 5);

			if (enemySet == 0) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 1) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverBlue];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 3) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 4) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRedFast];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 5) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
				];
				Screen.UsesMixedEnemies = true;
			}

			if (Screen.IsLake || Screen.IsCoast) {
				Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Four];
			}
			
			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);
		}
	}
}