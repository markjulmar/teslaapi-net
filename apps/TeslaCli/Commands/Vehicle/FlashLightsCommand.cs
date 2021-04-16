using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class FlashLightsCommand : CommandOption<CommandParameters>
    {
        public FlashLightsCommand(Func<Task<TeslaClient>> getClientAsync)
            : base("flash", "Flash the lights.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Flashing lights on {vehicle.Id}... ");
            bool rc = await vehicle.FlashLightsAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }
    }
}
