using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraficaCurone.Model;
using GraficaCurone.View;

namespace GraficaCurone.ViewModel
{
	public partial class IntroScreenViewModel : ObservableObject
	{
		[ObservableProperty] private string buttonText;
		private int position;

		public int Position
		{
			get => position;
			set
			{
				if (value == IntroScreens.Count - 1) ButtonText = "Inizia";
				else ButtonText = "Avanti";

				position = value;
				OnPropertyChanged();
			}
		}
		
		public ObservableCollection<IntroScreenModel> IntroScreens { get; set; } = new ObservableCollection<IntroScreenModel>();

		public IntroScreenViewModel()
		{
			ButtonText = "Avanti";

			IntroScreens.Add(new IntroScreenModel
			{
				IntroTitle = "Parco del Curone",
				IntroDescription = "Test prima pagina",
				IntroIcon = "forest_image"
			});
			
			IntroScreens.Add(new IntroScreenModel
			{
				IntroTitle = "Nfc",
				IntroDescription = "Test seconda pagina",
				IntroIcon = "nfc_illustration"
			});
		}

		[RelayCommand]
		private async Task Next()
		{
			if (Position >= IntroScreens.Count - 1)
			{
				await MainThread.InvokeOnMainThreadAsync(async () => await App.Current.MainPage.Navigation.PushAsync(new MainView()));
			}
			else Position += 1;
		}
		
	}
}

