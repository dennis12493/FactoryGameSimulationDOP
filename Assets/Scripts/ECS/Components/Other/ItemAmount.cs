using Unity.Entities;

namespace ECS.Components.Other
{
    [InternalBufferCapacity(12)]
    public struct ItemAmount : IBufferElementData
    {
        public ItemAmount(int itemID, int amount)
        {
            this.itemID = itemID;
            this.amount = amount;
        }

        public int itemID;
        public int amount;
    }
}