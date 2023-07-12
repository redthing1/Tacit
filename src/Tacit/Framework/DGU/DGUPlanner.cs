using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public class DGUPlanner {
    public record PlannerConfig(long MaxSimulationDepth = 16);

    public record PlanInvocationContext(long time);

    public record PlanResult(List<VirtualAction> Actions) {
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var action in Actions) {
                sb.Append(action);
                sb.Append(", ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }

    private int _idCounter = 0;
    public PlannerConfig Config { get; }
    public DGUAgent RootAgent { get; }

    public DGUPlanner(PlannerConfig config, DGUAgent rootAgent) {
        Config = config;
        RootAgent = rootAgent;
    }

    private int GetNextId() => _idCounter++;

    public async Task<PlanResult?> Plan(PlanInvocationContext context) {
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Info, $"Planning for agent {RootAgent}");
        // get seed plan states
        var planStates = await GenerateInitialPlanStates();
        var initialFacts = RootAgent.FactMemory;
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"  Initial plan states: {planStates.Count}");

        while (planStates.Count > 0) {
            // evaluate plan states and take the highest scoring one
            foreach (var planState in planStates) {
                // await planState.Update();
                planState.Score = await RootAgent.EvaluatePlanState(planState);
            }
            var bestPlanState = planStates.MaxBy(x => x.Score);
            if (bestPlanState == null)
                throw new ArgumentNullException(nameof(bestPlanState), $"No plan state was found with a score greater than 0. This should not happen.");

            // remove best plan state from list
            planStates.Remove(bestPlanState);

            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"  Selected best plan state: {bestPlanState}");

            var unsatisfiedPreconditions = await bestPlanState.GetUnsatisfiedPreconditions();
            if (!unsatisfiedPreconditions.Any()) {
                // if there are no unsatisfied preconditions, then we have found a plan
                RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Info, $"  No unsatisfied preconditions. Found plan: {bestPlanState}");
                return new PlanResult(bestPlanState.CollectPredecessorActions());
            }

            // begin generating successor plan states
            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"  Generating successor plan states");
            var possibleNextActions = new List<VirtualAction>();
            var successorStates = new List<DGUPlanState>();

            // find actions that can satisfy the unsatisfied preconditions
            foreach (var unsatisfiedPrecondition in unsatisfiedPreconditions) {
                RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"    Checking unsatisfied precondition: {unsatisfiedPrecondition}");
                var actionsApplicableToPrecondition = FindApplicableActions(unsatisfiedPrecondition, RootAgent.ConsumableActions);
                foreach (var applicableAction in actionsApplicableToPrecondition) {
                    // var mappedAction = MapKeysToFacts(applicableAction, initialFacts);
                    // possibleNextActions.Add(mappedAction);
                    RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"        Found applicable action: {applicableAction}");
                    possibleNextActions.Add(applicableAction);
                }
            }

            // simulate each action and generate a successor state
            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"    Simulating actions");
            foreach (var action in possibleNextActions) {
                // simulate the execution of the action
                var successorState = await SimulateActionExecution(bestPlanState, action);
                // add a successor state to the list
                successorStates.Add(successorState);
            }

            // add successor plan states to search space
            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"    Adding {successorStates.Count} successor plan states to search space");
            planStates.AddRange(successorStates);
        }

        // if we get here, then we were unable to find a plan
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Error, $"Unable to find a plan for agent {RootAgent}");
        return null;
    }

    private Task<List<DGUPlanState>> GenerateInitialPlanStates() {
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Info, $"Generating initial plan states for agent {RootAgent}");

        var states = new List<DGUPlanState>();
        var agent = RootAgent;

        // generate a root plan state
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"  Generating root plan state");
        var rootSoftGoalConditions = agent.Goals.SelectMany(x => x.Conditions).ToList();
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"    Root soft goal conditions: {rootSoftGoalConditions.Count}");
        var rootState = new DGUPlanState(GetNextId(), hardGoalConditions: null, softGoalConditions: rootSoftGoalConditions, parent: null, agent.FactMemory, actionGeneratedBy: null);
        states.Add(rootState);

        // for each consumable action, generate a plan state
        // we want the fact change to match the condition key of any goal
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"  Generating plan states for consumable actions");
        foreach (var consumableAction in agent.ConsumableActions) {
            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"    Checking consumable action: {consumableAction}");
            // check the effect of the action and see if it matches the condition key of any goal
            foreach (var effect in consumableAction.Effects) {
                RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"      Checking effect: {effect}");
                foreach (var goal in agent.Goals) {
                    RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"        Checking goal: {goal}");
                    // see if the effect key matches a goal condition
                    foreach (var condition in goal.Conditions) {
                        if (effect.Change == condition.SatisfactionCriterion) {
                            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"          Action effect {effect} is applicable to goal condition {condition}");
                            // this effect would satisfy a goal condition
                            // create a new plan state
                            var hardGoalConditions = new List<IPartialCondition> {
                                condition
                            };
                            var newStateFacts = rootState.HypotheticalFacts.Fork();
                            var newState = new DGUPlanState(
                                GetNextId(), hardGoalConditions: hardGoalConditions, softGoalConditions: rootSoftGoalConditions,
                                parent: rootState, newStateFacts, actionGeneratedBy: consumableAction
                            );
                            RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"            Generated plan state: {newState}");
                            states.Add(newState);
                        }
                    }
                }
            }
        }

        return Task.FromResult(states);
    }

    /// <summary>
    /// simuulate the world state after the execution of an action
    /// </summary>
    /// <param name="inputState"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<DGUPlanState> SimulateActionExecution(DGUPlanState inputState, VirtualAction action) {
        RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"      Simulating action: {action}");
        var newState = inputState.Fork();
        // collect all actions that lead to this state
        var predecessorActions = inputState.CollectPredecessorActions();
        // apply these actions in forward order
        foreach (var predecessorAction in predecessorActions) {
            // apply the effects of this single action
            foreach (var effect in predecessorAction.Effects) {
                var correspondingFact = newState.HypotheticalFacts.GetFactFromChangeKey(effect.Change);
                if (correspondingFact == null)
                    throw new ArgumentNullException(nameof(correspondingFact), $"No fact was found with a change key of {effect.Change}. This should not happen.");

                // the world diff created by the action's effect
                var actionEffectWorldDiff = new WorldDiff(effect.Change, correspondingFact, depth: 0);
                RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"        Simulating effect: {effect}");
                await SimulateWorldDiffsUntilEquilibrium(newState, effect, actionEffectWorldDiff);
            }
        }

        return newState;
    }

    private async Task SimulateWorldDiffsUntilEquilibrium(DGUPlanState newState, VirtualEffect effect, WorldDiff seedDiff) {
        var diffQueue = new Queue<WorldDiff>();
        diffQueue.Enqueue(seedDiff);
        while (diffQueue.Count > 0) {
            var diff = diffQueue.Dequeue();

            // ensure the simulation depth did not run away
            if (diff.depth > Config.MaxSimulationDepth) {
                RootAgent.Doctor?.Log(DGUDoctor.LogLevel.Error, $"Simulation depth exceeded {Config.MaxSimulationDepth}.");
                throw new Exception($"Simulation depth exceeded {Config.MaxSimulationDepth}.");
            }

            // simulate this diff
            var newDiffs = await effect.SimulateInWorld(diff);

            // add new diffs to queue
            foreach (var newDiff in newDiffs) {
                diffQueue.Enqueue(newDiff);
            }
        }

        // if we got here, equilibrium has been reached
    }

    // /// <summary>
    // /// maps an action with preconditions and effects that is not yet associated with any facts
    // /// </summary>
    // /// <param name="unmappedAction"></param>
    // /// <param name="factsBefore"></param>
    // /// <returns></returns>
    // /// <exception cref="NotImplementedException"></exception>
    // private VirtualAction MapKeysToFacts(VirtualAction unmappedAction, FactMemory factsBefore) {
    //     var mappedAction = unmappedAction.Fork();
    //     foreach (var precondition in mappedAction.Preconditions) {
    //         // map each precondition to its relevant fact
    //         // then match each effect of the action to its relevant fact
    //         // provides each precondition with the fact needed to evaluate if the precondition is satisfied
    //         // provides each effect with the fact needed to know which fact the effect will change
    //         
    //         // find the fact that matches the change key of the precondition
    //         var matchingFact = factsBefore.GetFactFromChangeKey(precondition.SatisfactionCriterion);
    //     }
    //
    //     foreach (var effect in mappedAction.Effects) {
    //         var matchingFact = factsBefore.GetFactFromChangeKey(effect.Change);
    //     }
    //     
    //     return mappedAction;
    // }

    private IEnumerable<VirtualAction> FindApplicableActions(IPartialCondition condition, List<VirtualAction> consumableActions) {
        var candidates = new List<VirtualAction>();
        foreach (var action in consumableActions) {
            // see if the effect of the action matches the condition
            foreach (var effect in action.Effects) {
                if (effect.Change == condition.SatisfactionCriterion) {
                    // this action would satisfy the condition
                    candidates.Add(action);
                }
            }
        }
        // var applicableActions = new List<VirtualAction>();
        // foreach (var environmentAction in allActions) {
        //     // see if the environment action overlaps with the candidates
        //     
        // }
        // ???

        // var results = new List<VirtualAction>();
        // foreach (var candidate in candidates) {
        //     var action = new VirtualAction(candidate.Name, candidate.Preconditions, candidate.Effects);
        // }
        // ???

        return candidates;
    }
}