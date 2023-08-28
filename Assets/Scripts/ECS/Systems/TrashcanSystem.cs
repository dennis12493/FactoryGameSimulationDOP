using ECS.Components;
using ECS.Components.Other;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile(CompileSynchronously = true)]
    public partial struct TrashcanSystem : ISystem
    {
        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            new TrashcanJob().ScheduleParallel();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    [WithNone(typeof(UnregisteredTag))]
    [WithAll(typeof(TrashcanComponent))]
    public partial struct TrashcanJob : IJobEntity
    {
        [BurstCompile(CompileSynchronously = true)]
        private void Execute(ref DynamicBuffer<InputComponent> inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                input.itemID = -1;
                inputs[i] = input;
            }
        }
    }
}