using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Run,
    Dash,
    Victory,
    Lose,
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

    /// <summary>
    /// Set player's animation state
    /// </summary>
    /// <param name="state">Animation state</param>
    public void SetState(PlayerState state)
    {
        playerState = state;
        animator.SetInteger("State", (int)state);
    }

    /// <summary>
    /// Set deltapos for root motion
    /// </summary>
    private void OnAnimatorMove()
    {
        deltaPos = animator.deltaPosition;
    }
}
