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
        public static double CelsiusToFahrenheit(double temperature) => temperature * 9 / 5 + 32;

        /// <summary>
        /// Convert fahrenheit to celsius degrees
        /// </summary>
        /// <param name="temperature">Temperature to convert.</param>
        /// <returns>Converted value</returns>
        public static double FahrenheitToCelsius(double temperature) => (temperature - 32) * 5 / 9;
    }
}