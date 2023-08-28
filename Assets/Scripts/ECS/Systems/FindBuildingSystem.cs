using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct FindBuildingSystem : ISystem
    {
        private ComponentLookup<InputConveyorComponent> inputConveyorLookup;
        private BufferLookup<InputComponent> inputLookup;
        private BufferLookup<ConveyorComponent> conveyorLookup;
        private ComponentLookup<OutputConveyorComponent> outputConveyorLookup;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            inputConveyorLookup = state.GetComponentLookup<InputConveyorComponent>();
            inputLookup = state.GetBufferLookup<InputComponent>();
            conveyorLookup = state.GetBufferLookup<ConveyorComponent>();
            outputConveyorLookup = state.GetComponentLookup<OutputConveyorComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            inputConveyorLookup.Update(ref state);
            inputLookup.Update(ref state);
            conveyorLookup.Update(ref state);
            outputConveyorLookup.Update(ref state);
            
            var buildingDictionarySystemHandle = state.World.GetExistingSystem<RegistrationSystem>();
            var buildingDictionary = state.EntityManager
                .GetComponentData<BuildingDictionaryComponent>(buildingDictionarySystemHandle).buildingDictionary;
            
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            new FindBuildingJob
            {
                buildingDictionary = buildingDictionary,
                conveyorLookup = conveyorLookup,
                inputLookup = inputLookup,
                inputConveyorLookup = inputConveyorLookup,
                outputConveyorLookup = outputConveyorLookup,
                ecb = ecb
            }.Schedule();
        }
    }
    
    [WithAll(typeof(OutputNotFoundTag))]
    public partial struct FindBuildingJob : IJobEntity
    {
        [ReadOnly] public NativeParallelHashMap<int2, Entity> buildingDictionary;
        [ReadOnly] public BufferLookup<InputComponent> inputLookup;
        [ReadOnly] public ComponentLookup<InputConveyorComponent> inputConveyorLookup;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public BufferLookup<ConveyorComponent> conveyorLookup;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public ComponentLookup<OutputConveyorComponent> outputConveyorLookup;
        public EntityCommandBuffer ecb;
        private void Execute(Entity e, ref DynamicBuffer<ConveyorComponent> beltPath, ref OutputConveyorComponent output)
        {
            if (buildingDictionary.ContainsKey(output.pos))
            {
                var entity = buildingDictionary[output.pos];
                if (inputConveyorLookup.HasComponent(entity))
                {
                    MergeConveyor(ref output, ref beltPath, entity);
                }else if (inputLookup.HasBuffer(entity))
                {
                    output.outputEntity = entity;
                    ecb.RemoveComponent<OutputNotFoundTag>(e);
                }
            }
        }

        private void MergeConveyor(ref OutputConveyorComponent output, ref DynamicBuffer<ConveyorComponent> beltPath, Entity nextConveyor)
        {
            var nextBeltPath = conveyorLookup[nextConveyor];
            foreach (var conveyorComponent in nextBeltPath)
            {
                beltPath.Add(conveyorComponent);
            }
            if(output.item != Entity.Null) ecb.DestroyEntity(output.item);
            output = outputConveyorLookup[nextConveyor];
            ecb.DestroyEntity(nextConveyor);
        }
    }
}