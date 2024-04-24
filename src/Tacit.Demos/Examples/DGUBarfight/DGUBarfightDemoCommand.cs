using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Minlog;
using Tacit.Demos.Examples.DGUBarfight.AI;
using Tacit.Demos.Util;
using Tacit.Framework.DGU;
using Tacit.Layers.Game;

namespace Tacit.Demos.Examples.DGUBarfight;

[Command("dgu barfight", Description = "Demo of a barfight using DGU AI")]
public class DGUBarfightDemoCommand : ICommand {
    private readonly Logger _rootLog;
    private readonly ILogger _log;
    
    [CommandOption("steps", 's', Description = "Number of steps to run the simulation for")]
    public int Steps { get; set; } = 10;
    
    [CommandOption("interactive", 'i', Description = "Run the simulation in interactive mode")]
    public bool Interactive { get; set; } = false;

    public DGUBarfightDemoCommand(Logger rootLog) {
        _rootLog = rootLog;
        _log = rootLog.For<DGUBarfightDemoCommand>();
    }

    public async ValueTask ExecuteAsync(IConsole console) {
        var game = new BarfightGame(_rootLog);

        // create one person, and attach a doctor
        var bob = new DrunkPersonAgent("Bob", new BarfightEnvironment(game));
        bob.AttachDoctor(new MinlogDGUDoctor(_rootLog));
        bob.AttachPlanner(new DGUPlanner(new DGUPlanner.PlannerConfig(), bob));
        var joe = new DrunkPersonAgent("Joe", new BarfightEnvironment(game));
        // joe.AttachDoctor(new MinlogDGUDoctor(_rootLog));
        joe.AttachPlanner(new DGUPlanner(new DGUPlanner.PlannerConfig(), joe));
        game.AddPerson(bob);
        game.AddPerson(joe);

        if (Interactive) {
            _log.Info("Running game simulation in interactive mode");
        }

        while (true) {
            if (Interactive) {
                var input = await console.Input.ReadLineAsync();
                if (input == "q") {
                    _log.Info("Stopping game simulation");
                    break;
                }
            }
            var status = await game.Update();
            if (status != SimpleGame.Status.Continue) {
                _log.Info($"Stopping game simulation with status: {status}");
                break;
            }
            if (Steps > 0 && game.Steps >= Steps) {
                _log.Info($"Stopping game simulation after {Steps} steps");
                break;
            }
        }
    }
}