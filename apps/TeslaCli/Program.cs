using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Julmar.TeslaApi;
using System.CommandLine;
using TeslaCli.Commands;

namespace TeslaCli
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return new Program().Run(args);
        }

        async Task Run(string[] args)
        {
            await BuildCommandGraph().InvokeAsync(args);
        }

        private async Task<TeslaClient> CreateClient()
        {
            AccessToken tokenInfo = await Utilities.ReadTokenInfo();
            if(tokenInfo?.Token != null)
            {
                if (tokenInfo.ExpirationTimeUtc <= DateTime.UtcNow.AddHours(1))
                {
                    var client = TeslaClient.CreateFromToken(tokenInfo.Token);
                    if (tokenInfo.RefreshToken != null)
                    {
                        client.AutoRefreshToken = async () =>
                        {
                            tokenInfo = await client.RefreshLoginAsync(tokenInfo.RefreshToken);
                            await Utilities.WriteTokenInfo(tokenInfo);
                            return tokenInfo.Token;
                        };
                    }
                    return client;
                }
                else if (tokenInfo.RefreshToken != null)
                {
                    var client = new TeslaClient();
                    tokenInfo = await client.RefreshLoginAsync(tokenInfo.RefreshToken);
                    await Utilities.WriteTokenInfo(tokenInfo);
                    return client;
                }
            }
            return null;
        }

        RootCommand BuildCommandGraph()
        {
            return new RootCommand
            {
                new LoginCommand(token => Utilities.WriteTokenInfo(token)).CreateCommand(),
                VehicleCommands.Build(CreateClient)
            };
        }
    }
}
