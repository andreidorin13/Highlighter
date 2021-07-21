using Microsoft.Extensions.Options;
using GalaSoft.MvvmLight;
using Highlighter.Models;
using GalaSoft.MvvmLight.Command;

namespace Highlighter {
    public sealed class MainViewModel : ViewModelBase {

        public Settings Config { get; private set; }

        //public RelayCommand Add { get; private set; } = new RelayCommand(() => System.Windows.MessageBox.Show("Clicked Add"));

        public MainViewModel(IOptions<Settings> settings) {
            Config = settings.Value;
        }
    }
}