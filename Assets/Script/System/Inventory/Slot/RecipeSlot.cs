using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeSlot : MonoBehaviour
{
    [SerializeField] private Image imageShow;
    private RecipeSO recipe;

    private void OnEnable()
    {
        EventDispatcher.ShowRecipeEvent += ShowRecipe;
        AddEventTrigger.AddEvent(gameObject, EventTriggerType.PointerClick, delegate { ShowDetail(this.recipe); });
    }

    private void OnDisable()
    {
        EventDispatcher.ShowRecipeEvent -= ShowRecipe;
    }

    private void ShowRecipe(Transform slot, RecipeSO recipe)
    {
        if (this.transform != slot)
            return;
        this.recipe = recipe;
        imageShow.sprite = recipe.resultItem.iconItem;
    }

    private void ShowDetail(RecipeSO recipe)
    {
        EventDispatcher.HideIngredient();
        EventDispatcher.ShowDetail(recipe);
    }
}
