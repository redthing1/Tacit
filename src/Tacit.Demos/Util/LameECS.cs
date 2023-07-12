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

    public List<Entity> GetEntitiesWithComponent<T>() {
        return _entities.Where(x => x.GetComponent<T>() != null).ToList();
    }
}

public class Entity {
    private List<object> _components = new();
    public string Name { get; set; }

    public Entity(string name) {
        Name = name;
    }

    public T AddComponent<T>(T component) {
        _components.Add(component);
        return component;
    }

    public T GetComponent<T>() {
        return (T)_components.FirstOrDefault(x => x is T);
    }

    public List<T> GetComponents<T>() {
        return _components.OfType<T>().ToList();
    }

    public bool RemoveComponent<T>() {
        var component = GetComponent<T>();
        if (component == null) return false;
        _components.Remove(component);
        return true;
    }

    public bool RemoveComponent<T>(T component) {
        if (!_components.Contains(component)) return false;
        _components.Remove(component);
        return true;
    }

    public override string ToString() {
        return $"Entity({Name})";
    }
}