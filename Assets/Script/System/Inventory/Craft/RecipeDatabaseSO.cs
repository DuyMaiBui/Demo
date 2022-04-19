using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName ="Craft System/Database")]
public class RecipeDatabaseSO : ScriptableObject
{
    public RecipeSO[] recipes;
}
