using System;
using RaptorOS.Commands;
using Sys = Cosmos.System;

namespace RaptorOS;

public class Kernel : Sys.Kernel
{
    protected override void BeforeRun()
    {
        Console.Clear();
        Console.WriteLine("=========================");

        Console.WriteLine("RaptorOS Booted. Welcome");
        Console.WriteLine("Release: v0.1.0");
        Console.WriteLine("=========================");
    }


    protected override void Run() {
    }
}
