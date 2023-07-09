using System;
using System.Collections.Generic;

namespace Tacit.Framework.DGU;

public class DguAgent {
    public List<Drive> drives;
    public AgentEnvironment environment;
    public FactBank factMemory;
    public List<Goal> goals;
    public DguPlanner planner;
    public List<Sensor> sensors;

    #region Implement Sense

    private void Perceive() {}

    #endregion

    #region Implement Act

    private void PropagateEffects() {}

    #endregion

    #region Sense-Think-Act outline

    public void Sense() {
        Perceive();
    }

    public void Think() {
        // - update internal state
        UpdateFacts();
        UpdateDrives();
        UpdateGoals();

        // - ask the planning algorithm
        RequestPlans();
    }

    public void Act() {
        PropagateEffects();
    }

    #endregion

    #region Implement Think

    private void UpdateFacts() {
        throw new NotImplementedException();
    }

    private void UpdateDrives() {
        throw new NotImplementedException();
    }

    private void UpdateGoals() {
        throw new NotImplementedException();
    }

    private void RequestPlans() {
        throw new NotImplementedException();
    }

    #endregion

}