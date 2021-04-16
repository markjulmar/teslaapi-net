using System;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Some basic converters for Tesla units.
    /// </summary>
    public static class UnitConversion
    {
        /// <summary>
        /// Convert celsius to fahrenheit degrees
        /// </summary>
        /// <param name="temperature">Temperature to convert.</param>
        /// <returns>Converted value</returns>
        public static double CelsiusToFahrenheit(double temperature) => temperature * 9d / 5d + 32;

        /// <summary>
        /// Convert fahrenheit to celsius degrees
        /// </summary>
        /// <param name="temperature">Temperature to convert.</param>
        /// <returns>Converted value</returns>
        public static double FahrenheitToCelsius(double temperature) => (temperature - 32) * 5d / 9d;

        public static string HeadingToDirection(double heading)
        {
            if (heading < 0 || heading > 360)
                throw new ArgumentOutOfRangeException(nameof(heading));

            switch (heading)
            {
                case >= 0 and <= 11.25:
                case > 348.75 and <= 360:
                    return "N";
                case > 11.25 and <= 33.75:
                    return "NNE";
                case > 33.75 and <= 56.25:
                    return "NE";
                case > 56.25 and <= 78.75:
                    return "ENE";
                case > 78.75 and <= 101.25:
                    return "E";
                case > 101.25 and <= 123.75:
                    return "ESE";
                case > 123.75 and <= 146.25:
                    return "SE";
                case > 146.25 and <= 168.75:
                    return "SSE";
                case > 168.75 and <= 191.25:
                    return "S";
                case > 191.25 and <= 213.75:
                    return "SSW";
                case > 213.75 and <= 236.25:
                    return "SW";
                case > 236.25 and <= 258.75:
                    return "WSW";
                case > 258.75 and <= 281.25:
                    return "W";
                case > 281.25 and <= 303.75:
                    return "WNW";
                case > 303.75 and <= 326.25:
                    return "NW";
                case > 326.25 and <= 348.75:
                    return "NNW";
            }

            return "";
        }
    }
}