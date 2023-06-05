using GraficaCurone.ViewModel;

namespace GraficaCurone.View;

public partial class MapPage : ContentPage
{
    public MapPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }
}