using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    [UpdateAfter(typeof(ProcessingBuildingSystem))]
    public partial struct CreateItemSystem : ISystem
    {
        private ComponentLookup<InputConveyorComponent> inputLookup;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ItemEntitiesComponent>();
            inputLookup = state.GetComponentLookup<InputConveyorComponent>();
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            
            inputLookup.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var itemEntities = SystemAPI.GetSingleton<ItemEntitiesComponent>();
            new CreateItemJob
            {
                inputLookup = inputLookup,
                ecb = ecb,
                itemEntities = itemEntities
            }.Schedule();
            state.CompleteDependency();
        }
        
        [BurstCompile(CompileSynchronously = true)]
        [WithNone(typeof(OutputNotFoundTag))]
        public partial struct CreateItemJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentLookup<InputConveyorComponent> inputLookup;
            [ReadOnly] public ItemEntitiesComponent itemEntities;

            [BurstCompile(CompileSynchronously = true)]
            private void Execute(ref OutputProcessingBuildingComponent output)
            {
                if(output.itemID == -1) return;
                var itemID = output.itemID;
                var input = inputLookup[output.outputEntity];
                if(input.occupied || input.item != Entity.Null) return;
                output.itemID = -1;
                output.itemCreated = true;
                var itemEntity = itemEntities.GetEntityWithID(itemID);
                var item = ecb.Instantiate(itemEntity);
                ecb.SetComponent(item, LocalTransform.FromPositionRotationScale(new float3(output.pos.x, output.pos.y, -0.5f), quaternion.identity, 0.5f));
                ecb.SetComponent(item, new ItemComponent{pos = output.pos, itemID = itemID});
                ecb.SetComponent(output.outputEntity, new InputConveyorComponent{ item = item, pos = input.pos, occupied = true});
            }
        }
    }
}