using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct OutputProcessingBuildingComponent : IComponentData
    {
        public Entity outputEntity;
        public int2 pos;
        public int itemID;
        public bool itemCreated;
    }
}