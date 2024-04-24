using System.Collections.Generic;
using System.Linq;

namespace Tacit.Demos.Util;

public class LameECS {
    private readonly List<Entity> _entities = new();

    public void Initialize() {
        _entities.Clear();
    }

    public Entity CreateEntity(string name) {
        var entity = new Entity(name);
        _entities.Add(entity);
        return entity;
    }

    public bool DestroyEntity(Entity entity) {
        if (!_entities.Contains(entity)) return false;
        _entities.Remove(entity);
        return true;
    }

    public List<Entity> GetEntities() {
        return _entities;
    }

    public List<Entity> GetEntitiesWithComponent<T>() where T : class, IComponent {
        return _entities.Where(x => x.GetComponent<T>() != null).ToList();
    }
}

public class Entity {
    private List<IComponent> _components = new();
    public string Name { get; set; }

    public Entity(string name) {
        Name = name;
    }

    public T AddComponent<T>(T component) where T : IComponent {
        component.Entity = this;
        _components.Add(component);
        return component;
    }

    public T? GetComponent<T>() where T : class, IComponent {
        return _components.FirstOrDefault(x => x is T) as T;
    }

    public List<T> GetComponents<T>() where T : IComponent {
        return _components.OfType<T>().ToList();
    }

    public bool RemoveComponent<T>() where T : class, IComponent {
        var component = GetComponent<T>();
        return RemoveComponent(component);
    }

    public bool RemoveComponent<T>(T component) where T : IComponent {
        if (!_components.Contains(component)) return false;
        component.Entity = null;
        _components.Remove(component);
        return true;
    }

    public override string ToString() {
        return $"Entity({Name})";
    }
}

public interface IComponent {
    public Entity? Entity { get; set; }
}