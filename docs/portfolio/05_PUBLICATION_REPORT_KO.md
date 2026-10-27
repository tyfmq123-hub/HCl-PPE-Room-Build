# PPE 포트폴리오 공개 결과 — 2026-10-27

**팀 저장소는 Public으로 전환했다. PPE 포트폴리오는 공개 작업 브랜치와 PR #1에 게시했으며 main에는 아직 병합하지 않았다.** 새 player 빌드나 Release는 만들지 않았고 기존 September Release도 교체하지 않았다.

## 게시 위치와 커밋

- [공개 팀 저장소](https://github.com/tyfmq123-hub/HCl-Leak-Response-VR-Prototype): Public, 비로그인 GitHub API HTTP 200 및 `private=false` 확인. 공개 설정만 변경했으며 코드와 커밋 이력은 보존했다.
- [PPE 작업 브랜치](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/tree/codex/ppe-portfolio-improvement): `codex/ppe-portfolio-improvement`.
- [main 대상 PR #1](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/pull/1): OPEN, 자동 병합 꺼짐. PR 생성 시 GitHub CI 항목은 없었으며 로컬 Unity 검증과 구분한다.

| 커밋 | 내용 |
|---|---|
| `871f806165eebb9e03ba944ed95db5af3dc83819` | 게시 전 기준 및 현재 main의 원본 스냅샷 |
| `f528abf` | PPE 런타임 5개 수정, 회귀 fixture 3개와 meta: 11개 파일 |
| `342765495fab085534f4c3d4e7cefe932f147c2f` | README, 포트폴리오·공개 검증 사본·실제 캡처·고지: 46개 파일 |

이 보고서와 게시 링크를 반영하는 후속 문서 커밋은 PR의 Commits 탭에서 확인한다. 원격 이력을 덮어쓰거나 force-push하지 않았다. 지원자가 최신 자료를 공유할 때에는 병합 전까지 위 작업 브랜치 링크를 사용한다.

## 변경 범위

- 가방의 유효 대상·입력 손 확인과 실패한 장갑 등록의 복구를 보강했다. 교육 순서와 성공 조건을 테스트에 맞추어 완화하지 않았다.
- `PPEPortfolioSequenceTests` 13개, `PPEGlovePairStateTests` 17개, `ResponseBagPackingTests` 20개를 추가했다. 실제 실행에서 드러난 attach 초기화·Unity null 전제는 fixture에서 보완했다.
- 공용 `Assets/Common/XR`, `Assets/Scripts/App`, 다른 Stage, Packages/ProjectSettings와 Unity 버전은 변경하지 않았다. 기존 Runtime과 Release 바이너리도 보존했다.
- README에 실제 준비실·Test Runner 화면, 교육 절차와 C# 책임 구조, 두 결함 개선, 실제 수치와 Windows 데모 설치 안내를 반영했다.
- 3인 팀에서 지원자가 **PPE 준비실 Stage의 씬·상호작용·교육 흐름·장비 준비·UI와 관련 기능 전체를 전담**한 점을 명시했다. 다른 Stage와 공용 XR/App은 팀 영역이며 10월 개선·검증은 원래 개발 기간과 구분했다.

## 실제 검증과 남은 오류

| 항목 | 확인된 결과 |
|---|---|
| Unity Editor | 6000.4.8f1, revision `f8b72d3d7343` |
| import·C# 컴파일 | 오류 0개, import 종료 0 |
| 신규 회귀 | 발견·실행 50, 통과 50, 실패·Skip·미발견 0 |
| 전체 EditMode | 217/218 통과, 기존 테스트 1개 실패 |
| 신규 50개 GUI 선택 재실행 | 50 통과, 0 실패. 나머지 168개는 해당 선택 회차에서 미실행 |
| 로드한 PPE 씬 | 524개 GameObject / 1,048개 Component, Missing Script 0, 끊어진 직렬화 참조 0 |
| 미실행 | PlayMode smoke, PCVR HMD, standalone Quest Android, 새 Windows/Android player 빌드 |

남은 실패는 기존 `PPEInnerGloveHandTintTests.ApplyToHand_TintsThenRestoresDefaultHandColor`의 정확한 Color 비교이며 최대 약 `8.94e-8` 차이를 관측했다. 기존 test·tint 코드를 변경하지 않았고 독립 Editor 프로세스에서도 재현했다. 전체 218개가 통과했다거나 실기기 검증까지 끝났다고 주장하지 않는다. 게시 준비 중 C# 게임 코드를 추가 수정하거나 Unity 테스트를 새로 실행하지 않았으며, 이미 실행한 결과와 동일한 런타임 5개·fixture 3개의 source SHA-256을 대조했다.

## 포트폴리오 파일과 버전

- [편집 가능한 공개용 PPTX 8장](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pptx)
- [Microsoft PowerPoint에서 내보낸 PDF 8장](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.pdf)
- [발표 원고](03_JOB_APPLICATION_PORTFOLIO_KO_PUBLIC_20261027.md), [자료 목차](PORTFOLIO_INDEX.md), [기술 감사](01_TECHNICAL_AUDIT.md), [테스트 보고서·수동 검수표](02_TEST_REPORT.md)
- [공개 Unity 검증 요약](public-validation/20261027-unity6000.4.8f1/actual-validation-summary.json), [원본·사본 해시와 치환 범위](public-validation/20261027-unity6000.4.8f1/PUBLIC_EVIDENCE_MANIFEST.json)

PPTX 구조·레이아웃·폰트·8장 수·재import를 검사했고 실제 PowerPoint로 PDF를 내보낸 뒤 8페이지를 각각 확인했다. 발표 자료에는 실제 Editor 캡처 3개를 사용했다. 과거 PPTX와 제작 원본, 원시 Unity 로그·XML·JSON은 로컬에서 보존했다.

2026-09-10 영상과 Windows 빌드는 당시 동작의 기록이고, 2026-09-18은 소스 스냅샷, 2026-10-27은 후속 개선·검증 기준이다. 실제 9월 PPE 녹화는 지원자가 보유하며 로컬 파일을 찾지 못해 공개 URL 등록 자리를 마련했다. 임의 영상 URL·성능·학습 효과 수치는 넣지 않았다.

## 공개 전 점검과 보존

- 공개 준비 전 미커밋 소스·문서·제작 원본 등 151개 파일을 저장소 밖 복구 ZIP에 보관하고 전체 파일 해시를 검증했다. 앞선 백업도 보존했다.
- PPE 저장소의 LFS 856개를 점검하고 `git lfs fsck`를 통과했다. 새 공개 자료의 LFS 4개를 업로드했다. Unity 캐시·기기로컬 설정·백업·불필요한 바이너리를 새 커밋에 넣지 않았다.
- 팀 저장소의 8개 원격 브랜치, 82커밋과 5,142개 텍스트 blob 버전에서 자격증명 패턴을 점검했다. 추가로 노출하면 안 되는 자격증명은 발견하지 못했다. 612개 LFS 바이너리의 실제 payload까지 전부 검사했다는 의미는 아니다.
- 팀 이력에 남은 VR Hand Pose Tool의 개인 사용 라이선스를 확인한 후, **사용자가 해당 권리자이며 공개 배포를 허용한다고 명시적으로 확인**했다. 이 확인 뒤 팀 저장소를 Public으로 전환했다.
- 실제 폰트 metadata의 OFL과 설치된 Unity/XRI/XR Hands 고지를 [외부 자산 고지](../third-party/THIRD_PARTY_NOTICES.md)에 보완했다. 모델·음원 전체의 계정별 계약을 저장소만으로 독립 검증했다고 주장하거나 포괄적 재배포 허가를 부여하지 않는다.
- 개인 PC 경로가 있는 원시 자료와 전체 Console 캡처는 게시하지 않았다. 공개 XML·JSON은 경로만 치환한 사본이며, 원본·사본 해시와 변환 범위를 기록했다. 테스트 이름·결과·시간·수치는 유지했다. 원문 진단과 라이선스에 포함된 공백은 보존했다.

발견된 구체적 공개 차단 항목은 권리자 허가로 해소했다. GitHub 연결 앱의 PR 생성 403은 관리자 권한을 확인한 GitHub CLI로 처리했으며 추가 사용자 인증은 필요하지 않았다.

## 지원 전 남은 수동 작업

1. 실제 2026-09-10 PPE 녹화를 공개하고 [영상 자리](PORTFOLIO_INDEX.md#시연-영상)에 URL과 당시 버전 설명을 추가한다.
2. [PCVR 17항목·Quest Android 3항목](02_TEST_REPORT.md#6-실무용-pcvr-수동-검수표)과 PlayMode smoke를 실행한다. 장갑 재시도, 가방 Grip, 추적 손실, 부분·전체 reset, TTS와 Inventory 전달을 기록한다.
3. 기존 색상 정밀도 test의 허용 기준과 실제 시각 결과를 검토하고 3종/4종 수납 SOP 및 거절 Telemetry 의미를 팀과 확인한다.
4. 검수 이후 별도 경로에 새 데모를 빌드하고 실행 결과를 확인한다. 기존 September 실행물·Release와 구분한다.
5. PR 내용을 검토한 후 main 병합 여부를 결정한다. 현재 최신 포트폴리오는 **공개 작업 브랜치와 PR에만** 있으며 main에는 아직 없다.
