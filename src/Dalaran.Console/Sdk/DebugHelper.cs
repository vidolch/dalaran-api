// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Serilog;

    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void HandleDebugSwitch(ref string[] args)
        {
            if (args.Length > 0 && string.Equals("--debug", args[0], StringComparison.OrdinalIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                Log.Debug("Waiting for debugger to attach. Press ENTER to continue");
                Log.Debug($"Process ID: {Process.GetCurrentProcess().Id}");
                Console.ReadLine();
            }
        }
    }
}
