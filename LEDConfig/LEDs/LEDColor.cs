using System.Windows.Media;

namespace AbaciLabs.LEDConfig.LEDs
{
    /// <summary>
    /// Color description for a single LED
    /// </summary>
    public class LEDColor
    {
        #region Instance Properties
        /// <summary>
        /// Brush representing the color
        /// </summary>
        public Brush Brush
        {
            get
            {
                return new SolidColorBrush(this.Value);
            }
        }
        /// <summary>
        /// Raw color value
        /// </summary>
        public Color Value { get; set; } = Color.FromRgb(255,0,0);
        #endregion
        #region Instance Methods
        /// <summary>
        /// Create a new LED color descriptor using the specified color
        /// </summary>
        /// <param name="color">Color</param>
        public LEDColor(Color color)
        {
            this.Value = color;
        }
        #endregion
    }
}
