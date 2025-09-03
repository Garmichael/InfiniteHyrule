using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class IslandBuilder : ScreenBuilder {
		public IslandBuilder(Screen screen) : base(screen) { }

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.IsGraveyard
				? EnvironmentColor.Grey
				: Utilities.GetRandomInt(0, 1) == 0
					? EnvironmentColor.Green
					: EnvironmentColor.Brown;

			Screen.PaletteInterior = Screen.PaletteBorder;
			AssignEdges();

			if (FrontEnd.MainWindow.HideNormalDungeons && Screen.IsOpenDungeon && Screen.IsDockIsland) {
				Screen.CaveIsHidden = true;
			}

			if (!Screen.IsDock) {
				if (Screen.IsOpenDungeon) {
					BuildWaterBorder();
					BuildScreenConnections();
					TileDrawingTemplates.BuildDungeonEntrance(Screen);
				} else {
					if (Screen.IsHiddenDungeon && Screen.IsDockIsland) {
						BuildHiddenDungeonDockIsland();
					} else {
						BuildDecor();
						if (Screen.CaveDestination > 0) {
							AddCaveEntrance();
						}
					}
				}
			}

			if (Screen.IsDock) {
				BuildWaterBorder();
				BuildScreenConnections();
				TileDrawingTemplates.BuildDock(Screen);
			}
		}

		private void BuildHiddenDungeonDockIsland() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 0, 0, 15, 10);


			TileType topDecor = Utilities.GetRandomInt(0, 1) == 0 ? TileType.Rock : TileType.Tree;
			TileType bottomDecor = Utilities.GetRandomInt(0, 1) == 0 ? TileType.Boulder : TileType.Tree;

			TileDrawing.FillRectWithTiles(Screen, topDecor, 0, 0, 15, 3);

			for (int tileIndex = 16 * 3; tileIndex <= 16 * 6; tileIndex++) {
				if (Utilities.GetTileUp(Screen, tileIndex) == topDecor && Utilities.GetRandomInt(0, 2) > 0) {
					TileDrawing.DrawTile(Screen, topDecor, tileIndex);
				}
			}
			
			for (int tileIndex = 16 * 7; tileIndex <= 16 * 8; tileIndex++) {
				int col = Utilities.GetColFromTileIndex(tileIndex);
				List<int> acceptableColumns = new List<int> { 3, 4, 8, 9, 10, 11, 12 };
				bool isInAcceptableColumn = acceptableColumns.Contains(col);

				if (isInAcceptableColumn && Utilities.GetRandomInt(0, 1) == 0) {
					TileDrawing.DrawTile(Screen, bottomDecor, tileIndex);
				}
			}

			TileDrawing.DrawTile(Screen, TileType.Water, 1, 2);
			TileDrawing.DrawTile(Screen, TileType.Water, 1, 8);
			TileDrawing.DrawTile(Screen, TileType.Water, 14, 2);
			TileDrawing.DrawTile(Screen, TileType.Water, 14, 8);

			CleanSoloRocks();
			BuildWaterBorder();
			AddCaveEntrance();
			BuildScreenConnections();
		}

		private void BuildDecor() {
			List<Action> templates = new List<Action>();
			if (Screen.CaveDestination > 0 && !Screen.CaveIsHidden) {
				templates.Add(TemplateGroveWithACave);

				if (!Screen.IsDockIsland) {
					templates.Add(TemplateRockyIsland);
				}
			} else if (Screen.CaveDestination > 0 && Screen.CaveIsHidden) {
				templates.Add(TemplateThreeLakes);

				if (!Screen.IsDockIsland) {
					templates.Add(TemplateTwoIslands);
					templates.Add(TemplateRockyIsland);
				}
			} else {
				templates.Add(TemplateThreeLakes);

				if (!Screen.IsDockIsland) {
					templates.Add(TemplateTwoIslands);
					templates.Add(TemplateRockyIsland);
				}
			}

			templates[Utilities.GetRandomInt(0, templates.Count - 1)]();
		}

		private void TemplateTwoIslands() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 0, 0, Game.LastTileColumn, Game.LastTileRow);
			TileDrawing.FillRectWithTiles(Screen, TileType.Bridge, 5, 5, Screen.ExitsEast ? 14 : 7, 5);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 2, 4, 8);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 7, 2, 11, 8);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 1, 2, 4, 2);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 1, 8, 4, 8);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 9, 2, 11, 2);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 10, 3, 11, 3);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 11, 4, 11, 4);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 11, 6, 11, 6);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 10, 7, 11, 7);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 9, 8, 11, 8);

			BuildScreenConnections();
		}

		private void TemplateThreeLakes() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 0, 0, Game.LastTileColumn, Game.LastTileRow);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 2, Game.LastTileColumn - 1, Game.LastTileRow - 2);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 4, 4, 5, 6);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 7, 4, 9, 6);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 11, 4, 12, 6);
			TileDrawing.DrawTile(Screen, TileType.Tree, 1, 2);
			TileDrawing.DrawTile(Screen, TileType.Tree, 1, 8);
			TileDrawing.DrawTile(Screen, TileType.Tree, 14, 2);
			TileDrawing.DrawTile(Screen, TileType.Tree, 14, 8);
			TileDrawing.DrawTile(Screen, TileType.Tree, 2, 2);
			TileDrawing.DrawTile(Screen, TileType.Tree, 2, 8);
			TileDrawing.DrawTile(Screen, TileType.Tree, 13, 2);
			TileDrawing.DrawTile(Screen, TileType.Tree, 13, 8);
			BuildScreenConnections();
		}

		private void TemplateGroveWithACave() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 0, 0, Game.LastTileColumn, Game.LastTileRow);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 2, Game.LastTileColumn - 1, Game.LastTileRow - 2);
			TileDrawing.DrawTile(Screen, TileType.Water, 1, 2);
			TileDrawing.DrawTile(Screen, TileType.Water, 1, 8);
			TileDrawing.DrawTile(Screen, TileType.Water, 14, 2);
			TileDrawing.DrawTile(Screen, TileType.Water, 14, 8);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 9, 3, 12, 4);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 9, 6, 12, 7);
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 3, 6, 5, 7);
			TileDrawingTemplates.PlaceBigTreeCaveStamp(Screen, 3, 3);
			Screen.ExitCavePositionX = 4;
			Screen.ExitCavePositionY = 3;

			BuildScreenConnections();
		}

		private void TemplateRockyIsland() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 0, 0, Game.LastTileColumn, Game.LastTileRow);
			TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 2, 2, Game.LastTileColumn - 1, 7);

			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 8, 1, 10, 1);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 2, 6, 2, 7);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 14, 6, 14, 7);
			TileDrawing.FillRectWithTiles(Screen, TileType.Water, 7, 6, 9, 7);

			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 2, 1, 6, 3);
			TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 10, 1, 14, 3);
			TileDrawing.FillRectWithTiles(Screen, TileType.Boulder, 3, 6, 6, 7);
			TileDrawing.FillRectWithTiles(Screen, TileType.Boulder, 10, 6, 13, 7);

			if (Screen.ExitsSouth) {
				TileDrawing.FillRectWithTiles(Screen, TileType.BridgeVertical, 8, 6, 8, 10);
			}

			if (Screen.ExitsWest) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Bridge, 0, 5, 1, 5);
			}

			BuildScreenConnections();
		}

		private void BuildWaterBorder() {
			TileDrawing.FillPerimeterWithTiles(Screen, TileType.Water, 0, 0, Game.LastTileColumn, Game.LastTileRow);
			TileDrawing.FillPerimeterWithTiles(Screen, TileType.Water, 0, 1, Game.LastTileColumn, Game.LastTileRow - 1);
		}

		private void BuildScreenConnections() {
			if (Screen.IsDockIsland) {
				TileDrawingTemplates.BuildDockReceiving(Screen);
			} else {
				if (Screen.ExitsNorth) {
					TileDrawing.DrawTile(Screen, TileType.BridgeVertical, 8, 0);
					TileDrawing.DrawTile(Screen, TileType.BridgeVertical, 8, 1);
				}

				if (Screen.ExitsSouth) {
					TileDrawing.DrawTile(Screen, TileType.BridgeVertical, 8, Game.LastTileRow);
					TileDrawing.DrawTile(Screen, TileType.BridgeVertical, 8, Game.LastTileRow - 1);
				}

				if (Screen.ExitsWest) {
					TileDrawing.DrawTile(Screen, TileType.Bridge, 0, 5);
				}

				if (Screen.ExitsEast) {
					TileDrawing.DrawTile(Screen, TileType.Bridge, Game.LastTileColumn, 5);
				}
			}
		}

		public override void AssignEnemies() {
			int enemySet = Utilities.GetRandomInt(0, 13);
			EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.One;
			Screen.EnemyCount = Game.EnemyCountLookup[count];
			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);

			Screen.UsesMixedEnemies = false;

			if (enemySet == 0) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRed];
			} else if (enemySet == 1) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRedFast];
			} else if (enemySet == 2) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokBlue];
			} else if (enemySet == 3) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokBlueFast];
			} else if (enemySet == 4) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteBlue];
			} else if (enemySet == 5) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteRed];
			} else if (enemySet == 6) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.Peahat];
			} else if (enemySet == 7) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverBlue];
			} else if (enemySet == 8) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
			} else if (enemySet == 9) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinBlue];
			} else if (enemySet == 10) {
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinRed];
			} else if (enemySet == 11) {
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_Peahat_Peahat
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 12) {
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 13) {
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
				];
				Screen.UsesMixedEnemies = true;
			}
		}
	}
}