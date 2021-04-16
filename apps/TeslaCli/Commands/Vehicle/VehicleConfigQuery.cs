using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class VehicleConfigQuery : QueryOption<QueryParameters>
    {
        public VehicleConfigQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("config", "Get the vehicle configuration.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters qp)
        {
            var vehicle = await GetVehicleAsync(qp);

            var info = await vehicle.GetVehicleConfigurationAsync();
            var output = new OutputDefinition<VehicleConfiguration>(info);

            output.Add("Car Type", ct => ct.CarType);
            output.Add("Exterior Color", ct => ct.ExteriorColor);
            output.Add("Wheel Type", ct => ct.WheelType);
            output.Add("Roof Color", ct => ct.RoofColor);
            output.Add("Spoiler Type", ct => ct.SpoilerType);
            output.Add("Charge Port Type", ct => ct.ChargePortType);

            output.Render(qp.Output);
        }
    }
}
