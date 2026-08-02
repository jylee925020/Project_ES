using UnityEngine;

/// <summary>
/// 하나의 의미를 공유하는 애니메이션 클립들과
/// 공통 재생 설정을 보유한다.
/// </summary>
[CreateAssetMenu(
    fileName = "AnimationData",
    menuName = "Scriptable Objects/Animation Data"
)]
public class AnimationData : ScriptableObject
{
    [Header("Clips")]
    [SerializeField] private AnimationClip[] clips;

    [Header("Playback")]
    [SerializeField, Min(0f)] private float playbackSpeed = 1f;
    [SerializeField, Min(0f)] private float transitionDuration = 0.05f;

    /// <summary>
    /// canBeInturrupted가 false면 절대 인터럽트 불가
    /// canBeInturrupted가 true면 interruptibleNormalizedTime만큼 재생된 후에 인터럽트 가능
    /// </summary>
    [Header("Interruption")]
    [SerializeField] private bool canBeInterrupted = true;

    [SerializeField, Range(0f, 1f)]
    private float interruptibleNormalizedTime = 0f;     // 0.4면 40% 재생된 후에 인터럽트 가능

    public bool CanBeInterrupted => canBeInterrupted;
    public float InterruptibleNormalizedTime =>
        interruptibleNormalizedTime;

    public float PlaybackSpeed => playbackSpeed;
    public float TransitionDuration => transitionDuration;

    /// <summary>
    /// 등록된 클립 중 하나를 무작위로 반환한다.
    /// 유효한 클립이 없으면 null을 반환한다.
    /// </summary>
    public AnimationClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        int startIndex = Random.Range(0, clips.Length);

        // 배열 중간에 비어 있는 슬롯이 있더라도
        // 다른 유효한 클립을 찾는다.
        for (int offset = 0; offset < clips.Length; offset++)
        {
            int index = (startIndex + offset) % clips.Length;

            if (clips[index] != null)
                return clips[index];
        }

        return null;
    }
}