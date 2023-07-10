using System.Collections.Generic;
using System.Threading.Tasks;
using Minlog;
using Tacit.Layers.Game;

namespace Tacit.Demos.Examples.DGUBarfight; 

public class BarfightGame : SimpleGame {
    private readonly ILogger _log;
    public List<DrunkPerson> people = new();

    public BarfightGame(Logger log) {
        _log = log.For<BarfightGame>();
    }

    public override async Task<Status> Update() {
        await base.Update();
        _log.Info($"Step {Steps}");
        
        // update all the people
        foreach (var person in people) {
            await person.Update(Steps);
        }
        
        return Status.Continue;
    }
}