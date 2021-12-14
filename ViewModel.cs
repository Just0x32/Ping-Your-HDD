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

        public ViewModel ()
        {
             model = new Model();
        }

        public string ToPingFilePath { get => model.ToPingFilePath; }

        public bool IsFileCreatingError { get => model.IsFileCreatingError; }

        public bool IsFileWritingError { get => model.IsFileWritingError; }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool IsFilePathValid { get => model.IsFilePathValidProperty; }       // Debug

        public string ToDirectoryPath { get => model.ToDirectoryPath; }               // Debug

        public string DebugMessage                                                    // Debug
        {
            get
            {
                return "ToPingFilePath: " + ToPingFilePath + Environment.NewLine +
                    "IsFileCreatingError: " + IsFileCreatingError.ToString() + Environment.NewLine +
                    "IsFileWritingError: " + IsFileWritingError.ToString() + Environment.NewLine +
                    "IsFileReadingError: " + IsFileReadingError.ToString() + Environment.NewLine +
                    "ToDirectoryPath: " + ToDirectoryPath + Environment.NewLine +
                    "IsFilePathValid: " + IsFilePathValid.ToString();
            }
        }

        public void TransferToPingFilePath(string toFilePath) => model.CheckAndSaveReceivedFilePath(toFilePath);
    }
}
