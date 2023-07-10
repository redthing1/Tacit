using System.Collections.Generic;
using System.Threading.Tasks;
using Minlog;
using Tacit.Demos.Examples.DGUBarfight.AI;
using Tacit.Layers.Game;

namespace Tacit.Demos.Examples.DGUBarfight;

public record DrunkPersonStats(float Health, float Drunkenness);

public class BarfightGame : SimpleGame {
    private readonly ILogger _log;
    private readonly List<DrunkPerson> _people = new();

    public Dictionary<DrunkPerson, DrunkPersonStats> PersonStatsMap { get; } = new();

    public BarfightGame(Logger log) {
        _log = log.For<BarfightGame>();
    }

    public void AddPerson(DrunkPerson person) {
        _people.Add(person);
        PersonStatsMap.Add(person, new DrunkPersonStats(100, 0));
    }

    public override async Task<Status> Update() {
        await base.Update();
        _log.Info($"Step {Steps}");

        // update all the people
        foreach (var person in _people) {
            await person.Update(Steps);
        }

        return Status.Continue;
    }
}