using Unity.Entities;

namespace ECS.Components
{
    public struct BuildingEntitiesComponent : IComponentData
    {
        public Entity smelter;
        public Entity miningDrill;
        public Entity assembler;
        public Entity conveyorUI;
        public Entity trashcan;
    }
}