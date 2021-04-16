using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class DriveStateQuery : QueryOption<QueryParameters>
    {
        public DriveStateQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("location", "Get the current driving status and location for a vehicle.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters qp)
        {
            var vehicle = await GetVehicleAsync(qp);

            var driveState = await vehicle.GetDriveStateAsync();
            var output = new OutputDefinition<DriveState>(driveState);

            output.Add("Time", ds => ds.GpsAsOf);
            output.Add("Latitude", ds => ds.Latitude);
            output.Add("Longitude", ds => ds.Longitude);
            output.Add("Heading", ds => ds.Heading);
            output.Add("Direction", ds => ds.Direction);
            output.Add("Power", ds => ds.Power, " kW");

            if (driveState.Power > 0)
            {
                output.Add("Shift state", ds => ds.ShiftState);
                output.Add("Speed", ds => ds.Speed, " mph");
            }

            output.Render(qp.Output);
        }
    }
}
