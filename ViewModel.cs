using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ping_Your_HDD
{
    public class ViewModel : INotifyPropertyChanged
    {
        Model model;

        public ViewModel ()
        {
            model = new Model();

            model.PropertyChanged += ModelNotify;
        }

        public bool IsPinging { get => model.IsPinging; }

        public string ToPingFilePath { get => model.ToPingFilePath; }

        public string PingDelay { get => model.PingDelay.ToString(); }

        public bool IsPingingOnAppStart { get => model.IsPingingOnAppStart; }

        public bool IsFileCreatingError { get => model.IsFileCreatingError; }

        public bool IsFileWritingError { get => model.IsFileWritingError; }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool IsIOError { get => model.IsIOError(); }

        public void TooglePingState() => model.TooglePingState();

        public void TransferFromViewSettingsValues(string fromViewToPingFilePathValue, string fromViewPingDelayValue, string fromViewIsPingingOnAppStartValue)
            => model.CheckFromViewSettingsValues(fromViewToPingFilePathValue, fromViewPingDelayValue, fromViewIsPingingOnAppStartValue);

        public void CloseApp() => model.CloseApp();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ModelNotify(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);
    }
}
