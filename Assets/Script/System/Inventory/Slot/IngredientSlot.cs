using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        EventDispatcher.ShowIngredientEvent += ShowIngredient;
    }

    private void OnDisable()
    {
        EventDispatcher.ShowIngredientEvent -= ShowIngredient;
    }

    private void ShowIngredient(Transform slot, Ingredient ingredient)
    {
        if (slot != this.transform)
            return;
        image.sprite = ingredient.ingredientItem.iconItem;
        text.text = ingredient.amount > 1 ? ingredient.amount.ToString() : string.Empty;
    }
}
