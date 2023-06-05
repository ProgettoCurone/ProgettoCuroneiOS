using CommunityToolkit.Mvvm.ComponentModel;
using GraficaCurone.View;
using GraficaCurone.ViewModel;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraficaCurone.Manager;

public partial class NFCManager : ObservableObject
{
    #region Variabili
    NFCNdefTypeFormat _type;
    bool _makeReadOnly = false;
    bool _eventsAlreadySubscribed = false;
    bool _isDeviceiOS = false;
    private TrackManager trackManager;
    public MainViewModel mainViewModel;
    #endregion

    public NFCManager(TrackManager trackManager, MainViewModel mainViewModel)
    {
        this.trackManager = trackManager;
        this.mainViewModel = mainViewModel;
    }

    #region Property per NFC
    public bool DeviceIsListening
    {
        get => _deviceIsListening;
        set
        {
            _deviceIsListening = value;
            OnPropertyChanged(nameof(DeviceIsListening));
        }
    }

    private bool _deviceIsListening;

    private bool _nfcIsEnabled;

    public bool NfcIsEnabled
    {
        get => _nfcIsEnabled;
        set
        {
            _nfcIsEnabled = value;
            OnPropertyChanged(nameof(NfcIsEnabled));
            OnPropertyChanged(nameof(NfcIsDisabled));
        }
    }

    public bool NfcIsDisabled => !NfcIsEnabled;
    #endregion

    #region showAlert
    Task ShowAlert(string message, string title = null) => App.Current.MainPage.DisplayAlert(string.IsNullOrWhiteSpace(title) ? "TITOLO" : title, message, "OK");
    #endregion

    #region NFC
    async void Current_OnMessageReceived(ITagInfo tagInfo)
    {
        if (tagInfo == null)
        {
            return;
        }

        var identifier = tagInfo.Identifier;
        var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
        var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

        if (!tagInfo.IsSupported)
        {
            return;
        }
        else if (tagInfo.IsEmpty)
        {
            return;
        }
        else
        {
            int n;
            bool success = int.TryParse(tagInfo.Records[0].Message, out n);
            if (!success)
                return;
            mainViewModel.MapVisible = true;
            mainViewModel.CompassVisible = false;
            mainViewModel.CameraVisible = false;
            await trackManager.PlayTheTrack(n-1);
        }
    }

    string GetMessage(NFCNdefRecord record)
    {
        var message = $"CONTENUTO: {record.Message}";
        if (!string.IsNullOrWhiteSpace(record.MimeType))
        {
            message += Environment.NewLine;
            message += $"TIPO DI MESSAGGIO: {record.MimeType}";
            message += Environment.NewLine;
        }
        return message;
    }

    async Task StartListeningIfNotiOS()
    {
        if (_isDeviceiOS)
        {
            SubscribeEvents();
            return;
        }
        await BeginListening();
    }

    public async Task BeginListening()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SubscribeEvents();
                CrossNFC.Current.StartListening();
            });

            
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }
    #endregion

    #region Metodi gestione NFC
    async Task AutoStartAsync()
    {
        await Task.Delay(500);
        await StartListeningIfNotiOS();
    }

    void SubscribeEvents()
    {
        if (_eventsAlreadySubscribed)
            UnsubscribeEvents();

        _eventsAlreadySubscribed = true;
        CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
        CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
        CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;
    }

    void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
        CrossNFC.Current.OnNfcStatusChanged -= Current_OnNfcStatusChanged;
        CrossNFC.Current.OnTagListeningStatusChanged -= Current_OnTagListeningStatusChanged;

        _eventsAlreadySubscribed = false;
    }

    void Current_OnTagListeningStatusChanged(bool isListening) => DeviceIsListening = isListening;

    async void Current_OnNfcStatusChanged(bool isEnabled)
    {
        NfcIsEnabled = isEnabled;
        await ShowAlert($"NFC has been {(isEnabled ? "enabled" : "disabled")}");
    }
    #endregion

    public async Task Init()
    {
        CrossNFC.Legacy = false;

        if (CrossNFC.IsSupported)
        {
            if (!CrossNFC.Current.IsAvailable)
                await ShowAlert("NFC is not available");

            NfcIsEnabled = CrossNFC.Current.IsEnabled;
            if (!NfcIsEnabled)
                await ShowAlert("NFC is disabled");

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                _isDeviceiOS = true;
            await AutoStartAsync().ConfigureAwait(false);
        }
    }
}