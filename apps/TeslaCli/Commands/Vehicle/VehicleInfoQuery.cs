using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class VehicleStatusQuery : QueryOption<QueryParameters>
    {
        public VehicleStatusQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("status", "Get the current status for a vehicle.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters qp)
        {
            var vehicle = await GetVehicleAsync(qp);

            var info = await vehicle.GetVehicleStateAsync();
            var output = new OutputDefinition<VehicleState>(info);

            output.Add("Name", car => car.VehicleName);
            output.Add("Car Version", car => car.SoftwareUpdate.Version);
            output.Add("Has Update", car => string.IsNullOrEmpty(car.SoftwareUpdate.Status) ? "No" : car.SoftwareUpdate.Status);
            output.Add("Odometer", car => Math.Round(car.Odometer,2), " mi");
            output.Add("Locked", car => car.Locked);
            output.Add("Sentry On", car => car.SentryMode);
            output.Add("Center Display", car => car.CenterDisplayState);
            output.Add("User Present", car => car.IsUserPresent);
            output.Add("Driver Door", car => car.FrontDriverDoorOpen ? "Open" : "Closed");
            output.Add("Passenger Door", car => car.FrontPassengerDoorOpen ? "Open" : "Closed");
            output.Add("Driver Window", car => car.FrontDriverWindowOpen ? "Open" : "Closed");
            output.Add("Passenger Window", car => car.FrontPassengerWindowOpen ? "Open" : "Closed");
            output.Add("Rear Driver Door", car => car.RearDriverDoorOpen ? "Open" : "Closed",showInStyles: OutputStyleNoTable);
            output.Add("Rear Passenger Door", car => car.RearPassengerDoorOpen ? "Open" : "Closed", showInStyles: OutputStyleNoTable);
            output.Add("Rear Driver Window", car => car.RearDriverWindowOpen ? "Open" : "Closed", showInStyles: OutputStyleNoTable);
            output.Add("Rear Passenger Window", car => car.RearPassengerWindowOpen ? "Open" : "Closed", showInStyles: OutputStyleNoTable);
            output.Add("Front Trunk", car => car.FrontTrunkOpen ? "Open" : "Closed");
            output.Add("Rear Trunk", car => car.RearTrunkOpen ? "Open" : "Closed");


            output.Render(qp.Output);
        }
    }
}
