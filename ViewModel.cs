using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclic_Ping_Your_HDD
{
    public class ViewModel
    {
        Model model;

        public ViewModel () => model = new Model();

        public bool IsPinging { get => model.IsPinging; }

        public string ToPingFilePath { get => model.ToPingFilePath; }

        public string PingDelay { get => model.PingDelay.ToString(); }

        public bool IsPingingOnAppStart { get => model.IsPingingOnAppStart; }

        public bool IsFileCreatingError { get => model.IsFileCreatingError; }

        public bool IsFileWritingError { get => model.IsFileWritingError; }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool AreFromSettingsValuesValid { get => model.AreFromSettingsValuesValid; }       // Debug

        public bool AreFromViewValuesValid { get => model.AreFromViewValuesValid; }       // Debug

        public string SettingsFromView { get => model.SettingsFromView; }               // Debug

        public string ToDirectoryPath { get => model.ToDirectoryPath; }               // Debug

        public string DebugMessage                                                    // Debug
        {
            get
            {
                return "ToPingFilePath: " + ToPingFilePath + Environment.NewLine +
                    "ToDirectoryPath: " + ToDirectoryPath + Environment.NewLine +
                    "PingDelay: " + PingDelay + Environment.NewLine +
                    "IsPingingOnAppStart: " + IsPingingOnAppStart.ToString() + Environment.NewLine +
                    Environment.NewLine +
                    "AreFromSettingsValuesValid: " + AreFromSettingsValuesValid.ToString() + Environment.NewLine +
                    Environment.NewLine +
                    "SettingsFromView: " + Environment.NewLine + SettingsFromView + Environment.NewLine +
                    "AreFromViewValuesValid: " + AreFromViewValuesValid.ToString() + Environment.NewLine +
                    Environment.NewLine +
                    "IsPinging: " + IsPinging.ToString() + Environment.NewLine +
                    "IsFileCreatingError: " + IsFileCreatingError.ToString() + Environment.NewLine +
                    "IsFileWritingError: " + IsFileWritingError.ToString() + Environment.NewLine +
                    "IsFileReadingError: " + IsFileReadingError.ToString();
            }
        }

        public void TooglePingState() => model.TooglePingState();

        public void TransferFromViewSettingsValues(string fromViewToPingFilePathValue, string fromViewPingDelayValue, string fromViewIsPingingOnAppStartValue)
            => model.CheckFromViewSettingsValues(fromViewToPingFilePathValue, fromViewPingDelayValue, fromViewIsPingingOnAppStartValue);

        public void CloseApp() => model.CloseApp();
    }
}
