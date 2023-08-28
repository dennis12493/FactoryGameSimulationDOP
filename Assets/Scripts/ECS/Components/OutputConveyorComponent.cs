using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct OutputConveyorComponent : IComponentData
    {
        public Entity outputEntity;
        public int2 pos;
        public Entity item;
    }
}