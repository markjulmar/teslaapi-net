using System;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class ListQuery : QueryOption<QueryParameters>
    {
        public ListQuery(Func<Task<TeslaClient>> getClientAsync)
            : base("list", "List all the vehicles associated with the account.", getClientAsync)
        {
        }

        protected override async Task ExecuteAsync(QueryParameters qp)
        {
            var client = await GetClientAsync();
            var vehicles = await client.GetVehiclesAsync();
            if (vehicles.Count == 0)
                throw new Exception("No cars found on this account.");

            var output = new OutputDefinition<Vehicle>(vehicles);

            output.Add("Name", car => car.DisplayName);
            output.Add("Id", car => car.Id);
            output.Add("VIN", car => car.VIN);
            output.Add("State", car => car.State.ToCamelCase());

            output.Render(qp.Output);
        }
    }
}
