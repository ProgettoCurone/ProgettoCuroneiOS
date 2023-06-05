using static System.Net.Mime.MediaTypeNames;

namespace GraficaCurone;

public partial class MainPage : Shell
{
    public bool MappaVisibile = true;
    public bool BussolaVisibile = false;
    public bool QrVisibile = false;
    public const string MIME_TYPE = "application/com.companyname.nfcsample";

    public MainPage()
	{
		InitializeComponent();
	}

    private void MappaButton_Clicked(object sender, EventArgs e)
    {
        MappaButton.BackgroundColor = Color.FromArgb("#FFFFFF");
        BussolaButton.BackgroundColor = Color.FromArgb("#224436");
        QRButton.BackgroundColor = Color.FromArgb("#224436");
        MappaButton.TextColor = Color.FromArgb("#000000");
        BussolaButton.TextColor = Color.FromArgb("#FFFFFF");
        QRButton.TextColor = Color.FromArgb("#FFFFFF");
        FrameMappa.IsVisible= true;
        FrameBussola.IsVisible = false;
        FrameQR.IsVisible = false;
    }

    private void BussolaButton_Clicked(object sender, EventArgs e)
    {
        MappaButton.BackgroundColor = Color.FromArgb("#224436");
        BussolaButton.BackgroundColor = Color.FromArgb("#FFFFFF");
        QRButton.BackgroundColor = Color.FromArgb("#224436");
        MappaButton.TextColor = Color.FromArgb("#FFFFFF");
        BussolaButton.TextColor = Color.FromArgb("#000000");
        QRButton.TextColor = Color.FromArgb("#FFFFFF");
        FrameMappa.IsVisible = false;
        FrameBussola.IsVisible = true;
        FrameQR.IsVisible = false;

        if (Compass.Default.IsSupported)
        {
            if (!Compass.Default.IsMonitoring)
            {
                // Turn on compass
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.Fastest);
            }
            else
            {
                // Turn off compass
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= Compass_ReadingChanged;
            }
        }
    }
    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        Bussola.Rotation = -(e.Reading.HeadingMagneticNorth);
        testoBuss.Text = $"{Math.Round(e.Reading.HeadingMagneticNorth, 0)}°";
    }
    private void QRButton_Clicked(object sender, EventArgs e)
    {
        MappaButton.BackgroundColor = Color.FromArgb("#224436");
        BussolaButton.BackgroundColor = Color.FromArgb("#224436");
        QRButton.BackgroundColor = Color.FromArgb("#FFFFFF");
        MappaButton.TextColor = Color.FromArgb("#FFFFFF");
        BussolaButton.TextColor = Color.FromArgb("#FFFFFF");
        QRButton.TextColor = Color.FromArgb("#000000");
        FrameMappa.IsVisible = false;
        FrameBussola.IsVisible = false;
        FrameQR.IsVisible = true;
    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Qr.Text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
        });
    }
}

