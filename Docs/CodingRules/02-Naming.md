% Naming

- 클래스/공용 멤버: PascalCase
- 지역 변수/매개변수: camelCase
- [SerializeField] 비공개 필드: `_` 접두어 + camelCase (예: `_borderCanvas`)
- 이벤트 필드: `on...` 소문자 시작 권장 (예: `onTextShowed`)
- 이벤트/핸들러 메서드: `On...` (예: `OnCompleted`, `OnPointerEnter`)
- UI: `Show...`, `Hide...`, `Reset...`, `Init...`, 상태 변경 `Update...`
- 팩토리/생성: `Create...`; 조회: `Get...`
- 비동기 메서드: `...Async` 접미사
- 데이터 변경은 `Apply`가 아니라 `Update`

