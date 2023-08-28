using System.Collections;
using System.Collections.Generic;
using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ItemAuthering : MonoBehaviour
{
    public int2 pos;
    public int itemID;
}

public class ItemBaker : Baker<ItemAuthering>
{
    public override void Bake(ItemAuthering authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ItemComponent
        {
            pos = authoring.pos,
            itemID = authoring.itemID
        });
    }
}
