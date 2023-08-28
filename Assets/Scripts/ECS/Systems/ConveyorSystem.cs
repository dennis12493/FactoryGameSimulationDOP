using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct ConveyorSystem : ISystem
    {
        private float timeToMove;
        private ComponentLookup<ItemComponent> itemLookup;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            timeToMove = 2f;
            itemLookup = state.GetComponentLookup<ItemComponent>(false);
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            timeToMove -= SystemAPI.Time.DeltaTime;
            if (timeToMove <= 0)
            {
                itemLookup.Update(ref state);
                new ConveyorJob
                {
                    itemLookup = itemLookup
                }.ScheduleParallel();
                timeToMove += 2;
            }
            state.CompleteDependency();
        }
        
        [BurstCompile(CompileSynchronously = true)]
        public partial struct ConveyorJob : IJobEntity
        {
            [NativeDisableParallelForRestriction]
            public ComponentLookup<ItemComponent> itemLookup;

            [BurstCompile(CompileSynchronously = true)]
            private void Execute(ref InputConveyorComponent input, ref OutputConveyorComponent output, ref  DynamicBuffer<ConveyorComponent> beltPath)
            {
                var lastBelt = beltPath[^1];
                if (lastBelt.item != Entity.Null && output.item == Entity.Null)
                {
                    var item = lastBelt.item;
                    var itemComponent = itemLookup[item];
                    itemLookup[item] = new ItemComponent { pos = output.pos, itemID = itemComponent.itemID};
                    output.item = item;
                    lastBelt.item = Entity.Null;
                    beltPath[^1] = lastBelt;
                }
                for (int i = beltPath.Length-2; i >= 0; i--)
                {
                    var thisConveyor = beltPath[i];
                    var lastConveyor = beltPath[i + 1];
                    if (thisConveyor.item != Entity.Null)
                    {
                        if (lastConveyor.item == Entity.Null)
                        {
                            var item = thisConveyor.item;
                            var itemComponent = itemLookup[item];
                            lastConveyor.item = item;
                            itemLookup[item] = new ItemComponent { pos = lastConveyor.pos, itemID = itemComponent.itemID};
                            thisConveyor.item = Entity.Null;
                            beltPath[i] = thisConveyor;
                            beltPath[i + 1] = lastConveyor;
                        }
                    }
                }
                var firstConveyor = beltPath[0];
                if (firstConveyor.item != Entity.Null) input.occupied = true;
                else input.occupied = false;
                if (input.item != Entity.Null && firstConveyor.item == Entity.Null)
                {
                    firstConveyor.item = input.item;
                    input.item = Entity.Null;
                    beltPath[0] = firstConveyor;
                }
            }
        }
    }
}