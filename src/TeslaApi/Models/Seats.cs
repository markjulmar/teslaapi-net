namespace Julmar.TeslaApi.Models
{
    /// <summary>
    /// Seat
    /// </summary>
    public enum Seat
    {
        Driver = 0,
        Passenger = 1,
        RearLeft = 2,
        RearCenter = 4,
        RearRight = 5
    }
    
    /// <summary>
    /// Seat heater settings
    /// </summary>
    public enum SeatHeater
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }
}