using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class WakeOptions : CommandParameters
    {
        public int Wait { get; set; }
    }

    public class WakeCommand : CommandOption<WakeOptions>
    {
        public WakeCommand(Func<Task<TeslaClient>> getClientAsync)
            : base(BuildCommand, getClientAsync)
        {
        }

        private static Command BuildCommand() => new("wake", "Wake up a sleeping car") {
                new Option<int>(new[] { "--wait", "-w" }, "wait time in seconds, defaults to 0."),
                new Option<long>(new[] { "--id", "-i" }, "id of the vehicle to work with, defaults to first one.")
            };

        protected override ICommandHandler Handler => CommandHandler.Create(async (WakeOptions options) => await ExecuteAsync(options));
    
        protected override async Task ExecuteAsync(WakeOptions options)
        {
            var vehicle = await GetVehicleAsync(options);

            Console.Write($"Waking up {vehicle.Id}... ");
            var rc = await vehicle.WakeupAsync(options.Wait);
            Console.WriteLine(rc ? "OK" : "Timed out");
        }
    }
}
