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
        private Model model = new Model();

        private string toPingFilePath;
        public string ToPingFilePath
        {
            get => toPingFilePath;
            set
            {
                toPingFilePath = value;
                OnPropertyChanged();
                IsSavedSettings = default;
            }
        }

        private string pingDelay;
        public string PingDelay
        {
            get => pingDelay;
            set
            {
                pingDelay = value;
                OnPropertyChanged();
                IsSavedSettings = default;
            }
        }

        private bool isPingingOnAppStart;
        public bool IsPingingOnAppStart
        {
            get => isPingingOnAppStart;
            set
            {
                isPingingOnAppStart = value;
                OnPropertyChanged();
                IsSavedSettings = default;
            }
        }

        public bool IsNotPinging { get => !model.IsPinging; private set => OnPropertyChanged(); }

        public bool IsSavedSettings
        {
            get => ToPingFilePath == model.ToPingFilePath
                    && PingDelay == model.PingDelay.ToString()
                    && IsPingingOnAppStart == model.IsPingingOnAppStart
                    && ToScriptFilePath == model.ToScriptFilePath;
            private set => OnPropertyChanged();
        }

        private string toScriptFilePath;
        public string ToScriptFilePath
        {
            get => toScriptFilePath;
            set
            {
                toScriptFilePath = value;
                OnPropertyChanged();
                IsSavedSettings = default;
                IsScriptAvailable = default;
            }
        }

        public bool IsScriptAvailable
        {
            get => !string.IsNullOrEmpty(ToScriptFilePath);
            private set => OnPropertyChanged();
        }

        public bool IsFileCreatingError { get => model.IsFileCreatingError; }
        public bool IsFileWritingError { get => model.IsFileWritingError; }
        public bool IsFileReadingError { get => model.IsFileReadingError; }
        public bool IsIOError { get => model.IsIOError(); }

        public ViewModel() => model.PropertyChanged += ModelNotify;

        public void Start() => model.Start();

        public void TooglePingState()
        {
            if (IsSavedSettings)
                model.TooglePingState();
        }

        public void ChangeToPingFilePath(string path) => ToPingFilePath = path;

        public void ChangeToScriptFilePath(string path) => ToScriptFilePath = path;

        public void ResetToScriptFilePath() => ToScriptFilePath = string.Empty;

        public void SetFromViewSettings()
            => model.SetFromViewSettings(ToPingFilePath, PingDelay, IsPingingOnAppStart.ToString(), ToScriptFilePath);

        public void RunScript() => model.RunScript();

        public void CloseApp() => model.CloseApp();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ModelNotify(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.ToPingFilePath))
                ToPingFilePath = model.ToPingFilePath;
            else if (e.PropertyName == nameof(model.PingDelay))
                PingDelay = model.PingDelay.ToString();
            else if (e.PropertyName == nameof(model.IsPingingOnAppStart))
                IsPingingOnAppStart = model.IsPingingOnAppStart;
            else if (e.PropertyName == nameof(model.IsPinging))
                IsNotPinging = default;
            else if (e.PropertyName == nameof(model.ToScriptFilePath))
                ToScriptFilePath = model.ToScriptFilePath;
            else
                OnPropertyChanged(e.PropertyName);
        }
    }
}
