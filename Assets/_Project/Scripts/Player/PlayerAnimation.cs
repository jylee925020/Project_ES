using UnityEngine;

/// <summary>
/// 플레이어의 상태에 따라 기본 애니메이션을 선택하고,
/// 외부에서 요청된 특별 애니메이션을 재생한다.
///
/// Animator는 상태를 판단하지 않고
/// 애니메이션 재생과 전환만 담당한다.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Base Animation")]
    [SerializeField] private AnimationData idleAnimation;
    [SerializeField] private AnimationData runAnimation;
    [SerializeField] private AnimationData jumpAnimation;
    [SerializeField] private AnimationData fallAnimation;

    [Header("Base Animation Threshold")]
    [SerializeField, Min(0f)]
    private float runningVelocityThreshold = 0.01f;

    [Header("Special Animation")]
    [SerializeField] private AnimationData landAnimation;
    [SerializeField] private AnimationData hitAnimation;
    [SerializeField] private AnimationData deathAnimation;

    private PlayerHealth health;
    private PlayerHitReceiver hitReceiver;
    private PlayerPhysics physics;
    private Weapon attack;
    private PlayerState state;
    private PlayerCombat combat;

    private AnimationData currentBaseAnimation;
    private AnimationData currentSpecialAnimation;
    private AnimationClip currentClip;

    private float specialAnimationElapsedTime;
    private float specialAnimationDuration;

    #region Lifecycle

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        hitReceiver = GetComponent<PlayerHitReceiver>();
        physics = GetComponent<PlayerPhysics>();
        state = GetComponent<PlayerState>();
        combat = GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        state.OnActionChanged += HandleActionChanged;
        state.OnLanded += PlayLandAnimation;
        hitReceiver.OnHit += PlayHitAnimation;
        health.OnDied += PlayDeathAnimation;
        combat.OnAttackStarted += RequestSpecialAnimation;
    }

    private void OnDisable()
    {
        state.OnActionChanged -= HandleActionChanged;
        state.OnLanded -= PlayLandAnimation;
        hitReceiver.OnHit -= PlayHitAnimation;
        health.OnDied -= PlayDeathAnimation;
        combat.OnAttackStarted -= RequestSpecialAnimation;
    }

    private void Update()
    {
        UpdateSpecialAnimation();
        UpdateBaseAnimation();
    }

    #endregion

    #region Base Animation

    // 특별한 애니메이션을 재생하고 있지 않다면 기본 애니메이션 중에 재생
    private void UpdateBaseAnimation()
    {
        // 공격 등의 게임 액션 중에는
        // Base 애니메이션으로 전환하지 않는다.
        if (state.CurrentAction != PlayerActionType.None)
            return;

        AnimationData animationData = SelectBaseAnimation();

        // Base 상태가 그대로라면 Special을 굳이 중단하지 않는다.
        if (currentBaseAnimation == animationData)
            return;

        // 착지 등의 Special이 최소 재생 구간을 지나지 않았다면 기다린다.
        if (!CanInterruptCurrentSpecialAnimation())
            return;

        ReleaseSpecialAnimation();
        PlayBaseAnimation(animationData);
    }

    private AnimationData SelectBaseAnimation()
    {
        if (!state.IsGrounded)
        {
            return physics.CurrentVelocityY > 0f
                ? jumpAnimation
                : fallAnimation;
        }

        bool isRunning =
            Mathf.Abs(physics.CurrentVelocityX)
            > runningVelocityThreshold;

        return isRunning
            ? runAnimation
            : idleAnimation;
    }

    private void PlayBaseAnimation(AnimationData animationData)
    {
        if (animationData == null)
            return;

        // 같은 기본 상태가 유지되는 동안에는
        // 새로운 후보 클립을 선택하거나 재생을 시작하지 않는다.
        if (currentBaseAnimation == animationData)
            return;

        AnimationClip clip = GetClip(animationData);

        if (clip == null)
            return;

        PlayClip(
            clip,
            animationData.PlaybackSpeed,
            animationData.TransitionDuration
        );

        currentBaseAnimation = animationData;
    }

    #endregion

    #region Special Animation
    private void UpdateSpecialAnimation()
    {
        if (currentSpecialAnimation == null)
            return;

        specialAnimationElapsedTime += Time.deltaTime;

        // 게임 액션에 묶인 애니메이션은
        // 로직의 종료 이벤트가 해제한다.
        if (state.CurrentAction != PlayerActionType.None)
            return;

        if (currentClip == null || currentClip.isLooping)
            return;

        if (specialAnimationElapsedTime >= specialAnimationDuration)
        {
            ReleaseSpecialAnimation();
        }
    }

    // 공격, 착지, 피격, 사망 등 특별 애니메이션 재생 요청
    private void RequestSpecialAnimation(AnimationData animationData)
    {
        if (animationData == null)
            return;

        if (!CanInterruptCurrentSpecialAnimation())
            return;

        AnimationClip clip = GetClip(animationData);

        if (clip == null)
            return;

        PlayClip(
            clip,
            animationData.PlaybackSpeed,
            animationData.TransitionDuration
        );

        currentSpecialAnimation = animationData;

        specialAnimationElapsedTime = 0f;

        float playbackSpeed =
            Mathf.Max(animationData.PlaybackSpeed, 0.0001f);

        specialAnimationDuration =
            clip.length / playbackSpeed;
    }

    private void HandleActionChanged(PlayerActionType action)
    {
        if (action != PlayerActionType.None)
            return;

        ReleaseSpecialAnimation();
        currentBaseAnimation = null;
    }

    private void ReleaseSpecialAnimation()
    {
        currentSpecialAnimation = null;
        specialAnimationElapsedTime = 0f;
        specialAnimationDuration = 0f;
    }


    #endregion

    #region Play General Special Animation
    private void PlayLandAnimation()
    {
        RequestSpecialAnimation(landAnimation);
    }

    private void PlayHitAnimation(HitInfo hitInfo)
    {
        RequestSpecialAnimation(hitAnimation);
    }

    private void PlayDeathAnimation()
    {
        RequestSpecialAnimation(deathAnimation);
    }
    #endregion


    #region Playback

    private AnimationClip GetClip(AnimationData animationData)
    {
        AnimationClip clip = animationData.GetRandomClip();

        if (clip != null)
            return clip;

        Debug.LogWarning(
            $"{animationData.name}에 재생 가능한 클립이 없습니다.",
            animationData
        );

        return null;
    }

    private void PlayClip(
        AnimationClip clip,
        float playbackSpeed,
        float transitionDuration)
    {
        currentClip = clip;

        animator.speed = playbackSpeed;

        animator.CrossFadeInFixedTime(
            clip.name,
            transitionDuration,
            0,
            0f
        );
    }

    private bool CanInterruptCurrentSpecialAnimation()
    {
        if (currentSpecialAnimation == null)
            return true;

        if (!currentSpecialAnimation.CanBeInterrupted)
            return false;

        if (specialAnimationDuration <= 0f)
            return true;

        float normalizedTime =
            specialAnimationElapsedTime /
            specialAnimationDuration;

        return normalizedTime >=
            currentSpecialAnimation.InterruptibleNormalizedTime;
    }

    #endregion
}