using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class RiverBuilder : ScreenBuilder {
		public RiverBuilder(Screen screen) : base(screen) { }

		private bool RiverFlowsNorth => Screen.GetScreenUp() == null ||
		                                Screen.GetScreenUp().Region == Screen.Region && Screen.ExitsNorth;

		private bool RiverFlowsSouth => Screen.GetScreenDown() == null ||
		                                Screen.GetScreenDown().Region == Screen.Region && Screen.ExitsSouth;

		private bool RiverFlowsEast => Screen.GetScreenRight() == null ||
		                               Screen.GetScreenRight().Region == Screen.Region && Screen.ExitsEast;

		private bool RiverFlowsWest => Screen.GetScreenLeft() == null ||
		                               Screen.GetScreenLeft().Region == Screen.Region && Screen.ExitsWest;


		private const int HorizontalStart = 7;
		private const int HorizontalEnd = 9;
		private const int VerticalStart = 5;
		private const int VerticalEnd = 6;
		private const int RowWhereForestStarts = 2;

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			DoubleTopAndBottomEdges();

			if (RiverFlowsNorth) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Water, HorizontalStart, 0, HorizontalEnd, VerticalEnd);
			}

			if (RiverFlowsSouth) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Water,
					HorizontalStart, VerticalStart, HorizontalEnd, 10
				);
			}

			if (RiverFlowsEast) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Water, HorizontalStart, VerticalStart, 15, VerticalEnd);
			}

			if (RiverFlowsWest) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Water, 0, VerticalStart, HorizontalEnd, VerticalEnd);
			}

			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}


			if (Screen == Screen.Region.Screens[0] && Screen.Row == 0) {
				DrawRiverSource();
			}

			if (Screen == Screen.Region.Screens[1] && Screen.Row == 1) {
				DrawSouthOfRiverMouth();
			}

			if (Screen.Row < RowWhereForestStarts) {
				Screen.EnvironmentColor = EnvironmentColor.Brown;
				Screen.PaletteInterior = Screen.PaletteBorder;

				if (Screen.HasRiverBridge) {
					BuildBridge();
				}
			} else {
				TileDrawing.ReplaceTile(Screen, TileType.Rock, TileType.Tree);

				if (Screen != Screen.Region.Screens.Last()) {
					FillTreeBank();
				}

				AddWatersideGrass();

				if (Screen.HasRiverBridge) {
					BuildBridge();
				}

				if (Screen != Screen.Region.Screens.Last()) {
					AddTrees();
				}
			}

			if (Screen == Screen.Region.Screens.Last()) {
				DrawLakeEnd();
				AddCorners(TileType.Tree);
			}

			if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}
		}

		private void DrawRiverSource() {
			Screen.EdgeSouth = GetSolidFilledEdgeTiles(true);
			Screen.EdgeSouth[HorizontalStart - 2] = false;
			Screen.EdgeSouth[HorizontalEnd + 2] = false;

			TileDrawing.FillRectWithTiles(Screen, TileType.Rock,
				0, Game.LastTileRow - 1, Game.LastTileColumn, Game.LastTileRow);

			TileDrawing.FillRectWithTiles(Screen, TileType.Ladder,
				HorizontalStart - 2, Game.LastTileRow - 1, HorizontalStart - 2, Game.LastTileRow);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ladder,
				HorizontalEnd + 2, Game.LastTileRow - 1, HorizontalEnd + 2, Game.LastTileRow);

			TileDrawing.FillRectWithTiles(Screen, TileType.Waterfall, HorizontalStart - 1, 0, HorizontalEnd + 1, 1);
			TileDrawing.FillRectWithTiles(Screen, TileType.Waterfall,
				HorizontalStart, Game.LastTileRow - 1, HorizontalEnd, Game.LastTileRow);

			TileDrawing.FillRectWithTiles(Screen, TileType.Water, HorizontalStart - 3, 2, HorizontalEnd + 3, 6);
		}

		private void DrawSouthOfRiverMouth() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 0, 0, Game.LastTileColumn, 1);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ladder, HorizontalStart - 2, 0, HorizontalStart - 2, 1);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ladder, HorizontalEnd + 2, 0, HorizontalEnd + 2, 1);
			TileDrawing.FillRectWithTiles(Screen, TileType.Waterfall, HorizontalStart, 0, HorizontalEnd, 1);
		}

		private void BuildBridge() {
			List<Direction> segmentToSpanOptions = new List<Direction>();
			if (RiverFlowsNorth) {
				segmentToSpanOptions.Add(Direction.Up);
			}

			if (RiverFlowsSouth) {
				segmentToSpanOptions.Add(Direction.Down);
			}

			if (RiverFlowsEast) {
				segmentToSpanOptions.Add(Direction.Right);
			}

			if (RiverFlowsWest) {
				segmentToSpanOptions.Add(Direction.Left);
			}

			Direction segmentToSpan = segmentToSpanOptions[Utilities.GetRandomInt(0, segmentToSpanOptions.Count - 1)];

			if (segmentToSpan == Direction.Up) {
				int y = Utilities.GetRandomInt(VerticalStart - 3, VerticalStart - 2);
				TileDrawing.FillRectWithTiles(Screen, TileType.Bridge, HorizontalStart, y, HorizontalEnd, y);
			} else if (segmentToSpan == Direction.Down) {
				int y = Utilities.GetRandomInt(VerticalEnd + 2, VerticalEnd + 3);
				TileDrawing.FillRectWithTiles(Screen, TileType.Bridge, HorizontalStart, y, HorizontalEnd, y);
			} else if (segmentToSpan == Direction.Left) {
				int x = Utilities.GetRandomInt(HorizontalStart - 3, HorizontalStart - 2);
				TileDrawing.FillRectWithTiles(Screen, TileType.BridgeVertical, x, VerticalStart, x, VerticalEnd);
			} else if (segmentToSpan == Direction.Right) {
				int x = Utilities.GetRandomInt(HorizontalEnd + 2, HorizontalEnd + 3);
				TileDrawing.FillRectWithTiles(Screen, TileType.BridgeVertical, x, VerticalStart, x, VerticalEnd);
			}
		}

		private void FillTreeBank() {
			if (Screen.ExitsNorth) {
				if (Screen.ExitsWest) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree,
						0, 0, Screen.EdgeNorth.IndexOf(false) - 1, Screen.EdgeWest.IndexOf(false) - 1
					);
				}

				if (Screen.ExitsEast) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree,
						Screen.EdgeNorth.LastIndexOf(false) + 1, 0,
						Game.LastTileColumn, Screen.EdgeEast.IndexOf(false) - 1
					);
				}
			}

			if (Screen.ExitsSouth) {
				if (Screen.ExitsWest) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree,
						0, Screen.EdgeWest.LastIndexOf(false) + 1,
						Screen.EdgeSouth.IndexOf(false) - 1, Game.LastTileRow
					);
				}

				if (Screen.ExitsEast) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree,
						Screen.EdgeSouth.LastIndexOf(false) + 1, Screen.EdgeEast.LastIndexOf(false) + 1,
						Game.LastTileColumn, Game.LastTileRow
					);
				}
			}

			if (!Screen.ExitsWest) {
				int topIndex = Screen.EdgeNorth.IndexOf(false) - 1;
				int bottomIndex = Screen.EdgeSouth.IndexOf(false) - 1;

				if (topIndex < 0) {
					topIndex = Game.LastTileColumn;
				}

				if (bottomIndex < 0) {
					bottomIndex = Game.LastTileColumn;
				}

				int right = topIndex < bottomIndex
					? topIndex
					: bottomIndex;

				if (right >= 0 && Screen.Column != 0) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 0, 0, right, Game.LastTileRow);
				}
			}

			if (!Screen.ExitsEast) {
				int topIndex = Screen.EdgeNorth.LastIndexOf(false) + 1;
				int bottomIndex = Screen.EdgeSouth.LastIndexOf(false) + 1;

				int left = topIndex > bottomIndex
					? topIndex
					: bottomIndex;

				if (left >= 0 && Screen.Column != Game.LastScreenColumn) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Tree,
						left, 0, Game.LastTileColumn, Game.LastTileRow);
				}
			}
		}

		private void DrawLakeEnd() {
			TileDrawing.FillRectWithTiles(
				Screen, TileType.Water,
				HorizontalStart - 1, VerticalStart - 1, HorizontalEnd + 1, VerticalEnd + 1
			);
		}

		private void AddTrees() {
			List<Rect> quadrants = new List<Rect> {
				new Rect(4, 2, HorizontalStart - 1, VerticalStart - 1),
				new Rect(HorizontalEnd + 1, 2, Game.LastTileColumn - 1, VerticalStart - 1),
				new Rect(1, VerticalEnd + 1, HorizontalStart - 1, Game.LastTileRow - 2),
				new Rect(HorizontalEnd + 1, VerticalEnd + 1, Game.LastTileColumn - 1, Game.LastTileRow - 2),
			};

			foreach (Rect quadrant in quadrants) {
				List<int> validTilesForTrees = new List<int>();
				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					int col = Utilities.GetColFromTileIndex(tileIndex);
					int row = Utilities.GetRowFromTileIndex(tileIndex);

					if (
						col >= quadrant.X &&
						col <= quadrant.Width &&
						row >= quadrant.Y &&
						row <= quadrant.Height &&
						Utilities.GetTileUp(Screen, tileIndex) != TileType.Bridge &&
						Utilities.GetTileDown(Screen, tileIndex) != TileType.Bridge &&
						Utilities.GetTileLeft(Screen, tileIndex) != TileType.Bridge &&
						Utilities.GetTileRight(Screen, tileIndex) != TileType.Bridge &&
						Utilities.GetTileUp(Screen, tileIndex) != TileType.BridgeVertical &&
						Utilities.GetTileDown(Screen, tileIndex) != TileType.BridgeVertical &&
						Utilities.GetTileLeft(Screen, tileIndex) != TileType.BridgeVertical &&
						Utilities.GetTileRight(Screen, tileIndex) != TileType.BridgeVertical &&
						(Utilities.GetTile(Screen, tileIndex) == TileType.Ground ||
						 Utilities.GetTile(Screen, tileIndex) == TileType.Desert)
					) {
						validTilesForTrees.Add(tileIndex);
					}
				}

				if (validTilesForTrees.Count > 0) {
					int tileToPlaceTree =
						validTilesForTrees[Utilities.GetRandomInt(0, validTilesForTrees.Count - 1)];
					Screen.Tiles[tileToPlaceTree] = Game.TileLookup[TileType.Tree];
				}
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			AssignSolidEdge(edge);

			if (edge == Direction.Up) {
				Screen.EdgeNorth[5] = false;
				Screen.EdgeNorth[6] = false;
				Screen.EdgeNorth[10] = false;
				Screen.EdgeNorth[11] = false;
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth[5] = false;
				Screen.EdgeSouth[6] = false;
				Screen.EdgeSouth[10] = false;
				Screen.EdgeSouth[11] = false;
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest[3] = false;
				Screen.EdgeWest[4] = false;
				Screen.EdgeWest[7] = false;
				Screen.EdgeWest[8] = false;
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast[3] = false;
				Screen.EdgeEast[4] = false;
				Screen.EdgeEast[7] = false;
				Screen.EdgeEast[8] = false;
			}
		}

		public override void AssignEnemies() {
			if (Screen.Row <= 1) {
				AssignRockyEnemies();
			} else {
				AssignForestryEnemies();
			}
		}

		private void AssignRockyEnemies() {
			int enemySet = Utilities.GetRandomInt(0, 4);

			if (enemySet == 0) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LynelRed];
				Screen.UsesMixedEnemies = false;
				Screen.EnemiesEnterFromSides = false;
			} else if (enemySet == 1) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LynelBlue];
				Screen.UsesMixedEnemies = false;
				Screen.EnemiesEnterFromSides = false;
			} else if (enemySet == 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue
				];
				Screen.UsesMixedEnemies = true;
				Screen.EnemiesEnterFromSides = false;
			} else if (enemySet == 3) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None
				];
				Screen.UsesMixedEnemies = true;
				Screen.EnemiesEnterFromSides = false;
			} else if (enemySet == 4) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LynelRed_LynelBlue_LeeverBlue
				];
				Screen.UsesMixedEnemies = true;
				Screen.EnemiesEnterFromSides = false;
			}

			if (Utilities.GetRandomInt(0, 3) == 0) {
				Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.One];
			}
		}

		private void AssignForestryEnemies() {
			int enemySet = Utilities.GetRandomInt(0, 4);

			if (enemySet == 0) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet <= 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 3) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverBlue];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 4) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
				Screen.UsesMixedEnemies = false;
			}

			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);
		}
	}
}