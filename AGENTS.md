# Repository Guidelines

## 프로젝트 구조 및 모듈 구성
BilliardsProtoType_github는 Unity 6000.0.58f1 기반 프로젝트입니다. `Assets/` 폴더는 게임플레이 콘텐츠를 담고 있으며 `Library/`, `Temp/`, `Logs/`는 빌드 캐시이므로 Git에 추가하지 마십시오.

## 개발 환경과 도구
Unity Hub에서 6000.0.58f1 에디터로 열고, Rider 또는 Unity 워크로드가 포함된 Visual Studio를 사용해 `.csproj` 동기화를 유지합니다. 패키지 변경 후에는 `Unity.exe -batchmode -nographics -projectPath "%CD%" -quit`을 실행해 솔루션을 재생성합니다. 대용량 에셋은 필요 시 `git lfs pull`로 동기화합니다.

## 코딩 스타일과 인코딩
C# 스크립트는 4스페이스 들여쓰기, 클래스·공용 멤버는 PascalCase, 지역 변수는 camelCase를 사용합니다. `[SerializeField] private Canvas _borderCanvas;`처럼 직렬화된 비공개 필드는 `_` 접두어를 권장합니다. 모든 에디터 전용 코드는 `#if UNITY_EDITOR` 블록으로 감싸고 공통 로직은 `Assets/Scripts/Common`에 모읍니다.

## 커밋 및 PR 지침
Git 로그는 `build:`, `modified:`와 같은 `<type>: <요약>` 접두어를 사용하므로 동일한 형식을 유지하고 언어는 한 커밋에서 일관되게 한국어나 영어 중 하나로 작성합니다. 생성물(`Library/`, `Temp/`, `Logs/`)은 커밋 대상에서 제외하고, PR에는 변경 목적, 테스트 결과 표기(`Tests: editmode` 등), 관련 이슈 링크, UI 변화가 있다면 전·후 스크린샷을 포함합니다.

## Agent 응답 원칙
이 문서를 따르는 모든 에이전트는 질의에 대한 답변을 반드시 한국어로 작성해야 하며, 영어 예시가 필요하더라도 한국어 설명을 우선 제공합니다. 코드를 작성하거나 파일을 생성할 때만 각 답변의 말미에는 `장점`과 `단점`을 요약한 문장을 명확히 적고, 이어서 후속 조치를 안내하는 `TodoNext` 항목을 제시해 주세요. 장점, 단점, TodoNext는 답변 가시성을 위해 아래 예시처럼 답변합니다.
- 예시
  장점
  1) LoopChoice와 UI 계층을 분리해 선택지 바인딩이 명확해졌고 재사용성이 높아졌습니다.
  
  단점
  1) PopupManager 등록 여부나 ChoiceElement 리셋 로직은 실제 장면에서 한 번 더 검증이 필요합니다.
  
  TodoNext
  1) Unity 씬에서 ChoicePopup이 PopupManager 리스트에 포함돼 있는지 확인 
  2) 선택 후 조건 저장/리셋이 정상 동작하는지 플레이 모드 테스트 진행

## 자료 디렉터리
- 원본 참조 아트: `WorkRef/`

## 한글 인코딩 유의사항
- 모든 문서는 UTF-8(BOM 없음)으로 저장해 한글이 깨지지 않도록 합니다.
- PowerShell에서 파일을 수정할 때는 `[System.IO.File]::WriteAllText` 및 `::AppendAllText`에 `[System.Text.Encoding]::UTF8`을 명시하거나 `Set-Content -Encoding UTF8`을 사용합니다.
- Git diff에서 한글이 물음표 등으로 보이면, 해당 파일을 UTF-8로 다시 저장한 뒤 커밋 전에 내용이 올바른지 재확인합니다.
- 에디터 기본 인코딩을 UTF-8로 강제하고, 혼합 인코딩 파일을 편집할 때는 저장 전에 미리보기로 한글이 정상 출력되는지 확인합니다.

# Code Guidelines

## 공통
- 모든 에디터 전용 코드는 `#if UNITY_EDITOR` 블록으로 감싸기
- 스크립트를 저장할 때는 UTF-8 인코딩을 유지해 한글이 깨지지 않도록 함
- 기다리는 팝업 로직은 이벤트 기반으로 작성하고, 이벤트를 발행하면 호출 측이 이를 구독해 후속 처리를 진행
- 여러 곳에서 사용하는 변수가 아니라면 전역변수로 만들지 않고, 함수 내부에 선언하기
- 한 상위 객체 안에서 다른 하위 객체들의 상태를 전역변수 bool로 선언하지 않고 해당 객체에 상태 전역변수 선언하기
- 예외 처리는 반드시 필요한 곳에만 사용
- 함수 안에 코드가 1줄이라면 함수 사용하지 않기
```
// 테스트 주석
protected override void Awake()
{
    base.Awake();
    Debug.Log("테스트 로그")
}
```

## 이름
- C# 스크립트는 4스페이스 들여쓰기, 클래스·공용 멤버는 PascalCase, 지역 변수는 camelCase를 사용
- `[SerializeField] private Canvas _borderCanvas;`처럼 직렬화된 비공개 필드는 `_` 접두어를 권장
- 실제로 호출되어 동작을 수행하는 함수나 Action 등의 이벤트는 `On...` 접두사를 사용하며, 주로 특정 행동이 완료된 이후 호출합니다 (`OnHide`, `OnCompleted`)
- UI 표시·숨김·초기화 함수는 `Show...`, `Hide...`, `Reset...` 패턴을 사용합니다 (`Map`, `ChoicePopup`, `MenuLayout` 등).
- 이벤트를 등록하거나 해지하는 함수는 `Event...` 접두사를 사용합니다 (`MapLayout.RegisterMarkerEvents`, `MapLayout.UnregisterMarkerEvents` 등).
- 이벤트를 실행하는 델리게이트나 이벤트 혹은 함수 이름은 `On`접두사를 사용합니다 (`OnPointerEnter`,`onStart`)
- 상태를 변경하는 함수는 `Update...` 접두사를 사용합니다 (`UpdateLockState`).
- 생성 로직은 `Create...` 접두사를 사용해 반환 역할을 명확히 합니다 (`Map.CreateLocationStates`, `Map.CreateConfirmationMessage`).
- 선행 세팅이나 메시지 준비 함수는 `Init...` 접두사를 사용합니다 (`MapConfirmPopup.InitSelection`).
- 비동기 작업을 수행하는 메서드는 `Async` 접미사로 구분합니다 (`TimelineDirector.PlayAsync`).
- 반환형이 있을 때는 `Get` 접두사 붙이기
- 데이터를 변경하는 함수는 접두사를 `Apply`가 아니라 `Update`를 사용

## 직렬화/역직렬화
- Newtonsoft.Json 사용

## 주석
- 함수 위에 간단 명료한 한글 주석 포함

## 네임스페이스
- `Aftertime.MyTinyStreamer`이 prefix가 되며, Scripts 폴더 하위 폴더가 그 다음 네임스페이스가 됨
- 네임스페이스는 총 3번만 올 수 있다 (예: `Aftertime.MyTinyStreamer.UI`)

## 비동기
- UniTask의 async/await 사용
- `await`가 있다면 `CancellationToken`으로 항상 관리
- `onFailed`, `onSuccessed` 등의 이벤트 대신 UniTask 반환형에 bool이나 enum을 반환하기
- WaitUntil이나 while에서 Yield나 WaitOfEndFrame을 하는 코드를 지양하고 이벤트 콜백 방식으로 처리

## 클래스 구조
- 내용이 적다면 1개 클래스 안에 모두 정의
- 내용이 많다면 여러 개 클래스에 분할 및 interface/abstract 등의 상속 적극 활용
- 클래스 구조 적용 전에 반드시 위 2개 사항 모두 예시를 들고 어느 것을 선택할지 제시하기

## UI
- Popup이 Function을 참조하지 않음
- Function이 Popup을 참조함

## 자료구조
- 자료구조에서 데이터를 조회/변환/필터링할 때는 LINQ 사용

## 금지 문법
- `sealed`
- `??`
- `var` (타입이 너무 길거나 복잡할 때는 예외 허용)
- 쌍방 참조
- UniTaskCompletionSource
- 아래처럼 불필요한 null 예외처리 금지
```
private bool DetermineAvailability(FunctionValue functionValue, string conditionKey)
{
	bool isAvailable = true;
	bool hasExplicitAvailability = functionValue.HasValue(ActionSelectionValue.IsAvailable);
	if (hasExplicitAvailability)
	{
		isAvailable = functionValue.Get<bool>(ActionSelectionValue.IsAvailable);
	}

	if (string.IsNullOrEmpty(conditionKey) == false)
	{
		SaveLoader loader = SaveLoader.Instance;
		if (loader != null)
		{
			GameConditionData conditionData = loader.CurGameConditionData;
			if (conditionData != null)
			{
				Dictionary<string, object> conditions = conditionData.Conditions;
				if (conditions != null)
				{
					object storedValue;
					bool hasStoredValue = conditions.TryGetValue(conditionKey, out storedValue);
					if (hasStoredValue && storedValue is bool)
					{
						bool storedBool = (bool)storedValue;
						if (storedBool)
						{
							isAvailable = false;
						}
					}
				}
			}
		}
	}

	return isAvailable;
}
```

## 기존 코드 스타일 가이드
간단 예시 (기존 `ConversationPresenter` 발췌)
```
using System;
using Aftertime.StorylineEngine;
using Cysharp.Threading.Tasks;

namespace Aftertime.SecretSome
{
    public class TimeWait : TypedFunction<TimeWaitValue>, IElementChangeFunc, IStopFunc
    {
        public event Action onStop;
        public event Action onResume;

        public override async UniTask StartFunction()
        {
            onStop?.Invoke();

            float delayTime = TextElement.FunctionValue.Get<float>(TimeWaitValue.DelayTime);

            try
            {
                await UniTask.WaitForSeconds(delayTime).AttachExternalCancellation(cts.Token);
                if (cts.IsCancellationRequested == false)
                    onResume?.Invoke();
            }
            catch (OperationCanceledException exception)
            {
            }
        }
        
        public TextElement GetNextElement()
        {
            int curElementIndex = TextElement.Index;
            TextElement nextTextElement = EpisodeData.GetNextTextElement(curElementIndex);
            return nextTextElement;
        }
    }

    public enum TimeWaitValue
    {
        DelayTime
    }
}
```
