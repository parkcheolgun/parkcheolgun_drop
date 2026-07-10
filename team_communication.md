# 작업 이력 (Team Communication Log)

작업/커뮤니케이션 이력을 시간순으로 기록합니다. 새 작업은 최상단에 추가합니다.

---

## 2026-07-10 — 프로토타입 기획서 기반 코어 루프 구현 (Claude Code)

**브랜치**: `claude/drop-blob-prototype-spec-kpl3e6`
**PR**: [#1 프로토타입 기획서 기반 코어 루프 구현](https://github.com/parkcheolgun/parkcheolgun_drop/pull/1) (draft, open)

### 배경
- 구글 드라이브에서 "Drop the Blob — 초기 프로토타입 기획서(Core Loop Only)" 문서를 확인.
- 기존 저장소 코드(`GridGenerator.cs` 9x9, `BlockController.cs` 자유 드래그+스냅)는 색상/방향/매칭/벽 규칙이 전혀 없어 기획서와 충돌 — 삭제 후 재구현하기로 결정.

### 구현 내용
- **삭제**: `Assets/Script/GridGenerator.cs`, `Assets/Script/BlockController.cs` (+ .meta)
- **신규 스크립트** (`Assets/Script/`)
  - `GridTypes.cs` — `ColorType`/`DirectionType` enum, `Opposite`/`ToOffset`/`ToColor` 유틸리티
  - `GridManager.cs` — 6x6 고정 격자(기획서 축소판), 칸별 오브젝트 점유 딕셔너리, 좌표 변환 (싱글톤)
  - `BoardObjectController.cs` — 고정 오브젝트(색상/방향), Start 시 격자에 자동 등록
  - `HoleController.cs` — 드래그 1회 = 인접 칸 1칸 이동, 다른 색 오브젝트 진입 차단(벽), 정면 방향 매칭 시 오브젝트 제거·정원 감소, 다중 셀 홀 형태 지원
  - `GameManager.cs` — 홀 추적 및 클리어 판정, R키로 스테이지 리셋(씬 재로드)
- **신규 프리팹** (`Assets/Prefab/`): `BoardObject.prefab`, `Hole.prefab` — 기획서 9.8절대로 에디터에서 드래그 배치해 테스트 레벨 2~5를 구성할 수 있도록 준비
- **씬 변경** (`SampleScene.unity`): `GridManager`/`RedObject`(BoardObjectController)로 교체, `Hole_Red`·`GameManager` 추가 → 기획서 테스트 레벨 1(최소 성립 확인) 재현, 카메라 Orthographic 전환

### QA (코드 리뷰 + 수동 로직 시뮬레이션, Unity 에디터 미실행)
- 실행 환경에 Unity 에디터가 없어 실제 Play 테스트는 불가 — 코드 추적 시뮬레이션과 씬/프리팹 YAML 참조 무결성 검사로 대체 검증.
- Level 1 시나리오(Hole_Red → RedObject, 위로 2회 드래그) 매칭/클리어 로직 수동 트레이스로 정상 동작 확인.
- 발견 후 수정한 버그 2건:
  1. `RedObject`에 남아있던 `BoxCollider2D`가 홀과 겹칠 때 마우스 입력을 가로챌 수 있는 잠재 위험 → 제거
  2. `HoleController`가 `color.ToColor()` 대입 시 프리팹의 반투명(alpha 0.6) 값을 완전 불투명으로 덮어쓰던 시각 버그 → alpha 보존하도록 수정

### 남은 작업 / 확인 필요 사항
- **실제 Unity 에디터에서 Play 테스트 필요** (컴파일 에러, 드래그 조작감, Missing Script 여부 등은 코드 리뷰로 확인 불가)
- `directionMarker`(방향 표시 화살표)가 프리팹에 연결 안 되어 있어, 방향성 오브젝트 배치 시 시각적으로 어느 쪽을 보는지 안 보임 — 화살표 스프라이트 제작 후 연결 필요
- 테스트 레벨 2~5(방향 매칭, 벽 차단, 다중 셀 홀, 홀 2개 이상)는 아직 씬에 미구성 — `BoardObject`/`Hole` 프리팹으로 직접 배치 필요
- PR #1은 draft 상태, CI 미구성(체크 0건), 리뷰 코멘트 없음 — 1시간 간격으로 상태 모니터링 중
