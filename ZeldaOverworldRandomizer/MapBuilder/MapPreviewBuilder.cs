using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Pen = System.Drawing.Pen;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static class MapPreviewBuilder {
		private static Dictionary<TileType, Rectangle> _tileAtlasRects;
		private static Dictionary<SpriteNames, Rectangle> _spriteAtlasRects;

		private enum SpriteNames {
			Link,
			Fairy,
			DockHeart,
			ShopA,
			ShopB,
			ShopC,
			ShopD,
			Rupees10,
			Rupees30,
			Rupees100,
			Bracelet,
			Dungeon1,
			Dungeon2,
			Dungeon3,
			Dungeon4,
			Dungeon5,
			Dungeon6,
			Dungeon7,
			Dungeon8,
			Dungeon9,
			WoodSword,
			WhiteSword,
			MagicSword,
			HeartCont,
			Letter,
			Potion,
			AnyRoad,
			Tax,
			MoneyGame,
			HintOne,
			HintTwo,
			PayHintOne,
			PayHintTwo
		}

		public static void DrawFullMap() {
			const int tileSize = 16;
			const int columnsPerScreen = 16;
			const int rowsPerScreen = 11;

			const string assetsPath = "pack://application:,,,/ZeldaOverworldRandomizer;component/Assets/";
			const string tileAtlasPath = assetsPath + "tileAtlas.png";
			const string spriteAtlasPath = assetsPath + "spriteAtlas.png";

			FrontEnd.MainWindow.FullMapCanvas.Children.Clear();

			if (_tileAtlasRects == null || _tileAtlasRects.Count == 0) {
				BuildTileAtlasRectTable();

				if (_tileAtlasRects == null) {
					return;
				}
			}

			if (_spriteAtlasRects == null || _spriteAtlasRects.Count == 0) {
				BuildSpriteAtlasRectTable();

				if (_spriteAtlasRects == null) {
					return;
				}
			}

			BitmapImage tileBitmap = new BitmapImage();
			tileBitmap.BeginInit();
			tileBitmap.UriSource = new Uri(tileAtlasPath);
			tileBitmap.EndInit();

			Bitmap tileAtlas = BitmapImage2Bitmap(tileBitmap);

			BitmapImage spriteBitmap = new BitmapImage();
			spriteBitmap.BeginInit();
			spriteBitmap.UriSource = new Uri(spriteAtlasPath);
			spriteBitmap.EndInit();

			Bitmap spriteAtlas = BitmapImage2Bitmap(spriteBitmap);

			foreach (Screen screen in Game.Screens) {
				Bitmap screenImage = new Bitmap(tileSize * columnsPerScreen, tileSize * rowsPerScreen);
				Graphics graphics = Graphics.FromImage(screenImage);

				for (int tileIndex = 0; tileIndex < screen.Tiles.Count; tileIndex++) {
					Rectangle atlasRect = _tileAtlasRects[
						Game.TileLookupById[screen.Tiles[tileIndex]]
					];

					if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.Armos] &&
					    Game.ArmosCaveScreens.Contains(screen.ScreenId) &&
					    FrontEnd.MainWindow.GenerateWithMapSpoilers
					) {
						int indexOfArmosThing = Game.ArmosCaveScreens.IndexOf(screen.ScreenId);
						int col = Utilities.GetColFromTileIndex(tileIndex);
						int row = Utilities.GetRowFromTileIndex(tileIndex);

						if (col == Game.ArmosCaveScreenPositionXs[indexOfArmosThing] &&
						    row == Game.ArmosSecretPositionY
						) {
							atlasRect = _tileAtlasRects[TileType.StaircaseDown];
						}
					}

					if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.Armos] &&
					    screen.ScreenId == Game.ArmosHiddenItemScreen &&
					    FrontEnd.MainWindow.GenerateWithMapSpoilers
					) {
						int col = Utilities.GetColFromTileIndex(tileIndex);
						int row = Utilities.GetRowFromTileIndex(tileIndex);

						if (col == Game.ArmosHiddenItemPositionX && row == Game.ArmosSecretPositionY) {
							atlasRect = _tileAtlasRects[TileType.BoulderPushable];
						}
					}

					if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.RockBombWall] &&
					    !FrontEnd.MainWindow.GenerateWithMapSpoilers
					) {
						atlasRect = _tileAtlasRects[TileType.Rock];
					}

					if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.TreeBurnable] &&
					    !FrontEnd.MainWindow.GenerateWithMapSpoilers
					) {
						atlasRect = _tileAtlasRects[TileType.Tree];
					}

					if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.BoulderPushable] &&
					    !FrontEnd.MainWindow.GenerateWithMapSpoilers
					) {
						atlasRect = _tileAtlasRects[TileType.Boulder];
					}

					int rectOffset = 0;

					if (
						Utilities.GetColFromTileIndex(tileIndex) < 2 ||
						Utilities.GetColFromTileIndex(tileIndex) > Game.TilesWide - 3 ||
						Utilities.GetRowFromTileIndex(tileIndex) < 2 ||
						Utilities.GetRowFromTileIndex(tileIndex) > Game.TilesHigh - 3
					) {
						if (screen.PaletteBorder == 0) {
							rectOffset = 2;
						} else if (screen.PaletteBorder == 3) {
							rectOffset = 1;
						}
					} else {
						if (screen.PaletteInterior == 0) {
							rectOffset = 2;
						} else if (screen.PaletteInterior == 3) {
							rectOffset = 1;
						}
					}

					rectOffset *= 96;

					Rectangle sourceRect = new Rectangle(
						atlasRect.X + rectOffset, atlasRect.Y, atlasRect.Width, atlasRect.Height
					);

					Rectangle destinationRect = new Rectangle(
						tileSize * Utilities.GetColFromTileIndex(tileIndex),
						tileSize * Utilities.GetRowFromTileIndex(tileIndex),
						tileSize, tileSize
					);

					if (tileAtlas != null) {
						graphics.DrawImage(tileAtlas, destinationRect, sourceRect, GraphicsUnit.Pixel);
					}
				}

//				if (FrontEnd.MainWindow.ShowRegionBorders) {
//					Color regionPenColor = Color.FromArgb(128, 255, 255, 255);
//
//					if (screen.Column == 0 || screen.GetScreenLeft().Region != screen.Region) {
//						const int x1 = 0;
//						const int x2 = 0;
//						const int y1 = 0;
//						const int y2 = tileSize * rowsPerScreen;
//
//						Pen pen = new Pen(regionPenColor) {Width = 10};
//						graphics.DrawLine(pen, x1, y1, x2, y2);
//					}
//
//					if (screen.Column == Game.LastScreenColumn || screen.GetScreenRight().Region != screen.Region) {
//						const int x1 = tileSize * columnsPerScreen;
//						const int x2 = tileSize * columnsPerScreen;
//						const int y1 = 0;
//						const int y2 = tileSize * rowsPerScreen;
//
//						Pen pen = new Pen(regionPenColor) {Width = 10};
//						graphics.DrawLine(pen, x1, y1, x2, y2);
//					}
//
//					if (screen.Row == 0 || screen.GetScreenUp().Region != screen.Region) {
//						const int x1 = 0;
//						const int x2 = tileSize * columnsPerScreen;
//						const int y1 = 0;
//						const int y2 = 0;
//
//						Pen pen = new Pen(regionPenColor) {Width = 10};
//						graphics.DrawLine(pen, x1, y1, x2, y2);
//					}
//
//					if (screen.Row == Game.LastScreenRow || screen.GetScreenDown().Region != screen.Region) {
//						const int x1 = 0;
//						const int x2 = tileSize * columnsPerScreen;
//						const int y1 = tileSize * rowsPerScreen;
//						const int y2 = tileSize * rowsPerScreen;
//
//						Pen pen = new Pen(regionPenColor) {Width = 10};
//						graphics.DrawLine(pen, x1, y1, x2, y2);
//					}
//				}

				if (screen.ScreenId == Game.LinkStartScreen) {
					Pen pen = new Pen(Color.FromArgb(128, 255, 255, 0)) {Width = 10};
					Brush brush = new SolidBrush(Color.FromArgb(64, 255, 0, 0));
					const int x1 = 0;
					const int x2 = tileSize * columnsPerScreen;
					const int y1 = 0;
					const int y2 = tileSize * rowsPerScreen;

					graphics.FillRectangle(brush, x1, y1, x2, y2);
					graphics.DrawRectangle(pen, x1, y1, x2, y2);

					Rectangle atlasRect = _spriteAtlasRects[SpriteNames.Link];

					Rectangle sourceRect = new Rectangle(
						atlasRect.X, atlasRect.Y, atlasRect.Width, atlasRect.Height
					);

					Rectangle destinationRect = new Rectangle(
						tileSize * 8,
						tileSize * 5,
						tileSize, tileSize
					);

					if (spriteAtlas != null) {
						graphics.DrawImage(spriteAtlas, destinationRect, sourceRect, GraphicsUnit.Pixel);
					}
				}

				// if (screen.IsSecretScreen) {
				// 	Pen pen = new Pen(Color.FromArgb(128, 0, 128, 128)) {Width = 10};
				// 	Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 0));
				// 	const int x1 = 0;
				// 	const int x2 = tileSize * columnsPerScreen;
				// 	const int y1 = 0;
				// 	const int y2 = tileSize * rowsPerScreen;
				//
				// 	graphics.FillRectangle(brush, x1, y1, x2, y2);
				// 	graphics.DrawRectangle(pen, x1, y1, x2, y2);
				// }

				if (screen.IsFairyPond) {
					Rectangle atlasRect = _spriteAtlasRects[SpriteNames.Fairy];

					Rectangle sourceRect = new Rectangle(
						atlasRect.X, atlasRect.Y, atlasRect.Width, atlasRect.Height
					);

					Rectangle destinationRect = new Rectangle(
						tileSize * 7 + tileSize / 2,
						tileSize * 4,
						tileSize, tileSize
					);

					if (spriteAtlas != null) {
						graphics.DrawImage(spriteAtlas, destinationRect, sourceRect, GraphicsUnit.Pixel);
					}
				}

				if (screen.IsOverworldItemScreen) {
					Rectangle atlasRect = _spriteAtlasRects[SpriteNames.DockHeart];

					Rectangle sourceRect = new Rectangle(
						atlasRect.X, atlasRect.Y, atlasRect.Width, atlasRect.Height
					);

					Rectangle destinationRect = new Rectangle(
						tileSize * Game.OverworldItemPositionX,
						tileSize * Game.OverworldItemPositionY,
						tileSize, tileSize
					);

					if (spriteAtlas != null) {
						graphics.DrawImage(spriteAtlas, destinationRect, sourceRect, GraphicsUnit.Pixel);
					}
				}

				if (screen.CaveDestination != 0 && FrontEnd.MainWindow.GenerateWithMapSpoilers) {
					Dictionary<CaveType, SpriteNames> mappedEnums = new Dictionary<CaveType, SpriteNames> {
						{CaveType.Dungeon1, SpriteNames.Dungeon1},
						{CaveType.Dungeon2, SpriteNames.Dungeon2},
						{CaveType.Dungeon3, SpriteNames.Dungeon3},
						{CaveType.Dungeon4, SpriteNames.Dungeon4},
						{CaveType.Dungeon5, SpriteNames.Dungeon5},
						{CaveType.Dungeon6, SpriteNames.Dungeon6},
						{CaveType.Dungeon7, SpriteNames.Dungeon7},
						{CaveType.Dungeon8, SpriteNames.Dungeon8},
						{CaveType.Dungeon9, SpriteNames.Dungeon9},
						{CaveType.HeartContainer, SpriteNames.HeartCont},
						{CaveType.WoodSword, SpriteNames.WoodSword},
						{CaveType.WhiteSword, SpriteNames.WhiteSword},
						{CaveType.MasterSword, SpriteNames.MagicSword},
						{CaveType.AnyRoad, SpriteNames.AnyRoad},
						{CaveType.HintSecretInTreeAtDeadEnd, SpriteNames.HintOne},
						{CaveType.MoneyGame, SpriteNames.MoneyGame},
						{CaveType.Tax, SpriteNames.Tax},
						{CaveType.Letter, SpriteNames.Letter},
						{CaveType.HintMeetOldManAtGrave, SpriteNames.HintTwo},
						{CaveType.PotionShop, SpriteNames.Potion},
						{CaveType.PayHint1, SpriteNames.PayHintOne},
						{CaveType.PayHint2, SpriteNames.PayHintTwo},
						{CaveType.ShopA, SpriteNames.ShopA},
						{CaveType.ShopB, SpriteNames.ShopB},
						{CaveType.ShopC, SpriteNames.ShopC},
						{CaveType.BlueRingShop, SpriteNames.ShopD},
						{CaveType.RupeesThirty, SpriteNames.Rupees30},
						{CaveType.RupeesHundred, SpriteNames.Rupees100},
						{CaveType.RupeesTen, SpriteNames.Rupees10}
					};

					Rectangle atlasRect =
						_spriteAtlasRects[mappedEnums[Game.CaveLookupById[screen.CaveDestination]]];

					Rectangle sourceRect = new Rectangle(
						atlasRect.X, atlasRect.Y, atlasRect.Width, atlasRect.Height
					);

					Rectangle destinationRect = new Rectangle(0, 0, tileSize * 2, tileSize * 2);

					if (spriteAtlas != null) {
						graphics.DrawImage(spriteAtlas, destinationRect, sourceRect, GraphicsUnit.Pixel);
					}
				}

				const int imageSize = tileSize;

				BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					screenImage.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions()
				);

				Image newImage = new Image {
					Width = imageSize * columnsPerScreen,
					Height = imageSize * rowsPerScreen,
					Source = bitmapSource
				};

				FrontEnd.MainWindow.FullMapCanvas.Children.Add(newImage);
				Canvas.SetLeft(newImage, screen.Column * tileSize * columnsPerScreen + screen.Column * 2);
				Canvas.SetTop(newImage, screen.Row * tileSize * rowsPerScreen + screen.Row * 2);

//				TextBlock regionIdText = new TextBlock {
//					Background = new SolidColorBrush(Colors.Black),
//					Foreground = new SolidColorBrush(Colors.Chartreuse),
//					Text = screen.JourneyDistanceToScreen(Game.Screens[17]).ToString()
//				};
//
//				FrontEnd.MainWindow.FullMapCanvas.Children.Add(regionIdText);
//				Canvas.SetLeft(regionIdText, screen.Column * tileSize * columnsPerScreen * scale + screen.Column * 2);
//				Canvas.SetTop(regionIdText, screen.Row * tileSize * rowsPerScreen * scale + screen.Row * 2 + 32);
			}

			float fullMapWidth = tileSize * columnsPerScreen * Game.ScreensWide + Game.ScreensWide * 2;
			float fullMapHeight = tileSize * rowsPerScreen * Game.ScreensHigh + Game.ScreensHigh * 2;
//
//			fullMapWidth = 100;
//			fullMapHeight = 100;
//			FrontEnd.MainWindow.MapContainers.Width = fullMapWidth;
//			FrontEnd.MainWindow.MapContainers.Height = fullMapHeight;
			FrontEnd.MainWindow.FullMapCanvas.Width = fullMapWidth;
			FrontEnd.MainWindow.FullMapCanvas.Height = fullMapHeight;
		}

		private static void BuildTileAtlasRectTable() {
			_tileAtlasRects = new Dictionary<TileType, Rectangle>();
			const int tileSize = 16;
			int yPos = 0;

			_tileAtlasRects.Add(TileType.StaircaseDown, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Boulder, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Ground, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockTopLeft, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockTop, new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockTopRight, new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.Ladder, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Tree, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Armos, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockBottomLeft, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Rock, new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockBottomRight, new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.BigTreeTopLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BigTreeTopMiddle, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BigTreeTopRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthTopLeft,
				new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthTopMiddle,
				new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthTopRight,
				new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.BigTreeBottomLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Cave, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BlackFloor, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BigTreeBottomRight,
				new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthBottomLeft,
				new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthTopMiddleAlt,
				new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.DungeonMouthBottomRight,
				new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.ShoreTopLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.ShoreTop, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.ShoreTopRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.WaterTopLeftInsideCorner,
				new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.WaterTopRightInsideCorner,
				new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Waterfall, new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.ShoreLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Water, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.ShoreRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.WaterBottomLeftInsideCorner,
				new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.WaterBottomRightInsideCorner,
				new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Bridge, new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.ShoreBottomLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.ShoreBottom, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.ShoreBottomRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Desert, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Grave, new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoPost, new Rectangle(80, yPos, tileSize, tileSize));

			yPos += 16;

			_tileAtlasRects.Add(TileType.TreeBurnable, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.RockBombWall, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BoulderPushable, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.GravePushable, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.WaterfallCave, new Rectangle(64, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.Debug, new Rectangle(80, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.CaveAlt, new Rectangle(80, yPos, tileSize, tileSize));
			
			yPos += 16;

			_tileAtlasRects.Add(TileType.KakarikoHouseTopLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseTopMiddle, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseTopRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseFront, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseDoor, new Rectangle(64, yPos, tileSize, tileSize));
			
			yPos += 16;

			_tileAtlasRects.Add(TileType.KakarikoHouseBottomLeft, new Rectangle(0, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseBottomMiddle, new Rectangle(16, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.KakarikoHouseBottomRight, new Rectangle(32, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.BridgeVertical, new Rectangle(48, yPos, tileSize, tileSize));
			_tileAtlasRects.Add(TileType.PalmTree, new Rectangle(64, yPos, tileSize, tileSize));
		}

		private static void BuildSpriteAtlasRectTable() {
			_spriteAtlasRects = new Dictionary<SpriteNames, Rectangle>();
			const int tileSize = 16;
			int yPos = 0;

			_spriteAtlasRects.Add(SpriteNames.Link, new Rectangle(0, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Fairy, new Rectangle(16, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.DockHeart, new Rectangle(32, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.ShopA, new Rectangle(48, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.ShopB, new Rectangle(64, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.ShopC, new Rectangle(80, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.ShopD, new Rectangle(96, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Rupees10, new Rectangle(112, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Bracelet, new Rectangle(128, yPos, tileSize, tileSize));

			yPos += 16;

			_spriteAtlasRects.Add(SpriteNames.Dungeon1, new Rectangle(0, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon2, new Rectangle(16, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon3, new Rectangle(32, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.WoodSword, new Rectangle(48, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.HeartCont, new Rectangle(64, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Letter, new Rectangle(80, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Potion, new Rectangle(96, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Rupees30, new Rectangle(112, yPos, tileSize, tileSize));

			yPos += 16;

			_spriteAtlasRects.Add(SpriteNames.Dungeon4, new Rectangle(0, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon5, new Rectangle(16, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon6, new Rectangle(32, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.WhiteSword, new Rectangle(48, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.AnyRoad, new Rectangle(64, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Tax, new Rectangle(80, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.MoneyGame, new Rectangle(96, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Rupees100, new Rectangle(112, yPos, tileSize, tileSize));

			yPos += 16;

			_spriteAtlasRects.Add(SpriteNames.Dungeon7, new Rectangle(0, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon8, new Rectangle(16, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.Dungeon9, new Rectangle(32, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.MagicSword, new Rectangle(48, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.HintOne, new Rectangle(64, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.HintTwo, new Rectangle(80, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.PayHintOne, new Rectangle(96, yPos, tileSize, tileSize));
			_spriteAtlasRects.Add(SpriteNames.PayHintTwo, new Rectangle(112, yPos, tileSize, tileSize));
		}

		private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage) {
			using (MemoryStream outStream = new MemoryStream()) {
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				enc.Save(outStream);
				Bitmap bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}
	}
}