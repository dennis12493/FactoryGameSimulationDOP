using ECS.Components;
using ECS.Components.Other;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private EntityManager manager;
    private BuildingEntitiesComponent buildings;
    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(BuildingEntitiesComponent));
        buildings = query.GetSingleton<BuildingEntitiesComponent>();
    }

    public Entity SpawnConveyor(Vector2 start, Vector2 end, float rotation)
    {
        var conveyorUI = buildings.conveyorUI;
        var distance = (int) Vector2.Distance(start, end);
        Vector2 direction = (end - start).normalized;

        if (direction.Equals(Vector2.zero))
        {
            switch (rotation)
            {
                case 0:
                    direction = new Vector2(1, 0);
                    break;
                case 90:
                    direction = new Vector2(0, 1);
                    break;
                case 180:
                    direction = new Vector2(-1, 0);
                    break;
                case 270:
                    direction = new Vector2(0, -1);
                    break;
            }
        }

        Entity beltPathEntity = manager.CreateEntity();
        var beltPath = manager.AddBuffer<ConveyorComponent>(beltPathEntity);
        for (int i = 0; i < distance + 1; i++)
        {
            Vector2 pos = start + (direction * i);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var ui = manager.Instantiate(conveyorUI);
            manager.SetComponentData(ui, LocalTransform.FromPositionRotation(new float3(pos.x, pos.y, -0.1f), Quaternion.AngleAxis(angle, Vector3.forward)));
            beltPath.Add(new ConveyorComponent {pos = new int2(pos)});
        }
        manager.AddComponentData(beltPathEntity, new InputConveyorComponent{ pos = new int2(start), occupied = false, item = Entity.Null});
        manager.AddComponentData(beltPathEntity, new OutputConveyorComponent{ pos = new int2(end + direction)});
        manager.AddComponentData(beltPathEntity, new LocalTransform{Position = new float3(start.x, start.y, 0f)});
        manager.AddComponentData(beltPathEntity, new UnregisteredTag());
        manager.AddComponentData(beltPathEntity, new OutputNotFoundTag());
        return beltPathEntity;
    }

    public Entity SpawnMiningDrill(int2 pos, Quaternion rotation, Entity recipe)
    {
        int2 output = int2.zero;
        switch (rotation.eulerAngles.z)
        {
            case 0:
                output = new int2(pos.x + 1, pos.y);
                break;
            case 90:
                output = new int2(pos.x, pos.y + 1);
                break;
            case 180:
                output = new int2(pos.x - 1, pos.y);
                break;
            case 270:
                output = new int2(pos.x, pos.y - 1);
                break;
        }
        var miningDrill = manager.Instantiate(buildings.miningDrill);
        manager.SetComponentData(miningDrill, LocalTransform.FromPositionRotation(new float3(pos.x, pos.y, 0f), rotation));
        manager.SetComponentData(miningDrill, new ProcessingBuildingComponent {recipe = recipe, timer = 2f, isProducing = false});
        manager.SetComponentData(miningDrill, new OutputProcessingBuildingComponent {outputEntity = Entity.Null, pos = output, itemID = -1, itemCreated = true});
        SetPositions(miningDrill, new []{pos});
        return miningDrill;
    }

    public Entity SpawnSmelter(int2 pos, Quaternion rotation, Entity recipe)
    {
        int2 output = int2.zero, pos2 = int2.zero;
        switch (rotation.eulerAngles.z)
        {
            case 0:
                output = new int2(pos.x + 2, pos.y);
                pos2 = new int2(pos.x + 1, pos.y);
                break;
            case 90:
                output = new int2(pos.x, pos.y + 2);
                pos2 = new int2(pos.x, pos.y + 1);
                break;
            case 180:
                output = new int2(pos.x - 2, pos.y);
                pos2 = new int2(pos.x - 1, pos.y);
                break;
            case 270:
                output = new int2(pos.x, pos.y - 2);
                pos2 = new int2(pos.x, pos.y - 1);
                break;
        }
        var smelter = manager.Instantiate(buildings.smelter);
        manager.SetComponentData(smelter, LocalTransform.FromPositionRotation(new float3(pos.x, pos.y, 0f), rotation));
        manager.SetComponentData(smelter, new ProcessingBuildingComponent{recipe = recipe, isProducing = false, timer = 100f});
        manager.SetComponentData(smelter, new OutputProcessingBuildingComponent{ outputEntity = Entity.Null, pos = output, itemID = -1, itemCreated = true});
        SetPositions(smelter, new []{pos, pos2});
        var inputs = manager.GetBuffer<InputComponent>(smelter);
        inputs.Add(new InputComponent{itemID = -1, pos = pos, occupied = false, lastItemID = -1});
        manager.AddBuffer<ItemAmount>(smelter);
        return smelter;
    }

    public Entity SpawnAssembler(int2 pos, Quaternion rotation, Entity recipe)
    {
        int2 output = int2.zero, input1 = int2.zero, input2 = int2.zero, pos2 = int2.zero, pos3 = int2.zero;
        switch (rotation.eulerAngles.z)
        {
            case 0:
                output = new int2(pos.x + 2, pos.y);
                input1 = new int2(pos.x - 1, pos.y + 1);
                input2 = new int2(pos.x - 1, pos.y - 1);
                pos2 = new int2(pos.x + 1, pos.y + 1);
                pos3 = new int2(pos.x + 1, pos.y - 1);
                break;
            case 90:
                output = new int2(pos.x, pos.y + 2);
                input1 = new int2(pos.x + 1, pos.y - 1);
                input2 = new int2(pos.x - 1, pos.y - 1);
                pos2 = new int2(pos.x + 1, pos.y + 1);
                pos3 = new int2(pos.x - 1, pos.y + 1);
                break;
            case 180:
                output = new int2(pos.x - 2, pos.y);
                input1 = new int2(pos.x + 1, pos.y + 1);
                input2 = new int2(pos.x + 1, pos.y - 1);
                pos2 = new int2(pos.x - 1, pos.y + 1);
                pos3 = new int2(pos.x - 1, pos.y - 1);
                break;
            case 270:
                output = new int2(pos.x, pos.y - 2);
                input1 = new int2(pos.x + 1, pos.y + 1);
                input2 = new int2(pos.x - 1, pos.y + 1);
                pos2 = new int2(pos.x + 1, pos.y - 1);
                pos3 = new int2(pos.x - 1, pos.y - 1);
                break;
        }
        var assembler = manager.Instantiate(buildings.assembler);
        manager.SetComponentData(assembler, LocalTransform.FromPositionRotation(new float3(pos.x, pos.y, 0f), rotation));
        manager.SetComponentData(assembler, new ProcessingBuildingComponent{recipe = recipe, isProducing = false, timer = 100f});
        manager.SetComponentData(assembler, new OutputProcessingBuildingComponent{ outputEntity = Entity.Null, pos = output, itemID = -1, itemCreated = true});
        SetPositions(assembler, new []{pos, input1, input2, pos2, pos3, new int2(pos.x, pos.y + 1), new int2(pos.x, pos.y - 1), new int2(pos.x + 1, pos.y), new int2(pos.x - 1, pos.y)});
        var inputs = manager.GetBuffer<InputComponent>(assembler);
        inputs.Add(new InputComponent{itemID = -1, pos = input1, occupied = false, lastItemID = -1});
        inputs.Add(new InputComponent{itemID = -1, pos = input2, occupied = false, lastItemID = -1});
        manager.AddBuffer<ItemAmount>(assembler);
        return assembler;
    }
    
    public Entity SpawnTrashcan(int2 pos)
    {
        var trashcan = manager.Instantiate(buildings.trashcan);
        manager.SetComponentData(trashcan, LocalTransform.FromPositionRotation(new float3(pos.x, pos.y, 0f), Quaternion.identity));
        var inputs = manager.GetBuffer<InputComponent>(trashcan);
        inputs.Add(new InputComponent{itemID = -1, pos = pos, occupied = false, lastItemID = -1});
        SetPositions(trashcan, new []{pos});
        return trashcan;
    }

    private void SetPositions(Entity entity, int2[] pos)
    {
        var positions = manager.GetBuffer<PositionComponent>(entity);
        foreach (var position in pos)
        {
            positions.Add(new PositionComponent{ pos = position });
        }
    }
}