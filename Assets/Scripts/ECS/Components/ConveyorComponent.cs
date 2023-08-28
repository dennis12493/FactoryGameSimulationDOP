using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct ConveyorComponent : IBufferElementData
    {
        public Entity item;
        public int2 pos;
    }
}