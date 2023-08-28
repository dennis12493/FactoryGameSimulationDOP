using Unity.Entities;

namespace ECS.Components
{
    public struct MiningDrillComponent : IComponentData
    {
        public float timer;
        public Entity item;
        public bool isItemSet;
    }
}