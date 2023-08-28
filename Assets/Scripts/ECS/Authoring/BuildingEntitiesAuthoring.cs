using ECS.Components;
using Unity.Entities;
using UnityEngine;

public class BuildingEntitiesAuthoring : MonoBehaviour
{
    public GameObject smelter;
    public GameObject miningDrill;
    public GameObject assembler;
    public GameObject conveyorUI;
    public GameObject trashcan;
}

public class BuildingEntitiesBaker : Baker<BuildingEntitiesAuthoring>
{
    public override void Bake(BuildingEntitiesAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BuildingEntitiesComponent
        {
            smelter = GetEntity(authoring.smelter, TransformUsageFlags.Dynamic),
            miningDrill = GetEntity(authoring.miningDrill, TransformUsageFlags.Dynamic),
            assembler = GetEntity(authoring.assembler, TransformUsageFlags.Dynamic),
            conveyorUI = GetEntity(authoring.conveyorUI, TransformUsageFlags.Dynamic),
            trashcan = GetEntity(authoring.trashcan, TransformUsageFlags.Dynamic)
        });
    }
}