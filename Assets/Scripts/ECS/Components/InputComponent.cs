using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct InputComponent : IBufferElementData
    {
        public int itemID;
        public int lastItemID;
        public int2 pos;
        public bool occupied;
    }
}