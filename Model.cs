using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0168

namespace Cyclic_Ping_Your_HDD
{
    public class Model
    {
        private readonly string toSettingsFilePath = @"settings.txt";

        private readonly string[] settingsProperties = { @"ToPingFilePath=", @"IsPingingOnAppStart=" };
        private readonly string[] settingsDefaultValues = { @"ping.txt", @"false" };

        private delegate bool ValidationMethod(string value);
        private readonly ValidationMethod[] AreSettingsValuesValid;

        private readonly char[] fileNameForbiddenSymbols = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private readonly char[] pathNameForbiddenSymbols = { '<', '>', '"', '/', '|', '?', '*' };

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public Model()
        {
            AreSettingsValuesValid = new ValidationMethod[] { IsToPingFilePathValueValid, IsPingingOnAppStartValueValid };
            CheckSettingsFile();
        }

        public bool IsPinging { get; private set; } = false;
        
        public string ToPingFilePath { get; private set; }
        
        private bool IsPingingOnAppStart { get; set; } = false;

        public bool IsFileCreatingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        public bool IsFileReadingError { get; private set; } = false;

        public bool AreFromSettingsValuesValid { get; private set; } = false;          // Debug

        public string ToDirectoryPath { get; private set; }                 // Debug

        public void TooglePingState()
        {
            IsPinging = !IsPinging;


        }

        private bool IsToPingFilePathValueValid(string toFilePath)
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

        private bool IsPingingOnAppStartValueValid(string value)
        {
            if (value == "true" || value == "false")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckSettingsFile ()
        {
            if (!File.Exists(toSettingsFilePath))
                CreatingFile(toSettingsFilePath);

            string[] fromSettingsValues = new string[] { "", "" };

#nullable enable
            string? fromSettingsValue;

            for (int i = 0; i < settingsProperties.Length; i++)
            {
                fromSettingsValue = GetFromSettingsFileValue(settingsProperties[i]);

                if (fromSettingsValue != null && AreSettingsValuesValid[i](fromSettingsValue))
                {
                    fromSettingsValues[i] = fromSettingsValue;
                }
            }
#nullable disable

            bool areFromSettingsValuesValid = true;

            foreach (var item in fromSettingsValues)
            {
                if (item.Length == 0)
                {
                    areFromSettingsValuesValid = false;
                    break;
                }
            }

            AreFromSettingsValuesValid = areFromSettingsValuesValid;

            if (areFromSettingsValuesValid)
            {
                ToPingFilePath = fromSettingsValues[0];
                IsPingingOnAppStart = Convert.ToBoolean(fromSettingsValues[1]);
            }
            else
            {
                SetDefaultSettings();
            }
        }

        //public void CheckAndSaveReceivedFilePath(string receivedFilePath)
        //{
        //    bool settingsFileExist = File.Exists(toSettingsFilePath);
        //    bool isFilePathValid = false;

        //    if (IsFilePathValid(receivedFilePath))
        //    {
        //        isFilePathValid = true;
        //        ToPingFilePath = receivedFilePath;
        //    }

        //    IsFilePathValidProperty = isFilePathValid;

        //    if (!settingsFileExist && !isFilePathValid)
        //    {
        //        CreatingFile(toSettingsFilePath);
        //        WriteLineToFile(toSettingsFilePath, toPingFilePathProperty + settingsFileDefaultValue);

        //        ToPingFilePath = settingsFileDefaultValue;
        //    }
        //    else if (!settingsFileExist || isFilePathValid)
        //    {
        //        CreatingFile(toSettingsFilePath);
        //        WriteLineToFile(toSettingsFilePath, toPingFilePathProperty + receivedFilePath);

        //        ToPingFilePath = receivedFilePath;
        //    }
        //}

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

#nullable enable
        private string? GetFromSettingsFileValue(string propertyName)
        {
            string? textLine = null;

            try
            {
                streamReader = new StreamReader(toSettingsFilePath);

                do
                {
                    textLine = streamReader.ReadLine();

                    if (textLine is null)
                        break;
                }
                while (!textLine.StartsWith(propertyName));
            }
            catch (IOException e)
            {
                IsFileReadingError = true;
            }
            finally
            {
                streamReader?.Dispose();
            }
                            
            if (textLine != null && textLine.Length > propertyName.Length)
            {
                textLine = textLine.Replace(propertyName, "");
            }

            return textLine;
        }
#nullable disable

        private void SetDefaultSettings()
        {
            try
            {
                streamWriter = new StreamWriter(toSettingsFilePath);

                for (int i = 0; i < settingsProperties.Length; i++)
                    streamWriter.WriteLine(settingsProperties[i] + settingsDefaultValues[i]);
            }
            catch (IOException e)
            {
                IsFileWritingError = true;
            }
            finally
            {
                streamWriter?.Dispose();
            }

            ToPingFilePath = settingsDefaultValues[0];
            IsPingingOnAppStart = Convert.ToBoolean(settingsDefaultValues[1]);

        }
    }
}