// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Commands
{
    using System.Threading.Tasks;

    public interface ICommand
    {
        Task ExecuteAsync(CommandContext context);
    }
}
