using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeslaApi;

namespace TeslaApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if USE_EMAIL
            string email = "elon@tesla.com";
            string password = "spacex-rox";
            string mfa_passcode = "12345";
            string backup_passcode = null;

            var api = TeslaClient.Create();
            // Pass email, password and optional mfa resolver for Multi-Factor auth.
            // Can return either passcode from auth app, or backup passcode from MFA.
            await api.LoginAsync(email, password, () => (mfa_passcode, backup_passcode));
#else
            string accessToken = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tesla-token.txt"));
            var api = TeslaClient.CreateFromToken(accessToken);
#endif

#if false
            // Optional debugging - traces all HTTP traffic to/from.
            api.TraceLog = s =>
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                try
                {
                    Console.WriteLine(s);
                }
                finally
                {
                    Console.ForegroundColor = c;
                }
            };
#endif

            // Get all vehicles on this account
            var all = await api.GetVehiclesAsync();
            foreach (var item in all)
                Console.WriteLine(item);

            // Test vehicle apis
            var myCar = await api.GetVehicleAsync(all.First().Id);
            Console.WriteLine(myCar.OptionCodes);

            // Loop until the car wakes up.
            int count = 0;
            if (myCar.State != "online")
            {
                myCar = await api.WakeupAsync(myCar.Id);
                while (myCar.State != "online")
                {
                    await Task.Delay(1000);
                    myCar = await api.WakeupAsync(myCar.Id);
                    if (count++ > 10)
                    {
                        Console.WriteLine("Could not wake up the car.");
                        return;
                    }
                }
            }

            // Test state APIs
            Console.WriteLine($"Mobile access enabled: {await api.IsMobileAccessEnabledAsync(myCar.Id)}");

            Console.WriteLine(await api.GetChargeStateAsync(myCar.Id));
            Console.WriteLine(await api.GetClimateStateAsync(myCar.Id));
            Console.WriteLine(await api.GetDriveStateAsync(myCar.Id));
            Console.WriteLine(await api.GetGuiSettingsAsync(myCar.Id));
            Console.WriteLine(await api.GetVehicleStateAsync(myCar.Id));
            Console.WriteLine(await api.GetVehicleConfigurationAsync(myCar.Id));

            var nearbyChargers = await api.GetNearbyChargingStations(myCar.Id);
            Console.WriteLine(nearbyChargers);
            Console.WriteLine(nearbyChargers.DestinationCharging.FirstOrDefault()?.ToString());
            Console.WriteLine(nearbyChargers.Superchargers.FirstOrDefault()?.ToString());
        }
    }
}
