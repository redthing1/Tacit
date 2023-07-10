using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Minlog;
using Tacit.Tests.Framework.Utility;
using Xunit;

namespace Tacit.Demos.Examples.UtilityAnt; 

[Command("utility ant", Description = "Ants demo with Utility AI")]
public class UtilityAntDemoCommand : ICommand {
    public ValueTask ExecuteAsync(IConsole console) {
        Console.WriteLine("mind demos");

        var log = new Logger(Verbosity.Information);
        log.Verbosity = Verbosity.Trace;
        log.Sinks.Add(new Logger.ConsoleSink());

        log.Info("testing cake game");
        // test cake game
        var game = new CakeGame();
        var result = game.Run(100000);
        Assert.True(result);
        log.Info("cake game success");
        
        return default;
    }
}