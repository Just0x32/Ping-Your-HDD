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

        private const int propertiesQuantities = 3;

        private readonly string[] settingsProperties = { @"ToPingFilePath=", @"PingDelay=", @"IsPingingOnAppStart=" };
        private readonly string[] settingsDefaultValues = { @"ping.txt", "3", @"false" };

        private delegate bool ValidationMethod(string value);
        private readonly ValidationMethod[] AreSettingsValuesValid;

        private readonly char[] fileNameForbiddenSymbols = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private readonly char[] pathNameForbiddenSymbols = { '<', '>', '"', '/', '|', '?', '*' };

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public Model()
        {
            AreSettingsValuesValid = new ValidationMethod[] { IsToPingFilePathValueValid, IsPingDelayValueValid, IsPingingOnAppStartValueValid };

            CheckAllArraysLength();
            CheckSettingsFile();

            if(IsPingingOnAppStart)
                TooglePingState();
        }

        public bool IsPinging { get; private set; } = false;
        
        public string ToPingFilePath { get; private set; }

        public int PingDelay { get; private set; }
        
        public bool IsPingingOnAppStart { get; private set; } = false;

        public bool IsFileCreatingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        public bool IsFileReadingError { get; private set; } = false;

        public bool AreFromSettingsValuesValid { get; private set; } = false;          // Debug

        public bool AreFromViewValuesValid { get; private set; } = false;          // Debug

        public string SettingsFromView { get; private set; }                        // Debug

        public string ToDirectoryPath { get; private set; }                          // Debug

        public void TooglePingState()
        {
            IsPinging = !IsPinging;


        }

        private bool IsToPingFilePathValueValid(string toFilePathValue)
        {
            foreach (var forbiddenSymbol in pathNameForbiddenSymbols)
                if (toFilePathValue.Contains(forbiddenSymbol))
                    return false;

            int indexOfFileName = IndexOfFileName(toFilePathValue);

            if (indexOfFileName >= 0)
            {
                string toDirectoryPath = toFilePathValue[..indexOfFileName];
                string fileName = toFilePathValue[indexOfFileName..];

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

        private bool IsPingDelayValueValid(string delayValue)
        {
            int parsed;

            if (!int.TryParse(delayValue, out parsed) || parsed < 3 || parsed > 3600)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        private bool IsPingingOnAppStartValueValid(string value)
        {
            if (value.ToLower() == "true" || value.ToLower() == "false")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckAllArraysLength()
        {
            if (settingsProperties.Length != propertiesQuantities || settingsDefaultValues.Length != propertiesQuantities || AreSettingsValuesValid.Length != propertiesQuantities)
            {
                throw new ArgumentException("Not all quantities are equal.");
            }
        }

        private void CheckSettingsFile ()
        {
            if (!File.Exists(toSettingsFilePath))
                CreatingFile(toSettingsFilePath);

            string[] fromSettingsValues = new string[propertiesQuantities];
            for (int i = 0; i < propertiesQuantities; i++)
            {
                fromSettingsValues[i] = "";
            }

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
                SetPublicProperties(fromSettingsValues);
            }
            else
            {
                SetSettingsValues(settingsDefaultValues);
            }
        }

        public void CheckFromViewSettingsValues(string toPingFilePathValue, string pingDelayValue, string isPingingOnAppStartValue)
        {
            SettingsFromView = toPingFilePathValue + Environment.NewLine + pingDelayValue + Environment.NewLine + isPingingOnAppStartValue;        // Debug

            if (IsToPingFilePathValueValid(toPingFilePathValue) && IsPingDelayValueValid(pingDelayValue) && IsPingingOnAppStartValueValid(isPingingOnAppStartValue))
            {
                string[] fromViewSettingsValues = new string[propertiesQuantities] { toPingFilePathValue, pingDelayValue, isPingingOnAppStartValue };
                SetSettingsValues(fromViewSettingsValues);

                AreFromViewValuesValid = true;          // Debug
            }
            else
            {
                AreFromViewValuesValid = false;         // Debug
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

        private void SetSettingsValues(string[] settingsValues)
        {
            try
            {
                streamWriter = new StreamWriter(toSettingsFilePath);

                for (int i = 0; i < propertiesQuantities; i++)
                    streamWriter.WriteLine(settingsProperties[i] + settingsValues[i]);
            }
            catch (IOException e)
            {
                IsFileWritingError = true;
            }
            finally
            {
                streamWriter?.Dispose();
            }

            SetPublicProperties(settingsValues);
        }

        private void SetPublicProperties(string[] values)
        {
            ToPingFilePath = values[0];
            PingDelay = Convert.ToInt32(values[1]);
            IsPingingOnAppStart = Convert.ToBoolean(values[2]);
        }

        public void CloseApp()
        {

        }
    }
}