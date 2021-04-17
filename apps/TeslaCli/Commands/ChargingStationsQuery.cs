using System;
using System.Linq;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class ChargingStationsQuery : QueryOption<QueryParameters>
    {
        public ChargingStationsQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("chargers", "Find nearby charging stations", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters parameters)
        {
            var vehicle = await GetVehicleAsync(parameters);

            var chargingStations = await vehicle.GetNearbyChargingStationsAsync();
            var dcOutput = new OutputDefinition<ChargerLocation>(chargingStations.DestinationChargers
                                                    .Union<ChargerLocation>(chargingStations.Superchargers
                                                        .Where(s => !s.SiteClosed))
                                                    .OrderBy(c => c.DistanceMiles));
            dcOutput.Add("Type", ch => ch.Type.ToCamelCase());
            dcOutput.Add("Name", ch => ch.Name);
            dcOutput.Add("Distance", ch => Math.Round(ch.DistanceMiles,1), " mi");
            dcOutput.Add("Location", ch => $"{ch.Location.Latitude}, {ch.Location.Longitude}");
            dcOutput.Add("Available", ch => 
                ch is Supercharger sc ? $"{sc.AvailableStalls}/{sc.TotalStalls}": "n/a");
            dcOutput.Render(parameters.Output);
        }
    }
}