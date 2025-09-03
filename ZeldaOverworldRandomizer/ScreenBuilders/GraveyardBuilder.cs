using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class GraveyardBuilder : ScreenBuilder {
		public GraveyardBuilder(Screen screen) : base(screen) { }

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			DoubleTopAndBottomEdges();

			Screen.PaletteInterior = 0;
			Screen.EnvironmentColor = EnvironmentColor.Grey;

			if (Screen.IsFairyPond) {
				TileDrawingTemplates.BuildFairyLake(Screen);
				TileDrawing.FillRectWithLatticeTiles(Screen, TileType.Grave, 3, 4, 3, 6);
				TileDrawing.FillRectWithLatticeTiles(Screen, TileType.Grave, 12, 4, 12, 6);
			} else if (Screen.IsOpenDungeon) {
				TileDrawingTemplates.BuildDungeonEntrance(Screen);
			} else {
				BuildGenericScreen();
			}

			if (Screen.GetScreenUp() != null && Screen.GetScreenUp().IsSecretScreen) {
				TileDrawing.DrawTile(Screen, TileType.Rock, 8, 0);
				TileDrawing.DrawTile(Screen, TileType.Rock, 8, 1);
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			bool isVerticalEdge = edge == Direction.Down || edge == Direction.Up;

			List<bool> edgeProfile = GetSolidFilledEdgeTiles(isVerticalEdge);

			if (edge == Direction.Up && Screen.GetScreenUp() != null && Screen.GetScreenUp().IsSecretScreen ||
			    edge == Direction.Down && Screen.IsSecretScreen
			) {
				for (int i = 0; i < Game.TilesWide; i++) {
					edgeProfile[i] = true;
				}

				edgeProfile[8] = false;
			} else if (isVerticalEdge) {
				for (int i = 0; i < edgeProfile.Count; i++) {
					edgeProfile[i] = i < 2 || i > 13;
				}
			} else {
				for (int i = 0; i < edgeProfile.Count; i++) {
					edgeProfile[i] = i < 2 || i > 8;
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

			for (int i = 3; i <= 12; i += 3) {
				for (int j = 3; j <= 7; j += 2) {
					bool allNineAreGround =
						Screen.Tiles[Utilities.GetTileByColAndRow(i - 1, j - 1)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i, j - 1)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i + 1, j - 1)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i - 1, j)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i, j)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i + 1, j)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i - 1, j + 1)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i, j + 1)] == Game.TileLookup[TileType.Ground] &&
						Screen.Tiles[Utilities.GetTileByColAndRow(i + 2, j + 1)] == Game.TileLookup[TileType.Ground];

					if (allNineAreGround) {
						Screen.Tiles[Utilities.GetTileByColAndRow(i, j)] = Game.TileLookup[TileType.Grave];
					}
				}
			}


			if (Screen.ScreenId == 69) {
				int v = 9;
				v++;
			}

			if (Screen.CaveDestination > 0) {
				List<CaveType> definitePushable = new List<CaveType> {
					CaveType.BlueRingShop, CaveType.Letter, CaveType.WhiteSword, CaveType.MasterSword
				};
				bool isDefinitePushable = definitePushable.Contains(Game.CaveLookupById[Screen.CaveDestination]);
				bool isMaybePushable = !isDefinitePushable && Screen.CaveIsHidden && Utilities.GetRandomInt(0, 2) == 0;

				if (isDefinitePushable || isMaybePushable) {
					List<int> gravesThatCanBePushed = new List<int>();

					if (Screen.Tiles[Utilities.GetTileByColAndRow(5, 3)] == Game.TileLookup[TileType.Grave]) {
						gravesThatCanBePushed.Add(Utilities.GetTileByColAndRow(5, 3));
					}

					if (Screen.Tiles[Utilities.GetTileByColAndRow(6, 5)] == Game.TileLookup[TileType.Grave]) {
						gravesThatCanBePushed.Add(Utilities.GetTileByColAndRow(6, 5));
					}

					if (Screen.Tiles[Utilities.GetTileByColAndRow(9, 5)] == Game.TileLookup[TileType.Grave]) {
						gravesThatCanBePushed.Add(Utilities.GetTileByColAndRow(9, 5));
					}

					if (gravesThatCanBePushed.Count > 0) {
						int pushableGraveTileIndex =
							gravesThatCanBePushed[Utilities.GetRandomInt(0, gravesThatCanBePushed.Count - 1)];

						TileDrawing.DrawTile(Screen, TileType.GravePushable, pushableGraveTileIndex);

						if (pushableGraveTileIndex == Utilities.GetTileByColAndRow(5, 3)) {
							Screen.PushedStairsPositionId = 0;
							Screen.ExitCavePositionX = 5;
							Screen.ExitCavePositionY = 2;
						} else if (pushableGraveTileIndex == Utilities.GetTileByColAndRow(6, 5)) {
							Screen.PushedStairsPositionId = 3;
							Screen.ExitCavePositionX = 6;
							Screen.ExitCavePositionY = 4;
						} else if (pushableGraveTileIndex == Utilities.GetTileByColAndRow(9, 5)) {
							Screen.PushedStairsPositionId = 2;
							Screen.ExitCavePositionX = 9;
							Screen.ExitCavePositionY = 4;
						}
					} else {
						AddCaveEntrance();
					}
				} else {
					AddCaveEntrance();
				}
			}
		}

		public override void AssignEnemies() {
			if (Screen.IsFairyPond) {
				return;
			}

			Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.One];
			Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.GhiniMaster];
			Screen.UsesMixedEnemies = false;
			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen);
		}
	}
}