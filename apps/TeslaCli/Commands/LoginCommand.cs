using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public sealed class LoginCommand
    {
        private readonly Func<AccessToken, Task> setAccessTokenAsync;

        public LoginCommand(Func<AccessToken, Task> setAccessTokenAsync)
        {
            this.setAccessTokenAsync = setAccessTokenAsync;
        }

        public Command CreateCommand()
        {
            var loginCommand = new Command("login", "Sign in to the Tesla API with a username/password.")
            {
                new Argument<string>("email", "Email account"),
                new Option<string>(new[] { "--password", "-p" }, "Password"),
                new Option<string>(new[] { "--code", "-c" }, "Multifactor auth passcode"),
                new Option<string>(new[] { "--backup", "-b" }, "Multifactor auth backup code")
            };

            loginCommand.Handler = CommandHandler.Create(
                async (string email, string password, string passcode, string backupCode) =>
                {
                    try
                    {
                        await ExecuteAsync(email, password, passcode, backupCode);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return 1;
                    }
                    return 0;
                });

            return loginCommand;
        }

        private string PromptForPassword()
        {
            Console.Write($"\r\nEnter password: ");
            return Console.ReadLine();
        }

        public async Task ExecuteAsync(string email, string password, string passcode, string backupCode)
        {
            Console.WriteLine($"Logging into tesla.com as {email}");

            if (string.IsNullOrWhiteSpace(password))
            {
                password = PromptForPassword();
                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.Error.WriteLine("Must enter a valid password.");
                    return;
                }
            }

            var client = new TeslaClient();

            (string, string) mfaResolver()
            {
                if (passcode != null || backupCode != null)
                {
                    return (passcode != null)
                        ? (passcode, null)
                        : (null, backupCode);
                }

                return GetMfaCode();
            }

            try
            {
                var token = await client.LoginAsync(email, password, mfaResolver);
                if (token != null)
                {
                    await this.setAccessTokenAsync(token);
                }
            }
            catch (TeslaAuthenticationException ex)
            {
                Console.Error.WriteLine($"Authentication failed: {ex.Message}");
            }
        }

        private (string passcode, string backupCode) GetMfaCode()
        {
            Console.WriteLine();
            Console.WriteLine("Account has multi - factor authentication turned on.");
            int index = Utilities.Prompt("Select MFA response type:",
                new string[] {"Passcode", "Backup passcode" });

            Console.Write("Enter code:");
            string value = Console.ReadLine();

            return index == 0 ? (value, null) : (null, value);
        }
    }
}
