---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
---

<style>
section {
  font-family: 'Segoe UI', 'Noto Sans KR', sans-serif;
}
h1 {
  color: #4ecdc4;
}
h2 {
  color: #ff6b6b;
}
code {
  background-color: #2d2d44;
  padding: 2px 6px;
  border-radius: 3px;
}
pre {
  background-color: #16213e;
  border: 1px solid #4ecdc4;
  border-radius: 8px;
  padding: 16px;
}
blockquote {
  border-left: 4px solid #4ecdc4;
  background-color: #2d2d44;
  padding: 12px 16px;
  margin: 16px 0;
}
</style>

<!-- _class: lead -->
# 🚀 Session 14: Steam 출시 & CI/CD
## Steamworks.NET·GitHub Actions·steamcmd 업로드

**Steamworks 연동, CLI 빌드, GitHub Actions CI/CD**

---

# 📋 목차

1. Steamworks SDK 소개
2. Steam 출시 준비
3. Unity CLI 빌드 자동화
4. GitHub Actions CI/CD
5. steamcmd 활용
6. AI 프롬프트로 CI/CD 구축

---

<!-- _class: lead -->
# 1️⃣ Steamworks SDK 소개

**Steam 플랫폼과 연동하는 기술 스택**

---

# Steamworks란?

**Steam 플랫폼 통합 SDK**

Steamworks는 Valve에서 제공하는 Steam 플랫폼 통합 도구입니다.

- **Steam API**: 업적, 리더보드, 저장소 등 Steam 기능 연동
- **Steam Networking**: P2P 네트워킹 및 릴리스
- **Steam Input**: 컨트롤러 지원
- **Steam Cloud**: 세이브 데이터 클라우드 동기화

```csharp
// Steamworks.NET 기본 사용
using Steamworks;

void Start() {
    if (SteamManager.Initialized) {
        string name = SteamFriends.GetPersonaName();
        Debug.Log($"Steam 사용자: {name}");
    }
}
```

---

# Steamworks.NET 설치

**Unity Package Manager로 설치**

1. Window > Package Manager > Add package from git URL
2. 아래 URL 입력:

```
https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net
```

또는 manifest.json에 직접 추가:

```json
{
  "dependencies": {
    "com.rlabrecque.steamworks.net": "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net"
  }
}
```

---

# Steam App ID 설정

**steam_appid.txt 파일 생성**

프로젝트 루트에 `steam_appid.txt` 파일을 생성합니다.

```
480
```

- 480: Spacewar (Steamworks 테스트용 묵시적 App ID)
- 실제 출시 시 Steam에서 발급받은 App ID로 변경

```csharp
// SteamManager 스크립트 자동 생성됨
// 게임 시작 시 자동 초기화
public class SteamManager : MonoBehaviour {
    void Awake() {
        if (!SteamAPI.Init()) {
            Debug.LogError("Steam 초기화 실패");
            return;
        }
    }
}
```

---

# 주요 Steamworks 기능

**자주 사용하는 API**

```csharp
// 업적 달성
SteamUserStats.SetAchievement("ACH_WIN_ONE_GAME");
SteamUserStats.StoreStats();

// 리더보드 점수 등록
SteamUserStats.UploadLeaderboardScore(
    leaderboardHandle, 
    ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
    score
);

// 저장소 데이터 설정
SteamUserStats.SetStat("STAT_KILLS", killCount);
SteamUserStats.StoreStats();
```

---

<!-- _class: lead -->
# 2️⃣ Steam 출시 준비

**Steamworks 파트너 설정과 빌드 업로드**

---

# Steamworks 파트너 등록

**출시 전 필수 절차**

1. **Steamworks 파트너 등록**
   - https://partner.steamgames.com 접속
   - 회사 또는 개인 개발자 등록
   - Steam Direct 수수료 $100 납부

2. **App ID 발급**
   - 새 앱 생성 시 고유 App ID 할당
   - Store 페이지 설정
   - 빌드 관리 설정

3. **Depot 설정**
   - 업로드할 파일 그룹 정의
   - 플랫폼별 분리 (Windows, macOS, Linux)

---

# Depot 구조

**파일 배포 단위**

```
App ID: 1234560 (Your Game)
├── Depot 1234561 (Windows)
│   ├── Game.exe
│   ├── Game_Data/
│   └── steam_api64.dll
├── Depot 1234562 (macOS)
│   └── Game.app
└── Depot 1234563 (Linux)
    └── Game.x86_64
```

**app.vdf 설정 파일**

```vdf
"AppBuild"
{
    "AppID" "1234560"
    "Desc" "Version 1.0.0"
    
    "ContentRoot" "C:\\Builds\\Windows"
    "BuildOutput" "C:\\Builds\\Output"
    
    "Depots"
    {
        "1234561" "depot_windows.vdf"
    }
}
```

---

# Build Configuration

**depot_windows.vdf 예시**

```vdf
"DepotBuild"
{
    "DepotID" "1234561"
    
    "ContentRoot" "C:\\Builds\\Windows"
    "FileMapping"
    {
        "LocalPath" "*"
        "DepotPath" "."
        "recursive" "1"
    }
    
    "FileExclusion" "*.pdb"
    "FileExclusion" "*.zip"
}
```

---

<!-- _class: lead -->
# 3️⃣ Unity CLI 빌드 자동화

**명령줄로 Unity 빌드하기**

---

# Unity Batch Mode

**CLI 빌드 기본 명령어**

```bash
# Windows 빌드
Unity.exe -quit -batchmode -projectPath "C:\\MyProject" -buildWindows64Player "C:\\Builds\\Game.exe"

# macOS 빌드
Unity -quit -batchmode -projectPath "/Users/dev/MyProject" -buildOSXUniversalPlayer "/Users/dev/Builds/Game.app"

# Linux 빌드
Unity -quit -batchmode -projectPath "/home/dev/MyProject" -buildLinux64Player "/home/dev/Builds/Game.x86_64"
```

**주요 옵션**

| 옵션 | 설명 |
|------|------|
| `-quit` | 빌드 후 Unity 종료 |
| `-batchmode` | GUI 없이 백그라운드 실행 |
| `-nographics` | 그래픽스 초기화 없음 |
| `-projectPath` | 프로젝트 경로 |

---

# Build Script 작성

**Editor 스크립트로 자동화**

```csharp
// Assets/Editor/BuildCommand.cs
using UnityEditor;
using System;
using System.Linq;

public class BuildCommand {
    static string[] GetEnabledScenes() {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }
    
    static void BuildWindows() {
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/Windows/Game.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(options);
    }
    
    static void BuildMacOS() {
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = GetEnabledScenes(),
            locationPathName = "Builds/macOS/Game.app",
            target = BuildTarget.StandaloneOSX
        };
        
        BuildPipeline.BuildPlayer(options);
    }
}
```

---

# CLI에서 Build Script 실행

**ExecuteMethod로 빌드 메서드 호출**

```bash
# Windows
Unity.exe -quit -batchmode -projectPath "C:\\MyProject" -executeMethod BuildCommand.BuildWindows -logFile "build.log"

# macOS
/Applications/Unity/Hub/Editor/2022.3.20f1/Unity.app/Contents/MacOS/Unity \
    -quit \
    -batchmode \
    -projectPath "/Users/dev/MyProject" \
    -executeMethod BuildCommand.BuildWindows \
    -logFile "build.log"
```

---

# Version 자동 설정

**빌드 시 자동 버전 증가**

```csharp
// Assets/Editor/AutoVersion.cs
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AutoVersion : IPreprocessBuildWithReport {
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report) {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        
        if (parts.Length == 3) {
            int build = int.Parse(parts[2]) + 1;
            PlayerSettings.bundleVersion = $"{parts[0]}.{parts[1]}.{build}";
            
            PlayerSettings.Android.bundleVersionCode = build;
            PlayerSettings.macOS.buildNumber = build.ToString();
        }
        
        UnityEngine.Debug.Log($"빌드 버전: {PlayerSettings.bundleVersion}");
    }
}
```

---

# 빌드 배치 스크립트

**Windows (.bat)**

```batch
@echo off
set UNITY_PATH="C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.20f1\\Editor\\Unity.exe"
set PROJECT_PATH="C:\\Projects\\MyGame"
set BUILD_PATH="C:\\Builds"

echo [1/3] Windows 빌드 중...
%UNITY_PATH% -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildCommand.BuildWindows -logFile "%BUILD_PATH%\\build_windows.log"

echo [2/3] macOS 빌드 중...
%UNITY_PATH% -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildCommand.BuildMacOS -logFile "%BUILD_PATH%\\build_macos.log"

echo [3/3] 빌드 완료!
pause
```

---

# 빌드 배치 스크립트 (macOS/Linux)

**Shell Script (.sh)**

```bash
#!/bin/bash

UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.20f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="$HOME/Projects/MyGame"
BUILD_PATH="$HOME/Builds"

echo "[1/3] Windows 빌드 중..."
$UNITY_PATH -quit -batchmode \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildCommand.BuildWindows \
    -logFile "$BUILD_PATH/build_windows.log"

echo "[2/3] macOS 빌드 중..."
$UNITY_PATH -quit -batchmode \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildCommand.BuildMacOS \
    -logFile "$BUILD_PATH/build_macos.log"

echo "[3/3] 빌드 완료!"
```

---

<!-- _class: lead -->
# 4️⃣ GitHub Actions CI/CD

**GitHub으로 자동 빌드 파이프라인 구축**

---

# GitHub Actions 개요

**GitHub 저장소에서 자동화된 워크플로우**

- **Trigger**: Push, PR, Schedule 등 이벤트 기반 실행
- **Runner**: GitHub 제공 VM (Ubuntu, Windows, macOS)
- **Job**: 병렬 또는 순차 실행 작업 단위
- **Step**: Job 내에서 순차적으로 실행하는 작업

```yaml
# .github/workflows/build.yml
name: Unity Build

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
```

---

# Unity Builder Action

**game-ci/unity-builder 사용**

```yaml
name: Build

on: [push]

jobs:
  build-windows:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout Repository
        uses: actions/checkout@v4
        with:
          lfs: true

      - name: Build Unity Project
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          targetPlatform: StandaloneWindows64
          buildName: MyGame
          buildPath: build
```

---

# 멀티 플랫폼 빌드

**Matrix Strategy로 여러 플랫폼 동시 빌드**

```yaml
name: Multi-Platform Build

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    name: Build ${{ matrix.targetPlatform }}
    runs-on: ubuntu-latest
    strategy:
      fail-fast: false
      matrix:
        targetPlatform:
          - StandaloneWindows64
          - StandaloneOSX
          - StandaloneLinux64

    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true

      - uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          targetPlatform: ${{ matrix.targetPlatform }}
          buildName: MyGame-${{ matrix.targetPlatform }}

      - uses: actions/upload-artifact@v4
        with:
          name: Build-${{ matrix.targetPlatform }}
          path: build/${{ matrix.targetPlatform }}
```

---

# Secrets 설정

**GitHub 저장소에 민감 정보 등록**

Settings > Secrets and variables > Actions > New repository secret

| Secret 이름 | 설명 |
|-------------|------|
| `UNITY_LICENSE` | Unity 라이선스 파일 (Base64 인코딩) |
| `UNITY_EMAIL` | Unity 계정 이메일 |
| `UNITY_PASSWORD` | Unity 계정 비밀번호 |

**라이선스 파일 생성**

```bash
# Unity Professional/Plus
# Unity Hub에서 라이선스 파일 날개 후 Base64 인코딩
cat Unity_lic.ulf | base64 | pbcopy

# macOS
base64 -i Unity_lic.ulf | pbcopy

# Windows
[Convert]::ToBase64String([IO.File]::ReadAllBytes("Unity_lic.ulf")) | Set-Clipboard
```

---

# Release 자동 생성

**태그 푸시 시 GitHub Release 생성**

```yaml
name: Release

on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  build:
    runs-on: ubuntu-latest
    # ... 빌드 단계 ...

  release:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - uses: actions/download-artifact@v4
        with:
          path: artifacts

      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: artifacts/**/*
          generate_release_notes: true
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

---

<!-- _class: lead -->
# 5️⃣ steamcmd 활용

**Steam에 빌드 자동 업로드**

---

# steamcmd란?

**Steam 클라이언트 명령줄 도구**

Steamworks SDK에 포함된 명령줄 도구로 Steam에 빌드를 업로드합니다.

```bash
# Windows
steamcmd.exe +login <username> <password> +app_update 1007 +quit

# macOS/Linux
./steamcmd.sh +login <username> <password> +app_update 1007 +quit
```

**기능**
- Steam에 빌드 업로드 (steamworks sdk)
- Dedicated 서버 설치
- 게임 파일 다운로드/업데이트

---

# steamcmd 설치

**각 플랫폼별 설치**

**Windows**
```bash
# 1. Steamworks SDK 다운로드
# https://partner.steamgames.com/doc/sdk

# 2. sdk/tools/ContentBuilder/builder 폰더 확인
steamcmd.exe
```

**macOS**
```bash
# Homebrew로 설치
brew install steamcmd
```

**Linux (Ubuntu)**
```bash
# 패키지 설치
sudo add-apt-repository multiverse
sudo apt update
sudo apt install steamcmd

# 또는 수동 설치
mkdir ~/steamcmd
cd ~/steamcmd
wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz
tar -xvzf steamcmd_linux.tar.gz
```

---

# 빌드 업로드 스크립트

**Steam에 자동 업로드**

```bash
#!/bin/bash
# upload_to_steam.sh

STEAMCMD="/path/to/steamcmd"
USERNAME="$1"
PASSWORD="$2"
APP_ID="1234560"
BUILD_VDF="/path/to/app.vdf"

echo "Steam 로그인 및 빌드 업로드 중..."

$STEAMCMD +login "$USERNAME" "$PASSWORD" \
    +run_app_build "$BUILD_VDF" \
    +quit

if [ $? -eq 0 ]; then
    echo "업로드 성공!"
else
    echo "업로드 실패!"
    exit 1
fi
```

---

# GitHub Actions + steamcmd

**CI/CD에 Steam 업로드 통합**

```yaml
name: Build and Deploy to Steam

on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  build:
    # ... 빌드 작업 ...

  deploy:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/download-artifact@v4
        with:
          name: Build-StandaloneWindows64
          path: build/Windows

      - name: Setup steamcmd
        uses: CyberAndrii/setup-steamcmd@v1

      - name: Deploy to Steam
        env:
          STEAM_USERNAME: ${{ secrets.STEAM_USERNAME }}
          STEAM_PASSWORD: ${{ secrets.STEAM_PASSWORD }}
          STEAM_CONFIG_VDF: ${{ secrets.STEAM_CONFIG_VDF }}
        run: |
          echo "$STEAM_CONFIG_VDF" | base64 -d > ~/.local/share/Steam/config/config.vdf
          steamcmd +login "$STEAM_USERNAME" "$STEAM_PASSWORD" \
            +run_app_build scripts/app.vdf \
            +quit
```

---

# Steam Guard 처리

**2단계 인증 자동화**

Steam Guard가 활성화된 경우 SDA(Steam Desktop Authenticator) 또는 ssfn 파일 사용:

```yaml
- name: Setup Steam Guard
  run: |
    mkdir -p ~/.local/share/Steam/config
    echo "${{ secrets.STEAM_SSFN_BASE64 }}" | base64 -d > ~/.local/share/Steam/ssfn
    echo "${{ secrets.STEAM_CONFIG_VDF }}" | base64 -d > ~/.local/share/Steam/config/config.vdf
```

**Secrets 설정:**
- `STEAM_SSFN_BASE64`: ssfn 파일 Base64 인코딩
- `STEAM_CONFIG_VDF`: config.vdf 파일 Base64 인코딩

---

<!-- _class: lead -->
# 6️⃣ AI 프롬프트로 CI/CD 구축

**ChatGPT/Claude로 자동화 스크립트 생성**

---

# 프롬프트 1: Unity 빌드 스크립트

**목적: 멀티 플랫폼 빌드 Editor 스크립트 생성**

```
Unity 2022.3 프로젝트용 멀티 플랫폼 빌드 Editor 스크립트를 작성해주세요.

요구사항:
- Windows 64bit, macOS, Linux 64bit 빌드 메서드
- 현재 씬 목록 자동 수집
- BuildOptions.Development 옵션 설정 가능
- 빌드 결과 로그 출력
- 빌드 폰더 자동 생성

출력 형식: C# Unity Editor 스크립트
```

**기대 출력:**
BuildCommand.cs 파일로 여러 플랫폼 빌드 메서드가 포함된 스크립트

---

# 프롬프트 2: GitHub Actions Workflow

**목적: Unity CI/CD 파이프라인 생성**

```
Unity 프로젝트용 GitHub Actions workflow를 작성해주세요.

요구사항:
- main 브랜치 push 시 자동 빌드
- Windows, macOS, Linux 빌드 (Matrix strategy)
- game-ci/unity-builder 사용
- 빌드 아티팩트 업로드
- 태그 push 시 GitHub Release 자동 생성

Unity 버전: 2022.3.20f1
```

**기대 출력:**
.github/workflows/build.yml 파일로 완전한 CI/CD 파이프라인

---

# 프롬프트 3: Steam 자동 업로드

**목적: Steam에 빌드 자동 배포 스크립트**

```
Unity 빌드를 Steam에 자동 업로드하는 GitHub Actions workflow를 작성해주세요.

요구사항:
- v*.*.* 태그 push 시 실행
- Unity 빌드 먼저 수행 (Windows, macOS)
- steamcmd로 Steam에 업로드
- app.vdf 설정 파일 예시 포함
- Steam Guard 2FA 처리 방법 설명

참고: App ID는 1234560, Depot ID는 1234561
```

**기대 출력:**
Steam 업로드가 포함된 deploy.yml과 app.vdf 예시

---

# 프롬프트 4: 빌드 배치 파일

**목적: 로컬 빌드 자동화 스크립트**

```
Unity 프로젝트를 로컬에서 빌드하는 배치 스크립트를 작성해주세요.

요구사항:
- Windows (.bat) 버전
- Unity 경로 자동 감지 (Unity Hub)
- Windows, macOS, Linux 순차 빌드
- 각 빌드 로그 파일 저장
- 빌드 완료 후 결과 요약 출력
- 빌드 실패 시 오류 메시지 표시

프로젝트 경로: 사용자 입력 또는 현재 디렉토리
Unity 버전: 2022.3.20f1
```

**기대 출력:**
build.bat 파일로 순차 빌드 및 로그 관리 기능 포함

---

# 프롬프트 5: 자동 버전 관리

**목적: 빌드 시 자동 버전 증가**

```
Unity에서 빌드할 때마다 자동으로 버전을 증가시키는 Editor 스크립트를 작성해주세요.

요구사항:
- Semantic Versioning (Major.Minor.Build)
- 빌드 번호 자동 증가
- Android bundleVersionCode 동기화
- iOS buildNumber 동기화
- PlayerSettings.bundleVersion에 저장
- 빌드 전에 버전 확인/수정 UI (선택사항)

스크립트 위치: Assets/Editor/AutoVersion.cs
```

**기대 출력:**
IPreprocessBuildWithReport 인터페이스를 사용한 자동 버전 관리 스크립트

---

# AI 프롬프트 팁

**더 나은 결과를 위한 프롬프트 작성법**

1. **구체적인 정보 제공**
   - Unity 버전, 플랫폼, 파일 경로 명시
   - 예시: "Unity 2022.3.20f1", "Windows Standalone"

2. **요구사항 목록화**
   - 번호 또는 글머리 기호로 정리
   - 우선순위 표시 (필수/선택)

3. **예시 포함**
   - 기대하는 출력 형식 예시 제공
   - 샘플 코드나 설정 파일

4. **제약 조건 명시**
   - 파일 위치, 명명 규칙, 사용 라이브러리
   - 예: "game-ci/unity-builder 사용"

5. **반복 개선**
   - 첫 결과를 바탕으로 추가 요청
   - "X 부분을 Y 방식으로 수정해주세요"

---

# 전체 파이프라인 요약

**Steam 출시까지의 자동화 플로우**

```
[개발자 푸시]
    ↓
[GitHub Actions]
    ├─ Unity 빌드 (Win/Mac/Linux)
    ├─ 아티팩트 업로드
    └─ Release 생성
    ↓
[Steam Upload]
    ├─ steamcmd 실행
    ├─ 빌드 업로드
    └─ Steam 심사 대기
    ↓
[출시]
```

**핵심 파일들:**
- `.github/workflows/build.yml`: CI/CD 파이프라인
- `Assets/Editor/BuildCommand.cs`: 빌드 스크립트
- `scripts/app.vdf`: Steam 빌드 설정
- `build.bat` / `build.sh`: 로컬 빌드

---

# 실전 체크리스트

**Steam 출시 전 확인 사항**

- [ ] Steamworks 파트너 등록 완료
- [ ] App ID 발급 및 설정
- [ ] Depot 설정 완료
- [ ] Unity Build Script 작성
- [ ] GitHub Actions Workflow 설정
- [ ] Secrets 등록 (Unity License, Steam 계정)
- [ ] 테스트 빌드 성공 확인
- [ ] Steam 테스트 환경에서 실행 검증
- [ ] Store 페이지 작성 완료
- [ ] 출시 버전 업로드 및 심사 요청

---

# Q&A

**자주 묻는 질문**

**Q: Unity Personal 라이선스로 CI/CD 가능한가요?**
A: 네, 가능합니다. game-ci에서 Personal 라이선스 활성화 방법을 제공합니다.

**Q: Steam Guard 2FA를 어떻게 자동화하나요?**
A: ssfn 파일과 config.vdf를 GitHub Secrets에 저장하여 자동 로그인합니다.

**Q: 빌드 시간이 너무 오래 걸립니다.**
A: caching을 활용하거나, self-hosted runner를 사용하세요.

**Q: macOS 빌드를 Linux에서 할 수 있나요?**
A: 아니요, macOS 빌드는 macOS runner에서만 가능합니다 (Apple 개발자 계약).

---

# 참고 자료

**공식 문서 및 도구**

- **Steamworks SDK**: https://partner.steamgames.com/doc/sdk
- **Steamworks.NET**: https://steamworks.github.io/
- **game-ci**: https://game.ci/
- **GitHub Actions**: https://docs.github.com/ko/actions

**유용한 GitHub Actions**

```yaml
# Unity 테스트
game-ci/unity-test-runner@v4

# Unity 빌드
game-ci/unity-builder@v4

# Release 생성
softprops/action-gh-release@v1

# steamcmd 설치
CyberAndrii/setup-steamcmd@v1
```

---

<!-- _class: lead -->
# 감사합니다

**Session 14: Steam 출시 + 빌드 자동화**

질문 있으신가요?
