using GraficaCurone.View;
namespace GraficaCurone;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new MainView();
	}
}