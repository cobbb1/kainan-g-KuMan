using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStates : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PreferState = STATE_IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePreferState();
        CalculateState();
        ExecuteMovingState();
    }

    void CalculateState()
    {
        if (PreferState != State)
        {
            if (PreferState == STATE_EXHAUSTED)
            {
                State = STATE_EXHAUSTED;
            }
            else if (!IgnoreStateTransist)
            {
                State = PreferState;
            }
            if (State == STATE_EXHAUSTED)
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Attacking", false);
                animator.SetBool("Exhausted", true);
            }
            else if (State == STATE_IDLE)
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Attacking", false);
                animator.SetBool("Exhausted", false);
            }
            else if (State == STATE_MOVING)
            {
                animator.SetBool("Moving", true);
                animator.SetBool("Attacking", false);
                animator.SetBool("Exhausted", false);
            }
            else if (State == STATE_ATTACK)
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Attacking", true);
                animator.SetBool("Exhausted", false);
            }
        }
    }

    void ExecuteMovingState()
    {
        if (State == STATE_MOVING)
        {
            if (TargetPoint)
            {
                transform.position = transform.position + (TargetPoint.position - transform.position).normalized * Velocity * Time.deltaTime;
            }
            else if (Target)
            {
                Transform TargetTransform = Target.transform;
                transform.position = transform.position + (TargetTransform.position - transform.position).normalized * Velocity * Time.deltaTime;
            }
        }
    }

    void CalculatePreferState()
    {
        if (Hp <= 0)
        {
            PreferState = STATE_EXHAUSTED;
            return;
        }

        bool TargetExhausted = Target.GetComponent<ObjectStates>().GetExhausted();
        if (TargetPoint)
        {
            if (Vector3.Distance(transform.position, TargetPoint.position) > 0.000001)
            {
                PreferState = STATE_MOVING;
            }
            else
            {
                PreferState = STATE_IDLE;
            }
            RefreshRotation(TargetPoint);
        }
        else if (!TargetExhausted)
        {
            if (Vector3.Distance(transform.position, Target.transform.position) > Range)
            {
                PreferState = STATE_MOVING;
            }
            else
            {
                PreferState = STATE_ATTACK;
            }
            RefreshRotation(Target.transform);
        }
        else
        {
            PreferState = STATE_IDLE;
        }

        
    }

    void SetAttack()
    {

    }


    void BeginIgnoreTransist()
    {
        IgnoreStateTransist = true;
    }

    void EndIgnoreTransist()
    {
        IgnoreStateTransist = false;
    }

    void HitTarget()
    {
        if (Target)
        {
            ObjectStates TargetObjectState = Target.GetComponent<ObjectStates>();
            if (TargetObjectState)
            {
                TargetObjectState.Hp = TargetObjectState.Hp - Attack;
            }
        }
    }

    void RefreshRotation(Transform TargetTransform)
    {
        if (transform.position.x > TargetTransform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void Remove()
    {
        Destroy(gameObject);
    }

    bool GetExhausted()
    {
        return State == STATE_EXHAUSTED;
    }

    public float Attack = 2;
    public float MaxHp = 11;
    public float Velocity = 1;
    public float Range = 1;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int PreferState = 0;
    public int State = 0;
    public bool IgnoreStateTransist = false;
    public float Hp = 10;

    public GameObject Target;
    public Transform TargetPoint;

    static int STATE_MOVING = 1;
    static int STATE_ATTACK = 2;
    static int STATE_EXHAUSTED = -1;
    static int STATE_IDLE = 0;
    
}
