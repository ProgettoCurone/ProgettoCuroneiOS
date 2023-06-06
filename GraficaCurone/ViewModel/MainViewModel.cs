using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraficaCurone.Manager;
using GraficaCurone.View;
using Plugin.Maui.Audio;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.NFC;
using System.Threading;
using GraficaCurone.Utils;
using System.Diagnostics;

namespace GraficaCurone.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        #region Variabili_Parte_Grafica
        [ObservableProperty]private bool selected;

        private bool mapVisible;
        public bool MapVisible
        {
            get => mapVisible;

            set {
                mapVisible = value;

                if (value)
                {
                    CompassVisible = false;
                    CameraVisible = false;
                }

                OnPropertyChanged();
            }
        }

        private bool compassVisible;
        public bool CompassVisible
        {
            get => compassVisible;

            set
            {
                compassVisible = value;

                if (value)
                {
                    MapVisible = false;
                    CameraVisible = false;
                }

                OnPropertyChanged();
            }
        }

        private bool cameraVisible;
        public bool CameraVisible
        {
            get => cameraVisible;

            set
            {
                cameraVisible = value;

                if (value)
                {
                    MapVisible = false;
                    CompassVisible = false;
                }

                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private double rotation;
        [ObservableProperty]
        private string textCompass;
        [ObservableProperty] private Microsoft.Maui.Controls.View currentPage;
        [ObservableProperty]
        public LocalizationResourceManager localizationManager = LocalizationResourceManager.Instance;

        [ObservableProperty]
        private Stream documentStream;

        public CameraView cameraView;
        private MainView mainView;
        private bool isBusy;

        private QrCodePage QrCodePage;
        private CompassPage CompassPage;
        private MapPage MapPage;

        public NFCManager NFCManager { get; set; }
        public TrackManager trackManager { get; set; }
        public IFileSaver fileSaver;
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        #endregion

        //public void ControlloNFC()
        //{
        //    bool c = true;
        //    while (c)
        //    {
        //        if (nfcManager.NfcIsEnabled)
        //        {
        //            c = false;
        //        }
        //    }
        //}

        public MainViewModel(MainView mainView) 
        {
            //Thread thread = new Thread(ControlloNFC);
            //thread.Start();
            //MapVisible = true;
            this.mainView = mainView;
            cameraView = mainView.camera;
            trackManager = new TrackManager(AudioManager.Current);
            NFCManager = new NFCManager(trackManager, this);
            fileSaver = FileSaver.Default;

            QrCodePage = new QrCodePage(this);
            CompassPage = new CompassPage(this);
            MapPage = new MapPage(this);
        }

        public async Task Init()
        {
            await ShowMap();
            DocumentStream = await FileSystem.OpenAppPackageFileAsync("documentazione_applicazione_curone.pdf");
        }

        #region MetodiPagine

        [RelayCommand]
        public async void BottoneLinguaClicked(EventArgs e)
        {
            string linguaScelta;
            linguaScelta = await App.Current.MainPage.DisplayActionSheet("Select language:", "Cancel", null, "italiano", "english");

            if (linguaScelta == "italiano")
            {
                trackManager.InEnglish = false;
                LocalizationResourceManager.Instance.SettaCultura(new CultureInfo("it-IT"));
            }
            else if(linguaScelta == "english")
            {
                trackManager.InEnglish = true;
                LocalizationResourceManager.Instance.SettaCultura(new CultureInfo("en-gb"));
            }
            else
            {
                return;
            }
            
            if (trackManager.player != null)
            {
                await trackManager.LoadTracksAsync();
                //await trackManager.PlayTheTrack(trackManager.LastTrack);
            }
        }

        #region ChooseThePage
        [RelayCommand]
        public async Task SwitchPage(string pageType)
        {
            if (isBusy) return;
            isBusy = true;
            //utilizziamo una scringa come valore di riferimento in MVVM, ogni volta che un bottone verrà premuto, chiamerà il seguente metodo 
            //utilizzando come stringa "pageType", e il metodo non farà altro di verificare quale delle pagine è stata chiamata. 
            switch (pageType)
            {
                case "showmap":
                    await ShowMap();
                    break;

                case "showcompass":
                    await ShowCompass();
                    break;

                case "showcamera":
                    ShowCamera();
                    break;
            }
            isBusy = false;
        }
        #endregion

        #region Mappa
        public async Task ShowMap()
        {
            CurrentPage = null;
            CurrentPage = MapPage.Content;

            MapVisible = true;

            await cameraView.StopCameraAsync();
        }
        #endregion

        #region Bussola
        public async Task ShowCompass()
        {
            //Fai direttamente un metodo switch page che stoppa la camera metti la current a null ecc
            await cameraView.StopCameraAsync();
            if (Compass.Default.IsSupported)
            {
                if (!Compass.Default.IsMonitoring)
                {
                    // Turn on compass
                    Compass.Default.ReadingChanged += Compass_ReadingChanged;
                    Compass.Default.Start(SensorSpeed.Fastest);
                }
                CurrentPage = null;
                CurrentPage = CompassPage.Content;
            }

            CompassVisible = true;
        }
        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            Rotation = -(e.Reading.HeadingMagneticNorth);
            TextCompass = $"{Math.Round(e.Reading.HeadingMagneticNorth, 0)}°";
        }
        #endregion

        #region Camera

        public void ShowCamera()
        {
            CurrentPage = null;
            CurrentPage = QrCodePage.Content;
            CameraLoadAsync();

            CameraVisible = true;
        }

        public void CameraLoadAsync()
        {
            if (cameraView.Camera == null && cameraView.Cameras.Count > 0)
                cameraView.Camera = cameraView.Cameras.First();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
        public void BarCodeResultAsync(BarcodeEventArgs args)
        {
            if (args == null) { }
            MainThread.BeginInvokeOnMainThread(async() =>
            {
                int result;
                bool check = int.TryParse((args.Result[0].Text), out result);
                if (!check) return;

                await trackManager.PlayTheTrack(result-1);
                await ShowMap();
                await cameraView.StopCameraAsync();
            });
        }
        #endregion

        #endregion

        //public void PdfViewerPage(string pdfFilePath)
        //{
        //    if (File.Exists(pdfFilePath))
        //    {
        //        using (var stream = new FileStream(pdfFilePath, FileMode.Open))
        //        {
        //            var document = PdfDocument.Load(stream);

        //            var imageSource = ImageSource.FromStream(() =>
        //            {
        //                var memoryStream = new MemoryStream();
        //                document.
        //                document.Render(0, 300, 300, true).Save(memoryStream, Microsoft.Maui.Graphics.ImageFormat.Png);
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                return memoryStream;
        //            });

        //            var image = new Image { Source = imageSource };
        //            Content = new ScrollView { Content = image };
        //        }
        //    }
        //}

        [RelayCommand]
        private async void SaveFile()
        {
            if (isBusy) return;
            //isBusy = true;

            CrossNFC.Current.StopListening();
            //var externalDir = FileSystem.AppDataDirectory;
            //var externalDownloadDir = Path.Combine(externalDir, "Download");
            var fileStream = await FileSystem.Current.OpenAppPackageFileAsync("documentazione_applicazione_curone.pdf");

            bool status = await PermissionUtils.CheckForStoragePermission();
            if (!status) return;

            var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (!result.IsSuccessful) return;

            var endStream = File.Create(Path.Combine(result.Folder.Path, "documentazione_applicazione_curone.pdf"));
            await fileStream.CopyToAsync(endStream);

            isBusy = false;


            //var path = await FileSaver.Default.SaveAsync(Path.Combine(result.Folder.Path, "documentazione_applicazione_curone.pdf"), fileStream, CancellationTokenSource.Token);
            //if (path.IsSuccessful)
            //{
            //    await Toast.Make($"The file was saved successfully to location: {path.FilePath}").Show(default);
            //}
            //else
            //{
            //    await Toast.Make($"The file was not saved successfully with error: {path.Exception.Message}").Show(default);
            //}
        }

        private int clickVar;
        private bool nfcBusy = false;
        private Timer timer;

        [RelayCommand]
        private async void TapGesture()
        {
            try
            {
                if (nfcBusy) return;

                if (timer == null)
                {
                    timer = new Timer(_ =>
                    {
                        clickVar = 0;
                        timer = null;
                    }, null, TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
                }

                clickVar++;
                Debug.Print(clickVar.ToString());

                if (clickVar >= 3)
                {
                    nfcBusy = true;

                    await NFCManager.BeginListening();
                    clickVar = 0;
                    timer = null;

                    nfcBusy = false;
                }
            } catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Ciao", ex.Message, "OK");
            }
        }
    }
}