using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.GameData;

namespace ZeldaOverworldRandomizer.RomData {
	public static partial class Rom {
		public static void LoadRomData(byte[] romData) {
			_romHeader = new List<byte>();
			_romData = new List<byte>();

			for (int index = 0; index < romData.Length; index++) {
				if (index < 16) {
					_romHeader.Add(romData[index]);
				} else {
					_romData.Add(romData[index]);
				}
			}

			_isProgZero = _romData[ProgIndicator] == ProgZeroValue;

			ApplyKansInfiniteLayoutPatch();
			ApplyNewGraphicsPatch();
			ApplyMetaTilePatch();
			ApplyWhistleSecretsInBothQuestsPatch();
		}

		public static void SaveRom(string fileName) {
			WriteMetaDataToRom();

			SaveScreenData();
			WriteScreenDataToRom();

			SaveLayoutData();
			WriteLayoutDataToRom();

			if (FrontEnd.MainWindow.Dungeon9Hint) {
				WriteGanonHintToRom();	
			}
			
			SaveRomDataToFile(fileName);
		}

		private static void WriteMetaDataToRom() {
			_romData[LinkStartScreen] = (byte) Game.LinkStartScreen;
			_romData[OverworldItemScreenId] = (byte) Game.OverworldItemScreenId;

			_romData[OverworldItemPositionX] = (byte) (Game.OverworldItemPositionX * 16);
			_romData[OverworldItemPositionY] =
				(byte) ((Game.OverworldItemPositionY + OverworldItemPositionYOffset) * 16);

			while (Game.OverworldLadderScreens.Count < 6) {
				Game.OverworldLadderScreens.Add(0);
			}

			while (Game.OverworldLadderScreens.Count > 6) {
				Game.OverworldLadderScreens.RemoveAt(Game.OverworldLadderScreens.Count - 1);
			}

			for (int index = 0; index < Game.OverworldLadderScreens.Count; index++) {
				_romData[OverworldLadderScreens + index] = (byte) Game.OverworldLadderScreens[index];
			}

			for (int index = 0; index < Game.OverworldWhistleSecretScreens.Count; index++) {
				_romData[WhistleSecretScreens + index] = (byte) Game.OverworldWhistleSecretScreens[index];
			}

			for (int index = 0; index < Game.WarpWhistleDestinations.Count; index++) {
				_romData[WhistleDestinationScreens + index] = (byte) Game.WarpWhistleDestinations[index];
			}

			_romData[LostScreenA] = Game.LostScreenA;
			_romData[LostScreenB] = Game.LostScreenB;


			for (int index = 0; index < Game.ArmosCaveScreens.Count; index++) {
				_romData[ArmosCaveScreens + index] = (byte) Game.ArmosCaveScreens[index];
			}

			for (int index = 0; index < Game.ArmosCaveScreenPositionXs.Count; index++) {
				_romData[ArmosCaveScreenPositionX + index] = (byte) (Game.ArmosCaveScreenPositionXs[index] * 16);
			}

			_romData[ArmosHiddenItemScreen] = (byte) Game.ArmosHiddenItemScreen;
			_romData[ArmosHiddenItemPositionX] = (byte) (Game.ArmosHiddenItemPositionX * 16);
			_romData[ArmosSecretPositionY] = (byte) ((Game.ArmosSecretPositionY + 4) * 16);

			for (int index = 0; index < 4; index++) {
				_romData[AnyRoadScreen + index] = (byte) Game.AnyRoadCaveScreens[index].ScreenId;
			}

			Screen secretScreen = Game.Screens.Where(screen => screen.IsSecretScreen).ToList().First();
			_romData[SecretJingleScreen] = (byte) secretScreen.ScreenId;
			_romData[SecretWallScreen] = (byte) secretScreen.GetScreenDown().ScreenId;

			int totalDocksWritten = 0;

			foreach (Screen screen in Game.Screens) {
				if (screen.IsDock) {
					if (totalDocksWritten == 0) {
						_romData[DockScreenA] = (byte) screen.ScreenId;
					}

					if (totalDocksWritten == 1) {
						_romData[DockScreenB] = (byte) screen.ScreenId;
					}

					totalDocksWritten++;
				}
			}

			switch (FrontEnd.MainWindow.PaletteComboBox.Text) {
				case "Palette: Original":
					break;
				case "Palette: Passion Fruit":
					SetPaletteDataToRom(PaletteA, "0F", "30", "00", "12");
					SetPaletteDataToRom(PaletteC, "0F", "14", "35", "12");
					SetPaletteDataToRom(PaletteD, "0F", "16", "35", "12");
					break;
				case "Palette: Glacial":
					SetPaletteDataToRom(PaletteA, "0F", "30", "00", "12");
					SetPaletteDataToRom(PaletteC, "0F", "21", "31", "11");
					SetPaletteDataToRom(PaletteD, "0F", "2C", "31", "11");
					break;
				case "Palette: Midnight":
					SetPaletteDataToRom(PaletteA, "0F", "3D", "0C", "01");
					SetPaletteDataToRom(PaletteC, "0F", "09", "03", "01");
					SetPaletteDataToRom(PaletteD, "0F", "08", "03", "01");
					break;
				case "Palette: Bubblegum":
					SetPaletteDataToRom(PaletteA, "06", "30", "00", "12");
					SetPaletteDataToRom(PaletteC, "0F", "23", "34", "12");
					SetPaletteDataToRom(PaletteD, "0F", "25", "34", "12");
					break;
				case "Palette: Deep":
					SetPaletteDataToRom(PaletteA, "06", "30", "00", "12");
					SetPaletteDataToRom(PaletteC, "0F", "19", "36", "01");
					SetPaletteDataToRom(PaletteD, "0F", "17", "36", "01");
					break;
				case "Palette: Vintage":
					SetPaletteDataToRom(PaletteA, "0F", "3C", "1C", "0C");
					SetPaletteDataToRom(PaletteC, "0F", "19", "3C", "1C");
					SetPaletteDataToRom(PaletteD, "0C", "18", "3C", "1C");
					break;
				case "Palette: Noir":
					SetPaletteDataToRom(PaletteA, "2D", "06", "2D", "06");
					SetPaletteDataToRom(PaletteC, "0C", "30", "0F", "2D");
					SetPaletteDataToRom(PaletteD, "0F", "20", "0F", "2D");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void SetPaletteDataToRom(int romOffset, string aHex, string bHex, string cHex, string dHex) {
			_romData[romOffset + 0] = (byte) Utilities.GetIntFromHex(aHex);
			_romData[romOffset + 1] = (byte) Utilities.GetIntFromHex(bHex);
			_romData[romOffset + 2] = (byte) Utilities.GetIntFromHex(cHex);
			_romData[romOffset + 3] = (byte) Utilities.GetIntFromHex(dHex);
			_romData[Utilities.GetIntFromHex("3FEE4")] = (byte) Utilities.GetIntFromHex(bHex);
			_romData[Utilities.GetIntFromHex("3FEE5")] = (byte) Utilities.GetIntFromHex(cHex);
			_romData[Utilities.GetIntFromHex("3FEF2")] = (byte) Utilities.GetIntFromHex(cHex);
		}

		private static void SaveRomDataToFile(string fileName) {
			if (FrontEnd.MainWindow.DistinctSecretTiles) {
				ApplyIndicatedBombBurnTiles();
			} else {
				ApplyNonIndicatedBombBurnTiles();
			}

			File.WriteAllBytes(fileName, _romHeader.Concat(_romData).ToArray());
		}

		private static void ApplyKansInfiniteLayoutPatch() {
			_romHeader[4] = 16;
			_romData[Utilities.GetIntFromHex("168BF")] = (byte) Utilities.GetIntFromHex("F9");
			_romData[Utilities.GetIntFromHex("168C0")] = (byte) Utilities.GetIntFromHex("6B");

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("1937B"), "A9 08 20 AC FF 20 F7 A9 A9 05 20 AC FF 60");

			const int additionalBanks = 8;
			int bankLength = Utilities.GetIntFromHex("4000");
			int startOfNewBank = bankLength * 7;

			for (int additionalBank = 0; additionalBank < additionalBanks; additionalBank++) {
				for (int newByte = 0; newByte < bankLength; newByte++) {
					_romData.Insert(startOfNewBank, (byte) Utilities.GetIntFromHex("FF"));
				}
			}

			const string endOfBankAsm = "A9 01 85 01 A9 03 85 03 A4 EB B9 7E 68 A4 03 C0 " +
			                            "02 90 05 A4 EB B9 FE 68 85 00 A5 03 29 01 D0 06 " +
			                            "46 00 46 00 46 00 46 00 46 00 A5 02 24 01 D0 09 " +
			                            "06 01 C6 03 10 D2 A9 08 60 A5 00 29 07 60 A9 13 " +
			                            "85 06 A2 19 D0 48 20 98 72 4C 8E A4 A9 A0 85 00 " +
			                            "A9 9F 85 01 A9 47 85 02 A9 65 85 03 A9 5A 85 04 " +
			                            "A9 65 85 05 A9 0A 85 06 A0 00 B1 00 F0 D0 91 02 " +
			                            "91 04 C9 DE F0 08 C9 E2 B0 04 69 01 91 04 A9 01 " +
			                            "A2 01 C6 06 D0 08 A9 0A 85 06 A9 0D A2 1F 20 82 " +
			                            "72 8A CA F0 B1 20 8E 72 20 74 72 C9 EE D0 CB A9 " +
			                            "30 85 02 A9 65 85 03 A9 EF 85 04 A9 67 85 05 B1 " +
			                            "02 91 04 C9 DD F0 22 C9 E0 B0 0D C9 DC B0 04 69 " +
			                            "01 91 04 18 69 01 91 04 20 98 72 20 80 72 C9 90 " +
			                            "D0 DD A5 03 C9 66 D0 D7 60 A9 DC D0 E9 01 02 04 " +
			                            "08 EE 2A 66 A2 9F A0 A0 A0 A1 4F 76 65 67 65 66 " +
			                            "66 14 01 01 02 02 2C 2C 03 03 02 02 01 01 02 02 " +
			                            "A2 03 A9 01 85 06 8A 48 85 0B BD D3 A4 85 02 20 " +
			                            "F6 A3 C9 05 B0 1C C9 04 D0 08 A9 08 D0 14 A9 09 " +
			                            "D0 2F 48 A5 02 49 FF 25 EE 85 EE 68 C9 01 B0 02 " +
			                            "A9 04 48 A5 EE 25 02 AA 68 E4 02 D0 14 A8 68 48 " +
			                            "AA 98 C9 07 F0 09 48 20 97 8A 68 C9 08 F0 CF A9 " +
			                            "04 A6 06 F0 0A A6 0B 48 20 F6 A3 20 41 B6 68 C9 " +
			                            "04 90 4C 38 E9 03 A8 C0 03 90 01 88 68 48 20 B4 " +
			                            "A5 A5 06 D0 0B BD EA A4 20 76 72 A9 06 20 82 72 " +
			                            "A0 00 BD EE A4 85 05 68 48 AA BD F2 A4 AA B1 02 " +
			                            "91 00 20 80 72 BD E7 A4 20 76 72 E0 00 D0 09 68 " +
			                            "48 C9 02 B0 03 20 74 72 CA 10 E3 C6 05 D0 D8 68 " +
			                            "AA C6 06 30 03 4C FC A4 CA 30 23 4C F8 A4 AA BD " +
			                            "D7 A4 85 02 BD DB A4 85 03 BD DF A4 85 00 BD E3 " +
			                            "A4 85 01 88 F0 08 A9 0C 20 82 72 4C C9 A5 60 01 " +
			                            "03 06 08 03 05 08 0A 03 06 04 07 05 08 00 04 08 " +
			                            "0A 22 22 23 21 5C 42 4F 4F 01 FF 10 F0 A5 12 C9 " +
			                            "12 F0 DB A5 27 D0 D7 A5 54 F0 D3 29 07 A0 01 84 " +
			                            "02 24 02 F0 01 4A C9 02 D0 04 A0 30 84 28 29 03 " +
			                            "38 E9 01 29 02 85 08 A5 54 C9 05 B0 0E A5 55 85 " +
			                            "02 20 F6 A3 C9 07 F0 03 4C 82 A6 20 B1 A6 A5 06 " +
			                            "85 04 A5 00 9D 02 03 E8 A5 01 9D 02 03 E8 A5 06 " +
			                            "9D 02 03 E8 20 00 94 D0 FB A9 20 05 01 85 01 C6 " +
			                            "05 D0 DB A9 FF 9D 02 03 8A 8D 01 03 E6 54 A5 54 " +
			                            "29 03 F0 05 A9 08 85 27 60 A5 54 C9 04 D0 14 A6 " +
			                            "09 20 A0 8A A5 55 49 0F 25 EE 85 EE A9 00 85 54 " +
			                            "4C F6 A4 A5 55 85 02 20 F6 A3 C9 07 F0 16 A6 09 " +
			                            "20 97 8A 98 18 7D EF A5 A8 8A 49 01 AA B1 00 1D " +
			                            "BE E6 91 00 A5 55 05 EE 4C 80 A6 A5 55 85 02 20 " +
			                            "F6 A3 C9 05 90 07 48 A9 04 20 7C 6D 68 C9 04 D0 " +
			                            "02 A9 08 C9 01 D0 02 A9 04 38 E9 03 A8 A5 08 F0 " +
			                            "06 C0 05 F0 07 A0 01 C0 03 90 01 88 A9 03 38 E5 " +
			                            "03 20 B4 A5 BD E7 A5 85 00 BD EB A5 85 01 86 09 " +
			                            "AE 01 03 A9 00 85 07 A9 02 85 06 85 05 60 D6 A2 " +
			                            "E6 A2 04 A3 15 A3 26 A3 3D A3 54 A3 65 A3 77 A3 " +
			                            "91 A3 B0 74 94 B4 70 68 F4 24 20 5A E8 48 A9 DE " +
			                            "85 02 A9 A0 85 03 68 0A 0A 85 00 20 82 72 A5 00 " +
			                            "20 82 72 A5 00 20 82 72 A9 8C 85 00 A9 65 85 01 " +
			                            "A0 00 84 06 A4 06 B1 02 29 F0 4A 4A 4A AA BD 04 " +
			                            "A7 85 04 BD 05 A7 85 05 B1 02 29 0F AA A0 00 B1 " +
			                            "04 10 03 CA 30 04 C8 4C 65 A7 98 20 8E 72 A9 00 " +
			                            "85 07 85 08 A0 00 B1 04 29 07 AA BD 18 A7 A0 00 " +
			                            "20 C1 A7 A9 02 20 76 72 A0 00 B1 04 29 70 4A 4A " +
			                            "4A 4A C5 08 F0 05 E6 08 4C A8 A7 A9 00 85 08 20 " +
			                            "8C 72 E6 07 A5 07 C9 07 90 CA A9 1E 20 76 72 E6 " +
			                            "06 A5 06 C9 0C B0 03 4C 4A A7 60 C9 70 90 1B C9 " +
			                            "F3 B0 17 AA 91 00 C8 E8 8A 91 00 98 18 69 15 A8 " +
			                            "E8 8A 91 00 E8 8A C8 91 00 60 91 00 C8 91 00 48 " +
			                            "98 18 69 15 A8 68 91 00 4C DC A7 A9 00 85 B7 85 " +
			                            "A3 20 5A E8 C9 21 D0 0A A9 40 85 7B 0A 85 8F 4C " +
			                            "2D A8 A2 08 A0 0A BD 00 E4 85 00 BD 01 E4 85 01 " +
			                            "B1 00 C9 B0 F0 08 E8 E8 E8 E8 E0 34 D0 E8 8A 0A " +
			                            "0A 85 7B A9 90 85 8F A9 68 8D 5A 03 60 A9 04 8D " +
			                            "00 06 A9 20 85 7C A9 01 85 7D A9 30 85 28 A9 24 " +
			                            "85 0A 20 D8 E8 E6 11 20 1D 6E A9 1B 8D 05 05 4C " +
			                            "0C E8 20 1D 6E 20 17 E8 A5 13 20 E2 E5 6D A8 77 " +
			                            "A8 8F A8 97 A8 AB A8 A5 28 D0 16 A9 30 85 28 D0 " +
			                            "15 A0 18 A5 28 F0 0B 29 07 C9 04 90 02 A0 78 84 " +
			                            "14 60 A9 02 85 63 E6 13 60 20 89 ED A5 63 F0 0E " +
			                            "60 A5 28 D0 0F 20 48 72 A5 7C C9 11 B0 06 A9 80 " +
			                            "85 28 E6 13 60 A5 28 D0 FB 20 F7 E5 A5 FF 29 FB " +
			                            "85 FF 8D 00 20 4C 47 B5 20 F4 A9 4C 73 AB 20 30 " +
			                            "AB A5 10 F0 F3 A9 F6 85 0A 20 D8 E8 20 6C B6 20 " +
			                            "42 A4 20 F6 A4 4C 20 A7 A9 1A 85 00 A9 65 85 01 " +
			                            "A6 E8 CA 8A AC 01 03 99 03 03 A9 21 99 02 03 A9 " +
			                            "16 20 76 72 CA 10 F8 A9 96 99 04 03 8A 99 1B 03 " +
			                            "98 AA A0 00 84 06 B1 00 9D 05 03 20 74 72 E8 E6 " +
			                            "06 A5 06 C9 16 90 EF E8 E8 E8 8E 01 03 60 A9 65 " +
			                            "85 01 A5 E9 AA 18 69 30 85 00 90 02 E6 01 A9 20 " +
			                            "8D 02 03 A9 E0 8D 03 03 AD 03 03 18 69 20 8D 03 " +
			                            "03 90 03 EE 02 03 CA 10 EF A9 20 8D 04 03 8E 25 " +
			                            "03 A2 00 A0 00 B1 00 9D 05 03 A9 16 20 76 72 E8 " +
			                            "E0 20 90 F1 A9 23 8D 01 03 60 62 63 64 65 66 67 " +
			                            "C8 D8 C4 BC C0 C0 24 6F F3 FA 98 90 8F 95 8E 90 " +
			                            "74 76 F3 24 26 89 03 04 70 C8 BC 8D 8F 93 95 C4 " +
			                            "CE D8 B0 B4 AA AC B8 9C A6 9A A2 A0 E5 E6 E7 E8 " +
			                            "E9 EA C0 E0 78 7A 7E 80 CC D0 D4 DC 89 84 24 24 " +
			                            "24 24 6F 6F 6F 6F F3 F3 F3 F3 FA FA FA FA 98 95 " +
			                            "26 26 90 95 90 95 8F 90 8F 90 95 96 95 96 8E 93 " +
			                            "90 95 90 95 92 97 74 74 75 75 76 77 76 77 F3 24 " +
			                            "F3 24 24 24 24 24 26 26 26 26 89 88 8B 88 FF FF " +
			                            "FF A6 EB BD FE 69 29 7F 48 4A 4A 4A 4A 4A 18 69 " +
			                            "08 20 AC FF AD AF 6B 85 08 AD B0 6B 85 09 20 07 " +
			                            "AC 68 29 1F 18 69 80 85 05 A9 00 85 03 85 0C 85 " +
			                            "06 A5 06 A8 0A 0A 0A 0A 85 04 4C 55 AA FF FF FF " +
			                            "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF " +
			                            "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF A9 " +
			                            "00 85 07 A0 00 B1 04 29 3F 85 0D AA BD 7C A9 48 " +
			                            "A4 EB B1 08 29 80 F0 1A 68 C9 E7 F0 08 C9 E6 F0 " +
			                            "0C C9 EA D0 0C A9 10 85 0D A9 70 D0 04 A9 0C 85 " +
			                            "0D 48 68 20 BF AA A0 00 20 F1 AA A9 02 20 76 72 " +
			                            "A0 00 B1 04 29 40 F0 06 45 0C 85 0C D0 03 20 8C " +
			                            "72 E6 07 A5 07 C9 0B D0 AA A9 16 20 76 72 E6 06 " +
			                            "A5 06 C9 10 B0 34 4C 27 AA A2 EA 86 0A A2 05 C5 " +
			                            "0A F0 07 C6 0A CA 10 F7 30 20 BD 76 A9 48 BD 70 " +
			                            "A9 8D 2B 05 A5 06 0A 0A 0A 0A 8D 2C 05 A5 07 0A " +
			                            "0A 0A 0A 18 69 40 8D 2D 05 68 60 A6 0D E0 10 90 " +
			                            "17 AA 91 00 C8 E8 8A 91 00 98 18 69 15 A8 E8 8A " +
			                            "91 00 E8 8A C8 91 00 60 8A 0A 0A AA BD B4 A9 91 " +
			                            "00 C8 E8 BD B4 A9 91 00 98 18 69 15 A8 E8 BD B4 " +
			                            "A9 91 00 E8 BD B4 A9 4C 0A AB AD 9E 9F AE 9F 9F " +
			                            "A4 10 F0 04 A9 D4 A2 A3 8D 27 68 8E 28 68 60 A8 " +
			                            "9B B8 9B B4 A3 C4 A3 A2 00 BD 45 AB 85 02 BD 46 " +
			                            "AB 85 03 E6 13 4C 1B AA A2 02 D0 ED A9 00 85 E9 " +
			                            "A2 04 20 5A E8 29 01 F0 E0 A2 06 D0 DC 20 CE E6 " +
			                            "0A B0 42 B1 00 29 20 F0 3C 20 07 AC 20 8A 71 4A " +
			                            "4A AA BD 00 E4 85 00 BD 01 E4 85 01 98 38 E9 40 " +
			                            "4A 4A 4A A8 B1 00 C9 C4 F0 1C C9 BC F0 17 C9 D8 " +
			                            "D0 14 AD 2B 05 C9 62 F0 0D A9 00 8D 2B 05 A9 0C " +
			                            "85 0D 20 F1 AA 60 A9 10 85 0D A9 70 D0 F4 8A 48 " +
			                            "B5 70 29 F0 4A 4A AA BD 00 E4 85 00 BD 01 E4 85 " +
			                            "01 68 48 AA B5 84 29 F0 38 E9 40 4A 4A 4A 20 76 " +
			                            "72 A0 00 A2 10 A5 05 C9 27 90 04 C9 F3 90 0A A2 " +
			                            "0E DD 7C A9 F0 03 CA D0 F8 86 0D 20 F1 AA 68 AA " +
			                            "60 A9 30 85 00 A9 65 85 01 60 20 16 AC B0 12 60 " +
			                            "20 24 A9 E6 E9 A5 E9 C9 16 60 20 C4 A8 A9 00 85 " +
			                            "E9 E6 13 60";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("223F6"), endOfBankAsm);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("263F6"), endOfBankAsm);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2A3F6"), endOfBankAsm);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2E3F6"), endOfBankAsm);

			const string endOfBankAsm2 = "78 D8 A9 00 8D 00 20 A2 FF 9A AD 02 20 29 80 F0 " +
			                             "F9 AD 02 20 29 80 F0 F9 09 FF 8D 00 80 8D 00 A0 " +
			                             "8D 00 C0 8D 00 E0 A9 0F 20 98 BF A9 00 8D 00 A0 " +
			                             "4A 8D 00 A0 4A 8D 00 A0 4A 8D 00 A0 4A 8D 00 A0 " +
			                             "A9 07 20 AC BF 4C 40 E4 8D 00 80 4A 8D 00 80 4A " +
			                             "8D 00 80 4A 8D 00 80 4A 8D 00 80 60 8D 00 E0 4A " +
			                             "8D 00 E0 4A 8D 00 E0 4A 8D 00 E0 4A 8D 00 E0 60";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("23F50"), endOfBankAsm2);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("27F50"), endOfBankAsm2);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2BF50"), endOfBankAsm2);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2FF50"), endOfBankAsm2);

			const string endOfBankAsm3 = "84 E4 50 BF F0 BF";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("23FFA"), endOfBankAsm3);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("27FFA"), endOfBankAsm3);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2BFFA"), endOfBankAsm3);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2FFFA"), endOfBankAsm3);
		}

		private static void ApplyNewGraphicsPatch() {
			const int indexOfGraphicsChange = 35456;
			const string graphicBytes = "00 20 02 00 40 00 08 FF FF DF FD FF BF FF F7 00 " +
			                            "01 20 00 00 82 00 10 FF FE DF FF FF 7D FF EF 01 " +
			                            "08 00 00 40 04 00 00 FE F7 FF FF BF FB FF FF 10 " +
			                            "00 00 02 10 00 00 40 EF FF FF FD EF FF FF BF FF " +
			                            "80 80 90 90 91 91 91 00 00 60 60 6E 6E 6E 6E 91 " +
			                            "91 91 91 91 91 91 FF 6E 6E 6E 6E 6E 6E 6E 00 FF " +
			                            "01 01 01 01 01 01 11 00 00 00 00 00 00 E0 E0 11 " +
			                            "11 11 11 11 11 11 FF EE EE EE EE EE EE EE 00 00 " +
			                            "07 17 37 77 77 77 77 F0 E8 C8 88 08 08 08 08 77 " +
			                            "77 77 77 77 77 77 77 08 08 08 08 08 08 08 08 00 " +
			                            "FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 FF " +
			                            "FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 " +
			                            "E0 E8 EC EE EE EE EE 0F 17 13 11 10 10 10 10 EE " +
			                            "EE EE EE EE EE EE EE 10 10 10 10 10 10 10 10 77 " +
			                            "77 77 77 77 77 77 77 08 08 08 08 08 08 08 08 77 " +
			                            "70 6F 6F 5F 5F 3F 3F 08 0F 10 10 20 20 40 40 FF " +
			                            "FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 FF " +
			                            "00 FF FF FF FF FF FF 00 FF 00 00 00 00 00 00 EE " +
			                            "EE EE EE EE EE EE EE 10 10 10 10 10 10 10 10 EE " +
			                            "0E F6 F6 FA FA FC FC 10 F0 08 08 04 04 02 02 00 " +
			                            "00 0B 28 64 54 50 50 00 00 00 03 0B 2B 2F 2F 40 " +
			                            "50 40 50 54 54 54 00 3F 2F 3F 2F 2B 2B 2B 00 00 " +
			                            "00 68 94 92 92 82 02 00 00 00 68 6C 6C 7C FC 02 " +
			                            "02 02 26 1A 82 92 00 E4 C8 C0 C0 E4 7C 6C 00 00 " +
			                            "00 00 00 18 18 00 00 FF FF E7 DB A5 A5 99 81 24 " +
			                            "3C 3C 3C 18 00 00 00 81 81 81 81 81 81 C3 FF 00 " +
			                            "00 00 00 18 18 00 00 FF FF E7 DB A5 A5 99 81 24 " +
			                            "3C 3C 3C 18 00 00 00 81 81 81 81 81 81 C3 FF 00 " +
			                            "00 7F 70 60 00 60 68 00 00 00 0F 1F 00 00 00 00 " +
			                            "00 00 00 1F 00 00 00 68 68 6F 60 60 7F 7F 00 00 " +
			                            "00 FE 0E 06 00 06 16 00 00 00 F0 F8 00 00 00 00 " +
			                            "00 00 00 F8 00 00 00 16 16 F6 06 06 FE FE 00 FF " +
			                            "5F 7F 7F 7F 5F 7F 00 80 00 00 00 00 00 00 00 FF " +
			                            "5F 7F 7F 7F 5F 7F 00 80 00 00 00 00 00 00 00 FF " +
			                            "FD FF FF FF FD FF 01 00 00 00 00 00 00 00 01 FF " +
			                            "FD FF FF FF FD FF 01 00 00 00 00 00 00 00 01 00 " +
			                            "43 0C 17 2E 5D 5D 7F FF BC F0 E0 C0 80 80 80 7B " +
			                            "7B 31 10 00 40 00 10 80 80 C8 EC FD BC FD E8 01 " +
			                            "80 D2 20 C8 D0 D8 9C FE 7F 25 03 01 00 00 00 8C " +
			                            "04 00 40 88 42 80 00 00 10 19 1B 17 1D 03 07 FF " +
			                            "FF FE 7E 64 C9 C9 F9 00 00 00 80 81 12 12 02 19 " +
			                            "13 98 3C 3C 38 30 30 22 A4 22 41 41 41 45 45 C3 " +
			                            "F0 72 7A FA FE F8 9C 08 01 80 80 00 00 01 01 1E " +
			                            "3E 7C 9C CC CC C8 C0 60 40 80 21 11 11 10 19 00 " +
			                            "0A 2F 35 5C 58 C8 85 FF F4 D0 C8 80 82 00 20 44 " +
			                            "C1 85 87 4E 0A 00 00 80 08 00 20 80 E5 F7 E0 00 " +
			                            "00 80 C8 E0 D0 40 20 FF 3F 07 03 03 01 03 01 24 " +
			                            "40 00 50 00 20 00 00 81 01 03 03 07 1F C0 03 00";

			ApplyHexAcrossBytes(indexOfGraphicsChange, graphicBytes);

			if (!_isProgZero) {
				ApplyHexAcrossBytes(indexOfGraphicsChange - 1, "00");
			}
		}

		private static void ApplyMetaTilePatch() {
			string metaTileBytes = "4C 48 70 C8 BC 8D 8F 93 95 C4 CE D8 B0 B4 AA AC " +
			                       "B8 9C A6 9A A2 A0 E5 E6 E7 E8 38 3A C0 E0 78 7A " +
			                       "7E 80 CC D0 D4 DC 89 30 36 3E 40 40 44 50 54 6F";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2298C"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2698C"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2A98C"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2E98C"), metaTileBytes);

			metaTileBytes = "3A 3B 3A 3B 40 41 40 41";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("229C0"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("269C0"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2A9C0"), metaTileBytes);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2E9C0"), metaTileBytes);

			const string noCollisionTablePointer = "C0 FF";
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("3EE8A"), noCollisionTablePointer);

			const string noCollisionLength = "21";
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("3EE85"), noCollisionLength);

			const string noCollisionTiles = "8D 91 9C AC AD CC D2 D5 DF 50 51 52 53 74 75 76 " +
			                                "77 78 79 7A 7B 7C 7D 7E 7F 80 81 82 83 70 71 72 " +
			                                "73";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("3FFC0"), noCollisionTiles);

			const string collisionStartIndex = "38";
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("17058"), collisionStartIndex);

			const string tileThatActsLikeStairs = "34";
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("1747A"), tileThatActsLikeStairs);

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("22A80"), tileThatActsLikeStairs);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("26A80"), tileThatActsLikeStairs);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2AA80"), tileThatActsLikeStairs);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2EA80"), tileThatActsLikeStairs);

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("10F33"), tileThatActsLikeStairs);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("3E896"), tileThatActsLikeStairs);

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("10ED1"), tileThatActsLikeStairs);

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("16BC1"), tileThatActsLikeStairs);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("10CCF"), tileThatActsLikeStairs);
		}

		private static void ApplyWhistleSecretsInBothQuestsPatch() {
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("3EFA2"), "D0");
		}

		private static void ApplyIndicatedBombBurnTiles() {
			const string updateSecretTiles = "C8 58 5C BC C0 C0";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("22976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("26976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2A976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2E976"), updateSecretTiles);
		}

		private static void ApplyNonIndicatedBombBurnTiles() {
			const string updateSecretTiles = "C8 D8 C4 BC C0 C0";

			ApplyHexAcrossBytes(Utilities.GetIntFromHex("22976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("26976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2A976"), updateSecretTiles);
			ApplyHexAcrossBytes(Utilities.GetIntFromHex("2E976"), updateSecretTiles);
		}

		private static void ApplyHexAcrossBytes(int start, string bytes) {
			string[] individualBytes = bytes.Split(' ');
			for (int byteToWrite = 0; byteToWrite < individualBytes.Length; byteToWrite++) {
				_romData[start + byteToWrite] = (byte) Utilities.GetIntFromHex(individualBytes[byteToWrite]);
			}
		}
	}
}