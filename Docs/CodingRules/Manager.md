# Coding Rules Manager (문서판)

이 문서는 “어떤 파일/상황에서 어떤 규칙 문서를 보면 되는가”를 간단 규칙으로 추천합니다. 코드(에디터 확장) 없이 문서만으로 운영되며, 아래 RuleMap을 사람이든 도구든 쉽게 해석할 수 있게 고정 포맷으로 제공합니다.

Quick Use
- 파일/폴더 경로를 보며 아래 RuleMap의 pattern과 매칭합니다.
- 제안된 문서 목록을 우선 순서대로 열어 확인합니다.
- 필요 시 TL;DR 체크리스트로 1차 자가검사 후 세부 문서를 확인합니다.

RuleMap
```
[RuleMap]
pattern:"/Function/"|"/Directing/" -> 04-Async-UniTask.md, 02-Naming.md, 07-Forbidden.md, 03-Namespaces.md
pattern:"/UI/"|"/View/"|"/Presenter/" -> 05-UI-Patterns.md, 02-Naming.md, 01-Core-Style.md, 03-Namespaces.md
pattern:"/Editor/" -> 03-Namespaces.md, 08-Comments-Formatting.md, 07-Forbidden.md
pattern:"/Common/"|"/SaveLoad/"|"/Stage/"|"/Util/" -> 01-Core-Style.md, 02-Naming.md, 06-Data-LINQ-Serialization.md, 08-Comments-Formatting.md
default -> 01-Core-Style.md, 02-Naming.md, 03-Namespaces.md, 07-Forbidden.md
```

결정 트리(자연어)
- UI/표시/입력/프레젠터인가? → UI-Patterns, Naming, Core-Style, Namespaces
- 컷신/타임라인/연출/함수(Function, Directing)인가? → Async-UniTask, Naming, Forbidden, Namespaces
- 에디터 도구/커스텀 인스펙터인가? → Namespaces(UNITY_EDITOR), Comments-Formatting, Forbidden
- 공통 유틸/세이브/스테이지/자료 처리인가? → Core-Style, Naming, Data-LINQ-Serialization, Comments-Formatting
- 위에 없으면 기본 세트(default)

TL;DR 체크리스트
- Namespaces: Waving.SlotGame[.Sub]
- Naming: public/클래스 PascalCase, local/param camelCase, [SerializeField] private는 `_` 접두어
- Async: UniTask + CancellationToken 파라미터, 이벤트 대신 반환값(bool/enum) 선호
- UI: Show/Hide/Reset/Init/Update 접두사, Presenter→View 단방향 참조, Popup은 Function을 참조하지 않음
- Data: 조회/변환/필터링은 LINQ 우선, 직렬화는 Newtonsoft.Json
- Forbidden: sealed, ??, var(필요 시 예외), UniTaskCompletionSource, 양방향 참조 금지
- Comments: 모든 함수 위 한글 요약 1~2줄, UTF-8 저장

FAQ
- 예외가 필요한가요? → 예외가 반드시 필요하면 해당 줄에 간단 주석으로 사유를 남기고(예: `// STYLE-EXCEPTION: var 허용 (긴 제네릭 타입)`), PR 설명에 링크하세요.
- 규칙 간 충돌? → Manager의 RuleMap 우선순서를 따릅니다. 문맥상 명확하지 않으면 Core-Style과 Naming을 기준으로 결정합니다.

