using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct InputProcessingBuildingSystem : ISystem
    {
        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            new InputProcessingBuildingJob().Schedule();
        }
    }
    
    [BurstCompile(CompileSynchronously = true)]
    [WithNone(typeof(UnregisteredTag))]
    public partial struct InputProcessingBuildingJob : IJobEntity
    {
        [BurstCompile(CompileSynchronously = true)]
        private void Execute(ref ProcessingBuildingComponent building, ref DynamicBuffer<InputComponent> inputs, ref DynamicBuffer<ItemAmount> inventory)
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                foreach (var itemAmount in inventory)
                {
                    if (!itemAmount.itemID.Equals(input.lastItemID)) continue;
                    input.occupied = itemAmount.amount >= 10;
                }
                var itemID = input.itemID;
                if (itemID.Equals(-1))
                {
                    inputs[i] = input;
                    continue;
                }
                input.lastItemID = itemID;
                building.AddItem(inventory, new ItemAmount(itemID, 1));
                input.itemID = -1;
                inputs[i] = input;
            }
            
        }
    }
}