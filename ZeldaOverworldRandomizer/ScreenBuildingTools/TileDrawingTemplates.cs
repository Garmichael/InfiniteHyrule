using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.ScreenBuildingTools {
	public static class TileDrawingTemplates {
		public static void BuildDungeonEntrance(Screen screen) {
			int roll = Utilities.GetRandomInt(0, 100);

			int xOffset = 0;

			if (screen.ExitsWest && !screen.ExitsEast) {
				xOffset = 1;
			}

			int leftX = 3 + xOffset;
			int rightX = 10 + xOffset;

			TileDrawing.FillRectWithTiles(screen, TileType.Ground, 1, 2, Game.TilesWide - 2, 8);

			TileDrawing.DrawTile(screen, TileType.DungeonMouthTopLeft, 6 + xOffset, 3);
			TileDrawing.DrawTile(screen, TileType.DungeonMouthTopMiddle, 7 + xOffset, 3);
			TileDrawing.DrawTile(screen, TileType.DungeonMouthTopRight, 8 + xOffset, 3);
			TileDrawing.DrawTile(screen, TileType.DungeonMouthBottomLeft, 6 + xOffset, 4);
			TileDrawing.DrawTile(screen, TileType.Cave, 7 + xOffset, 4);
			TileDrawing.DrawTile(screen, TileType.DungeonMouthBottomRight, 8 + xOffset, 4);

			screen.ExitCavePositionX = 7 + xOffset;
			screen.ExitCavePositionY = 3;

			if (roll < 10) {
				TileDrawing.DrawTile(screen, TileType.Armos, leftX, 4);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX + 1, 4);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX, 6);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX + 1, 6);

				TileDrawing.DrawTile(screen, TileType.Armos, rightX, 4);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX + 1, 4);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX, 6);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX + 1, 6);
			} else if (roll < 20) {
				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, leftX, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, leftX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, leftX, 4);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, leftX + 1, 4);

				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, leftX + 1, 6);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, leftX + 2, 6);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, leftX + 1, 7);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, leftX + 2, 7);

				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, rightX, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, rightX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, rightX, 4);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, rightX + 1, 4);

				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, rightX - 1, 6);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, rightX, 6);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, rightX - 1, 7);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, rightX, 7);
			} else if (roll < 30) {
				TileDrawing.DrawTile(screen, TileType.Armos, leftX, 3);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX, 5);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX + 1, 5);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX, 7);
				TileDrawing.DrawTile(screen, TileType.Armos, leftX + 1, 7);

				TileDrawing.DrawTile(screen, TileType.Armos, rightX, 3);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX, 5);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX + 1, 5);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX, 7);
				TileDrawing.DrawTile(screen, TileType.Armos, rightX + 1, 7);
			} else if (roll < 40) {
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX, 3);
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX, 5);
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX + 1, 5);
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX, 7);
				TileDrawing.DrawTile(screen, TileType.Boulder, leftX + 1, 7);

				TileDrawing.DrawTile(screen, TileType.Boulder, rightX, 3);
				TileDrawing.DrawTile(screen, TileType.Boulder, rightX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Boulder, rightX, 5);
				TileDrawing.DrawTile(screen, TileType.Boulder, rightX + 1, 5);
				TileDrawing.DrawTile(screen, TileType.Boulder, rightX, 7);
				TileDrawing.DrawTile(screen, TileType.Boulder, rightX + 1, 7);
			} else if (roll < 50) {
				TileDrawing.DrawTile(screen, TileType.Tree, leftX, 3);
				TileDrawing.DrawTile(screen, TileType.Tree, leftX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Tree, leftX, 4);
				TileDrawing.DrawTile(screen, TileType.Tree, leftX, 6);

				TileDrawing.DrawTile(screen, TileType.Tree, rightX, 3);
				TileDrawing.DrawTile(screen, TileType.Tree, rightX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.Tree, rightX + 1, 4);
				TileDrawing.DrawTile(screen, TileType.Tree, rightX + 1, 6);

				TileDrawing.FillRectWithTiles(screen, TileType.Tree, leftX, 7, rightX + 1, 7);
			} else if (roll < 60) {
				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, leftX, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, leftX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, leftX, 4);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, leftX + 1, 4);

				TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, rightX, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, rightX + 1, 3);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, rightX, 4);
				TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, rightX + 1, 4);

				TileDrawing.FillRectWithTiles(screen, TileType.Tree, leftX, 6, rightX + 1, 7);
				TileDrawing.DrawTile(screen, TileType.Tree, leftX + 2, 7);

				TileDrawing.FillRectWithTiles(screen, TileType.Tree, rightX, 6, rightX + 1, 7);
				TileDrawing.DrawTile(screen, TileType.Tree, rightX - 1, 7);
			} else if (roll < 70) {
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, leftX, 3, leftX + 1, 4);
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, rightX, 3, rightX + 1, 4);
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, leftX, 6, leftX + 1, 7);
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, rightX, 6, rightX + 1, 7);
			} else if (roll < 80) {
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, leftX + 1, 4, leftX + 1, 7);
				TileDrawing.FillRectWithTiles(screen, TileType.Tree, rightX, 4, rightX, 7);
			} else if (roll < 90) {
				TileDrawing.FillRectWithTiles(screen, TileType.Boulder, leftX, 3, leftX + 1, 7);
				TileDrawing.FillRectWithTiles(screen, TileType.Boulder, rightX, 3, rightX + 1, 7);
				TileDrawing.FillRectWithTiles(screen, TileType.Boulder, leftX + 3, 6, leftX + 5, 7);
			} else {
				TileDrawing.FillRectWithTiles(screen, TileType.Rock, leftX, 6, rightX + 1, 7);
			}
		}

		public static void BuildAnyRoadFormation(Screen screen) {
			const int x = 7;
			const int y = 3;
			TileDrawing.FillRectWithTiles(screen, TileType.Ground, x, y, x + 4, y + 4);
			TileDrawing.DrawTile(screen, TileType.BoulderPushable, x + 1, y + 2);
			TileDrawing.DrawTile(screen, TileType.Boulder, x + 2, y + 1);
			TileDrawing.DrawTile(screen, TileType.Boulder, x + 2, y + 3);
			TileDrawing.DrawTile(screen, TileType.Boulder, x + 3, y + 2);
			screen.PushedStairsPositionId = 2;
			screen.ExitCavePositionX = x;
			screen.ExitCavePositionY = y + 2;
		}

		public static void BuildCenterLakeRing(Screen screen) {
			TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, Game.TilesWide - 1, 0);
			TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 0, Game.TilesHigh - 1);
			TileDrawing.FillRectWithTiles(screen, TileType.Water, Game.TilesWide - 1, 0, Game.TilesWide - 1,
				Game.TilesHigh - 1);
			TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, Game.TilesHigh - 1, Game.TilesWide - 1,
				Game.TilesHigh - 1);

			if (screen.ExitsWest) {
				TileDrawing.DrawTile(screen, TileType.Bridge, 0, 5);
			}

			if (screen.ExitsEast) {
				TileDrawing.DrawTile(screen, TileType.Bridge, Game.TilesWide - 1, 5);
			}

			if (screen.ExitsNorth) {
				TileDrawing.DrawTile(screen, TileType.BridgeVertical, 8, 0);
			}

			if (screen.ExitsSouth) {
				TileDrawing.DrawTile(screen, TileType.BridgeVertical, 8, Game.TilesHigh - 1);
			}
		}

		public static void BuildFairyLake(Screen screen) {
			TileDrawing.FillRectWithTiles(screen, TileType.Water, 5, 3, 10, 6);
			TileDrawing.FillRectWithTiles(screen, TileType.Tree, 3, 4, 3, 4);
			TileDrawing.FillRectWithTiles(screen, TileType.Tree, 3, 6, 3, 6);
			TileDrawing.FillRectWithTiles(screen, TileType.Tree, 12, 4, 12, 4);
			TileDrawing.FillRectWithTiles(screen, TileType.Tree, 12, 6, 12, 6);

			if (!screen.IsGraveyard) {
				screen.PaletteInterior = 2;
			}

			screen.EnemyId = 47;
		}

		public static void BuildWhistleLake(Screen screen) {
			int left = Utilities.GetRandomInt(3, 5);
			int right = Utilities.GetRandomInt(9, 12);
			int top = Utilities.GetRandomInt(3, 4);
			int bottom = Utilities.GetRandomInt(6, 7);

			TileDrawing.FillRectWithTiles(screen, TileType.Water, left, top, right, bottom);

			TileType decorationType = Utilities.GetRandomInt(0, 1) == 0
				? TileType.Boulder
				: TileType.Tree;

			if (left >= 4) {
				TileDrawing.DrawTile(screen, decorationType, left - 2, 4);
				TileDrawing.DrawTile(screen, decorationType, left - 2, 6);
			}

			if (right <= 11) {
				TileDrawing.DrawTile(screen, decorationType, right + 2, 4);
				TileDrawing.DrawTile(screen, decorationType, right + 2, 6);
			}

			if (!screen.IsGraveyard) {
				screen.PaletteInterior = 3;
			}
		}

		public static void BuildArmosLayout(Screen screen) {
			if (screen.IsLake) {
				if (
					screen.LakeSpot == NineSliceSpot.Middle ||
					screen.LakeSpot == NineSliceSpot.TopLeft ||
					screen.LakeSpot == NineSliceSpot.Top ||
					screen.LakeSpot == NineSliceSpot.TopRight
				) {
					AddArmos(screen);
				} else {
					Direction dir = Direction.Left;

					if (screen.HasWaterTopLeft) {
						dir = Direction.Right;
					}

					if (screen.HasWaterTopRight) {
						dir = Direction.Left;
					}

					AddArmos(screen, dir);
				}
			} else {
				AddArmos(screen);
			}
		}

		private static void AddArmos(Screen screen, Direction shiftDirection = Direction.Up) {
			int maxWidth = shiftDirection == Direction.Left || shiftDirection == Direction.Right
				? 4
				: 12;

			int totalColumns = Utilities.GetRandomInt(2, 4);
			if (totalColumns >= 3) {
				totalColumns++;
			}

			int gap = Utilities.GetRandomInt(1, 4);

			int totalWidth = totalColumns + gap * (totalColumns - 1);
			while (totalWidth > maxWidth) {
				totalColumns--;
				totalWidth = totalColumns + gap * (totalColumns - 1);
			}

			int startColumn = maxWidth / 2 - totalWidth / 2 + 2;
			if (shiftDirection == Direction.Right) {
				startColumn += 8;
			}

			int totalRows = Utilities.GetRandomInt(1, 2);

			List<int> columnsWithArmos = new List<int>();
			for (int i = 0;
				i < totalColumns;
				i++) {
				int thisColumn = startColumn + i * (gap + 1);
				TileDrawing.DrawTile(screen, TileType.Armos, thisColumn, Game.ArmosSecretPositionY);

				if (totalRows == 2) {
					TileDrawing.DrawTile(screen, TileType.Armos, thisColumn, Game.ArmosSecretPositionY + 2);
				}

				columnsWithArmos.Add(thisColumn);
			}

			int stairsColumn = columnsWithArmos[Utilities.GetRandomInt(0, columnsWithArmos.Count - 1)];
			if (screen.IsArmosCave) {
				Game.ArmosCaveScreenPositionXs.Add(stairsColumn);
				Game.ArmosCaveScreens.Add(screen.ScreenId);
				screen.ExitCavePositionX = stairsColumn + 1;
				screen.ExitCavePositionY = Game.ArmosSecretPositionY - 1;
			} else if (screen.IsArmosHiddenItem) {
				Game.ArmosHiddenItemPositionX = stairsColumn;
				Game.ArmosHiddenItemScreen = screen.ScreenId;
			}
		}

		public static void BuildDock(Screen screen) {
			int dockCol = Game.SpecialDockScreens.Contains(screen.ScreenId)
				? 8
				: 6;

			int left = screen.HasWaterTopLeft || screen.GetScreenLeft() == null
				? 0
				: 1;

			int right = screen.HasWaterTopRight || screen.GetScreenRight() == null
				? Game.TilesWide - 1
				: Game.TilesWide - 2;

			TileDrawing.FillRectWithTiles(screen, TileType.Water, left, 0, right, 4);
			TileDrawing.FillRectWithTiles(screen, TileType.Bridge, dockCol, 4, dockCol, 4);
			TileDrawing.FillRectWithTiles(screen, TileType.Ground, dockCol - 2, 5, dockCol + 2, 7);

			if (screen.ExitsWest &&
			    screen.GetScreenLeft() != null &&
			    !screen.GetScreenLeft().IsLake
			) {
				int indexOfFirstOpening = screen.EdgeWest.IndexOf(false);

				for (int i = indexOfFirstOpening; i <= 4; i++) {
					int width = i - indexOfFirstOpening;
					TileDrawing.FillRectWithTiles(screen, TileType.Ground, 1, i, 1 + width, i);
				}
			}

			if (screen.ExitsEast &&
			    screen.GetScreenRight() != null &&
			    !screen.GetScreenRight().IsLake
			) {
				int indexOfFirstOpening = screen.EdgeEast.IndexOf(false);

				for (int i = indexOfFirstOpening; i <= 4; i++) {
					int width = i - indexOfFirstOpening;
					TileDrawing.FillRectWithTiles(screen, TileType.Ground, 14 - width, i, 14, i);
				}
			}
		}

		public static void BuildDockReceiving(Screen screen) {
			int dockCol = Game.SpecialDockScreens.Contains(screen.GetScreenDown().ScreenId)
				? 8
				: 6;

			TileDrawing.FillRectWithTiles(screen, TileType.RockTopRight, dockCol - 1, 9, dockCol - 1, 9);
			TileDrawing.FillRectWithTiles(screen, TileType.Ground, dockCol, 9, dockCol, 9);
			TileDrawing.FillRectWithTiles(screen, TileType.RockTopLeft, dockCol + 1, 9, dockCol + 1, 9);
			TileDrawing.FillRectWithTiles(screen, TileType.Rock, dockCol - 1, 10, dockCol + 1, 10);
			TileDrawing.FillRectWithTiles(screen, TileType.Ladder, dockCol, 10, dockCol, 10);
		}

		public static void BuildCoast(Screen screen) {
			Screen screenNorth = screen.GetScreenUp();
			Screen screenSouth = screen.GetScreenDown();
			Screen screenWest = screen.GetScreenLeft();
			Screen screenEast = screen.GetScreenRight();

			bool goesNorth = screenNorth == null || screenNorth.IsCoast;
			bool goesSouth = screenSouth == null || screenSouth.IsCoast;
			bool goesWest = screenWest == null || screenWest.IsCoast;
			bool goesEast = screenEast == null || screenEast.IsCoast;

			if (screen.IsFirstRow) {
				if (goesWest || goesEast) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 6, 0, 9, 4);
				} else {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 4, 0, 11, 4);
				}

				if (goesWest) {
					screen.EdgeWest[0] = true;
					screen.EdgeWest[1] = true;
					screen.EdgeWest[2] = true;
					screen.EdgeWest[3] = true;
					screen.EdgeWest[4] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 5, 4);
				}

				if (goesEast) {
					screen.EdgeEast[0] = true;
					screen.EdgeEast[1] = true;
					screen.EdgeEast[2] = true;
					screen.EdgeEast[3] = true;
					screen.EdgeEast[4] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 10, 0, 15, 4);
				}
			}

			if (screen.IsLastRow) {
				if (goesWest || goesEast) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 6, 6, 9, 10);
				} else {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 4, 6, 11, 10);
				}

				if (goesWest) {
					screen.EdgeWest[6] = true;
					screen.EdgeWest[7] = true;
					screen.EdgeWest[8] = true;
					screen.EdgeWest[9] = true;
					screen.EdgeWest[10] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 6, 5, 10);
				}

				if (goesEast) {
					screen.EdgeEast[6] = true;
					screen.EdgeEast[7] = true;
					screen.EdgeEast[8] = true;
					screen.EdgeEast[9] = true;
					screen.EdgeEast[10] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 10, 6, 15, 10);
				}
			}

			if (screen.IsFirstColumn) {
				if (goesNorth && goesSouth) {
					screen.EdgeNorth[0] = true;
					screen.EdgeNorth[1] = true;
					screen.EdgeNorth[2] = true;
					screen.EdgeNorth[3] = true;
					screen.EdgeNorth[4] = true;
					screen.EdgeNorth[5] = true;
					screen.EdgeNorth[6] = true;
					screen.EdgeSouth[0] = true;
					screen.EdgeSouth[1] = true;
					screen.EdgeSouth[2] = true;
					screen.EdgeSouth[3] = true;
					screen.EdgeSouth[4] = true;
					screen.EdgeSouth[5] = true;
					screen.EdgeSouth[6] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 6, 10);
				} else if (goesNorth) {
					screen.EdgeNorth[0] = true;
					screen.EdgeNorth[1] = true;
					screen.EdgeNorth[2] = true;
					screen.EdgeNorth[3] = true;
					screen.EdgeNorth[4] = true;
					screen.EdgeNorth[5] = true;
					screen.EdgeNorth[6] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 6, 4);
				} else if (goesSouth) {
					screen.EdgeSouth[0] = true;
					screen.EdgeSouth[1] = true;
					screen.EdgeSouth[2] = true;
					screen.EdgeSouth[3] = true;
					screen.EdgeSouth[4] = true;
					screen.EdgeSouth[5] = true;
					screen.EdgeSouth[6] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 6, 6, 10);
				}
			}

			if (screen.IsLastColumn) {
				if (goesNorth && goesSouth) {
					screen.EdgeNorth[9] = true;
					screen.EdgeNorth[10] = true;
					screen.EdgeNorth[11] = true;
					screen.EdgeNorth[12] = true;
					screen.EdgeNorth[13] = true;
					screen.EdgeNorth[14] = true;
					screen.EdgeNorth[15] = true;
					screen.EdgeSouth[9] = true;
					screen.EdgeSouth[10] = true;
					screen.EdgeSouth[11] = true;
					screen.EdgeSouth[12] = true;
					screen.EdgeSouth[13] = true;
					screen.EdgeSouth[14] = true;
					screen.EdgeSouth[15] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 9, 0, 15, 10);
				} else if (goesNorth) {
					screen.EdgeNorth[9] = true;
					screen.EdgeNorth[10] = true;
					screen.EdgeNorth[11] = true;
					screen.EdgeNorth[12] = true;
					screen.EdgeNorth[13] = true;
					screen.EdgeNorth[14] = true;
					screen.EdgeNorth[15] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 9, 0, 15, 4);
				} else if (goesSouth) {
					screen.EdgeSouth[9] = true;
					screen.EdgeSouth[10] = true;
					screen.EdgeSouth[11] = true;
					screen.EdgeSouth[12] = true;
					screen.EdgeSouth[13] = true;
					screen.EdgeSouth[14] = true;
					screen.EdgeSouth[15] = true;
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 9, 6, 15, 10);
				}
			}

			if (screen.IsOverworldItemScreen) {
				BuildOverworldItemDock(screen);
			}
		}

		private static void BuildOverworldItemDock(Screen screen) {
			Screen screenUp = screen.GetScreenUp();
			Screen screenDown = screen.GetScreenDown();

			if (screen.IsCoast) {
				if (screen.IsFirstColumn) {
					Game.OverworldItemPositionX = 3;
					Game.OverworldItemPositionY = screenDown != null && !screenDown.IsCoast
						? 2
						: screenUp != null && !screenUp.IsCoast
							? 7
							: 5;

					TileDrawing.DrawTile(screen, TileType.Bridge, Game.OverworldItemPositionX,
						Game.OverworldItemPositionY);
					TileDrawing.DrawTile(screen, TileType.Bridge, Game.OverworldItemPositionX + 2,
						Game.OverworldItemPositionY);
				} else if (screen.IsLastColumn) {
					Game.OverworldItemPositionX = 12;
					Game.OverworldItemPositionY = screenDown != null && !screenDown.IsCoast
						? 2
						: screenUp != null && !screenUp.IsCoast
							? 7
							: 5;

					TileDrawing.DrawTile(screen, TileType.Bridge, Game.OverworldItemPositionX - 2,
						Game.OverworldItemPositionY);
					TileDrawing.DrawTile(screen, TileType.Bridge, Game.OverworldItemPositionX,
						Game.OverworldItemPositionY);
				} else if (screen.IsLastRow) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 7);
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 9);
					Game.OverworldItemPositionX = 8;
					Game.OverworldItemPositionY = 9;
				} else if (screen.IsFirstRow) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 1);
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 3);
					Game.OverworldItemPositionX = 8;
					Game.OverworldItemPositionY = 1;
				}
			}

			if (screen.IsLake) {
				if (screen.LakeSpot == NineSliceSpot.Bottom) {
					int roll = Utilities.GetRandomInt(0, 100);
					int col = roll < 50 ? 6 : 10;

					TileDrawing.DrawTile(screen, TileType.Bridge, col, 1);
					TileDrawing.DrawTile(screen, TileType.Bridge, col, 3);
					Game.OverworldItemPositionX = col;
					Game.OverworldItemPositionY = 1;
				} else if (screen.LakeSpot == NineSliceSpot.BottomLeft) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 1);
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 3);
					Game.OverworldItemPositionX = 8;
					Game.OverworldItemPositionY = 1;
				} else if (screen.LakeSpot == NineSliceSpot.BottomRight) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 7, 1);
					TileDrawing.DrawTile(screen, TileType.Bridge, 7, 3);
					Game.OverworldItemPositionX = 7;
					Game.OverworldItemPositionY = 1;
				} else if (screen.LakeSpot == NineSliceSpot.Left) {
					int roll = Utilities.GetRandomInt(0, 100);
					int row = roll < 50 ? 3 : 7;

					TileDrawing.DrawTile(screen, TileType.Bridge, 8, row);
					TileDrawing.DrawTile(screen, TileType.Bridge, 10, row);
					Game.OverworldItemPositionX = 10;
					Game.OverworldItemPositionY = row;
				} else if (screen.LakeSpot == NineSliceSpot.TopLeft) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 7);
					TileDrawing.DrawTile(screen, TileType.Bridge, 8, 9);
					Game.OverworldItemPositionX = 8;
					Game.OverworldItemPositionY = 9;
				} else if (screen.LakeSpot == NineSliceSpot.Top) {
					int roll = Utilities.GetRandomInt(0, 100);
					int col = roll < 50 ? 6 : 10;

					TileDrawing.DrawTile(screen, TileType.Bridge, col, 7);
					TileDrawing.DrawTile(screen, TileType.Bridge, col, 9);
					Game.OverworldItemPositionX = col;
					Game.OverworldItemPositionY = 9;
				} else if (screen.LakeSpot == NineSliceSpot.TopRight) {
					TileDrawing.DrawTile(screen, TileType.Bridge, 7, 7);
					TileDrawing.DrawTile(screen, TileType.Bridge, 7, 9);
					Game.OverworldItemPositionX = 7;
					Game.OverworldItemPositionY = 9;
				} else if (screen.LakeSpot == NineSliceSpot.Right) {
					int roll = Utilities.GetRandomInt(0, 100);
					int row = roll < 50 ? 3 : 7;

					TileDrawing.DrawTile(screen, TileType.Bridge, 7, row);
					TileDrawing.DrawTile(screen, TileType.Bridge, 5, row);
					Game.OverworldItemPositionX = 5;
					Game.OverworldItemPositionY = row;
				}
			}
		}

		public static void BuildLake(Screen screen) {
			ApplyLakeTiles(screen);

			if (screen.IsOverworldItemScreen) {
				BuildOverworldItemDock(screen);
			}
		}

		private static void ApplyLakeTiles(Screen screen) {
			if (screen.IsLake) {
				if (screen.LakeSpot == NineSliceSpot.TopLeft) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 7, 6, 15, 10);
					screen.EdgeEast[6] = true;
					screen.EdgeEast[7] = true;
					screen.EdgeEast[8] = true;
					screen.EdgeEast[9] = true;
					screen.EdgeEast[10] = true;

					screen.EdgeSouth[7] = true;
					screen.EdgeSouth[8] = true;
					screen.EdgeSouth[9] = true;
					screen.EdgeSouth[10] = true;
					screen.EdgeSouth[11] = true;
					screen.EdgeSouth[12] = true;
					screen.EdgeSouth[13] = true;
					screen.EdgeSouth[14] = true;
					screen.EdgeSouth[15] = true;
				}

				if (screen.LakeSpot == NineSliceSpot.Top) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 6, 15, 10);
					screen.EdgeEast[6] = true;
					screen.EdgeEast[7] = true;
					screen.EdgeEast[8] = true;
					screen.EdgeEast[9] = true;
					screen.EdgeEast[10] = true;

					screen.EdgeWest[6] = true;
					screen.EdgeWest[7] = true;
					screen.EdgeWest[8] = true;
					screen.EdgeWest[9] = true;
					screen.EdgeWest[10] = true;

					for (int index = 0; index < screen.EdgeSouth.Count; index++) {
						screen.EdgeSouth[index] = true;
					}

					if (screen.ExitsSouth) {
						TileDrawing.FillRectWithTiles(screen, TileType.BridgeVertical, 8, 6, 8, 10);
						screen.EdgeSouth[8] = false;
					}
				}

				if (screen.LakeSpot == NineSliceSpot.TopRight) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 6, 8, 10);
					screen.EdgeWest[6] = true;
					screen.EdgeWest[7] = true;
					screen.EdgeWest[8] = true;
					screen.EdgeWest[9] = true;
					screen.EdgeWest[10] = true;

					screen.EdgeSouth[0] = true;
					screen.EdgeSouth[1] = true;
					screen.EdgeSouth[2] = true;
					screen.EdgeSouth[3] = true;
					screen.EdgeSouth[4] = true;
					screen.EdgeSouth[5] = true;
					screen.EdgeSouth[6] = true;
					screen.EdgeSouth[7] = true;
					screen.EdgeSouth[8] = true;
				}

				if (screen.LakeSpot == NineSliceSpot.Left) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 7, 0, 15, 10);
					screen.EdgeNorth[7] = true;
					screen.EdgeNorth[8] = true;
					screen.EdgeNorth[9] = true;
					screen.EdgeNorth[10] = true;
					screen.EdgeNorth[11] = true;
					screen.EdgeNorth[12] = true;
					screen.EdgeNorth[13] = true;
					screen.EdgeNorth[14] = true;
					screen.EdgeNorth[15] = true;

					screen.EdgeSouth[7] = true;
					screen.EdgeSouth[8] = true;
					screen.EdgeSouth[9] = true;
					screen.EdgeSouth[10] = true;
					screen.EdgeSouth[11] = true;
					screen.EdgeSouth[12] = true;
					screen.EdgeSouth[13] = true;
					screen.EdgeSouth[14] = true;
					screen.EdgeSouth[15] = true;

					for (int index = 0; index < screen.EdgeEast.Count; index++) {
						screen.EdgeEast[index] = true;
					}

					if (screen.ExitsEast) {
						TileDrawing.FillRectWithTiles(screen, TileType.Bridge, 7, 5, 15, 5);
						screen.EdgeEast[5] = false;
					}
				}

				if (screen.LakeSpot == NineSliceSpot.Right) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 8, 10);
					screen.EdgeNorth[0] = true;
					screen.EdgeNorth[1] = true;
					screen.EdgeNorth[2] = true;
					screen.EdgeNorth[3] = true;
					screen.EdgeNorth[4] = true;
					screen.EdgeNorth[5] = true;
					screen.EdgeNorth[6] = true;
					screen.EdgeNorth[7] = true;
					screen.EdgeNorth[8] = true;

					screen.EdgeSouth[0] = true;
					screen.EdgeSouth[1] = true;
					screen.EdgeSouth[2] = true;
					screen.EdgeSouth[3] = true;
					screen.EdgeSouth[4] = true;
					screen.EdgeSouth[5] = true;
					screen.EdgeSouth[6] = true;
					screen.EdgeSouth[7] = true;
					screen.EdgeSouth[8] = true;

					for (int index = 0; index < screen.EdgeWest.Count; index++) {
						screen.EdgeWest[index] = true;
					}

					if (screen.ExitsWest) {
						TileDrawing.FillRectWithTiles(screen, TileType.Bridge, 0, 5, 8, 5);
						screen.EdgeWest[5] = false;
					}
				}

				if (screen.LakeSpot == NineSliceSpot.BottomLeft) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 7, 0, 15, 4);
					screen.EdgeNorth[7] = true;
					screen.EdgeNorth[8] = true;
					screen.EdgeNorth[9] = true;
					screen.EdgeNorth[10] = true;
					screen.EdgeNorth[11] = true;
					screen.EdgeNorth[12] = true;
					screen.EdgeNorth[13] = true;
					screen.EdgeNorth[14] = true;
					screen.EdgeNorth[15] = true;

					screen.EdgeEast[0] = true;
					screen.EdgeEast[1] = true;
					screen.EdgeEast[2] = true;
					screen.EdgeEast[3] = true;
					screen.EdgeEast[4] = true;
				}

				if (screen.LakeSpot == NineSliceSpot.Bottom) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 15, 4);
					screen.EdgeEast[0] = true;
					screen.EdgeEast[1] = true;
					screen.EdgeEast[2] = true;
					screen.EdgeEast[3] = true;
					screen.EdgeEast[4] = true;

					screen.EdgeWest[0] = true;
					screen.EdgeWest[1] = true;
					screen.EdgeWest[2] = true;
					screen.EdgeWest[3] = true;
					screen.EdgeWest[4] = true;

					for (int index = 0; index < screen.EdgeNorth.Count; index++) {
						screen.EdgeNorth[index] = true;
					}

					if (screen.ExitsNorth) {
						TileDrawing.FillRectWithTiles(screen, TileType.BridgeVertical, 8, 0, 8, 4);
						screen.EdgeNorth[8] = false;
					}
				}

				if (screen.LakeSpot == NineSliceSpot.BottomRight) {
					TileDrawing.FillRectWithTiles(screen, TileType.Water, 0, 0, 8, 4);
					screen.EdgeNorth[0] = true;
					screen.EdgeNorth[1] = true;
					screen.EdgeNorth[2] = true;
					screen.EdgeNorth[3] = true;
					screen.EdgeNorth[4] = true;
					screen.EdgeNorth[5] = true;
					screen.EdgeNorth[6] = true;
					screen.EdgeNorth[7] = true;
					screen.EdgeNorth[8] = true;

					screen.EdgeWest[0] = true;
					screen.EdgeWest[1] = true;
					screen.EdgeWest[2] = true;
					screen.EdgeWest[3] = true;
					screen.EdgeWest[4] = true;
				}
			}
		}

		public static void PlaceBigTreeStamp(Screen screen, int topLeftX, int topLeftY) {
			TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, topLeftX, topLeftY);
			TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, topLeftX + 1, topLeftY);
			TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, topLeftX, topLeftY + 1);
			TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, topLeftX + 1, topLeftY + 1);
		}

		public static void PlaceBigTreeCaveStamp(Screen screen, int topLeftX, int topLeftY) {
			TileDrawing.DrawTile(screen, TileType.BigTreeTopLeft, topLeftX, topLeftY);
			TileDrawing.DrawTile(screen, TileType.BigTreeTopRight, topLeftX + 2, topLeftY);
			TileDrawing.DrawTile(screen, TileType.BigTreeTopMiddle, topLeftX + 1, topLeftY);
			TileDrawing.DrawTile(screen, TileType.Cave, topLeftX + 1, topLeftY + 1);
			TileDrawing.DrawTile(screen, TileType.BigTreeBottomLeft, topLeftX, topLeftY + 1);
			TileDrawing.DrawTile(screen, TileType.BigTreeBottomRight, topLeftX + 2, topLeftY + 1);
		}

		public static void PlaceHouse(Screen screen, int topLeftX, int topLeftY, int width) {
			width--;

			if (width < 2) {
				width = 2;
			}

			TileDrawing.DrawTile(screen, TileType.KakarikoHouseTopLeft, topLeftX, topLeftY);
			TileDrawing.DrawTile(screen, TileType.KakarikoHouseBottomLeft, topLeftX, topLeftY + 1);
			TileDrawing.DrawTile(screen, TileType.KakarikoHouseTopRight, topLeftX + width, topLeftY);
			TileDrawing.DrawTile(screen, TileType.KakarikoHouseBottomRight, topLeftX + width, topLeftY + 1);

			TileDrawing.FillRectWithTiles(
				screen, TileType.KakarikoHouseTopMiddle, topLeftX + 1, topLeftY, topLeftX + width - 1, topLeftY
			);

			TileDrawing.FillRectWithTiles(
				screen,
				TileType.KakarikoHouseBottomMiddle,
				topLeftX + 1, topLeftY + 1, topLeftX + width - 1, topLeftY + 1
			);

			TileDrawing.FillRectWithTiles(
				screen, TileType.KakarikoHouseFront, topLeftX, topLeftY + 2, topLeftX + width, topLeftY + 2
			);

			int doorX = Utilities.GetRandomInt(topLeftX + 1, topLeftX + width - 1);

			TileDrawing.DrawTile(screen, TileType.KakarikoHouseDoor, doorX, topLeftY + 2);
		}
	}
}