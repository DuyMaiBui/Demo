using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public delegate float GetStat(BuffStat stat);

public class EventDispatcher : MonoBehaviour
{
    public static EventDispatcher instance;

    // Game
    public static event UnityAction<GameObject> SetItemParentEvent;
    public static event UnityAction<TileData> GenerateNatureItemEvent;
    public static event UnityAction ClearMapEvent;

    // Save Load
    public static event UnityAction SaveEvent;
    public static event UnityAction LoadEvent;
    public static event UnityAction<List<ItemSaveData>> SaveItemDataEvent;
    public static event UnityAction DeleteSaveDataEvent;
    public static event UnityAction<Vector3> SetMiniMapEvent;

    // UI
    public static event UnityAction<int> SetMapSizeEvent;
    public static event UnityAction<bool> SetRandomSeedEvent;
    public static event UnityAction<int> SetSeedEvent;
    public static event UnityAction<int> LoadGameEvent;
    public static event UnityAction<string, float> ChangedFillBarEvent;
    public static event UnityAction<string> SetFloatTextEvent;
    public static event UnityAction<Vector3, string> ShowToolTipEvent;
    public static event UnityAction HideToolTipEvent;

    // Player Control
    public static event UnityAction<Vector3, int> PlayerPositionEvent;
    public static event UnityAction SetPlayerMiniMapEvent;
    public static event UnityAction<float, bool> ChangedEnergyEvent;
    public static event UnityAction UnlockTargetEvent;

    // Animation
    public static event UnityAction<float> MoveEvent;
    public static event UnityAction<bool> JumpEvent;
    public static event UnityAction AttackEvent;
    public static event UnityAction CollectEvent;
    public static event UnityAction FarmEvent;

    // Stat
    public static event UnityAction<StatSystem, Transform> OutOfStatEvent;
    public static event UnityAction<StatSystem> ChangedStatEvent;
    public static event UnityAction OutOfEnergyEvent;
    public static event GetStat GetStatEvent;
    public static event UnityAction<BuffStat, float> ModifierStatEvent;

    // Inventory
    public static event UnityAction<ItemHolderType, ItemGO> AddItemEvent;
    public static event UnityAction<ItemGO, int> DecreaseAmountEvent;
    public static event UnityAction<EquipmentSO, EquipmentSO> ChangeEquipmentEvent;
    public static event UnityAction<Slot, int, Vector3> DropItemEvent;
    public static event UnityAction<bool> EnableStateBinEvent;
    public static event UnityAction<int, ItemHolderType, float> ChangedEnduranceEvent;

    // Craft
    public static event UnityAction<Transform, RecipeSO> ShowRecipeEvent;
    public static event UnityAction<RecipeSO> ShowDetailEvent;
    public static event UnityAction<Transform, Ingredient> ShowIngredientEvent;
    public static event UnityAction HideIngredientEvent;
    public static event UnityAction<RecipeSO, Image> CraftEvent;
    public static event UnityAction<RecipeSO, Button> CheckRecipeEvent;
    public static event UnityAction<bool> ChangeDetailRecipeStateEvent;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public static void ClearMap() => ClearMapEvent?.Invoke();

    public static void SetItemParent(GameObject unit) => SetItemParentEvent?.Invoke(unit);

    public static void GenerateNatureItem(TileData data) => GenerateNatureItemEvent?.Invoke(data);

    public static void LoadGame(int id) => LoadGameEvent?.Invoke(id);

    public static void ChangedStat(StatSystem stat) => ChangedStatEvent?.Invoke(stat);

    public static float GetStatValue(BuffStat stat)
    {
        if (GetStatEvent != null)
        {
            return GetStatEvent.Invoke(stat);
        }
        return 0;
    }

    public static void ModifierStat(BuffStat stat, float value) => ModifierStatEvent?.Invoke(stat, value);

    #region Animation
    public static void Move(float speed) => MoveEvent?.Invoke(speed);

    public static void Jump(bool jumpState) => JumpEvent?.Invoke(jumpState);

    public static void Attack() => AttackEvent?.Invoke();

    public static void Collect() => CollectEvent?.Invoke();

    public static void Farm() => FarmEvent?.Invoke();
    #endregion

    #region Player Control

    public static void PlayerPosition(Vector3 position, int mapSize) => PlayerPositionEvent?.Invoke(position, mapSize);

    public static void ChangedEnergy(float value, bool state) => ChangedEnergyEvent?.Invoke(value, state);

    public static void SetPlayerMiniMap() => SetPlayerMiniMapEvent?.Invoke();

    public static void UnlockTarget() => UnlockTargetEvent?.Invoke();
    #endregion

    #region Save Load
    public void DeleteSaveData() => DeleteSaveDataEvent?.Invoke();

    public void Load() => LoadEvent?.Invoke();

    public void Save() => SaveEvent?.Invoke();

    public static void SaveItemData(List<ItemSaveData> list) => SaveItemDataEvent?.Invoke(list);

    public static void SetMiniMapCamera(Vector3 position) => SetMiniMapEvent?.Invoke(position);

    public static void SetMapSize(int size) => SetMapSizeEvent?.Invoke(size);

    public static void SetRandomSeed(bool randomSeed) => SetRandomSeedEvent?.Invoke(randomSeed);

    public static void SetSeed(int value) => SetSeedEvent?.Invoke(value);
    #endregion

    #region Inventory Event
    public static void AddItem(ItemHolderType type, ItemGO item) => AddItemEvent?.Invoke(type, item);

    public static void DecreaseAmount(ItemGO item, int amount) => DecreaseAmountEvent?.Invoke(item, amount);

    public static void ChangeEquipment(EquipmentSO lastEquipment, EquipmentSO newEquipment) => ChangeEquipmentEvent?.Invoke(lastEquipment, newEquipment);


    public static void DropItem(Slot slot, int amount, Vector3 position) => DropItemEvent?.Invoke(slot, amount, position);

    public static void EnableStateBin(bool state) => EnableStateBinEvent?.Invoke(state);

    public static void ChangedEndurance(int index, ItemHolderType type, float enduranceChange) => ChangedEnduranceEvent?.Invoke(index, type, enduranceChange);
    #endregion

    #region Craft
    public static void ShowDetail(RecipeSO ingredient) => ShowDetailEvent?.Invoke(ingredient);

    public static void ShowRecipe(Transform slot, RecipeSO recipe) => ShowRecipeEvent?.Invoke(slot, recipe);

    public static void ShowIngredient(Transform slot, Ingredient ingredient) => ShowIngredientEvent?.Invoke(slot, ingredient);

    public static void HideIngredient() => HideIngredientEvent?.Invoke();

    public static void Craft(RecipeSO recipe, Image fill) => CraftEvent?.Invoke(recipe, fill);

    public static void CheckRecipe(RecipeSO recipe, Button button) => CheckRecipeEvent?.Invoke(recipe, button);

    public static void ChangeDetailRecipeState(bool state) => ChangeDetailRecipeStateEvent?.Invoke(state);
    #endregion

    #region UI

    public static void ChangedFillBar(string name, float value) => ChangedFillBarEvent?.Invoke(name, value);

    public static void SetFloatText(string text) => SetFloatTextEvent?.Invoke(text);

    public static void OutOfStat(StatSystem stat, Transform unit) => OutOfStatEvent?.Invoke(stat, unit);

    public static void OutOfEnergy() => OutOfEnergyEvent?.Invoke();

    public static void ShowToolTip(Vector3 position, string text) => ShowToolTipEvent?.Invoke(position, text);

    public static void HideToolTip() => HideToolTipEvent?.Invoke();
    #endregion
}
