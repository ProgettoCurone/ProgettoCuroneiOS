using GraficaCurone.ViewModel;

namespace GraficaCurone.View;

public partial class CompassPage : ContentPage
{
    public CompassPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }
}