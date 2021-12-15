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

        public bool IsFileCreatingError { get => model.IsFileCreatingError; }

        public bool IsFileWritingError { get => model.IsFileWritingError; }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool AreFromSettingsValuesValid { get => model.AreFromSettingsValuesValid; }       // Debug

        public string ToDirectoryPath { get => model.ToDirectoryPath; }               // Debug

        public string DebugMessage                                                    // Debug
        {
            get
            {
                return "ToPingFilePath: " + ToPingFilePath + Environment.NewLine +
                    "ToDirectoryPath: " + ToDirectoryPath + Environment.NewLine +
                    Environment.NewLine +
                    "AreFromSettingsValuesValid: " + AreFromSettingsValuesValid.ToString() + Environment.NewLine +
                    Environment.NewLine +
                    "IsPinging: " + IsPinging.ToString() + Environment.NewLine +
                    "IsFileCreatingError: " + IsFileCreatingError.ToString() + Environment.NewLine +
                    "IsFileWritingError: " + IsFileWritingError.ToString() + Environment.NewLine +
                    "IsFileReadingError: " + IsFileReadingError.ToString();
            }
        }

        public void TransferToPingFilePath(string toFilePath)
        {
             //model.CheckAndSaveReceivedFilePath(toFilePath);
        }
    }
}
