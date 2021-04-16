using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class HonkHornCommand : CommandOption<CommandParameters>
    {
        public HonkHornCommand(Func<Task<TeslaClient>> getClientAsync)
            : base("honk", "Honk the horn", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Honking horn on {vehicle.Id}... ");
            bool rc = await vehicle.HonkHornAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }
    }
}
