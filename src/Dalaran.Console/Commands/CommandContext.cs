// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Commands
{
    using Dalaran.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;

    public class CommandContext
    {
        public CommandContext(
            IConsole console,
            IReporter reporter,
            ICommandDataRepository repository)
        {
            this.Console = console;
            this.Reporter = reporter;
            this.Repository = repository;
        }

        public IConsole Console { get; }

        public IReporter Reporter { get; }

        public ICommandDataRepository Repository { get; }
    }
}
