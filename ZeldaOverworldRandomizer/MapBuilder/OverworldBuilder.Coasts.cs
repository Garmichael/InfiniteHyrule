using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public static partial class OverworldBuilder {
		private static void AssignCoasts() {
			bool hasBottomLeftCoast = Utilities.GetRandomInt(0, 3) >= 1;
			bool hasBottomRightCoast = Utilities.GetRandomInt(0, 3) >= 1;

			if (!hasBottomLeftCoast && !hasBottomRightCoast) {
				if (Utilities.GetRandomInt(0, 1) == 0) {
					hasBottomLeftCoast = true;
				} else {
					hasBottomRightCoast = true;
				}
			}

			if (hasBottomLeftCoast) {
				int horizontalDistance = Utilities.GetRandomInt(5, 12);
				int verticalDistance = Utilities.GetRandomInt(4, 6);

				for (int i = 0; i < horizontalDistance; i++) {
					int screenId = Utilities.GetScreenIndexFromColAndRow(i, Game.LastScreenRow);
					Game.Screens[screenId].IsCoast = true;
				}

				for (int i = Game.LastScreenRow; i > Game.LastScreenRow - verticalDistance; i--) {
					int screenId = Utilities.GetScreenIndexFromColAndRow(0, i);
					Game.Screens[screenId].IsCoast = true;
				}
			}

			if (hasBottomRightCoast) {
				int horizontalDistance = Utilities.GetRandomInt(5, 12);
				int verticalDistance = Utilities.GetRandomInt(4, 6);

				for (int i = Game.LastScreenColumn; i > Game.LastScreenColumn - horizontalDistance; i--) {
					int screenId = Utilities.GetScreenIndexFromColAndRow(i, Game.LastScreenRow);
					Game.Screens[screenId].IsCoast = true;
				}

				for (int i = Game.LastScreenRow; i > Game.LastScreenRow - verticalDistance; i--) {
					int screenId = Utilities.GetScreenIndexFromColAndRow(Game.LastScreenColumn, i);
					Game.Screens[screenId].IsCoast = true;
				}
			}
		}
	}
}