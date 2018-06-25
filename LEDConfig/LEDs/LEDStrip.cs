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
    public class LEDStrip : INotifyCollectionChanged
    {
        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public ObservableCollection<LEDColor> Colors { get; } = new ObservableCollection<LEDColor>();
        private int _LEDCount = 32;
        public int LEDCount
        {
            get { return this._LEDCount; }
            set
            {
                this._LEDCount = value;
                this.InitializeColors();
            }
        }

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
        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(this.CollectionChanged != null)
                this.CollectionChanged(sender, e);
            return;
        }
    }
}
