using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TemplateObjects : MonoBehaviour
{
    [SerializeField] private int2[] tilesToCheck;
    private int2[] rotatedTileToCheck;
    private Tilemap tilemap;
    private Camera mainCamera;
    private Vector2? startPosition;
    private Vector2 endPosition;
    private Spawner spawner;
    
    void Start()
    {
        mainCamera = Camera.main;
        tilemap = GameObject.Find("Resources").GetComponent<Tilemap>();
        rotatedTileToCheck = new int2[tilesToCheck.Length];
        spawner = Spawner.Instance;
    }

    void Update()
    {
        endPosition = GetWorldPoint();
        transform.position = endPosition;
        // Object will be placed
        if (Input.GetMouseButtonDown(0))
        {
            if (tag.Equals("Conveyorbelt"))
            {
                //TODO check if occupied
                if (startPosition.HasValue)
                {
                    if (!startPosition.Value.x.Equals(endPosition.x) && !startPosition.Value.y.Equals(endPosition.y))
                    {
                        Debug.LogError("Error can only build in one direction");
                        return;
                    }

                    if (!BuildingDictionary.Instance.CheckIfOccupied(AllConveyorPositions(startPosition.Value,
                            endPosition)))
                    {
                        Entity e = spawner.SpawnConveyor(startPosition.Value, endPosition, transform.rotation.eulerAngles.z);
                        BuildingDictionary.Instance.InsertEntityInDictionary(AllConveyorPositions(startPosition.Value, endPosition), e);
                        startPosition = new Vector2?();
                    }
                }
                else
                {
                    startPosition = endPosition;
                }
            }
            else
            {
                if (!BuildingDictionary.Instance.CheckIfOccupied(AllPositions()))
                {
                    SpawnEntity(endPosition);
                    
                }    
            }
        }
    }
    
    private void SpawnEntity(Vector2 position)
    {
        var pos = new int2((int)position.x, (int)position.y);
        if (gameObject.tag.Equals("MiningDrill"))
        {
            if (!tilemap.HasTile(new Vector3Int(pos.x, pos.y, 0))) return;
            var recipe = Recipes.Instance.GetMineRecipe(tilemap.GetTile(new Vector3Int(pos.x, pos.y, 0)).name);
            var e = spawner.SpawnMiningDrill(pos, transform.rotation, recipe);
            BuildingDictionary.Instance.InsertEntityInDictionary(AllPositions(), e);
        }
        else if (gameObject.tag.Equals("Smelter"))
        {
            var e = spawner.SpawnSmelter(pos, transform.rotation, Recipes.Instance.GetIronIngotRecipe());
            BuildingDictionary.Instance.InsertEntityInDictionary(AllPositions(), e);
        }
        else if (gameObject.tag.Equals("Assembler"))
        {
            var e = spawner.SpawnAssembler(pos, transform.rotation, Recipes.Instance.GetSteelIngotRecipe());
            BuildingDictionary.Instance.InsertEntityInDictionary(AllPositions(), e);
        }
        else if (gameObject.tag.Equals("Trashcan"))
        {
            var e = spawner.SpawnTrashcan(pos);
            BuildingDictionary.Instance.InsertEntityInDictionary(AllPositions(), e);
        }
    }

    private Vector2 GetWorldPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane tileMap = new Plane(new Vector3(0, 0, 1), 0);
        tileMap.Raycast(ray, out float distance);
        var point = ray.GetPoint(distance);
        return new Vector2(Mathf.Round(point.x), Mathf.Round(point.y));
    }

    private void RotateTilesToCheck(float rotation)
    {
        for (int i = 0; i < tilesToCheck.Length; i++)
        {
            int2 v = tilesToCheck[i];
            switch (rotation)
            {
                case 0:
                    rotatedTileToCheck[i] = new int2(v.x, v.y);
                    break;
                case 90:
                    rotatedTileToCheck[i] = new int2(v.y * -1, v.x);
                    break;
                case 180:
                    rotatedTileToCheck[i] = new int2(v.x * -1, v.y * -1);
                    break;
                case 270:
                    rotatedTileToCheck[i] = new int2(v.y, v.x * -1);
                    break;
            }
        }

    }

    private int2[] AllPositions()
    {
        float rotation = transform.eulerAngles.z;
        RotateTilesToCheck(rotation);
        int2[] result = new int2[tilesToCheck.Length];
        int2 position = new int2((int)transform.position.x, (int)transform.position.y);
        for (int i = 0; i < tilesToCheck.Length; i++)
        {
            result[i] = rotatedTileToCheck[i] + position;
        }
        return result;
    }

    private int2[] AllConveyorPositions(Vector2 start, Vector2 end)
    {
        var distance = (int) Vector2.Distance(start, end);
        Vector2 direction = (end - start).normalized;
        int2[] positions = new int2[distance + 1];
        for (int i = 0; i < distance + 1; i++)
        {
            Vector2 pos = start + (direction * i);
            positions[i] = new int2(pos);
        }
        return positions;
    }
}