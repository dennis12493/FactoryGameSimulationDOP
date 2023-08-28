using System.Threading;
using ECS.Components.Other;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;

namespace ECS.Components
{
    public struct ProcessingBuildingComponent : IComponentData
    {
        public Entity recipe;
        public float timer;
        public bool isProducing;

        public void AddItem(DynamicBuffer<ItemAmount> internalInventory, ItemAmount item)
        {
            for (int i = 0; i < internalInventory.Length; i++)
            {
                if (item.itemID == internalInventory[i].itemID)
                {
                    var inventoryItem = internalInventory[i];
                    inventoryItem.amount += item.amount;
                    internalInventory[i] = inventoryItem;
                    return;
                }
            }
            internalInventory.Add(item);
        }
        
        public void RemoveItems(DynamicBuffer<ItemAmount> internalInventory, DynamicBuffer<ItemAmount> items)
        {
            foreach (var itemAmount in items)
            {
                AddItem(internalInventory, new ItemAmount(itemAmount.itemID, -itemAmount.amount));
            }
        }
    }
}