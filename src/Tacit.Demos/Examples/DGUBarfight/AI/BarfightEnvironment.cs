using System.Collections.Generic;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class BarfightEnvironment : AgentEnvironment {
    public BarfightGame Game { get; }

    public BarfightEnvironment(BarfightGame game) {
        Game = game;
    }
}