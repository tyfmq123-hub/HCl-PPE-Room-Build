# PPE 준비실 포트폴리오 자료 모음

3명 팀의 HCl 누출 대응 VR 프로젝트 중 **지원자가 PPE 준비실 Stage 전체 개발을 전담**했습니다. 씬 구성, 보호구 인터랙션, 교육 흐름, 대응 장비 준비, 전용 UI·안내와 관련 Stage 기능이 담당 범위입니다. 다른 Stage 및 공용 App/XR은 팀 개발 영역이며, 2026-10-27 보강과 회귀 검증은 원래 개발 기간 이후 작업입니다.

## 먼저 볼 자료

| 자료 | 링크 |
|---|---|
| 프로젝트 소개 | [README](../../README.md) |
| 편집 가능한 8장 발표 자료 | [공개용 PowerPoint](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx) |
| 열람용 PDF | [공개용 PDF 8장](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pdf) |
| 발표 원고·근거 | [공개용 Markdown](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md) |
| 기술 감사 | [01_TECHNICAL_AUDIT.md](01_TECHNICAL_AUDIT.md) |
| 실제 테스트·수동 검수표 | [02_TEST_REPORT.md](02_TEST_REPORT.md) |
| 변경·검증 작업 요약 | [04_WORK_SUMMARY_KO.md](04_WORK_SUMMARY_KO.md) |

PDF는 최종 편집 가능한 PPTX를 실제 PowerPoint에서 변환한 8장 자료입니다. 이전 PPTX와 제작 원본은 보존하며, 최신 공개용 발표 자료는 위 파일을 기준으로 합니다.

## Unity 검증 근거

**Unity 6000.4.8f1 / 컴파일 오류 0 / 신규 50/50 통과 / 전체 217/218 통과.** 로드한 PPE 씬의 Missing Script와 끊어진 직렬화 참조는 각각 0개입니다. 기존 손 색상 정밀도 테스트 1개 실패가 남았습니다. PlayMode·PCVR·Quest·새 player 빌드는 미실행입니다.

- [검증 요약 JSON](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json)
- [전체 EditMode 최종 XML](public-validation/20261027-unity6000.4.8f1/full-editmode-rerun-results.xml)
- [PPE 씬 검사와 테스트 discovery](public-validation/20261027-unity6000.4.8f1/scene-and-discovery.json)
- [실제 GUI 신규 50개 재실행 XML](public-validation/20261027-unity6000.4.8f1/gui-new-50-results.xml)
- [실제 Scene View](media/20261027-verified/ppe-editor-overview.png), [실제 Hierarchy·Inspector](media/20261027-verified/ppe-editor-hierarchy.png), [실제 Test Runner](media/20261027-verified/unity-test-runner.png)

공개 증거는 개인 PC의 절대 경로만 치환한 사본이며 결과·테스트 이름·수치·실행 시각은 유지합니다. 원시 XML·로그·JSON과 기존 백업은 별도로 보존합니다. GUI 캡처는 신규 50개만 선택한 회차이고, 회색 168개는 그 회차의 미실행 항목입니다. 전체 결과는 별도 전체 실행 XML을 따릅니다.

## 중요 소스

| 관심 항목 | 코드 |
|---|---|
| 교육 순서·중복 상태 | [PPESequenceManager](../../Assets/1.JSR/Scripts/PPESequenceManager.cs), [PPEEquipmentManager](../../Assets/1.JSR/Scripts/PPEEquipmentManager.cs) |
| Grab·놓기·착용 | [PPEDirectGrabProximityFilter](../../Assets/1.JSR/Scripts/PPEDirectGrabProximityFilter.cs), [PPEReleaseEquip](../../Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [PPEInnerGloveDonning](../../Assets/1.JSR/Scripts/PPEInnerGloveDonning.cs) |
| 양손 장갑·실패 복구 | [PPEGlovePairCoordinator](../../Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs) |
| 가방 수납·최종 상호작용 | [ResponseBagPackingController](../../Assets/1.JSR/Scripts/ResponseBagPackingController.cs), [ResponseBagPrepareInteractable](../../Assets/1.JSR/Scripts/ResponseBagPrepareInteractable.cs), [XRInteractionConfirmRouter](../../Assets/1.JSR/Scripts/XRInteractionConfirmRouter.cs) |
| 신규 회귀 50개 | [Sequence 13개](../../Assets/1.JSR/Editor/PPEPortfolioSequenceTests.cs), [Glove 17개](../../Assets/1.JSR/Editor/PPEGlovePairStateTests.cs), [Bag 20개](../../Assets/1.JSR/Editor/ResponseBagPackingTests.cs) |
| PPE 씬 | [04_PPE.unity](../../Assets/Scenes/04_PPE.unity) |
| 팀 공용 계약 | [Stage 경계](../integration/STAGE_EDITING_BOUNDARIES.md), [Shared XR](../integration/SHARED_XR_ARCHITECTURE.md), [Inventory](../integration/SESSION_INVENTORY_CONTRACT.md) |

## 시연 영상

**2026-09-10 PPE 게임플레이 녹화: 공개 URL 등록 예정.** 지원자가 보유한 실제 녹화이며 현재 작업 폴더에서는 파일을 찾지 못했습니다. 공개 주소를 확보하면 이 문장 아래에 링크를 추가합니다. 가상의 URL이나 영상 썸네일은 넣지 않았습니다.

영상 설명에는 **“2026-09-10 당시 Windows 빌드 플레이 기록. 2026-10-27 코드 보완·회귀 검증 반영 버전이 아님”**을 표시합니다. Editor 캡처를 실제 HMD 플레이 영상으로 소개하지 않습니다.

| 날짜 | 자료 구분 |
|---|---|
| 2026-09-10 | 기존 Windows 빌드와 PPE 게임플레이 녹화 |
| 2026-09-18 | 소스 스냅샷과 export manifest |
| 2026-10-27 | 후속 코드 보완·실제 Unity 검증·포트폴리오 정리 |

기존 실행물은 [Releases](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/releases)에 있습니다. 최신 검증 소스로 새 player를 만들거나 기존 Release를 교체하지 않았습니다. [팀 프로젝트 저장소](https://github.com/tyfmq123-hub/HCl-Leak-Response-VR-Prototype)는 2026-10-27 공개 전환 후 비로그인 접근을 확인했습니다. 팀 저장소의 코드와 이력은 이번 포트폴리오 작업에서 변경하지 않았습니다.
