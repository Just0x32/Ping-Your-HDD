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
        private readonly string fromSettingsFileProperty = "ToPingFolderPath=";
        private readonly string fromSettingsFileDefaultValue = "ping.txt";
        private readonly string toSettingsFilePath = "settings.txt";
        
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public Model()
        {
            bool isUsingFromSettingsFilePath = false;
            bool settingsFileExist = File.Exists(toSettingsFilePath);

            if (settingsFileExist)
            {
                streamReader = new StreamReader(toSettingsFilePath);
                
                string fromSettingsFilePath;
                string settingsFileFirstLine = streamReader.ReadLine();
                if (settingsFileFirstLine.IndexOf(fromSettingsFileProperty) == 0)
                {
                    fromSettingsFilePath = settingsFileFirstLine.Replace(fromSettingsFileProperty, "");

                    if (Directory.Exists(fromSettingsFilePath))
                        isUsingFromSettingsFilePath = true;
                }
            }

            if (!settingsFileExist || !isUsingFromSettingsFilePath)
            {
                CreatingFile(toSettingsFilePath);
                WriteLineToFile(fromSettingsFileDefaultValue, fromSettingsFileProperty + fromSettingsFileDefaultValue);
            }



        }

        public string ToPingFolderPath { get; private set; }

        public bool IsFileCreatingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        private void CreatingFile(string toFilePath)
        {
            try
            {
                File.CreateText(toFilePath);
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
                streamWriter.Close();
            }
        }
    }
}
