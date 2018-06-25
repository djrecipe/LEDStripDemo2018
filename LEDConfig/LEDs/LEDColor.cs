using System.Windows.Media;

namespace AbaciLabs.LEDConfig.LEDs
{
    public class LEDColor
    {
        #region Instance Properties
        public Brush Brush
        {
            get
            {
                return new SolidColorBrush(this.Value);
            }
        }
        public Color Value { get; set; } = Color.FromRgb(255,0,0);
        #endregion
        #region Instance Methods
        public LEDColor(Color color)
        {
            this.Value = color;
        }
        #endregion
    }
}
