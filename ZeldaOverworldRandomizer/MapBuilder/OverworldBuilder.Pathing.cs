using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void AssignPathing() {
			ConnectScreensInEachRegion();
			ConnectRegionsToEachOther();
			ForceFirstScreenOpen();

			if (GetTotalConnectingScreens() < Game.ScreensWide * Game.ScreensHigh) {
				Debug.Print("Rebuild Reason: Not every screen is connecting");
				ShouldRebuild = true;
			}
		}

		private static void ConnectScreensInEachRegion() {
			foreach (Region region in Game.Regions) {
				if (region.Screens.Count > 0) {
					SetExitFromRegionScreens(region);

					if (region.Biome != Biome.River) {
						while (!AllScreensInRegionAreConnected(region.Screens)) {
							foreach (Screen screen in region.Screens) {
								if (!screen.IsRegionConnected) {
									AttemptToConnectToConnected(screen);
								}
							}
						}
					}
				}
			}
		}

		private static void SetExitFromRegionScreens(Region region) {
			Dictionary<Screen, List<Region>> connections = GetAllConnectingRegions(region);

			if (region.Biome == Biome.Tunnel) {
				region.ExitScreens.Add(region.Screens[0]);
				region.ExitScreens.Add(region.Screens[region.Screens.Count - 1]);
			} else {
				int totalExitsForBiome = 0;

				if (region.Biome == Biome.StartZone) {
					totalExitsForBiome = 4;
				}

				if (region.Biome == Biome.RockyBeach) {
					totalExitsForBiome = (int) Math.Ceiling(region.Screens.Count / 3f);
				}

				if (region.Biome == Biome.DenseForest || region.Biome == Biome.LightForest) {
					totalExitsForBiome = (int) Math.Ceiling(region.Screens.Count / 4f);
				}

				if (region.Biome == Biome.MountainRange) {
					totalExitsForBiome = 2;
				}

				if (region.Biome == Biome.Graveyard) {
					totalExitsForBiome = 2;
				}

				if (region.Biome == Biome.Kakariko) {
					totalExitsForBiome = 2;
				}

				if (region.Biome == Biome.River) {
					totalExitsForBiome = (int) Math.Ceiling(region.Screens.Count / 2f);
				}

				if (totalExitsForBiome == 0) {
					totalExitsForBiome = 1;
				}

				for (int i = 0; i < totalExitsForBiome; i++) {
					if (connections.Count > 0) {
						int randomIndex = Utilities.GetRandomInt(0, connections.Count - 1);
						KeyValuePair<Screen, List<Region>> exitScreen = connections.ElementAt(randomIndex);
						region.ExitScreens.Add(exitScreen.Key);
						connections.Remove(exitScreen.Key);
					}
				}
			}

			if (region.ExitScreens.Count > 0) {
				region.ExitScreens[Utilities.GetRandomInt(0, region.ExitScreens.Count - 1)].IsRegionConnected = true;
			} else {
				Screen startScreen = region.Screens[Utilities.GetRandomInt(0, region.Screens.Count - 1)];
				startScreen.IsRegionConnected = true;
			}
		}

		private static Dictionary<Screen, List<Region>> GetAllConnectingRegions(Region region) {
			Dictionary<Screen, List<Region>> connections = new Dictionary<Screen, List<Region>>();

			foreach (Screen screen in region.Screens) {
				Screen screenUp = screen.GetScreenUp();
				Screen screenRight = screen.GetScreenRight();
				Screen screenDown = screen.GetScreenDown();
				Screen screenLeft = screen.GetScreenLeft();

				if (screenUp != null && screenUp.Region != screen.Region ||
				    screenRight != null && screenRight.Region != screen.Region ||
				    screenDown != null && screenDown.Region != screen.Region ||
				    screenLeft != null && screenLeft.Region != screen.Region
				) {
					if (!connections.ContainsKey(screen)) {
						connections.Add(screen, new List<Region>());
					}

					if (!connections[screen].Contains(region)) {
						connections[screen].Add(region);
					}
				}
			}

			return connections;
		}

		private static bool AllScreensInRegionAreConnected(List<Screen> screens) {
			bool allConnected = true;

			foreach (Screen screen in screens) {
				if (!screen.IsRegionConnected) {
					allConnected = false;
					break;
				}
			}

			return allConnected;
		}

		private static void AttemptToConnectToConnected(Screen screen) {
			Screen screenUp = screen.GetScreenUp();
			Screen screenDown = screen.GetScreenDown();
			Screen screenLeft = screen.GetScreenLeft();
			Screen screenRight = screen.GetScreenRight();

			List<Screen> possibleConnections = new List<Screen>();

			if (screenUp != null && screenUp.Region == screen.Region && screenUp.IsRegionConnected) {
				possibleConnections.Add(screenUp);
			}

			if (screenDown != null && screenDown.Region == screen.Region && screenDown.IsRegionConnected) {
				possibleConnections.Add(screenDown);
			}

			if (screenRight != null && screenRight.Region == screen.Region && screenRight.IsRegionConnected) {
				possibleConnections.Add(screenRight);
			}

			if (screenLeft != null && screenLeft.Region == screen.Region && screenLeft.IsRegionConnected) {
				possibleConnections.Add(screenLeft);
			}

			if (possibleConnections.Count == 0) {
				return;
			}

			Dictionary<Biome, List<int>> maxTotalConnectionsPerBiome = new Dictionary<Biome, List<int>> {
				{Biome.StartZone, new List<int> {3}},
				{Biome.RockyBeach, new List<int> {3}},
				{Biome.Tunnel, new List<int> {3}},
				{Biome.DenseForest, new List<int> {1, 1, 1, 2, 2, 3}},
				{Biome.LightForest, new List<int> {1, 1, 1, 2, 2, 3}},
				{Biome.GhostForest, new List<int> {2, 2, 3}},
				{Biome.Graveyard, new List<int> {2, 2, 3}},
				{Biome.Kakariko, new List<int> {1, 1, 1, 2, 2, 3}},
				{Biome.Desert, new List<int> {2, 2, 3}},
				{Biome.MountainRange, new List<int> {1, 1, 1, 1, 1, 2, 2}},
				{Biome.Island, new List<int> {4}}
			};

			List<int> odds = maxTotalConnectionsPerBiome.ContainsKey(screen.Region.Biome)
				? maxTotalConnectionsPerBiome[screen.Region.Biome]
				: new List<int> {3};

			int totalConnections = odds[Utilities.GetRandomInt(0, odds.Count - 1)];

			if (totalConnections > possibleConnections.Count) {
				totalConnections = possibleConnections.Count;
			}

			for (int i = 0; i < totalConnections; i++) {
				Screen screenToConnectTo =
					possibleConnections[Utilities.GetRandomInt(0, possibleConnections.Count - 1)];

				if (screenToConnectTo == screenUp && screenToConnectTo != null) {
					screen.ExitsNorth = true;
					screenToConnectTo.ExitsSouth = true;
				} else if (screenToConnectTo == screenDown && screenToConnectTo != null) {
					screen.ExitsSouth = true;
					screenToConnectTo.ExitsNorth = true;
				} else if (screenToConnectTo == screenRight && screenToConnectTo != null) {
					screen.ExitsEast = true;
					screenToConnectTo.ExitsWest = true;
				} else if (screenToConnectTo == screenLeft && screenToConnectTo != null) {
					screen.ExitsWest = true;
					screenToConnectTo.ExitsEast = true;
				}

				possibleConnections.Remove(screenToConnectTo);
			}

			screen.IsRegionConnected = true;
		}

		private static void ConnectRegionsToEachOther() {
			foreach (Region region in Game.Regions) {
				ConnectRegionToNeighbors(region);
			}
		}

		private static void ConnectRegionToNeighbors(Region region) {
			foreach (Screen screen in region.ExitScreens) {
				List<Direction> possibleExitDirections = new List<Direction>();
				Screen screenUp = screen.GetScreenUp();
				Screen screenDown = screen.GetScreenDown();
				Screen screenLeft = screen.GetScreenLeft();
				Screen screenRight = screen.GetScreenRight();

				if (screenUp != null && screenUp.Region != screen.Region) {
					possibleExitDirections.Add(Direction.Up);
				}

				if (screenDown != null && screenDown.Region != screen.Region) {
					possibleExitDirections.Add(Direction.Down);
				}

				if (screenRight != null && screenRight.Region != screen.Region) {
					possibleExitDirections.Add(Direction.Right);
				}

				if (screenLeft != null && screenLeft.Region != screen.Region) {
					possibleExitDirections.Add(Direction.Left);
				}

				if (possibleExitDirections.Count > 0) {
					Direction direction =
						possibleExitDirections[Utilities.GetRandomInt(0, possibleExitDirections.Count - 1)];

					if (screenUp != null && direction == Direction.Up) {
						screen.ExitsNorth = true;
						screenUp.ExitsSouth = true;
					}

					if (screenDown != null && direction == Direction.Down) {
						screen.ExitsSouth = true;
						screenDown.ExitsNorth = true;
					}

					if (screenRight != null && direction == Direction.Right) {
						screen.ExitsEast = true;
						screenRight.ExitsWest = true;
					}

					if (screenLeft != null && direction == Direction.Left) {
						screen.ExitsWest = true;
						screenLeft.ExitsEast = true;
					}
				}
			}
		}

		private static int GetTotalConnectingScreens() {
			List<Screen> connectedScreens = new List<Screen>();
			GrowConnectingScreens(Game.Screens[0], connectedScreens);

			return connectedScreens.Count;
		}

		private static void GrowConnectingScreens(Screen screen, List<Screen> connectedScreens) {
			connectedScreens.Add(screen);

			Screen screenUp = screen.GetScreenUp();
			Screen screenDown = screen.GetScreenDown();
			Screen screenRight = screen.GetScreenRight();
			Screen screenLeft = screen.GetScreenLeft();

			if (screen.ExitsNorth && screenUp != null && !connectedScreens.Contains(screenUp)) {
				GrowConnectingScreens(screenUp, connectedScreens);
			}

			if (screen.ExitsSouth && screenDown != null && !connectedScreens.Contains(screenDown)) {
				GrowConnectingScreens(screenDown, connectedScreens);
			}

			if (screen.ExitsEast && screenRight != null && !connectedScreens.Contains(screenRight)) {
				GrowConnectingScreens(screenRight, connectedScreens);
			}

			if (screen.ExitsWest && screenLeft != null && !connectedScreens.Contains(screenLeft)) {
				GrowConnectingScreens(screenLeft, connectedScreens);
			}
		}

		private static void ForceFirstScreenOpen() {
			Game.Screens[0].ExitsSouth = true;
			Game.Screens[16].ExitsNorth = true;
			Game.Screens[0].ExitsEast = true;
			Game.Screens[1].ExitsWest = true;
		}
	}
}