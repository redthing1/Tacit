using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Minlog;
using Tacit.Layers.Game;

namespace Tacit.Demos.Examples.DGUBarfight;

[Command("dgu barfight", Description = "Demo of a barfight using DGU AI")]
public class DGUBarfightDemoCommand : ICommand {
    private readonly Logger _rootLog;
    private readonly ILogger _log;

    public DGUBarfightDemoCommand(Logger rootLog) {
        _rootLog = rootLog;
        _log = rootLog.For<DGUBarfightDemoCommand>();
    }

    public async ValueTask ExecuteAsync(IConsole console) {
        var game = new BarfightGame(_rootLog);

        while (true) {
            var status = await game.Update();
            if (status != SimpleGame.Status.Continue) {
                _log.Info($"Stopping game simulation with status: {status}");
                break;
            }
        }
    }
}