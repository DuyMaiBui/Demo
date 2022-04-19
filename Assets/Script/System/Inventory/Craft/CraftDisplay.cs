using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class CraftDisplay : MonoBehaviour
{
    [Header("Tab Show")]
    [SerializeField] Outline[] tabs;

    [Header("Change Sort")]
    [SerializeField] Sprite sortNameSprite;
    [SerializeField] Sprite sortTypeSprite;

    [Space(10)]
    [SerializeField] private string slotKey;
    [SerializeField] private RecipeDatabaseSO database;

    private List<RecipeSO> ingredientShow = new List<RecipeSO>();
    private List<RecipeSO> ingredientSort = new List<RecipeSO>();
    private RecipeType lastShowType = RecipeType.Tool;

    private void Start()
    {
        AddEventTrigger.AddEvent(tabs[0].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.None, tabs[0]); });
        AddEventTrigger.AddEvent(tabs[1].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Tool, tabs[1]); });
        AddEventTrigger.AddEvent(tabs[2].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Weapon, tabs[2]); });
        AddEventTrigger.AddEvent(tabs[3].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Cloth, tabs[3]); });
        AddEventTrigger.AddEvent(tabs[4].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Food, tabs[4]); });
        AddEventTrigger.AddEvent(tabs[5].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Material, tabs[5]); });
        AddEventTrigger.AddEvent(tabs[6].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Machine, tabs[6]); });
        AddEventTrigger.AddEvent(tabs[7].gameObject, EventTriggerType.PointerClick, delegate { ShowIngredient(RecipeType.Building, tabs[7]); });
        Sort();

        ShowIngredient(RecipeType.None, tabs[0]);
    }

    private void ShowIngredient(RecipeType ingredientType, Outline tab)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].enabled = false;
        }
        tab.enabled = true;
        if (lastShowType == ingredientType)
            return;
        lastShowType = ingredientType;
        ingredientShow.Clear();
        DeactiveSlot();
        if (ingredientType == RecipeType.None)
        {
            for (int i = 0; i < database.recipes.Length; i++)
            {
                ingredientShow.Add(database.recipes[i]);
            }
        }
        else
        {
            for (int i = 0; i < database.recipes.Length; i++)
            {
                if (database.recipes[i].type == ingredientType)
                    ingredientShow.Add(database.recipes[i]);
            }
        }
        Sort();
        InstanceSlot();
    }

    private void InstanceSlot()
    {
        foreach (var item in ingredientSort)
        {
            var slot = ObjectPoolManager.instance.InstanceGameObject(slotKey);
            EventDispatcher.ShowRecipe(slot.transform, item);
            slot.transform.SetParent(transform, false);
        }
    }

    private void Sort()
    {
        ingredientSort = ingredientShow.OrderBy(ingredient => ingredient.level).ToList();
    }

    private void DeactiveSlot()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ObjectPoolManager.instance.DeactiveGameObject(slotKey, transform.GetChild(i).gameObject);
        }
    }
}