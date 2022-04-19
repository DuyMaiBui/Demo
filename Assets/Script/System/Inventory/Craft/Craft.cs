using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Craft : MonoBehaviour
{
    [SerializeField] private GameObject craftInformation;
    [SerializeField] private Transform ingredientParent;

    [Space(10)]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button craftButton;
    [SerializeField] private Image fillProcess;
    [SerializeField] private string slotKey;

    [Space(10)]
    [SerializeField] private Vector2 space;
    [SerializeField] private Vector2 place;

    private RecipeSO recipeCraft;

    private void Awake()
    {
        craftButton.onClick.AddListener(delegate { CraftItem(); });
        EventDispatcher.ShowDetailEvent += ShowDetail;
        EventDispatcher.HideIngredientEvent += HideIngredient;
        EventDispatcher.ChangeDetailRecipeStateEvent += ChangeDetailRecipeState;
        fillProcess.fillAmount = 0;

        ChangeDetailRecipeState(false);
    }

    private void OnDisable()
    {
        EventDispatcher.ShowDetailEvent -= ShowDetail;
        EventDispatcher.HideIngredientEvent -= HideIngredient;
        EventDispatcher.ChangeDetailRecipeStateEvent -= ChangeDetailRecipeState;
    }

    private void ChangeDetailRecipeState(bool state)
    {
        if(state)
            craftInformation.transform.DOScaleY(1, 0.25f).SetEase(Ease.Flash);
        else
            craftInformation.transform.DOScaleY(0, 0.25f).SetEase(Ease.Flash);
    }

    private void CraftItem()
    {
        EventDispatcher.Craft(recipeCraft, fillProcess);
    }

    private void ShowDetail(RecipeSO recipe)
    {
        EventDispatcher.CheckRecipe(recipe, craftButton);
        recipeCraft = recipe;
        ChangeDetailRecipeState(true);
        itemImage.sprite = recipe.resultItem.iconItem;
        itemName.text = recipe.resultItem.itemName;
        description.text = recipe.resultItem.description;
        place.x = -space.x * (recipe.ingredientItem.Length - 1) / 2;
        for (int i = 0; i < recipe.ingredientItem.Length; i++)
        {
            var item = ObjectPoolManager.instance.InstanceGameObject(slotKey);
            EventDispatcher.ShowIngredient(item.transform, recipe.ingredientItem[i]);
            item.transform.SetParent(ingredientParent, false);
            item.transform.localPosition = SetPosition(i);
        }
    }

    private void HideIngredient()
    {
        for (int i = 0; i < ingredientParent.childCount; i++)
        {
            ObjectPoolManager.instance.DeactiveGameObject(slotKey, ingredientParent.GetChild(i).gameObject);
        }
    }

    private Vector2 SetPosition(int index)
    {
        return new Vector2(place.x + space.x * index, place.y);
    }
}
