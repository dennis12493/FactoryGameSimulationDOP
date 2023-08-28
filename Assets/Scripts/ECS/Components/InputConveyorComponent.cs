using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct InputConveyorComponent : IComponentData
    {
        public Entity item;
        public int2 pos;
        public bool occupied;
    }
}