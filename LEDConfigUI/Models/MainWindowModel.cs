using System.Collections.Specialized;
using System.ComponentModel;
using AbaciLabs.LEDConfig.LEDs;

namespace AbaciLabs.LEDConfig.UI.Models
{
    public class MainWindowModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Events
        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Instance Properties
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
        #endregion
        #region Instance Methods
        public MainWindowModel()
        {
            return;
        }
        private void OnPropertyChanged(string name)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
        #endregion
    }
}
