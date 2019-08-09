namespace UnityEngine
{
    public static partial class ColorExtensions 
    {
        public static Color Fade(this Color color, float opacity)
        {
            return new Color(color.r, color.g, color.b, opacity);
        }
    }
}
