using ECS.Components;
using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile(CompileSynchronously = true)]
public partial struct ItemSystem : ISystem
{
    
    [BurstCompile(CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        new ItemMoveJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

    [BurstCompile(CompileSynchronously = true)]
    public partial struct ItemMoveJob : IJobEntity
    {
        public float deltaTime;
        
        [BurstCompile(CompileSynchronously = true)]
        private void Execute(ref LocalTransform transform, in ItemComponent item)
        {
            transform = transform.WithPosition(Vector3.Lerp(transform.Position.xyz, new Vector3(item.pos.x, item.pos.y, -0.5f), deltaTime * 2f));
        }
    }
}