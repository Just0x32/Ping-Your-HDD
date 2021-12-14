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
        
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public Model()
        {
            SetToPingFilePath();
        }
        
        public bool IsPingRunning { get; private set; } = false;

        public string ToPingFilePath { get; private set; }

        public bool IsFileCreatingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        public bool IsFileReadingError { get; private set; } = false;

        public void ToogleWritingState()
        {

        }

        private void SetToPingFilePath ()
        {
            bool settingsFileExist = File.Exists(toSettingsFilePath);
            bool isValueFromSettingsFileValid = false;

            if (settingsFileExist)
            {
                string settingsFileFirstLine = ReadLineFromFile(toSettingsFilePath);
                string settingsFileValue;

                if (settingsFileFirstLine.IndexOf(settingsFileProperty) == 0)
                {
                    settingsFileValue = settingsFileFirstLine.Replace(settingsFileProperty, "");
                    int indexOfFileName = IndexOfFileName(settingsFileValue);

                    if (indexOfFileName >= 0 && Directory.Exists(settingsFileValue[..indexOfFileName]))
                    {
                        isValueFromSettingsFileValid = true;
                        ToPingFilePath = settingsFileValue;
                    }
                }
            }

            if (!settingsFileExist || !isValueFromSettingsFileValid)
            {
                ToPingFilePath = settingsFileDefaultValue;

                CreatingFile(toSettingsFilePath);
                WriteLineToFile(toSettingsFilePath, settingsFileProperty + settingsFileDefaultValue);
            }

            int IndexOfFileName(string toFilePath)
            {
                int indexOfLastDot = toFilePath.LastIndexOf(".");
                int indexOfLastBackSlash = toFilePath.LastIndexOf(@"\");

                if (indexOfLastDot > indexOfLastBackSlash && indexOfLastBackSlash >= 0)
                {
                    return indexOfLastBackSlash + 1;
                }
                else if (indexOfLastDot > indexOfLastBackSlash && indexOfLastBackSlash < 0)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
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
    }
}
