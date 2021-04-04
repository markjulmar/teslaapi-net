using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
#if false
            var api = new TeslaClient();
            //DebugLogging(api);
            
            string email = "elon@tesla.com";
            string password = "spacex-roxx";
            // Pass email, password and optional mfa resolver for Multi-Factor auth.
            // Can return either passcode from auth app, or backup passcode from MFA.
            await api.LoginAsync(email, password);
#else
            string accessToken = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tesla-token.txt"));
            var api = TeslaClient.CreateFromToken(accessToken);
            DebugLogging(api);
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
                while (!await myCar.WakeupAsync() && count++ < 10)
                    await Task.Delay(5000);
            }

            // Test state APIs
            Console.WriteLine($"Mobile access enabled: {await myCar.IsMobileAccessEnabledAsync()}");

            Console.WriteLine(await myCar.GetChargeStateAsync());
            Console.WriteLine(await myCar.GetClimateStateAsync());
            Console.WriteLine(await myCar.GetDriveStateAsync());
            Console.WriteLine(await myCar.GetGuiSettingsAsync());
            Console.WriteLine(await myCar.GetVehicleStateAsync());
            Console.WriteLine(await myCar.GetVehicleConfigurationAsync());

            var nearbyChargers = await myCar.GetNearbyChargingStations();
            Console.WriteLine(nearbyChargers);
            Console.WriteLine(nearbyChargers.DestinationCharging.FirstOrDefault()?.ToString());
            Console.WriteLine(nearbyChargers.Superchargers.FirstOrDefault()?.ToString());

            var allVehicleData = await myCar.GetAllVehicleDataAsync();
            Console.WriteLine(allVehicleData.VIN);

            await myCar.HonkHorn();
        }

        static void DebugLogging(TeslaClient api)
        {
            // Optional debugging - traces all HTTP traffic to/from.
            api.TraceLog = (level,s) =>
            {
                var c = Console.ForegroundColor;

                switch (level)
                {
                    case LogLevel.Info: Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevel.Query: Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogLevel.Response: Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogLevel.RawData: Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }

                if (level == LogLevel.RawData)
                {
                    try
                    {
                        var doc = JsonDocument.Parse(s);
                        s = JsonSerializer.Serialize(doc, new JsonSerializerOptions {WriteIndented = true});
                    }
                    catch
                    {
                    }
                }
                
                try
                {
                    Console.WriteLine(s);
                }
                finally
                {
                    Console.ForegroundColor = c;
                }
            };
        }
    }
}
