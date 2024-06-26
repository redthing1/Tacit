using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Utils;

namespace Tacit.Framework.DGU;

public abstract class DGUAgent : ISmartObject, IForkable<DGUAgent>, IDGUDoctorable {
    public string Id { get; init; }
    public string? Name { get; init; }
    public AgentEnvironment Environment { get; }
    public FactMemory FactMemory { get; private set; }
    public List<Drive> Drives { get; } = new();
    public List<Goal> Goals { get; } = new();
    public List<Sensor> Sensors { get; } = new();
    public DGUPlanner? Planner { get; private set; }
    public DGUDoctor? Doctor { get; set; }

    public List<VirtualAction> ConsumableActions { get; } = new();
    public List<VirtualAction> SuppliedActions { get; } = new();

    public DGUAgent(string id, AgentEnvironment environment) {
        Id = id;
        Environment = environment;
        FactMemory = new FactMemory();
    }
    
    public void AttachDoctor(DGUDoctor doctor) {
        doctor.OnAttach(this);
        Doctor = doctor;
    }
    
    public DGUPlanner AttachPlanner(DGUPlanner planner) {
        Planner = planner;
        return planner;
    }

    public async Task Update(long time) {
        await SenseStage(time);
        await ThinkStage(time);
        // await ActStage();
    }

    #region Implement Sense

    /// <summary>
    /// sensors read data from the environment and update facts
    /// </summary>
    /// <returns></returns>
    private Task UpdateSensors(long time) {
        foreach (var sensor in Sensors) {
            sensor.Update(time, FactMemory);
        }

        return Task.CompletedTask;
    }

    #endregion

    // #region Implement Act
    //
    // private Task PropagateEffects() {
    //     return Task.CompletedTask;
    // }
    //
    // #endregion

    #region Sense-Think-Act outline

    public async Task SenseStage(long time) {
        await UpdateSensors(time);
    }

    public async Task ThinkStage(long time) {
        // - update internal state
        await UpdateDrives(time);
        await UpdateGoals(time);
        await UpdateInternalState(time);
    }

    // public Task ActStage() {
    //     PropagateEffects();
    //
    //     return Task.CompletedTask;
    // }

    #endregion

    #region Implement Think

    private async Task UpdateDrives(long time) {
        foreach (var drive in Drives.ToArray()) {
            // update drive state
            await drive.Update(time, FactMemory);

            // check removal triggers
            foreach (var trigger in drive.RemovalTriggers) {
                if (await trigger.Evaluate(FactMemory)) {
                    // remove the drive
                    Drives.Remove(drive);
                    // remove all goals generated by the drive
                    var goalsGeneratedByDrive = Goals.FindAll(goal => goal.Drive == drive);
                    foreach (var goal in goalsGeneratedByDrive) {
                        Goals.Remove(goal);
                    }
                }
            }
        }
    }

    private async Task UpdateGoals(long time) {
        foreach (var goal in Goals.ToArray()) {
            await goal.Update(time, FactMemory);

            // check removal triggers
            foreach (var trigger in goal.RemovalTriggers) {
                if (await trigger.Evaluate(FactMemory)) {
                    Goals.Remove(goal);
                }
            }
        }
    }

    protected virtual Task UpdateInternalState(long time) {
        return Task.CompletedTask;
    }

    public virtual async Task<float> EvaluateAggregateDriveSatisfaction(FactMemory memory) {
        float totalDriveSatisfaction = 0;
        long totalDriveWeight = 0;
        foreach (var drive in Drives) {
            var satisfaction = await drive.Evaluate(memory);
            totalDriveSatisfaction += satisfaction * drive.Weight;
            totalDriveWeight += drive.Weight;
        }
        if (totalDriveWeight == 0) {
            return 0;
        }

        return totalDriveSatisfaction / totalDriveWeight;
    }

    public virtual Task<float> EvaluatePlanState(DGUPlanState planState) {
        return EvaluateAggregateDriveSatisfaction(planState.HypotheticalFacts);
    }

    #endregion

    /// <summary>
    /// deep copy/clone this to an agent that can be simulated in parallel
    /// </summary>
    /// <returns></returns>
    public DGUAgent Fork() {
        var ret = (DGUAgent)MemberwiseClone();
        ret.FactMemory = FactMemory.Fork();
        return ret;
    }

    public override string ToString() {
        return $"{nameof(DGUAgent)}({Name}#{Id})";
    }
}