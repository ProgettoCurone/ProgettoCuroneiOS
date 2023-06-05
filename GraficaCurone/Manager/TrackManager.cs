using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.Maui.Audio;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficaCurone.Manager
{
    public partial class TrackManager : ObservableObject
    {
        #region Variabili
        [ObservableProperty]
        private string currentText;

        [ObservableProperty]
        public string pathImage = "mappa.png";

        public List<string> percorsoImmagini = new List<string>(5);
        public List<string> tracceAudio = new List<string>(5);
        public List<string> tracceTesto = new List<string>(5);
        public double secondi = 0;
        public IAudioPlayer player;
        public bool InEnglish;
        private IAudioManager audioManager;
        public int LastTrack { get; set; }
        #endregion

        public TrackManager(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        public async Task Init()
        {
            var load = LoadTracksAsync();
            StartAccelerometer();
            await load;
        }

        public async Task LoadTracksAsync()
        {
            if (tracceTesto.Count != 0 || tracceAudio.Count != 0)
            {
                tracceAudio.Clear();
                tracceTesto.Clear();
                percorsoImmagini.Clear();
            }

            var path = "";

            if (InEnglish)
            {
                for (int i = 1; i < tracceTesto.Capacity + 1; i++)
                {
                    percorsoImmagini.Add($"mappa{i}.png");
                    tracceAudio.Add($"audioTrack_{i}.mp3");
                    path = $"textTrack_{i}.txt";
                    var result = await FileSystem.OpenAppPackageFileAsync(path);
                    StreamReader stream = new StreamReader(result);
                    tracceTesto.Add(stream.ReadToEnd());
                }
            }
            else
            {
                for (int i = 1; i < tracceTesto.Capacity + 1; i++)
                {
                    percorsoImmagini.Add($"mappa{i}.png");
                    tracceAudio.Add($"tracceAudio_{i}.mp3");
                    path = $"tracceTesto_{i}.txt";
                    var result = await FileSystem.OpenAppPackageFileAsync(path);
                    StreamReader stream = new StreamReader(result);
                    tracceTesto.Add(stream.ReadToEnd());
                }
            }
        }

        public async Task PlayTheTrack(int i)
        {
            if (i < 1 || i >= tracceTesto.Count)
                return;

            LastTrack = i;

            CurrentText = tracceTesto[i];
            PathImage = percorsoImmagini[i];

            if (player != null && player.IsPlaying) player.Dispose();

            player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(tracceAudio[i]));
            if (player == null) { return; }
            player.Seek(secondi);
            player.Play();
        }

        public void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            if (player == null || player == default)
                return;

            if (!player.IsPlaying)
                player.Play();

            else
                player.Pause();
        }

        public void StartAccelerometer()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                    Accelerometer.Default.Start(SensorSpeed.Fastest);
                }
                else
                {
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
                }
            }
        }
    }
}
