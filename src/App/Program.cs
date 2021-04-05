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
            var tokenResponse = await api.LoginAsync(email, password);
            
            // TODO: save off token response?
#else
            string accessToken = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tesla-token.txt"));
            var api = TeslaClient.CreateFromToken(accessToken);
            DebugLogging(api);

            //var response = await api.RefreshLoginAsync("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ilg0RmNua0RCUVBUTnBrZTZiMnNuRi04YmdVUSJ9.eyJpc3MiOiJodHRwczovL2F1dGgudGVzbGEuY29tL29hdXRoMi92MyIsImF1ZCI6Imh0dHBzOi8vYXV0aC50ZXNsYS5jb20vb2F1dGgyL3YzL3Rva2VuIiwiaWF0IjoxNjE3MzE5MDk3LCJzY3AiOlsib3BlbmlkIiwib2ZmbGluZV9hY2Nlc3MiXSwiZGF0YSI6eyJ2IjoiMSIsImF1ZCI6Imh0dHBzOi8vb3duZXItYXBpLnRlc2xhbW90b3JzLmNvbS8iLCJzdWIiOiJhMGUwNDE4Yi02YjIwLTRhN2ItOGE5NS1lY2JkYTg4YzMxMGEiLCJzY3AiOlsib3BlbmlkIiwiZW1haWwiLCJvZmZsaW5lX2FjY2VzcyJdLCJhenAiOiJvd25lcmFwaSIsImFtciI6WyJwd2QiLCJtZmEiLCJvdHAiXSwiYXV0aF90aW1lIjoxNjE3MzE5MDk3fX0.wHJ68BH9lg2fMfbYQcx3Q-S3xCJ0MyYkJQa17Tsx8PBZ3sfz8ZhZ3ey_eT9YiM5eQYOm1HwBIQ7jqBniR2Itnlu4Yywt0fK3bG11_IWiHuw27Ttqjvb2-OlZEEM2OZD227GeDUEpBuCFJIohUdzxyJWqSuUeFcE_F4KJR-WW-l8H8Y_8eHxWJSsRVyPMzxh_1vzU62J4Nc8cHRBlituG5dIxj39ikpaZFoU4uvE2LPKbtOG3Ot2R3x2rmcmGztFhVvSBe6cDnzNe9xzC8z51Osm6vgr9nFIMXW-ck2wlaCDF8MCtNELFaz1392ZBd7d_kPyYsJ4c5hT9u5c0zljLVT480G3LiWEW_FgfdXN1rztokd-lzRFX_lilU4X7Zzn7un6Z2VXTWikR-pUn6F4D5fR--eM1NAzDzmIyAhSJZPM3ZYvwxv-4CyyjRVw2xr6uxlOmtmRcfnZ1STx6G0fXZW2kPABNDNfBwQgwaF5VnhSwkU8S2cU9s_r-fkZ4UVIQKbv2PYlbWWSQnQOA8Kvg3r5MjYcocBFjjR1ZsXKyiH0eNrDTuNkSeuFBkNv5B7rCQ5sBUBFKNPL-B3aGc0tcna1oZw3w3knI2NiO3XxR42vU9M0d0MBcb1dJ84Op13uZuG2_WZQ8UxOArVLVkB_AVEggH70aJ3UFpLJI8w_ciek");
            //Console.WriteLine(response.Token);
            //return;
#endif

            // Get all vehicles on this account
            var all = await api.GetVehiclesAsync();
            foreach (var item in all)
                Console.WriteLine(item);
            
            // Test vehicle apis
            var myCar = await api.GetVehicleAsync(all.First().Id);
            //Console.WriteLine(myCar.OptionCodes);

            // Loop until the car wakes up.
            int count = 0;
            if (myCar.State != "online")
            {
                while (!await myCar.WakeupAsync() && count++ < 10)
                    await Task.Delay(5000);
            }

            // Test state APIs
            //Console.WriteLine($"Mobile access enabled: {await myCar.IsMobileAccessEnabledAsync()}");

            Console.WriteLine(await myCar.GetChargeStateAsync());
            // Console.WriteLine(await myCar.GetClimateStateAsync());
            // Console.WriteLine(await myCar.GetDriveStateAsync());
            // Console.WriteLine(await myCar.GetGuiSettingsAsync());
            // Console.WriteLine(await myCar.GetVehicleStateAsync());
            // Console.WriteLine(await myCar.GetVehicleConfigurationAsync());
            //
            // var nearbyChargers = await myCar.GetNearbyChargingStations();
            // Console.WriteLine(nearbyChargers);
            // Console.WriteLine(nearbyChargers.DestinationCharging.FirstOrDefault()?.ToString());
            // Console.WriteLine(nearbyChargers.Superchargers.FirstOrDefault()?.ToString());
            //
            // var allVehicleData = await myCar.GetAllVehicleDataAsync();
            // Console.WriteLine(allVehicleData.VIN);

            //await myCar.HonkHorn();
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
