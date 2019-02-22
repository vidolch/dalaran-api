// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Persistence
{
    public interface ICommandDataRepository
    {
        CommandData GetCommandData();

        void SetCommandData(CommandData commandData);
    }
}
