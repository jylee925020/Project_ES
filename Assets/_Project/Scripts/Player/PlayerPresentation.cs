using UnityEngine;

/// <summary>
/// 플레이어 상태 변화에 따라 시각 표현을 갱신한다.
/// </summary>
public class PlayerPresentation : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;

    private PlayerState state;

    #region lifecycle
    private void Awake()
    {
        state = GetComponent<PlayerState>();
    }

    private void OnEnable()
    {
        state.OnFacingChanged += HandleFacingChanged;
    }

    private void Start()
    {
        ApplyFacing(state.IsFacingRight);
    }

    private void OnDisable()
    {
        state.OnFacingChanged -= HandleFacingChanged;
    }
    #endregion

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
}