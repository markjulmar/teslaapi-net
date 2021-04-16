using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class LockUnlockCommand : CommandOption<CommandParameters>
    {
        public LockUnlockCommand(Func<Task<TeslaClient>> getClientAsync) : base(null, getClientAsync)
        {
        }

        public override Command CreateCommand()
        {
            return new Command("doors", "Lock/Unlock the doors")
            {
                new Command("lock", "Lock the doors")
                {
                    Handler = CommandHandler.Create(async (CommandParameters cp) => await ExecuteLockAsync(cp))
                },
                new Command("unlock", "Unlock the doors")
                {
                    Handler = CommandHandler.Create(async (CommandParameters cp) => await ExecuteUnlockAsync(cp))
                }
            };
        }

        private async Task ExecuteLockAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Locking doors on {vehicle.Id}... ");
            bool rc = await vehicle.LockDoorsAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }

        private async Task ExecuteUnlockAsync(CommandParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);
            Console.Write($"Unlocking doors on {vehicle.Id}... ");
            bool rc = await vehicle.UnlockDoorsAsync();
            Console.WriteLine(rc ? "OK" : "Failed");
        }
    }
}
