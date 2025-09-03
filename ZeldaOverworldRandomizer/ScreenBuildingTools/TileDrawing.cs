using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.ScreenBuildingTools {
	public static class TileDrawing {
		public static void FillRectWithTiles(
			Screen screen,
			TileType tileType,
			int startX,
			int startY,
			int endX,
			int endY,
			TileType mask = TileType.Debug
		) {
			if (startX > endX) {
				int temp = startX;
				startX = endX;
				endX = temp;
			}

			if (startY > endY) {
				int temp = startY;
				startY = endY;
				endY = temp;
			}

			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					int tileIndex = Utilities.GetTileByColAndRow(x, y);
					TileType currentTile = Game.TileLookupById[screen.Tiles[tileIndex]];
					if (mask == TileType.Debug || currentTile == mask) {
						screen.Tiles[tileIndex] = Game.TileLookup[tileType];
					}
				}
			}
		}

		public static void FillPerimeterWithTiles(
			Screen screen,
			TileType tileType,
			int startX,
			int startY,
			int endX,
			int endY
		) {
			if (startX > endX) {
				int temp = endX;
				startX = endX;
				endX = temp;
			}

			if (startY > endY) {
				int temp = endY;
				startY = endY;
				endY = temp;
			}

			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					if (x == startX || y == startY || x == endX || y == endY) {
						int tileId = Utilities.GetTileByColAndRow(x, y);
						screen.Tiles[tileId] = Game.TileLookup[tileType];
					}
				}
			}
		}

		public static void FillRectWithLatticeTiles(
			Screen screen,
			TileType tileType,
			int startX,
			int startY,
			int endX,
			int endY,
			int gapX = 1,
			int gapY = 1
		) {
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					if ((x - startX) % (1 + gapX) == 0 && (y - startY) % (1 + gapY) == 0) {
						int tileId = Utilities.GetTileByColAndRow(x, y);
						screen.Tiles[tileId] = Game.TileLookup[tileType];
					}
				}
			}
		}

		public static void DrawTile(Screen screen, TileType tileType, int col, int row) {
			screen.Tiles[Utilities.GetTileByColAndRow(col, row)] = Game.TileLookup[tileType];
		}

		public static void DrawTile(Screen screen, TileType tileType, int tileIndex) {
			screen.Tiles[tileIndex] = Game.TileLookup[tileType];
		}

		public static void CloneTileRow(Screen screen, int sourceRow, int destinationRow) {
			for (int i = 0; i < Game.TilesWide; i++) {
				screen.Tiles[Utilities.GetTileByColAndRow(i, destinationRow)] =
					screen.Tiles[Utilities.GetTileByColAndRow(i, sourceRow)];
			}
		}

		public static void ReplaceTile(Screen screen, TileType sourceTile, TileType replacementTile) {
			for (int i = 0; i < screen.Tiles.Count; i++) {
				if (screen.Tiles[i] == Game.TileLookup[sourceTile]) {
					screen.Tiles[i] = Game.TileLookup[replacementTile];
				}
			}
		}
	}
}