% Examples

// 예시 1) UniTask + CancellationToken
// 간단 설명: 토큰으로 취소 가능한 재생
public async UniTask PlayAsync(TimelineAsset asset, CancellationToken token)
{
    _playableDirector.Play(asset);
    try
    {
        await UniTask.WaitUntil(() => _playableDirector.state == PlayState.Paused, cancellationToken: token);
    }
    catch (OperationCanceledException)
    {
        // 취소 시 후속 처리
    }
}

// 예시 2) UI Show/Hide/Reset
// 간단 설명: Presenter에서 View를 제어
public void Show(float duration, bool useRaycast = true)
{
    _conversationView.FadeCanvasGroup(1, duration);
    _conversationView.SetInteractable(useRaycast);
    _conversationView.SetBlocksRaycasts(useRaycast);
}

