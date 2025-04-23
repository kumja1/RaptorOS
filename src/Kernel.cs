using System;
using RaptorOS.Commands;
using RaptorOS.Commands.Core;
using RaptorOS.Utils;
using Sys = Cosmos.System;

namespace RaptorOS;

public class Kernel : Sys.Kernel
{
    protected override void BeforeRun()
    {
        Console.Clear();
        Logger.Log("=========================");

        Logger.Log("RaptorOS Booted. Welcome");
        Logger.Log("Release: v0.1.0");
        Logger.Log("=========================");

        RegisterCommands();
        Logger.Log("Commands Registered");
    }

    private void RegisterCommands()
    {
        CommandManager.Instance.RegisterCommand(new EchoCommand());
    }

    protected override void Run()
    {
        CommandManager.Instance.Tick();
    }
}
