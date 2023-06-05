using Camera.MAUI.ZXingHelper;
using Camera.MAUI;
using GraficaCurone.ViewModel;

namespace GraficaCurone.View;

public partial class QrCodePage : ContentPage
{
    private MainViewModel viewModel;

    public QrCodePage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        viewModel.cameraView = cameraView;
    }

    //private async void CameraLoaded(object sender, EventArgs e)
    //{
    //    await viewModel.CameraLoadAsync();
    //}

    protected override async void OnAppearing()
    {
        if (viewModel.CameraVisible) await viewModel.CameraLoadAsync();
    }

    private async void BarcodeDetected(object sender, BarcodeEventArgs args)
    {
        await viewModel.BarCodeResultAsync(args);
    }
}