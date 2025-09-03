using System.Collections.Generic;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.MapBuilder {
	public class Region {
		public int Id;
		public Biome Biome = Biome.StartZone;
		public readonly List<Screen> Screens = new List<Screen>();
		public readonly List<Screen> ExitScreens = new List<Screen>();
		public EnvironmentColor EnvironmentColor = EnvironmentColor.Grey;
		public int Priority = 0;
		
		public Region() {
			Game.Regions.Add(this);
			Id = Game.Regions.Count;
		}

		public void AddScreen(Screen screen) {
			if (!Screens.Contains(screen)) {
				Screens.Add(screen);
				screen.Region = this;
			}
		}

		public void RemoveScreen(Screen screen) {
			if (Screens.Contains(screen)) {
				Screens.Remove(screen);
			}
		}

		public void Reset() {
			Id = 0;
			Screens.Clear();
			ExitScreens.Clear();
		}

		public virtual void PlaceRegion() { }

		protected void PaintScreenRegion(int x, int y, int width, int height) {
			if (x < 0) {
				x = 0;
			}

			if (y < 0) {
				y = 0;
			}

			while (x + width > Game.ScreensWide) {
				x -= 1;
			}

			while (y + height > Game.ScreensHigh) {
				y -= 1;
			}

			for (int col = x; col < x + width; col++) {
				for (int row = y; row < y + height; row++) {
					Screen screen = Game.Screens[Utilities.GetScreenIndexFromColAndRow(col, row)];
					if (screen.Region == null) {
						screen.Region = this;
						AddScreen(screen);
					}
				}
			}
		}

		protected void ExpandRegion(int expansionSize = 2) {
			List<Screen> validScreens = new List<Screen>();

			foreach (Screen screen in Game.Screens) {
				if (screen.Region != null) {
					continue;
				}

				Screen screenUp = screen.GetScreenUp();
				Screen screenDown = screen.GetScreenDown();
				Screen screenLeft = screen.GetScreenLeft();
				Screen screenRight = screen.GetScreenRight();

				bool screenUpIsRegion = screenUp != null && screenUp.Region == this;
				bool screenDownIsRegion = screenDown != null && screenDown.Region == this;
				bool screenLeftIsRegion = screenLeft != null && screenLeft.Region == this;
				bool screenRightIsRegion = screenRight != null && screenRight.Region == this;

				if (screenUpIsRegion || screenDownIsRegion || screenLeftIsRegion || screenRightIsRegion) {
					validScreens.Add(screen);
				}
			}

			for (int i = 0; i < expansionSize; i++) {
				if (validScreens.Count > 0) {
					Screen validScreen = validScreens[Utilities.GetRandomInt(0, validScreens.Count - 1)];
					AddScreen(validScreen);
					validScreens.Remove(validScreen);
				}
			}
		}
	}
}