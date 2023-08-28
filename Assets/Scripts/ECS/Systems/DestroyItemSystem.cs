using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct DestroyItemSystem : ISystem
    {
        private BufferLookup<InputComponent> inputsLookup;
        private ComponentLookup<ItemComponent> itemLookup;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            inputsLookup = state.GetBufferLookup<InputComponent>();
            itemLookup = state.GetComponentLookup<ItemComponent>();
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            inputsLookup.Update(ref state);
            itemLookup.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            new DestroyItemJob
            {
                inputsLookup = inputsLookup,
                itemLookup = itemLookup,
                ecb = ecb
            }.Schedule();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    [WithNone(typeof(OutputNotFoundTag))]
    public partial struct DestroyItemJob : IJobEntity
    {
        public BufferLookup<InputComponent> inputsLookup;
        [ReadOnly] public ComponentLookup<ItemComponent> itemLookup;
        public EntityCommandBuffer ecb;

        [BurstCompile(CompileSynchronously = true)]
        private void Execute(ref OutputConveyorComponent output)
        {
            if (output.item == Entity.Null) return;
            var inputs = inputsLookup[output.outputEntity];
            for (int i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                if (!input.pos.Equals(output.pos)) continue;
                if (input.occupied || !input.itemID.Equals(-1)) return;
                var item = output.item;
                output.item = Entity.Null;
                var itemID = itemLookup[item].itemID;
                ecb.DestroyEntity(item);
                input.itemID = itemID;
                inputs[i] = input;
            }
        }
    }
}
        