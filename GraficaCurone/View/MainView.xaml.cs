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

    public async void CameraLoaded(object sender, EventArgs e)
    {
        await mainViewModel.CameraLoadAsync();
    }

    private async void BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        await mainViewModel.BarCodeResultAsync(args);
    }

    protected override async void OnAppearing()
    {
        await mainViewModel.Init();
        await mainViewModel.trackManager.Init();
        await mainViewModel.NFCManager.Init();

        //mainViewModel.IosNfcManager.StartListening();
    }

    private async void OnAppearing(object sender, EventArgs e)
    {
        if (mainViewModel != null && mainViewModel.CameraVisible) await mainViewModel.ShowCamera();
    }
}