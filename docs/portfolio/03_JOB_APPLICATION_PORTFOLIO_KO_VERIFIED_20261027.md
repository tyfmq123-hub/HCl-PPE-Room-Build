# Unity XR 개발 포트폴리오 — HCl 누출 대응 PPE 준비실

검증·담당 확인 반영본: **2026-10-27**. 사용자 제공 8장 원고를 기준으로 구성했다. 기존 초안과 PPTX는 보존한다. 발표 자료의 검증 수치는 [실제 Unity 검증 요약](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json)과 [테스트 보고서](02_TEST_REPORT.md)를 근거로 한다.

[편집 가능한 PowerPoint 8장](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx) · [PDF](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pdf)

**담당 확인 반영:** 원래 프로젝트는 **3명 팀 개발**이며 **지원자는 PPE 준비실 Stage 전체 개발을 전담**했다. 씬 구성, 보호구 상호작용·착용 판정, 교육 흐름, 장비 준비, 전용 UI·안내와 관련 Stage 기능이 포함된다. 다른 Stage와 공용 App/XR·외부 자산은 구분한다. 2026-10-27 포트폴리오 보강과 회귀 검증은 원래 개발 기간 이후 작업이다. 이름·참여 기간과 발표 문구는 제출 전 본인이 최종 기입한다.

**검증 범위:** 정확한 Unity Editor의 import·컴파일, EditMode 테스트 및 로드한 PPE 씬의 직렬화 검사다. PCVR·Quest HMD 동작, PlayMode smoke, 최신 Windows/Android player 빌드는 별도 검증 대상이다. 캡처는 실제 Unity에서 확보한 화면만 사용하며, 확보되지 않은 장면과 데모 영상은 촬영 예정으로 표시한다.

## Slide 1. 산업 안전교육을 위한 VR PPE 준비실 개발

**HCl Chemical Leak Response VR Training**

**슬라이드 본문**

- 화학물질 누출 사고 대응 교육을 위한 Unity 기반 VR 콘텐츠
- 현장 진입 전 보호구 착용 및 대응 장비 준비 과정 구현
- 실제 XR 상호작용과 교육 절차 검증 기능을 연결
- 팀 프로젝트 중 PPE 준비실 Stage 중심의 개발 경험 소개

**실제 시각 자료:** Unity 6000.4.8f1에서 연 `04_PPE` 준비실의 Scene View다. EditMode의 씬 구성 화면이며 HMD 실행 화면과 구분한다.

![Unity 6000.4.8f1 EditMode Scene View의 PPE 준비실 전경](media/20261027-verified/ppe-editor-overview.png)

**발표자 메모:** “현장에 진입하기 전에 보호구와 대응 장비를 준비하는 과정을 다루는 공간입니다. 팀 프로젝트 전체 가운데 PPE 준비실의 상호작용과 교육 상태 관리를 중심으로 설명하겠습니다.” 교육 효과나 법정 교육 인증을 검증한 성과로 확대하지 않는다.

**근거:** [04_PPE.unity](../../Assets/Scenes/04_PPE.unity), [PPE_TASK.md](../tasks/PPE_TASK.md), [Stage 작업 경계](../integration/STAGE_EDITING_BOUNDARIES.md).

## Slide 2. 교육 흐름과 Unity 프로젝트 아키텍처

**슬라이드 본문**

- 전체 교육은 PPE 준비, 현장 진입 전 확인, 누출 대응 단계로 구성
- 공용 App은 XR 장비 입력과 씬 전환을 관리
- PPE Stage는 오브젝트 상호작용, 장비 상태, 교육 순서 및 안내를 담당
- PPE 완료 이벤트와 장비 상태 데이터를 공용 시스템에 전달

**시각 자료:** 편집 가능한 `App / Shared XR → PPE Stage → 상태·완료 이벤트 → 다음 Stage` 구조도. 별도 데이터 경로에 `SessionEquipmentInventory: 장비 ID·수량`을 표시한다.

**발표자 메모:** “공용 App이 XR 기반과 씬 흐름을 관리하고, PPE Stage가 교육 동작을 담당합니다. 완료 사실은 기존 이벤트 계약으로 전달합니다. Scene 사이에는 장비 오브젝트 자체를 이동하기보다 장비 ID와 수량 데이터를 전달합니다.” 전체 교육의 실기기 완주를 검증했다는 의미는 아니다.

**근거:** [Shared XR 구조](../integration/SHARED_XR_ARCHITECTURE.md), [Session Inventory 계약](../integration/SESSION_INVENTORY_CONTRACT.md), [PPESequenceManager](../../Assets/1.JSR/Scripts/PPESequenceManager.cs), [StageLifecycleAdapter](../../Assets/Scripts/App/StageLifecycleAdapter.cs), [SessionEquipmentInventory](../../Assets/Common/XR/SessionEquipmentInventory.cs).

## Slide 3. 개발 환경과 담당 영역

**슬라이드 본문**

- **Engine:** Unity 6000.4.8f1 / C#
- **XR:** XR Interaction Toolkit 3.4.1, XR Hands 1.7.3, OpenXR
- **개발 영역:** PPE 씬 구성, 보호구 인터랙션, 착용 판정, 교육 안내 및 장비 준비
- **협업:** Git 기반 Stage별 작업 영역 분리 및 공용 인터페이스 준수
- 3명 팀 중 PPE 준비실 Stage 전체 전담 개발, 공용 App/XR·타 Stage는 팀 영역

**실제 시각 자료:** `PPE_System`을 펼친 Hierarchy와 Equipment/Sequence Manager Inspector를 포함한 Unity Editor 화면이다. PPE Stage 전담 책임과 팀 공용 기반 사용을 구분해 설명한다.

![PPE System Hierarchy와 Equipment 및 Sequence Manager Inspector](media/20261027-verified/ppe-editor-hierarchy.png)

**발표자 메모:** “PPE Stage의 기능과 공용 XR/App을 구분해서 작업했습니다. 장비의 물리 상호작용, 착용 기준과 교육 상태 연결을 설명하되, 공용 시스템과 다른 Stage 및 Unity Samples·외부 자산은 팀/외부 영역으로 표시하겠습니다.” PPE Stage 전체 기능 개발 책임을 명확히 하되 Unity Samples·공용 App/XR·다른 Stage를 개인 제작으로 소개하지 않는다.

**근거:** [ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt), [manifest.json](../../Packages/manifest.json), [packages-lock.json](../../Packages/packages-lock.json), [PPE_WORKER.md](../codex/PPE_WORKER.md), [수정 경계](../integration/STAGE_EDITING_BOUNDARIES.md).

## Slide 4. XR Grab과 착용 판정 구현

**슬라이드 본문**

- 손과 물리 Collider의 근접 여부에 따라 Grab 허용
- 보호구별로 손, 얼굴, 몸, 하체에 착용 목표 위치 설정
- 착용 시점에 대상의 위치, 추적 유효성, 거리 및 교육 순서 검사
- Ghost 안내와 실제 착용 성공 판정을 구분해 잘못된 착용 방지

**핵심 코드:** `PPEDirectGrabProximityFilter`, `PPEReleaseEquip`, `PPEInnerGloveDonning`

**시각 자료:** `보호구 잡기 → 목표 위치로 이동 → 착용 판정 → 결과 피드백` 구조도와 실제 확보한 장비/target 화면. HMD에서 잡고 놓는 장면을 촬영하지 못한 경우 이를 실제 시연 이미지로 대체하지 않고 “실기기 착용 장면 촬영 예정”으로 표시한다.

**발표자 메모:** “잡을 수 있는 거리와 착용할 수 있는 거리는 별도 조건입니다. 보호구를 목표에 가져간 뒤 놓는 순간 live target과 순서를 검사합니다. Ghost의 Ready는 안내 상태이고 실제 장비 등록 성공과 구분합니다.” 현재 PPE profile은 Near 활성/Far 비활성이므로 Ray가 기본 활성인 것처럼 설명하지 않는다. 회전 검사는 장비별 설정이다.

**근거:** [근접 Grab 필터](../../Assets/1.JSR/Scripts/PPEDirectGrabProximityFilter.cs), [PPEReleaseEquip](../../Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [PPEInnerGloveDonning](../../Assets/1.JSR/Scripts/PPEInnerGloveDonning.cs), [PPEPlacementValidation](../../Assets/1.JSR/Scripts/PPEPlacementValidation.cs), [PPE profile](../../Assets/1.JSR/Resources/SharedXR/PPE.asset).

## Slide 5. PPE 착용 순서 및 상태 관리

**슬라이드 본문**

- `PPESequenceManager`: 교육 단계와 필수 착용 순서 관리
- `PPEEquipmentManager`: 장비 ID 및 착용 완료 상태 관리
- `PPEGlovePairCoordinator`: 양손 착용을 개별 추적하고 논리적 완료 판정
- 잘못된 순서, 중복 착용, 부분 착용 등을 구분
- 완료 및 상태 변경 이벤트를 UI·TTS에 연동

**시각 자료:** 왼손·오른손 개별 상태와 최종 장갑 완료 조건의 편집 가능한 상태도. `왼쪽만 완료 / 오른쪽만 완료 / 양쪽 완료 후 논리 Equip 성공`을 구분한다.

**발표자 메모:** “장갑의 물리 부품은 좌우 두 개지만 교육 단계에서 관리하는 장비 ID는 하나입니다. Coordinator가 양쪽 상태를 모으고 최종 장비 등록을 요청합니다. SequenceManager는 착용 6단계와 가방·수거용기 준비 2단계를 구분해 순서를 진행합니다.” 이벤트 연결의 소스 구현과 실제 HMD의 음성·시각 동작 검수는 구분한다.

**근거:** [PPESequenceManager](../../Assets/1.JSR/Scripts/PPESequenceManager.cs), [PPEEquipmentManager](../../Assets/1.JSR/Scripts/PPEEquipmentManager.cs), [PPEGlovePairCoordinator](../../Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs), [PPEChecklistPanel](../../Assets/1.JSR/Scripts/PPEChecklistPanel.cs), [PPETTSManager](../../Assets/1.JSR/Scripts/PPETTSManager.cs).

## Slide 6. XR 인터랙션 결함 분석 및 개선

**문제 A — 대응 가방과 무관한 Grip 입력**

- **원인:** 수납 완료 후 Grip 처리에 가방 대상 문맥 확인이 부족
- **개선:** 활성 가방의 Hover/Select와 입력 손 검증
- **결과:** 의도하지 않은 준비 완료 경로를 차단하도록 수정

**문제 B — 장갑 등록 실패 시 상태 불일치**

- **원인:** 양손 상태 갱신과 최종 장비 등록 성공 여부가 일치하지 않을 수 있음
- **개선:** 등록 실패 시 해당 손의 상태와 상호작용 정보를 복구
- **결과:** 먼저 성공한 손의 상태를 유지하면서 실패한 시도만 복원

**검증:** 관련 신규 회귀 테스트를 포함한 **50개 테스트 통과**. 실제 HMD 동작은 별도 검증 필요.

**시각 자료:** 문제 발생 조건, 코드 수정, 기대 상태 복구의 편집 가능한 흐름도. 가방은 무관한 Grip의 거절, 장갑은 두 번째 side 등록 실패 후 rollback을 보여준다.

**발표자 메모:** “입력이 발생했다는 사실만으로 가방 준비를 완료할 수 없어서 현재 대상과 입력한 손을 함께 확인했습니다. 장갑은 두 번째 시도에서 논리 등록이 실패하면 그 손만 복구합니다. 먼저 성공한 손을 유지해 재시도할 수 있도록 했습니다.” 최초 테스트에서 드러난 XRI attach 초기화와 Unity null 전제도 fixture에서 보완했으며, 검증을 위해 게임 동작이나 성공 조건을 바꾸지 않았다.

**근거:** [ResponseBagPackingController](../../Assets/1.JSR/Scripts/ResponseBagPackingController.cs), [ResponseBagPrepareInteractable](../../Assets/1.JSR/Scripts/ResponseBagPrepareInteractable.cs), [XRInteractionConfirmRouter](../../Assets/1.JSR/Scripts/XRInteractionConfirmRouter.cs), [PPEGlovePairCoordinator](../../Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs), [PPEReleaseEquip](../../Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [실제 검증 요약](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json).

## Slide 7. Unity 실제 컴파일 및 자동 테스트 검증

**슬라이드 본문**

| 검증 항목 | 실제 결과 |
|---|---|
| Unity 버전 | **6000.4.8f1**, revision `f8b72d3d7343` |
| C# 컴파일 오류 | **0개** |
| 신규 회귀 테스트 | **50/50 PASS**, Skip·미발견 0개 |
| 전체 회귀 테스트 | **217/218 PASS**, 기존 test 1개 FAIL |
| 로드한 PPE 씬 | **Missing Script 0개, 끊어진 직렬화 참조 0개** |
| 남은 이슈 | 기존 손 색상 test의 미세한 부동소수점 정밀도 차이 |
| 별도 검증 필요 | PCVR·Quest 실기기 사용성, PlayMode smoke, 최신 player 빌드 실행 |

**실제 시각 자료:** 아래는 Unity GUI Test Runner에서 **신규 50개만 선택해 재실행한 회차**의 `50 PASS / 0 FAIL` 화면이다. 회색 168개는 이 선택 실행에서 실행하지 않은 기존 테스트다. 위 표의 전체 회귀 `217/218`은 앞선 전체 실행 XML의 결과이며, 이 캡처를 전체 218개 통과 근거로 사용하지 않는다.

![신규 50개만 선택해 재실행한 실제 Unity Test Runner 결과](media/20261027-verified/unity-test-runner.png)

[GUI 선택 실행의 실제 XML](public-validation/20261027-unity6000.4.8f1/gui-new-50-results.xml) · Test Runner와 Console 전체 캡처(개인 작업 경로가 포함된 원본은 로컬 보존)

Console 캡처에는 이 GUI 회차의 로그 369개·경고 6개·오류 0개가 표시된다. 이 표시를 다른 회차나 프로젝트 전체의 모든 경고가 없다는 의미로 확대하지 않는다.

**발표자 메모:** “정확한 Editor로 현재 소스 복사본을 import하고 테스트를 실행했습니다. 신규 50개는 모두 발견·실행되어 통과했습니다. 전체 218개 중에는 변경하지 않은 기존 손 색상 테스트 1개가 정밀도 비교로 실패해 전체 PASS로 소개하지 않습니다.” 최대 채널 차이는 약 `8.94×10⁻⁸`로 측정했다. PPE 씬 검사는 inactive 객체를 포함한 524개 GameObject·1,048개 Component 범위이며 다른 씬과 실제 HMD를 보장하지 않는다.

**근거:** [실제 검증 요약](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json), [최종 전체 회귀 XML](public-validation/20261027-unity6000.4.8f1/full-editmode-rerun-results.xml), [씬·discovery JSON](public-validation/20261027-unity6000.4.8f1/scene-and-discovery.json), [진단 기록](public-validation/20261027-unity6000.4.8f1/diagnostics.txt), [테스트 보고서](02_TEST_REPORT.md).

## Slide 8. Unity XR 콘텐츠 개발 직무와의 연관성

**XR 인터랙션 구현**

손 추적과 컨트롤러 입력을 활용해 보호구 선택, 착용 및 장비 준비를 구현하는 기술 경험

**C# 프로그램 설계**

장비 상태, 양손 판정, 교육 단계, 이벤트 전달의 책임을 나눈 코드 구조

**안정성 및 문제 해결**

실제 상호작용 결함을 분석하고 수정 경로를 마련한 뒤 Unity 자동 회귀 테스트로 검증

**산업 교육 콘텐츠 이해**

안전 절차를 체험 가능한 가상환경과 교육 피드백으로 연결하는 개발 경험

**마무리 메시지:** 산업 현장의 교육 절차를 안정적인 XR 상호작용과 C# 로직으로 구현하는 개발자로 성장하고자 합니다.

**시각 자료:** 실제 PPE 교육 완료 화면과 확보한 데모 영상이 있으면 사용한다. 지원자가 보유한 2026-09-10 실제 PPE 게임플레이 녹화의 공개 URL은 **등록 예정**이다. 이 영상은 당시 Windows 빌드의 기록이며 10월 개선 반영 증거가 아니다. 최신 HMD 완료 화면은 별도 촬영이 필요하다. [GitHub 저장소](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build)를 연결하며 이번 수정·테스트 기록의 공개 상태는 실제 작업 브랜치·PR로 안내한다.

**발표자 메모:** “장비를 조작하는 입력, 장비 등록 상태, 교육 단계를 나누고 실패 시 재시도 가능한 상태를 만드는 경험을 설명하겠습니다. 자동 검증과 실기기 검수의 차이를 구분하고, 팀의 공용 계약을 지키면서 교육 과정을 구현하는 개발자로 성장하고자 합니다.” PPE Stage 전담 개발 경험과 지원 동기는 본인의 말로 설명한다.

**근거:** [기술 감사](01_TECHNICAL_AUDIT.md), [작업 요약](04_WORK_SUMMARY_KO.md), [PPE 작업 경계](../integration/STAGE_EDITING_BOUNDARIES.md), [GitHub 저장소](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build).

## 발표 전 확인할 사항

- **개인 기여:** 3명 팀 중 지원자가 PPE 준비실 Stage 전체를 전담 개발했다. 공용 App/XR·다른 Stage·Unity Samples·외부 자산과 10월 보강·회귀 검증을 구분한다.
- **실행 버전:** 포함된 Windows demo는 **2026-09-10**, 소스 export는 **2026-09-18**이다. 이번 검증을 반영한 최신 player는 만들지 않았다.
- **수납 사양:** 현재 필수 수납 구현은 측정기·흡착패드·흡착삭 **3종**이다. 집게 포함 4종 요구와의 최종 SOP는 확인이 필요하다.
- **근거 연결:** 코드 보완·테스트는 포트폴리오 작업 브랜치와 PR을 기준으로 안내한다. PR 병합 전 `blob/main` URL을 신규 코드의 근거로 사용하지 않는다.
- **기기와 시각 자료:** Editor 캡처, EditMode 자동 검증, 실제 HMD 시연을 구분한다. 데모 영상 URL·FPS·학습 성과를 만들어 넣지 않는다.
