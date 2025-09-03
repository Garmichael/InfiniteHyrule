using System.Collections.Generic;

namespace ZeldaOverworldRandomizer.RomData {
	public static partial class Rom {
		public static string FileName;
		private static List<byte> _romHeader;
		private static List<byte> _romData;
		private static bool _isProgZero;

		private static readonly List<int> ScreenAttributeTableIndexes = new List<int> {
			99328, //0x18400;
			99456, //0x18480;
			99584, //0x18500;
			99712, //0x18580;
			99968 //0x18680;
		};
		
		private const int ProgIndicator = 6203;
		private const int ProgZeroValue = 212;
		
		private const int TotalScreens = 128;
		private const int DockScreenA = 84292; // 0x14944;
		private const int DockScreenB = 84296; // 0x14948;

		private const int LinkPositionY = 103208; //0x19328;
		private const int LinkStartScreen = 103215; //0x1932F

		private const int OverworldItemPositionX = 96398; //0x1788E
		private const int OverworldItemPositionY = 96400; //0x17890
		private const int OverworldItemPositionYOffset = 4;
		private const int OverworldItemScreenId = 96410; //0x1789A

		private const int OverworldLadderScreens = 258573; //0x1F20D + 4 banks of 0x4000
		private const int ArmosCaveScreens = 68787; //0x10CB3
		private const int ArmosCaveScreenPositionX = 68794; //0x10CBA
		private const int ArmosSecretPositionY = 68837; //0x10CE5
		private const int WhistleSecretScreens = 257894; //0x1EF66 + 4 banks of 0x4000
		private const int WhistleDestinationScreens = 24592; //0x6010;

		private const int ArmosHiddenItemScreen = 68786; //0x10CB2
		private const int ArmosHiddenItemPositionX = 68793; //0x10CB9

		private const int AnyRoadScreen = 103220; //0x19334

		private const int LostScreenA = 28071; //0x6DA7
		private const int LostScreenB = 28105; //0x6DC9

		private const int PaletteA = 103171; // 0x19303
		private const int PaletteB = 103175; // 0x19307
		private const int PaletteC = 103179; // 0x1930B
		private const int PaletteD = 103183; // 0x1930F
		
		private const int SecretWallScreen = 257690;
		private const int SecretJingleScreen = 256652;
	}
}