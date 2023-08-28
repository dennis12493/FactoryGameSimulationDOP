using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct RegistrationSystem : ISystem
    {

        private NativeParallelHashMap<int2, Entity> buildingDictionary;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingDictionaryComponent>();
            buildingDictionary = new NativeParallelHashMap<int2, Entity>(32, Allocator.Persistent);
            state.EntityManager.AddComponent<BuildingDictionaryComponent>(state.SystemHandle);
            state.EntityManager.SetComponentData(state.SystemHandle, new BuildingDictionaryComponent{ buildingDictionary = buildingDictionary});
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnDestroy(ref SystemState state)
        {
            state.EntityManager.GetComponentData<BuildingDictionaryComponent>(state.SystemHandle).buildingDictionary
                .Dispose();
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            new RegistrationJob { 
                ecb = ecb, 
                buildingDictionary = buildingDictionary
            }.Schedule();
            new RegistrationConveyorJob
            {
                ecb = ecb,
                buildingDictionary = buildingDictionary
            }.Schedule();
            state.EntityManager.SetComponentData(state.SystemHandle, new BuildingDictionaryComponent{ buildingDictionary = buildingDictionary});
            state.Dependency.Complete();
        }

        
        [BurstCompile(CompileSynchronously = true)]
        [WithAll(typeof(UnregisteredTag))]
        public partial struct RegistrationJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [WriteOnly] public NativeParallelHashMap<int2, Entity> buildingDictionary;
            [BurstCompile(CompileSynchronously = true)]
            private void Execute(Entity e, in DynamicBuffer<PositionComponent> positions)
            {
                foreach (var position in positions)
                {
                    buildingDictionary.Add(position.pos, e);
                }
                ecb.RemoveComponent<UnregisteredTag>(e);
            }
        }
        
        [BurstCompile(CompileSynchronously = true)]
        [WithAll(typeof(UnregisteredTag))]
        public partial struct RegistrationConveyorJob : IJobEntity
        {
            public EntityCommandBuffer ecb;
            [WriteOnly] public NativeParallelHashMap<int2, Entity> buildingDictionary;
            [BurstCompile(CompileSynchronously = true)]
            private void Execute(Entity e, in InputConveyorComponent input)
            {
                buildingDictionary.Add(input.pos, e);
                ecb.RemoveComponent<UnregisteredTag>(e);
            }
        }
    }
}