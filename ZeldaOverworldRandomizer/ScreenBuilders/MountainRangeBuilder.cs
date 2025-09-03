using System.Collections.Generic;
using System.Diagnostics;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.MapBuilder;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class MountainRangeBuilder : ScreenBuilder {
		public MountainRangeBuilder(Screen screen) : base(screen) { }

		private bool _leftHalfIsTunnelly;
		private bool _rightHalfIsTunnelly;

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignHalveTypeScreen();

			AssignEdges();
			WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			DoubleTopAndBottomEdges();

			Screen.PaletteInterior = Screen.PaletteBorder;

			if (Screen.IsArmosCave || Screen.IsArmosHiddenItem || Screen.IsArmosDecor) {
				BuildArmosScreen();
			} else if (Screen.IsFairyPond) {
				BuildFairyPondScreen();
			} else if (Screen.IsWhistleLake) {
				BuildWhistleLake();
			} else if (Screen.IsOpenDungeon) {
				BuildDungeonEntranceScreen();
			} else {
				if (Screen.ScreenId != Game.LinkStartScreen &&
				    !Screen.IsLake &&
				    !Screen.IsCoast &&
				    !Screen.IsAnyRoad &&
				    !_leftHalfIsTunnelly &&
				    !_rightHalfIsTunnelly &&
				    (Screen.Region.Biome == Biome.StartZone || Utilities.GetRandomInt(0, 2) <= 1)
				   ) {
					BuildDecorScreen();
				} else {
					BuildGenericScreen();
				}
			}

			if (Screen.GetScreenUp() != null && Screen.GetScreenUp().IsSecretScreen) {
				TileDrawing.DrawTile(Screen, TileType.Rock, 8, 0);
				TileDrawing.DrawTile(Screen, TileType.Rock, 8, 1);
			}

			if (Screen.IsLake && Screen.LakeSpot == NineSliceSpot.Middle) {
				TileDrawingTemplates.BuildCenterLakeRing(Screen);
			}

			if (Game.LinkStartScreen == Screen.ScreenId) {
				List<TileType> acceptableGround = new List<TileType> { TileType.Ground, TileType.Desert };
				TileType linkSpotA = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(7, 5)]];
				TileType linkSpotB = Game.TileLookupById[Screen.Tiles[Utilities.GetTileByColAndRow(8, 5)]];

				if (!acceptableGround.Contains(linkSpotA) || !acceptableGround.Contains(linkSpotB)) {
					Debug.Print("Rebuild Reason: Not an acceptable place to put Link");
					OverworldBuilder.ShouldRebuild = true;
				}
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			if (Screen.Region.Biome == Biome.StartZone) {
				AssignConstructedEdgeForStartZone(edge);
				return;
			}

			bool isVerticalEdge = edge == Direction.Up || edge == Direction.Down;
			List<bool> edgeTiles = GetSolidFilledEdgeTiles(isVerticalEdge);

			if (edge == Direction.Up && Screen.GetScreenUp() != null && 
			    Screen.GetScreenUp().IsSecretScreen ||
			    edge == Direction.Down && Screen.IsSecretScreen
			   ) {
				for (int i = 0; i < Game.TilesWide; i++) {
					edgeTiles[i] = true;
				}

				edgeTiles[8] = false;
			} else if (isVerticalEdge) {
				Screen otherScreen = edge == Direction.Up
					? Screen.GetScreenUp()
					: Screen.GetScreenDown();

				if (Screen.IsLake || Screen.IsCoast) {
					if (Screen.IsCoast && Screen.IsFirstColumn || Screen.HasWaterLeft) {
						if (otherScreen.Region.Biome != Screen.Region.Biome) {
							int start = Utilities.GetRandomInt(10, 11);
							int length = Utilities.GetRandomInt(2, 3);
							for (int i = start; i < start + length; i++) {
								edgeTiles[i] = false;
							}
						} else {
							edgeTiles[Utilities.GetRandomInt(11, 14)] = false;
						}
					} else if (Screen.IsCoast && Screen.IsLastColumn || Screen.HasWaterRight) {
						if (otherScreen.Region.Biome != Screen.Region.Biome) {
							int start = Utilities.GetRandomInt(2, 4);
							int length = Utilities.GetRandomInt(2, 3);
							for (int i = start; i < start + length; i++) {
								edgeTiles[i] = false;
							}
						} else {
							edgeTiles[Utilities.GetRandomInt(2, 5)] = false;
						}
					} else {
						if (otherScreen.Region.Biome != Screen.Region.Biome) {
							int start = Utilities.GetRandomInt(3, 11);
							int length = Utilities.GetRandomInt(2, 3);
							for (int i = start; i < start + length; i++) {
								edgeTiles[i] = false;
							}
						} else {
							edgeTiles[Utilities.GetRandomInt(3, 14)] = false;
						}
					}
				} else {
					if (otherScreen.Region.Biome != Screen.Region.Biome) {
						int start = Utilities.GetRandomInt(2, 9);
						int length = Utilities.GetRandomInt(4, 7);
						for (int i = start; i < start + length; i++) {
							if (i < 14) {
								edgeTiles[i] = false;
							}
						}
					} else {
						edgeTiles[Utilities.GetRandomInt(2, 14)] = false;
					}
				}
			} else {
				if (Screen.IsLake && Screen.LakeSpot == NineSliceSpot.Middle ||
				    Screen.HasWaterLeft && edge == Direction.Left ||
				    Screen.HasWaterRight && edge == Direction.Right
				   ) {
					edgeTiles[5] = false;
				} else {
					int lengthOfOpening = Utilities.GetRandomInt(2, 4) * 2 - 1;
					int startOfOpening = Game.TilesHigh / 2 - lengthOfOpening / 2;

					if (_leftHalfIsTunnelly && edge == Direction.Left ||
					    _rightHalfIsTunnelly && edge == Direction.Right
					   ) {
						lengthOfOpening = Utilities.GetRandomInt(1, 2);
						startOfOpening = Game.TilesHigh / 2 - lengthOfOpening / 2 + Utilities.GetRandomInt(-2, 2);
					}

					for (int i = startOfOpening; i < startOfOpening + lengthOfOpening; i++) {
						edgeTiles[i] = false;
					}
				}
			}

			if (edge == Direction.Up) {
				Screen.EdgeNorth = edgeTiles;
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = edgeTiles;
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = edgeTiles;
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = edgeTiles;
			}
		}

		private void AssignConstructedEdgeForStartZone(Direction edge) {
			bool isVerticalEdge = edge == Direction.Up || edge == Direction.Down;
			List<bool> edgeTiles = GetSolidFilledEdgeTiles(isVerticalEdge);

			if (isVerticalEdge) {
				int start = Utilities.GetRandomInt(5, 10);
				int length = Utilities.GetRandomInt(2, 4);

				if (Screen.HasWaterRight) {
					start = 9 - length;
				}

				if (Screen.HasWaterLeft) {
					if (start < 7) {
						start = 7;
					}
				}

				for (int i = start; i < start + length; i++) {
					edgeTiles[i] = false;
				}
			} else {
				int roll = Utilities.GetRandomInt(0, 1);
				int start = roll == 0
					? 2
					: 3;

				int length = roll == 0
					? 7
					: 5;

				for (int i = start; i < start + length; i++) {
					edgeTiles[i] = false;
				}
			}

			if (edge == Direction.Up) {
				Screen.EdgeNorth = edgeTiles;
			}

			if (edge == Direction.Down) {
				Screen.EdgeSouth = edgeTiles;
			}

			if (edge == Direction.Left) {
				Screen.EdgeWest = edgeTiles;
			}

			if (edge == Direction.Right) {
				Screen.EdgeEast = edgeTiles;
			}
		}

		private void AssignHalveTypeScreen() {
			if (Screen.Region.Biome == Biome.StartZone) {
				return;
			}

			if (!Screen.IsLake && !Screen.IsCoast && !Screen.IsDock) {
				if (Screen.ExitsWest) {
					if (Screen.GetScreenLeft().IsAlreadyGenerated) {
						int lengthOfExit = Screen.GetScreenLeft().EdgeEast.LastIndexOf(false) -
						                   Screen.GetScreenLeft().EdgeEast.IndexOf(false);

						_leftHalfIsTunnelly = lengthOfExit <= 3;
					} else {
						_leftHalfIsTunnelly = Utilities.GetRandomInt(0, 1) == 0;
					}
				}

				if (Screen.ExitsEast) {
					if (Screen.GetScreenRight().IsAlreadyGenerated) {
						int lengthOfExit = Screen.GetScreenRight().EdgeWest.LastIndexOf(false) -
						                   Screen.GetScreenRight().EdgeWest.IndexOf(false);

						_rightHalfIsTunnelly = lengthOfExit <= 3;
					} else {
						_rightHalfIsTunnelly = Utilities.GetRandomInt(0, 1) == 0;
					}
				}

				if (_leftHalfIsTunnelly && _rightHalfIsTunnelly) {
					if (Utilities.GetRandomInt(0, 1) == 0) {
						_rightHalfIsTunnelly = false;
					} else {
						_leftHalfIsTunnelly = false;
					}
				}
			}
		}

		private void BuildArmosScreen() {
			TileDrawingTemplates.BuildArmosLayout(Screen);

			AddCorners(TileType.Rock);
			AddCracks();
			AddLadders();

			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			if (Screen.IsArmosDecor && Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}
		}

		private void BuildFairyPondScreen() {
			TileDrawingTemplates.BuildFairyLake(Screen);
			AddCorners(TileType.Rock);
			AddCracks();
			AddLadders();
		}

		private void BuildWhistleLake() {
			TileDrawingTemplates.BuildWhistleLake(Screen);
			AddCaveEntrance();
			AddCorners(TileType.Rock);
			AddCracks();
			AddLadders();
		}

		private void BuildDungeonEntranceScreen() {
			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			TileDrawingTemplates.BuildDungeonEntrance(Screen);

			AddCorners(TileType.Rock);
			AddCracks();
			AddLadders();

			if (!Screen.IsLake && !Screen.IsCoast && Utilities.GetRandomInt(0, 1) == 1) {
				Screen.PaletteInterior = 2;
			} else {
				Screen.PaletteInterior = 3;
			}
		}

		private void BuildGenericScreen() {
			if (_leftHalfIsTunnelly || _rightHalfIsTunnelly) {
				BuildNarrowHalf();
			} else {
				CarveMountain();
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);

				if (Screen.LakeSpot == NineSliceSpot.Bottom) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 5, Game.LastTileColumn - 1, 5);
				} else if (Screen.LakeSpot == NineSliceSpot.Top) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 5, Game.LastTileColumn - 1, 5);
				} else if (Screen.LakeSpot == NineSliceSpot.BottomRight) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, 5, 8, 5);
				} else if (Screen.LakeSpot == NineSliceSpot.BottomLeft) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 7, 5, Game.LastTileColumn - 1, 5);
				}
			}

			if (Screen.IsAnyRoad) {
				TileDrawingTemplates.BuildAnyRoadFormation(Screen);
			}

			if (Screen.IsDock) {
				TileDrawingTemplates.BuildDock(Screen);
				ClearHalfTrees();
			}

			CarveExits();
			CleanSoloRocks();

			if (!Screen.IsAnyRoad && Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}

			AddCorners(TileType.Rock);

			if (Screen.ScreenId != 39) {
				AddCracks();
			}

			AddLadders();

			if (Screen.IsCoast && FrontEnd.MainWindow.GenerateEnhancedBeaches) {
				AddWatersideGrass();
				AddPalmTrees();
			}
		}

		private void BuildDecorScreen() {
			int roll = Utilities.GetRandomInt(0, 1);

			if (roll == 0) {
				int start = Utilities.GetRandomInt(2, 5);
				for (int i = start; i < Game.TilesWide - 3; i++) {
					TileDrawingTemplates.PlaceBigTreeStamp(Screen, i, 3);
					i += 2 + Utilities.GetRandomInt(0, 2);
				}

				start = Utilities.GetRandomInt(2, 5);
				for (int i = start; i < Game.TilesWide - 3; i++) {
					TileDrawingTemplates.PlaceBigTreeStamp(Screen, i, 6);
					i += 2 + Utilities.GetRandomInt(0, 2);
				}
			} else if (roll == 1) {
				int style = Utilities.GetRandomInt(0, 1);
				TileType tileType = Utilities.GetRandomInt(0, 4) == 0
					? TileType.Tree
					: TileType.Boulder;

				if (style == 0) {
					for (int x = 3; x <= 9; x += 6) {
						int rows = Utilities.GetRandomInt(2, 3);
						int length = Utilities.GetRandomInt(2, 3);

						if (rows == 3) {
							TileDrawing.FillRectWithTiles(Screen, tileType, x, 5, x + length, 5);
							TileDrawing.FillRectWithTiles(Screen, tileType, x, 3, x + length, 3);
							TileDrawing.FillRectWithTiles(Screen, tileType, x, 7, x + length, 7);
						} else {
							TileDrawing.FillRectWithTiles(Screen, tileType, x, 4, x + length, 4);
							TileDrawing.FillRectWithTiles(Screen, tileType, x, 6, x + length, 6);
						}
					}
				}

				if (style == 1) {
					int gap = Utilities.GetRandomInt(2, 5);
					int columns = Utilities.GetRandomInt(2, 7);

					int totalWidth = columns * gap - gap + 1;

					while (totalWidth >= Game.TilesWide - 4) {
						columns--;
						totalWidth = columns * gap - gap + 1;
					}

					int start = Game.TilesWide / 2 - totalWidth / 2;

					for (int column = 0; column < columns; column++) {
						int x = start + column * gap;
						int rows = Utilities.GetRandomInt(1, 3);

						if (rows == 1 || rows == 3) {
							TileDrawing.DrawTile(Screen, tileType, x, 5);
							if (rows == 3) {
								TileDrawing.DrawTile(Screen, tileType, x, 3);
								TileDrawing.DrawTile(Screen, tileType, x, 7);
							}
						} else {
							TileDrawing.DrawTile(Screen, tileType, x, 4);
							TileDrawing.DrawTile(Screen, tileType, x, 6);
						}
					}
				}
			}

			Screen.PaletteInterior = Utilities.GetRandomInt(2, 3);

			if (Screen.IsDock) {
				TileDrawingTemplates.BuildDock(Screen);
				ClearHalfTrees();
			}

			if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}

			AddCorners(TileType.Rock);
			AddCracks();
			AddLadders();

			if (Screen.IsCoast) {
				AddWatersideGrass();
			}
		}

		private void CarveMountain() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);
				bool isInner = row > 1 && row < Game.TilesHigh - 2 && col > 0 && col < Game.TilesWide - 1;

				if (isInner) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Rock];
				}
			}

			int columnIndex = 1;

			while (columnIndex < Game.TilesWide - 2) {
				int width = Utilities.GetRandomInt(1, 2);

				int startRow = columnIndex > 3 && columnIndex < Game.TilesWide - 4 && !Screen.IsLake &&
				               !Screen.IsCoast
					? Utilities.GetRandomInt(2, 6)
					: Utilities.GetRandomInt(2, 3);

				int height = Game.TilesHigh - startRow - 4 > 2
					? Utilities.GetRandomInt(2, Game.TilesHigh - startRow - 4)
					: Game.TilesHigh - startRow - 4;

				bool lakeOnTheLeft = Screen.IsLake &&
				                     (Screen.LakeSpot == NineSliceSpot.TopRight ||
				                      Screen.LakeSpot == NineSliceSpot.Right ||
				                      Screen.LakeSpot == NineSliceSpot.BottomRight);
				bool lakeOnTheRight = Screen.IsLake &&
				                      (Screen.LakeSpot == NineSliceSpot.TopLeft ||
				                       Screen.LakeSpot == NineSliceSpot.Left ||
				                       Screen.LakeSpot == NineSliceSpot.BottomLeft);

				while (columnIndex < 10 && lakeOnTheLeft && startRow + height < 5) {
					height++;
				}

				while (columnIndex > 4 && lakeOnTheRight && startRow + height < 5) {
					height++;
				}

				while (columnIndex > 4 && Screen.IsDock && startRow + height < 5) {
					height++;
				}

				while (columnIndex + width > Game.TilesWide - 2) {
					width--;
				}

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, columnIndex, startRow, columnIndex + width,
					startRow + height);

				columnIndex += width;
			}
		}

		private void BuildNarrowHalf() {
			int startCol = _leftHalfIsTunnelly
				? 1
				: 8;

			int endCol = _leftHalfIsTunnelly
				? 7
				: 15;

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (col >= startCol && col <= endCol && !Utilities.IsBorderTile(tileIndex)) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Rock];
				}
			}

			List<bool> edgeToUse = _leftHalfIsTunnelly
				? Screen.EdgeWest
				: Screen.EdgeEast;

			for (int edgeIndex = 0; edgeIndex < edgeToUse.Count; edgeIndex++) {
				if (!edgeToUse[edgeIndex]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, startCol, edgeIndex, endCol, edgeIndex);
				}
			}

			for (int edgeIndex = startCol; edgeIndex < Screen.EdgeNorth.Count; edgeIndex++) {
				if (!Screen.EdgeNorth[edgeIndex] && edgeIndex <= endCol) {
					int bottom = edgeToUse.LastIndexOf(false);
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, edgeIndex, 0, edgeIndex, bottom);
				}
			}

			for (int edgeIndex = startCol; edgeIndex < Screen.EdgeSouth.Count; edgeIndex++) {
				if (!Screen.EdgeSouth[edgeIndex] && edgeIndex <= endCol) {
					int top = edgeToUse.IndexOf(false);
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, edgeIndex, top, edgeIndex, Game.LastTileRow);
				}
			}

			if (_leftHalfIsTunnelly) {
				int lastWidth = 6;
				for (int tileIndex = 24; tileIndex < Screen.Tiles.Count; tileIndex += Game.TilesWide) {
					TileType tileLeft = Utilities.GetTileLeft(Screen, tileIndex);

					if (tileLeft == TileType.Rock) {
						int row = Utilities.GetRowFromTileIndex(tileIndex);
						int thisWidth = Utilities.GetRandomInt(0, lastWidth);
						lastWidth = thisWidth;

						if (thisWidth == 0) {
							break;
						}

						TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 8, row, 8 + thisWidth, row);
					} else {
						break;
					}
				}
			}

			if (_rightHalfIsTunnelly) {
				int lastWidth = 6;
				for (int tileIndex = 23; tileIndex < Screen.Tiles.Count; tileIndex += Game.TilesWide) {
					TileType tileRight = Utilities.GetTileRight(Screen, tileIndex);

					if (tileRight == TileType.Rock) {
						int row = Utilities.GetRowFromTileIndex(tileIndex);
						int thisWidth = Utilities.GetRandomInt(0, lastWidth);
						lastWidth = thisWidth;

						if (thisWidth == 0) {
							break;
						}

						TileDrawing.FillRectWithTiles(Screen, TileType.Rock, 7 - thisWidth, row, 7, row);
					} else {
						break;
					}
				}
			}
		}

		private void AddCracks() {
			List<int> topValidCrackIndexes = new List<int>();
			List<int> bottomValidCrackIndexes = new List<int>();

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int col = Utilities.GetColFromTileIndex(tileIndex);
				int row = Utilities.GetRowFromTileIndex(tileIndex);

				if (col > 1 && col < Game.LastTileColumn - 1 && row > 0 && row < Game.LastTileRow) {
					if (Utilities.GetTile(Screen, tileIndex - 1) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex + 1) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex + 2) == TileType.Rock &&
					    Utilities.GetTileDown(Screen, tileIndex) == TileType.Ground &&
					    Utilities.GetTileDown(Screen, tileIndex + 1) == TileType.Ground
					   ) {
						topValidCrackIndexes.Add(tileIndex);
					}

					if (Utilities.GetTile(Screen, tileIndex - 1) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex + 1) == TileType.Rock &&
					    Utilities.GetTile(Screen, tileIndex + 2) == TileType.Rock &&
					    Utilities.GetTileUp(Screen, tileIndex) == TileType.Ground &&
					    Utilities.GetTileUp(Screen, tileIndex + 1) == TileType.Ground
					   ) {
						bottomValidCrackIndexes.Add(tileIndex);
					}
				}
			}

			if (topValidCrackIndexes.Count > 0) {
				int tileIndex = topValidCrackIndexes[Utilities.GetRandomInt(0, topValidCrackIndexes.Count - 1)];
				Screen.Tiles[tileIndex] = Game.TileLookup[TileType.RockBottomRight];
				Screen.Tiles[tileIndex + 1] = Game.TileLookup[TileType.RockBottomLeft];
			}

			if (bottomValidCrackIndexes.Count > 0) {
				int tileIndex = bottomValidCrackIndexes[Utilities.GetRandomInt(0, bottomValidCrackIndexes.Count - 1)];
				Screen.Tiles[tileIndex] = Game.TileLookup[TileType.RockTopRight];
				Screen.Tiles[tileIndex + 1] = Game.TileLookup[TileType.RockTopLeft];
			}
		}

		private void CarveExits() {
			for (int edgeIndex = 0; edgeIndex < Screen.EdgeWest.Count; edgeIndex++) {
				if (!Screen.EdgeWest[edgeIndex]) {
					int tileIndex = Utilities.GetTileByColAndRow(0, edgeIndex);

					for (int i = tileIndex; i < tileIndex + 15; i++) {
						TileType tile = Utilities.GetTile(Screen, i);
						int col = Utilities.GetColFromTileIndex(i);

						if (tile != TileType.Water &&
						    tile != TileType.Bridge &&
						    tile != TileType.BridgeVertical &&
						    (tile == TileType.Rock || col <= 1)
						   ) {
							Screen.Tiles[i] = Game.TileLookup[TileType.Ground];
						} else {
							break;
						}
					}
				}
			}

			for (int edgeIndex = 0; edgeIndex < Screen.EdgeEast.Count; edgeIndex++) {
				if (!Screen.EdgeEast[edgeIndex]) {
					int tileIndex = Utilities.GetTileByColAndRow(Game.LastTileColumn, edgeIndex);

					for (int i = tileIndex; i < tileIndex - 15; i--) {
						TileType tile = Utilities.GetTile(Screen, i);
						int col = Utilities.GetColFromTileIndex(i);

						if (tile != TileType.Water && tile != TileType.Bridge && tile != TileType.BridgeVertical &&
						    (tile == TileType.Rock || col >= Game.LastTileColumn - 1)
						   ) {
							Screen.Tiles[i] = Game.TileLookup[TileType.Ground];
						} else {
							break;
						}
					}
				}
			}

			for (int edgeIndex = 0; edgeIndex < Screen.EdgeNorth.Count; edgeIndex++) {
				if (!Screen.EdgeNorth[edgeIndex]) {
					int tileIndex = Utilities.GetTileByColAndRow(edgeIndex, 0);

					for (int i = tileIndex; i < Game.TilesHigh * 16; i += 16) {
						TileType tile = Utilities.GetTile(Screen, i);
						int row = Utilities.GetRowFromTileIndex(i);

						if (tile != TileType.Water && tile != TileType.Bridge && tile != TileType.BridgeVertical &&
						    (tile == TileType.Rock || row <= 1)
						) {
							Screen.Tiles[i] = Game.TileLookup[TileType.Ground];
						} else {
							break;
						}
					}
				}
			}

			for (int edgeIndex = 0; edgeIndex < Screen.EdgeSouth.Count; edgeIndex++) {
				if (!Screen.EdgeSouth[edgeIndex]) {
					int tileIndex = Utilities.GetTileByColAndRow(edgeIndex, Game.LastTileRow);

					for (int i = tileIndex; i > 0; i -= 16) {
						TileType tile = Utilities.GetTile(Screen, i);
						int row = Utilities.GetRowFromTileIndex(i);

						if (tile != TileType.Water && tile != TileType.Bridge && tile != TileType.BridgeVertical &&
						    (tile == TileType.Rock || row >= Game.LastTileRow - 1)
						   ) {
							Screen.Tiles[i] = Game.TileLookup[TileType.Ground];
						} else {
							break;
						}
					}
				}
			}

			if (Screen.ExitsNorth && Screen.GetScreenUp().IsSecretScreen) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 7,2,9,3);
			}
		}

		private void AddLadders() {
			if (Screen.IsLake && Screen.LakeSpot == NineSliceSpot.Middle) {
				return;
			}

			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (row == 0 && col > 1 && col < Game.LastScreenColumn) {
					int tile = Screen.Tiles[tileIndex];
					int tileLeft = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)];
					int tileRight = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)];

					if (
						tile == Game.TileLookup[TileType.Ground] &&
						tileLeft == Game.TileLookup[TileType.Rock] &&
						tileRight == Game.TileLookup[TileType.Rock]
					) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ladder];
					}
				}

				if (row > 0 && row < 7) {
					int tileLeft = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)];
					int tileRight = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)];
					int tileUp = Screen.Tiles[Utilities.GetTileByColAndRow(col, row - 1)];

					if (
						tileUp == Game.TileLookup[TileType.Ladder] &&
						tileLeft == Game.TileLookup[TileType.Rock] &&
						tileRight == Game.TileLookup[TileType.Rock]
					) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ladder];
					}
				}
			}

			for (int tileIndex = Screen.Tiles.Count - 1; tileIndex >= 0; tileIndex--) {
				int row = Utilities.GetRowFromTileIndex(tileIndex);
				int col = Utilities.GetColFromTileIndex(tileIndex);

				if (row == Game.TilesHigh - 1 && col > 1 && col < Game.LastScreenColumn) {
					int tile = Screen.Tiles[tileIndex];
					int tileLeft = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)];
					int tileRight = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)];

					if (
						tile == Game.TileLookup[TileType.Ground] &&
						tileLeft == Game.TileLookup[TileType.Rock] &&
						tileRight == Game.TileLookup[TileType.Rock]
					) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ladder];
					}
				}

				if (row > 2 && row < Game.TilesHigh - 1) {
					int tileLeft = Screen.Tiles[Utilities.GetTileByColAndRow(col - 1, row)];
					int tileRight = Screen.Tiles[Utilities.GetTileByColAndRow(col + 1, row)];
					int tileDown = Screen.Tiles[Utilities.GetTileByColAndRow(col, row + 1)];

					if (
						tileDown == Game.TileLookup[TileType.Ladder] &&
						tileLeft == Game.TileLookup[TileType.Rock] &&
						tileRight == Game.TileLookup[TileType.Rock]
					) {
						Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ladder];
					}
				}
			}
		}

		private void AddPalmTrees() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				bool isEdgeTile = Utilities.IsThickBorderTile(tileIndex);
				bool isDesert = Utilities.GetTile(Screen, tileIndex) == TileType.Desert;
				bool isCoastAdjacent = Utilities.GetTileDown(Screen, tileIndex) == TileType.Water ||
				                       Utilities.GetTileRight(Screen, tileIndex) == TileType.Water ||
				                       Utilities.GetTileLeft(Screen, tileIndex) == TileType.Water;

				bool isRandomlyATree = Utilities.GetRandomInt(0, 5) == 0;

				if (!isEdgeTile && isDesert && isCoastAdjacent && isRandomlyATree) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.PalmTree];
				}
			}

			if (Screen.ScreenId == Game.LinkStartScreen) {
				if (Screen.Tiles[Utilities.GetTileByColAndRow(7, 5)] == Game.TileLookup[TileType.PalmTree] ||
				    Screen.Tiles[Utilities.GetTileByColAndRow(8, 5)] == Game.TileLookup[TileType.PalmTree]
				   ) {
					Screen.Tiles[Utilities.GetTileByColAndRow(7, 5)] = Game.TileLookup[TileType.Desert];
					Screen.Tiles[Utilities.GetTileByColAndRow(8, 5)] = Game.TileLookup[TileType.Desert];
				}
			}
		}

		private void ClearHalfTrees() {
			for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
				if (
					Utilities.GetTile(Screen, tileIndex) == TileType.BigTreeTopLeft &&
					Utilities.GetTileRight(Screen, tileIndex) != TileType.BigTreeTopRight ||
					Utilities.GetTile(Screen, tileIndex) == TileType.BigTreeBottomLeft &&
					Utilities.GetTileRight(Screen, tileIndex) != TileType.BigTreeBottomRight ||
					Utilities.GetTile(Screen, tileIndex) == TileType.BigTreeTopRight &&
					Utilities.GetTileLeft(Screen, tileIndex) != TileType.BigTreeTopLeft ||
					Utilities.GetTile(Screen, tileIndex) == TileType.BigTreeBottomRight &&
					Utilities.GetTileLeft(Screen, tileIndex) != TileType.BigTreeBottomLeft
				) {
					Screen.Tiles[tileIndex] = Game.TileLookup[TileType.Ground];
				}
			}
		}

		public override void AssignEnemies() {
			if (Screen.IsFairyPond) {
				return;
			}

			if (Screen.Region.Biome == Biome.StartZone) {
				if (Screen.ScreenId != Game.LinkStartScreen) {
					AssignStartZoneEnemies();
				}
			} else {
				AssignMountainRangeEnemies();
			}
		}

		private void AssignStartZoneEnemies() {
			if (Screen.IsLake || Screen.IsCoast) {
				if (Utilities.GetRandomInt(0, 2) == 0) {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
					Screen.UsesMixedEnemies = false;
					Screen.EnemiesEnterFromSides = false;
				} else {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemiesEnterFromSides = false;
				}
			} else {
				int enemySet = Utilities.GetRandomInt(0, 3);

				if (enemySet == 0) {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRed];
					Screen.UsesMixedEnemies = false;
					Screen.EnemiesEnterFromSides = false;
				} else if (enemySet == 1) {
					EnemyCount count = Utilities.GetRandomInt(0, 2) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteRed];
					Screen.UsesMixedEnemies = false;
					Screen.EnemiesEnterFromSides = false;
				} else if (enemySet == 2) {
					EnemyCount count = Utilities.GetRandomInt(0, 2) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemiesEnterFromSides = false;
				} else if (enemySet == 3) {
					EnemyCount count = Utilities.GetRandomInt(0, 2) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoBlue_OctoBlue_OctoRedFast_OctoBlueFast
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemiesEnterFromSides = false;
				}
			}
		}

		private void AssignMountainRangeEnemies() {
			if (Screen.IsLake) {
				if (Utilities.GetRandomInt(0, 2) == 0) {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LeeverRed];
					Screen.UsesMixedEnemies = false;
					Screen.EnemiesEnterFromSides = false;
				} else {
					EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
					Screen.EnemyCount = Game.EnemyCountLookup[count];
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.LeeverBlue_LeeverRed_Peahat_Peahat_LeeverBlue_Peahat
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemiesEnterFromSides = false;
				}

				if (Screen.IsArmosCave && Screen.LakeSpot == NineSliceSpot.Top) {
					Screen.EnemiesEnterFromSides = true;
				}
			} else if (Screen.Row == 0) {
				int enemySet = Utilities.GetRandomInt(0, 4);
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];

				Screen.EnemiesEnterFromSides = false;

				if (enemySet == 0) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Four];
				} else if (enemySet == 1) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LynelBlue];
				} else if (enemySet == 2) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.LynelRed];
				} else if (enemySet == 3 && !(Screen.TotalExits > 1 && Screen.ExitsSouth)) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.FallingRocksGenerator];
				} else {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue
					];
					Screen.UsesMixedEnemies = true;
				}
			} else if (Screen.Region.Screens.Count == 1) {
				int enemySet = Utilities.GetRandomInt(0, 2);
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];

				Screen.EnemiesEnterFromSides = false;

				if (enemySet == 0) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteBlue];
				} else if (enemySet == 1) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue
					];
					Screen.UsesMixedEnemies = true;
				} else if (enemySet == 2) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
					];
					Screen.UsesMixedEnemies = true;
				}
			} else {
				int enemySet = Utilities.GetRandomInt(0, 5);
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];

				Screen.EnemiesEnterFromSides = false;

				if (enemySet == 0) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.LynelBlue_LynelBlue_LynelRed_LeeverBlue_LeeverRed_None
					];
					Screen.UsesMixedEnemies = true;
					Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Four];
				} else if (enemySet == 1) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteBlue];
				} else if (enemySet == 2 && !(Screen.TotalExits > 1 && Screen.ExitsSouth)) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.FallingRocksGenerator];
				} else if (enemySet == 2 && Screen.TotalExits > 1 && Screen.ExitsSouth) {
					Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.TektiteBlue];
				} else if (enemySet == 3) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.Peahat_Peahat_LynelRed_LynelBlue_LynelRed_LynelBlue
					];
					Screen.UsesMixedEnemies = true;
				} else if (enemySet == 4) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue
					];
					Screen.UsesMixedEnemies = true;
				} else if (enemySet == 5) {
					Screen.EnemyId = Game.MixedEnemyTypeLookup[
						MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
					];
					Screen.UsesMixedEnemies = true;
				}
			}

			if (!Utilities.EnemiesCanSpawnOnScreen(Screen)) {
				Screen.EnemiesEnterFromSides = true;
			}
		}
	}
}