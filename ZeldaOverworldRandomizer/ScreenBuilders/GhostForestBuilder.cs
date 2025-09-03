using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.ScreenBuilders {
	public class GhostForestBuilder : LightForestBuilder {
		public GhostForestBuilder(Screen screen) : base(screen) { }


		public override void BuildScreen() {
			base.BuildScreen();
			Screen.EnvironmentColor = EnvironmentColor.Grey;
			Screen.PaletteInterior = Screen.PaletteBorder;
		}
		
		public override void AssignEnemies() {
			if (Screen.IsFairyPond) {
				return;
			}

			EnemyCount count = Utilities.GetRandomInt(0, 1) == 0 ? EnemyCount.Four : EnemyCount.Five;
			Screen.EnemyCount = Game.EnemyCountLookup[count];
			Screen.EnemyId = Game.SingleEnemyTypeLookup[SingleEnemyTypes.GhiniMinion];
			Screen.UsesMixedEnemies = false;

			if (Utilities.GetRandomInt(0, 2) == 2) {
				Screen.EnemyCount = Game.EnemyCountLookup[EnemyCount.Six];
			}

			Screen.EnemiesEnterFromSides = true;

			if (!Utilities.EnemiesCanSpawnOnScreen(Screen)) {
				Screen.EnemiesEnterFromSides = false;
			}
		}
	}
}