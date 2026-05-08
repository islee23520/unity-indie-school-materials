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
3. 설치, CLI 명령어, Core Skills, 실전 워크플로우를 실전으로 연결

> 관점은 기능 암기가 아니라 "AI가 Unity 상태를 어떻게 보고 검증하는가"입니다.


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

**LUX(Linalab Unity X)**는 Unity Editor를 AI 작업 허브로 만드는 **unified AI adapter + automation toolkit**입니다.

- Unity Editor 상태를 AI 도구가 읽고 조작할 수 있게 연결
- 여러 AI CLI를 웹 기반 터미널에서 전환하며 사용
- 컴파일, 테스트, 스크린샷, 로그, Play Mode를 스킬로 실행
- 이미지 생성, 원격 제어, Git, 자동화 테스트를 하나의 개발 루프로 통합


---


# Linalab, 라이선스, 대상 환경

| 항목 | 내용 |
|---|---|
| 회사 | **Linalab(린랩)** |
| 프로젝트 | **LUX: Linalab Unity X** |
| 리포지토리 | `github.com/Linalab-io/Lux` |
| 라이선스 | **MIT Open Source** |
| Unity 패키지 | `com.linalab.lux` |
| 대상 엔진 | **Unity 6.x**, `Unity 6000.0+` |
| 초점 | Unity Editor 자동화, AI 도구 연결, 제작 파이프라인 통합 |


---


# 기존 방식 vs LUX 방식

| 관점 | 기존 방식 | LUX 방식 |
|---|---|---|
| AI 실행 위치 | 터미널/웹/IDE에 분산 | Web UI와 Unity Bridge로 통합 |
| Unity 상태 확인 | 사람이 Console/Scene을 읽음 | 스킬이 로그, 오브젝트, 스크린샷 수집 |
| 검증 | 수동 Play/Test | `lux compile`, `lux run-tests` 실행 |
| 반복 | 복사/붙여넣기 | 명령 → 실행 → JSON 결과 반환 루프 |

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
|  React 19 SPA + xterm     |        | Claude Code / Codex       |
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


# 코어 모듈 구성

| 모듈 | 설명 | 기술 |
|---|---|---|
| **LuxEditor** | 메인 어댑터. Workbench, 자동화 게이트웨이, WebRTC 프로듀서 | C# |
| **AiBridgeEditor** | AI 도구와 통신하는 TCP 서버와 프로토콜 핸들러 | C# |
| **UnityGitEditor** | Editor 안에서 Git 상태, staging, 브랜치 관리 | C# |
| **CodexImage** | 노드 기반 이미지 생성 파이프라인 엔진 | C# |
| **RustGateway** | Axum 기반 웹 서버와 CLI. Web UI와 REST API 제공 | Rust |
| **McpHelper** | Node.js MCP 헬퍼 | Node.js 22+ |
| **Skills** | Unity 제어용 코어 스킬 셋과 참고 문서 | Manifest + SKILL.md |


---


# Unity Editor 계층

**Unity Editor는 실제 프로젝트 상태와 실행 권한을 가진 계층입니다**

- `Lux Workbench`: Editor 안에서 LUX 기능을 탐색하고 실행하는 작업대
- `AI Bridge TCP Server`: 외부 명령을 Unity 안으로 받아들이는 통신 지점
- `LuxAIToolDispatcher`: 스킬 이름을 실제 Unity 작업으로 라우팅
- Unity API 접근: Scene, GameObject, Console, Test Runner, Play Mode, AssetDatabase

AI가 Unity API를 직접 호출할 수 없기 때문에 Bridge와 Dispatcher가 필요합니다.


---


# Rust Gateway CLI 계층

**Gateway는 Unity, Web UI, AI Tools 사이의 안정적인 중계 서버입니다**

- `Axum 0.7` 기반 HTTP/WebSocket 서버
- `/api/tools/execute` 같은 Tool Execution API 제공
- Web UI 요청을 Unity TCP Bridge로 전달
- `--idle-timeout` 기반 자동 종료와 heartbeat 유지
- AI CLI 실행 환경과 Unity 자동화 계층 사이의 경계 형성

```bash
lux serve --port 8080 --idle-timeout 60
```


---


# Web UI와 AI Tools 계층

| 계층 | 역할 |
|---|---|
| Web UI | React 19 SPA, xterm 터미널, ReactFlow 파이프라인, 원격 제어 화면 제공 |
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
  -> [Result: JSON with logs, screenshot path, test report]
```

중요한 점은 모든 명령이 **JSON 형식**으로 반환되어 AI와 사람 모두 기계적으로 파싱할 수 있다는 것입니다.


---


# 통신 프로토콜

| 구간 | 프로토콜 | 이유 |
|---|---|---|
| Unity ↔ Gateway | **TCP** | Editor 내부 서버와 외부 프로세스 간 명령 전달 |
| Gateway ↔ Web UI | **HTTP** | 명령 실행, 상태 조회, JSON 저장 같은 요청/응답 |
| Gateway ↔ Web UI | **WebSocket** | 로그 스트리밍, 터미널 입출력, 상태 갱신 |
| Browser ↔ Unity Stream | **WebRTC** | 낮은 지연의 영상/입력 전달 (`com.unity.webrtc`) |


---


<!-- _class: lead -->

# Part 3: CLI Reference

## `lux` 명령어로 Unity를 제어하는 방법


---


# `lux` CLI 기본 명령

| 명령 | 설명 |
|---|---|
| `lux serve` | 웹 서버와 게이트웨이 시작 |
| `lux compile` | Unity 프로젝트 컴파일 실행 |
| `lux run-tests` | PlayMode / EditMode 테스트 실행 |
| `lux unity context` | Unity 프로젝트 메타데이터 조회 |
| `lux status` | 서버 상태 확인 |

```bash
# 서버 실행 (기본 30분 idle timeout)
lux serve --port 8080

# 컴파일
lux compile

# 테스트
lux run-tests --test-platform EditMode
lux run-tests --test-platform PlayMode
```


---


# 서버 생명주기

```bash
# 기본 실행 (30분 비활동 시 자동 종료)
lux serve --port 8080

# idle timeout 변경 (0 = 비활성화)
lux serve --port 8080 --idle-timeout 60

# heartbeat 유지
POST /api/heartbeat

# 상태 확인
GET /api/health
```

서버는 Unity Editor가 활성 상태인 동안 유지됩니다. Editor 종료 시 자동으로 함께 종료됩니다.


---


# 서버 연결 확인

1. **Unity Editor 열기**: `Window > Linalab > Lux Workbench`
2. **서버 시작**: 터미널에서 `lux serve --port 8080`
3. **Web UI 접속**: 브라우저에서 `http://localhost:8080`
4. **연결 확인**: `Tools > Linalab > Lux > Server Status` 창에서 상태 확인

| 상태 | 의미 |
|---|---|
| 초록 | 서버 연결됨 |
| 노랑 | 서버 미실행 |
| 빨강 | 오류 발생 |


---


<!-- _class: lead -->

# Part 4: Multi-AI Terminal

## 브라우저에서 여러 AI CLI를 전환하며 쓰는 이유


---


# Multi-AI Terminal의 개념

**Multi-AI Terminal은 Web UI 안에서 AI CLI를 실행하고 전환하는 xterm 기반 터미널입니다**

- 브라우저에서 `Claude Code`, `Codex`, `OpenCode` 세션 접근
- Unity 상태와 Tool Execution API를 같은 화면에서 사용
- 터미널 출력, 명령 히스토리, 세션 컨텍스트 유지
- Unity Editor를 보면서 AI 명령을 바로 실행

핵심은 "웹 터미널"이 아니라 **Unity 자동화와 AI 터미널을 같은 작업면에 놓는 것**입니다.


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

1. `lux serve --port 8080`으로 Gateway 실행
2. 브라우저에서 LUX Web UI 접속
3. Multi-AI Terminal 패널 열기
4. 사용할 AI 도구 선택: `Claude Code`, `Codex`, `OpenCode`
5. Unity 관련 명령 또는 일반 개발 명령 실행

터미널은 Unity 밖에 있지만, 명령 결과는 Unity 안에서 만들어집니다.


---


<!-- _class: lead -->

# Part 5: Skill Dispatch & Core Skills

## AI가 Unity를 제어하는 명령 단위


---


# 스킬이란?

**스킬은 AI가 Unity에게 요청할 수 있는 제한된 명령 단위입니다**

| 관점 | 설명 |
|---|---|
| 입력 | 스킬 이름과 JSON 파라미터 |
| 실행 | `LuxAIToolDispatcher`가 Unity API 호출 |
| 출력 | JSON 형식의 성공/실패, 로그, 파일 경로 |
| 장점 | AI가 임의 행동이 아니라 등록된 작업만 실행 |

> **모든 명령은 JSON을 반환합니다.** 이것이 AI가 결과를 기계적으로 해석할 수 있는 핵심입니다.

스킬은 Unity 자동화의 API이자, AI에게 제공하는 안전한 행동 목록입니다.


---


# Core Skills 전체 목록

| 카테고리 | 스킬 | 설명 |
|---|---|---|
| **빌드/테스트** | `compile` | Unity 프로젝트 컴파일 |
| | `run-tests` | EditMode / PlayMode 테스트 실행 |
| **상태 조회** | `unity context` | 프로젝트 메타데이터 조회 (`--refresh`는 batch mode) |
| | `get-logs` | Console 로그 스트리밍 |
| | `find-game-objects` | 이름, 태그, 컴포넌트 기준 GameObject 검색 |
| | `get-hierarchy` | 씬 계층 구조 조회 |
| **시각** | `screenshot` | Editor/Game View 캡처, 어노테이션 지원 |
| | `annotations` | 스크린샷에 메모/마크 추가 |
| **입력/제어** | `play-mode` | Play, Pause, Stop 전환 |
| | `mouse` / `keyboard` | PlayMode 마우스, 키보드 입력 시뮬레이션 |
| **동적 실행** | `dynamic-code` | 제한된 C# 코드를 Editor 컨텍스트에서 실행 |
| **입력 기록** | `record` / `replay` | Input System 기반 입력 시퀀스 기록과 재생 |


---


# 빌드/테스트 스킬

```bash
# 컴파일
lux compile

# EditMode 테스트
lux run-tests --test-platform EditMode

# PlayMode 테스트
lux run-tests --test-platform PlayMode
```

| 스킬 | 역할 | 왜 중요한가 |
|---|---|---|
| `compile` | Unity 프로젝트 컴파일 상태 확인 | 코드 수정 후 가장 작은 신뢰 단위 |
| `run-tests` | EditMode / PlayMode 테스트 실행 | 사람이 확인한 느낌이 아니라 재실행 가능한 검증 제공 |


---


# 상태 조회 스킬

```bash
# Unity 컨텍스트 조회
lux unity context

# batch mode로 새로고침 (Editor를 잠시 재시작)
lux unity context --refresh

# Console 로그
lux get-logs

# GameObject 검색
lux find-game-objects --name "Player"
lux find-game-objects --tag "Enemy"

# 씬 계층
lux get-hierarchy
```

AI가 Unity 상태를 추측하지 않고 실제로 조회하게 만듭니다.


---


# 시각/입력/동적 스킬

```bash
# 스크린샷
lux screenshot --view game

# PlayMode 제어
lux play-mode --state play
lux play-mode --state stop

# 동적 C# 실행
lux dynamic-code
```

```csharp
// dynamic-code 예시
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


---


# 입력 기록과 재생

| 스킬 | 설명 |
|---|---|
| `record` | 키보드, 마우스, 터치 입력 시퀀스 기록 (Input System 기반) |
| `replay` | 기록된 입력을 같은 타이밍으로 재생 |

수동 재현 버그는 AI에게 설명하기 어렵습니다. 입력 시퀀스 기록이 있으면 같은 조건을 반복 실행할 수 있습니다.


---


# AI Tool Dispatcher의 역할

**`LuxAIToolDispatcher`는 스킬 이름을 실제 Unity 작업으로 연결하는 라우터입니다**

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
        +-- run-tests
        +-- screenshot
        +-- get-logs
        +-- find-game-objects
        +-- dynamic-code
```

중앙 Dispatcher는 확장과 제한을 동시에 쉽게 만듭니다.


---


# Tool Execution API

```bash
curl -X POST http://localhost:8080/api/tools/execute \
  -H "x-lux-token: $LUX_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"skill":"compile","args":{}}'
```

WebSocket을 통해 실행 이벤트를 연결된 모든 클라이언트에 브로드캐스팅합니다.

새 스킬을 만들 때는 네 가지를 함께 정합니다.
- 스킬 이름과 입력 스키마
- Unity API 실행 위치와 권한
- JSON 출력 형식
- 감사 로그와 승인 필요 여부


---


<!-- _class: lead -->

# Part 6: ReactFlow 파이프라인 에디터

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

노드 그래프는 "한 번 생성"보다 "좋은 절차를 저장해 반복"하는 데 가치가 있습니다.


---


# 편집과 저장

**파이프라인도 코드처럼 실험과 복구가 필요합니다**

- Undo/Redo 40단계로 구조 변경을 안전하게 시도
- 노드 복사/붙여넣기로 유사 파이프라인 빠르게 구성
- JSON 저장/로드로 버전 관리와 템플릿 공유 가능 (`/api/graphs`)
- 실행 결과를 기록해 어떤 프롬프트와 옵션이 산출물을 만들었는지 추적


---


<!-- _class: lead -->

# Part 7: Codex Image

## 생성 이미지를 Unity 2D 애셋으로 바꾸는 흐름


---


# 노드 기반 이미지 생성 파이프라인

**Codex Image 흐름은 프롬프트와 Unity 애셋 변환을 분리하지 않습니다**

1. Unity 프로젝트 컨텍스트 읽기
2. 프롬프트 템플릿 작성
3. 이미지 생성
4. 분할과 마스크 후처리
5. Unity용 Sprite, Sprite Sheet, Rig 데이터로 변환

이미지 생성의 목표는 "그럴듯한 그림"이 아니라 **프로젝트에 들어갈 수 있는 애셋**입니다.


---


# Unity 2D Animation과 Spine 지원

| 출력 흐름 | 설명 |
|---|---|
| Unity 2D Animation | Sprite, Bone, Weight 정보를 다루기 쉽게 파츠와 레이어 정리 |
| Spine | attachment 후보, 슬롯 이름, 본 구조 초안을 생성 규칙으로 맞춤 |
| Unity import | Sprite Library, PSD Importer, runtime import 흐름을 고려 |


---


# Sprite Sheet 익스포터

| 출력 | 목적 |
|---|---|
| PNG sheet | 프레임 이미지를 하나의 텍스처로 저장 |
| JSON metadata | 프레임 좌표, pivot, duration 기록 |
| Unity import hint | Sprite Mode, pixels per unit, filter mode 안내 |


---


<!-- _class: lead -->

# Part 8: WebRTC 원격 제어

## Unity 화면을 브라우저로 보내고 입력을 되돌려 보내기


---


# 원격 제어의 목표

**WebRTC 원격 제어는 Unity Editor 또는 Game View를 브라우저에서 조작하게 합니다**

- Unity 카메라 또는 Game View 캡처 (해상도/프레임레이트 설정 가능)
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
| Signaling Relay | Queue 기반 WebSocket 메시지 전달로 offer/answer SDP와 ICE candidate 교환 |
| `x-lux-token` | 허가된 브라우저와 CLI만 API 접근 허용 |
| STUN | NAT 뒤의 Peer가 공인 경로를 찾도록 도움 |
| TURN | 직접 연결이 실패할 때 중계 경로 제공 |

원격 제어는 기능만큼 인증과 세션 범위가 중요합니다.


---


# 원격 협업 시나리오

- 강사가 Unity Editor를 브라우저로 스트리밍하며 학생에게 설명
- 팀원이 특정 씬 버그를 원격으로 재현하고 입력 기록을 남김
- AI가 테스트를 실행하고, 사람은 브라우저로 화면 확인
- 외부 리뷰어는 Git checkout 없이 실행 화면과 로그만 확인


---


<!-- _class: lead -->

# Part 9: Unity Git 통합

## 에디터 안에서 변경 상태를 읽고 AI 작업 단위를 관리하기


---


# Editor 내 Git 기능

| 기능 | 설명 |
|---|---|
| Status | 변경, 삭제, untracked 파일 확인 |
| Staging | 코드와 `.meta` 파일을 함께 묶어 staging |
| History | 커밋 흐름 시각화 |
| Branch/Remote | 브랜치 전환, fetch/pull 상태 확인 |
| Submodule | 하위 모듈 초기화와 업데이트 상태 표시 |

Unity는 에셋과 메타파일이 함께 움직이므로 Editor에서 Git 상태를 보는 것이 중요합니다.


---


# AI와 Git의 연계

**AI 작업의 안전한 경계는 Git diff입니다**

```bash
git status
git diff
lux compile
git add Assets/Scripts/PlayerController.cs
git commit -m "Fix player jump buffering"
```

LUX가 Git 상태를 보여주면 AI는 구현 후 변경 범위, 테스트 결과, 커밋 준비 상태를 하나의 루프로 정리할 수 있습니다.


---


<!-- _class: lead -->

# Part 10: AI Bridge & 동적 코드 실행

## Unity 내부 상태를 안전하게 읽고 조작하는 방법


---


# AI Bridge TCP Server의 역할

**AI Bridge는 외부 명령이 Unity Editor 안으로 들어오는 입구입니다**

- TCP 연결을 수신하고 요청 메시지를 파싱
- Dispatcher로 스킬 실행을 위임
- 실행 결과를 JSON으로 반환
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

`dynamic-code`는 빠른 상태 조회에 유용하지만, 파일 삭제나 외부 프로세스 실행 같은 위험 행동은 제한해야 합니다.


---


# Automation Guardrails

| Guardrail | 목적 |
|---|---|
| Command blacklist | 위험한 shell/file/system 작업 차단 |
| Audit log | 누가 어떤 스킬을 언제 실행했는지 기록 |
| Approval state | 고위험 작업 전 사람의 확인 요구 |
| Input record/replay | 자동화 테스트와 버그 재현을 데이터화 |

AI 자동화의 목표는 무제한 권한이 아니라, 설명 가능한 권한 위임입니다.


---


<!-- _class: lead -->

# Part 11: 설치 및 설정

## Unity, Gateway, Web UI를 연결하는 기본 절차


---


# 전제 조건

| 항목 | 권장 조건 |
|---|---|
| Unity | Unity 6.x, `Unity 6000.0+` |
| Rust | `rustup`, `cargo` 사용 가능 |
| Node.js | MCP 헬퍼용 22+ |
| WebRTC | Unity 패키지 `com.unity.webrtc` 설치 필요 |
| AI CLI | `claude`, `codex`, `opencode` 중 사용할 도구 설치 |
| Git | Unity 프로젝트 루트에서 사용 가능 |


---


# Unity 패키지 설치

1. Unity 프로젝트의 `Packages/manifest.json`에 LUX 패키지 추가
2. `com.unity.webrtc` 패키지 설치 (원격 스트리밍 기능에 필요)

```json
{
  "dependencies": {
    "com.linalab.lux": "https://github.com/Linalab-io/Lux.git",
    "com.unity.webrtc": "3.0.1"
  }
}
```

설치 후 `Window > Linalab > Lux Workbench`에서 LUX 기능을 확인합니다.


---


# Rust CLI 빌드

```bash
cd Packages/com.linalab.lux/RustGateway~
cargo build --release

# PATH에 추가하거나 직접 실행
./target/release/lux --help
```

Gateway는 Unity Editor와 Web UI 사이의 서버이므로, Unity 프로젝트를 열어둔 상태에서 실행하는 것이 좋습니다.


---


# 서버 실행과 상태 확인

```bash
# 서버 실행 (기본 포트 8080)
lux serve --port 8080

# idle timeout 변경 (초 단위, 0 = 비활성화)
lux serve --port 8080 --idle-timeout 3600

# 상태 확인
GET /api/health
```

토큰은 저장소에 커밋하지 않습니다. 확인할 항목은 Gateway 실행, Unity Bridge 연결, 인증 요청 성공입니다.


---


<!-- _class: lead -->

# Part 12: 실전 워크플로우

## 하루 개발 루프 안에서 LUX를 사용하는 방법


---


# 일일 개발 워크플로우

```text
1. Unity 프로젝트 열기
2. lux serve 실행
3. Multi-AI Terminal에서 작업 지시
4. AI가 코드 수정
5. lux compile로 컴파일 확인
6. lux run-tests로 테스트 실행
7. lux screenshot으로 결과 확인
8. git diff 검토
9. 필요한 경우 commit 또는 다음 수정 루프
```

AI 수정 뒤 사람이 감으로 확인하지 않고, 스킬 결과를 기준으로 다음 결정을 내립니다.


---


# 이미지 생성 워크플로우

```text
UnityContext → PromptTemplate → CodexGeneration → Segmentation → export
```

| 단계 | 명령/노드 |
|---|---|
| 컨텍스트 설정 | `UnityContext` 노드 |
| 프롬프트 작성 | `PromptTemplate` 노드 |
| 이미지 생성 | `CodexGeneration` 노드 |
| 분할/정리 | `Segmentation` → `MaskPostProcessing` |
| Unity import | `OutputDirectory` → Sprite Sheet Export |

생성 AI의 결과물은 완성품보다 **제작 루프를 빠르게 하는 재료**로 보는 것이 안전합니다.


---


# 디버깅 워크플로우

```bash
# 로그 확인
lux get-logs

# 씬 스모크 테스트 (스크린샷 + 오류 확인)
lux screenshot --view game

# 입력 재현
lux replay --recording bug-reproduction.json
```

디버깅에서 가장 나쁜 입력은 사람이 요약한 부정확한 증상입니다.


---


# 원격 협업 워크플로우

1. 강사 또는 리드 개발자가 Unity와 Gateway 실행
2. 브라우저 접속 URL과 토큰을 제한된 대상에게 공유
3. WebRTC 화면으로 현재 Game View 확인
4. 필요한 입력을 원격으로 재현
5. 로그, 스크린샷, 입력 기록을 작업 이슈에 첨부

수업에서는 처음에 `lux compile`, `lux get-logs`, `lux screenshot` 세 스킬만 사용하고 점진적으로 확장하는 것이 좋습니다.


---


<!-- _class: lead -->

# Part 13: 마무리

## LUX가 바꾸는 Unity 개발


---


# LUX가 바꾸는 것

**LUX는 AI를 Editor 밖의 조언자에서 Unity 상태를 읽고 검증하는 실행 파트너로 옮깁니다**

| 이전 | 이후 |
|---|---|
| 사람이 로그/화면을 복사 | LUX가 JSON 결과를 구조화해 반환 |
| 생성 이미지는 별도 산출물 | 파이프라인을 통해 Unity 애셋으로 변환 |
| 원격 협업은 화면 공유 중심 | 스트리밍, 입력, 자동화 결과가 연결 |

> LUX는 **Unity Editor 쪽에서 AI 도구를 받아들이는 통합 계층**입니다.


---


# LUX 핵심 요약

- **Adapter + Automation Toolkit**: Unity Editor 안에서 AI 도구를 연결하는 통합 계층
- **Rust Gateway**: Axum 기반 웹 서버가 Unity, Web UI, AI Tools를 중계
- **Multi-AI Terminal**: Claude Code, Codex, OpenCode를 한 터미널에서 전환
- **Core Skills**: compile, run-tests, screenshot, get-logs, find-game-objects, dynamic-code 등 JSON 반환 명령
- **Pipeline Editor**: ReactFlow 기반 이미지 생성/후처리 노드 그래프
- **WebRTC**: 브라우저에서 Unity 화면 스트리밍과 입력 전달
- **Unity Git**: Editor 안에서 Git 상태 관리

추천 순서: `lux serve` → `lux compile` → Multi-AI Terminal → `lux get-logs` / `lux screenshot` → Pipeline → WebRTC/Git.


---


# 참고 자료

| 자료 | 경로 |
|---|---|
| GitHub 리포지토리 | `https://github.com/Linalab-io/Lux` |
| Developer Guide | `GUIDE.md` (영어 / 한국어 / 일본어) |
| Agent Guide | `AGENTS.md` |
| Core Skills | `Skills/lux-unity/SKILL.md` |
| Skill References | `Skills/lux-unity/references/*.md` |
