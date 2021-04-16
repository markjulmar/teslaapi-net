using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli.Commands
{
    public class CommandParameters
    {
        public long Id { get; set; }
    }

    public class QueryParameters : CommandParameters
    {
        public OutputStyle Output { get; set; }
    }

    public abstract class QueryOption<T> : CommandOption<T>
        where T : QueryParameters
    {
        public QueryOption(
            string command, string description,
            Func<Task<TeslaClient>> getClientAsync) : base(command, description, getClientAsync)
        {
        }

        public QueryOption(
            Func<Command> commandBuilder,
            Func<Task<TeslaClient>> getClientAsync) : base(commandBuilder, getClientAsync)
        {
        }

        public override Command CreateCommand()
        {
            var command = base.CreateCommand();
            command.AddOption(new Option<OutputStyle>(new[] { "--output", "-o" }, "Output type, defaults to text"));
            return command;
        }
    }

    public abstract class CommandOption<T> where T : CommandParameters
    {
        internal const OutputStyle OutputStyleNoTable = OutputStyle.Csv | OutputStyle.JSON | OutputStyle.Text;

        private readonly Func<Task<TeslaClient>> getClientAsync;
        private readonly Func<Command> commandBuilder;

        public CommandOption(
            string command, string description,
            Func<Task<TeslaClient>> getClientAsync)
        {
            this.commandBuilder = () => CreateNoParamCommand(command, description);
            this.getClientAsync = getClientAsync;
        }

        public CommandOption(
            Func<Command> commandBuilder,
            Func<Task<TeslaClient>> getClientAsync)
        {
            this.commandBuilder = commandBuilder;
            this.getClientAsync = getClientAsync;
        }

        private Command CreateNoParamCommand(string command, string description) => new(command, description);

        protected async Task<TeslaClient> GetClientAsync()
        {
            var clientFunc = getClientAsync.Invoke();
            if (clientFunc == null)
                throw new Exception("Error: you must use the 'login' command to get access to the Tesla API.");

            var client = await clientFunc;
            client.TraceLog = Utilities.JsonCatcher;
            return client;
        }

        protected async Task<Vehicle> GetVehicleAsync(T options)
        {
            var client = await GetClientAsync();

            Vehicle vehicle;
            if (options.Id == 0)
            {
                vehicle = (await client.GetVehiclesAsync()).FirstOrDefault();
            }
            else
            {
                vehicle = await client.GetVehicleAsync(options.Id);
            }

            if (vehicle == null)
                throw new Exception("No matching car found.");

            return vehicle;
        }

        public virtual Command CreateCommand()
        {
            var command = commandBuilder.Invoke();
            if (command.Handler == null)
                command.Handler = this.Handler;
            return command;
        }

        protected virtual ICommandHandler Handler => CommandHandler.Create(
            async(T parameters) =>
            {
                try
                {
                    await ExecuteAsync(parameters);
                }
                catch (SleepingException)
                {
                    Console.Error.WriteLine("Vehicle is asleep and not accepting commands. Use 'wake' to wakeup the vehicle.");
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    return 2;
                }

                return 0;
            });

        protected virtual Task ExecuteAsync(T parameters)
            => Task.CompletedTask;
    }
}