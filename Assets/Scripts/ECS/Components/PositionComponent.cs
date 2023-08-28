using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct PositionComponent : IBufferElementData
    {
        public int2 pos;
    }
}