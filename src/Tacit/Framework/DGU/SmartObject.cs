using System.Collections.Generic;

namespace Tacit.Framework.DGU;

public interface ISmartObject {
    string Id { get; }

    /// <summary>
    /// the actions that this object can consume
    /// </summary>
    List<VirtualAction> ConsumableActions { get; }

    /// <summary>
    /// the actions that can be performed on this object
    /// </summary>
    List<VirtualAction> SuppliedActions { get; }
}

public abstract class SmartObject : ISmartObject {
    public abstract string Id { get; }
    public List<VirtualAction> ConsumableActions { get; } = new();
    public List<VirtualAction> SuppliedActions { get; } = new();
}