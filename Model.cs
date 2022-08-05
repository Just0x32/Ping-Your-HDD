using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ping_Your_HDD
{
    public class Model : INotifyPropertyChanged
    {
        private readonly string toSettingsFilePath = @"settings.txt";

        private const int propertiesQuantities = 4;

        private readonly string[] settingsProperties
            = { @"ToPingFilePath=", @"PingDelay=", @"IsPingingOnAppStart=", @"ToScriptFilePath=" };
        private readonly string[] settingsDefaultValues = { @"ping.txt", "3", @"false", "" };

        private delegate bool ValidationMethod(string value);
        private readonly ValidationMethod[] AreSettingsValuesValid;

        private readonly char[] fileNameForbiddenSymbols = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private readonly char[] pathNameForbiddenSymbols = { '<', '>', '"', '/', '|', '?', '*' };

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        private Thread pingThread;

        private bool isStarted = false;
        private bool isFileCreatingError = false;
        private bool isFileWritingError = false;
        private bool isFileReadingError = false;

        private string toPingFilePath;
        public string ToPingFilePath
        {
            get => toPingFilePath;
            private set
            {
                toPingFilePath = value;
                OnPropertyChanged();
            }
        }

        private int pingDelay;
        public int PingDelay
        {
            get => pingDelay;
            private set
            {
                pingDelay = value;
                OnPropertyChanged();
            }
        }

        private bool isPingingOnAppStart = false;
        public bool IsPingingOnAppStart
        {
            get => isPingingOnAppStart;
            private set
            {
                isPingingOnAppStart = value;
                OnPropertyChanged();
            }
        }

        private bool isPinging;
        public bool IsPinging
        {
            get => isPinging;
            private set
            {
                isPinging = value;
                OnPropertyChanged();
            }
        }

        private string toScriptFilePath;
        public string ToScriptFilePath
        {
            get => toScriptFilePath;
            private set
            {
                toScriptFilePath = value;
                OnPropertyChanged();
            }
        }

        private bool IsClosingApp { get; set; } = false;

        public bool IsFileCreatingError
        {
            get => isFileCreatingError;
            private set
            {
                isFileCreatingError = value;
                OnPropertyChanged();
            }
        }

        public bool IsFileWritingError
        {
            get => isFileWritingError;
            private set
            {
                isFileWritingError = value;
                OnPropertyChanged();
            }
        }

        public bool IsFileReadingError
        {
            get => isFileReadingError;
            private set
            {
                isFileReadingError = value;
                OnPropertyChanged();
            }
        }

        public Model()
        {
            AreSettingsValuesValid = new ValidationMethod[]
            {
                IsToFilePathValid,
                IsPingDelayValid,
                IsPingingOnAppStartValueValid,
                IsRealOrEmptyToFilePathValid,
            };
            CheckAllArraysLength();
        }

        public void Start()
        {
            if (!isStarted)
            {
                CheckSettingsFile();
                IsPinging = false;
                RunScript();
                pingThread = new Thread(new ThreadStart(Ping));
                pingThread.Start();

                if (IsPingingOnAppStart)
                    TooglePingState();

                isStarted = true;
            }
        }

        public bool IsIOError() => IsFileCreatingError | IsFileWritingError | IsFileReadingError;

        public void TooglePingState()
        {
            IsPinging = !IsPinging;

            if (pingThread.IsAlive)
                pingThread.Interrupt();
        }

        private void Ping()
        {
            int pingDelay;
            int lineCounter = 0;
            bool appendLine;

            while (!IsClosingApp && !IsIOError())
            {
                pingDelay = PingDelay * 1000;
                ThreadDelay(Timeout.Infinite);

                if (IsPinging && !File.Exists(ToPingFilePath))
                    CreateFile(ToPingFilePath);

                while (IsPinging && !IsClosingApp && !IsIOError())
                {
                    if (lineCounter > 200)
                    {
                        appendLine = false;
                        lineCounter = 0;
                    }
                    else
                        appendLine = true;

                    WritePingFile(ToPingFilePath, appendLine);
                    lineCounter++;

                    ThreadDelay(pingDelay);
                }
            }

            void ThreadDelay(int delay)
            {
                try
                {
                    Thread.Sleep(delay);
                }
                catch (ThreadInterruptedException) { }
            }

            void WritePingFile(string toPingFilePath, bool appendLine)
            {
                try
                {
                    streamWriter = new StreamWriter(toPingFilePath, appendLine);
                    streamWriter.WriteLine(DateTime.Now);
                }
                catch (IOException)
                {
                    IsFileWritingError = true;
                }
                finally
                {
                    streamWriter?.Dispose();
                }
            }
        }

        private bool IsToFilePathValid(string toFilePath)
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
                    toDirectoryPath = @"\";

                if (Directory.Exists(toDirectoryPath))
                    return true;
            }

            return false;

            int IndexOfFileName(string toFilePath)
            {
                int indexOfLastDot = toFilePath.LastIndexOf(".");
                int indexOfLastBackSlash = toFilePath.LastIndexOf(@"\");

                if (indexOfLastDot < 0 || indexOfLastDot < indexOfLastBackSlash || (toFilePath.Length - indexOfLastDot) < 2)
                    return -1;
                else
                    return indexOfLastBackSlash + 1;
            }
        }

        private bool IsPingDelayValid(string delayValue)
            => !(!int.TryParse(delayValue, out int parsed) || parsed < 3 || parsed > 3600);

        private bool IsPingingOnAppStartValueValid(string value)
            => value.ToLower() == "true" || value.ToLower() == "false";

        private bool IsRealOrEmptyToFilePathValid(string toFilePath)
        {
            if (string.IsNullOrEmpty(toFilePath))
                return true;
            else
                return IsToFilePathValid(toFilePath);
        }

        private void CheckAllArraysLength()
        {
            if (settingsProperties.Length != propertiesQuantities
                || settingsDefaultValues.Length != propertiesQuantities
                || AreSettingsValuesValid.Length != propertiesQuantities)
                throw new ArgumentException("Not all quantities are equal.");
        }

        private void CheckSettingsFile ()
        {
            if (!File.Exists(toSettingsFilePath))
                CreateFile(toSettingsFilePath);

            string[] fromSettingsValues = new string[propertiesQuantities];

            for (int i = 0; i < propertiesQuantities; i++)
                fromSettingsValues[i] = "";

#nullable enable
            string? fromSettingsValue;

            for (int i = 0; i < settingsProperties.Length; i++)
            {
                fromSettingsValue = GetFromSettingsFileValue(settingsProperties[i]);

                if (fromSettingsValue != null && AreSettingsValuesValid[i](fromSettingsValue))
                    fromSettingsValues[i] = fromSettingsValue;
            }
#nullable disable

            bool areFromSettingsValuesValid = true;

            foreach (var item in fromSettingsValues)
                if (item.Length == 0)
                {
                    areFromSettingsValuesValid = false;
                    break;
                }

            if (areFromSettingsValuesValid)
                SetPublicProperties(fromSettingsValues);
            else
                SetSettingsValues(settingsDefaultValues);
        }

        public void SetFromViewSettings(string toPingFilePath, string pingDelay, string isPingingOnAppStart, string toScriptFilePath)
        {
            if (IsToFilePathValid(toPingFilePath)
                && IsPingDelayValid(pingDelay)
                && IsPingingOnAppStartValueValid(isPingingOnAppStart)
                && IsRealOrEmptyToFilePathValid(toScriptFilePath))
            {
                string[] fromViewSettingsValues
                    = new string[propertiesQuantities] { toPingFilePath, pingDelay, isPingingOnAppStart, toScriptFilePath };
                SetSettingsValues(fromViewSettingsValues);
            }
        }

        private void CreateFile(string toFilePath)
        {
            try
            {
                using (var stream = File.Create(toFilePath)) { };
            }
            catch (IOException)
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
            catch (IOException)
            {
                IsFileReadingError = true;
            }
            finally
            {
                streamReader?.Dispose();
            }
                            
            if (textLine != null && textLine.Length > propertyName.Length)
                textLine = textLine.Replace(propertyName, "");

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
            catch (IOException)
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
            ToScriptFilePath = values[3];
        }

        public void RunScript()
        {
            if (!string.IsNullOrEmpty(ToScriptFilePath))
            {
                Process process = new Process();
                process.StartInfo.FileName = ToScriptFilePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
        }

        public void CloseApp()
        {
            IsClosingApp = true;

            if (pingThread.IsAlive)
                pingThread.Interrupt();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}