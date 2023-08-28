using System;
using Unity.Entities;

namespace ECS.Components
{
    public struct ItemEntitiesComponent : IComponentData
    {
        public Entity wood;
        public Entity coal;
        public Entity ironOre;
        public Entity ironIngot;
        public Entity goldOre;
        public Entity goldIngot;
        public Entity steelIngot;

        public Entity GetEntityWithID(int id)
        {
            switch (id)
            {
                case 0: return ironOre;
                case 1: return goldOre;
                case 2: return wood;
                case 3: return coal;
                case 4: return ironIngot;
                case 5: return goldIngot;
                case 6: return steelIngot;
            }
            return Entity.Null;
        }
    }
}