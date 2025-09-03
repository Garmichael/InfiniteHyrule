using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.MapBuilder;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class DenseForestBuilder : ScreenBuilder {
		public DenseForestBuilder(Screen screen) : base(screen) { }

		private enum GenericStyle {
			MiddleFull,
			ExtendEdge
		}

		private GenericStyle _genericStyle = GenericStyle.MiddleFull;

		private readonly List<Biome> _rockyBiomes = new List<Biome>
			{ Biome.MountainRange, Biome.RockyBeach, Biome.Tunnel, Biome.StartZone };

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			if (!Screen.IsLake && Utilities.GetRandomInt(0, 2) == 0) {
				_genericStyle = GenericStyle.MiddleFull;
			} else {
				_genericStyle = GenericStyle.ExtendEdge;
			}

			AssignEdges();

			if (_genericStyle == GenericStyle.ExtendEdge && !Screen.IsLake && !Screen.IsCoast) {
				CloneOppositeEdges();
			}

			BuildEdgeTiles();

			if (Screen.IsArmosCave || Screen.IsArmosHiddenItem || Screen.IsArmosDecor) {
				BuildArmosScreen();
			} else if (Screen.IsFairyPond) {
				BuildFairyPondScreen();
			} else if (Screen.IsWhistleLake) {
				BuildWhistleLakeScreen();
			} else if (Screen.IsOpenDungeon) {
				BuildDungeonEntranceScreen();
			} else {
				BuildGeneric();
			}

			if (Screen.IsLake && Screen.LakeSpot == NineSliceSpot.Middle) {
				TileDrawingTemplates.BuildCenterLakeRing(Screen);
			}

			if (Screen.IsDock) {
				TileDrawingTemplates.BuildDock(Screen);
			}

			if (Screen.IsOverworldItemScreen) {
				bool canBeReached = false;
				for (int tileIndex = 0; tileIndex < Screen.Tiles.Count; tileIndex++) {
					int tileId = Screen.Tiles[tileIndex];
					if (tileId == Game.TileLookup[TileType.Bridge]) {
						bool groundToTheLeft = Screen.Tiles[tileIndex - 2] == Game.TileLookup[TileType.Ground];
						bool groundToTheRight = Screen.Tiles[tileIndex + 2] == Game.TileLookup[TileType.Ground];
						bool groundToTheTop = Utilities.GetRowFromTileIndex(tileIndex) >= 2 &&
						                      Screen.Tiles[tileIndex - 32] == Game.TileLookup[TileType.Ground];
						bool groundToTheBottom = Utilities.GetRowFromTileIndex(tileIndex) <= 8 &&
						                         Screen.Tiles[tileIndex + 32] == Game.TileLookup[TileType.Ground];

						if (groundToTheLeft || groundToTheRight || groundToTheTop || groundToTheBottom) {
							canBeReached = true;
							break;
						}
					}
				}

				if (!canBeReached) {
					Debug.Print("Rebuild Reason: Cannot reach Ladder Item in Dense Forest");
					OverworldBuilder.ShouldRebuild = true;
				}
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			bool isVerticalEdge = edge == Direction.Down || edge == Direction.Up;

			int edgeSize = isVerticalEdge
				? Game.TilesWide
				: Game.TilesHigh;

			if (
				isVerticalEdge && (Screen.HasWaterLeft || Screen.HasWaterRight) ||
				!isVerticalEdge && (Screen.HasWaterTop || Screen.HasWaterBottom)
			) {
				AssignConstructedEdgeForWater(edge);
				return;
			}

			int gapsCount = Utilities.GetRandomInt(1, 2);
			int gapWidth = gapsCount == 0
				? Utilities.GetRandomInt(3, 6)
				: Utilities.GetRandomInt(2, 3);

			while (gapWidth * gapsCount + 6 > edgeSize) {
				gapWidth--;
			}

			List<bool> edgeProfile = new List<bool>();

			for (int gap = 0; gap < gapsCount; gap++) {
				edgeProfile.Add(true);
				edgeProfile.Add(true);
				for (int i = 0; i < gapWidth; i++) {
					edgeProfile.Add(false);
				}
			}

			edgeProfile.Add(true);

			if (!isVerticalEdge) {
				edgeProfile.Add(true);
				edgeProfile.Insert(0, true);
			}

			while (edgeProfile.Count < edgeSize) {
				List<int> solidEdges = Enumerable.Range(0, edgeProfile.Count)
					.Where(i => edgeProfile[i])
					.ToList();

				edgeProfile.Insert(solidEdges[Utilities.GetRandomInt(0, solidEdges.Count - 1)], true);
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

		private void AssignConstructedEdgeForWater(Direction edge) {
			if (edge == Direction.Up || edge == Direction.Down) {
				List<bool> edgeProfile = new List<bool>();

				if (Screen.HasWaterLeft) {
					for (int i = 0; i < Game.TilesWide; i++) {
						edgeProfile.Add(i < 9 || i > 11);
					}
				} else if (Screen.HasWaterRight) {
					for (int i = 0; i < Game.TilesWide; i++) {
						edgeProfile.Add(i < 4 || i > 6);
					}
				}

				if (edge == Direction.Up) {
					Screen.EdgeNorth = edgeProfile;
				} else {
					Screen.EdgeSouth = edgeProfile;
				}
			} else if (edge == Direction.Left || edge == Direction.Right) {
				List<bool> edgeProfile = new List<bool>();

				if (Screen.HasWaterTop) {
					for (int i = 0; i < Game.TilesHigh; i++) {
						edgeProfile.Add(i < 5 || i > 7);
					}
				} else if (Screen.HasWaterBottom) {
					for (int i = 0; i < Game.TilesHigh; i++) {
						edgeProfile.Add(i < 3 || i > 5);
					}
				}

				if (edge == Direction.Left) {
					Screen.EdgeWest = edgeProfile;
				} else {
					Screen.EdgeEast = edgeProfile;
				}
			}
		}

		private void CloneOppositeEdges() {
			if (Screen.ExitsWest && Screen.ExitsEast) {
				if (Screen.GetScreenLeft().IsAlreadyGenerated && !Screen.GetScreenRight().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Right);
				}

				if (!Screen.GetScreenLeft().IsAlreadyGenerated && Screen.GetScreenRight().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Left);
				}
			}

			if (Screen.ExitsNorth && Screen.ExitsSouth) {
				if (Screen.GetScreenUp().IsAlreadyGenerated && !Screen.GetScreenDown().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Down);
				}

				if (!Screen.GetScreenUp().IsAlreadyGenerated && Screen.GetScreenDown().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Up);
				}
			}
		}

		private void BuildEdgeTiles() {
			Screen screenUp = Screen.GetScreenUp();
			Screen screenDown = Screen.GetScreenDown();
			Screen screenLeft = Screen.GetScreenLeft();
			Screen screenRight = Screen.GetScreenRight();

			WriteEdgeTilesToScreen(Direction.Left, TileType.Tree);
			WriteEdgeTilesToScreen(Direction.Right, TileType.Tree);
			WriteEdgeTilesToScreen(Direction.Up, TileType.Tree);
			WriteEdgeTilesToScreen(Direction.Down, TileType.Tree);

			if (Screen.ExitsNorth && screenUp != null && _rockyBiomes.Contains(screenUp.Region.Biome)) {
				WriteEdgeTilesToScreen(Direction.Up, TileType.Rock);
			}

			if (Screen.ExitsSouth && screenDown != null && _rockyBiomes.Contains(screenDown.Region.Biome)) {
				WriteEdgeTilesToScreen(Direction.Down, TileType.Rock);
			}

			if (Screen.ExitsWest && screenLeft != null && _rockyBiomes.Contains(screenLeft.Region.Biome)) {
				WriteEdgeTilesToScreen(Direction.Left, TileType.Rock);
			}

			if (Screen.ExitsEast && screenRight != null && _rockyBiomes.Contains(screenRight.Region.Biome)) {
				WriteEdgeTilesToScreen(Direction.Right, TileType.Rock);
			}

			DoubleTopAndBottomEdges();
		}

		private void BuildArmosScreen() {
			TileDrawingTemplates.BuildArmosLayout(Screen);

			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			if (Screen.CaveDestination == Game.CaveLookup[CaveType.AnyRoad]) {
				TileDrawingTemplates.BuildAnyRoadFormation(Screen);
			} else if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}
		}

		private void BuildFairyPondScreen() {
			TileDrawingTemplates.BuildFairyLake(Screen);
			AddCorners(TileType.Tree);
		}

		private void BuildWhistleLakeScreen() {
			TileDrawingTemplates.BuildWhistleLake(Screen);
			AddCaveEntrance();
			AddCorners(TileType.Tree);
		}

		private void BuildDungeonEntranceScreen() {
			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			AddDungeonEntrance();
		}

		private void BuildGeneric() {
			if (_genericStyle == GenericStyle.MiddleFull) {
				TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 3, 4, Game.TilesWide - 4, Game.TilesHigh - 5);
			} else {
				BuildExtendEdgeScreen();
			}

			if (Screen.IsLake) {
				TileDrawingTemplates.BuildLake(Screen);
			}

			if (Screen.IsCoast) {
				TileDrawingTemplates.BuildCoast(Screen);
			}

			if (Screen.CaveDestination == Game.CaveLookup[CaveType.AnyRoad]) {
				TileDrawingTemplates.BuildAnyRoadFormation(Screen);
			} else if (Screen.CaveDestination > 0) {
				AddCaveEntrance();
			}
		}

		private void BuildExtendEdgeScreen() {
			TileDrawing.FillRectWithTiles(Screen, TileType.Tree, 1, 2, Game.TilesWide - 2, Game.TilesHigh - 3);

			for (int index = 0; index < Screen.EdgeNorth.Count; index++) {
				int westEnd = Screen.EdgeWest.LastIndexOf(false);
				int eastEnd = Screen.EdgeEast.LastIndexOf(false);

				int end = westEnd > eastEnd
					? westEnd
					: eastEnd;

				if (end < 0) {
					end = Game.TilesHigh - 3;
				}

				if (!Screen.EdgeNorth[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, index, 0, index, end);
				}
			}

			for (int index = 0; index < Screen.EdgeSouth.Count; index++) {
				int westEnd = Screen.EdgeWest.IndexOf(false);
				int eastEnd = Screen.EdgeEast.IndexOf(false);

				if (westEnd == -1) {
					westEnd = Game.TilesWide;
				}

				if (eastEnd == -1) {
					eastEnd = Game.TilesWide;
				}

				int end = westEnd < eastEnd
					? westEnd
					: eastEnd;

				if (end > Game.TilesWide - 1) {
					end = 2;
				}

				if (!Screen.EdgeSouth[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, index, end, index, Game.TilesHigh - 1);
				}
			}

			if (Screen.ExitsNorth && Screen.ExitsSouth && !Screen.ExitsWest && !Screen.ExitsEast) {
				int firstIndex = Screen.EdgeNorth.IndexOf(false) < Screen.EdgeSouth.IndexOf(false)
					? Screen.EdgeNorth.IndexOf(false)
					: Screen.EdgeSouth.IndexOf(false);

				int lastIndex = Screen.EdgeNorth.LastIndexOf(false) > Screen.EdgeSouth.LastIndexOf(false)
					? Screen.EdgeNorth.LastIndexOf(false)
					: Screen.EdgeSouth.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, firstIndex, 2, lastIndex, 2);
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground,
					firstIndex, Game.TilesHigh - 3, lastIndex, Game.TilesHigh - 3
				);
			}

			for (int index = 0; index < Screen.EdgeWest.Count; index++) {
				int northEnd = Screen.EdgeNorth.LastIndexOf(false);
				int southEnd = Screen.EdgeSouth.LastIndexOf(false);

				int end = northEnd > southEnd
					? northEnd
					: southEnd;

				if (end < 0) {
					end = Game.TilesWide - 2;
				}

				if (!Screen.EdgeWest[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 0, index, end, index);
				}
			}

			for (int index = 0; index < Screen.EdgeEast.Count; index++) {
				int northEnd = Screen.EdgeNorth.IndexOf(false);
				int southEnd = Screen.EdgeSouth.IndexOf(false);

				if (northEnd == -1) {
					northEnd = Game.TilesWide;
				}

				if (southEnd == -1) {
					southEnd = Game.TilesWide;
				}

				int end = northEnd < southEnd
					? northEnd
					: southEnd;

				if (end > Game.TilesWide - 1) {
					end = 1;
				}

				if (!Screen.EdgeEast[index]) {
					TileDrawing.FillRectWithTiles(Screen, TileType.Ground, end, index, Game.TilesWide - 1, index);
				}
			}

			if (Screen.ExitsWest && Screen.ExitsEast && !Screen.ExitsNorth && !Screen.ExitsSouth) {
				int firstIndex = Screen.EdgeWest.IndexOf(false) < Screen.EdgeEast.IndexOf(false)
					? Screen.EdgeWest.IndexOf(false)
					: Screen.EdgeEast.IndexOf(false);

				int lastIndex = Screen.EdgeWest.LastIndexOf(false) > Screen.EdgeEast.LastIndexOf(false)
					? Screen.EdgeWest.LastIndexOf(false)
					: Screen.EdgeEast.LastIndexOf(false);

				TileDrawing.FillRectWithTiles(Screen, TileType.Ground, 1, firstIndex, 1, lastIndex);
				TileDrawing.FillRectWithTiles(Screen, TileType.Ground,
					Game.TilesWide - 2, firstIndex, Game.TilesWide - 2, lastIndex
				);
			}
		}

		private void AddDungeonEntrance() {
			TileDrawingTemplates.BuildDungeonEntrance(Screen);

			if (!Screen.IsLake && Utilities.GetRandomInt(0, 1) == 1) {
				Screen.PaletteInterior = 2;
			}
		}

		public override void AssignEnemies() {
			if (Screen.IsFairyPond) {
				return;
			}

			int enemySet = Utilities.GetRandomInt(0, 2);

			if (enemySet == 0) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 1) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.MoblinBlue];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.MoblinBlue_MoblinBlue_MoblinBlue_MoblinRed_MoblinRed_MoblinBlue
				];
				Screen.UsesMixedEnemies = true;
			}

			if (Screen.IsLake || Screen.IsCoast) {
				Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Four];
			}

			Screen.EnemiesEnterFromSides = !Utilities.EnemiesCanSpawnOnScreen(Screen) ||
			                               _genericStyle == GenericStyle.ExtendEdge &&
			                               Screen.TotalExits > 1 &&
			                               Utilities.GetRandomInt(0, 3) <= 2;
		}
	}
}