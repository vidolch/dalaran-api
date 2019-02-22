// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Commands
{
    using Dalaran.Console.Commands.Login;
    using Dalaran.Console.Sdk;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;

    public class CommandLineOptions
    {
        public CommandOption Help { get; private set; }

        public CommandOption Verbose { get; private set; }

        public ICommand Command { get; set; }

        public static CommandLineOptions Parse(string[] args, IConsole console, ServiceProvider services)
        {
            var options = new CommandLineOptions();

            var app = new CommandLineApplication(console);

            options.Verbose = app.VerboseOption();
            options.Help = app.HelpOption();

            // commands
            app.Command("login", command => LoginCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());

            if (app.Execute(args) != 0)
            {
                // when command line parsing error in subcommand
                return null;
            }

            return options;
        }
    }
}
