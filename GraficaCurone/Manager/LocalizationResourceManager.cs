using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using GraficaCurone.Resources.Languages;
namespace GraficaCurone.Manager
{
    public class LocalizationResourceManager:INotifyPropertyChanged
    {
        public LocalizationResourceManager() 
        { 
            AppResources.Culture=CultureInfo.CurrentCulture;
        }
        public static LocalizationResourceManager Instance { get; } = new();
        public object this[string resourceKey]=>AppResources
            .ResourceManager.GetObject(resourceKey, AppResources.Culture)?? Array.Empty<byte>();
        public event PropertyChangedEventHandler PropertyChanged;   
        public void SettaCultura(CultureInfo cultura)
        {
            AppResources.Culture = cultura;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
