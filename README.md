# HCl 누출 대응 VR · PPE 준비실

**화학물질 누출 현장에 들어가기 전, 보호구를 순서대로 착용하고 대응 장비를 준비하는 Unity XR 교육 콘텐츠입니다.**

3명이 개발한 HCl 누출 대응 VR 프로젝트 가운데 **지원자는 PPE 준비실 Stage의 씬·보호구 상호작용·교육 흐름·장비 준비·전용 UI와 안내 기능을 전담 개발했습니다.** 다른 Stage와 공용 App/XR 기반은 팀 개발 영역입니다. 이 저장소는 PPE 중심 소스 스냅샷과 당시 Windows 데모, 이후 포트폴리오 보강·검증 기록을 제공합니다.

![실제 Unity 6000.4.8f1 PPE 준비실 Scene View](docs/portfolio/media/20261027-verified/ppe-editor-overview.png)

*실제 04_PPE 씬을 연 Unity Editor의 EditMode 캡처입니다. 실기기 플레이 화면과 구분합니다.*

| 한눈에 보기 | 내용 |
|---|---|
| 교육 목적 | 보호구 착용과 대응 장비 준비 절차를 직접 조작하며 반복 연습 |
| 핵심 구현 | 근접 Grab, 신체 목표에 놓기, 양손 장갑 판정, 순서·중복 검사, 가방 수납, UI·TTS 안내 |
| 개발 환경 | Unity **6000.4.8f1** · C# · XRI **3.4.1** · XR Hands **1.7.3** · OpenXR **1.16.1** · URP |
| 실제 검증 | **C# 컴파일 오류 0개 / 신규 회귀 50/50 통과 / 전체 회귀 217/218 통과** |
| 남은 검증 | 기존 손 색상 정밀도 테스트 1개 실패, PCVR·Quest·PlayMode·새 player 빌드 미검증 |

[포트폴리오 자료 모음](docs/portfolio/PORTFOLIO_INDEX.md) · [8장 PowerPoint](docs/portfolio/03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx) · [PDF](docs/portfolio/03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pdf) · [발표 원고](docs/portfolio/03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md) · [테스트 보고서](docs/portfolio/02_TEST_REPORT.md)

## 교육 흐름

**속장갑 → 보호복 → 안전장화 → 호흡보호구 → 안면보호구 → 외부장갑 → 대응가방 → 수거용기**

학습자는 보호구를 잡아 목표 부위에 가져간 뒤 Grip을 놓습니다. 현재 순서, 목표 위치와 거리, 추적 유효성 등을 통과해야 착용 상태가 등록됩니다. 잘못된 순서는 거절하며 고스트·체크리스트·토스트·음성이 다음 행동을 안내합니다.

가방에는 **측정기·흡착패드·흡착삭 3종**을 수납하고, 수납 후 가방을 대상으로 한 유효한 상호작용으로 준비를 완료합니다. 이전 작업지시서에는 집게 포함 4종 요구가 남아 있어 최종 교육 SOP와의 합의가 필요합니다. 이번 보강에서는 장비 구성과 교육 규칙을 임의로 변경하지 않았습니다. 법정 교육 인증이나 학습 효과를 검증한 결과는 아닙니다.

## XR 상호작용과 C# 구조

PPEDirectGrabProximityFilter는 손과 물리 Collider의 근접 조건으로 Grab을 제한합니다. PPEReleaseEquip은 놓는 순간 장비별 신체 목표와 거리·회전 조건, 현재 교육 순서를 확인합니다. PPEInnerGloveDonning은 속장갑을 잡은 손과 착용 대상 손을 구분합니다. 고스트가 보여 주는 안내 상태와 실제 착용 성공 판정은 별도로 관리합니다.

| 책임 | 주요 컴포넌트 |
|---|---|
| 입력·착용 판정 | [근접 필터](Assets/1.JSR/Scripts/PPEDirectGrabProximityFilter.cs), [Release Equip](Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [속장갑](Assets/1.JSR/Scripts/PPEInnerGloveDonning.cs) |
| 장비 상태·교육 순서 | [Equipment Manager](Assets/1.JSR/Scripts/PPEEquipmentManager.cs), [Sequence Manager](Assets/1.JSR/Scripts/PPESequenceManager.cs), [Pair Coordinator](Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs) |
| 대응 장비 준비 | [Packing Controller](Assets/1.JSR/Scripts/ResponseBagPackingController.cs), [Bag Interactable](Assets/1.JSR/Scripts/ResponseBagPrepareInteractable.cs), [Response Equipment](Assets/1.JSR/Scripts/ResponseEquipmentManager.cs) |
| 안내·상태 전달 | [Placement Guide](Assets/1.JSR/Scripts/PPEPlacementGuideManager.cs), [TTS](Assets/1.JSR/Scripts/PPETTSManager.cs), [Loadout](Assets/1.JSR/Scripts/PPELoadoutData.cs) |

입력·배치 판정 뒤 Equipment Manager가 장비를 등록하고 Sequence Manager가 교육 단계를 진행합니다. 양손 장갑은 좌우 물리 오브젝트를 개별 추적하며, 양쪽 착용과 최종 등록이 성공해야 하나의 논리 장비가 됩니다. UI·TTS는 상태 이벤트를 읽습니다. PPE 완료 이벤트와 장비 ID·수량은 공용 App/Session Inventory 계약으로 전달하고 전체 씬 전환은 App이 담당합니다.

## 개인 담당과 팀 프로젝트

| 구분 | 개발 책임 |
|---|---|
| **지원자 전담** | **PPE 준비실 Stage 전체**: 04_PPE 씬 구성, 보호구 인터랙션·착용 판정, 교육 순서, 대응 장비 준비, 전용 UI·고스트·음성 안내와 관련 Stage 기능 |
| 3명 팀 개발 | 전체 HCl 대응 교육의 다른 Stage와 통합 작업. 전체 통합 프로젝트를 개인 단독 개발로 소개하지 않음 |
| 공용 App/XR | Assets/Common/XR, Assets/Scripts/App의 XR Origin·입력·씬 전환·Inventory·Telemetry 기반. PPE는 공개 인터페이스 사용 |
| 외부 자산 | Unity/XRI/XR Hands 샘플, 패키지와 외부 모델·텍스처·음원. 원저작자와 원본 라이선스 적용 |

이 담당 설명은 지원자가 확인한 **원래 개발 역할**입니다. **2026-10-27의 결함 보완·회귀 테스트·검증·문서 정리는 원래 개발 기간 이후의 포트폴리오 개선 작업**으로 구분합니다. [Stage 작업 경계](docs/integration/STAGE_EDITING_BOUNDARIES.md)를 유지하며 공용 기반을 임의로 수정하지 않았습니다.

## 두 가지 문제 해결

| 문제 | 분석과 개선 | 검증 |
|---|---|---|
| 가방과 무관한 Grip으로 준비 완료 가능 | 수납 완료 뒤 입력 처리에 대상 문맥이 부족했습니다. 현재 활성 가방의 Hover/Select와 입력 손을 함께 검사하고 오래된 선택 정보로 우회하지 않도록 보강했습니다. | 가방 수납·무효 입력·XRI 선택을 포함한 신규 회귀 검증. 실기기 타이밍 확인은 남음 |
| 장갑 등록 실패 후 양손 상태 고착 가능 | 양손 플래그와 최종 Equip 성공이 어긋날 수 있었습니다. 등록 실패 시 해당 시도의 손 상태와 상호작용을 복구하고, 먼저 성공한 손은 유지해 재시도하도록 수정했습니다. | 양손 완료·실패 복구·중복·reset 회귀 검증. 실제 Grab 재시도 확인은 남음 |

게임플레이 성공 조건을 완화해 테스트를 통과시키지 않았습니다. 실제 Editor 실행에서 발견한 XRI attach 초기화와 Unity null 전제 문제는 **테스트 fixture 2개에서만** 보완했습니다.

## 실제 Unity 검증 결과

**2026-10-27, Unity 6000.4.8f1 (revision f8b72d3d7343)**로 현재 소스의 복사본을 import하고 검증했습니다.

| 항목 | 결과 |
|---|---|
| C# import·컴파일 | 오류 **0개**, import 종료 code **0** |
| 신규 EditMode 회귀 | 발견·실행 **50개**, 통과 **50**, 실패 **0**, Skip **0**, 미발견 **0** |
| 신규 fixture별 | Sequence **13/13**, Glove **17/17**, Bag **20/20** |
| 전체 EditMode | **217/218 통과**, 기존 손 색상 테스트 **1개 실패** |
| 로드한 04_PPE 씬 | **524개 GameObject / 1,048개 Component**, Missing Script **0**, 끊어진 직렬화 참조 **0** |
| 미실행 | PlayMode smoke, PCVR HMD, standalone Quest Android, 새 Windows/Android player 빌드 |

![실제 Unity GUI Test Runner의 신규 회귀 50개 통과](docs/portfolio/media/20261027-verified/unity-test-runner.png)

*캡처는 신규 50개만 선택한 GUI 재실행입니다. 회색 168개는 해당 선택 실행의 미실행 항목이며, 위 전체 회귀 결과는 별도 전체 실행 XML에 근거합니다.*

남은 실패는 변경하지 않은 기존 PPEInnerGloveHandTintTests.ApplyToHand_TintsThenRestoresDefaultHandColor의 정확한 Color 동등 비교입니다. 측정한 최대 채널 차이는 약 **8.94×10⁻⁸**이며 독립 Editor 프로세스에서도 재현했습니다. 기존 assertion이나 tint 동작은 변경하지 않았고 **218/218 통과로 소개하지 않습니다.**

[테스트 보고서·한국어 PCVR/Quest 체크리스트](docs/portfolio/02_TEST_REPORT.md) · [기계 판독 요약](docs/portfolio/public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json) · [전체 실행 XML](docs/portfolio/public-validation/20261027-unity6000.4.8f1/full-editmode-rerun-results.xml)

공개 검증 자료에서는 개인 PC의 절대 경로만 치환했습니다. 원시 증거는 별도로 보존하며 테스트 이름·결과·수치·시각은 바꾸지 않았습니다. 자동 검증은 실제 손 추적, 물리 release, reset, 음성 연결과 전체 교육 완주를 대신하지 않습니다.

## 영상·실행물·소스 버전

| 날짜 | 자료 | 의미 |
|---|---|---|
| **2026-09-10** | 지원자가 보유한 PPE 게임플레이 녹화와 기존 Windows 빌드 | 당시 동작을 보여 주는 역사적 자료. 10월 개선 반영 증거가 아님 |
| **2026-09-18** | PPE 중심 소스 스냅샷 | EXPORT-MANIFEST.json에 당시 기준과 파일 해시 기록 |
| **2026-10-27** | 코드 보완·실제 Unity 검증·포트폴리오 정리 | 현재 문서와 회귀 결과의 기준. 새 player는 만들지 않음 |

**영상 공개 URL: 등록 예정.** 작업 폴더에서 원본 영상 파일을 찾지 못해 임의 링크를 넣지 않았습니다. [영상 등록 자리와 표기 원칙](docs/portfolio/PORTFOLIO_INDEX.md#시연-영상)에 공개 URL을 추가할 수 있습니다. 실제 FPS·지연·학습 성과 수치는 측정하지 않았습니다.

### 기존 Windows 데모 설치

1. [기존 Releases](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/releases)에서 HCl-PPE-Room-Win64.zip을 받습니다.
2. **전체 압축을 해제**합니다. EXE와 _Data, Unity DLL 등 동봉 파일을 함께 유지합니다.
3. 헤드셋과 PC의 OpenXR 런타임을 준비한 뒤 HClTrainingPPEDemo.exe를 실행합니다.

이 데모는 **2026-09-10 빌드**입니다. 이후 신발 고스트 수정, 9월 18일 소스와 10월 보완이 모두 반영됐다고 보장하지 않습니다. 기존 Release는 보존하며 최신 검증 소스로 만든 실행물로 소개하지 않습니다.

### Unity 소스 열기

    git lfs install
    git clone https://github.com/tyfmq123-hub/HCl-PPE-Room-Build.git
    cd HCl-PPE-Room-Build
    git switch codex/ppe-portfolio-improvement
    git lfs pull

Unity Hub에서 **6000.4.8f1**로 열고 Assets/Scenes/04_PPE.unity를 확인합니다. 패키지와 프로젝트 버전은 유지합니다. 신규 테스트는 빈 씬의 EditMode Test Runner에서 PPEPortfolioSequenceTests, PPEGlovePairStateTests, ResponseBagPackingTests를 선택합니다.

PPE 전용 빌드 프로필은 Assets/Settings/Build Profiles/PPE Portfolio Demo Win64.asset이며 HCL_PPE_DEMO를 정의합니다. 이번 작업에서는 새 빌드를 생성하지 않았습니다. 새 데모는 남은 검증을 마친 뒤 별도 출력 경로에서 만들어 기존 Runtime/Win64와 Release를 보존해야 합니다. Quest Link/Air Link는 **PCVR**이며 Quest Android APK 검증과 구분합니다.

## 자료와 제한

최신 개선 자료는 [공개 작업 브랜치](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/tree/codex/ppe-portfolio-improvement)와 [검토용 PR #1](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/pull/1)에 있으며 아직 main에 병합하지 않았습니다. [공개 결과와 지원 전 수동 작업](docs/portfolio/05_PUBLICATION_REPORT_KO.md)을 함께 확인할 수 있습니다.

- [포트폴리오 목차](docs/portfolio/PORTFOLIO_INDEX.md): 발표 자료, 기술 감사, 테스트와 중요 소스
- [기술 감사](docs/portfolio/01_TECHNICAL_AUDIT.md): 구현 근거, 공용 계약과 남은 위험
- [작업 요약](docs/portfolio/04_WORK_SUMMARY_KO.md): 원래 개발과 후속 보강·검증의 구분
- [팀 프로젝트 저장소](https://github.com/tyfmq123-hub/HCl-Leak-Response-VR-Prototype): 2026-10-27 공개 전환 후 비로그인 접근 확인. 코드·커밋 이력은 이번 작업에서 변경하지 않았습니다.

**새 포트폴리오 demo의 검증 완료 판정은 보류입니다.** 기존 정밀도 테스트 실패와 실기기 전체 흐름·추적 손실·부분/완전 reset·TTS·Inventory 전달 검수가 남았습니다. 3종/4종 수납 SOP와 거절 Telemetry 의미도 통합 확인 대상입니다. 이 저장소는 외부 자산에 대한 포괄적인 재배포 라이선스를 부여하지 않습니다.
