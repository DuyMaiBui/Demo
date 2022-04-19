using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Craft System/Recipe")]
public class RecipeSO : ScriptableObject
{
    public int level = 1;
    public float timeCraft = 1f;
    public RecipeType type;
    public ItemSO resultItem;
    public Ingredient[] ingredientItem;
}

[System.Serializable]
public struct Ingredient
{
    public ItemSO ingredientItem;
    public int amount;
}

public enum RecipeType
{
    None,
    Tool,
    Weapon,
    Cloth,
    Food,
    Material,
    Machine,
    Building
}