using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Components;
using ECS.Components.Other;
using Unity.Entities;
using UnityEngine;

public class Recipes : MonoBehaviour
{
    private EntityManager manger;
    private EntityArchetype recipeArchetype;
    public static Recipes Instance { get; private set; }

    private Entity steelIngotRecipe;
    private Entity ironIngotRecipe;
    private Entity coalMineRecipe;
    private Entity ironMineRecipe;

    private void Awake()
    {
        Instance = this;
        manger = World.DefaultGameObjectInjectionWorld.EntityManager;
        recipeArchetype = manger.CreateArchetype(typeof(RecipeComponent), typeof(ItemAmount));
        CreateRecipes();
    }

    private void CreateRecipes()
    {
        CreateSteelIngotRecipe();
        CreateIronIngotRecipe();
        CreateCoalMineRecipe();
        CreateIronMineRecipe();
    }

    private void CreateSteelIngotRecipe()
    {
        steelIngotRecipe = manger.CreateEntity(recipeArchetype);
        manger.AddComponentData(steelIngotRecipe,
            new RecipeComponent { timeToCraft = 4f, outputItemID = ItemAssets.STEEL });
        var buffer = manger.AddBuffer<ItemAmount>(steelIngotRecipe);
        buffer.Add(new ItemAmount(ItemAssets.IRON_INGOT, 1));
        buffer.Add(new ItemAmount(ItemAssets.COAL, 2));
    }
    
    private void CreateIronIngotRecipe()
    {
        ironIngotRecipe = manger.CreateEntity(recipeArchetype);
        manger.AddComponentData(ironIngotRecipe,
            new RecipeComponent { timeToCraft = 3f, outputItemID = ItemAssets.IRON_INGOT });
        var buffer = manger.AddBuffer<ItemAmount>(ironIngotRecipe);
        buffer.Add(new ItemAmount(ItemAssets.IRON_ORE, 1));
    }

    private void CreateCoalMineRecipe()
    {
        coalMineRecipe = manger.CreateEntity(recipeArchetype);
        manger.AddComponentData(coalMineRecipe,
            new RecipeComponent { timeToCraft = 2f, outputItemID = ItemAssets.COAL });
    }
    
    private void CreateIronMineRecipe()
    {
        ironMineRecipe = manger.CreateEntity(recipeArchetype);
        manger.AddComponentData(ironMineRecipe,
            new RecipeComponent { timeToCraft = 2f, outputItemID = ItemAssets.IRON_ORE });
    }

    public Entity GetSteelIngotRecipe()
    {
        return steelIngotRecipe;
    }

    public Entity GetIronIngotRecipe()
    {
        return ironIngotRecipe;
    }

    public Entity GetCoalMineRecipe()
    {
        return coalMineRecipe;
    }

    public Entity GetIronMineRecipe()
    {
        return ironMineRecipe;
    }
    
    public Entity GetMineRecipe(String tilename)
    {
        var id = ItemAssets.Instance.GetItemID(tilename);
        switch (id)
        {
            case ItemAssets.COAL: return coalMineRecipe;
            case ItemAssets.IRON_ORE: return ironMineRecipe;
            default: return Entity.Null;
        }
    }

}
