using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct ProcessingBuildingSystem : ISystem
    {
        private ComponentLookup<RecipeComponent> recipeLookup;
        private BufferLookup<ItemAmount> recipeItemsLookup;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            recipeLookup = state.GetComponentLookup<RecipeComponent>(true);
            recipeItemsLookup = state.GetBufferLookup<ItemAmount>(true);
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            recipeLookup.Update(ref state);
            recipeItemsLookup.Update(ref state);
            new ProcessingBuildingJob
            {
                time = SystemAPI.Time.DeltaTime,
                recipeLookup = recipeLookup,
                recipeItemsLookup = recipeItemsLookup
            }.ScheduleParallel();
            state.CompleteDependency();
        }
        
        [BurstCompile(CompileSynchronously = true)]
        [WithNone(typeof(UnregisteredTag))]
        public partial struct ProcessingBuildingJob : IJobEntity
        {
            public float time;
            [ReadOnly] public ComponentLookup<RecipeComponent> recipeLookup;
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public BufferLookup<ItemAmount> recipeItemsLookup;

            [BurstCompile(CompileSynchronously = true)]
            private void Execute(ref ProcessingBuildingComponent building, ref OutputProcessingBuildingComponent output, ref DynamicBuffer<ItemAmount> inventory)
            {
                var recipe = recipeLookup[building.recipe];
                if (building.isProducing)
                {
                    building.timer -= time;
                    if (building.timer > 0) return;
                    output.itemID = recipe.outputItemID;
                    output.itemCreated = false;
                    building.isProducing = false;
                    building.timer = recipe.timeToCraft;
                }
                else if(!building.isProducing && output.itemCreated)
                {
                    var recipeItems = recipeItemsLookup[building.recipe];
                    if (recipe.CanCraft(recipeItems, inventory))
                    {
                        building.RemoveItems(inventory, recipeItems);
                        building.timer = recipe.timeToCraft;
                        building.isProducing = true;
                    }
                }
            }
        }
    }
}