using System;
using Minlog;
using Tacit.Tests.Framework.Utility;
using Xunit;

namespace Tacit.Demos;

internal class Program {
    private static void Main(string[] args) {
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
    }
}