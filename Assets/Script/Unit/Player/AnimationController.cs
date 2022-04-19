using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator anim;

    private int speedID;
    private int collectID;
    private int attackID;
    private int farmID;

    private void OnEnable()
    {
        speedID = Animator.StringToHash("Speed");
        collectID = Animator.StringToHash("Collect");
        attackID = Animator.StringToHash("Attack");
        farmID = Animator.StringToHash("Farm");

        EventDispatcher.MoveEvent += MoveAnim;
        EventDispatcher.CollectEvent += Collect;
        EventDispatcher.AttackEvent += Attack;
        EventDispatcher.FarmEvent += Farm;
    }

    private void OnDisable()
    {
        EventDispatcher.MoveEvent -= MoveAnim;
        EventDispatcher.CollectEvent -= Collect;
        EventDispatcher.AttackEvent -= Attack;
        EventDispatcher.FarmEvent -= Farm;
    }

    private void Farm()
    {
        anim.SetTrigger(farmID);
    }

    private void Collect()
    {
        anim.SetTrigger(collectID);
    }

    private void Attack()
    {
        anim.SetTrigger(attackID);
    }

    private void MoveAnim(float speed)
    {
        anim.SetFloat(speedID, speed);
    }
}
