---
marp: true
theme: default
paginate: true
---


<!-- _class: lead -->

# LUX Guide

## Linalab Unity X

Unity Editor를 AI 작업 허브로 만드는 adapter + automation toolkit


---


# 오늘의 목표

**AI 도구를 Unity 개발 루프 안에 안전하게 연결하는 방법을 이해합니다**

1. LUX가 해결하는 문제와 전체 구조 이해
2. Multi-AI Terminal, Skill Dispatch, Pipeline Editor의 역할 파악
3. 설치, 원격 제어, Git, 동적 코드 실행을 실전 워크플로우로 연결

> 관점은 기능 암기가 아니라 “AI가 Unity 상태를 어떻게 보고 검증하는가”입니다.


---


<!-- _class: lead -->

# Part 1: LUX 소개

## Unity 개발에서 AI 도구가 흩어져 있을 때 생기는 문제


---


# Unity 개발의 AI 도구 분산 문제

**AI는 강력하지만 Unity 개발 표면은 여러 곳으로 나뉘어 있습니다**

- 코드 수정은 `Claude Code`, `Codex`, `OpenCode` 같은 CLI에서 수행
- 결과 확인은 Unity Editor에서 수동으로 Compile, Play, Test 실행
- 이미지 생성은 별도 웹 UI 또는 스크립트에서 수행
- 로그, 스크린샷, 입력 재현은 사람이 직접 모아 전달

문제는 AI 성능 부족이 아니라 **Unity 상태와 AI 실행 루프의 단절**입니다.


---


# LUX의 정의

**LUX(Linalab Unity X)**는 Unity Editor를 AI 작업 허브로 만드는 **AI adapter + automation toolkit**입니다.

- Unity Editor 상태를 AI 도구가 읽고 조작할 수 있게 연결
- 여러 AI CLI를 웹 기반 터미널에서 전환하며 사용
- 컴파일, 테스트, 스크린샷, 로그, Play Mode를 스킬로 실행
- 이미지 생성, 원격 제어, Git, 자동화 테스트를 하나의 개발 루프로 통합

LUX는 AI 모델이 아니라 **AI가 Unity 프로젝트와 협업하는 방식**을 바꿉니다.


---


# Linalab, 라이선스, 대상 환경

| 항목 | 내용 |
|---|---|
| 회사 | **Linalab(린랩)** |
| 프로젝트 | **LUX: Linalab Unity X** |
| 라이선스 | **MIT Open Source** |
| 대상 엔진 | **Unity 6.x**, `Unity 6000.0+` |
| 초점 | Unity Editor 자동화, AI 도구 연결, 제작 파이프라인 통합 |

MIT 라이선스는 학습, 내부 도구화, 팀별 확장에 유리합니다.


---


# 기존 방식 vs LUX 방식

| 관점 | 기존 방식 | LUX 방식 |
|---|---|---|
| AI 실행 위치 | 터미널/웹/IDE에 분산 | Web UI와 Unity Bridge로 통합 |
| Unity 상태 확인 | 사람이 Console/Scene을 읽음 | 스킬이 로그, 오브젝트, 스크린샷 수집 |
| 검증 | 수동 Play/Test | `compile`, `test`, `scene-smoke-test` 실행 |
| 반복 | 복사/붙여넣기 | 명령 → 실행 → 결과 반환 루프 |

핵심은 자동화를 늘리는 것이 아니라 **검증 가능한 반복 루프**를 만드는 것입니다.


---


<!-- _class: lead -->

# Part 2: 아키텍처

## Unity Editor, Rust Gateway, Web UI, AI Tools가 연결되는 방식


---


# 전체 아키텍처 다이어그램

```text
+---------------------------+        +---------------------------+
|        Web UI             |        |         AI Tools          |
|  React SPA + xterm        |        | Claude Code / Codex       |
|  ReactFlow Pipeline       |        | OpenCode                  |
+-------------+-------------+        +-------------+-------------+
              | HTTP/WebSocket                      |
              v                                     |
+-------------+-------------------------------------v-------------+
|                   Rust Gateway CLI                              |
|          Axum HTTP Server + WebSocket + Tool API                |
+-------------+-------------------------------------+-------------+
              | TCP                                 |
              v                                     |
+-------------+-------------------------------------v-------------+
|                         Unity Editor                            |
| Lux Workbench | AI Bridge TCP Server | Tool Dispatcher          |
+-----------------------------------------------------------------+
```


---


# Unity Editor 계층

**Unity Editor는 실제 프로젝트 상태와 실행 권한을 가진 계층입니다**

- `Lux Workbench`: Editor 안에서 LUX 기능을 탐색하고 실행하는 작업대
- `AI Bridge TCP Server`: 외부 명령을 Unity 안으로 받아들이는 통신 지점
- `LuxAIToolDispatcher.cs`: 스킬 이름을 실제 Unity 작업으로 라우팅
- Unity API 접근: Scene, GameObject, Console, Test Runner, Play Mode, AssetDatabase

AI가 Unity API를 직접 호출할 수 없기 때문에 Bridge와 Dispatcher가 필요합니다.


---


# Rust Gateway CLI 계층

**Gateway는 Unity, Web UI, AI Tools 사이의 안정적인 중계 서버입니다**

- `Axum` 기반 HTTP/WebSocket 서버
- `/api/tools/execute` 같은 Tool Execution API 제공
- Web UI 요청을 Unity TCP Bridge로 전달
- 터미널 세션, 인증 토큰, idle timeout 같은 런타임 관리
- AI CLI 실행 환경과 Unity 자동화 계층 사이의 경계 형성

Gateway가 있으면 브라우저와 Unity가 직접 강하게 결합되지 않습니다.


---


# Web UI와 AI Tools 계층

| 계층 | 역할 |
|---|---|
| Web UI | React SPA, xterm 터미널, ReactFlow 파이프라인, 원격 제어 화면 제공 |
| Claude Code | 긴 맥락 기반 설계 검토와 리팩터링에 강점 |
| OpenAI Codex | 코드 생성, 명령 실행, 테스트 수정 루프에 강점 |
| OpenCode | 에이전트형 멀티 파일 작업과 검증 흐름에 강점 |

LUX는 특정 AI 하나를 고정하지 않고, 작업에 맞는 도구를 선택하게 합니다.


---


# 데이터 흐름

```text
[User]
  -> [Web UI Terminal]
  -> [AI Tool Command]
  -> [Rust Gateway /api/tools/execute]
  -> [Unity AI Bridge TCP Server]
  -> [LuxAIToolDispatcher]
  -> [Unity API Execution]
  -> [Result: logs, screenshot, test report, JSON]
```

중요한 점은 결과가 다시 AI와 사람에게 돌아와 다음 판단의 컨텍스트가 된다는 것입니다.


---


# 통신 프로토콜

| 구간 | 프로토콜 | 이유 |
|---|---|---|
| Unity ↔ AI/Gateway | **TCP** | Editor 내부 서버와 외부 프로세스 간 단순한 명령 전달 |
| Gateway ↔ Web UI | **HTTP** | 명령 실행, 상태 조회, JSON 저장 같은 요청/응답 처리 |
| Gateway ↔ Web UI | **WebSocket** | 로그 스트리밍, 터미널 입출력, 상태 갱신 |
| Browser ↔ Unity Stream | **WebRTC** | 낮은 지연의 영상/입력 전달 |

프로토콜을 나누면 각 구간의 책임과 실패 지점을 분리할 수 있습니다.


---


<!-- _class: lead -->

# Part 3: Multi-AI Terminal

## 브라우저에서 여러 AI CLI를 전환하며 쓰는 이유


---


# Multi-AI Terminal의 개념

**Multi-AI Terminal은 Web UI 안에서 AI CLI를 실행하고 전환하는 xterm 기반 터미널입니다**

- 브라우저에서 `Claude Code`, `Codex`, `OpenCode` 세션 접근
- Unity 상태와 Tool Execution API를 같은 화면에서 사용
- 터미널 출력, 명령 히스토리, 세션 컨텍스트 유지
- Unity Editor를 보면서 AI 명령을 바로 실행

핵심은 “웹 터미널”이 아니라 **Unity 자동화와 AI 터미널을 같은 작업면에 놓는 것**입니다.


---


# 왜 여러 AI를 터미널 안에서 쓰는가

| 작업 | 적합한 AI 성격 |
|---|---|
| 긴 설계 검토 | 맥락 유지가 강한 모델 |
| 빠른 코드 수정 | 명령 실행과 diff 생성이 빠른 모델 |
| 실패 로그 분석 | 원인 추적과 재현 단계 정리가 강한 모델 |
| 대규모 리팩터링 | 계획, 하위 작업 분해, 검증 루프가 강한 에이전트 |

하나의 AI가 모든 작업에서 항상 최고일 필요는 없습니다. 전환 비용을 낮추는 것이 중요합니다.


---


# 실제 사용 흐름

1. `lux serve --token <TOKEN>`으로 Gateway 실행
2. 브라우저에서 LUX Web UI 접속
3. Multi-AI Terminal 패널 열기
4. 사용할 AI 도구 선택: `Claude Code`, `Codex`, `OpenCode`
5. Unity 관련 명령 또는 일반 개발 명령 실행

```bash
codex "Run Unity compile through LUX and summarize errors"
```

터미널은 Unity 밖에 있지만, 명령 결과는 Unity 안에서 만들어집니다.


---


# 세션 지속성과 명령 히스토리

**세션이 유지되면 AI와 사람 모두 반복 맥락을 잃지 않습니다**

- 이전 명령과 출력이 남아 원인 추적이 쉬움
- 같은 Unity 프로젝트 루트에서 계속 작업 가능
- `git status`, 테스트 결과, 로그를 흐름 안에서 비교 가능
- 장기 작업에서 “어디까지 했는지”를 터미널 자체가 기록

AI 작업은 한 번의 답변보다 여러 번의 관찰과 수정으로 완성됩니다.


---


<!-- _class: lead -->

# Part 4: Skill Dispatch & Tool Execution

## AI가 Unity를 제어하는 명령 단위


---


# 스킬이란?

**스킬은 AI가 Unity에게 요청할 수 있는 제한된 명령 단위입니다**

| 관점 | 설명 |
|---|---|
| 입력 | 스킬 이름과 JSON 파라미터 |
| 실행 | `LuxAIToolDispatcher.cs`가 Unity API 호출 |
| 출력 | 성공/실패, 로그, 파일 경로, JSON 결과 |
| 장점 | AI가 임의 행동이 아니라 등록된 작업만 실행 |

스킬은 Unity 자동화의 API이자, AI에게 제공하는 안전한 행동 목록입니다.


---


# Core Skills: compile / test

| 스킬 | 역할 | 왜 중요한가 |
|---|---|---|
| `compile` | Unity 프로젝트 컴파일 상태 확인 | 코드 수정 후 가장 작은 신뢰 단위 |
| `test` | Edit Mode / Play Mode 테스트 실행 | 사람이 확인한 느낌이 아니라 재실행 가능한 검증 제공 |

```bash
lux tools execute compile
lux tools execute test --mode edit
lux tools execute test --mode play
```

AI가 “맞아 보인다”가 아니라 실제 Unity 결과를 기준으로 수정하게 만듭니다.


---


# Core Skills: screenshot / logs / playmode

| 스킬 | 역할 | AI에게 유용한 이유 |
|---|---|---|
| `screenshot` | Editor/Game View 캡처 | UI 깨짐, Scene 배치, Play 결과 확인 |
| `logs` | Console 로그 스트리밍 | 오류, 경고, 런타임 메시지 원본 전달 |
| `playmode` | Play, Pause, Stop, Step 제어 | 실행 상태를 반복적으로 검증 |

```bash
lux tools execute screenshot --view game
lux tools execute logs --follow
lux tools execute playmode --state play
```


---


# Core Skills: execute-code / find-objects

**동적 조회와 Scene 검색은 Unity 상태를 추측하지 않게 만듭니다**

```csharp
using UnityEngine;

public static class LuxQuery
{
    public static string Run()
    {
        int count = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;
        return $"Objects: {count}";
    }
}
```

- `execute-code`: 제한된 C# 코드를 Editor 컨텍스트에서 실행
- `find-objects`: 이름, 태그, 컴포넌트 기준으로 GameObject 검색


---


# Core Skills: input / scene-smoke-test

| 스킬 | 설명 |
|---|---|
| `input-record` | 키보드, 마우스, 터치 입력 시퀀스 기록 |
| `input-playback` | 기록된 입력을 같은 타이밍으로 재생 |
| `scene-smoke-test` | 씬 로드, Play 진입, 오류/예외/스크린샷 확인 |

수동 재현 버그는 AI에게 설명하기 어렵습니다. 입력 시퀀스와 스모크 테스트가 있으면 같은 조건을 반복 실행할 수 있습니다.


---


# AI Tool Dispatcher의 역할

**`LuxAIToolDispatcher.cs`는 스킬 이름을 실제 Unity 작업으로 연결하는 라우터입니다**

```text
/api/tools/execute
        |
        v
AI Bridge TCP Server
        |
        v
LuxAIToolDispatcher
        |
        +-- compile
        +-- test
        +-- screenshot
        +-- logs
        +-- playmode
        +-- execute-code
```

중앙 Dispatcher는 확장과 제한을 동시에 쉽게 만듭니다.


---


# Tool Execution API와 스킬 확장

```bash
curl -X POST http://localhost:31337/api/tools/execute   -H "x-lux-token: $LUX_TOKEN"   -H "Content-Type: application/json"   -d '{"skill":"compile","args":{}}'
```

새 스킬을 만들 때는 네 가지를 함께 정합니다.

- 스킬 이름과 입력 스키마
- Unity API 실행 위치와 권한
- 성공/실패 출력 형식
- 감사 로그와 승인 필요 여부


---


<!-- _class: lead -->

# Part 5: ReactFlow 파이프라인 에디터

## 이미지 생성과 후처리를 노드 그래프로 조립하기


---


# 6개 노드 타입

| 노드 타입 | 역할 |
|---|---|
| `UnityContext` | 프로젝트 메타데이터와 Unity 환경 정보 제공 |
| `OutputDirectory` | 결과 파일 저장 경로 설정 |
| `PromptTemplate` | 재사용 가능한 프롬프트 템플릿 작성 |
| `CodexGeneration` | 이미지 생성 요청 실행 |
| `Segmentation` | 생성 이미지를 영역/객체 단위로 분할 |
| `MaskPostProcessing` | 마스크 정리, 확장, 노이즈 제거 |


---


# Context, Output, Prompt

| 노드 | 핵심 데이터 | 왜 필요한가 |
|---|---|---|
| `UnityContext` | 프로젝트 이름, platform, sprite settings, pixels per unit | 생성물이 Unity import 규칙에 맞도록 함 |
| `OutputDirectory` | `Assets/Generated/...` 같은 저장 경로 | 결과가 프로젝트 안에서 추적되도록 함 |
| `PromptTemplate` | 스타일, 시점, 제약 조건 | 즉흥 문장이 아니라 제작 규격으로 관리 |

좋은 파이프라인은 같은 입력에서 같은 구조의 산출물을 만듭니다.


---


# Generation, Segmentation, Mask

**생성 이미지를 Unity 애셋으로 쓰려면 분리와 정리가 필요합니다**

- `CodexGeneration`: 프롬프트와 Unity 컨텍스트를 받아 이미지 생성
- `Segmentation`: 캐릭터, 장비, 배경, 그림자 영역 분리
- `MaskPostProcessing`: 구멍 메우기, 가장자리 정리, 작은 노이즈 제거

이 단계가 없으면 이미지는 예쁘지만 Sprite, Rig, Hitbox, Animation 작업에 쓰기 어렵습니다.


---


# 2D 애셋 생성 워크플로우

```text
UnityContext
    |
    v
PromptTemplate ---> CodexGeneration
    |                    |
    v                    v
OutputDirectory ---> Segmentation
                         |
                         v
                 MaskPostProcessing
                         |
                         v
                Unity Asset Export
```

노드 그래프는 “한 번 생성”보다 “좋은 절차를 저장해 반복”하는 데 가치가 있습니다.


---


# 편집과 저장

**파이프라인도 코드처럼 실험과 복구가 필요합니다**

- Undo/Redo 40단계로 구조 변경을 안전하게 시도
- 노드 복사/붙여넣기로 유사 파이프라인 빠르게 구성
- JSON 저장/로드로 버전 관리와 템플릿 공유 가능
- 실행 결과를 기록해 어떤 프롬프트와 옵션이 산출물을 만들었는지 추적

시각 편집기의 품질은 “실수했을 때 돌아갈 수 있는가”에서 드러납니다.


---


<!-- _class: lead -->

# Part 6: Codex Image

## 생성 이미지를 Unity 2D 애셋으로 바꾸는 흐름


---


# 노드 기반 이미지 생성 파이프라인

**Codex Image 흐름은 프롬프트와 Unity 애셋 변환을 분리하지 않습니다**

1. Unity 프로젝트 컨텍스트 읽기
2. 프롬프트 템플릿 작성
3. 이미지 생성
4. 분할과 마스크 후처리
5. Unity용 Sprite, Sprite Sheet, Rig 데이터로 변환

이미지 생성의 목표는 “그럴듯한 그림”이 아니라 **프로젝트에 들어갈 수 있는 애셋**입니다.


---


# Unity 2D Animation과 Spine 지원

| 출력 흐름 | 설명 |
|---|---|
| Unity 2D Animation | Sprite, Bone, Weight 정보를 다루기 쉽게 파츠와 레이어 정리 |
| Spine 4.2 | attachment 후보, 슬롯 이름, 본 구조 초안을 생성 규칙으로 맞춤 |
| Unity import | Sprite Library, PSD Importer, runtime import 흐름을 고려 |

AI가 리깅을 완전히 대체하기보다 반복적인 파츠 정리와 초안 생성을 줄입니다.


---


# Sprite Sheet 익스포터

| 출력 | 목적 |
|---|---|
| PNG sheet | 프레임 이미지를 하나의 텍스처로 저장 |
| JSON metadata | 프레임 좌표, pivot, duration 기록 |
| Unity import hint | Sprite Mode, pixels per unit, filter mode 안내 |

Sprite Sheet는 수업에서 빠르게 결과를 확인하기 좋은 포맷입니다.


---


# 실제 워크플로우와 판단 기준

```text
Prompt -> Image Generation -> Segmentation -> Mask Cleanup
       -> Sprite Sheet Export -> Unity Import -> Play Mode Preview
```

- 컨셉 탐색에는 빠른 seed 반복이 중요
- 프로젝트 투입에는 레이어, pivot, 해상도, import setting이 중요
- 팀 작업에서는 프롬프트와 생성 옵션이 기록되어야 함

생성 AI의 결과물은 완성품보다 **제작 루프를 빠르게 하는 재료**로 보는 것이 안전합니다.


---


<!-- _class: lead -->

# Part 7: WebRTC 원격 제어

## Unity 화면을 브라우저로 보내고 입력을 되돌려 보내기


---


# 원격 제어의 목표

**WebRTC 원격 제어는 Unity Editor 또는 Game View를 브라우저에서 조작하게 합니다**

- Unity 카메라 또는 Game View 캡처
- 브라우저로 저지연 영상 스트리밍
- 마우스, 키보드, 터치 입력을 Unity로 전달
- 원격 리뷰, 강의, 협업 디버깅에 활용

단순 화면 공유와 다른 점은 입력이 LUX 제어 루프 안으로 들어온다는 것입니다.


---


# 스트리밍과 입력 흐름

```text
[Unity Camera/Game View]
          |
          v
[Frame Capture] -> [WebRTC PeerConnection] -> [Browser Video]
                                                     |
                                                     v
[Unity Input Adapter] <- mouse / keyboard / touch <- [Browser Input]
```

브라우저 입력은 Game View 포인터 입력, 키보드 입력, 모바일 터치 시뮬레이션으로 변환됩니다.


---


# Signaling Relay, Token, ICE

| 요소 | 역할 |
|---|---|
| Signaling Relay | offer/answer SDP와 ICE candidate 교환 |
| `x-lux-token` | 허가된 브라우저와 CLI만 API 접근 허용 |
| STUN | NAT 뒤의 Peer가 공인 경로를 찾도록 도움 |
| TURN | 직접 연결이 실패할 때 중계 경로 제공 |

```bash
curl -H "x-lux-token: $LUX_TOKEN" http://localhost:31337/api/status
```

원격 제어는 기능만큼 인증과 세션 범위가 중요합니다.


---


# 원격 협업 시나리오

- 강사가 Unity Editor를 브라우저로 스트리밍하며 학생에게 설명
- 팀원이 특정 씬 버그를 원격으로 재현하고 입력 기록을 남김
- AI가 `scene-smoke-test`를 실행하고, 사람은 브라우저로 화면 확인
- 외부 리뷰어는 Git checkout 없이 실행 화면과 로그만 확인

원격 제어는 배포 빌드가 아니라 Editor 중심 개발 상황에서 특히 유용합니다.


---


<!-- _class: lead -->

# Part 8: Unity Git 통합

## 에디터 안에서 변경 상태를 읽고 AI 작업 단위를 관리하기


---


# Editor 내 Git 기능

| 기능 | 설명 |
|---|---|
| Status | 변경, 삭제, untracked 파일 확인 |
| Staging | 코드와 `.meta` 파일을 함께 묶어 staging |
| History Graph | 브랜치와 커밋 흐름 시각화 |
| Branch/Remote | 브랜치 전환, fetch/pull 상태 확인 |
| Submodule | 하위 모듈 초기화와 업데이트 상태 표시 |

Unity는 에셋과 메타파일이 함께 움직이므로 Editor에서 Git 상태를 보는 것이 중요합니다.


---


# AI와 Git의 연계

**AI 작업의 안전한 경계는 Git diff입니다**

```bash
git status
git diff
lux tools execute compile
git add Assets/Scripts/PlayerController.cs
git commit -m "Fix player jump buffering"
```

LUX가 Git 상태를 보여주면 AI는 구현 후 변경 범위, 테스트 결과, 커밋 준비 상태를 하나의 루프로 정리할 수 있습니다.


---


<!-- _class: lead -->

# Part 9: AI Bridge & 동적 코드 실행

## Unity 내부 상태를 안전하게 읽고 조작하는 방법


---


# AI Bridge TCP Server의 역할

**AI Bridge는 외부 명령이 Unity Editor 안으로 들어오는 입구입니다**

- TCP 연결을 수신하고 요청 메시지를 파싱
- Dispatcher로 스킬 실행을 위임
- 실행 결과를 JSON 또는 텍스트로 반환
- Unity 메인 스레드가 필요한 작업을 Editor 흐름에 맞게 예약

Unity API는 일반 외부 프로세스에서 직접 호출할 수 없으므로 Bridge가 경계를 담당합니다.


---


# 동적 C# 코드 실행

```csharp
using UnityEngine;

public static class LuxSceneCheck
{
    public static string Run()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int count = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;
        return $"Scene={scene}, GameObjects={count}";
    }
}
```

`execute-code`는 빠른 상태 조회에 유용하지만, 파일 삭제나 외부 프로세스 실행 같은 위험 행동은 제한해야 합니다.


---


# Automation Guardrails

| Guardrail | 목적 |
|---|---|
| 명령 블랙리스트 | 위험한 shell/file/system 작업 차단 |
| 감사 로그 | 누가 어떤 스킬을 언제 실행했는지 기록 |
| 승인 상태 | 고위험 작업 전 사람의 확인 요구 |
| 입력 기록/재생 | 자동화 테스트와 버그 재현을 데이터화 |

AI 자동화의 목표는 무제한 권한이 아니라, 설명 가능한 권한 위임입니다.


---


<!-- _class: lead -->

# Part 10: 설치 및 설정

## Unity, Gateway, Web UI를 연결하는 기본 절차


---


# 전제 조건과 Unity 패키지

| 항목 | 권장 조건 |
|---|---|
| Unity | Unity 6.x, `Unity 6000.0+` |
| Rust | `rustup`, `cargo` 사용 가능 |
| Node.js | Web UI 빌드용 LTS 버전 |
| AI CLI | `claude`, `codex`, `opencode` 중 사용할 도구 설치 |
| Git | Unity 프로젝트 루트에서 사용 가능 |

Unity 패키지 설치 후 `Lux Workbench`, AI Bridge TCP Server, core skill 목록을 먼저 확인합니다.


---


# Gateway와 Web UI 빌드

```bash
# Rust Gateway CLI
cargo build --release
./target/release/lux status

# Web UI
npm install
npm run build
```

Gateway는 Unity Editor와 Web UI 사이의 서버이므로, Unity 프로젝트를 열어둔 상태에서 상태 확인을 진행하는 것이 좋습니다.


---


# 서버 실행과 상태 확인

```bash
lux serve --token <TOKEN> --host 127.0.0.1 --port 31337 --idle-timeout 3600
lux status

curl -H "x-lux-token: $LUX_TOKEN"   http://127.0.0.1:31337/api/status
```

토큰은 저장소에 커밋하지 않습니다. 확인할 항목은 Gateway 실행, Unity Bridge 연결, 인증 요청 성공입니다.


---


<!-- _class: lead -->

# Part 11: 실전 워크플로우

## 하루 개발 루프 안에서 LUX를 사용하는 방법


---


# 일일 개발 워크플로우

```text
1. Unity 프로젝트 열기
2. lux serve 실행
3. Multi-AI Terminal에서 작업 지시
4. AI가 코드 수정
5. compile/test/scene-smoke-test 실행
6. screenshot/logs로 결과 확인
7. git diff 검토
8. 필요한 경우 commit 또는 다음 수정 루프
```

AI 수정 뒤 사람이 감으로 확인하지 않고, 스킬 결과를 기준으로 다음 결정을 내립니다.


---


# 이미지 생성과 디버깅 워크플로우

| 워크플로우 | 단계 |
|---|---|
| 이미지 생성 | `UnityContext` → `PromptTemplate` → `CodexGeneration` → `Segmentation` → export |
| 디버깅 | `logs --follow` → `scene-smoke-test` → `screenshot` → AI 원인 분석 |

```bash
lux tools execute logs --follow
lux tools execute scene-smoke-test --scene Assets/Scenes/BossRoom.unity
lux tools execute screenshot --view game
```

디버깅에서 가장 나쁜 입력은 사람이 요약한 부정확한 증상입니다.


---


# 원격 협업 워크플로우

1. 강사 또는 리드 개발자가 Unity와 Gateway 실행
2. 브라우저 접속 URL과 토큰을 제한된 대상에게 공유
3. WebRTC 화면으로 현재 Game View 확인
4. 필요한 입력을 원격으로 재현
5. 로그, 스크린샷, 입력 기록을 작업 이슈에 첨부

수업에서는 처음에 `compile`, `logs`, `screenshot` 세 스킬만 사용하고 점진적으로 확장하는 것이 좋습니다.


---


<!-- _class: lead -->

# Part 12: 마무리

## LUX가 바꾸는 Unity 개발


---

# LUX가 바꾸는 것과 참고 자료

**LUX는 AI를 Editor 밖의 조언자에서 Unity 상태를 읽고 검증하는 실행 파트너로 옮깁니다**

| 이전 | 이후 |
|---|---|
| 사람이 로그/화면을 복사 | LUX가 결과를 구조화해 반환 |
| 생성 이미지는 별도 산출물 | 파이프라인을 통해 Unity 애셋으로 변환 |
| 원격 협업은 화면 공유 중심 | 스트리밍, 입력, 자동화 결과가 연결 |

추천 순서: `lux status` → `compile` → Multi-AI Terminal → `logs/screenshot` → Pipeline → WebRTC/Git.

참고: `https://github.com/linalab/lux`, Unity 6.x Manual, Axum, ReactFlow, WebRTC.
