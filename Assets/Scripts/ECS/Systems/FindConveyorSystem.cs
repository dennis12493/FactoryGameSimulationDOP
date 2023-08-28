using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [UpdateAfter(typeof(RegistrationSystem))]
    [BurstCompile(CompileSynchronously = true)]
    public partial struct FindConveyorSystem : ISystem
    {
        private ComponentLookup<InputConveyorComponent> inputLookup;
        
        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            inputLookup = state.GetComponentLookup<InputConveyorComponent>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            
            var buildingDictionarySystemHandle = state.World.GetExistingSystem<RegistrationSystem>();
            var buildingDictionary = state.EntityManager
                .GetComponentData<BuildingDictionaryComponent>(buildingDictionarySystemHandle).buildingDictionary;
            inputLookup.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            new FindConveyorJob
            {
                buildingDictionary = buildingDictionary,
                inputLookup = inputLookup,
                ecb = ecb
            }.Schedule();
            state.CompleteDependency();
        }

        [WithAll(typeof(OutputNotFoundTag))]
        [BurstCompile(CompileSynchronously = true)]
        public partial struct FindConveyorJob : IJobEntity
        {
            [ReadOnly] public NativeParallelHashMap<int2, Entity> buildingDictionary;
            [ReadOnly] public ComponentLookup<InputConveyorComponent> inputLookup;
            public EntityCommandBuffer ecb;
            
            [BurstCompile(CompileSynchronously = true)]
            private void Execute(Entity e, ref OutputProcessingBuildingComponent output)
            {
                if (!buildingDictionary.ContainsKey(output.pos)) return;
                Entity inputEntity = buildingDictionary[output.pos];
                var input = inputLookup[inputEntity];
                if(!input.pos.Equals(output.pos)) return;
                output.outputEntity = inputEntity;
                ecb.RemoveComponent<OutputNotFoundTag>(e);
            }
        }
    }
}