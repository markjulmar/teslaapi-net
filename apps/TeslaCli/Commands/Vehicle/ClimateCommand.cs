using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class ClimateCommand : CommandOption<CommandParameters>
    {
        public ClimateCommand(Func<Task<TeslaClient>> getClientAsync) : base(null, getClientAsync)
        {
        }

        public override Command CreateCommand()
        {
            var cmd = new Command("climate", "Control the climate system.")
            {
                new Command("on", "Turn on the climate system.")
                {
                    Handler = CommandHandler.Create(async (CommandParameters cp) => await ExecuteStartClimateAsync(cp))
                },
                new Command("off", "Turn off the climate system.")
                {
                    Handler = CommandHandler.Create(async (CommandParameters cp) => await ExecuteStopClimateAsync(cp))
                }
            };

            cmd.Handler = CommandHandler.Create(async (QueryParameters qp) => await ExecuteGetAsync(qp));

            var tempCmd = new Command("set", "Set the temperature for the climate system.")
            {
                new Argument<double>("temp", "Temperature in degrees.")
            };
            tempCmd.Handler = CommandHandler.Create(async (CommandParameters cp, double temp) => await ExecuteChangeTempAsync(cp, temp));

            cmd.AddCommand(tempCmd);

            return cmd;
        }

        private async Task ExecuteGetAsync(QueryParameters qp)
        {
            const string degrees = "° F";
            var vehicle = await GetVehicleAsync(qp);

            var climateState = await vehicle.GetClimateStateAsync();
            var output = new OutputDefinition<ClimateState>(climateState);

            output.Add("Climate on", cs => cs.IsClimateOn);
            output.Add("Inside temp", $"{UnitConversion.CelsiusToFahrenheit(climateState.InsideTemp)}", degrees);
            output.Add("Outside temp", $"{UnitConversion.CelsiusToFahrenheit(climateState.OutsideTemp)}", degrees);
            output.Add("Driver temp setting", $"{UnitConversion.CelsiusToFahrenheit(climateState.DriverTempSetting)}", degrees);
            output.Add("Passenger temp setting", $"{UnitConversion.CelsiusToFahrenheit(climateState.PassengerTempSetting)}", degrees);
            output.Add("Fan speed", cs => cs.FanSpeed);
            output.Add("Front defroster on", cs => cs.IsFrontDefrosterOn);
            output.Add("Rear defroster on", cs => cs.IsRearDefrosterOn);
            output.Add("Driver seat heater", cs => cs.DriverSeatHeater, showInStyles: OutputStyleNoTable);
            output.Add("Passenger seat heater", cs => cs.PassengerSeatHeater, showInStyles: OutputStyleNoTable);
            output.Add("Rear/left seat heater", cs => cs.RearLeftSeatHeater, showInStyles: OutputStyleNoTable);
            output.Add("Rear/right seat heater", cs => cs.RearRightSeatHeater, showInStyles: OutputStyleNoTable);
            output.Add("Rear/center seat heater", cs => cs.RearCenterSeatHeater, showInStyles: OutputStyleNoTable);

            output.Render(qp.Output);
        }

        private async Task ExecuteChangeTempAsync(CommandParameters cp, double temp)
        {
            var vehicle = await GetVehicleAsync(cp);
            Console.Write($"Setting the temperature on {vehicle.Id} to {temp} degrees ... ");
            bool rc = await vehicle.SetClimateControlTemperatureAsync(UnitConversion.FahrenheitToCelsius(temp));
            Console.WriteLine(rc ? "OK" : "Failed");
        }

        private async Task ExecuteStartClimateAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Turning on the climate system on {vehicle.Id}... ");
            bool rc = await vehicle.StartClimateControlAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }

        private async Task ExecuteStopClimateAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Turning off the climate system on {vehicle.Id}... ");
            bool rc = await vehicle.StopClimateControlAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }
    }
}
