# HCl PPE Room — Standalone Demo

PPE 준비실을 단독 실행하는 Unity 프로젝트 소스와 Windows 64비트 실행 빌드입니다.

## 바로 실행하기

[Releases](https://github.com/tyfmq123-hub/HCl-PPE-Room-Build/releases)에서 `HCl-PPE-Room-Win64.zip`을 다운로드하고 **전체 압축을 해제**한 뒤 `HClTrainingPPEDemo.exe`를 실행하세요. EXE만 따로 옮기면 실행되지 않습니다. VR 사용 시 헤드셋과 PC의 OpenXR 런타임을 준비하세요.

## 소스 받기

큰 모델·텍스처와 `Runtime/Win64/` 실행 파일은 Git LFS로 관리합니다. 소스 개발 시 Git LFS가 필요합니다.

```sh
git lfs install
git clone https://github.com/tyfmq123-hub/HCl-PPE-Room-Build.git
cd HCl-PPE-Room-Build
git lfs pull
```

Unity Hub에서 이 폴더를 프로젝트로 추가하고 **Unity 6000.4.8f1**로 엽니다. Windows 빌드 지원 모듈이 필요합니다.

## PPE 전용 빌드

`Assets/Settings/Build Profiles/PPE Portfolio Demo Win64.asset` 프로필을 선택해 활성화하고 빌드합니다. 프로필은 `HCL_PPE_DEMO`를 정의하며 `04_PPE`를 첫 씬으로 사용합니다. 프로필 재생성 메뉴는 `Tools > App > Create PPE Portfolio Demo Build Profile`입니다. Classic Build Settings만 사용하면 PPE 단독 동작이 활성화되지 않을 수 있습니다.

소스에는 원본 App 및 다른 Stage 자산도 포함되어 있습니다. 기존 데모의 참조와 빌드 프로필을 유지하기 위한 것이며, 실행 범위는 PPE 전용 데모 코드가 제어합니다.

## 버전과 검증 범위

- 원본: [HCl-Leak-Response-VR-Prototype](https://github.com/tyfmq123-hub/HCl-Leak-Response-VR-Prototype)
- `integration/ppe-portfolio-demo` 작업 폴더의 소스와 미커밋 PPE 전용 변경을 2026-09-18에 별도 스냅샷으로 보관했습니다.
- 실행 빌드는 **2026-09-10에 만들어진 기존 빌드**입니다. 이번 업로드에서 재빌드하지 않았으며, 9월 14일 본 프로젝트에 적용한 신발 고스트 수정본이 아닙니다.
- 파일 복사 무결성과 필수 실행 파일 존재를 확인했습니다. 이번 업로드에서 Unity 재컴파일 및 실제 VR 실행은 수행하지 않았습니다.
- `EXPORT-MANIFEST.json`에 원본 기준 커밋과 내보낸 파일 SHA-256을 기록했습니다. `docs/`의 과거 검증 기록은 당시 버전에 대한 기록입니다.
- Unity 캐시, 로컬 로그, 사용자 설정, 배포 불필요 디버그 심볼은 제외했습니다.

새 저장소의 이력은 이 스냅샷부터 시작합니다. 원본 프로젝트와 기존 Git 이력은 변경하지 않습니다. 포함된 외부 자산과 패키지의 라이선스는 각 원본 조건을 따르며, 이 저장소는 별도의 포괄적 재배포 라이선스를 부여하지 않습니다.
