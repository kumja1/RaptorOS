using System;

namespace RaptorOS.Commands;

public class Echo : Command
{
public Echo()
{
    Name = "echo";
    Description = "Echos input";

}
public override void Execute(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: echo <message>");
        return;

    }
    Console.WriteLine(string.Join(" ", args[1..]));
}
}