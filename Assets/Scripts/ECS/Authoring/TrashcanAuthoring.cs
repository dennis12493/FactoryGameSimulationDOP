using System.Collections;
using System.Collections.Generic;
using ECS.Components;
using ECS.Components.Other;
using Unity.Entities;
using UnityEngine;

public class TrashcanAuthoring : MonoBehaviour
{

}

public class TrashcanBaker : Baker<TrashcanAuthoring>
{
    public override void Bake(TrashcanAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        //Input
        AddBuffer<InputComponent>(entity);
        //Position
        AddBuffer<PositionComponent>(entity);
        //Tags
        AddComponent<UnregisteredTag>(entity);
        AddComponent<TrashcanComponent>(entity);
    }
}
