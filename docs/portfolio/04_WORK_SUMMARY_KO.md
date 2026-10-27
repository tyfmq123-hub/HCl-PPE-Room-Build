# PPE 포트폴리오 보강 작업 요약

**게시 완료:** [공개 작업 브랜치](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/tree/codex/ppe-portfolio-improvement)에 코드 `f528abf`와 문서 `3427654`를 푸시하고 [PR #1](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/pull/1)을 생성했다. PR은 OPEN이며 자동 병합은 꺼져 있다. main에는 아직 병합하지 않았다. [게시 결과·변경 파일·남은 수동 작업](05_PUBLICATION_REPORT_KO.md)에 최신 상태를 정리했다. 아래 commit·push 미수행 설명은 각 과거 검증 회차의 기록이다.

작업일: 2026-10-27, Asia/Seoul. 로컬 저장소 `<PROJECT_ROOT>`. 작업 브랜치 `codex/ppe-portfolio-improvement`, 시작 커밋 `871f806165eebb9e03ba944ed95db5af3dc83819`.

원격 저장소를 clone하고 LFS를 내려받았다. 시작 시 미커밋 변경이 없음을 확인한 뒤 별도 브랜치를 만들었다. **최초 보강·검증 회차에는 변경을 로컬에 보존**하고 commit·push·PR·Release 게시를 하지 않았다. 후속 요청에서 별도 포트폴리오 브랜치의 커밋·푸시·PR 생성이 승인됐다. 이 문서의 이전 회차 기록과 현재 게시 상태는 구분한다. 팀 저장소의 코드·커밋 이력과 기존 실행 바이너리는 수정하지 않았다.

## 최신 결과: 실제 Unity 설치·컴파일·50개 검증

**Unity 6000.4.8f1 설치 완료 후 실제 import·컴파일과 신규 테스트 50/50 통과를 확인했다.** 정확한 ProductVersion은 `6000.4.8f1_f8b72d3d7343`다. 최신 검증은 [실제 테스트 보고서](02_TEST_REPORT.md#최신-실제-unity-검증--2026-10-27)와 [결과 JSON](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json)을 기준으로 한다. 아래 “후속 검증 회차”는 설치 전 기록이다.

- 실행 전 최신 미커밋 36개와 제작 원본 등 **48개 파일**을 `<BACKUP_ROOT>/ppe-before-unity-install-20261027T043043448168Z/uncommitted-work.zip`에 보존하고 파일 해시 48/48을 확인했다. 백업은 저장소 밖에 있으며 receipt(로컬 원본 보존)를 보관했다.
- 원본 import 변경을 피하려고 현재 소스와 동일한 `<VALIDATION_COPY>`에서 실행했다. Assets 기존 3,888개·Packages 2개·ProjectSettings 27개가 원본과 일치했다. 게임플레이 runtime 5개는 설치 전 백업과 동일하다.
- 실제 import는 종료 code **0**, C# compile error **0**이었다. 기존 player·프로젝트 버전·공용 XR/App·Scene/Prefab·GUID·패키지는 바꾸지 않았다.
- 신규 TestRunner discovery는 **13+17+20=50개 모두 Runnable**. 최종 전체 XML의 신규 50개 full name과 정확히 일치했고 **통과 50, 실패 0, Skip 0, 미발견 0**이다.
- 실제 PPE Scene에서 **524개 GameObject·1,048개 Component**, Missing Script **0**, broken serialized reference **0**, scan 오류 **0**을 확인했다. 다른 Scene·기기 동작까지 검사한 결과는 아니다.

| 실제 실행 | 결과 |
|---|---|
| 최초 신규 집중 실행 | 49/50 통과, 가방 fixture의 attach 초기화 예외 1개 |
| fixture attach 준비 보완 후 재실행 | **50/50 통과**, 프로세스 exit 0 |
| 최초 전체 회귀 | 216/218 통과, 기존 Tint와 신규 fixture null 전제 실패 각 1개 |
| Unity null 전제 보완 후 전체 재실행 | **217/218 통과**, 신규 50/50 포함. 기존 167/168 |
| 변경 없는 기존 Tint 단독 재현 | 같은 실패 1개 재현. 기존 코드/assertion 유지 |

수정은 **테스트 fixture 2개**에 한정했다. `ResponseBagPackingTests`에는 실제 런타임 Awake가 만드는 XRI attach 전제를 명시했고, `PPEPortfolioSequenceTests`의 Loadout 사전 조건은 파괴된 Unity wrapper를 올바르게 판단하되 살아 있는 객체는 계속 거절하도록 Unity null 비교를 사용했다. 사례 수와 게임 동작·성공 조건은 유지했다. 기존 변경을 지우거나 fixture 밖 runtime을 바꾸지 않았다.

남은 기존 실패는 `PPEInnerGloveHandTintTests.ApplyToHand_TintsThenRestoresDefaultHandColor`의 정확한 `Color` 동등 비교다. 기대 `(0.378, 0.495, 0.492, 1)`과 실제 `(0.377999961, 0.494999945, 0.491999924, 1)` 사이 최대 차이 **8.94×10⁻⁸**를 실제 Editor 진단으로 확인했다. 해당 test와 tint runtime은 HEAD와 동일하고 독립 프로세스에서도 재현됐다. 현재 headless 검증에서 재현되는 기존 정밀도 비교 실패이며, 다른 실행 모드에서도 같은지 확인하지 않았다. 임의 tolerance 추가나 색상 변경으로 통과시키지 않았다. **전체 218개 PASS라는 주장은 하지 않는다.**

최초 실패와 재실행 [XML](public-validation/20261027-unity6000.4.8f1/full-editmode-rerun-results.xml), [씬/discovery](public-validation/20261027-unity6000.4.8f1/scene-and-discovery.json), [오류·종료 진단](public-validation/20261027-unity6000.4.8f1/diagnostics.txt), 원본 보존 비교(로컬 원본 보존)를 남겼다. Editor 검사 도구는 원본의 `docs/portfolio/tools/UnityPpeValidationProbe.cs`에 보관하며 실제 import에는 검증 복사본만 사용했다.

**요청한 컴파일·신규 50개 검증은 완료했다.** 전체 회귀의 기존 1개 실패와 PlayMode·PCVR·Quest·실제 reset/음성 검수가 남으므로 검증 완료된 새 포트폴리오 demo/Release 준비 판정은 **보류**한다. 빌드·Release·commit·push는 하지 않았다. 이전 README·03 Markdown·PPTX는 검증 전 작성본으로 보존했다. 후속 공개용 README·발표 자료에는 최신 결과를 반영했다. 현재 수치는 이 최신 절과 02 보고서가 우선한다.


## 공개 준비 및 담당 확인 반영

- **원래 개발:** 3명 팀 프로젝트. 지원자는 PPE 준비실 Stage의 씬·보호구 인터랙션·교육 흐름·장비 준비·전용 UI와 안내·관련 Stage 기능을 전담 개발했다. 타 Stage와 공용 App/XR·외부 자산은 구분한다.
- **후속 보강:** 2026-10-27 결함 수정, 회귀 테스트와 실제 Unity 검증, 포트폴리오 정리 작업은 원래 개발 기간 이후 작업이다.
- README를 실제 준비실·Test Runner 캡처, 두 문제 해결 사례, 정확한 검증 수치와 세 날짜로 정리했다. [최신 공개용 발표 원고](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md)와 [자료 목차](PORTFOLIO_INDEX.md)를 추가했다.
- 2026-09-10 실제 게임플레이 녹화는 지원자가 보유한다. 작업 폴더에서는 파일을 찾지 못해 공개 URL 등록 자리를 문서화했고 임의 URL을 만들지 않았다. 해당 영상은 당시 Windows 빌드를 보여 주며 10월 보완 반영 근거가 아니다.
- 개인 PC 경로는 공개 문서와 공개 검증 사본에서 치환했다. 원시 증거·백업·이전 발표 파일은 로컬에 보존한다. 테스트 수치·이름·실행 시각과 실패 결과는 바꾸지 않았다.
- 사용자 요청으로 팀 저장소를 공개 전환하고 비로그인 접근을 확인했다. 팀 저장소의 코드·커밋 이력은 변경하지 않았다. 포트폴리오 PR은 자동 병합하지 않으며 새 player/Release를 만들지 않는다.

아래 “후속 검증 회차”와 번호별 절은 초기 보강·설치 전 검사 이력을 보존한 기록이다. 현재 실행 결과는 위 최신 실제 검증과 02 보고서를 우선한다. GitHub 게시 여부는 실제 브랜치·PR 링크를 기준으로 확인한다.
## 후속 검증 회차 결과

이번 후속 요청에서는 **기존 구현을 보존하고 검증·기록만** 진행했다. 아래 1~7절은 앞선 보강 작업의 기록이다. 이번 시작 상태는 이미 미커밋 27개 파일(tracked 수정 7개·미추적 20개)이 있는 상태였다. 기존 README·PPTX·C# 코드·fixture/meta·공유 영역·프로젝트 버전은 바꾸지 않았다. 갱신한 파일은 `02_TEST_REPORT.md`, 이 요약, 별도 `validation/20261027-validation/`의 검증 증거다.

| 항목 | 후속 검증 결과 |
|---|---|
| 복구 백업 | `<BACKUP_ROOT>/ppe-validation-20261027T041140344868Z/uncommitted-work.zip`. 기존 미커밋 27개와 ignored 포트폴리오 제작 원본 등을 포함한 39개 파일, ZIP CRC 및 SHA-256 **39/39 일치** |
| 백업 식별 | SHA-256 `b2f32833f0d3cc000f1a908481b03695daa7fa7b76490f297bb79fdd567b6a6c`. 원본 덮어쓰기/복구 실행 없음. receipt(이전 로컬 기록 보존) |
| Editor 검색 | 확인한 기본/보조 경로에 **6000.4.8f1 미발견**. 설치된 Editor는 6000.3.16f1/6000.4.1f1/6000.4.2f1. Hub CLI 1.0.0을 Editor로 오인하지 않음 |
| 실제 Unity 실행 | runner가 정확한 버전 부재로 시작 전 차단. import·compile·직렬화 해석·PlayMode·PCVR·Android 실행 없음 |
| 신규 테스트 50사례 | 소스 작성 13+17+20. **실행 0, 통과 0, 실패 0, Skip 0, 환경 미실행 50**. Unity 발견/undiscovered 수는 실행 전이므로 **측정 불가**이며 0이나 50으로 만들어 쓰지 않음 |
| 기존 테스트와 신규 회귀 구분 | 기존 회귀도 미실행. 기존 실패/신규 회귀로 분류할 실제 실행 근거 없음 |
| 실제 실행한 정적 검사 | 변경 경계·JSON·LFS pointer·Runtime 244개 hash·meta/GUID·작성 사례·문서 링크 **8/8 PASS**, `git lfs fsck` OK, `git diff --check` PASS |
| PPE 씬 참조 사전 검사 | 로컬 PPtr 2,085회에 dangling ID 없음. local `.cs` 연결 117회/46 GUID. 미해석 script105회/26 GUID와 전체 외부116회는 package cache/import 부재로 **미확정**, Missing Script=0 주장은 불가 |
| 이전 C# 보조 검사 | 이전 회차의 133개 구문 검사/88개 보조 컴파일 기록을 보존. 이번에 다른 Editor/구버전 참조를 대체 실행하지 않음 |
| 포트폴리오 검수 | README·01~04 MD·PPTX 8 slide/8 notes 확인. 기여/버전/미실행 경계 일관, 허구의 영상·화면·FPS 없음. PPTX ZIP 정상·media0개·표4개, 파일 hash 유지 |
| 남은 문서 경고 | PPTX 특히 6·7장 notes의 GitHub `main` 링크는 미커밋 로컬 코드·신규 테스트 근거가 아님. 공개 후 별도 revision 출처 검수 필요. deck은 덮어쓰지 않음 |
| 빌드 준비 판정 | **NOT READY / 보류**. 정확한 Unity import·컴파일·50개 discovery/run·기존 회귀·PCVR 전체 흐름/reset 확인 전 새 포트폴리오 데모의 검증 완료를 선언할 수 없음 |

이번 회차 원시 상태(이전 로컬 기록 보존), 정적 검사(이전 로컬 기록 보존), 씬 preflight(이전 로컬 기록 보존), 포트폴리오 일관성(이전 로컬 기록 보존), 보존 비교(이전 로컬 기록 보존)를 남겼다. 백업 ZIP은 저장소 밖에 있으며 커밋/푸시/Release는 하지 않았다.

새 확정 컴파일/기기 오류는 관측하지 못했다. 이는 오류가 없다는 뜻이 아니라 **실행할 정확한 Editor가 없어 관측하지 못했다는 뜻**이다. 코드 검토상 검증 공백은 유효하지 않은 공간 release, 성공 release의 실제 SelectExited/다음 프레임 숨김, 완료된 pair 전체 reset, 가방 packed flag/home pose, 실제 Scene Inventory 전달 및 TTS 연결이다. 기존 `TryEquipSingle`의 비동기 등록 반환 위험과 수납 4종/3종 사양·거절 telemetry 문제를 보존했다. 테스트 통과를 위해 동작을 변경하지 않았다.

다음 조치는 [공식 6000.4.8f1 페이지](https://unity.com/releases/editor/whats-new/6000.4.8f1)의 Changeset `f8b72d3d7343` 버전을 설치한 뒤, [테스트 보고서 0.4절](02_TEST_REPORT.md#04-정확한-editor-설치-후-실행-절차)의 정확한 버전 runner로 진행하는 것이다. 집중 runner는 기존 tint 1개를 포함해 예상 51개이므로 신규 fixture **13/17/20개**를 따로 대조한다. 결과의 총 수만으로 50개 전부 실행됐다고 판단하지 않는다.

실기기 검수는 [테스트 보고서 6절](02_TEST_REPORT.md#6-실무용-pcvr-수동-검수표)에 **초기 상태·행동·기대 결과·실패 증상·관련 C#**를 갖춘 PCVR 17항목과 standalone Quest Android 3항목으로 준비했다. Quest Link/Air Link는 PCVR이며 Android APK 검수와 구분한다. 새 기능 개발·기존 변경 폐기·소스/바이너리 덮어쓰기·프로젝트 업그레이드는 수행하지 않았다.

## 1. 감사 요약과 실제 결함

소스에서 PPE 6단계 착용과 대응가방·수거용기 2단계 준비, 좌우 장비, 배치/놓기 검증, 수납, TTS/UI 및 공용 App 연결 경로를 확인했다. 구현 확인과 실제 장치 실행 성공을 분리했다.

| 확인한 결함 | 증거와 구현 조치 |
|---|---|
| 가방 대상 없이 마지막 Grip 완료 | 기존 `ResponseBagPackingController.OnSharedInputPerformed`는 `AllPacked` 후 global Select만 검사하고 manager 완료 함수를 호출했다. 현재 활성 가방의 hover/select와 입력 손을 검사하도록 prepare/router 경로를 보강했다. |
| 논리 등록 실패 뒤 좌우 완료 상태 고착 | Coordinator의 두 완료 API가 좌우 flag를 설정한 뒤 manager `Equip()` 실패를 복구하지 않았다. 거절된 두 번째 side만 flag·물리 pose·grab·착용 시각을 되돌리고 재시도를 허용한다. |
| 놓기 호출자가 pair 실패를 성공으로 처리 | `PPEReleaseEquip`이 coordinator 결과를 무시하고 `NotifyEquipped`를 호출했다. 실패 반환을 전달하여 잘못된 보관 상태 전이를 막는다. |

오입력의 `ContentViewed` 분류, controller tracking fallback, 단일 장비 비동기 등록 반환은 [기술 감사](01_TECHNICAL_AUDIT.md)에 근거와 후속 조치를 기록했다. 보호된 공용 계약을 임의로 바꾸지 않았다. 기존 작업지시서의 4종과 후속 코드·대본의 3종 수납 차이는 공식 교육 사양 확인 항목이다.

## 2. 변경 파일

런타임 수정은 PPE 소유 파일 5개다. Scene·Prefab·기존 meta·App/Common XR·다른 Stage·Packages/ProjectSettings·Runtime·원본 export manifest는 보존했다.

| 파일 / 경로 | 변경 |
|---|---|
| `Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs` | 실패 시 두 번째 side 복구·foreign member 거절 |
| `Assets/1.JSR/Scripts/PPEReleaseEquip.cs` | pair 실패 반환 전파 |
| `Assets/1.JSR/Scripts/ResponseBagPackingController.cs` | 가방 대상/입력 손 검사 경로 사용, reset 접촉 목록 초기화 |
| `Assets/1.JSR/Scripts/ResponseBagPrepareInteractable.cs` | 최종 입력의 공통 검사와 기존 API 호환 |
| `Assets/1.JSR/Scripts/XRInteractionConfirmRouter.cs` | 입력 손 전달, 오래된 선택을 통한 완료 우회 차단 |
| `Assets/1.JSR/Editor/PPEPortfolioSequenceTests.cs` + `.meta` | 순서·중복·장비 등록·reset·부분 Loadout 회귀 테스트 |
| `Assets/1.JSR/Editor/PPEGlovePairStateTests.cs` + `.meta` | 좌우 장갑 및 실패 복구 회귀 테스트 |
| `Assets/1.JSR/Editor/ResponseBagPackingTests.cs` + `.meta` | 수납·실제 XRI 이벤트 목록·잘못된 최종 입력 회귀 테스트 |
| `README.md` | 한국어 소개, 구조·역할·실행·문제 해결·버전/검증 표시 |
| `docs/handoffs/ppe/LATEST.md` | 이번 변경·검증·통합 요청을 상단에 추가, 과거 기록 유지 |
| `docs/portfolio/01_TECHNICAL_AUDIT.md` | 구현/미검증 구분, 전체 감사·우선순위 |
| `docs/portfolio/02_TEST_REPORT.md` | 실제 검사, 미실행 테스트, 재현 명령, 기기 체크리스트 |
| `docs/portfolio/03_JOB_APPLICATION_PORTFOLIO_KO.md` | Letuin Edu 직무를 위한 8장 초안, 근거·추천 화면·면접 메모 |
| `docs/portfolio/03_JOB_APPLICATION_PORTFOLIO_KO.pptx` | 편집 가능한 8장 발표 자료 |
| `docs/portfolio/04_WORK_SUMMARY_KO.md` | 이 작업 요약 |
| `docs/portfolio/tools/Run-UnityPpeTests.ps1` | 정확한 Editor 확인·집중/전체 EditMode 실행·결과 확인 |
| `docs/portfolio/tools/Validate-PpeCSharp.ps1` | Roslyn 구문/API 보조 검사 재현 |
| `docs/portfolio/tools/validate_static.py` | 변경 경계·LFS·Runtime hash·GUID·문서 링크 검사 |
| `docs/portfolio/validation/` | `static-validation.json`, `csharp-static.json`, C# 진단 텍스트 3개, `presentation-validation.json` |

현재 추적/신규 파일 목록과 검사 시점은 정적 검사 JSON(이전 로컬 기록 보존)에도 기록한다. 컴파일 중간물·PPTX 제작용 중간 파일은 Git ignore 대상 `Temp/`, `.utmp/`에 둔다.

## 3. 실행한 검사와 실행하지 못한 검사

- Git LFS 객체 검사: `git lfs fsck` **OK**, 856개 LFS 항목 materialized 확인.
- 과거 export manifest에 기록된 Runtime 파일 **244개 SHA-256 일치**. 새 빌드로 교체하지 않았음을 별도 확인했다.
- `git diff --check`, 변경 소유 범위·JSON·PPE `.meta`/GUID·문서 상대 링크 정적 검사 통과.
- PPE C# **133개**를 Player/Editor 기호 구성으로 구문 분석: 오류 **0개**.
- PPE runtime **85개 + 신규 fixture 3개**를 구버전 Player와 Editor/NUnit DLL에 대해 보조 컴파일: 오류 **0개**, 필드 관련 경고 **41개**. `CS0436` 중복 소스 경고는 의도적으로 제외했다. **현재 Unity 프로젝트 컴파일 성공이 아니다.**
- PPTX: 정확히 8장, 전체 렌더링/시각 검토, 구조·레이아웃·폰트·재가져오기 검사 통과. PowerPoint 앱 실행 미검증.

정확한 Unity **6000.4.8f1**이 확인되지 않았다. 설치된 6000.3.16f1/6000.4.1f1/6000.4.2f1로 프로젝트를 열지 않았다. **새 Unity 테스트·기존 회귀·PlayMode·Quest/PCVR·프레임 성능은 미실행/미검증**이다. 테스트 결과 XML과 새 Windows/APK 빌드는 생성하지 않았다. 작성된 테스트의 범위와 사례 수는 [테스트 보고서](02_TEST_REPORT.md)를 따른다.

## 4. README와 지원 자료

README 첫 화면에 프로젝트 목적, 핵심 흐름, 기술 스택, 담당 범위와 실제 검증 상태를 배치했다. 공용 XR/App, 타 Stage, Unity Samples·외부 자산을 구분했다. 이후 지원자 확인에 따라 원래 프로젝트가 3명 팀이며 PPE 준비실 Stage 전체 개발을 전담했다는 책임을 최신 README·발표 자료에 명시했다. LFS, 정확한 Editor, Build Profile 및 `HCL_PPE_DEMO` 사용법을 유지했다. 영상 URL이나 화면·성능 수치를 만들지 않고 필요한 실제 캡처 목록을 제공했다.

포트폴리오는 프로젝트 개요·구조·기술/역할·XR 상호작용·상태 관리·실제 두 결함·검증·직무 연관의 8장이다. 각 장에 본문, 추천 캡처/도식, 실제 근거 파일과 면접 메모를 포함한다. 현재 채용공고를 별도 확인한 자료가 아니며 사용자 제공 직무 맥락에 맞춘 초안이다.

2026-09-10 데모와 2026-09-18 소스 export의 차이를 모든 주요 자료에 유지했다. 과거 테스트 PASS를 현재 실행 결과로 재사용하지 않았다.

## 5. 수동 검증 순서와 남은 위험

1. Unity 6000.4.8f1에서 컴파일·Missing Script를 확인하고 새 fixture와 기존 회귀를 실행한다. 결과 XML·Console 로그를 저장한다.
2. PPE 단독에서 정상 착용, 순서 오류, 좌/우 먼저 장갑, 실패 후 재잡기, 추적 손실과 배치 임계값을 확인한다.
3. 3종 수납 전/후, 가방과 무관한 Grip, 다른 손 Grip, stale 선택, 직접 XRI select, 부분/완료 reset을 검사한다.
4. 통합 App에서 단일 XR 기반·입력 이벤트 타이밍·TTS/UI·수거용기 이후 완료 이벤트/Scene 이동·Inventory 유지를 확인한다.
5. Windows PCVR과 Quest 대상 환경을 각각 검수하고 Profiler로 프레임/GC를 측정한다. 검증된 소스로 새 빌드를 별도 경로에 만든다.

세부 합격 조건은 테스트 보고서의 15항목 표에 있다. 입력과 XRI hover/select 갱신 순서, controller pose 유효성, 단일 장비의 비동기 등록, 미검증 물리 복구가 실행 단계의 주요 위험이다. 수납 사양·telemetry 의미는 담당자 확인이 필요하다. PPE Stage 전담 책임은 지원자 확인으로 확정했다.

## 6. 채용 적합성 기준의 다음 개선

| 순서 | 개선 | 보여줄 근거 |
|---|---|---|
| 1 | 정확한 Editor·HMD에서 전체 과정 검증 | 테스트 XML, 기기/플랫폼 정보, 완료/부분 reset 영상 |
| 2 | 실제 30~60초 데모와 대표 화면 확보 | 착용·순서 거절·양손 상태·수납/완료 장면 |
| 3 | PPE Stage 전담 개발과 팀 협업 사례 설명 | 3명 팀과 공용 기반을 구분하고 공유 계약을 지킨 변경 설명 |
| 4 | 집게 수납/SOP와 거절 telemetry의 통합 결정 | 승인된 사양, 이벤트 의미와 회귀 검사 |
| 5 | 측정 후 tracking/가이드 비용과 단일 장비 실패 경로 보강 | Profiler capture와 재현 조건, 작은 수정/회귀 테스트 |

## 7. 면접에서 설명할 C# 핵심

“장비를 잡고 놓는 XRI 입력과 교육 상태를 분리했습니다. `PPEEquipable`은 장비 ID와 착용 기준을, 배치 코드는 실제 목표 pose와 거리·회전을 검사합니다. `PPEEquipmentManager`가 장비 등록과 중복을 관리하고 `PPESequenceManager`가 현재 순서에 맞을 때만 다음 단계로 전진합니다. 좌우 장갑은 물리 부품 두 개이지만 논리 장비 ID는 하나라서 coordinator가 양쪽을 모읍니다. 마지막 등록이 실패하면 두 번째 시도만 복구해 재시도할 수 있게 했습니다. UI/TTS는 상태 이벤트를 읽고, 전체 완료 뒤 Scene 전환은 공용 App이 처리합니다.”

이 설명은 지원자가 코드를 읽고 실제 검증한 범위에서 자신의 말로 정리한다. 이번 포트폴리오 보완과 과거 본인 기여를 구분해 설명한다.
