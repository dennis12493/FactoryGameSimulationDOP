using System;
using Unity.Entities;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public const int COAL = 3;
    public const int IRON_ORE = 0;
    public const int IRON_INGOT = 4;
    public const int WOOD = 2;
    public const int STEEL = 6;

    public int GetItemID(String tileName)
    {
        switch (tileName)
        {
            case "iron_tile":
                return IRON_ORE;
            case "tree_tile":
                return WOOD;
            case "coal_tile":
                return COAL;
            default:
                return -1;
        }
    }
}
