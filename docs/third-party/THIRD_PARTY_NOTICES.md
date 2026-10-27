# 외부 자산 및 라이선스 고지

이 문서는 저장소에 포함된 외부 구성요소의 확인 가능한 출처와 고지를 모은다. 프로젝트 전체에 새 오픈소스 라이선스를 부여하지 않으며, 팀 소스·모델·음원·텍스처의 권리를 일괄적으로 재허가하지 않는다. 원본 파일과 Unity 컴파일에 필요한 자산은 변경하거나 삭제하지 않았다.

## 폰트

2026-10-27에 실제 폰트 바이너리의 `name` 테이블에서 글꼴명·버전·저작권·라이선스 항목을 읽었다. 전체 7개 파일의 상대 경로, SHA-256 및 내장 고지는 [font-metadata.json](font-metadata.json)에 기록했다. 아래 원문 OFL 고지는 해당 폰트 배포와 함께 유지한다.

| 글꼴 | 포함 위치 | 실제 파일의 저작권·버전 | 라이선스 원문 |
|---|---|---|---|
| Pretendard SemiBold | `Assets/1.JSR/Fonts/PPE/`, `Assets/1.JSR/Resources/`, `Assets/2.YJH/UI/Fonts/`, `Assets/3.SSH/Font/` | Copyright © 2022 Kil Hyung-jin, Version 1.301 | [Pretendard-OFL.txt](Pretendard-OFL.txt) |
| Noto Sans KR | `Assets/2.YJH/UI/Fonts/NotoSansKR-Regular.otf` | © 2014–2021 Adobe, Version 2.004 | [NotoSansCJK-OFL.txt](NotoSansCJK-OFL.txt) |
| Inter | `Assets/Samples/XR Hands/1.7.3/Hand Capture/Materials/Fonts/Inter/Inter-Regular.ttf` | Copyright 2019 The Inter project authors, Version 3.010 | [Inter-OFL.txt](Inter-OFL.txt) |
| Liberation Sans | `Assets/TextMesh Pro/Fonts/LiberationSans.ttf` | 실제 내장 고지는 metadata 기록 참조 | 기존 [LiberationSans - OFL.txt](../../Assets/TextMesh%20Pro/Fonts/LiberationSans%20-%20OFL.txt) |

이 폰트들은 실제 바이너리에 SIL Open Font License 1.1을 명시한다. 글꼴을 수정·재배포할 때에는 원문 고지와 Reserved Font Name 조건을 포함한 해당 OFL 조건을 따른다. 폰트 바이너리는 이번 작업에서 수정하지 않았다. TextMesh Pro 폰트 자산 등 파생 데이터에도 원래 폰트 조건이 적용될 수 있다.

공식 고지 출처:

- Pretendard: [v1.3.1 LICENSE](https://github.com/orioncactus/pretendard/blob/v1.3.1/LICENSE). 원문은 Copyright (c) 2021을 표기하며 실제 파일의 2022 내장 저작권 고지도 위와 metadata에 함께 보존했다.
- Inter: [공식 LICENSE.txt](https://github.com/rsms/inter/blob/master/LICENSE.txt). 원문의 Copyright (c) 2016 고지와 실제 파일의 2019 내장 저작권 고지를 함께 보존한다.
- Noto Sans CJK: [공식 Sans LICENSE](https://github.com/notofonts/noto-cjk/blob/main/Sans/LICENSE). 실제 파일의 Adobe 저작권 고지는 위와 metadata에 보존했다.

## Unity 패키지 및 샘플

Unity/XR 패키지와 Samples는 지원자의 단독 저작물로 소개하지 않는다. 설치된 정확한 패키지의 고지를 복사해 아래에 보존했다. 패키지 버전은 `Packages/manifest.json` 및 `Packages/packages-lock.json`을 기준으로 한다.

| 구성요소 | 버전 | 원본 고지 |
|---|---|---|
| XR Hands 및 해당 Samples | 1.7.3 | [라이선스](Unity-XRHands-LICENSE.md), [Third Party Notices](Unity-XRHands-Third-Party-Notices.md) |
| XR Interaction Toolkit 및 해당 Samples | 3.4.1 | [라이선스](Unity-XRI-LICENSE.md), [Third Party Notices](Unity-XRI-Third-Party-Notices.md) |

이 고지의 보존은 다른 Unity 패키지나 외부 자산에 대한 포괄적 권리 확인을 의미하지 않는다. Unity 패키지에는 각 패키지의 조건을 적용한다. 외부 구성요소의 원래 헤더와 고지도 유지한다.

## 팀 소스와 제작 자산

- 원 프로젝트는 3인 팀 프로젝트이며 지원자가 PPE 준비실 Stage 전체를 담당했다. 다른 Stage와 공용 App/XR 기반은 팀 영역으로 구분한다.
- `Assets/VRHandPoseToolOutput/`는 생성된 Ghost prefab·material을 담고 있다. 현재 PPE 저장소의 `Packages/`에는 VR Hand Pose Tool 배포용 `.tgz`나 해당 도구 의존성이 없다. 도구에서 생성한 출력물의 이름만으로 도구 소스가 포함됐다고 판단하지 않는다.
- 모델·텍스처·TTS/음원 전부에 대한 구입 영수증·개별 출처·계약을 저장소만으로 확인하지는 못했다. 확인되지 않은 자산을 공개 도메인이나 자유 재배포 자산으로 표시하지 않는다. 권리자의 허용 범위를 벗어난 별도 재배포는 이 문서가 허가하지 않는다.
- 폰트 고지는 이번 점검에서 보완했으며, 패키지·자산의 새 버전으로 교체하거나 Unity 프로젝트 버전을 올리지 않았다.
