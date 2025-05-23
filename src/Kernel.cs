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
        Logger.Log("=========================");

        Logger.Log("RaptorOS Booted. Welcome");
        Logger.Log("Release: v0.1.0");
        Logger.Log("=========================");

        RegisterCommands();
        Logger.LogInfo("Commands Registered");
    }

    private void RegisterCommands()
    {
        CommandManager.Instance.RegisterCommand(new EchoCommand());
    }

    protected override void Run()
    {
        Console.Write("raptor> ");
        CommandManager.Instance.Tick();
    }

    protected override void AfterRun()
    {
        Logger.Log("=========================");
        Logger.Log("RaptorOS Shutting Down");
        Logger.Log("=========================");
        CommandManager.Instance.RemoveCommand("echo");
    }
}