using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 상태 변화에 따라 시각 표현을 갱신한다.
/// 방향 전환과 피격 무적 깜빡임을 담당한다.
/// </summary>
public class PlayerPresentation : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform playerRoot;

    [Header("Invincibility Blink")]
    [SerializeField, Range(0f, 1f)]
    private float invincibleAlpha = 0.3f;

    [SerializeField]
    private float blinkInterval = 0.1f;

    private PlayerState state;
    private PlayerHitReceiver hitReceiver;

    private SpriteRenderer[] spriteRenderers;
    private Coroutine blinkRoutine;

    #region Lifecycle

    private void Awake()
    {
        state = GetComponent<PlayerState>();
        hitReceiver = GetComponent<PlayerHitReceiver>();

        spriteRenderers =
            playerRoot.GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void OnEnable()
    {
        state.OnFacingChanged += HandleFacingChanged;

        hitReceiver.OnInvincibilityStarted += StartInvincibilityBlink;
        hitReceiver.OnInvincibilityEnded += StopInvincibilityBlink;
    }

    private void Start()
    {
        ApplyFacing(state.IsFacingRight);
        SetAlpha(1f);
    }

    private void OnDisable()
    {
        state.OnFacingChanged -= HandleFacingChanged;

        hitReceiver.OnInvincibilityStarted -= StartInvincibilityBlink;
        hitReceiver.OnInvincibilityEnded -= StopInvincibilityBlink;

        StopInvincibilityBlink();
    }

    #endregion

    #region Facing

    private void HandleFacingChanged(bool isFacingRight)
    {
        ApplyFacing(isFacingRight);
    }

    private void ApplyFacing(bool isFacingRight)
    {
        Vector3 scale = playerRoot.localScale;

        scale.x = Mathf.Abs(scale.x)
            * (isFacingRight ? 1f : -1f);

        playerRoot.localScale = scale;
    }

    #endregion

    #region Invincibility Blink

    private void StartInvincibilityBlink()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(InvincibilityBlinkRoutine());
    }

    private void StopInvincibilityBlink()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        SetAlpha(1f);
    }

    private IEnumerator InvincibilityBlinkRoutine()
    {
        bool isTransparent = false;

        while (true)
        {
            isTransparent = !isTransparent;

            SetAlpha(isTransparent ? invincibleAlpha : 1f);

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    #endregion
}