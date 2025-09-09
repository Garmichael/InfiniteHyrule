using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ZeldaOverworldRandomizer.Common;
using ZeldaOverworldRandomizer.MapBuilder;
using ZeldaOverworldRandomizer.RomData;

namespace ZeldaOverworldRandomizer {
	public static class FrontEnd {
		public static MainWindow MainWindow;
	}

	public sealed partial class MainWindow : INotifyPropertyChanged {
		private bool _showSpoilerShield = true;

		public MainWindow() {
			FrontEnd.MainWindow = this;
			DataContext = this;
			InitializeComponent();
		}

		private void LoadRom(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();

			if (openFileDialog.ShowDialog() == true) {
				byte[] content = File.ReadAllBytes(openFileDialog.FileName);
				Rom.FileName = openFileDialog.FileName;

				Rom.LoadRomData(content);

				LoadRomForm.Visibility = Visibility.Hidden;
				RandomizerForm.Visibility = Visibility.Visible;
			}
		}

		private void Save(object sender, RoutedEventArgs e) {
			List<string> directoryParts = Rom.FileName.Split('\\').ToList();
			directoryParts.RemoveAt(directoryParts.Count - 1);
			string directory = string.Join("\\", directoryParts);
			string fileName = "InfiniteHyrule_" + _seed + "_" + FrontEnd.MainWindow.FlagSet;
			
			Rom.SaveRom(directory + "\\" + fileName + ".nes");

			if (GenerateWithMapPreview) {
				ExportMapImage(directory + "\\" + fileName);
			}

			MessageBox.Show("Rom Saved as " + fileName + ".nes");
		}

		private void ExportMapImage(string fileName) {
			Rect rect = new Rect(FullMapCanvas.RenderSize);

			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
				(int) rect.Right, (int) rect.Bottom, 96d, 96d,
				System.Windows.Media.PixelFormats.Default
			);

			bool wasHidden = FullMapCanvas.Visibility == Visibility.Hidden;
			FullMapCanvas.Visibility = Visibility.Visible;

			renderTargetBitmap.Render(FullMapCanvas);
			BitmapEncoder pngEncoder = new PngBitmapEncoder();
			pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

			MemoryStream memoryStream = new MemoryStream();
			pngEncoder.Save(memoryStream);
			memoryStream.Close();

			if (wasHidden) {
				FullMapCanvas.Visibility = Visibility.Hidden;
			}

			File.WriteAllBytes(fileName + "_spoiler.png", memoryStream.ToArray());
		}

		private void SaveAs(object sender, RoutedEventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			if (saveFileDialog.ShowDialog() == true) {
				Rom.FileName = saveFileDialog.FileName;
				Rom.SaveRom(saveFileDialog.FileName);
			}
		}

		private void RandomizeSeed(object sender, RoutedEventArgs e) {
			Utilities.SetSeed(Utilities.GenerateRandomSeed());
		}

		private async void BuildMap(object sender, RoutedEventArgs e) {
			// SetSeed();
			Utilities.SetSeed(Seed);
			
			SetUpUiElementsPreGeneration();

			await Task.Delay(100);

			OverworldBuilder.BuildOverworld();
			SetUpUiElementsPostGeneration();
		}

		private void SetUpUiElementsPreGeneration() {
			CompleteForm.IsEnabled = false;
			GeneratingMapMessage.Visibility = Visibility.Visible;
			GeneratedMapMessage.Visibility = Visibility.Hidden;
			FullMapCanvas.Children.Clear();
		}

		private void SetUpUiElementsPostGeneration() {
			CompleteForm.IsEnabled = true;
			GeneratingMapMessage.Visibility = Visibility.Hidden;
			GeneratedMapMessage.Visibility = Visibility.Visible;

			if (GenerateWithMapPreview) {
				MapPreviewBuilder.DrawFullMap();
			}

			if (!GenerateWithMapPreview) {
				MapPreviewSpoilerShield.Visibility = Visibility.Hidden;
				MapPreview.Visibility = Visibility.Hidden;
			} else if (_showSpoilerShield) {
				MapPreviewSpoilerShield.Visibility = Visibility.Visible;
				MapPreview.Visibility = Visibility.Hidden;
			} else {
				MapPreviewSpoilerShield.Visibility = Visibility.Hidden;
				MapPreview.Visibility = Visibility.Visible;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChangedEventHandler handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void PopUpZ1RHelp(object sender, RoutedEventArgs e) {
			const string boxMessage =
				"If you want to randomize Dungeons and Items, you can use Fred Coughlin's Zelda Randomizer (Z1R). " +
				"\nhttps://sites.google.com/site/zeldarandomizer/" +
				"\n\n" +
				"Run your rom through Z1R, and then run the resulting rom through Infinite Hyrule." +
				"\n\n" +
				"Only versions 3.5.20 and higher of Z1R is supported. The Title Bar of Z1R will show its version number.\n" +
				"-----------------------------------------------------\n\n" +
				"Some flags can make the seed unbeatable or even crash the game." +
				"\n\n" +
				"The Following Settings must be set as:" +
				"\n\n" +
				"   Top Part:\n" +
				"       Overworld Quest: 1st Quest" +
				"\n\n" +
				"   Overworld Tab:\n" +
				"       Cave Shuffle: Vanilla\n" +
				"       Force Overworld Block: Disabled\n" +
				"       Mirror Overworld: Disabled" +
				"\n\n" +
				"   Dungeon Items Tab:\n" +
				"       White Sword Item: White Sword" +
				"\n\n" +
				"   Monsters Tab:\n" +
				"       Randomize Boss Groups: Disabled\n" +
				"       Shuffle Overworld Group: Disabled" +
				"\n\n" +
				"   Misc Tab:\n" +
				"       Replace Book Fire with Explosion: Disabled\n" +
				"       Race ROM: Disabled" +
				"\n\n" +
				"   Cosmetic Tab:\n" +
				"       Select Swap: Pause";

			MessageBox.Show(
				boxMessage, "Using Z1R?", MessageBoxButton.OK, MessageBoxImage.None
			);
		}

		private void PopUpCompatibleRomVersions(object sender, RoutedEventArgs e) {
			const string boxMessage =
				"Infinite Hyrule is compatible with English Roms that show only ©1986 on the title screen." +
				"\n\nIf the title screen shows ©1986-2003, the game will crash when defeating Ganon";

			MessageBox.Show(
				boxMessage, "Compatible Rom Versions", MessageBoxButton.OK, MessageBoxImage.None
			);
		}

		private void PopUpAutoMapperInstructions(object sender, RoutedEventArgs e) {
			const string boxMessage =
				"Included with Infinite Hyrule are two AutoMapper scripts that will show which screens you have explored on the Minimap." +
				"\n\n" +
				"These scripts only work with the emulators Mesen and FCEUX. Follow the instructions below for your Emulator after loading your rom." +
				"\n\n" +
				"With Mesen:" +
				"\n " +
				"1. Navigate to [Debug -> Script Window] in the file menu, or use the hotkey Control + N" +
				"\n " +
				"2. On the window that pops up, navigate to [File -> Open] in the file menu, or use the hotkey Control + O, and Select the automapper script with [mesen] in the filename." +
				"\n " +
				"3. Click the Run Script button or press F5. The bottom status window will read 'Script loaded successfully.'" +
				"\n " +
				"4. Keep this window open while playing." +
				"\n\n" +
				"With FCEUX:" +
				"\n " +
				"1. Navigate to [File -> Lua -> New Lua Script Window] in the file menu" +
				"\n " +
				"2. Click Browse and select the automapper script with [fceux] in the filename." +
				"\n " +
				"3. Click the Run button." +
				"\n " +
				"4. Keep this window open while playing.";

			MessageBox.Show(
				boxMessage, "Using the AutoMapper Script", MessageBoxButton.OK, MessageBoxImage.None
			);
		}

		private void ToggleSpoiler(object sender, RoutedEventArgs e) {
			_showSpoilerShield = !_showSpoilerShield;

			if (_showSpoilerShield) {
				MapPreviewSpoilerShield.Visibility = Visibility.Visible;
				MapPreview.Visibility = Visibility.Hidden;
			} else {
				MapPreviewSpoilerShield.Visibility = Visibility.Hidden;
				MapPreview.Visibility = Visibility.Visible;
			}
		}
	}
}