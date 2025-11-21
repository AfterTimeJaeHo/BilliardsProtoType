% Async / UniTask

- UniTask의 async/await 사용
- `await`가 있다면 항상 `CancellationToken`으로 관리 (파라미터로 전달)
- `onFailed`/`onSuccessed` 이벤트 대신 반환값(bool/enum)으로 결과 표현
- `WaitUntil`, `while` + `Yield`/`WaitForEndOfFrame` 지양 → 이벤트/콜백 기반으로 전환

