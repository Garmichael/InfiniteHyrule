using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ZeldaOverworldRandomizer.Common;

namespace ZeldaOverworldRandomizer {
	public partial class MainWindow : INotifyPropertyChanged {
		private string VersionNumber = "2.20";

		public string WindowTitle => "Infinite Hyrule - Zelda Overworld Randomizer       v" + VersionNumber;
		private bool _generateWithMapPreview;
		private bool _generateWithMapSpoilers;
		private int _seed = 99999999;
		private bool _distinctSecretTiles = true;
		private bool _hideNormalDungeons = false;
		private bool _hideDungeon4OnIsland = true;
		private bool _hideDungeon7UnderLake = true;
		private bool _hideDungeon8 = false;
		private bool _hideDungeon9 = true;
		private bool _dungeon9Hint = true;

		private bool _enabledDungeon9HintBox = true;
		private bool _enableDungeon9Box = true;

		private bool _includeKakariko = true;
		private bool _includeGhostForest = true;
		private bool _includeRiver = true;
		private bool _generateEnhancedDeserts = true;
		private bool _generateEnhancedBeaches = true;

		private string _flagset = "00";

		private readonly List<string> _presetOptions = new List<string> {
			"Infinite Hyrule Standard",
			"Vanilla",
			"Vanilla Enhanced",
			"Scavenger Hunt",
			"Scavenger Hunt Extreme"
		};

		private string _selectedPreset = "Infinite Hyrule Standard";

		public string FlagSet {
			get { return _flagset; }
			set {
				_flagset = DigestFlagSet(value);
				OnPropertyChanged();
			}
		}

		private string DigestFlagSet(string flagSet) {
			bool isValidHex = Utilities.ValidateStringIsHex(flagSet);
			if (!isValidHex) {
				flagSet = "00";
			}

			string bits = Utilities.GetBinaryFromHex(flagSet);
			while (bits.Length < 14) {
				bits = "0" + bits;
			}

			GenerateWithMapSpoilers = bits.Substring(13, 1) == "1";
			HideDungeon9 = bits.Substring(4, 1) == "1";
			Dungeon9Hint = bits.Substring(5, 1) == "1";

			HideNormalDungeons = bits.Substring(0, 1) == "1";
			HideDungeon4 = bits.Substring(1, 1) == "1";
			HideDungeon7 = bits.Substring(2, 1) == "1";
			HideDungeon8 = bits.Substring(3, 1) == "1";
			IncludeKakariko = bits.Substring(6, 1) == "1";
			IncludeGhostForest = bits.Substring(7, 1) == "1";
			IncludeRiver = bits.Substring(8, 1) == "1";
			GenerateEnhancedBeaches = bits.Substring(9, 1) == "1";
			GenerateEnhancedDeserts = bits.Substring(10, 1) == "1";
			DistinctSecretTiles = bits.Substring(11, 1) == "1";
			GenerateWithMapPreview = bits.Substring(12, 1) == "1";

			return Utilities.GetHexFromBinary(bits);
		}

		private void UpdateFlagSet() {
			string flagSet = "";

			flagSet += HideNormalDungeons ? "1" : "0";
			flagSet += HideDungeon4 ? "1" : "0";
			flagSet += HideDungeon7 ? "1" : "0";
			flagSet += HideDungeon8 ? "1" : "0";
			flagSet += HideDungeon9 ? "1" : "0";
			flagSet += Dungeon9Hint ? "1" : "0";
			flagSet += IncludeKakariko ? "1" : "0";
			flagSet += IncludeGhostForest ? "1" : "0";
			flagSet += IncludeRiver ? "1" : "0";
			flagSet += GenerateEnhancedBeaches ? "1" : "0";
			flagSet += GenerateEnhancedDeserts ? "1" : "0";
			flagSet += DistinctSecretTiles ? "1" : "0";
			flagSet += GenerateWithMapPreview ? "1" : "0";
			flagSet += GenerateWithMapSpoilers ? "1" : "0";

			flagSet = Utilities.GetHexFromBinary(flagSet);

			FlagSet = flagSet;
		}

		public bool GenerateWithMapPreview {
			get { return _generateWithMapPreview; }
			set {
				if (_generateWithMapPreview != value) {
					_generateWithMapPreview = value;

					if (!_generateWithMapPreview) {
						GenerateWithMapSpoilers = false;
					}

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool GenerateWithMapSpoilers {
			get { return _generateWithMapSpoilers; }
			set {
				if (_generateWithMapSpoilers != value) {
					_generateWithMapSpoilers = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public int Seed {
			get { return _seed; }
			set {
				if (_seed != value) {
					_seed = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool DistinctSecretTiles {
			get { return _distinctSecretTiles; }
			set {
				if (_distinctSecretTiles != value) {
					_distinctSecretTiles = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool HideNormalDungeons {
			get { return _hideNormalDungeons; }
			set {
				if (_hideNormalDungeons != value) {
					_hideNormalDungeons = value;

					if (_hideNormalDungeons) {
						HideDungeon9 = true;
						Dungeon9Hint = false;
						EnabledDungeon9HintBox = false;
						EnableDungeon9Box = false;
					} else {
						EnableDungeon9Box = true;
					}

					if (HideDungeon9 && !HideNormalDungeons) {
						EnabledDungeon9HintBox = true;
					}

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool EnableDungeon9Box {
			get { return _enableDungeon9Box; }
			set {
				if (_enableDungeon9Box != value) {
					_enableDungeon9Box = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool HideDungeon4 {
			get { return _hideDungeon4OnIsland; }
			set {
				if (_hideDungeon4OnIsland != value) {
					_hideDungeon4OnIsland = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool HideDungeon7 {
			get { return _hideDungeon7UnderLake; }
			set {
				if (_hideDungeon7UnderLake != value) {
					_hideDungeon7UnderLake = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool HideDungeon8 {
			get { return _hideDungeon8; }
			set {
				if (_hideDungeon8 != value) {
					_hideDungeon8 = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool HideDungeon9 {
			get { return _hideDungeon9; }
			set {
				if (_hideDungeon9 != value) {
					_hideDungeon9 = value;

					if (!_hideDungeon9) {
						Dungeon9Hint = false;
						EnabledDungeon9HintBox = false;
					}

					if (HideDungeon9 && !HideNormalDungeons) {
						EnabledDungeon9HintBox = true;
					}

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool Dungeon9Hint {
			get { return _dungeon9Hint; }
			set {
				if (_dungeon9Hint != value) {
					_dungeon9Hint = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool EnabledDungeon9HintBox {
			get { return _enabledDungeon9HintBox; }
			set {
				if (_enabledDungeon9HintBox != value) {
					_enabledDungeon9HintBox = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool IncludeKakariko {
			get { return _includeKakariko; }
			set {
				if (_includeKakariko != value) {
					_includeKakariko = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool IncludeGhostForest {
			get { return _includeGhostForest; }
			set {
				if (_includeGhostForest != value) {
					_includeGhostForest = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool IncludeRiver {
			get { return _includeRiver; }
			set {
				if (_includeRiver != value) {
					_includeRiver = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool GenerateEnhancedDeserts {
			get { return _generateEnhancedDeserts; }
			set {
				if (_generateEnhancedDeserts != value) {
					_generateEnhancedDeserts = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public bool GenerateEnhancedBeaches {
			get { return _generateEnhancedBeaches; }
			set {
				if (_generateEnhancedBeaches != value) {
					_generateEnhancedBeaches = value;

					UpdateFlagSet();
					OnPropertyChanged();
				}
			}
		}

		public List<string> PresetOptions {
			get { return _presetOptions; }
			set { }
		}

		public string SelectedPreset {
			get { return _selectedPreset; }
			set { _selectedPreset = value; }
		}

		public void SetPreset(object sender, RoutedEventArgs e) {
			if (_selectedPreset == PresetOptions[0]) {
				FlagSet = "1BFC";
			} else if (_selectedPreset == PresetOptions[1]) {
				FlagSet = "1B00";
			} else if (_selectedPreset == PresetOptions[2]) {
				FlagSet = "1B1C";
			} else if (_selectedPreset == PresetOptions[3]) {
				FlagSet = "22FC";
			} else if (_selectedPreset == PresetOptions[4]) {
				FlagSet = "22F8";
			}
		}
	}
}