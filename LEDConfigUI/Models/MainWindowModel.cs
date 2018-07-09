using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using AbaciLabs.LEDConfig.Arduino;
using AbaciLabs.LEDConfig.LEDs;

namespace AbaciLabs.LEDConfig.UI.Models
{
    public class MainWindowModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Instance Members
        #region Events
        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        private readonly BackgroundWorker workerCommitSettings = new BackgroundWorker();
        private readonly BackgroundWorker workerDiscover = new BackgroundWorker();
        private readonly ArduinoNanoFactory arduinoFactory = new ArduinoNanoFactory();
        private ArduinoNano device = null;
        #endregion
        #region Instance Properties
        /// <summary>
        /// Currently busy
        /// </summary>
        public bool Busy
        {
            get { return this.workerDiscover.IsBusy || this.workerCommitSettings.IsBusy; }
        }
        /// <summary>
        /// Connected to a device
        /// </summary>
        public bool Connected
        {
            get { return this.device?.Connected ?? false; }
        }
        public string ConnectionStatusText
        {
            get
            {
                if (this.Connected)
                    return string.Format("Connected to {0}", this.device.Port);
                return "Disconnected";
            }
        }
        private LEDStrip _LEDs = new LEDStrip(32);
        public LEDStrip LEDs
        {
            get { return this._LEDs; }
            set
            {
                this._LEDs = value;
                this.OnPropertyChanged("LEDs");
            }
        }
        private int _SelectedDelay = 10;
        public int SelectedDelay
        {
            get { return this._SelectedDelay; }
            set
            {
                this._SelectedDelay = value;
                this.OnPropertyChanged("SelectedDelay");
                return;
            }
        }
        private PatternModes _SelectedPattern = PatternModes.Unknown;
        public PatternModes SelectedPattern
        {
            get { return this._SelectedPattern; }
            set
            {
                this._SelectedPattern = value;
                this.OnPropertyChanged("SelectedPattern");
            }
        }
        #endregion
        #region Instance Methods
        public MainWindowModel()
        {
            this.workerCommitSettings.DoWork += this.workerCommitSettings_DoWork;
            this.workerCommitSettings.RunWorkerCompleted += workerCommitSettings_RunWorkerCompleted;

            this.workerDiscover.DoWork += this.workerDiscover_DoWork;
            this.workerDiscover.RunWorkerCompleted += this.workerDiscover_RunWorkerCompleted;
            return;
        }


        public void CommitSettings()
        {
            this.workerCommitSettings.RunWorkerAsync();
            this.OnPropertyChanged("Busy");
            return;
        }

        private void Disconnect()
        {
            this.device?.Dispose();
            this.device = null;
            return;
        }
        public void Discover()
        {
            this.Disconnect();
            this.workerDiscover.RunWorkerAsync();
            this.OnPropertyChanged("Busy");
            return;
        }
        private void OnPropertyChanged(string name)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }

        private void ShowError(Exception e)
        {
            MessageBox.Show(string.Format("Error while discovering Arduino device:\n{0}", e.Message), "ERROR",
                MessageBoxButton.OK, MessageBoxImage.Error);
            LogManager.Log.Error("Error while discovering Arduino device", e);
            return;
        }

        private void UpdateSelectedValues()
        {
            this.SelectedDelay = this.device?.Delay ?? 10;
            this.SelectedPattern = this.device?.PatternMode ?? PatternModes.Unknown;
            return;
        }
        #endregion
        #region Instance Events
        private void workerCommitSettings_DoWork(object sender, DoWorkEventArgs e)
        {
            FirmwareCommand command = new FirmwareCommand(this.SelectedPattern, this.SelectedDelay);
            this.device.SendCommand(command);
            this.device.RetrieveSettings();
            return;
        }
        private void workerCommitSettings_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.ShowError(e.Error);
            }
            this.UpdateSelectedValues();
            this.OnPropertyChanged("Busy");
            return;
        }
        private void workerDiscover_DoWork(object sender, DoWorkEventArgs e)
        {
            Task<ArduinoNano> task = this.arduinoFactory.Search();
            task.Wait(TimeSpan.FromSeconds(10.0));
            e.Result = task.Result;
            return;
        }
        private void workerDiscover_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.device = null;
                this.ShowError(e.Error);
            }
            else
            {
                this.device = e.Result as ArduinoNano;
            }
            this.UpdateSelectedValues();
            this.OnPropertyChanged("Busy");
            this.OnPropertyChanged("Connected");
            this.OnPropertyChanged("ConnectionStatusText");
            return;
        }

        #endregion
    }
}
