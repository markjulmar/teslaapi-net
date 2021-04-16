using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class ChargeInfoQuery : QueryOption<QueryParameters>
    {
        public ChargeInfoQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("chargeinfo", "Get the current charge information for a vehicle.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters qp)
        {
            var vehicle = await GetVehicleAsync(qp);

            var chargeStatus = await vehicle.GetChargeStateAsync();
            var output = new OutputDefinition<ChargeState>(chargeStatus);
            output.Add("Battery Level", cs => cs.BatteryLevel, "%");
            output.Add("Battery Range", cs => cs.RatedBatteryRange, " mi");
            output.Add("Charging State", cs => cs.ChargingState.ToCamelCase());
            output.Add("Estimated Battery Range", cs => cs.EstBatteryRange, " mi");
            output.Add("Charge Port", cs => cs.ChargePortDoorOpen ? "Open" : "Closed");
            output.Add("Charge Port Latch", cs => cs.ChargePortLatch.ToCamelCase());
            output.Add("Charge Limit SOC", cs => cs.ChargeLimitSoc, "%");

            if (chargeStatus.IsCharging)
            {
                output.Add("Charger voltage", cs => cs.ChargerVoltage, " V");
                output.Add("Charger Actual Current", cs => cs.ChargerActualCurrent, " A");
                output.Add("Charger Power", cs => cs.ChargerPower, " kW");
                output.Add("Charge Rate", cs => cs.ChargeRate, " km/h");

                output.Add("Added Ideal", cs => cs.ChargeMilesAddedIdeal, " mi", showInStyles: OutputStyleNoTable);
                output.Add("Added Rated", cs => cs.ChargeMilesAddedRated, " mi", showInStyles: OutputStyleNoTable);
                output.Add("Time to full charge", $"{chargeStatus.MinutesToFullCharge / 60}:{chargeStatus.MinutesToFullCharge % 60}");
            }

            output.Render(qp.Output);
        }
    }
}
