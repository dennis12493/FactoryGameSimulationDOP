using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct ItemComponent : IComponentData
    {
        public int2 pos;
        public int itemID;
    }
}
