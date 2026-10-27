# Unity XR 콘텐츠 개발 지원 포트폴리오 초안

대상: 사용자가 제시한 **Letuin Edu 신입 Unity XR/VR 콘텐츠 개발 직무**. 산업 교육·레벨 구성·XR 상호작용·C# 구현에 맞춘 발표 초안이며 현재 채용공고를 외부 확인한 자료는 아니다. 총 8장, 약 5~7분 설명을 권장한다.

이 파일은 **실제 Unity 검증 전 작성한 초기 초안**이다. 최신 [공개용 PowerPoint 8장](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx)과 [최신 원고](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md)를 우선한다. 이전 PPTX는 로컬에 보존한다. 텍스트·구조도·표와 발표자 메모를 편집할 수 있다. 실제 Unity 화면은 아직 촬영하지 않았으므로 발표자 메모의 캡처 제안을 따라 교체한다.

**담당 확인 반영:** 원래 프로젝트는 3명 팀 개발이며 지원자는 PPE 준비실 Stage 전체의 씬·상호작용·교육 흐름·장비 준비·전용 UI와 관련 Stage 기능을 전담 개발했다. 공용 App/XR·다른 Stage·외부 자산은 구분한다. 2026-10-27 코드 보완·회귀 검증은 원래 개발 기간 이후 작업이다. 아래 미실행 표기는 초안 당시 기록이며 최신 검증 결과를 대신하지 않는다.

캡처와 데모 영상은 아직 확보하지 않았다. 아래 시각 자료는 **촬영 지시 또는 코드 기반 도식**이며 실제 Unity 실행 화면을 꾸며 만든 이미지가 아니다. 실행·테스트 결과는 [02_TEST_REPORT.md](02_TEST_REPORT.md)를 기준으로 한다.

## Slide 1. HCl 누출 대응 VR 교육과 PPE 준비실

**화면 본문**

- 염산 누출 상황에서 현장 진입 전 보호구와 대응 장비를 준비하는 VR 교육 콘텐츠
- PPE 준비실: 착용 순서 안내, 직접 Grip 상호작용, 장비 수납과 완료 확인
- 전체 팀 프로젝트 중 PPE Stage를 중심으로 설명하는 기술 포트폴리오
- 지원자: **[이름 확인 필요]**, 담당 기간: **[확인 필요]**

**추천 화면**: Unity `04_PPE` Game View의 화이트보드·캐비닛·장비 준비대가 함께 보이는 전경. 헤드셋 또는 검증된 Editor에서 직접 촬영한다. 미확보 상태에서는 “PPE 준비실 전경 캡처 예정”이라고 표시한다.

**실제 근거**: [04_PPE.unity](../../Assets/Scenes/04_PPE.unity), [PPE_TASK.md](../tasks/PPE_TASK.md), [역할 경계](../integration/STAGE_EDITING_BOUNDARIES.md).

**면접 설명 메모**: “교육생이 누출 현장에 바로 접근하기 전에 보호구와 장비를 순서대로 준비하는 공간입니다. 저는 PPE 담당 범위와 공용 기반을 구분해서 설명하겠습니다. 3명 팀 중 제가 PPE 준비실 Stage 전체 개발을 전담했습니다.”

## Slide 2. PPE Stage와 공용 App의 연결

**화면 본문**

- App이 공용 XR rig와 Scene 흐름을 관리한다.
- PPE Stage는 착용·준비 순서, 상호작용, 안내와 완료 이벤트를 소유한다.
- `OnPPESequenceCompleted`를 기존 App Adapter가 받아 다음 흐름을 진행한다.
- Scene 간에는 장비 ID와 수량을 전달한다. 장비 GameObject는 각 Stage에 둔다.

**추천 도식**: 편집 가능한 세로 구조도. `App / Common XR`에서 `PPE Stage`로 연결하고, PPE 내부의 `Interaction → Equipment → Sequence`를 표시한다. 오른쪽에는 `SessionEquipmentInventory` 데이터 경로, 아래에는 `PreEntry / LeakResponse`를 배치한다. 타 Stage는 공용 연계 범위라는 점을 표기한다.

**실제 근거**: [SHARED_XR_ARCHITECTURE.md](../integration/SHARED_XR_ARCHITECTURE.md), [Session Inventory 계약](../integration/SESSION_INVENTORY_CONTRACT.md), [PPESequenceManager.cs](../../Assets/1.JSR/Scripts/PPESequenceManager.cs), [StageLifecycleAdapter.cs](../../Assets/Scripts/App/StageLifecycleAdapter.cs), [SessionEquipmentInventory.cs](../../Assets/Common/XR/SessionEquipmentInventory.cs).

**면접 설명 메모**: “Stage 내부 동작을 고치더라도 공용 Camera나 Scene 전환을 함께 수정하지 않았습니다. Stage는 교육 완료라는 사실을 이벤트로 전달하고, 이후 Scene 이동은 App이 맡습니다. Inventory도 Transform 대신 ID 데이터를 유지합니다.”

## Slide 3. 기술 스택과 담당 경계

**화면 본문**

| 항목 | 확인한 구성 |
|---|---|
| 개발 | Unity 6000.4.8f1, C# |
| XR | XRI 3.4.1, XR Hands 1.7.3, OpenXR 1.16.1 |
| 제작·관리 | PPE Scene/Editor 도구, Git, Git LFS |
| PPE 담당 범위 | `Assets/1.JSR/`, `04_PPE`, 전용 UI/TTS/상호작용 |
| 공용·팀 범위 | App/Shared XR, 타 Stage, Unity Samples·외부 자산 |

PCVR/OpenXR 및 Meta Quest를 고려한 소스 경로가 존재한다. **이번 수정의 기기 실행은 미검증**이다.

**추천 화면**: Unity Hierarchy에서 PPE manager·캐비닛·대응 장비를 펼친 화면과 짧은 역할 표. PPE Stage 전담 책임과 공용·팀 개발 영역을 함께 표시한다.

**실제 근거**: [ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt), [manifest.json](../../Packages/manifest.json), [packages-lock.json](../../Packages/packages-lock.json), [PPE_WORKER.md](../codex/PPE_WORKER.md), [수정 경계](../integration/STAGE_EDITING_BOUNDARIES.md).

**면접 설명 메모**: “Unity와 C#으로 PPE 동작을 다루고 XRI의 select lifecycle을 사용했습니다. 공용 XR과 타 Stage는 팀 자산입니다. 패키지나 샘플을 직접 제작한 것처럼 소개하지 않으며, PPE Stage 전체를 전담한 범위와 팀 공용 코드 사용을 구분해 설명하겠습니다.”

## Slide 4. Grab과 Release로 완성하는 착용 상호작용

**화면 본문**

- 손을 장비에 가까이 가져가 Grip으로 잡는다. select filter가 물리 collider와 손의 거리를 확인한다.
- 장갑은 반대 손 target, 장화는 하체 target, 얼굴·몸 장비는 HMD/몸 기준 target을 사용한다.
- 장갑·장화의 Ghost는 위치 안내용이다. 실제 착용은 놓는 순간 검증한다.
- 추적 유효성·거리·필요 시 회전·교육 순서를 확인한 뒤 장비 상태를 등록한다.

**추천 화면/도식**: 실제 왼쪽 장갑을 오른손으로 잡은 상태와 release 직전 target의 캡처. 미확보 시 `SelectEntered → target 검증 → SelectExited → Equip` 코드 기반 도식을 사용한다. visible far ray를 현재 PPE 기본 동작처럼 묘사하지 않는다.

**실제 근거**: [PPEHandGrabbable.cs](../../Assets/1.JSR/Scripts/PPEHandGrabbable.cs), [PPEDirectGrabProximityFilter.cs](../../Assets/1.JSR/Scripts/PPEDirectGrabProximityFilter.cs), [PPEInnerGloveDonning.cs](../../Assets/1.JSR/Scripts/PPEInnerGloveDonning.cs), [PPEReleaseEquip.cs](../../Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [PPEPlacementValidation.cs](../../Assets/1.JSR/Scripts/PPEPlacementValidation.cs), [PPEPlayerBodyReference.cs](../../Assets/1.JSR/Scripts/PPEPlayerBodyReference.cs).

**면접 설명 메모**: “잡을 수 있는 거리와 착용 가능한 거리는 서로 다른 판정입니다. 잡은 장비는 손을 따라가고, 놓을 때 live target과 비교합니다. Ghost의 Ready는 안내 상태이므로 실제 Equip 성공과 구분합니다. 추적 손실 grace와 controller fallback은 실기기에서 검수할 부분입니다.”

## Slide 5. 교육 순서와 좌우 상태의 관리

**화면 본문**

- Donning: 속장갑, 보호복, 장화, 호흡보호구, 안면보호구, 외부 장갑
- EquipmentPreparation: 대응가방, 수거용기. 모든 단계 후 Complete
- `PPESequenceManager`: 현재 단계와 완료 여부를 검증한다.
- `PPEEquipmentManager`: ID 등록과 중복을 관리한다. Pair coordinator는 좌우 완성 후 한 ID를 등록한다.
- Reset은 선택 취소·물리 pose·장비 상태·교육 순서를 함께 복원한다.

**추천 도식**: 편집 가능한 Phase 진행도와 `Left / Right / Logical ID` 표. 한쪽만 착용한 경우 논리 ID가 미완료임을 표시한다. Checklist 캡처가 있으면 단계 설명 옆에 한 장만 사용한다.

**실제 근거**: [PPETrainingPhase.cs](../../Assets/1.JSR/Scripts/PPETrainingPhase.cs), [PPESequenceManager.cs](../../Assets/1.JSR/Scripts/PPESequenceManager.cs), [PPEEquipmentManager.cs](../../Assets/1.JSR/Scripts/PPEEquipmentManager.cs), [PPEGlovePairCoordinator.cs](../../Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs), [PPEChecklistPanel.cs](../../Assets/1.JSR/Scripts/PPEChecklistPanel.cs).

**면접 설명 메모**: “왼쪽 장갑을 착용했다고 장갑 단계 전체를 완료하면 안 됩니다. 물리 부품 두 개와 논리 장비 ID 하나를 구분합니다. 한쪽 착용 상태는 유지하되 두 번째 등록이 실패했을 때 그 시도만 복구하도록 보완했습니다. 완료 이벤트는 수거용기까지 끝나야 발생합니다.”

## Slide 6. 실제 결함 두 가지와 복구 전략

**화면 본문**

| 확인한 문제 | 원인 | 이번 보완 |
|---|---|---|
| 가방에서 떨어져 누른 Grip도 최종 완료로 처리 | 세 장비 수납 뒤 전역 Grip callback이 가방 문맥을 확인하지 않음 | 활성 가방의 live hover/select와 입력 손을 확인하고 router와 fallback이 같은 검사 사용 |
| 논리 등록 실패 후 좌우 장갑 완료 flag가 남음 | 두 번째 flag를 먼저 세운 뒤 `Equip` 실패를 복구하지 않음 | 거부된 side의 flag·pose·grab·시각을 복원하고 먼저 성공한 side 유지 |

`PPEReleaseEquip`도 coordinator의 실패를 반환하도록 연결했다. **수정 소스와 회귀 테스트 작성 완료, Unity 실행 결과는 미검증**이다.

**추천 도식**: 실패 직전/직후의 상태 표. 가방은 `원격 Grip = 거부`, 장갑은 `Left=true, Right=false, Logical=false` 복구 예시를 보여준다. 가짜 실행 로그나 before/after 화면을 만들지 않는다.

**실제 근거**: [ResponseBagPackingController.cs](../../Assets/1.JSR/Scripts/ResponseBagPackingController.cs), [ResponseBagPrepareInteractable.cs](../../Assets/1.JSR/Scripts/ResponseBagPrepareInteractable.cs), [XRInteractionConfirmRouter.cs](../../Assets/1.JSR/Scripts/XRInteractionConfirmRouter.cs), [PPEGlovePairCoordinator.cs](../../Assets/1.JSR/Scripts/PPEGlovePairCoordinator.cs), [PPEReleaseEquip.cs](../../Assets/1.JSR/Scripts/PPEReleaseEquip.cs), [기술 감사](01_TECHNICAL_AUDIT.md).

**면접 설명 메모**: “입력 자체와 그 입력이 유효한 대상에서 발생했는지는 별개였습니다. 마지막 가방 Grip에도 현재 대상과 손의 문맥을 확인했습니다. 장갑은 임시 상태를 manager에 전달하되 실패하면 두 번째 시도만 되돌립니다. 기존 public 계약과 물리 상호작용을 유지하는 범위로 수정했습니다.”

## Slide 7. 테스트와 검증 상태

**화면 본문**

| 구분 | 상태 |
|---|---|
| 신규 테스트 | 순서·중복·reset, 좌우 완료/등록 실패 복구, 가방 수납·무효 최종 Grip 테스트 작성 |
| Unity 컴파일 / EditMode | **NOT VERIFIED**. 정확한 Unity 6000.4.8f1 부재 |
| PlayMode / PCVR / Quest | **NOT VERIFIED**. 수동 체크리스트 준비 |
| 기존 데모 | 2026-09-10 빌드. 이번 수정 미포함 |
| 소스 스냅샷 | 2026-09-18 export. 데모와 동작 동일성 미보장 |

실행 로그가 생긴 뒤에만 PASS/FAIL과 플랫폼 지원 실적을 갱신한다.

**추천 화면**: 현시점에는 편집 가능한 위 상태 표. 나중에 정확한 Unity 버전의 Test Runner 결과·전체 완료 화면·Quest 검수 기록으로 교체한다. 성능 수치는 Profiler capture 후에만 넣는다.

**실제 근거**: [PPEPortfolioSequenceTests.cs](../../Assets/1.JSR/Editor/PPEPortfolioSequenceTests.cs), [PPEGlovePairStateTests.cs](../../Assets/1.JSR/Editor/PPEGlovePairStateTests.cs), [ResponseBagPackingTests.cs](../../Assets/1.JSR/Editor/ResponseBagPackingTests.cs), [02_TEST_REPORT.md](02_TEST_REPORT.md), [EXPORT-MANIFEST.json](../../EXPORT-MANIFEST.json).

**면접 설명 메모**: “실행한 검증과 준비한 테스트를 나눠 기록했습니다. 현재 Unity 테스트 통과 수치를 말할 수 없습니다. 다음 단계는 정확한 Editor로 컴파일와 EditMode를 실행하고, 양손·잘못된 순서·부분 reset·가방 Grip을 HMD에서 확인하는 것입니다.”

## Slide 8. 산업 교육 XR 직무와 연결되는 경험

**화면 본문**

- 산업 교육 절차를 단계별 상태와 피드백으로 구현하는 구조를 설명할 수 있다.
- Unity Scene에서 장비 배치와 target을 연결하고 XRI 수명에 맞춰 C# 동작을 구성한다.
- 좌우 부품·논리 장비·교육 단계의 상태 불일치를 찾아 복구 경로를 만든다.
- 팀 공용 계약을 보호하고 근거 코드·검증 범위를 문서로 전달한다.

제출 전 우선 작업: 정확한 Editor 테스트, 기기에서 전체 과정 완주, 짧은 데모 촬영. **집게 포함 SOP 확인이 필요**하다. PPE Stage 전담 책임은 지원자 확인으로 확정했다.

**추천 화면**: 실제 촬영 후 30~60초 데모의 마지막 완료 화면과 저장소 링크. 현재는 “데모 캡처 예정” 텍스트와 신규 문서/소스 링크로 구성한다. 지원자의 이름·연락처는 확인 후 추가한다.

**실제 근거**: [README](../../README.md), [01_TECHNICAL_AUDIT.md](01_TECHNICAL_AUDIT.md), [02_TEST_REPORT.md](02_TEST_REPORT.md), [PPE_WORKER.md](../codex/PPE_WORKER.md), [PPE 인수인계](../handoffs/ppe/LATEST.md).

**면접 설명 메모**: “신입 개발자로서 장비 상호작용과 교육 진행을 연결하고, 실패 상태를 재현 가능한 코드와 테스트로 설명하는 점을 보여드리고 싶습니다. 실기기 검수가 남은 부분을 공개하고 팀의 공용 기능과 제 담당 범위를 구분해 협업하겠습니다. 실제 참여 경험과 지원 동기는 본인의 말로 덧붙이겠습니다.”

## 제출 전 확인

- PPE Stage 전담 책임을 명확히 하고 공용 App/XR·타 Stage·외부 자산의 저작 범위를 구분한다.
- 9월 10일 demo, 9월 18일 source, 이번 수정/검증 버전을 함께 표기한다.
- 세 종류 수납 구현을 네 종류 또는 집게 수납 완료라고 부풀리지 않는다.
- 현재 PPE profile은 near 상호작용 중심이다. Ray selection은 실제 설정과 기기 검수 범위를 나눠 설명한다.
- 추적 손실 시 controller fallback 및 마지막 pose 허용 여부를 기기에서 확인한다.
- 산업 안전 절차의 적합성은 교육/SOP 담당자와 확인한다. 교육 효과·성과 지표·FPS·전체 테스트 PASS를 만들어 넣지 않는다.
