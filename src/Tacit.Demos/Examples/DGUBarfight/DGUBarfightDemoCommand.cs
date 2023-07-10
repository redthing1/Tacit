using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Minlog;

namespace Tacit.Demos.Examples.DGUBarfight;

[Command("dgu barfight", Description = "Demo of a barfight using DGU AI")]
public class DGUBarfightDemoCommand : ICommand {
    private readonly Logger _log;

    public DGUBarfightDemoCommand(Logger log) {
        _log = log;
    }

    public ValueTask ExecuteAsync(IConsole console) {
        throw new System.NotImplementedException();
    }
}