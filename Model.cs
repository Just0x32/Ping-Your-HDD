using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclic_Ping_Your_HDD
{
    public class Model
    {
        private readonly string settingsFileProperty = @"ToPingFilePath=";
        private readonly string settingsFileDefaultValue = @"ping.txt";
        private readonly string toSettingsFilePath = @"settings.txt";
        private readonly char[] fileNameForbiddenSymbols = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private readonly char[] pathNameForbiddenSymbols = { '<', '>', '"', '/', '|', '?', '*' };


        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public Model() => ReadAndCheckSettingsFile();
        
        public bool IsPingRunning { get; private set; } = false;

        public string ToPingFilePath { get; private set; }

        public bool IsFileCreatingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        public bool IsFileReadingError { get; private set; } = false;

        public bool IsFilePathValidProperty { get; private set; } = false;          // Debug

        public string ToDirectoryPath { get; private set; }                 // Debug

        public void ToogleWritingState()
        {

        }

        private void ReadAndCheckSettingsFile ()
        {
            bool settingsFileExist = File.Exists(toSettingsFilePath);
            bool isFilePathValid = false;

            if (settingsFileExist)
            {
                string settingsFileFirstLine = ReadLineFromFile(toSettingsFilePath);
                string settingsFileValue;

                if (settingsFileFirstLine.IndexOf(settingsFileProperty) == 0)
                {
                    settingsFileValue = settingsFileFirstLine.Replace(settingsFileProperty, "");

                    if (IsFilePathValid(settingsFileValue))
                    {
                        isFilePathValid = true;
                        ToPingFilePath = settingsFileValue;
                    }
                }
            }

            if (!settingsFileExist || !isFilePathValid)
            {
                CreatingFile(toSettingsFilePath);
                WriteLineToFile(toSettingsFilePath, settingsFileProperty + settingsFileDefaultValue);

                ToPingFilePath = settingsFileDefaultValue;
            }
        }

        public void CheckAndSaveReceivedFilePath(string receivedFilePath)
        {
            bool settingsFileExist = File.Exists(toSettingsFilePath);
            bool isFilePathValid = false;

            if (IsFilePathValid(receivedFilePath))
            {
                isFilePathValid = true;
                ToPingFilePath = receivedFilePath;
            }

            IsFilePathValidProperty = isFilePathValid;

            if (!settingsFileExist && !isFilePathValid)
            {
                CreatingFile(toSettingsFilePath);
                WriteLineToFile(toSettingsFilePath, settingsFileProperty + settingsFileDefaultValue);

                ToPingFilePath = settingsFileDefaultValue;
            }
            else if (!settingsFileExist || isFilePathValid)
            {
                CreatingFile(toSettingsFilePath);
                WriteLineToFile(toSettingsFilePath, settingsFileProperty + receivedFilePath);

                ToPingFilePath = receivedFilePath;
            }
        }

        private void CreatingFile(string toFilePath)
        {
            try
            {
                using (var stream = File.Create(toFilePath)) { };
            }
            catch (IOException e)
            {
                IsFileCreatingError = true;
            }
        }

        private void WriteLineToFile(string toFilePath, string textLine)
        {
            try
            {
                streamWriter = new StreamWriter(toFilePath);
                streamWriter.WriteLine(textLine);
            }
            catch (IOException e)
            {
                IsFileWritingError = true;
            }
            finally
            {
                streamWriter?.Dispose();
            }
        }

        private string ReadLineFromFile(string toFilePath)
        {
            string textLine = String.Empty;

            try
            {
                streamReader = new StreamReader(toFilePath);
                textLine = streamReader.ReadLine();
            }
            catch (IOException e)
            {
                IsFileReadingError = true;
            }
            finally
            {
                streamReader?.Dispose();
            }

            return textLine;
        }

        private bool IsFilePathValid(string toFilePath)
        {
            foreach (var forbiddenSymbol in pathNameForbiddenSymbols)
                if (toFilePath.Contains(forbiddenSymbol))
                    return false;

            int indexOfFileName = IndexOfFileName(toFilePath);

            if (indexOfFileName >= 0)
            {
                string toDirectoryPath = toFilePath[..indexOfFileName];
                string fileName = toFilePath[indexOfFileName..];

                foreach (var forbiddenSymbol in fileNameForbiddenSymbols)
                    if (fileName.Contains(forbiddenSymbol))
                        return false;

                if (toDirectoryPath == "")
                {
                    toDirectoryPath = @"\";
                }

                ToDirectoryPath = toDirectoryPath;                              // Debug

                if (Directory.Exists(toDirectoryPath))
                {
                    return true;
                }
            }

            return false;

            int IndexOfFileName(string toFilePath)
            {
                int indexOfLastDot = toFilePath.LastIndexOf(".");
                int indexOfLastBackSlash = toFilePath.LastIndexOf(@"\");

                if (indexOfLastDot < 0 || indexOfLastDot < indexOfLastBackSlash || (toFilePath.Length - indexOfLastDot) < 2)
                {
                    return -1;
                }
                else
                {
                    return indexOfLastBackSlash + 1;
                }
            }
        }
    }
}
