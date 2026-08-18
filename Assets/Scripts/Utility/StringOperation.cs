using System.Globalization;
using UnityEngine;


namespace Utility.String
{
    public class StringOperation
    {
        static public bool TryParseHexColor(string str, out Color color)
        {
            return ColorUtility.TryParseHtmlString(str, out color);
        }

        static public bool TryParseFloat(string str, out float result)
        {
            return float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        static public bool TryParseVector2(string str, out Vector2 result)
        {
            result = Vector2.zero;

            string[] parts = str.Split(',');

            if (parts.Length != 2)
                return false;

            if (!TryParseFloat(parts[0], out float x))
                return false;

            if (!TryParseFloat(parts[1], out float y))
                return false;

            result = new Vector2(x, y);
            return true;
        }
    }
}