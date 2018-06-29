using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AbaciLabs.LEDConfig.LEDs
{
    /// <summary>
    /// Strip of LEDs
    /// </summary>
    public class LEDStrip : INotifyCollectionChanged
    {
        #region Instance Members
        /// <summary>
        /// Collection change occurred
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion
        #region Instance Properties
        /// <summary>
        /// Collection of LED color descriptors
        /// </summary>
        public ObservableCollection<LEDColor> Colors { get; } = new ObservableCollection<LEDColor>();
        private int _LEDCount = 32;
        /// <summary>
        /// Number of LEDs in the strip
        /// </summary>
        public int LEDCount
        {
            get { return this._LEDCount; }
            set
            {
                this._LEDCount = value;
                this.InitializeColors();
            }
        }
        #endregion
        #region Instance Methods
        /// <summary>
        /// Create a new LED strip with the specified number of LEDs
        /// </summary>
        /// <param name="count">Number of LEDs</param>
        public LEDStrip(int count)
        {
            this.Colors.CollectionChanged += this.Colors_CollectionChanged;
            this.LEDCount = count;
            return;
        }
        private void InitializeColors()
        {
            while (this.Colors.Count > this.LEDCount)
                this.Colors.RemoveAt(this.Colors.Count - 1);
            while (this.Colors.Count < this.LEDCount)
                this.Colors.Add(new LEDColor(Color.FromRgb(255,0,0)));
            return;
        }
        #endregion
        #region Instance Events
        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(this.CollectionChanged != null)
                this.CollectionChanged(sender, e);
            return;
        }
        #endregion
    }
}
