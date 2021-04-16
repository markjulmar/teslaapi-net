using System;
using System.CommandLine;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public static class VehicleCommands
    {
        public static Command Build(Func<Task<TeslaClient>> getClientAsync)
        {
            return new Command("vehicle", "Perform commands on vehicles")
            {
                new ListQuery(getClientAsync).CreateCommand(),
                new WakeCommand(getClientAsync).CreateCommand(),
                new ChargeInfoQuery(getClientAsync).CreateCommand(),
                new DriveStateQuery(getClientAsync).CreateCommand(),
                new VehicleStatusQuery(getClientAsync).CreateCommand(),
                new VehicleConfigQuery(getClientAsync).CreateCommand(),
                new HonkHornCommand(getClientAsync).CreateCommand(),
                new FlashLightsCommand(getClientAsync).CreateCommand(),
                new LockUnlockCommand(getClientAsync).CreateCommand(),
                new ClimateCommand(getClientAsync).CreateCommand()
            };
        }
    }
}
