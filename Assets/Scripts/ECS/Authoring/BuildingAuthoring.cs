using ECS.Components;
using ECS.Components.Other;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
    public class BuildingAuthoring : MonoBehaviour
    {
        
    }
    
    public class BuildingBaker : Baker<BuildingAuthoring>
    {
        public override void Bake(BuildingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            //Input
            AddBuffer<InputComponent>(entity);
            //Inventory
            AddBuffer<ItemAmount>(entity);
            //Output
            AddComponent(entity, new OutputProcessingBuildingComponent
            {
                itemCreated = true,
                outputEntity = Entity.Null,
                itemID = -1
            });
            //Processing building
            AddComponent(entity, new ProcessingBuildingComponent
            {
                recipe = Entity.Null,
                isProducing = false
            });
            //Position
            AddBuffer<PositionComponent>(entity);
            //Tags
            AddComponent<OutputNotFoundTag>(entity);
            AddComponent<UnregisteredTag>(entity);
        }
    }
}