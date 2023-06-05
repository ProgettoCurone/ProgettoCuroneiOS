using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Plugin.NFC;

namespace GraficaCurone;

[IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = MainPage.MIME_TYPE)]
[IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = MainPage.MIME_TYPE)]

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        CrossNFC.Init(this);
        base.OnCreate(savedInstanceState);
    }

    protected override void OnResume()
    {   
        CrossNFC.OnResume();
        base.OnResume();
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        CrossNFC.OnNewIntent(intent);
    }
}
