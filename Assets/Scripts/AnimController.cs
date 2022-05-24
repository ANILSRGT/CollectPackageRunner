using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Run,
    Dash,
}

[RequireComponent(typeof(Animator))]
public class AnimController : MonoBehaviour
{
    [HideInInspector] public Vector3 deltaPos;
    [HideInInspector]
    public float currentAnimTime
    {
        get
        {
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] animatorClip = animator.GetCurrentAnimatorClipInfo(0);
            float time = animatorClip[0].clip.length * animationState.normalizedTime;
            return time;
        }

        private set { }
    }

    public Animator animator;
    public PlayerState playerState { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        SetState(PlayerState.Idle);
    }

    public void SetState(PlayerState state)
    {
        playerState = state;
        animator.SetInteger("State", (int)state);
    }

    private void OnAnimatorMove()
    {
        deltaPos = animator.deltaPosition;
    }
}
