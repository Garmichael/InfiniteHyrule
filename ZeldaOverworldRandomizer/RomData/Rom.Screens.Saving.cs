using System.Collections.Generic;
using System.Text;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.RomData {
	public static partial class Rom {
		private static readonly List<List<int>> ScreenByteTables = new List<List<int>>();

		private static void SaveScreenData() {
			FillScreenByteData();
			UpdateTableData();
		}

		private static void FillScreenByteData() {
			ScreenByteTables.Clear();

			const int totalTables = 5;
			for (int tableIndex = 0; tableIndex < totalTables; tableIndex++) {
				List<int> tableData = new List<int>();

				for (int screenIndex = 0; screenIndex < TotalScreens; screenIndex++) {
					tableData.Add(GetOriginalData(ScreenAttributeTableIndexes[tableIndex], screenIndex));
				}

				ScreenByteTables.Add(tableData);
			}
		}

		private static int GetOriginalData(int tableStart, int screenIndex) {
			return _romData[tableStart + screenIndex];
		}

		private static void UpdateTableData() {
			for (int i = 0; i < Game.Screens.Count; i++) {
				Screen screen = Game.Screens[i];
				
				if (i > 0) {	//Prevents weird screen glitch when bringing up the submenu
					UpdateTableDataBits(screen.ExitCavePositionX, ScreenByteTables[0], i, 0, 3);
				}

				UpdateTableDataBits(screen.HasZora ? 1 : 0, ScreenByteTables[0], i, 4, 4);
				UpdateTableDataBits(screen.HasOceanSound ? 1 : 0, ScreenByteTables[0], i, 5, 5);
				UpdateTableDataBits(screen.PaletteBorder, ScreenByteTables[0], i, 6, 7);

				UpdateTableDataBits(screen.CaveDestination, ScreenByteTables[1], i, 0, 5);
				UpdateTableDataBits(screen.PaletteInterior, ScreenByteTables[1], i, 6, 7);

				UpdateTableDataBits(screen.EnemyCount, ScreenByteTables[2], i, 0, 1);
				UpdateTableDataBits(screen.EnemyId, ScreenByteTables[2], i, 2, 7);

				UpdateTableDataBits(screen.UsesMixedEnemies ? 1 : 0, ScreenByteTables[3], i, 0, 0);
				UpdateTableDataBits(screen.LayoutId, ScreenByteTables[3], i, 1, 7);

				UpdateTableDataBits(screen.HasQuestTwoSecret ? 1 : 0, ScreenByteTables[4], i, 0, 0);
				UpdateTableDataBits(screen.HasQuestOneSecret ? 1 : 0, ScreenByteTables[4], i, 1, 1);
				UpdateTableDataBits(screen.PushedStairsPositionId, ScreenByteTables[4], i, 2, 3);
				UpdateTableDataBits(screen.EnemiesEnterFromSides ? 1 : 0, ScreenByteTables[4], i, 4, 4);
				UpdateTableDataBits(screen.ExitCavePositionY, ScreenByteTables[4], i, 5, 7);
			}
		}

		private static void UpdateTableDataBits(int value, List<int> table, int screenIndex, int bitStart, int bitEnd) {
			int lengthOfBits = bitEnd - bitStart + 1;
			string binaryInputValue = Utilities.GetBinaryFromInt(value, lengthOfBits);
			string binaryTableValue = Utilities.GetBinaryFromInt(table[screenIndex]);

			for (int i = bitStart; i < bitStart + lengthOfBits; i++) {
				StringBuilder binary = new StringBuilder(binaryTableValue) {[i] = binaryInputValue[i - bitStart]};
				binaryTableValue = binary.ToString();
			}

			table[screenIndex] = Utilities.GetIntFromBinary(binaryTableValue);
		}

		private static void WriteScreenDataToRom() {
			for (int i = 0; i < ScreenByteTables.Count; i++) {
				int romStartIndex = ScreenAttributeTableIndexes[i];
				List<int> data = ScreenByteTables[i];

				for (int j = 0; j < data.Count; j++) {
					_romData[romStartIndex + j] = (byte) data[j];
				}
			}
		}
	}
}