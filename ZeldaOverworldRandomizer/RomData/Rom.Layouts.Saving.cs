using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.RomData {
	public static partial class Rom {
		private static void SaveLayoutData() {
			ResetExistingLayoutData();
			CreateLayoutsFromScreenTiles();
		}

		private static void ResetExistingLayoutData() {
			Game.ScreenLayouts.Clear();

			for (int i = 0; i < Game.ScreensWide * Game.ScreensHigh; i++) {
				Game.ScreenLayouts.Add(new List<Column>());
			}
		}

		private static void CreateLayoutsFromScreenTiles() {
			foreach (Screen screen in Game.Screens) {
				List<Column> columns = new List<Column>();

				for (int i = 0; i < Game.TilesWide; i++) {
					columns.Add(new Column());
				}

				for (int tileIndex = 0; tileIndex < screen.Tiles.Count; tileIndex++) {
					int column = Utilities.GetColFromTileIndex(tileIndex);
					columns[column].Tiles.Add(screen.Tiles[tileIndex]);
				}

				Game.ScreenLayouts[screen.LayoutId] = columns;
			}
		}

		private static void WriteLayoutDataToRom() {
			for (int layoutIndex = 0; layoutIndex < Game.ScreenLayouts.Count; layoutIndex++) {
				WriteLayoutIdBanner(layoutIndex);
				WriteTileData(layoutIndex);
			}
		}

		private static void WriteLayoutIdBanner(int layoutId) {
			int digitA;
			int digitB;
			int digitC;

			if (layoutId < 10) {
				digitA = 30 + layoutId;
				digitB = 20;
				digitC = 20;
			} else if (layoutId < 100) {
				string layoutIdString = layoutId.ToString();
				digitA = 30 + int.Parse(layoutIdString[0].ToString());
				digitB = 30 + int.Parse(layoutIdString[1].ToString());
				digitC = 20;
			} else {
				string layoutIdString = layoutId.ToString();
				digitA = 30 + int.Parse(layoutIdString[0].ToString());
				digitB = 30 + int.Parse(layoutIdString[1].ToString());
				digitC = 30 + int.Parse(layoutIdString[2].ToString());
			}

			string banner =
				"FF FF FF FF FF FF FF FF FF FF FF 3C 3D 3D 3D 5C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 20 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 20 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 4C 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 41 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 59 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 4F 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 55 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 54 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 20 20 7C " +
				$"FF FF FF FF FF FF FF FF FF FF FF 20 20 {digitA} 20 7C " +
				$"FF FF FF FF FF FF FF FF FF FF FF 20 20 {digitB} 20 7C " +
				$"FF FF FF FF FF FF FF FF FF FF FF 20 20 {digitC} 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 20 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 20 20 20 20 7C " +
				"FF FF FF FF FF FF FF FF FF FF FF 3C 3D 3D 3D 2F";

			ApplyHexAcrossBytes(GetRomAddressOfLayoutId(layoutId), banner);
		}

		private static void WriteTileData(int layoutId) {
			List<Column> layout = Game.ScreenLayouts[layoutId];
			int startAddress = GetRomAddressOfLayoutId(layoutId);

			for (int columnIndex = 0; columnIndex < layout.Count; columnIndex++) {
				Column column = layout[columnIndex];

				for (int tileIndex = 0; tileIndex < column.Tiles.Count; tileIndex++) {
					int tile = column.Tiles[tileIndex];

					_romData[startAddress + columnIndex * 16 + tileIndex] = (byte) tile;
				}
			}
		}

		private static int GetRomAddressOfLayoutId(int layoutId) {
			int bank = (int) Math.Floor(layoutId / 32f);
			int offset = layoutId % 32;

			return Utilities.GetIntFromHex("20000") + bank * Utilities.GetIntFromHex("4000") + offset * 256;
		}

		private static void WriteGanonHintToRom() {
			Dictionary<string, char> lettersById = new Dictionary<string, char> {
				{ "00", '0' }, { "01", '1' }, { "02", '2' }, { "03", '3' }, { "04", '4' }, { "05", '5' },
				{ "06", '6' }, { "07", '7' }, { "08", '8' }, { "09", '9' }, { "0A", 'A' }, { "0B", 'B' },
				{ "0C", 'C' }, { "0D", 'D' }, { "0E", 'E' }, { "0F", 'F' }, { "10", 'G' }, { "11", 'H' },
				{ "12", 'I' }, { "13", 'J' }, { "14", 'K' }, { "15", 'L' }, { "16", 'M' }, { "17", 'N' },
				{ "18", 'O' }, { "19", 'P' }, { "1A", 'Q' }, { "1B", 'R' }, { "1C", 'S' }, { "1D", 'T' },
				{ "1E", 'U' }, { "1F", 'V' }, { "20", 'W' }, { "21", 'X' }, { "22", 'Y' }, { "23", 'Z' },
				{ "24", ' ' }, { "25", '_' }, { "28", ',' }, { "29", '!' }, { "2A", '\'' }, { "2B", '&' },
				{ "2C", '.' }, { "2D", '"' }, { "2E", '?' }, { "2F", '-' }
			};

			Dictionary<char, string> letters = new Dictionary<char, string> {
				{ '0', "00" }, { '1', "01" }, { '2', "02" }, { '3', "03" }, { '4', "04" }, { '5', "05" },
				{ '6', "06" }, { '7', "07" }, { '8', "08" }, { '9', "09" }, { 'A', "0A" }, { 'B', "0B" },
				{ 'C', "0C" }, { 'D', "0D" }, { 'E', "0E" }, { 'F', "0F" }, { 'G', "10" }, { 'H', "11" },
				{ 'I', "12" }, { 'J', "13" }, { 'K', "14" }, { 'L', "15" }, { 'M', "16" }, { 'N', "17" },
				{ 'O', "18" }, { 'P', "19" }, { 'Q', "1A" }, { 'R', "1B" }, { 'S', "1C" }, { 'T', "1D" },
				{ 'U', "1E" }, { 'V', "1F" }, { 'W', "20" }, { 'X', "21" }, { 'Y', "22" }, { 'Z', "23" },
				{ ' ', "24" }, { '_', "25" }, { ',', "28" }, { '!', "29" }, { '\'', "2A" }, { '&', "2B" },
				{ '.', "2C" }, { '"', "2D" }, { '?', "2E" }, { '-', "2F" }
			};

			const int totalLength = 76;
			int tableStart = Utilities.GetIntFromHex("4000");
			List<int> tableOfContents = new List<int>();

			for (int index = 0; index < totalLength; index += 2) {
				int high = (_romData[tableStart + index + 1] - 64) * 256;
				int low = _romData[tableStart + index];

				tableOfContents.Add(high + low);
			}

			List<string> messages = new List<string>();

			foreach (int messageIndex in tableOfContents) {
				string message = "";
				int messageLetter = 0;
				while (true) {
					int characterCode = _romData[messageIndex + messageLetter];
					string binary = Utilities.GetBinaryFromInt(characterCode);
					bool isEndOfMessage = false;

					if (binary[0] == '1') {
						if (binary[1] == '1') {
							isEndOfMessage = true;
						}
					}

					message += Utilities.GetHexFromInt(characterCode);

					if (isEndOfMessage) {
						break;
					}

					message += " ";
					messageLetter++;
				}

				messages.Add(message);
			}

			//good luck: 10 18 18 0D
			Screen dungeon9 = Game.Screens.Where(
				screen => screen.CaveDestination == Game.CaveLookup[CaveType.Dungeon9]
			).ToList().First();

			Biome gannonBiome = dungeon9.Region.Biome;

			Dictionary<Biome, string> message1Lines = new Dictionary<Biome, string> {
				{ Biome.Graveyard, "_____GANON HIDES IN" },
				{ Biome.LightForest, "_____GANON HIDES IN" },
				{ Biome.MountainRange, "_____GANON HIDES IN" },
				{ Biome.Desert, "_____GANON HIDES IN" },
				{ Biome.River, "___GANON HIDES ALONG" },
				{ Biome.GhostForest, "_____GANON HIDES IN" },
				{ Biome.RockyBeach, "___GANON HIDES ALONG" }
			};

			Dictionary<Biome, string> message2Lines = new Dictionary<Biome, string> {
				{ Biome.Graveyard, "_____THE GRAVEYARD" },
				{ Biome.LightForest, "___THE SPARSE FOREST" },
				{ Biome.MountainRange, "_____THE MOUNTAINS" },
				{ Biome.Desert, "______THE DESERT" },
				{ Biome.River, "_______THE RIVER" },
				{ Biome.GhostForest, "__THE HAUNTED FOREST" },
				{ Biome.RockyBeach, "_______THE COAST" },
			};

			if (message1Lines.ContainsKey(gannonBiome) && message2Lines.ContainsKey(gannonBiome)) {
				string messageLine1 = message1Lines[gannonBiome];
				string messageLine2 = message2Lines[gannonBiome];

				List<string> hex = new List<string>();

				foreach (char character in messageLine1) {
					hex.Add(letters[character]);
				}

				string lastOnLine = Utilities.GetBinaryFromHex(hex.Last());
				char[] array = lastOnLine.ToCharArray();
				array[1] = '1';
				lastOnLine = new string(array);

				hex[hex.Count - 1] = Utilities.GetHexFromBinary(lastOnLine);

				foreach (char character in messageLine2) {
					hex.Add(letters[character]);
				}

				hex.Add("EC");
				string newMessageHex = string.Join(" ", hex);

				messages[3] = newMessageHex;

				List<int> newTableOfContents = new List<int> { 30576 };

				for (int messageIndex = 0; messageIndex < messages.Count; messageIndex++) {
					string message = messages[messageIndex];
					List<string> bytes = message.Split(' ').ToList();

					if (messageIndex < messages.Count - 1) {
						newTableOfContents.Add(newTableOfContents.Last() + bytes.Count);
					}
				}

				string tableOfContentsBytes = "";

				for (int messageIndex = 0; messageIndex < newTableOfContents.Count; messageIndex++) {
					int tableIndex = newTableOfContents[messageIndex];
					string newBytes = Utilities.GetHexFromInt(tableIndex);
					string high = newBytes.Substring(0, 2);
					string low = newBytes.Substring(2, 2);
					int highInt = Utilities.GetIntFromHex(high) + 64;
					high = Utilities.GetHexFromInt(highInt);
					tableOfContentsBytes += low + " " + high;
					if (messageIndex < messages.Count - 1) {
						tableOfContentsBytes += " ";
					}
				}

				ApplyHexAcrossBytes(tableStart, tableOfContentsBytes);

				string allBytes = "";
				for (int messageIndex = 0; messageIndex < messages.Count; messageIndex++) {
					string message = messages[messageIndex];
					allBytes += message;
					if (messageIndex < messages.Count - 1) {
						allBytes += " ";
					}
				}

				ApplyHexAcrossBytes(30576, allBytes);
			}
		}
	}
}