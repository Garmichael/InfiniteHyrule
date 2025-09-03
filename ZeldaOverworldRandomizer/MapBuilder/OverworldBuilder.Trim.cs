using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void BuildTrim(Screen screen) {
			List<int> trimMap = new List<int>();

			for (int tileIndex = 0; tileIndex < screen.Tiles.Count; tileIndex++) {
				int tileRow = Utilities.GetRowFromTileIndex(tileIndex);
				int tileCol = Utilities.GetColFromTileIndex(tileIndex);

				TileType tileToUse = Game.TileLookupById[screen.Tiles[tileIndex]];

				TileType tileNorth = tileRow == 0
					? TileType.CaveAlt
					: Game.TileLookupById[screen.Tiles[tileIndex - Game.TilesWide]];

				TileType tileSouth = tileRow == Game.TilesHigh - 1
					? TileType.CaveAlt
					: Game.TileLookupById[screen.Tiles[tileIndex + Game.TilesWide]];

				TileType tileWest = tileCol == 0
					? TileType.CaveAlt
					: Game.TileLookupById[screen.Tiles[tileIndex - 1]];

				TileType tileEast = tileCol == Game.TilesWide - 1
					? TileType.CaveAlt
					: Game.TileLookupById[screen.Tiles[tileIndex + 1]];

				if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.Water]) {
					tileToUse = GetWaterTrim(tileNorth, tileWest, tileEast, tileSouth);
				}

				if (screen.Tiles[tileIndex] == Game.TileLookup[TileType.Rock]) {
					tileToUse = GetRockTrim(tileNorth, tileWest, tileEast, tileSouth);
				}

				trimMap.Add(Game.TileLookup[tileToUse]);
			}

			ApplyTrim(screen, trimMap);
		}

		private static TileType GetWaterTrim(
			TileType tileNorth,
			TileType tileWest,
			TileType tileEast,
			TileType tileSouth
		) {
			TileType tileToUse = TileType.Water;

			List<TileType> countsAsWater = new List<TileType> {
				TileType.Water, TileType.Bridge, TileType.BridgeVertical, TileType.CaveAlt, TileType.Waterfall
			};

			if (!countsAsWater.Contains(tileNorth) &&
			    countsAsWater.Contains(tileEast) &&
			    countsAsWater.Contains(tileSouth) &&
			    !countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreTopLeft;
			}

			if (!countsAsWater.Contains(tileNorth) &&
			    countsAsWater.Contains(tileEast) &&
			    countsAsWater.Contains(tileSouth) &&
			    countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreTop;
			}

			if (!countsAsWater.Contains(tileNorth) &&
			    !countsAsWater.Contains(tileEast) &&
			    countsAsWater.Contains(tileSouth) &&
			    countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreTopRight;
			}

			if (countsAsWater.Contains(tileNorth) &&
			    !countsAsWater.Contains(tileEast) &&
			    countsAsWater.Contains(tileSouth) &&
			    countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreRight;
			}

			if (countsAsWater.Contains(tileNorth) &&
			    !countsAsWater.Contains(tileEast) &&
			    !countsAsWater.Contains(tileSouth) &&
			    countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreBottomRight;
			}

			if (countsAsWater.Contains(tileNorth) &&
			    countsAsWater.Contains(tileEast) &&
			    !countsAsWater.Contains(tileSouth) &&
			    countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreBottom;
			}

			if (countsAsWater.Contains(tileNorth) &&
			    countsAsWater.Contains(tileEast) &&
			    !countsAsWater.Contains(tileSouth) &&
			    !countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreBottomLeft;
			}

			if (countsAsWater.Contains(tileNorth) &&
			    countsAsWater.Contains(tileEast) &&
			    countsAsWater.Contains(tileSouth) &&
			    !countsAsWater.Contains(tileWest)
			) {
				tileToUse = TileType.ShoreLeft;
			}

			return tileToUse;
		}

		private static TileType GetRockTrim(
			TileType tileNorth,
			TileType tileWest,
			TileType tileEast,
			TileType tileSouth
		) {
			TileType tileToUse = TileType.Rock;

			List<TileType> tilesActingLikeRock = new List<TileType> {
				TileType.Rock, TileType.Ladder, TileType.Cave, TileType.RockBombWall, TileType.CaveAlt,
				TileType.RockBottomLeft, TileType.RockBottomRight, TileType.RockTopLeft, TileType.RockTopRight,
			};

			List<TileType> tilesActingLikeGround = new List<TileType> {
				TileType.Ground, TileType.Desert
			};

			if (tilesActingLikeGround.Contains(tileNorth) && tileSouth == TileType.Rock) {
				if (tilesActingLikeGround.Contains(tileWest) && tilesActingLikeRock.Contains(tileEast)) {
					tileToUse = TileType.RockTopLeft;
				} else if (tilesActingLikeGround.Contains(tileEast) && tilesActingLikeRock.Contains(tileWest)) {
					tileToUse = TileType.RockTopRight;
				} else {
					tileToUse = TileType.RockTop;
				}
			}


			if (tilesActingLikeGround.Contains(tileSouth) && tileNorth == TileType.Rock) {
				if (tilesActingLikeGround.Contains(tileWest) && tilesActingLikeRock.Contains(tileEast)) {
					tileToUse = TileType.RockBottomLeft;
				} else if (tilesActingLikeGround.Contains(tileEast) && tilesActingLikeRock.Contains(tileWest)) {
					tileToUse = TileType.RockBottomRight;
				}
			}

			return tileToUse;
		}

		private static void ApplyTrim(Screen screen, List<int> trimMap) {
			for (int tileIndex = 0; tileIndex < Game.TilesWide * Game.TilesHigh; tileIndex++) {
				screen.Tiles[tileIndex] = trimMap[tileIndex];
			}
		}
	}
}