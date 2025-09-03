using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;
using ZeldaOverworldRandomizer.ScreenBuildingTools;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class LightForestBuilder : ScreenBuilder {
		public LightForestBuilder(Screen screen) : base(screen) { }

		private readonly List<Biome> _rockyBiomes = new List<Biome>
			{ Biome.MountainRange, Biome.RockyBeach, Biome.Tunnel, Biome.StartZone };

		public override void BuildScreen() {
			Screen.EnvironmentColor = Screen.Region.EnvironmentColor;
			Screen.PaletteInterior = Screen.PaletteBorder;

			AssignEdges();
			CloneOppositeEdges();
			BuildEdgeTiles();

			Screen.PaletteInterior = Screen.PaletteBorder;

			if (Screen.IsArmosCave || Screen.IsArmosHiddenItem || Screen.IsArmosDecor) {
				BuildArmosScreen();
			} else if (Screen.IsFairyPond) {
				TileDrawingTemplates.BuildFairyLake(Screen);
			} else if (Screen.IsWhistleLake) {
				TileDrawingTemplates.BuildWhistleLake(Screen);
				AddCaveEntrance();
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
			
			if (Screen.GetScreenUp() != null && Screen.GetScreenUp().IsSecretScreen) {
				TileType tile = Utilities.GetTile(Screen, Utilities.GetTileByColAndRow(7, 0));
				TileDrawing.DrawTile(Screen, tile, 8, 0);
				TileDrawing.DrawTile(Screen, tile, 8, 1);
			}
		}

		protected override void AssignConstructedEdge(Direction edge) {
			if (edge == Direction.Up || edge == Direction.Down) {
				bool matchingBiome = edge == Direction.Up
					? Screen.GetScreenUp().Region.Biome == Screen.Region.Biome
					: Screen.GetScreenDown().Region.Biome == Screen.Region.Biome;

				List<bool> edgeProfile = new List<bool>();

				if (edge == Direction.Up && Screen.GetScreenUp() != null && Screen.GetScreenUp().IsSecretScreen ||
				    edge == Direction.Down && Screen.IsSecretScreen
				) {
					for (int i = 0; i < Game.TilesWide; i++) {
						edgeProfile.Add(true);
					}

					edgeProfile[8] = false;
				} else if (matchingBiome) {
					int totalExits = Utilities.GetRandomInt(4, 6);

					edgeProfile.Add(true);

					for (int i = 0; i < totalExits; i++) {
						edgeProfile.Add(false);
						edgeProfile.Add(true);
					}

					edgeProfile.Add(true);

					while (edgeProfile.Count < 16) {
						List<int> indexesOfGaps = new List<int>();
						for (int i = 0; i < edgeProfile.Count; i++) {
							if (!edgeProfile[i]) {
								indexesOfGaps.Add(i);
							}
						}

						int indexOfNewSpace = indexesOfGaps[Utilities.GetRandomInt(0, indexesOfGaps.Count - 1)];

						edgeProfile.Insert(indexOfNewSpace, false);
					}
				} else {
					edgeProfile = new List<bool> {
						true, true, true,
						false, false, false, false, false, false, false, false, false, false,
						true, true, true
					};
				}

				if (edge == Direction.Up) {
					Screen.EdgeNorth = edgeProfile;
				} else {
					Screen.EdgeSouth = edgeProfile;
				}
			} else {
				List<bool> edgeProfile = new List<bool> {
					true, true, true,
					false, false, false, false, false,
					true, true, true,
				};

				if (edge == Direction.Left) {
					Screen.EdgeWest = edgeProfile;
				} else {
					Screen.EdgeEast = edgeProfile;
				}
			}
		}

		private void CloneOppositeEdges() {
			if (Screen.ExitsWest &&
			    Screen.ExitsEast &&
			    Screen.GetScreenLeft().Region.Biome == Screen.Region.Biome &&
			    Screen.GetScreenRight().Region.Biome == Screen.Region.Biome
			) {
				if (Screen.GetScreenLeft().IsAlreadyGenerated && !Screen.GetScreenRight().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Right);
				}

				if (!Screen.GetScreenLeft().IsAlreadyGenerated && Screen.GetScreenRight().IsAlreadyGenerated) {
					AssignCloneEdge(Screen, Direction.Left);
				}
			}

			if (Screen.ExitsNorth &&
			    Screen.ExitsSouth &&
			    Screen.GetScreenUp().Region.Biome == Screen.Region.Biome &&
			    Screen.GetScreenDown().Region.Biome == Screen.Region.Biome
			) {
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

			if (Screen.ExitsNorth && screenUp != null && _rockyBiomes.Contains(screenUp.Region.Biome) && !screenUp.IsSecretScreen) {
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
			} else if (Screen.CaveDestination > 0 && Screen.IsArmosDecor) {
				AddCaveEntrance();
			}
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
			BuildInterior();

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

		protected virtual void BuildInterior() {
			List<int> columns = new List<int>();
			if (Screen.ExitsNorth && Screen.GetScreenUp().Region.Biome == Screen.Region.Biome) {
				for (int i = 2; i < Screen.EdgeNorth.Count - 2; i++) {
					if (Screen.EdgeNorth[i]) {
						columns.Add(i);
					}
				}
			} else if (Screen.ExitsSouth && Screen.GetScreenDown().Region.Biome == Screen.Region.Biome) {
				for (int i = 2; i < Screen.EdgeSouth.Count - 2; i++) {
					if (Screen.EdgeSouth[i]) {
						columns.Add(i);
					}
				}
			} else {
				columns.Add(2);
				columns.Add(4);
				columns.Add(6);
				columns.Add(9);
				columns.Add(11);
				columns.Add(13);
			}

			int lastRowCount = 0;
			for (int i = 0; i < columns.Count; i++) {
				int rows;
				
				if (i > 0 && columns[i - 1] == columns[i] - 1) {
					rows = lastRowCount;
				} else {
					rows = Screen.HasWaterBottom || Screen.HasWaterTop
						? 2
						: Utilities.GetRandomInt(2, 3);
				}

				lastRowCount = rows;
				if (rows == 2) {
					TileDrawing.DrawTile(Screen, TileType.Tree, columns[i], 4);
					TileDrawing.DrawTile(Screen, TileType.Tree, columns[i], 6);
				} else {
					TileDrawing.DrawTile(Screen, TileType.Tree, columns[i], 3);
					TileDrawing.DrawTile(Screen, TileType.Tree, columns[i], 5);
					TileDrawing.DrawTile(Screen, TileType.Tree, columns[i], 7);
				}
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
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokRed];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 1) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.OctorokBlue];
				Screen.UsesMixedEnemies = false;
			} else if (enemySet == 2) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoBlueFast_OctoBlueFast_OctoRed_OctoRed_OctoRed_MoblinBlue
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 3) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoRed_OctoRed_OctoRedFast_OctoBlue
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 4) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoRedFast_OctoRedFast_OctoBlue_OctoBlue_OctoRedFast_OctoBlueFast
				];
				Screen.UsesMixedEnemies = true;
			} else if (enemySet == 5) {
				EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
				Screen.EnemyCount = Game.EnemyCountLookup[count];
				Screen.EnemyId = Game.MixedEnemyTypeLookup[
					MixedEnemyTypes.OctoBlue_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast_OctoRedFast
				];
				Screen.UsesMixedEnemies = true;
			}

			if (Screen.IsLake || Screen.IsCoast) {
				Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Four];
			}
			
			Screen.EnemiesEnterFromSides = Screen.TotalExits >= 3 &&
			                               !Screen.IsLake &&
			                               Utilities.GetRandomInt(0, 3) <= 2;

			if (!Utilities.EnemiesCanSpawnOnScreen(Screen)) {
				Screen.EnemiesEnterFromSides = false;
			}
		}
	}
}