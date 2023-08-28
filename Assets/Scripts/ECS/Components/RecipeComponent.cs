using ECS.Components.Other;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Components
{
    public struct RecipeComponent : IComponentData
    {
        public float timeToCraft;
        public int outputItemID;

        public bool CanCraft(DynamicBuffer<ItemAmount> recipeItems, DynamicBuffer<ItemAmount> items)
        {
            foreach (var inputItem in recipeItems)
            {
                bool containing = false;
                foreach (var item in items)
                {
                    if (inputItem.itemID.Equals(item.itemID))
                    {
                        if (item.amount >= inputItem.amount)
                        {
                            containing = true;
                            break;
                        }
                    }
                }
                if (containing == false) return false;
            }
            return true;
        }
    }
}