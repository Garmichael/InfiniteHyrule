using System.Collections.Generic;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class DebugBuilder : ScreenBuilder {
		public DebugBuilder(Screen screen) : base(screen) { }

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
	}
}