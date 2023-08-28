using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;

public class BuildingDictionary : MonoBehaviour
{

    public static BuildingDictionary Instance { get; private set;}
    private EntityManager manager;
    private Dictionary<int2, Entity> buildings;

    void Awake()
    {
        Instance = this;
        buildings = new Dictionary<int2, Entity>();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public Entity GetEntityAtPosition(int2 key)
    {
        if (buildings.ContainsKey(key)) return buildings[key];
        return Entity.Null;
    }
    
    public bool CheckIfOccupied(params int2[] int2s){
        foreach(int2 v in int2s){
            if(buildings.ContainsKey(v)) return true;
        }
        return false;
    }

    public bool IsConveyor(int2 pos)
    {
        Entity entity;
        if (buildings.TryGetValue(pos, out entity))
        {
            return manager.HasComponent<ConveyorComponent>(entity);
        }
        return false;
    }

    public bool IsBuilding(int2 pos)
    {
        Entity entity;
        if (buildings.TryGetValue(pos, out entity))
        {
            return manager.HasComponent<ProcessingBuildingComponent>(entity);
        }
        return false;
    }

    public void InsertEntityInDictionary(int2[] position, Entity entity)
    {
        foreach (int2 pos in position) {
            buildings[pos] = entity;
        }
    }

    public void DeleteEntityAtPosition(int2 position)
    {
        if (buildings.ContainsKey(position))
        {
            buildings.Remove(position);
            manager.DestroyEntity(buildings[position]);
        }
    }
}