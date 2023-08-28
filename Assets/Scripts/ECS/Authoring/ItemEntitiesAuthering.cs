using System.Collections;
using System.Collections.Generic;
using ECS.Components;
using Unity.Entities;
using UnityEngine;

public class ItemEntitiesAuthering : MonoBehaviour
{
    public GameObject wood;
    public GameObject coal;
    public GameObject ironOre;
    public GameObject ironIngot;
    public GameObject goldOre;
    public GameObject goldIngot;
    public GameObject steelIngot;
}

public class ItemEntitiesBaker : Baker<ItemEntitiesAuthering>
{
    public override void Bake(ItemEntitiesAuthering authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ItemEntitiesComponent
        {
            coal = GetEntity(authoring.coal, TransformUsageFlags.Dynamic),
            wood = GetEntity(authoring.wood, TransformUsageFlags.Dynamic),
            ironOre = GetEntity(authoring.ironOre, TransformUsageFlags.Dynamic),
            ironIngot = GetEntity(authoring.ironIngot, TransformUsageFlags.Dynamic),
            goldOre = GetEntity(authoring.goldOre, TransformUsageFlags.Dynamic),
            goldIngot = GetEntity(authoring.goldIngot, TransformUsageFlags.Dynamic),
            steelIngot = GetEntity(authoring.steelIngot, TransformUsageFlags.Dynamic),
        });
    }
}
