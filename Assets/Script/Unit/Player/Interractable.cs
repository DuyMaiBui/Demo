using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interractable : MonoBehaviour
{
    [Header("Interract Setting")]
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private float radiusCheck;

    [Header("Target")]
    [SerializeField] private GameObject targetDisplay;
    [SerializeField] private float radiusLockTarget;

    [Header("Weapon & Tool")]
    [SerializeField] ItemGO[] equipmentHandle;

    [SerializeField] private bool showGizmos;

    // Interract
    private LayerMask layerInteract;
    private Collider[] targetResult = new Collider[20];
    private Collider targetCollider;
    private Collider lastTargetCollider;
    private float rangeHit;
    private bool lockTarget = false;

    private ItemGO currentEquipment;
    private InputSetting _input;


    private void Start()
    {
        _input = InputSetting.instance;
        layerInteract = defaultLayer;
    }

    private void OnEnable()
    {
        EventDispatcher.ChangeEquipmentEvent += ChangeLayer;
    }

    private void OnDisable()
    {
        EventDispatcher.ChangeEquipmentEvent -= ChangeLayer;
    }

    private void Update()
    {
        if (_input.interract)
            SetAnimation();
    }

    private void FixedUpdate()
    {
        SetTarget();
    }

    private void SetTarget()
    {
        if (_input.leftClick)
        {
            _input.leftClick = false;
            targetCollider = GetCollider.GetMouseCollider(_input.position, layerInteract);
            if (targetCollider != null)
            {
                if (lastTargetCollider != targetCollider || targetCollider.transform.position != targetDisplay.transform.position)
                    SetTargetDisplay();
                lastTargetCollider = targetCollider;
                TargetState(true);
            }
            else
            {
                TargetState(false);
            }
        }
        if (lockTarget)
        {
            if (targetCollider == null || !CheckDistance(radiusLockTarget, targetCollider.transform) || targetCollider.gameObject.activeInHierarchy == false)
            {
                TargetState(false);
                targetCollider = null;
            }
        }
        else
        {
            targetCollider = GetCollider.GetColliderAround(transform, radiusCheck, targetResult, layerInteract);
            if (targetCollider != null)
            {
                targetDisplay.SetActive(true);
                if (lastTargetCollider != targetCollider || targetCollider.transform.position != targetDisplay.transform.position)
                    SetTargetDisplay();
                lastTargetCollider = targetCollider;
            }
            else
            {
                targetDisplay.SetActive(false);
            }
        }
    }

    private void SetTargetDisplay()
    {
        targetDisplay.transform.position = targetCollider.transform.position;
        float scale = targetCollider.bounds.size.x > targetCollider.bounds.size.z ? targetCollider.bounds.size.x : targetCollider.bounds.size.z;
        targetDisplay.transform.localScale = new Vector3(scale, 0.5f, scale);
    }

    private void TargetState(bool state)
    {
        lockTarget = state;
        targetDisplay.SetActive(state);
    }

    private void ChangeLayer(EquipmentSO lastEquipment, EquipmentSO newEquipment)
    {
        if (newEquipment == null)
        {
            layerInteract = defaultLayer;
            this.rangeHit = 0f;
            if (currentEquipment != null)
                currentEquipment.gameObject.SetActive(false);
        }
        else if (newEquipment.catalog == EquipmentCatalog.Weapon)
        {
            layerInteract = newEquipment.layerInteract;
            this.rangeHit = newEquipment.rangeHit;
            for (int i = 0; i < equipmentHandle.Length; i++)
            {
                if (equipmentHandle[i].data.item.GetEquipment() == newEquipment)
                {
                    currentEquipment = equipmentHandle[i];
                    currentEquipment.gameObject.SetActive(true);
                }
                else
                    equipmentHandle[i].gameObject.SetActive(false);
            }
        }
    }

    private bool CheckDistance(float maxDistance, Transform target)
    {
        return (Vector3.Distance(transform.position, target.position) > maxDistance) ? false : true;
    }

    private void SetAnimation()
    {
        _input.interract = false;
        if (targetCollider == null)
            EventDispatcher.Attack();
        else
        {
            LookAtTarget(targetCollider.transform);
            switch (targetCollider.gameObject.layer)
            {
                case 6:
                    if (CheckDistance(rangeHit, targetCollider.transform))
                        EventDispatcher.Attack();
                    break;
                case 7:
                    if (CheckDistance(1f, targetCollider.transform))
                        EventDispatcher.Collect();
                    break;
                case 8:
                    if (CheckDistance(rangeHit, targetCollider.transform))
                        EventDispatcher.Farm();
                    break;
                case 9:
                    if (CheckDistance(rangeHit, targetCollider.transform))
                        EventDispatcher.Farm();
                    break;
                default:
                    break;
            }
        }
    }

    private void LookAtTarget(Transform target)
    {
        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
    }

    public void CollectItem()
    {
        if (targetCollider == null)
            return;
        EventDispatcher.ChangedEnergy(40, false);
        if (!CheckDistance(radiusCheck, targetCollider.transform))
            return;
        EventDispatcher.ChangedEnergy(40, false);
        var item = targetCollider.gameObject.GetComponent<ItemGO>();
        EventDispatcher.AddItem(ItemHolderType.Inventory, item);
    }

    public void AttackEvent()
    {
        if (targetCollider == null)
            return;
        EventDispatcher.ChangedEnergy(70, false);
        if (!CheckDistance(radiusCheck, targetCollider.transform))
            return;
        EventDispatcher.ChangedEnergy(70, false);
        EventDispatcher.ChangedEndurance(0, ItemHolderType.Equipment, currentEquipment.data.item.enduranceRatio);
        if (targetCollider.TryGetComponent<IModifierStat>(out IModifierStat damage))
        {
            damage.Modifier(BuffStat.Health, -EventDispatcher.GetStatValue(BuffStat.Strength));
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, radiusCheck);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radiusLockTarget);
            Gizmos.color = Color.red;
        }
    }
}
