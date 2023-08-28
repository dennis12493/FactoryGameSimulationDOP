using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public partial struct BuildingDictionaryComponent : IComponentData
    {
        public NativeParallelHashMap<int2, Entity> buildingDictionary;
    }
}