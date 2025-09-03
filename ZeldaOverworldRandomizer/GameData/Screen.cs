using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.MapBuilder;

namespace ZeldaOverworldRandomizer.GameData {
	public class Screen {
		public bool IsRebuildable;

		public int ScreenId;

		public int PaletteBorder {
			get {
				if (EnvironmentColor == EnvironmentColor.Brown) {
					return 3;
				}

				if (EnvironmentColor == EnvironmentColor.Green) {
					return 2;
				}

				return 0;
			}
		}

		public int PaletteInterior;

		public int EnemyId;
		public int LayoutId => ScreenId;

		public bool UsesMixedEnemies;
		public bool EnemiesEnterFromSides;
		public int EnemyCount;

		public bool HasQuestOneSecret;
		public bool HasQuestTwoSecret;

		public EnvironmentColor EnvironmentColor = EnvironmentColor.Green;
		public bool IsGraveyard;
		public bool IsLake;
		public bool IsCoast;
		public bool HasRiverBridge;

		public bool HasWaterTopLeft {
			get {
				bool lakeFeatureMatches = IsLake &&
				                          (LakeSpot == NineSliceSpot.BottomRight ||
				                           LakeSpot == NineSliceSpot.Right ||
				                           LakeSpot == NineSliceSpot.Bottom
				                          );

				bool verticalCoastMatches = Column == 0 && IsCoast &&
				                            (GetScreenUp() == null || GetScreenUp().IsCoast);

				bool horizontalCoastMatches = Row == 0 && IsCoast &&
				                              (GetScreenLeft() == null || GetScreenLeft().IsCoast);

				return lakeFeatureMatches || verticalCoastMatches || horizontalCoastMatches;
			}
		}

		public bool HasWaterTopRight {
			get {
				bool lakeFeatureMatches = IsLake &&
				                          (LakeSpot == NineSliceSpot.BottomLeft ||
				                           LakeSpot == NineSliceSpot.Left ||
				                           LakeSpot == NineSliceSpot.Bottom);

				bool verticalCoastMatches = Column == Game.ScreensWide - 1 && IsCoast &&
				                            (GetScreenUp() == null || GetScreenUp().IsCoast);

				bool horizontalCoastMatches = Row == 0 && IsCoast &&
				                              (GetScreenRight() == null || GetScreenRight().IsCoast);

				return lakeFeatureMatches || verticalCoastMatches || horizontalCoastMatches;
			}
		}

		public bool HasWaterBottomLeft {
			get {
				bool lakeFeatureMatches = IsLake &&
				                          (LakeSpot == NineSliceSpot.TopRight ||
				                           LakeSpot == NineSliceSpot.Right ||
				                           LakeSpot == NineSliceSpot.Top
				                          );

				bool verticalCoastMatches = Column == 0 && IsCoast &&
				                            (GetScreenDown() == null || GetScreenDown().IsCoast);

				bool horizontalCoastMatches = Row == Game.ScreensHigh - 1 && IsCoast &&
				                              (GetScreenLeft() == null || GetScreenLeft().IsCoast);

				return lakeFeatureMatches || verticalCoastMatches || horizontalCoastMatches;
			}
		}

		public bool HasWaterBottomRight {
			get {
				bool lakeFeatureMatches = IsLake &&
				                          (LakeSpot == NineSliceSpot.TopLeft ||
				                           LakeSpot == NineSliceSpot.Left ||
				                           LakeSpot == NineSliceSpot.Top);

				bool verticalCoastMatches = Column == Game.ScreensWide - 1 && IsCoast &&
				                            (GetScreenDown() == null || GetScreenDown().IsCoast);

				bool horizontalCoastMatches = Row == Game.ScreensHigh - 1 && IsCoast &&
				                              (GetScreenRight() == null || GetScreenRight().IsCoast);

				return lakeFeatureMatches || verticalCoastMatches || horizontalCoastMatches;
			}
		}

		public bool HasWaterLeft => HasWaterBottomLeft || HasWaterTopLeft;
		public bool HasWaterRight => HasWaterBottomRight || HasWaterTopRight;
		public bool HasWaterTop => HasWaterTopLeft || HasWaterTopRight;
		public bool HasWaterBottom => HasWaterBottomLeft || HasWaterBottomRight;

		public Region Region;
		public bool ExitsNorth;
		public bool ExitsEast;
		public bool ExitsSouth;
		public bool ExitsWest;

		public bool IsDock;
		public bool IsDockIsland;
		public bool IsDockable;
		public bool IsOasis;

		public bool HasZora => (IsLake || IsCoast || Region.Biome == Biome.River) &&
		                       ScreenId != Game.LinkStartScreen &&
		                       !IsAnyRoad &&
		                       !IsGraveyard;

		public bool HasOceanSound => IsCoast;
		public NineSliceSpot LakeSpot = NineSliceSpot.None;

		public bool IsAlreadyGenerated;
		public bool IsRegionConnected;

		public readonly List<int> Tiles = new List<int>();

		public bool CanBeFairyPond {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.DenseForest, Biome.LightForest, Biome.MountainRange, Biome.Graveyard
				};

				return
					!IsCoast &&
					!IsLake &&
					!IsDock &&
					!IsFairyPond &&
					!IsWhistleLake &&
					ScreenId != Game.LinkStartScreen &&
					acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeWhistleLake {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.DenseForest, Biome.LightForest, Biome.MountainRange
				};

				return
					ScreenId > 0 &&
					!IsCoast &&
					!IsLake &&
					!IsDock &&
					!IsFairyPond &&
					!IsWhistleLake &&
					ScreenId != Game.LinkStartScreen &&
					acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeSecretScreen {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.DenseForest, Biome.LightForest, Biome.MountainRange
				};

				List<Biome> acceptableEntranceBiomes = new List<Biome> {
					Biome.MountainRange, Biome.LightForest, Biome.Graveyard, Biome.GhostForest
				};

				return
					ScreenId > 0 &&
					TotalExits == 1 &&
					ExitsSouth &&
					!IsCoast &&
					!IsLake &&
					!IsDock &&
					!IsDockIsland &&
					!IsFairyPond &&
					!IsWhistleLake &&
					ScreenId != Game.LinkStartScreen &&
					acceptableEntranceBiomes.Contains(GetScreenDown().Region.Biome) &&
					acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool IsFairyPond;
		public bool IsWhistleLake;
		public bool IsOverworldItemScreen;
		public bool IsSecretScreen;


		public int ExitCavePositionX;
		public int ExitCavePositionY;
		public int CaveDestination;
		public bool CaveIsHidden;
		public int PushedStairsPositionId;

		public bool CanBeDungeon {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.Desert,
					Biome.Graveyard,
					Biome.Island,
					Biome.MountainRange
				};

				return ScreenId > 0 &&
				       CaveDestination == 0 &&
				       !IsDock &&
				       !IsDockIsland &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsOverworldItemScreen &&
				       !IsSecretScreen &&
				       (IsDeadEnd || Region.Biome == Biome.Island) &&
				       DistanceToStartScreen > 4 &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeSpecialCave {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.Desert,
					Biome.Graveyard,
					Biome.Island,
					Biome.MountainRange
				};

				return ScreenId > 0 &&
				       CaveDestination == 0 &&
				       !IsDock &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsOverworldItemScreen &&
				       !IsSecretScreen &&
				       (IsDeadEnd && !ExitsNorth || Region.Biome == Biome.Island || IsGraveyard) &&
				       (DistanceToStartScreen > 6 || IsDockIsland) &&
				       acceptableBiomes.Contains(Region.Biome) &&
				       !(IsGraveyard && IsLake && LakeSpot != NineSliceSpot.Middle) &&
				       !(IsGraveyard && IsCoast && Row == Game.LastScreenRow);
			}
		}

		public bool CanBeNormalOpenCave {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.MountainRange, Biome.Island, Biome.RockyBeach,
					Biome.Tunnel, Biome.StartZone, Biome.River, Biome.Graveyard, Biome.Kakariko
				};

				return ScreenId > 0 &&
				       !IsDock &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsOverworldItemScreen &&
				       !IsSecretScreen &&
				       CaveDestination == 0 &&
				       !(IsLake && LakeSpot == NineSliceSpot.Bottom) &&
				       !(Region.Biome == Biome.River && Row > 1) &&
				       !(Region.Biome == Biome.River && this == Region.Screens.Last()) &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeNormalHiddenCave {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.MountainRange, Biome.Desert, Biome.Island, Biome.RockyBeach, Biome.GhostForest,
					Biome.Tunnel, Biome.StartZone, Biome.LightForest, Biome.DenseForest, Biome.River, Biome.Graveyard,
					Biome.Kakariko
				};

				return ScreenId > 0 &&
				       !IsDock &&
				       !IsFairyPond &&
				       !IsOverworldItemScreen &&
				       !IsSecretScreen &&
				       CaveDestination == 0 &&
				       !(IsLake && LakeSpot == NineSliceSpot.Bottom) &&
				       !(Region.Biome == Biome.River && this == Region.Screens.Last()) &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeAnyRoad {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.DenseForest, Biome.LightForest, Biome.MountainRange, Biome.Tunnel
				};

				return ScreenId > 0 &&
				       !IsDock &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsOverworldItemScreen &&
				       !IsSecretScreen &&
				       ScreenId != Game.LinkStartScreen &&
				       !IsDeadEnd &&
				       CaveDestination == 0 &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeArmosStairs {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.LightForest, Biome.MountainRange
				};

				return ScreenId > 0 &&
				       CaveDestination > 0 &&
				       !IsOpenDungeon &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsAnyRoad &&
				       !IsSecretScreen &&
				       !IsDock &&
				       LakeSpot != NineSliceSpot.Bottom &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeArmosHiddenItem {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.LightForest, Biome.MountainRange, Biome.Island
				};

				return CaveDestination == 0 &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsDock &&
				       !IsSecretScreen &&
				       !IsOverworldItemScreen && 
				       LakeSpot != NineSliceSpot.Bottom &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool CanBeArmosWithNothing {
			get {
				List<Biome> acceptableBiomes = new List<Biome> {
					Biome.LightForest, Biome.MountainRange, Biome.Island
				};

				return !IsArmosCave &&
				       !IsArmosHiddenItem &&
				       !IsOpenDungeon &&
				       !IsFairyPond &&
				       !IsWhistleLake &&
				       !IsAnyRoad &&
				       !IsDock &&
				       !IsSecretScreen &&
				       LakeSpot != NineSliceSpot.Bottom &&
				       !(Region.Biome == Biome.LightForest && CaveDestination > 0) &&
				       acceptableBiomes.Contains(Region.Biome);
			}
		}

		public bool IsAnyRoad => CaveDestination == Game.CaveLookup[CaveType.AnyRoad];
		public bool IsOpenDungeon => Game.DungeonCaves.Contains(Game.CaveLookupById[CaveDestination]) && !CaveIsHidden;
		public bool IsHiddenDungeon => Game.DungeonCaves.Contains(Game.CaveLookupById[CaveDestination]) && CaveIsHidden;

		public bool IsArmosCave;
		public bool IsArmosHiddenItem;
		public bool IsArmosDecor;

		public int TotalExits {
			get {
				int totalExits = 0;
				if (ExitsEast) {
					totalExits++;
				}

				if (ExitsNorth) {
					totalExits++;
				}

				if (ExitsWest) {
					totalExits++;
				}

				if (ExitsSouth) {
					totalExits++;
				}

				return totalExits;
			}
		}

		public bool IsDeadEnd => TotalExits == 1;

		public int Column => ScreenId % Game.ScreensWide;
		public int Row => (int) Math.Floor(ScreenId / (float) Game.ScreensWide);

		public bool IsFirstColumn => Column == 0;
		public bool IsLastColumn => Column == Game.ScreensWide - 1;
		public bool IsFirstRow => Row == 0;
		public bool IsLastRow => Row == Game.ScreensHigh - 1;

		public List<bool> EdgeNorth = new List<bool>();
		public List<bool> EdgeSouth = new List<bool>();
		public List<bool> EdgeWest = new List<bool>();
		public List<bool> EdgeEast = new List<bool>();

		public Screen GetScreenUp() {
			return Row == 0
				? null
				: Game.Screens[Utilities.GetScreenIndexFromColAndRow(Column, Row - 1)];
		}

		public Screen GetScreenRight() {
			return Column == Game.ScreensWide - 1
				? null
				: Game.Screens[Utilities.GetScreenIndexFromColAndRow(Column + 1, Row)];
		}

		public Screen GetScreenDown() {
			return Row == Game.ScreensHigh - 1
				? null
				: Game.Screens[Utilities.GetScreenIndexFromColAndRow(Column, Row + 1)];
		}

		public Screen GetScreenLeft() {
			return Column == 0
				? null
				: Game.Screens[Utilities.GetScreenIndexFromColAndRow(Column - 1, Row)];
		}

		public int DistanceToStartScreen => DistanceToScreen(Game.Screens[Game.LinkStartScreen]);

		public int DistanceToScreen(Screen destinationScreen) {
			int horizontalDistance = Math.Abs(destinationScreen.Column - Column);
			int verticalDistance = Math.Abs(destinationScreen.Row - Row);
			return horizontalDistance + verticalDistance;
		}

		public int JourneyDistanceToStartScreen => JourneyDistanceToScreen(Game.Screens[Game.LinkStartScreen]);

		public int JourneyDistanceToScreen(Screen destinationScreen, bool weighted = true) {
			List<PathingNode> nodes = new List<PathingNode>();

			for (int i = 0; i < Game.Screens.Count; i++) {
				nodes.Add(new PathingNode { Id = i });
			}

			PathingNode targetNode = nodes[destinationScreen.ScreenId];
			nodes[ScreenId].Status = PathingNodeStatus.Open;

			while (true) {
				List<PathingNode> openNodes = nodes.Where(node => node.Status == PathingNodeStatus.Open).ToList();

				foreach (PathingNode node in openNodes) {
					node.Status = PathingNodeStatus.Closed;

					Screen currentScreen = Game.Screens[node.Id];
					List<Screen> neighbors = new List<Screen>();

					if (currentScreen.ExitsNorth) {
						neighbors.Add(currentScreen.GetScreenUp());
					}

					if (currentScreen.ExitsSouth) {
						neighbors.Add(currentScreen.GetScreenDown());
					}

					if (currentScreen.ExitsWest) {
						neighbors.Add(currentScreen.GetScreenLeft());
					}

					if (currentScreen.ExitsEast) {
						neighbors.Add(currentScreen.GetScreenRight());
					}

					foreach (Screen neighbor in neighbors) {
						PathingNode matchingNode = nodes[neighbor.ScreenId];
						if (matchingNode.Status == PathingNodeStatus.Unchecked) {
							matchingNode.Status = PathingNodeStatus.Open;
							matchingNode.ParentNode = node;
						}
					}
				}

				if (openNodes.Contains(targetNode) || openNodes.Count == 0) {
					break;
				}
			}

			int totalDistance = 0;
			PathingNode currentOnPath = targetNode;
			while (currentOnPath.ParentNode != null) {
				currentOnPath = currentOnPath.ParentNode;
				Screen matchingScreen = Game.Screens[currentOnPath.Id];

				int screenCost = 1;

				if (weighted && matchingScreen.Region == Game.Regions[1]) {
					screenCost = 5;
				}

				totalDistance += screenCost;
			}

			return totalDistance;
		}
	}
}