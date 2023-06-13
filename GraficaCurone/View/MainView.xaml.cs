using System.Diagnostics;
using Camera.MAUI;
using GraficaCurone.ViewModel;

namespace GraficaCurone.View;

public partial class MainView : Shell
{
	public CameraView camera { get; set; }
    private MainViewModel mainViewModel;

    bool i = true;

    public MainView()
	{
        try
        {
            mainViewModel = new MainViewModel(this);
            InitializeComponent();
            BindingContext = mainViewModel;

        } catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    protected override async void OnAppearing()
    {
        try
        {
            await mainViewModel.Init();
            await mainViewModel.trackManager.Init();
            await mainViewModel.NFCManager.Init();
        } catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //mainViewModel.IosNfcManager.StartListening();
    }

    private void OnAppearing(object sender, EventArgs e)
    {
        try
        {
            if (mainViewModel != null && mainViewModel.CameraVisible) mainViewModel.ShowCamera();
        } catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}