# PPE 포트폴리오 테스트 보고서

작성: 2026-10-27 (Asia/Seoul). 기준 커밋 `871f806165eebb9e03ba944ed95db5af3dc83819`, 브랜치 `codex/ppe-portfolio-improvement`.

**최신 실제 결과: Unity 6000.4.8f1 import·컴파일 통과, 신규 EditMode 50/50 통과. 전체 EditMode는 217/218 통과이며 기존 손 색상 테스트 1개가 실패했다.** PCVR·Quest·PlayMode smoke·새 player 빌드는 실행하지 않았다. 아래 설치 전 기록과 과거 인수인계의 통과 수치는 최신 결과와 구분한다.

## 최신 실제 Unity 검증 — 2026-10-27

사용자가 정확한 Editor 설치와 실제 실행을 승인한 뒤 진행했다. 설치된 실행 파일은 `<UNITY_EDITOR_6000.4.8f1>`, ProductVersion **`6000.4.8f1_f8b72d3d7343`**다. 라이선스 entitlement 확인과 Hub의 core Editor 설치 완료를 확인했다. 다른 버전은 사용하지 않았다.

원본의 최신 미커밋 **36개 파일**과 ignored 포트폴리오 제작 원본을 포함한 **48개 파일**을 `<BACKUP_ROOT>/ppe-before-unity-install-20261027T043043448168Z/uncommitted-work.zip`에 보관했다. ZIP CRC와 파일별 SHA-256 **48/48 일치**, ZIP SHA-256은 `65814b1ef23c3af9a41f4f426914548e53da51f76d088c281e3610f7635df231`이다. 백업 receipt(로컬 원본 보존)를 남겼다.

### 실행 대상과 보존

실제 실행은 현재 소스를 복사한 **`<VALIDATION_COPY>`**에서 했다. `.git`, Library/Temp/Logs/UserSettings와 기존 제작 중간물은 복사하지 않았다. Unity가 원본 작업 파일을 import·재직렬화하지 않게 하기 위한 실행 격리다. 기존 Assets 3,888개·Packages 2개·ProjectSettings 27개가 원본과 byte 단위로 일치함을 확인했다. 아래 fixture 보완도 원본과 복사본에 동일하게 반영한 뒤 실행했다.

게임플레이 runtime 5개는 이번 설치 전 백업과 byte 단위로 동일하다. 원본 Scene·Prefab·기존 GUID/meta·공용 XR/App·다른 Stage·Packages/ProjectSettings·기존 player는 변경하지 않았다. 원본의 추가 C# 도구는 `docs/portfolio/tools/UnityPpeValidationProbe.cs`이며 Unity import 대상이 아니다. 이 도구를 검증 복사본의 Editor 폴더에만 추가해 씬 검사와 discovery를 실행했다. Scene을 저장하지 않았다.

### 실제 실행 결과

| 실행 | 발견/실행 | 통과 | 실패 | Skip | 결과와 근거 |
|---|---:|---:|---:|---:|---|
| 최초 전체 import·컴파일 | 런타임/Editor 어셈블리 생성 | — | C# 오류 **0** | — | Unity 로그의 종료 code **0**, [진단 기록](public-validation/20261027-unity6000.4.8f1/diagnostics.txt) |
| 신규 50 최초 실행 | 50 | 49 | 1 | 0 | XRI attach 초기화가 빠진 fixture. [최초 XML](public-validation/20261027-unity6000.4.8f1/new-50-results.xml) 보존 |
| attach 보완 후 집중 재실행 | 50 | **50** | **0** | **0** | 프로세스 exit **0**, [XML](public-validation/20261027-unity6000.4.8f1/new-50-rerun-results.xml) |
| 첫 전체 EditMode | 218 | 216 | 2 | 0 | 기존 Tint 1개와 신규 fixture의 CLR-null/Unity-null 전제 1개. [XML](public-validation/20261027-unity6000.4.8f1/full-editmode-results.xml) |
| null 전제 보완 후 전체 재실행 | **218** | **217** | **1** | **0** | exit **2**. 신규 **50/50** 포함, 기존 **167/168**. [최종 XML](public-validation/20261027-unity6000.4.8f1/full-editmode-rerun-results.xml) |
| 변경 없는 기존 Tint 단독 실행 | 1 | 0 | 1 | 0 | 새 Editor 프로세스에서도 같은 실패 재현. [XML](public-validation/20261027-unity6000.4.8f1/existing-tint-only-results.xml) |
| 실제 PPE 씬 검사 + discovery | 전체 218 / 신규 50 | — | 검사 오류 **0** | — | probe exit **0**, [씬/discovery JSON](public-validation/20261027-unity6000.4.8f1/scene-and-discovery.json) |

최종 fixture별 결과는 **Sequence 13/13, Glove 17/17, Bag 20/20**이다. TestRunnerApi의 discovery에서 신규 50개 모두 `Runnable`이었고 full name 50개가 최종 전체 실행 XML의 신규 test-case 50개와 정확히 일치했다. **실행 50, 통과 50, 실패 0, Skip 0, 미발견 0**이며 count만 맞춘 것이 아니다. [기계 판독 요약](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json)에 목록 대조·각 회차·source SHA-256을 기록했다.

### fixture만 보완한 두 항목

1. `ResponseBagPackingTests.NewInteractor`: EditMode의 `AddComponent<XRRayInteractor>()`는 런타임 `XRBaseInteractor.Awake()`의 attach 생성 전제를 자동으로 충족하지 않았다. 가방 준비 중 선택 취소가 `RestoreAttachTransform()`을 호출하면서 null 예외가 났다. fixture가 interactor 자식 attach를 명시적으로 만든 뒤 등록하도록 보완했다. 실제 Hover/Select·가방 준비 경로와 기존 assertions는 유지했다.
2. `PPEPortfolioSequenceTests.RecordedPartialLoadout...`: 기존 AppFlow fixture가 파괴된 Unity wrapper를 static slot에 남길 수 있었다. CLR 참조 기준 `Assert.IsNull` 대신 Unity의 `Instance == null` 의미로 **살아 있는 Loadout이 없는지** 확인한다. 살아 있는 객체를 삭제하거나 통과시키지 않는다. 혼합 순서 실행에서 확인한 fixture 격리 전제 보완이다.

사례를 제거하거나 Skip하지 않았고 조건을 완화해 게임플레이를 통과시키지 않았다. 실행 중 발견한 fixture 준비 문제를 고친 두 파일 외의 게임 코드와 기존 Tint test는 보존했다.

### 남은 기존 테스트 실패

`PPEInnerGloveHandTintTests.ApplyToHand_TintsThenRestoresDefaultHandColor`는 기존 test 19행의 **정확한 Color 구조체 동등 비교**에서 실패한다. 이 test와 `PPEInnerGloveHandTint.cs`는 baseline HEAD와 줄바꿈 정규화 후 동일하며 새 프로세스 단독 실행에서도 실패했다. 별도 진단에서 다음 왕복값을 측정했다.

| 채널 | 기대 | 실제 MaterialPropertyBlock 읽기 |
|---|---:|---:|
| R | 0.378 | 0.377999961 |
| G | 0.495 | 0.494999945 |
| B | 0.492 | 0.491999924 |
| A | 1 | 1 |

최대 채널 차이는 **`8.940696716308594e-8`**이다. 현재 headless Editor에서 재현되는 기존 정밀도 비교 실패로 분류한다. non-headless 비교를 실행하지 않았으므로 headless에서만 발생한다고 단정하지 않는다. 기존 assertion에 tolerance를 임의로 넣거나 game tint 값을 바꾸지 않았다. 후속 작업에서 허용 오차와 실제 시각 결과를 검토할 수 있지만, **전체 218개 PASS로 소개하지 않는다.**

### 실제 Unity 직렬화 검사와 미검증 범위

Editor로 `04_PPE`를 열어 inactive 자식을 포함한 **524개 GameObject·1,048개 Component**를 검사했다. `GameObjectUtility.GetMonoBehavioursWithMissingScriptCount`의 합계 **0**, `SerializedObject`의 전체 property에서 null object와 남은 EntityId를 확인한 broken reference **0**, scan exception **0**이다. 검사 후 Scene을 저장하지 않고 닫았다. 설치 전 package GUID를 해석하지 못했던 정적 검사와 달리 이번 수치는 **실제 Editor의 로드된 PPE 씬**에 대한 결과다.

다른 모든 Scene·선택적 null 설정의 설계 적합성·native XR 동작·추적·물리 release·성공 후 다음 프레임 시각·전체 room reset·실제 Scene 전환/음성 및 PCVR/Quest는 이 수치로 보장하지 않는다. PlayMode smoke와 player 빌드도 이번에 실행하지 않았다. 아래 6절의 수동 검수표는 계속 미실행 상태다.

### 재현과 빌드 판정

실행한 집중 검증과 같은 CLI 형식이다. 새 결과 디렉터리를 먼저 만들고 기존 XML을 덮어쓰지 않는다. Test Framework 실행에는 `-quit`를 붙이지 않는다.

```powershell
& '<UNITY_EDITOR_6000.4.8f1>' `
  -batchmode -nographics -projectPath '<VALIDATION_COPY>' `
  -runTests -testPlatform EditMode `
  -testFilter 'PPEPortfolioSequenceTests;PPEGlovePairStateTests;ResponseBagPackingTests' `
  -testResults '<새 결과 폴더>/new-50-results.xml' -logFile '<새 결과 폴더>/new-50.log'
```

전체 회귀는 `-testFilter`를 제외한다. 원시 Unity 로그·XML·JSON은 로컬에 보존했다. 공개용 `public-validation/20261027-unity6000.4.8f1/`에는 개인 경로를 치환한 XML·JSON·중요 진단 `diagnostics.txt`를 제공하며 원시 `.log`는 게시하지 않는다. [공개 증거 manifest](public-validation/20261027-unity6000.4.8f1/PUBLIC_EVIDENCE_MANIFEST.json)에 원본·공개 사본 해시와 치환 범위를 기록했다. Probe 재현 시 원본의 docs 도구를 **검증 복사본** Editor 폴더에 두고 `-executeMethod UnityPpeValidationProbe.Run`을 사용한다. 비동기 discovery callback이 종료하므로 이 호출에도 `-quit`를 붙이지 않는다.

**컴파일·신규 50개 자동 검증 gate는 통과했다.** 전체 회귀에는 기존 정밀도 비교 1개 실패가 남았고 실기기/PlayMode 검수가 남아 있으므로, 검증 완료된 새 포트폴리오 데모·Release 준비 판정은 계속 **보류**한다. 새 빌드·Release·commit·push는 만들지 않았다.

초기 README·03 초안·이전 PPTX의 Unity 미실행 표기는 **실제 검증 전 작성 상태**다. 후속 공개용 README와 [최신 발표 원고](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md)는 실제 결과를 반영했다. 공개 XML/JSON은 개인 PC 경로만 치환한 사본이며 원시 증거와 기존 발표 파일은 로컬에 보존했다. **이하 0~5절은 설치 전 검토·준비 기록이며, 현재 실행 결과로 읽지 않는다.**

## 0. 설치 전 검증 기록: 보존·실행 가능성·빌드 판정

이번 요청은 **검증만** 수행했다. 아래 기존 구현/보조 컴파일 기록은 보존했고, 게임플레이·테스트·Scene·Prefab·공용 XR/App·패키지·프로젝트 버전·README·PPTX를 수정하지 않았다. 이번 회차에는 이 보고서와 작업 요약을 보완하고 별도 검증 기록 폴더(이전 로컬 기록 보존)를 추가했다. 이전 C# 보조 검사와 PPTX 제작 검증은 이번에 다시 실행한 Unity 결과가 아니다.

### 0.1 복구용 백업

- 시작 상태: 지정 브랜치 `codex/ppe-portfolio-improvement`, HEAD `871f806165eebb9e03ba944ed95db5af3dc83819`, tracked 수정 7개와 신규/미추적 20개, 합계 **27개 파일**.
- 백업: `<BACKUP_ROOT>/ppe-validation-20261027T041140344868Z/uncommitted-work.zip`.
- SHA-256: `b2f32833f0d3cc000f1a908481b03695daa7fa7b76490f297bb79fdd567b6a6c`.
- 수정/미추적 파일의 실제 bytes, binary diff와 index diff, HEAD/브랜치/상태 manifest 및 기존 ignored `.utmp/portfolio`의 제작 소스·미리보기·receipt까지 **39개 파일**을 보관했다. ZIP CRC와 각 파일 SHA-256을 읽어 비교해 **39/39 일치**를 확인한 후 문서 수정과 검사를 시작했다.
- `files/<저장소 상대경로>`에 현재 작업 bytes가 들어 있다. 변경 없는 baseline은 기록된 HEAD로 식별한다. 복구가 필요하면 ZIP을 새 임시 폴더에 풀어 manifest/bytes를 먼저 비교한다. 원본에 덮어쓰거나 patch를 적용하는 복구는 별도 승인 전 실행하지 않는다.
- 백업 디렉터리의 manifest/receipt와 보관 증명 사본(이전 로컬 기록 보존)을 함께 유지한다. stash/reset/revert/commit/push는 하지 않았다.

### 0.2 이번에 실제 실행한 검사

| 검사 | 이번 결과 | 범위 |
|---|---|---|
| 기존 산출물 존재·보존 | PASS | 5개 수정 runtime, 3개 fixture/meta, README, 감사/테스트/지원/요약 문서와 PPTX 확인. 최종 보존 비교는 기록(이전 로컬 기록 보존) 참조 |
| 복구 백업 | PASS | 27개 미커밋 파일 포함, 보관한 39개 파일의 ZIP/해시 검증 |
| 정확한 Editor 검색 | **미발견** | `<UNITY_EDITOR_ROOT>`, `<SECONDARY_EDITOR_ROOT>`, `<UNITY_HUB_ROOT>`, `<SECONDARY_EDITOR_ROOT>`, `<SECONDARY_EDITOR_ROOT>`, `<SECONDARY_EDITOR_ROOT>`, Hub 보조 설치 경로, PATH/실행 중 Unity 확인 |
| 실행 파일 버전 | 6000.3.16f1 / 6000.4.1f1 / 6000.4.2f1 | 별도 `<UNITY_HUB_CLI>`는 ProductVersion 1.0.0의 Hub CLI이므로 Editor로 세지 않음 |
| 실제 Unity 사전 검사 | 실행 전 차단 | 기존 runner가 `NOT VERIFIED: required Unity 6000.4.8f1 is unavailable` 반환. 원문(이전 로컬 기록 보존) |
| 저장소 정적 무결성 | **8/8 검사 PASS** | LFS materialized 856개, Runtime SHA-256 244개, 변경 경계·JSON·GUID/meta·작성 테스트 수·문서 링크. 이번 JSON(이전 로컬 기록 보존) |
| `git lfs fsck` / `git diff --check` | PASS / PASS | LFS 객체 검사 OK, 공백 오류 없음. LF→CRLF 정책 안내는 남음 |
| PPE 씬 YAML 사전 검사 | 로컬 참조에서 증명된 누락 없음 | 아래 한계 및 JSON(이전 로컬 기록 보존) 참조 |
| README/MD/PPTX 일관성 | 출처 링크 경고 1종 | 8 slide XML·8 notes 확인, media 0개·표 4개, 조작된 화면/성능/Unity PASS 주장 없음. 검토 기록(이전 로컬 기록 보존) |

씬의 nonzero 로컬 PPtr **2,085회**는 선언된 object ID **943개** 안에서 모두 해석됐다. `m_Script` 222회 중 **117회/46개 GUID**는 기존 로컬 `.cs`로 연결되고 명시적 script `fileID: 0`은 없었다. 나머지 **105회/26개 script GUID**와 UI action을 포함한 전체 외부 참조 **116회**는 `Assets` 맵에서 해석하지 못했다. `Library/PackageCache`도 없어 패키지 참조를 확정할 수 없다. 이는 **105개 Missing Script라는 뜻도, Missing Script가 0이라는 뜻도 아니다.** package·prefab·subasset fileID·필드 타입은 정확한 Unity import로 확인해야 한다.

### 0.3 50사례의 정확한 실행 상태

| Fixture | 소스상 작성 | Unity 발견 | 실행 | 통과 | 실패 | Skip | 환경 때문에 미실행 |
|---|---:|---|---:|---:|---:|---:|---:|
| PPEPortfolioSequenceTests | 13 | 측정 불가 | 0 | 0 | 0 | 0 | 13 |
| PPEGlovePairStateTests | 17 | 측정 불가 | 0 | 0 | 0 | 0 | 17 |
| ResponseBagPackingTests | 20 | 측정 불가 | 0 | 0 | 0 | 0 | 20 |
| 합계 | **50** | **미실행** | **0** | **0** | **0** | **0** | **50** |

`undiscovered`는 **측정하지 못함(null)**이다. discovery를 실행하지 않았으므로 50개를 실패·Skip·undiscovered로 임의 분류하지 않는다. 방법별 기대 사례 목록과 상태는 Unity 상태 JSON(이전 로컬 기록 보존)에 있다. 기존 회귀도 미실행이므로 기존 실패/신규 회귀의 구분을 확정할 수 없다. 이번에 확인한 실행 오류는 **Editor 미설치로 인한 사전 조건 차단**이며 Unity 컴파일 오류 수는 알 수 없다.

### 0.4 정확한 Editor 설치 후 실행 절차

1. [Unity 공식 6000.4.8f1 배포 페이지](https://unity.com/releases/editor/whats-new/6000.4.8f1)에서 해당 버전을 Unity Hub로 설치한다. 공식 Changeset **`f8b72d3d7343`**가 `ProjectVersion.txt`와 일치한다. 최신 버전 설치/프로젝트 업그레이드를 선택하지 않는다.
2. Hub에서 6000.4.8f1 설치 위치를 확인한다. 필요한 Windows 빌드 모듈을 준비한다. 기존 backend가 IL2CPP이면 Windows Build Support (IL2CPP)가 필요하다. Android standalone 검수 때만 Android Build Support·SDK/NDK·OpenJDK를 별도로 준비한다. 설치/모듈 관리는 [Hub 공식 안내](https://docs.unity.com/en-us/hub/install-editors)를 따른다. 이번 회차에는 설치하지 않았다.
3. 위 백업이 읽히는지 확인한 뒤 Hub에 `<PROJECT_ROOT>`를 추가하고 **6000.4.8f1을 명시해** 연다. 초기 package restore/import/컴파일 완료까지 기다린다. 버전 변경/API updater 요구나 예상 밖 ProjectSettings/Packages 변경이 나타나면 적용하지 않고 로그를 보관한다.
4. Console의 컴파일 오류를 확인하고 `%LOCALAPPDATA%/Unity/Editor/Editor.log`를 별도 실행 폴더에 복사한다. `04_PPE`를 열어 inactive 오브젝트·nested prefab까지 Missing Script/Inspector의 Missing Object 경고를 확인한다. null 선택 필드는 선택 사항인지 실제 코드와 구분한다. 자동 저장/씬 재생성/공유 setup 도구 실행을 검증의 전제로 삼지 않는다.
5. 씬 검수 후 저장 없이 빈 테스트 씬으로 전환하거나 Editor를 닫고 아래 runner를 실행한다. Test Runner EditMode 트리에서 신규 **13/17/20 = 50개**를 각각 확인한다. 기존 tint 테스트 1개까지 포함한 집중 runner의 예상 합계는 **51개**다. 총 수만 보지 말고 fixture별 목록과 XML `test-case`의 이름/결과를 대조한다.
6. 아래 집중 실행 후 전체 EditMode 회귀를 별도 결과 폴더로 실행한다. failed/skipped/NotRunnable 및 예상 목록에서 빠진 사례를 기록한다. 기존 테스트 이름·스택·기준 커밋과 비교해 분류하며, 기원을 확인하지 못한 실패는 “분류 보류”로 둔다. source를 되돌려 baseline을 만들지 않는다.

```powershell
# 정확한 Editor 설치 뒤에만 실행할 명령이며, 이번 회차 실행 결과가 아니다.
./docs/portfolio/tools/Run-UnityPpeTests.ps1 `
  -UnityEditor '<UNITY_EDITOR_6000.4.8f1>'
./docs/portfolio/tools/Run-UnityPpeTests.ps1 `
  -UnityEditor '<UNITY_EDITOR_6000.4.8f1>' -FullEditMode
```

runner의 `total > 0` 확인만으로 신규 50개 discovery가 증명되지는 않는다. 결과 XML에서 각 fixture의 실제 test-case 수·full name을 JSON 기대 목록과 대조해야 한다. 0개 발견, 빠진 사례 또는 Skip이 있으면 새 테스트 전부 통과로 소개하지 않는다. 현재 runner는 `.utmp/ppe-tests-<시각>/`를 만들므로 기존 XML을 재사용하지 않는다.

### 0.5 소스 검토로 확인한 검증 공백

| 중점 항목 | 작성 테스트가 다루는 범위 | 실제 Unity/기기로 남은 범위 |
|---|---|---|
| 순서·오순서·중복 | phase 진행, 잘못된 순서, 등록/완료 중복 | Grab 입력부터 UI·음성까지 실제 이벤트 연결 |
| 좌우 장갑·등록 실패 | 양쪽 순서, 두 번째 side rollback, 재시도, 실패 반환 | 실제 SelectExited 순서·성공 후 다음 프레임 숨김/손 시각 |
| 무효 release | 유효 boot target에서 등록 거절 전달 | 거리/회전 초과·pose 없음·잘못된 holding hand의 전용 공간 판정 검증 없음 |
| 가방 수납·최종 Grip | ID 3종, 중복, 현재 대상·입력 손, sticky/legacy 우회 | 실제 물리 입구/직접 Near interactor와 공용 입력의 프레임 순서 |
| 부분/전체 reset | 부분 pair, suit, 가방 prepared/inspected/접촉 초기화 | 완성된 pair 전체 reset, 실제 `item.IsPacked`·책상 home pose 복원 |
| 상태 유지·음성 | manager 제거 후 기록된 부분 Loadout 보존 | 실제 additive Scene 전환·다음 Stage Inventory·TTS/whiteboard. fixture는 PPETTSManager를 구성하지 않음 |

기존 위험인 `PPEReleaseEquip.TryEquipSingle`의 비동기 등록 예약 후 true 반환/manager 결과 무시는 이번에 변경하지 않았다. 위 공백과 TTS busy·추적 fallback 문제는 **관측된 테스트 실패가 아닌 미검증 위험**이다. 동작을 바꿔 테스트를 통과시키는 조치를 하지 않았다.

### 0.6 포트폴리오 일관성·배포 판정

README, 01~04 Markdown 및 PPTX의 8장/8개 notes를 비교했다. 개인 PPE 담당과 공유 App/XR·타 Stage·외부 자산 구분, 9월 10일 빌드/9월 18일 소스 구분, Unity 테스트 미실행 표시를 확인했다. PPTX는 media 0개이며 실제 화면이나 FPS/영상 URL을 만들어 넣은 흔적은 없다. 기존 파일 해시는 `545d1cbeaafa92b7dd755d4c4981824baf4ab305027ee14f5a5780307f1cf793`로 유지했다. 이번에는 PowerPoint 앱 실행·재렌더링을 하지 않았고 이전 시각 검토 기록과 구분한다.

**출처 경고:** PPTX의 특히 6·7장 notes에 있는 GitHub `blob/main` 링크는 미커밋 로컬 수정/신규 테스트의 공개 근거가 아니다. 신규 fixture와 이 보고서는 baseline HEAD에도 없다. 현재 근거는 로컬 source와 백업이다. 공개 권한을 별도로 받은 후 최종 revision에 맞춘 출처 링크 검수가 필요하며 이번에는 deck을 덮어쓰거나 push하지 않았다.

**새 포트폴리오 데모 빌드 준비 판정: 보류(NOT READY).** 정확한 Editor import/compile·직렬화 해석·신규 50개 discovery/run·기존 회귀·PCVR 전체 흐름과 reset 검수가 필요하다. 기존 Release를 최신 소스 시연물로 사용하지 않는다. 새 Release/빌드/commit/push는 생성하지 않았다.

## 1. 환경과 실제 실행 결과

| 검사 | 실제 결과 | 의미 / 한계 |
|---|---|---|
| 저장소 clone + Git LFS | 완료, 856개 LFS 항목 수신 | 소스와 기존 실행 자산을 로컬에 확보 |
| `git lfs fsck` | **PASS**, `Git LFS fsck OK` | 내려받은 LFS 객체 무결성 검사 |
| `git diff --check` | **PASS**, 공백 오류 없음 | README의 LF→CRLF 안내는 Git 줄바꿈 정책 경고이며 컴파일 결과가 아님 |
| Python 저장소 정적 검사 | 결과는 실제 JSON 기록(이전 로컬 기록 보존) 참조 | 변경 소유 범위, LFS pointer, 원본 Runtime SHA-256, JSON, PPE script meta/GUID, 문서 상대 링크 |
| 보조 C# 구문 검사 | **PASS**, PPE C# 133개 × Player/Editor 구성, 구문 오류 0개 | 실행 기록은 C# JSON(이전 로컬 기록 보존) 참조 |
| 보조 C# API 호환성 컴파일 | **PASS**, 현재 PPE runtime 85개와 신규 fixture 3개, 오류 0개·필드 관련 경고 41개 | 9월 10일 Player와 6000.4.2f1 Editor/NUnit 참조 DLL을 사용. Editor 실행·현재 Unity 프로젝트 컴파일이 아님 |
| 정확한 Editor 사전 확인 | **NOT VERIFIED / 실행 차단** | `Run-UnityPpeTests.ps1`이 `6000.4.8f1` 미설치를 감지하고 테스트를 시작하지 않음 |
| Unity 프로젝트 컴파일 | **NOT VERIFIED** | 6000.4.8f1이 확인되지 않음 |
| 새 EditMode 50사례 | **NOT RUN** | 테스트 코드 작성과 정적 검토 완료; 테스트 실행 결과 XML 없음 |
| 기존 EditMode / Loadout smoke / App smoke | **NOT RUN** | 새 checkout의 회귀 결과를 주장하지 않음 |
| PlayMode / Windows 재빌드 | **NOT VERIFIED / NOT BUILT** | 기존 `Runtime/Win64/` 보존 |
| PCVR / Meta Quest / Android APK | **NOT VERIFIED** | 기기 연결·설치·실행·성능 측정을 하지 않음 |

확인된 설치: `<UNITY_EDITOR_ROOT>/`의 `6000.3.16f1`, `6000.4.1f1`, `6000.4.2f1`. 프로젝트 요구는 `6000.4.8f1`이다. 다른 버전으로 프로젝트를 열어 패키지/직렬화를 바꾸지 않았다. 추가 Editor나 패키지를 설치하지 않았다.

## 2. 추가한 Unity EditMode 테스트

기존 `Assets/1.JSR/Editor/PPEInnerGloveHandTintTests.cs`와 같은 predefined Editor assembly/NUnit 패턴을 사용한다. 공용 `asmdef`를 수정하지 않았다. 신규 script별 `.meta`를 추가하고 기존 GUID는 보존했다.

| Fixture | 작성 사례 수 | 검사 내용 |
|---|---:|---|
| [PPEPortfolioSequenceTests](../../Assets/1.JSR/Editor/PPEPortfolioSequenceTests.cs) | 13 | 기본 6단계와 준비 2단계 분리, 전체 완료 이벤트 1회, 잘못된 순서 3경우, 중복 완료, 무효 ID 3경우, 검사만으로 단계 건너뛰기 금지, 장비 등록 1회, 방 리셋, prepared/equipped 분리, Scene manager 부재 후 부분 Loadout 보존 |
| [PPEGlovePairStateTests](../../Assets/1.JSR/Editor/PPEGlovePairStateTests.cs) | 17 | 왼쪽/오른쪽만 완료, 양쪽 순서, 중복, 잘못된 순서, manager 부재·논리 등록 거절의 좌우 롤백과 재시도, 두 완료 API 경로, 부분 reset, 타 pair member, 실제 release API 실패 전파 |
| [ResponseBagPackingTests](../../Assets/1.JSR/Editor/ResponseBagPackingTests.cs) | 20 | 필수 3종 누락 각 경우, 중복 수납 수량, 수납만으로 미완료, 가방 대상 없음, 다른 손 입력, 올바른 손/중복 입력, Trigger 거절, 잘못된 단계, stale sticky selection, standalone/legacy router 우회 차단, 다른 PPE legacy 호환, 실제 XRI select, 대체 Activate의 실제 interactor, 비활성 대상/콜백, reset 접촉 초기화, 기존 무인자 API |

사례 수는 `[Test]`와 `[TestCase]`를 소스로 집계한 **작성 수**다. Unity Test Runner가 발견·실행·통과한 수가 아니다. `[NonParallelizable]`은 보조 참조 NUnit에서 지원하지 않아 사용하지 않았다.

테스트는 빈 씬에서 실행한다. XRI hover/select 테스트는 `XRInteractionManager` API로 실제 이벤트 목록을 만들지만 물리 컨트롤러 입력과 콜백 프레임 순서를 재현하지 않는다. 장갑 fixture는 serialized state와 singleton을 명시적으로 설정하여 상태 전이를 고립시킨다. 실제 트래킹, end-of-frame hide, 물리 잡기/놓기 및 시각 피드백은 기기 확인 대상이다. Loadout manager 부재 테스트는 데이터 보존을 검증하며 실제 additive Scene 전환 테스트를 대체하지 않는다.

## 3. 재현 명령

저장소 루트에서 PowerShell로 실행한다. Unity Editor에서 프로젝트를 열고 있다면 먼저 닫고, 정확한 버전과 필요한 Windows 모듈·라이선스를 준비한다.

```powershell
python ./docs/portfolio/tools/validate_static.py --output ./docs/portfolio/validation/static-validation.json
./docs/portfolio/tools/Validate-PpeCSharp.ps1
git diff --check
git lfs fsck

# 정확한 Editor가 없으면 NOT VERIFIED로 중단한다.
./docs/portfolio/tools/Run-UnityPpeTests.ps1
# 전체 EditMode 회귀 검사
./docs/portfolio/tools/Run-UnityPpeTests.ps1 -FullEditMode
```

Editor가 다른 위치에 설치됐다면 `-UnityEditor '절대경로/6000.4.8f1/Editor/Unity.exe'`를 전달한다. 스크립트는 제품 버전을 확인하고 결과 XML·로그를 `.utmp/ppe-tests-<실행시각>/`에 저장한다. 결과 파일 없음·0건·실패·비정상 종료는 통과로 처리하지 않는다. `.utmp`는 Git에서 제외되어 있다.

같은 작업의 원시 Test Framework 명령:

```powershell
& '<UNITY_EDITOR_6000.4.8f1>' `
  -batchmode -nographics -projectPath '<PROJECT_ROOT>' `
  -runTests -testPlatform EditMode `
  -testFilter 'PPEPortfolioSequenceTests;PPEGlovePairStateTests;ResponseBagPackingTests;PPEInnerGloveHandTintTests' `
  -testResults '<PROJECT_ROOT>/.utmp/editmode-results.xml' `
  -logFile '<PROJECT_ROOT>/.utmp/editmode.log'
```

원시 명령의 `.utmp` 폴더는 먼저 생성한다. 테스트 실행에는 `-quit`를 추가하지 않는다. Test Framework가 종료를 관리한다. 실행 후 XML의 `result`, `total`, `passed`, `failed`와 컴파일 로그를 함께 확인한다.

보조 C# 도구는 설치된 .NET SDK의 Roslyn을 사용한다. 현재 PPE 소스와 과거 `Assembly-CSharp` 타입이 중복되는 `CS0436`만 명시적으로 숨기고 소스 타입을 우선한다. 구문 검사는 `LanguageVersion.Latest`를 사용한다. 따라서 현재 Unity 컴파일러 버전·패키지 resolve·직렬화·Editor 전용 전체 도구의 호환성까지 보장하지 않는다. 과거 빌드 참조를 사용한 이유와 정확한 경로·컴파일 로그는 JSON과 연결된 텍스트 기록에 남겼다.

이전 편집 가능한 8장 PPTX(로컬 원본 보존)는 8개 슬라이드 렌더링 및 전체 시각 검토를 수행했다. Artifact Tool의 구조·레이아웃·폰트·8장 수·재가져오기 검사를 통과했으며 검증 기록(이전 로컬 기록 보존)을 보관했다. 이는 당시 PowerPoint 앱 실행 검증이나 Unity 화면 검증을 뜻하지 않는다. 최신 공개용 자료는 [공개용 PowerPoint](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx)와 [PowerPoint에서 내보낸 PDF](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pdf)를 기준으로 한다.

추가 기존 도구의 실제 public 진입점은 `PPELoadoutSourceGuardSmokeTest.Run()`, `IntegrationSceneValidator.Validate()`, `AppPlayModeSmokeRunner.Run()`이다. 현재 소스에는 해당 도구의 과거 문서상 메뉴를 등록하는 `MenuItem`이 없으므로 메뉴 존재를 가정하지 않는다. 예를 들어 Loadout smoke는 정확한 Editor에 `-batchmode -nographics -projectPath <저장소> -executeMethod PPELoadoutSourceGuardSmokeTest.Run -quit -logFile <로그>`로 호출할 수 있다. 이 smoke는 실패 시 `Debug.LogError`를 쓰므로 로그도 확인한다. Integration validator는 첫 Scene이 App인 전체 교육 구성을 전제로 하며 PPE 전용 프로필과 구분한다. App smoke는 여러 PlayMode/Scene 단계를 비동기로 실행하므로 곧바로 `-quit`하지 말고 원래 runner의 완료 로그를 확인한다. 이 도구들은 이번에 실행하지 않았다.

## 4. 수동 PCVR / Meta Quest 확인표

각 항목을 **PPE 단독**과 **통합 App의 PPE 진입**에서 반복한다. Windows PCVR과 Quest standalone은 별도 결과다. Quest APK를 아직 빌드하지 않았다면 APK 검증은 미실행으로 둔다. 기록할 정보: 실행 바이너리/소스 revision, Editor·패키지 버전, 플랫폼·헤드셋·OpenXR 런타임, 손 입력 모드, 결과·오류 로그·캡처. 모든 항목은 현재 **미실행**이다.

| 항목 | 행동과 기대 결과 | 현재 |
|---|---|---|
| 초기화 | PPE 진입 시 활성 XR Origin·Camera·EventSystem·InteractionManager 중복 여부와 Missing Script/Console 오류 확인 | 미실행 |
| Ray / Near / UI | 현재 PPE profile에 맞는 포인터 활성 확인. Ray를 사용하는 구성은 실제 대상·UI 선택과 손별 포인터 중복을 검사 | 미실행 |
| Grip 직접 잡기 | 가까운 장비만 잡힘. 먼 거리·반대/같은 손 제한을 검사. 놓은 뒤 손이 이전 대상에 고착되지 않음 | 미실행 |
| 착용 순서 | 속장갑부터 외부장갑까지 정상 순서 진행. 순서를 건너뛰면 장비·단계·체크리스트가 진행되지 않음 | 미실행 |
| 좌우 장갑 | 좌/우 먼저 착용 모두 검사. 한쪽만으로 논리 완료 불가. 양쪽 완료 시 1회 등록·진행 | 미실행 |
| 장갑 실패 복구 | 개발용 별도 씬에서 manager 누락/등록 거절을 재현. 첫 손 유지, 두 번째 손 복구, 원인 해소 후 재시도 | 미실행 |
| 트래킹 손실 | 손 또는 컨트롤러 가림·전환·재연결. 0.30초 속장갑 grace와 controller fallback의 오래된 pose로 착용되는지 확인 | 미실행 |
| 배치 안내 | 거리 경계, 장비별 회전 검사, Ready 고스트와 놓기 판정 일치. 신발 고스트가 하체 목표를 따름 | 미실행 |
| 수납 | 측정기·패드·삭 각각 입구 안/밖에서 놓기. 한 항목 누락 시 마지막 완료 금지. 중복 수납은 수량 증가 없음 | 미실행 |
| 잘못된 마지막 Grip | 3종 수납 후 가방을 보거나 hover/select하지 않고 멀리서 Grip. 준비·단계 진행 없음 | 미실행 |
| 입력 손 일치 | 왼손만 가방 hover일 때 오른손 Grip 거절. 같은 손 Grip·직접 XRI select로 1회 완료. stale sticky 선택으로 완료 불가 | 미실행 |
| TTS / UI / SDS | 단계 안내·완료 음성 순서, 오래된 안내 중단, 화이트보드 전환, 손색·이름표·toast·체크리스트·SDS 가독성 | 미실행 |
| 부분 / 전체 reset | 잡은 상태·한쪽 장갑·수납 중·완료 후 reset. 상태·물리 위치·collider·접촉 목록·착용 시각·음성 진행 확인 | 미실행 |
| 장면 완료 / 데이터 | 가방 다음 수거용기 완료 후 기존 이벤트 1회. App이 원래 순서로 전환하고 Loadout/장비 Inventory 유지 | 미실행 |
| 프레임 성능 | 타깃 기기에서 CPU/GPU·GC allocation·프레임 시간 기록. 들고 있는 동안 debug 문자열 비용과 Scene 전환 프레임 확인 | 미실행 |

## 5. 남은 위험과 빌드 조건

1. **정확한 Unity 실행 검증**: 새 코드·테스트가 실제 프로젝트의 타입/패키지 설정으로 컴파일되는지 확인한 뒤 배포한다.
2. **XRI 프레임 순서**: 공용 입력 이벤트, Near/Far hover, selectEntered의 실제 타이밍과 handedness를 검증한다. 현재 대상 목록이 비어 있으면 완료를 거절하는 보수적 동작이다. 장치에서 Grip 재입력이 필요한지 확인한다.
3. **트래킹**: controller fallback은 active Transform 유효성에 의존하는 기존 경로가 있다. 장치 pose가 멈춰도 active로 남는 상황은 감사 문서의 위험 항목이다.
4. **단일 장비 비동기 등록**: 기존 `TryEquipSingle`의 반환 시점과 다음 프레임 등록 성공 시점 차이는 이번 pair 수정에 포함하지 않았다. 실제 manager 부재/비활성·Scene 종료에서 추가 확인한다.
5. **공유 계약**: Equip 거절의 `ContentViewed` 분류, TTS busy 때 보고 누락, 집게를 포함한 4종/3종 사양·전달 경로는 통합 검토가 필요하다.
6. **기존 데모**: 2026-09-10 빌드는 과거 결과이며 현재 수정을 포함하지 않는다. 2026-09-18 소스와 같다고 소개하지 않는다.

정확한 Editor의 컴파일, 신규/기존 EditMode, 필요한 PlayMode/기기 확인이 끝난 뒤 `PPE Portfolio Demo Win64` 프로필로 새 출력을 `Builds/PPEPortfolio/`에 생성한다. 이번 작업에서는 재빌드·기존 바이너리 덮어쓰기·Release 게시를 하지 않았다.

## 6. 실무용 PCVR 수동 검수표

**모든 행은 준비만 했으며 현재 미실행이다.** PCVR은 Windows의 현재 소스를 정확한 Editor PlayMode로 실행하거나 검증된 새 Windows player로 실행하는 경로다. Quest를 Link/Air Link로 PC에 연결해도 이 표의 **PCVR**에 해당한다. 9월 10일 기존 player로 이번 수정의 합격을 판정하지 않는다.

준비: source/branch·실행 모드·PC/OpenXR runtime·HMD/컨트롤러 모델·입력 source·실행 시각을 기록한다. PPE 단독과 통합 App을 각각 검사한다. Console/플레이어 로그 및 상태 Inspector를 관찰하고 Scene/Prefab을 저장하지 않는다. 결함 주입은 백업한 별도 개발 복사본의 PlayMode에서만 수행한다. 기기 착용자와 로그 관찰자를 나누면 재현 조건을 남기기 쉽다.

| ID / 초기 상태 | 사용자 행동 | 기대 결과 | 실패 증상 | 관련 C# 컴포넌트 |
|---|---|---|---|---|
| P01 · 새 PPE 진입, 장비 미착용 | 단독과 App 진입을 각각 시작하고 양손 이동·UI를 확인 | 해당 실행 모드의 XR 카메라/입력 소유가 일관됨. 중복 포인터·Missing Script·Console 오류 없음 | 시야 두 개, 손 위치 분리, UI 무반응, inactive Stage가 입력에 반응 | `PPEActiveXrOrigin`, `PPESequenceManager`; 공용 XR/Adapter는 읽기만 관찰 |
| P02 · 현재 PPE profile, 초기 장비 | 근접 선택·UI를 사용하고 Ray가 켜진 별도 구성에서는 Ray hit도 확인 | Near 활성/Far 비활성 설정을 존중. 가까운 대상/정상 UI 선택. Ray 비활성 자체는 오류 아님 | 손별 포인터 중복, 뒤쪽 물체 선택, 가까운 장비를 잡지 못함 | `PPEDirectGrabProximityFilter`, `PPEHandGrabbable`, `PPESelectionManager` |
| P03 · 요구 단계 속장갑 | 속장갑부터 보호복·장화·호흡보호구·안면보호구·외부장갑까지 순서대로 잡고 목표에서 놓기 | 각 성공만 1회 체크. 6단계 후 준비 phase, 가방 다음 수거용기 | 단계 건너뛰기, 착용했는데 체크 미반영, 수거용기 전 전체 완료 | `PPESequenceManager`, `PPEEquipmentManager`, `PPEPlacementGuideManager` |
| P04 · 속장갑 미완료 | 보호복/외부장갑을 먼저 착용 시도 | 장비 등록·진행 거절, 적절한 안내. 물리 장비 재시도 가능 | 오순서가 완료로 기록, 장비 소실·손 고착 | `PPESequenceManager`, `PPEEquipmentManager`, `PPEReleaseEquip` |
| P05 · 장갑 양손 미착용 | 좌→우와 우→좌 순서로 반대 손으로 잡아 목표에서 놓기. 속장갑·외부장갑 각각 반복 | 한쪽은 partial, 양쪽 성공 후 논리 ID/단계 1회 완료. 속장갑 손색이 해당 손만 변경 | 한 손만으로 전체 완료, 같은 손 착용 허용, 남은 장갑 잡기 실패, 숨김 안 됨 | `PPEInnerGloveDonning`, `PPEGlovePairCoordinator`, `PPEGlovePairMember`, `PPEInnerGloveHandTint` |
| P06 · 장갑 한쪽 완료, 별도 개발 복사본 PlayMode | Debug Inspector에서 logical `PPEEquipable.isEquipped`를 일시 true로 만들어 등록 거절을 유도하고 두 번째 손을 시도. false로 복원 후 재시도 | 첫 손 유지, 두 번째 손만 복구/재잡기 가능. manager에 실패 등록 없음. 원인 해소 후 1회 성공 | 양손 true에 고착, 두 번째 물체 소실/비활성, 재시도 불가 | `PPEGlovePairCoordinator`, `PPEEquipmentManager`, `PPEReleaseEquip` |
| P07 · 한 장비 착용 완료 | 같은 장비 완료 API/입력이 다시 발생하는 상황을 반복하거나 이미 완료한 단계의 물체를 재선택 | 중복 등록/이벤트/수량·단계 전진 없음 | 다음 단계까지 건너뜀, 완료 toast/이벤트 반복 | `PPEEquipmentManager`, `PPESequenceManager`, `PPEGlovePairCoordinator` |
| P08 · 장비를 들고 있음 | 목표 거리 밖에서 놓기, 잘못된 손, tracking 없는 상태로 놓기. 회전 검사가 켜진 장비만 각도 초과도 검사 | 조건 불충족은 등록되지 않음. 장비를 다시 잡을 수 있음. 회전 검사 off 장비의 회전만으로 거절을 요구하지 않음 | 허공 착용, 잘못된 손 성공, 유효하지 않은 Ready와 실제 완료 혼동 | `PPEReleaseEquip`, `PPEPlacementValidation`, `PPEInnerGloveDonning` |
| P09 · 장갑을 잡은 채 목표 손 tracking | 손 가림·컨트롤러 연결 변화·복구를 순서대로 수행하고 grace 경계에서 release | 유효 source/grace 조건 내에서만 착용. 마지막 pose가 무기한 살아 있지 않음 | 추적이 끊긴 오래된 pose에서 계속 성공, 복구 후 target 점프/고착 | `PPEGloveHandTargetFollower`, `PPEInnerGloveDonning`, `PPEPlayerBodyReference` |
| P10 · 수납 0/3, 준비 phase | 측정기·패드·삭을 각각 가방 입구 안/밖에서 놓고 한 종류를 누락해 보기 | 내부 release만 각 ID 1회 수납. 누락 시 AllPacked/준비 완료 거절 | 입구 밖에서도 수납, 한 장비 중복 수량, 물리적으로 넣었는데 수납 불가 | `ResponseBagPackingController`, `ResponseBagPackableItem`, `ResponseEquipmentManager` |
| P11 · 3/3 수납, 가방 미준비 | 가방을 현재 대상으로 삼지 않고 멀리서 Grip. 과거 sticky 선택만 남긴 경우도 반복 | 가방 준비/단계 진행 없음 | 원격 완료, 가방 숨김, 수거용기로 진행 | `ResponseBagPackingController`, `ResponseBagPrepareInteractable`, `XRInteractionConfirmRouter` |
| P12 · 3/3 수납, 왼손만 가방 hover | 오른손 Grip 후 왼손 Grip. 손을 바꿔 반복하고 직접 XRI select도 검사 | 다른 손 입력은 거절, 현재 대상으로 삼은 손의 Grip/select만 1회 완료 | 다른 손으로 완료, 정상 Grip 무반응, 이중 완료. hovered hand/handedness와 이벤트 시각 기록 | `ResponseBagPrepareInteractable`, `XRInteractionConfirmRouter`, `ResponseEquipmentManager` |
| P13 · 장갑 한쪽/수납 일부/장비 잡기 각 상태 | 기존 `PPESequenceManager`의 `Reset PPE Room Training` context action으로 reset | 선택 취소·장비 위치·pair flags·packed flags·목록·체크리스트가 초기 상태. 다시 전 과정 진행 가능 | 손 고착, 이전 item.IsPacked 잔류, 장비가 숨은 채 남음 | `PPESequenceManager`, `PPEEquipmentManager`, `PPEGlovePairCoordinator`, `ResponseBagPackingController` |
| P14 · 전 과정 완료, 단독 PPE에서 씬 이동 전 | 같은 room reset 후 전체 과정을 재완주 | 양쪽 착용 시각·가방/table home pose·교육 순서가 복원되고 완료 이벤트도 새 회차에 1회 | 완료 pair만 복구 안 됨, 가방 AllPacked 잔류, 재완주 불가 | `ResetPpeRoomTraining`, `ResetPair`, `ResetResponseBag`, `PPEStoredDisplayState` |
| P15 · 정상 단계, TTS 재생 중/직후 | 안내·완료·다음 단계 입력을 순차 실행하고 체크리스트·SDS·화이트보드를 확인 | 음성과 실제 단계가 맞고 오래된 cue가 교육 진행을 가리지 않음. UI가 실제 상태를 표시 | 겹치는 음성, 이전 안내 반복, 체크/화이트보드 순서 불일치, 다음 단계 음성 누락 | `PPETTSManager`, `PPEChecklistPanel`, `PPEProcessDiagramController`, `PPEEquipFeedbackUI` |
| P16 · 통합 App에서 가방과 수거용기 완료 | 안내된 완료 입력을 수행하고 다음 Stage의 장비 ID/수량을 관찰 | 기존 완료 이벤트/Adapter 흐름으로 이동. 부분 Loadout을 임의 preset으로 덮지 않고 실제 Inventory 유지 | Scene 이동 중 상태 소실, 장비 수량 증가, 두 Stage 입력 동시 반응 | `PPESequenceManager`, `PPELoadoutData`; 공유 `SessionEquipmentInventory`·Adapter 관찰 |
| P17 · 장비를 든 상태·Scene 전환·reset 반복 | Profiler로 CPU/GPU frame time·GC allocation을 저장하고 반복 전후 비교 | 측정값과 재현 조건을 기록. 정한 타깃 예산과 비교해 판정 | 반복할수록 frame time/GC 증가, 전환 지연·멈춤. 수치 없이 “성능 개선” 판정 금지 | `PPEPlacementGuideManager`, `PPEGloveHandTargetFollower`, `PPETTSManager` |

각 ID에 실행 환경·결과(PASS/FAIL/미실행)·로그 시각·증거 파일을 따로 기록한다. P06의 실패 주입은 개발용 복사본에서 Inspector runtime 값만 바꾸고 저장하지 않는다. manager disable만으로는 `Equip()` 거절이 보장되지 않으므로 그 방법을 실패 재현 근거로 사용하지 않는다.

### Standalone Quest Android 검수는 별도

PCVR 결과를 Android 검증으로 복사하지 않는다. Android는 정확한 Editor·Android 모듈과 기존 프로젝트 설정으로 별도 APK를 만들 수 있는지 먼저 확인해야 한다. 이번에는 Android target 전환·설정 변경·APK 생성·설치를 하지 않았다.

| 초기 상태 | 사용자 행동 | 기대 결과 | 실패 증상 | 관련 컴포넌트 |
|---|---|---|---|---|
| Q01 · 검증할 APK와 source revision/패키지 정보 확보 | PC 연결 없이 Quest에서 APK 실행 | Android OpenXR 초기화, 단일 XR 입력 및 PPE 진입 | 시작 crash, 검은 화면, controller/손 pose 없음 | 공용 OpenXR 구성 관찰, `PPEActiveXrOrigin` |
| Q02 · Quest에서 새 PPE 회차 | P03~P16을 Android 기기에서 다시 수행 | 착용·tracking·수납·TTS·reset·완료가 그 환경에서도 일치 | Android에서만 target/오디오/순서 실패 | 위 PCVR 표의 해당 PPE 컴포넌트 |
| Q03 · standalone 정상 플레이 | pause/resume·tracking 복구·반복 reset과 device Profiler/로그 측정 | 실제 기기 결과·CPU/GPU/GC·오디오 경로를 별도 기록 | focus 복구 실패, 입력 중복, Android 성능 저하 | `PPEGloveHandTargetFollower`, `PPETTSManager`, 단계별 상태 manager |

Q01~Q03도 현재 **미실행**이다. Windows demo 또는 Link/Air Link만으로 Quest Android 지원 성공을 주장하지 않는다.
