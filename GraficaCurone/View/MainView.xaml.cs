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
		InitializeComponent();
        mainViewModel = new MainViewModel(this);
        BindingContext = mainViewModel;
	}

    protected override async void OnAppearing()
    {
        await mainViewModel.Init();
        await mainViewModel.trackManager.Init();
        await mainViewModel.NFCManager.Init();

        //mainViewModel.IosNfcManager.StartListening();
    }

    private void OnAppearing(object sender, EventArgs e)
    {
        if (mainViewModel != null && mainViewModel.CameraVisible) mainViewModel.ShowCamera();
    }
}