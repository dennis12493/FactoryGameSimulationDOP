using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestGenerator : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private TileBase ironTile;
    [SerializeField] private TileBase coalTile;
    private Spawner spawner;
    public int rows;
    public int columns;
    private int i;
    void Start()
    {
        spawner = Spawner.Instance;
        tilemap = GameObject.Find("Resources").GetComponent<Tilemap>();
        //SpawnSteelBlueprint(0,0);
        i = 0;
    }

    private void Update()
    {
        if (i < rows)
        {
            for (int j = 0; j < columns; j++)
            {
                SpawnSteelBlueprint(j * 25,i * 5);    
            }
            i++;
        }
    }

    private void SpawnSteelBlueprint(int x, int y)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), ironTile);
        tilemap.SetTile(new Vector3Int(x, y + 2, 0), coalTile);
        spawner.SpawnMiningDrill(new int2(x, y), Quaternion.identity, Recipes.Instance.GetIronMineRecipe());
        spawner.SpawnConveyor(new Vector2(x + 1, y), new Vector2(x + 6, y), 0f);
        spawner.SpawnSmelter(new int2(x + 7, y), new Quaternion(), Recipes.Instance.GetIronIngotRecipe());
        spawner.SpawnConveyor(new Vector2(x + 9, y), new Vector2(x + 12, y), 0f);
        spawner.SpawnMiningDrill(new int2(x, y + 2), Quaternion.identity, Recipes.Instance.GetCoalMineRecipe());
        spawner.SpawnConveyor(new Vector2(x + 1, y + 2), new Vector2(x + 12, y + 2), 0f);
        spawner.SpawnAssembler(new int2(x + 14, y + 1), Quaternion.identity, Recipes.Instance.GetSteelIngotRecipe());
        spawner.SpawnConveyor(new Vector2(x + 16, y + 1), new Vector2(x + 20, y + 1), 0f);
        spawner.SpawnTrashcan(new int2(x + 21, y + 1));
    }
    
    private void SpawnIronBlueprint(int x, int y)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), ironTile);
        spawner.SpawnMiningDrill(new int2(x, y), Quaternion.identity, Recipes.Instance.GetIronMineRecipe());
        spawner.SpawnConveyor(new Vector2(x + 1, y), new Vector2(x + 6, y), 0f);
        spawner.SpawnSmelter(new int2(x + 7, y), new Quaternion(), Recipes.Instance.GetIronIngotRecipe());
        spawner.SpawnConveyor(new Vector2(x + 9, y), new Vector2(x + 12, y), 0f);
        spawner.SpawnTrashcan(new int2(x + 13, y));
    }
}
