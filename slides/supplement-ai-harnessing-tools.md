---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# AI Harnessing Tools

## OMO · OMX · OMC · LUX

AI 코딩 에이전트를 더 강력하게 만드는 워크플로우 레이어

---

## 목차

1. AI 하네싱이란?
2. OMO: oh-my-openagent
3. OMX: oh-my-codex
4. OMC: oh-my-claudecode
5. LUX: Linalab Unity X
6. 비교 및 선택 가이드

---

<!-- _class: lead -->

# Part 1. AI 하네싱이란?

원본 AI 도구 위에 얹는 작업 흐름, 오케스트레이션, 자동화 계층

---

## AI 하네싱의 정의

**AI 하네싱**은 AI 코딩 에이전트를 직접 실행하는 것을 넘어, 반복 가능한 개발 워크플로우로 묶는 레이어입니다.

- 프롬프트를 매번 새로 작성하지 않고 패턴화합니다.
- 큰 작업을 작은 단위로 나누고 병렬로 실행합니다.
- 에이전트의 결과를 검증, 반복, 복구하는 절차를 자동화합니다.
- 사람은 목표와 판단에 집중하고, 도구는 실행 흐름을 담당합니다.

---

## 원본 도구와 하네싱 툴

| 구분 | 예시 | 역할 |
|---|---|---|
| 원본 도구 | Codex, Claude Code, OpenCode | 코드 생성과 수정의 실행 엔진 |
| 하네싱 툴 | OMO, OMX, OMC, LUX | 실행 엔진을 워크플로우로 묶는 제어 계층 |

원본 도구는 “코드를 잘 쓰는 엔진”이고, 하네싱 툴은 “그 엔진을 팀처럼 움직이게 하는 운영 방식”입니다.

---

## 왜 하네싱이 필요한가?

단일 AI 에이전트는 강력하지만, 실제 개발 작업은 한 번의 답변으로 끝나지 않습니다.

- 요구사항 분석, 설계, 구현, 검증이 반복됩니다.
- 여러 파일과 도메인을 동시에 추적해야 합니다.
- 실패한 시도를 복구하고 다른 전략으로 재시도해야 합니다.
- 좋은 프롬프트와 작업 패턴을 재사용해야 합니다.

하네싱은 이 반복 구조를 도구화합니다.

---

## 하네싱 레이어 아키텍처

```bash
+---------------------------------+
|        AI Harnessing Layer       |
|  (OMO / OMX / OMC / LUX)         |
|  - Workflow automation           |
|  - Multi-agent orchestration     |
|  - Pattern learning and reuse    |
+---------------+-----------------+
                |
        +-------+-------+
        |       |       |
        v       v       v
      Codex   Claude   OpenCode
      (Code)  (Code)   (Agent)
```

---

## 하네싱이 바꾸는 작업 방식

| 기존 방식 | 하네싱 방식 |
|---|---|
| 프롬프트를 매번 수동 작성 | 명령, 스킬, 워크플로우로 재사용 |
| 한 에이전트가 순차 처리 | 여러 에이전트가 병렬 탐색과 검증 |
| 사용자가 상태를 기억 | 상태 디렉토리와 런타임이 흐름 추적 |
| 실패 시 수동 재시도 | 루프와 플래너가 재시도 전략 수행 |

핵심은 “더 많은 자동화”가 아니라 “더 안정적인 개발 루프”입니다.

---

## 대표 하네싱 툴 네 가지

| 도구 | 대상 | 한 줄 설명 |
|---|---|---|
| OMO | OpenCode | OpenCode를 멀티 에이전트 실행 환경으로 확장 |
| OMX | Codex CLI | Codex 위에 계획, 팀 모드, 자율 루프를 추가 |
| OMC | Claude Code | Claude Code 안에서 자연어 기반 팀 오케스트레이션 제공 |
| LUX | Unity Editor | Unity Editor를 AI 작업 허브로 만드는 통합 툴킷 |

---

<!-- _class: lead -->

# Part 2. OMO

## oh-my-openagent

OpenCode용 멀티 에이전트 오케스트레이션 플러그인

---

## OMO 개요

**OMO(oh-my-openagent)**는 OpenCode를 단순한 AI 코딩 실행기가 아니라 팀형 워크플로우 하니스로 확장하는 플러그인입니다.

- 대상 도구: OpenCode
- 작성자: code-yeongyu
- 초점: 멀티 에이전트 작업 분해, 병렬 실행, 검증 루프
- 실행 모델: tmux 기반 병렬 런타임과 OpenCode 에이전트 조합

---

## OMO의 핵심 구조

```bash
+---------------------------+
| Sisyphus Orchestrator     |
| - Decompose goal          |
| - Route by category       |
| - Run agents in parallel  |
+------------+--------------+
             |
   +---------+---------+
   |         |         |
   v         v         v
 Hephaestus Explore   Oracle
 Implement  Research  Review
```

Sisyphus는 작업을 나누고, Hephaestus와 보조 에이전트들이 실행과 검증을 맡습니다.

---

## OMO 핵심 기능

- **Sisyphus 오케스트레이터**: 작업 분해와 병렬 실행을 담당합니다.
- **Hephaestus 자율 구현 에이전트**: 구체적인 구현 작업을 끝까지 수행합니다.
- **해시 앵커드 편집**: 변경 위치를 더 정확하게 추적합니다.
- **LSP 통합**: 타입, 심볼, 진단 정보를 작업 흐름에 연결합니다.
- **AST-Grep 통합**: 구조적 코드 검색과 리라이트를 지원합니다.

---

## OMO 런타임과 프로토콜

OMO는 단순 명령 모음이 아니라 런타임 계층에 가깝습니다.

- **tmux 기반 병렬 런타임**으로 여러 작업 세션을 관리합니다.
- **MCP 프로토콜 지원**으로 외부 도구와 컨텍스트를 연결합니다.
- **Planner / Loop 워크플로우**로 계획 수립과 반복 실행을 구성합니다.
- 작업 상태와 에이전트 역할을 분리해 큰 작업을 안정적으로 처리합니다.

---

<!-- _class: lead -->

# OMO 하네싱 구조

Hook, Agent, Category, Skill, Tool이 OpenCode 생명주기 위에 얹힌다

---

## 전체 아키텍처: OpenCode 위의 제어층

OMO는 OpenCode를 교체하지 않습니다. OpenCode의 실행 지점마다 Hook을 꽂아 입력, 판단, 도구 호출, 응답을 제어합니다.

```bash
+------------------------------+
| User Goal                    |
+---------------+--------------+
                |
                v
+------------------------------+
| OpenCode Lifecycle Handlers  |
| chat / messages / tools      |
+---------------+--------------+
                |
                v
+------------------------------+
| OMO Hook Registry            |
| 52 hooks in 5 layers         |
+---------------+--------------+
                |
      +---------+---------+
      |         |         |
      v         v         v
  Agents    Skills    Tools/MCP
```

핵심은 “프롬프트를 잘 쓰는 것”이 아니라 “생명주기 이벤트를 가로채 워크플로우를 강제하는 것”입니다.

---

## Hook 시스템: 왜 필요한가?

AI 에이전트는 기본적으로 다음 답변을 생성하는 모델 호출입니다. OMO의 Hook은 그 호출 전후에 정책과 상태를 주입합니다.

- 사용자의 문장을 작업 의도로 분류합니다.
- 위험한 도구 호출을 실행 전에 차단하거나 수정합니다.
- 도구 결과에 경고, 다음 단계, 복구 힌트를 붙입니다.
- 컨텍스트가 커지면 자동으로 압축을 예약합니다.
- 실패한 모델 호출을 다른 모델로 이어받게 만듭니다.

따라서 Hook은 OMO에서 “자동화가 실제로 개입하는 지점”입니다.

---

## Hook 시스템: 52개 Hook, 5계층

| 계층 | 개수 | 담당 영역 |
|---|---:|---|
| Session Hooks | 24 | `session.idle`, `session.error` 같은 세션 이벤트 처리 |
| Tool Guard Hooks | 14 | `tool.execute.before/after`에서 도구 호출 보호와 수정 |
| Transform Hooks | 5 | `messages.transform` 중심의 메시지 변환 |
| Continuation Hooks | 7 | Todo Enforcer, Atlas 등 세션 연속성 유지 |
| Skill Hooks | 2 | 스킬 탐지, 로딩, 권한 연결 |

5계층으로 나누는 이유는 Hook이 모두 같은 책임을 갖지 않기 때문입니다.

---

## Hook 계층 1: Session Hooks

Session Hooks는 에이전트가 “대화 중”인지 “장애 상태”인지 “유휴 상태”인지 감지합니다.

```bash
session.idle
    |
    v
+------------------------+
| Idle Hook              |
| - unfinished todos?    |
| - background tasks?    |
| - continuation needed? |
+-----------+------------+
            |
            v
   resume / notify / stop
```

이 계층이 없으면 장기 작업은 사용자가 직접 “계속해”라고 말해야만 이어집니다.

---

## Session Hooks 작동 예시

`session.error`는 API 장애, 모델 오류, 도구 실패를 같은 방식으로 보지 않습니다. 오류 성격에 따라 복구 경로를 다르게 잡습니다.

```bash
session.error
    |
    +--> transient API error --> session-recovery
    |
    +--> model failure -------> model-fallback
    |
    +--> tool failure --------> warn + retry guidance
```

`session-recovery`는 대화 상태를 보존하고, `model-fallback`은 가능한 대체 모델을 선택합니다.

---

## Hook 계층 2: Tool Guard Hooks

Tool Guard Hooks는 도구 실행 직전과 직후에 개입합니다.

```bash
Agent wants tool call
        |
        v
+-----------------------------+
| tool.execute.before         |
| - allow?                    |
| - rewrite arguments?        |
| - require safer command?    |
+-------------+---------------+
              |
              v
          Tool runs
              |
              v
+-----------------------------+
| tool.execute.after          |
| - append warning            |
| - normalize output          |
| - trigger continuation      |
+-----------------------------+
```

도구 권한은 프롬프트 규칙보다 강합니다. 실행 직전의 구조화된 차단 지점이기 때문입니다.

---

## Tool Guard Hooks가 막는 것

`tool.execute.before`는 특히 파일, Git, 셸 명령처럼 되돌리기 어려운 작업에서 중요합니다.

- 파괴적 Git 명령을 차단합니다.
- 대화형 명령을 비대화형 환경에서 실행하지 않게 합니다.
- 파일 편집 범위가 사용자 요청을 벗어나는지 확인합니다.
- 긴 셸 작업에는 timeout과 안전한 환경 변수를 붙입니다.
- 승인되지 않은 비밀 파일 접근을 경고합니다.

이 계층은 “모델이 실수로 실행하기 전에” 막기 위해 존재합니다.

---

## Hook 계층 3: Transform Hooks

Transform Hooks는 모델에 들어가는 메시지와 시스템 프롬프트를 재구성합니다.

```bash
raw messages
    |
    v
+-----------------------+
| messages.transform    |
| - trim noise          |
| - inject summaries    |
| - attach task state   |
+-----------+-----------+
            |
            v
+-----------------------+
| system.transform      |
| - agent role          |
| - category rules      |
| - tool policy         |
+-----------------------+
```

모델은 같은 대화라도 어떤 문맥을 받느냐에 따라 완전히 다르게 행동합니다.

---

## Transform Hooks가 필요한 이유

긴 작업에서는 모든 대화 원문을 계속 넣을 수 없습니다. Transform 계층은 “지금 필요한 문맥”으로 모델 입력을 재구성합니다.

- 완료된 탐색은 요약으로 바꿉니다.
- 현재 목표, 결정, 파일 목록을 앞쪽에 배치합니다.
- 에이전트별 역할 지시사항을 주입합니다.
- 카테고리별 검증 기준을 시스템 프롬프트에 반영합니다.

즉, Transform Hook은 토큰 절약이 아니라 의사결정 품질을 위한 입력 편집기입니다.

---

## Hook 계층 4: Continuation Hooks

Continuation Hooks는 작업이 중간에 끊기지 않게 합니다.

```bash
assistant turn ends
        |
        v
+-----------------------------+
| Continuation Hooks          |
| - unfinished todos?         |
| - active background tasks?  |
| - pending verification?     |
+-------------+---------------+
              |
      +-------+-------+
      |               |
      v               v
continue loop     finish safely
```

`todo-continuation-enforcer`는 할 일이 남았는데 답변을 끝내려는 흐름을 감지합니다.

---

## Continuation Hook 예시

`ralph-loop`와 `atlas`는 모두 연속성을 다루지만, 목표가 다릅니다.

| Hook | 작동 원리 | 목적 |
|---|---|---|
| `ralph-loop` | 목표 달성 여부를 반복 평가 | 자기 참조 개발 루프 |
| `atlas` | 백그라운드 작업과 할일 상태를 묶음 | 바위 연속성 오케스트레이션 |
| `todo-continuation-enforcer` | 미완료 todo를 종료 조건에 반영 | 중도 종료 방지 |

이 계층은 “AI가 답변을 생성했다”와 “작업이 끝났다”를 구분하게 만듭니다.

---

## Hook 계층 5: Skill Hooks

Skill Hooks는 사용자의 요청이 특정 도메인 작업인지 감지하고, 필요한 스킬을 로딩합니다.

```bash
chat.message
    |
    v
keyword-detector
    |
    +--> browser task --> playwright skill
    +--> git task -----> git-master skill
    +--> review task --> review-work skill
```

스킬은 단순 프롬프트가 아니라 도구, MCP, 권한 정책까지 포함할 수 있으므로 별도 Hook 계층이 필요합니다.

---

## OpenCode 연결점: 10개 핸들러

OMO Hook은 OpenCode가 제공하는 생명주기 핸들러에 등록됩니다.

| 핸들러 | OMO가 개입하는 지점 |
|---|---|
| `chat.params` | 모델, temperature, 토큰 같은 채팅 파라미터 수정 |
| `chat.headers` | Provider 요청 헤더 주입 |
| `chat.message` | 사용자 메시지 처리, 의도 분류, 컨텍스트 수집 |
| `messages.transform` | 대화 메시지 재배열과 요약 주입 |
| `system.transform` | 시스템 프롬프트 변환 |

이 다섯 지점은 모델 호출 “전”의 입력과 정책을 다룹니다.

---

## OpenCode 연결점: 실행과 이벤트

| 핸들러 | OMO가 개입하는 지점 |
|---|---|
| `tool.execute.before` | 도구 실행 전 차단, 수정, 권한 확인 |
| `tool.execute.after` | 도구 결과 후처리, 경고 추가, 후속 작업 예약 |
| `event` | `session.idle`, `session.error` 같은 세션 이벤트 처리 |
| `experimental.session.compacting` | 컨텍스트 압축 전 상태 정리 |
| `command.execute.before` | slash command 실행 전 정책 적용 |

이 다섯 지점은 실행 중의 안전성, 복구, 연속성을 담당합니다.

---

## Hook 등록 모델

Hook은 “이름”이 아니라 “핸들러 + 우선순위 + 실행 함수”로 이해해야 합니다.

```json
{
    "hooks": {
        "tool.execute.before": [
            "tool-guard",
            "secret-file-guard",
            "git-safety-guard"
        ],
        "event": [
            "session-recovery",
            "model-fallback",
            "atlas"
        ]
    }
}
```

여러 Hook이 같은 핸들러에 붙으면, 실행 순서가 곧 정책의 우선순위가 됩니다.

---

## Hook 실행 흐름 예시

사용자가 “이 버그 고쳐줘”라고 말하면 OMO는 단순히 모델에 전달하지 않습니다.

```bash
chat.message
    |
    v
keyword-detector
    |
    v
intent classification: fix
    |
    v
category routing: quick or deep
    |
    v
system.transform injects category rules
    |
    v
tool.execute.before guards edits/tests
    |
    v
event hooks watch errors and idle state
```

한 문장이 여러 Hook 계층을 지나며 실행 가능한 워크플로우로 바뀝니다.

---

## 주요 Hook: model-fallback

`model-fallback`은 모델 호출 실패를 “작업 실패”로 끝내지 않고 “다음 모델로 이어받을 조건”으로 처리합니다.

```bash
model call fails
      |
      v
+------------------------+
| model-fallback         |
| - classify failure     |
| - pick next provider   |
| - preserve messages    |
+-----------+------------+
            |
            v
gpt-5.5 -> claude-opus -> system default
```

핵심은 같은 목표와 문맥을 유지한 채 실행 엔진만 교체하는 것입니다.

---

## 주요 Hook: preemptive-compaction

`preemptive-compaction`은 컨텍스트가 한계에 가까워진 뒤가 아니라, 위험 임계치에 도달했을 때 먼저 움직입니다.

```bash
context tokens grow
        |
        v
threshold reached?
        |
        +-- no --> continue
        |
        +-- yes --> experimental.session.compacting
                         |
                         v
                    compress old phase
```

압축 Hook은 완료된 탐색, 결정, 파일 목록을 고밀도 요약으로 바꾸어 다음 턴의 판단 품질을 보존합니다.

---

## 주요 Hook: session-recovery

`session-recovery`는 API 장애나 네트워크 실패가 발생했을 때 세션을 새로 시작하지 않도록 돕습니다.

- 마지막 성공 메시지를 기준점으로 삼습니다.
- 실패한 도구 호출과 모델 호출을 분리해 기록합니다.
- 재시도 가능한 오류와 중단해야 하는 오류를 구분합니다.
- 필요하면 `model-fallback`과 이어져 대체 모델을 선택합니다.

장기 작업에서 복구 Hook은 생산성보다 신뢰성을 위해 중요합니다.

---

## 주요 Hook: keyword-detector

`keyword-detector`는 사용자의 문장에 들어 있는 워크플로우 신호를 감지합니다.

```bash
"ultrawork this"
        |
        v
keyword-detector
        |
        +--> mode: ultrawork
        +--> category: deep or ultrabrain
        +--> continuation policy: stronger
```

`ultrawork`, `search`, `analyze`, `review` 같은 단어는 단순 텍스트가 아니라 라우팅 힌트가 됩니다.

---

## Agent 아키텍처: 독립 실행 단위

OMO의 에이전트는 단순 persona가 아닙니다. 각 에이전트는 모델, 시스템 프롬프트, 도구 권한, 실행 표면이 묶인 단위입니다.

```bash
+---------------------------+
| Agent Definition          |
| - model                   |
| - system prompt           |
| - tool permissions        |
| - category behavior       |
+-------------+-------------+
              |
              v
        OpenCode run
```

그래서 같은 질문도 Sisyphus, Oracle, Explore가 받으면 전혀 다른 방식으로 처리합니다.

---

## 11개 내장 에이전트

| 에이전트 | 모델 | 역할 |
|---|---|---|
| **Sisyphus** | claude-opus-4-7 max | 메인 오케스트레이터, 작업 분해와 라우팅 |
| **Hephaestus** | gpt-5.5 medium | 자율 딥 워커, 복잡한 구현 수행 |
| **Prometheus** | claude-opus-4-7 max | 전략 플래너, 인터뷰 모드 요구사항 정제 |
| **Oracle** | gpt-5.5 high | 읽기 전용 컨설턴트, 아키텍처와 디버깅 |
| **Librarian** | gpt-5.4-mini-fast | 외부 문서와 코드 검색 전문가 |
| **Explore** | gpt-5.4-mini-fast | 코드베이스 grep, Contextual Grep |

---

## 11개 내장 에이전트 계속

| 에이전트 | 모델 | 역할 |
|---|---|---|
| **Multimodal-Looker** | gpt-5.3-codex medium | PDF, 이미지, 시각 자료 분석 |
| **Metis** | claude-opus-4-7 max | 사전 계획 컨설턴트, 숨겨진 의도와 모호성 식별 |
| **Momus** | gpt-5.5 xhigh | 계획 검토자, 명확성/검증가능성/완전성 평가 |
| **Atlas** | claude-sonnet-4-6 | 할일 목록 오케스트레이터 |
| **Sisyphus-Junior** | claude-sonnet-4-6 | 카테고리 작업 실행자 |

에이전트 목록은 모델 성능 순서가 아니라 역할 분리 순서로 보는 것이 좋습니다.

---

## 에이전트 라이프사이클

```bash
Prompt build
    |
    v
Agent execution
    |
    v
Hook interception
    |
    v
Tool calls / model calls
    |
    v
Response synthesis
```

Hook은 라이프사이클 바깥의 부가 기능이 아니라, 실행 중간마다 에이전트의 선택을 제한하거나 보강하는 제어면입니다.

---

## 에이전트별 권한 차이

에이전트마다 도구 권한이 다르기 때문에 “누가 실행하느냐”가 안전성과 속도에 영향을 줍니다.

| 에이전트 | 일반 권한 모델 |
|---|---|
| Sisyphus | 위임, 상태 관리, 제한적 직접 실행 |
| Hephaestus | 파일 편집, 테스트, 빌드, 구현 도구 사용 |
| Oracle | 읽기 중심, 분석과 조언, 직접 수정 없음 |
| Explore | 검색과 파일 읽기 중심 |
| Librarian | 웹 검색, 문서 조회, 외부 코드 예시 수집 |
| Multimodal-Looker | 이미지, PDF, 비주얼 입력 분석 |

권한 분리는 “모든 에이전트가 모든 것을 할 수 있는” 구조보다 실수 반경을 줄입니다.

---

## 권한 차이가 Hook과 만나는 지점

권한은 에이전트 정의에만 있지 않고 Tool Guard Hook에서 다시 검증됩니다.

```bash
Oracle requests edit
        |
        v
tool.execute.before
        |
        v
permission check fails
        |
        v
block and explain
```

반대로 Hephaestus가 편집을 요청하더라도, 요청 범위 밖의 파일을 건드리면 Hook이 경고하거나 차단할 수 있습니다.

---

## Sisyphus 오케스트레이터 원리

Sisyphus는 “가장 똑똑한 단일 에이전트”가 아니라 “라우터와 감독자”입니다.

- 사용자 의도를 분류합니다.
- 탐색, 계획, 구현, 검증 중 어디서 시작할지 정합니다.
- 전문 에이전트나 카테고리에 작업을 위임합니다.
- 백그라운드 작업 결과를 합성합니다.
- 필요한 경우 후속 작업을 다시 라우팅합니다.

기본 편향은 직접 구현이 아니라 위임입니다.

---

## 의도 분류 시스템

| 표면 형태 | 진짜 의도 | 라우팅 |
|---|---|---|
| "explain X" | 연구/이해 | explore/librarian → synthesize → answer |
| "implement X" | 구현 | plan → delegate or execute |
| "look into X" | 조사 | explore → report |
| "broken" | 수정 | quick diagnosis → delegate fix |
| "refactor X" | 변경 | scoped exploration → proposal |

Sisyphus는 단어 그대로가 아니라 작업의 성공 조건을 기준으로 분류합니다.

---

## 위임 결정 규칙

Sisyphus의 라우팅 질문은 세 단계입니다.

1. 이 요청에 완벽히 맞는 전문 에이전트가 있는가?
2. 아니면 가장 잘 맞는 카테고리가 있는가?
3. 내가 직접 하는 게 확실히 최선인가?

```bash
User goal
   |
   v
specialized agent? -- yes --> delegate
   |
   no
   v
category match? ---- yes --> spawn category worker
   |
   no
   v
direct execution only if trivial
```

이 구조가 큰 작업에서 병렬성과 전문성을 만듭니다.

---

## 카테고리 시스템: 도메인 최적화 모델

카테고리는 “작업 성격 → 모델 → 행동 규칙”의 매핑입니다.

| 카테고리 | 기본 모델 | 최적화 영역 |
|---|---|---|
| `visual-engineering` | google/gemini-3.1-pro-preview | UI/UX, 디자인, 스타일링 |
| `deep` | openai/gpt-5.5 medium | 자율 연구와 구현 |
| `ultrabrain` | openai/gpt-5.5 xhigh | 복잡한 논리와 아키텍처 |
| `quick` | 경량 모델 | 단일 파일, 단순 수정 |
| `artistry` | 창의적 모델/규칙 | 비표준 패턴, 창의적 접근 |

카테고리는 모델 선택만이 아니라 검증 강도와 탐색 예산도 바꿉니다.

---

## 카테고리 Hook 흐름

```bash
chat.message
    |
    v
intent classification
    |
    v
category selection
    |
    v
chat.params
    |
    +--> model = category.defaultModel
    +--> reasoning = category.depth
    +--> tools = category.allowedTools
```

`chat.params`와 `system.transform`은 선택된 카테고리를 실제 모델 호출 조건으로 바꿉니다.

---

## 모델 폴백 시스템

OMO의 모델 해상도는 4단계 파이프라인입니다.

```bash
1. Override
   UI에서 사용자가 선택한 모델, 주 에이전트 중심
        |
        v
2. Category default
   카테고리 설정의 기본 모델
        |
        v
3. Provider fallback
   AGENT_MODEL_REQUIREMENTS 체인
        |
        v
4. System default
   최종 안전 폴백
```

정상 경로에서는 위에서 아래로 결정하고, 에러 경로에서는 다음 후보로 이동합니다.

---

## 모델 폴백 실행 예시

```bash
openai/gpt-5.5 call
        |
        v
provider error
        |
        v
model-fallback hook
        |
        v
github-copilot/gpt-5.5
        |
        v
same task, same context, new provider
```

폴백의 목적은 답변 품질을 무작정 낮추는 것이 아니라, 같은 역할 요구사항을 만족하는 다음 실행 엔진을 찾는 것입니다.

---

## 스킬 시스템: 프롬프트 이상의 기능

OMO에서 스킬은 “긴 프롬프트 파일”보다 넓은 단위입니다.

- 도메인별 시스템 지시사항을 제공합니다.
- 필요할 때 임베디드 MCP 서버를 생성할 수 있습니다.
- 스킬 범위에 맞는 도구 권한을 부여합니다.
- 특정 작업 표면의 검증 절차를 강제합니다.

예를 들어 `playwright`는 브라우저 작업에서 단순 설명이 아니라 실제 브라우저 자동화를 요구합니다.

---

## 내장 스킬과 커스텀 스킬

| 종류 | 예시 | 용도 |
|---|---|---|
| 내장 스킬 | `git-master` | 원자 커밋, 안전한 Git 작업 |
| 내장 스킬 | `playwright` | 브라우저 자동화와 UI 검증 |
| 내장 스킬 | `review-work` | 구현 후 다중 관점 QA |
| 커스텀 스킬 | `~/.config/opencode/skills/*/SKILL.md` | 사용자 전역 스킬 |
| 프로젝트 스킬 | `.opencode/skills/*/SKILL.md` | 저장소별 작업 규칙 |

Skill Hook은 요청과 사용 가능한 스킬을 연결해 “언제 로딩할지”를 결정합니다.

---

## 스킬 로딩 흐름

```bash
User: "브라우저에서 확인해줘"
        |
        v
chat.message
        |
        v
keyword-detector + skill hooks
        |
        v
load playwright skill
        |
        v
system.transform adds browser QA rules
        |
        v
tool permission includes browser tools
```

스킬은 모델의 지식이 아니라 실행 환경의 능력을 확장합니다.

---

## 백그라운드 작업 시스템

OMO는 큰 작업을 단일 응답 안에서만 처리하지 않습니다. BackgroundManager가 하위 세션을 만들고 동시성을 제어합니다.

```bash
Parent session
      |
      +--> background task A: Explore
      +--> background task B: Librarian
      +--> background task C: Oracle
      |
      v
batched notifications
      |
      v
parent synthesis
```

부모 세션은 모든 세부 탐색을 직접 수행하지 않고 결과를 합성합니다.

---

## BackgroundManager가 필요한 이유

백그라운드 작업은 병렬성을 주지만, 통제 없이는 비용과 루프 문제가 생깁니다.

- 모델/프로바이더별 동시성을 제한합니다.
- 하위 세션 깊이를 추적해 무한 위임을 막습니다.
- 완료 알림을 배치해 부모 세션에 표시합니다.
- 작업 기록과 진행 상태를 유지합니다.
- 오래된 작업을 자동 정리합니다.

서킷 브레이커는 “에이전트가 에이전트를 계속 부르는” 실패 모드를 차단합니다.

---

## 백그라운드 Hook 흐름

```bash
Sisyphus delegates research
        |
        v
BackgroundManager creates child session
        |
        v
child agent runs with scoped prompt
        |
        v
event hooks capture completion
        |
        v
notification batched into parent
        |
        v
Sisyphus synthesizes result
```

Hook은 백그라운드 작업의 시작보다 완료와 복귀 지점에서 특히 중요합니다.

---

## 도구 통합 계층: 26개 도구

OMO의 도구 계층은 “모델이 직접 추측하지 않게 하는 외부 감각기관”입니다.

| 도구군 | 역할 |
|---|---|
| LSP | `goto_definition`, `find_references`, `symbols`, `diagnostics` |
| AST-Grep | 25개 언어의 구조적 패턴 매칭과 리라이트 |
| Hashline Edit | 줄번호 대신 `#ID` 해시로 정확한 코드 수정 |
| Compress | 대화 범위를 고밀도 요약으로 압축 |
| Tmux | REPL, 디버거, TUI 같은 대화형 터미널 실행 |

도구가 많을수록 Tool Guard Hook의 중요성도 커집니다.

---

## LSP와 AST-Grep Hook 원리

```bash
Agent asks: find callers
        |
        v
tool.execute.before
        |
        +--> permission check
        +--> argument normalization
        |
        v
LSP find_references
        |
        v
tool.execute.after
        |
        +--> summarize diagnostics
        +--> suggest next verification
```

LSP는 “이름 검색”보다 정확한 심볼 관계를 제공하고, AST-Grep은 텍스트 검색보다 안전한 구조 변경을 돕습니다.

---

## Hashline Edit와 Compress

Hashline Edit는 줄번호가 바뀌어도 안정적인 편집 앵커를 제공합니다.

```bash
source line -> hash id
      |
      v
edit references #abc123
      |
      v
runtime verifies target text
      |
      v
apply exact replacement
```

Compress는 완료된 대화 구간을 요약으로 대체해 장기 세션의 기억 품질을 유지합니다.

---

## Tmux 도구의 역할

Tmux 통합은 일반 셸 실행과 다릅니다. 계속 살아 있는 대화형 표면을 다룹니다.

- REPL을 켜고 여러 입력을 보낼 수 있습니다.
- 디버거, TUI, 장기 실행 서버를 유지할 수 있습니다.
- 세션을 분리해 부모 대화와 실행 표면을 나눕니다.
- Manual QA에서 실제 사용 흐름을 확인할 수 있습니다.

따라서 Tmux는 “명령 실행 도구”가 아니라 “작업 표면 유지 도구”입니다.

---

## MCP 통합: 3계층

MCP는 OMO가 OpenCode 밖의 지식과 도구에 접근하는 표준 연결면입니다.

| 계층 | 예시 | 역할 |
|---|---|---|
| 내장 원격 MCP | websearch, context7, grep_app | 검색, 공식 문서, GitHub 코드 사례 |
| Claude Code MCP | Claude Code 설정에서 가져온 MCP | 기존 MCP 자산 재사용 |
| 스킬 임베디드 MCP | 스킬이 요청 시 생성 | 도메인별 전용 도구 제공 |

MCP 계층은 Tool Guard Hook을 통해 권한과 호출 시점을 통제받습니다.

---

## MCP 호출 흐름

```bash
Need external docs
        |
        v
Librarian selected
        |
        v
skill or built-in MCP available?
        |
        v
tool.execute.before checks scope
        |
        v
context7 / websearch / grep_app
        |
        v
tool.execute.after summarizes evidence
```

OMO는 모델 기억에 없는 최신 정보를 MCP로 가져오되, 결과를 다시 Hook으로 정리해 부모 세션에 넘깁니다.

---

## 컨텍스트 관리: compress와 상태 슬라이스

장기 세션에서 컨텍스트 관리는 기능이 아니라 생존 조건입니다.

- `compress`는 대화 범위를 고밀도 요약으로 변환합니다.
- `PreemptiveCompactionHook`은 임계치 도달 시 자동 압축을 예약합니다.
- 상태 슬라이스 파일은 핵심 작업 상태를 작은 파일로 나눕니다.
- `goal.md`, `decisions.md`, `files.md`, `blockers.md`가 대표적인 슬라이스입니다.

컨텍스트 관리는 “잊지 않기”보다 “지금 필요한 것만 선명하게 남기기”에 가깝습니다.

---

## 상태 슬라이스 작동 방식

```bash
Long task state
      |
      +--> goal.md       : 현재 목표와 완료 기준
      +--> decisions.md  : 이미 내린 설계 결정
      +--> files.md      : 관련 파일과 변경 이유
      +--> blockers.md   : 막힌 지점과 필요한 입력
      |
      v
system.transform injects active slices
```

슬라이스 파일은 모든 에이전트가 같은 장기 목표를 공유하게 만드는 공용 메모리 역할을 합니다.

---

## 설정 시스템: 3레벨 병합

OMO 설정은 한 파일만 읽는 것이 아니라 범위별 설정을 병합합니다.

```bash
User config
~/.config/opencode/
        |
        v
Project config
.opencode/
        |
        v
Default config
plugin defaults
        |
        v
merged runtime config
```

사용자 설정은 개인 작업 습관을, 프로젝트 설정은 저장소 규칙을, 기본값은 안전한 출발점을 제공합니다.

---

## `oh-my-openagent.jsonc` 설정 예시

```jsonc
{
    "categories": {
        "deep": {
            "model": "openai/gpt-5.5",
            "reasoning": "medium",
            "exploration": "generous"
        },
        "visual-engineering": {
            "model": "google/gemini-3.1-pro-preview",
            "surface": "browser"
        }
    },
    "hooks": {
        "tool.execute.before": ["tool-guard", "git-safety-guard"],
        "event": ["session-recovery", "model-fallback", "atlas"],
        "experimental.session.compacting": ["preemptive-compaction"]
    }
}
```

설정은 “어떤 모델을 쓸지”뿐 아니라 “어떤 Hook 정책을 적용할지”까지 결정합니다.

---

## 에이전트 설정 예시

```jsonc
{
    "agents": {
        "oracle": {
            "model": "openai/gpt-5.5",
            "reasoning": "high",
            "permissions": ["read", "search", "diagnostics"]
        },
        "hephaestus": {
            "model": "openai/gpt-5.5",
            "reasoning": "medium",
            "permissions": ["read", "edit", "bash", "test"]
        }
    }
}
```

권한을 설정으로 분리하면 역할별 사고방식과 실행 가능 작업을 함께 제한할 수 있습니다.

---

## OMO 설치

```bash
bunx oh-my-opencode install
```

설치 후에는 OpenCode 환경 안에서 OMO가 제공하는 오케스트레이터, 에이전트, 워크플로우 명령을 사용할 수 있습니다.

---

## 실제 작업 흐름 예시

사용자가 “로그인 버그를 고치고 검증해줘”라고 요청하면 OMO는 다음처럼 움직입니다.

```bash
chat.message
    |
    v
Sisyphus intent classification: fix
    |
    v
Explore searches related files
    |
    v
Hephaestus edits implementation
    |
    v
Tool Guard runs tests safely
    |
    v
review-work or Oracle verifies risks
    |
    v
Continuation Hook checks remaining todos
```

이 흐름에서 Hook은 각 단계 사이의 접착제이자 안전장치입니다.

---

## OMO를 쓰는 이유

OMO는 OpenCode를 “한 번 실행하는 AI 도구”에서 “팀처럼 움직이는 작업 시스템”으로 바꿉니다.

- 큰 구현 작업을 여러 전문 역할로 나눌 수 있습니다.
- 탐색, 구현, 검증을 한 흐름으로 묶을 수 있습니다.
- LSP와 AST-Grep으로 코드베이스 이해도를 높일 수 있습니다.
- 반복적인 에이전트 운영 패턴을 재사용할 수 있습니다.

---

## OMO 핵심 요약

OMO의 본질은 Hook 기반 실행 제어입니다.

- 52개 Hook이 OpenCode의 10개 핸들러에 붙어 생명주기를 제어합니다.
- 11개 에이전트는 모델, 프롬프트, 권한이 다른 독립 실행 단위입니다.
- Sisyphus는 직접 구현보다 위임과 합성을 우선합니다.
- 카테고리와 모델 폴백은 작업 성격과 장애 상황에 맞게 실행 엔진을 바꿉니다.
- 스킬, 백그라운드 작업, 도구, MCP, 컨텍스트 관리가 Hook 계층을 통해 하나의 하니스가 됩니다.

---

<!-- _class: lead -->

# Part 3. OMX

## oh-my-codex
OpenAI Codex CLI를 요구사항 분석, 계획, 팀 실행, 자율 루프로 확장하는 워크플로우 레이어

---

## OMX 개요

**OMX(oh-my-codex)**는 OpenAI Codex CLI 위에 얹는 작업 흐름 하니스입니다.
- GitHub: `Yeachan-Heo/oh-my-codex`
- 작성자: Yeachan-Heo
- 대상 도구: OpenAI Codex CLI
- 설치 형태: npm 전역 패키지
- 핵심 초점: 분석, 계획, 병렬 실행, 지속 실행, 상태 보존
Codex가 “코드를 생성하는 엔진”이라면, OMX는 그 엔진을 “작업 시스템”으로 운영하는 계층입니다.

---

## OMX가 보는 문제

Codex CLI 자체는 강력한 코드 생성기이지만, 장기 작업의 모든 운영 문제를 대신 해결하지는 않습니다.
- 요구사항이 흐릿하면 좋은 코드도 잘못된 방향으로 나옵니다.
- 계획 없이 바로 구현하면 파일 변경이 산발적으로 커집니다.
- 큰 작업은 한 세션, 한 프롬프트, 한 응답으로 끝나지 않습니다.
- 실패 후 재시도 기준과 완료 판정이 사용자 머릿속에 남습니다.
OMX는 이 빈틈을 워크플로우 명령과 상태 디렉토리로 메웁니다.

---

## OMX 핵심 명령 지도

| 명령 | 역할 | 산출물 |
|---|---|---|
| `$deep-interview` | 요구사항 심층 질문 | `.omx/specs/` |
| `$ralplan` | 합의 기반 계획 수립 | `.omx/plans/` |
| `$plan` | 계획 정책과 승인 게이트 | `.omx/plans/` |
| `$team` | tmux 기반 다중 worker 실행 | `.omx/logs/`, `.omx/state/` |
| `$ralph` | 지속 실행 루프 | 검증된 완료 상태 |
| `$ultragoal` | 다중 목표 추적 | `.omx/ultragoal/` |
명령들은 독립 기능이 아니라 하나의 작업 파이프라인을 구성합니다.

---

## OMX 아키텍처 개요

```bash
+--------------------------------------------------+
|                    OMX Layer                     |
|  interview  plan  team  ralph  ultragoal         |
+-----------------------+--------------------------+
                        |
                        v
+--------------------------------------------------+
|              OpenAI Codex CLI Engine             |
|  read files  edit code  run commands  explain    |
+-----------------------+--------------------------+
                        |
                        v
+--------------------------------------------------+
|                 Project Workspace                |
|  source files  tests  docs  .omx/ state          |
+--------------------------------------------------+
```
OMX는 Codex CLI를 대체하지 않습니다. Codex가 작업하기 좋은 순서와 기억 장치를 제공합니다.

---

## tmux 기반 런타임 구조

```bash
+-------------------- tmux session --------------------+
|                                                       |
|  +----------------+    +--------------------------+   |
|  | leader pane    | -> | worker pane: architect   |   |
|  | orchestration  | -> | worker pane: implementer |   |
|  | dispatch       | -> | worker pane: reviewer    |   |
|  +----------------+    +--------------------------+   |
|                                                       |
+-------------------------------------------------------+
```
Leader pane은 작업을 쪼개고 worker pane은 실제 Codex 실행 단위처럼 움직입니다.

---

## OMX 워크플로우 전체 흐름

```bash
User Goal
    |
    v
$deep-interview
    |
    v
Structured Spec (.omx/specs/)
    |
    v
$ralplan / $plan
    |
    v
Approved Plan (.omx/plans/)
    |
    v
$team or $ralph
    |
    v
Architect Verification
    |
    v
Done or Retry
```
OMX가 강조하는 지점은 구현 이전의 명료화와 구현 이후의 판정입니다.

---

<!-- _class: lead -->

# `$deep-interview`

요구사항을 구현 가능한 명세로 바꾸는 소크라테스식 분석 명령

---

## `$deep-interview`의 목적

`$deep-interview`는 바로 코드를 쓰기 전에 요구사항의 빈칸을 찾아 질문합니다.
- 사용자의 목표 문장을 분해합니다.
- 불명확한 단어와 범위를 찾습니다.
- 구현 전 결정해야 할 선택지를 드러냅니다.
- 답변을 누적해 구조화된 spec으로 변환합니다.
- 결과를 `.omx/specs/`에 저장합니다.
“AI가 알아서 해줘”를 “AI가 구현할 수 있는 계약”으로 바꾸는 단계입니다.

---

## 소크라테스식 질문 흐름

```bash
User: 결제 화면을 개선해줘
  |
  v
OMX: 개선이 성능, UX, 디자인, 오류 처리 중 무엇인가요?
  |
  v
User: UX와 오류 메시지
  |
  v
OMX: 대상 사용자는 누구이고 성공 기준은 무엇인가요?
  |
  v
User: 신규 사용자, 결제 실패 사유가 바로 보여야 함
  |
  v
Spec: scope, persona, acceptance criteria
```
질문은 많이 하는 것이 목적이 아니라, 구현을 막는 모호성을 줄이는 것이 목적입니다.

---

## `$deep-interview`가 찾는 모호성

| 모호성 | 예시 | 필요한 질문 |
|---|---|---|
| 범위 | “대시보드 개선” | 어떤 화면과 기능인가? |
| 품질 기준 | “더 빠르게” | 어떤 지표를 얼마나 줄일까? |
| 사용자 | “쓰기 쉽게” | 누구에게 쉬워야 하는가? |
| 제약 | “리팩터링” | 공개 API를 바꿔도 되는가? |
| 완료 조건 | “버그 수정” | 어떤 재현 시나리오가 통과해야 하는가? |
이 단계가 없으면 계획과 구현은 그럴듯하지만 틀린 방향으로 갈 수 있습니다.

---

## Spec 문서 예시

```markdown
# Spec: Checkout Failure UX
## Goal
결제 실패 시 사용자가 실패 원인과 다음 행동을 즉시 이해하게 한다.
## Scope
- Checkout page error state
- Payment API failure message mapping
- Retry CTA copy
## Acceptance Criteria
- 카드 거절, 네트워크 실패, 인증 실패가 서로 다른 메시지를 보여준다.
- 실패 후 재시도 버튼이 같은 결제 정보를 유지한다.
- 기존 성공 결제 흐름은 변경되지 않는다.
```
이 문서는 `$ralplan`의 입력으로 이어집니다.

---

<!-- _class: lead -->

# `$ralplan` / `$plan`

합의 가능한 실행 계획을 만들고 승인 지점을 통제하는 계획 명령

---

## `$ralplan`의 목적

`$ralplan`은 요구사항 명세를 실행 가능한 단계로 바꿉니다.
- `$deep-interview` 결과를 읽습니다.
- 목표를 순서가 있는 작업 단위로 나눕니다.
- 각 단계의 산출물과 검증 방법을 붙입니다.
- 이해관계자 합의가 필요한 지점을 표시합니다.
- 계획을 `.omx/plans/`에 저장합니다.
이름은 다르지만 `$plan`과 함께 계획 정책을 다루는 핵심 축입니다.

---

## Consensus Planning의 의미

Consensus planning은 단순히 “AI가 계획을 써준다”가 아닙니다.
| 일반 계획 | Consensus planning |
|---|---|
| AI가 단독으로 단계 나열 | 사용자와 합의 가능한 선택지 제시 |
| 구현 순서 중심 | 결정, 위험, 승인 지점 포함 |
| 대화 안에서 소멸 | `.omx/plans/`에 지속 |
| 변경 시 기준 불명확 | 계획 업데이트 이력 기준 존재 |
합의는 속도를 늦추기 위한 장치가 아니라, 큰 작업의 재작업을 줄이는 장치입니다.

---

## `$ralplan` 계획 구조

```markdown
# Plan: Checkout Failure UX
## Inputs
- Spec: .omx/specs/checkout-failure-ux.md
## Steps
1. Map payment failure codes to user-facing reasons.
2. Update checkout view model error state.
3. Add retry CTA behavior.
4. Run focused tests and manual checkout failure scenarios.
## Approval Gates
- Message copy must be approved before implementation.
- API error code mapping must preserve existing contract.
```
좋은 계획은 작업 목록뿐 아니라 멈춰야 할 지점도 포함합니다.

---

## `$plan`의 역할

`$plan`은 계획의 세부 정책을 다룹니다.
- 계획을 얼마나 잘게 나눌지 정합니다.
- 사용자 승인 없이 진행 가능한 범위를 제한합니다.
- 위험한 명령이나 큰 리팩터링 앞에 게이트를 둡니다.
- `$team`이나 `$ralph`가 따라야 할 실행 기준을 제공합니다.
즉 `$ralplan`이 계획을 만들고, `$plan`은 그 계획의 운영 규칙을 정하는 축으로 볼 수 있습니다.

---

## Approval Gate 예시

```bash
Plan step 1: inspect current checkout flow
  -> no approval needed
Plan step 2: choose error message taxonomy
  -> approval required
Plan step 3: edit payment error mapping
  -> approval required if public API changes
Plan step 4: run tests and browser QA
  -> no approval needed
```
승인 게이트는 에이전트의 자율성을 없애는 것이 아니라, 되돌리기 어려운 선택을 보호합니다.

---

<!-- _class: lead -->

# `$team`

tmux leader/worker pane으로 Codex 실행을 실제 병렬 작업처럼 운영하는 명령

---

## `$team`의 목적

`$team`은 Codex 작업을 하나의 대화가 아니라 여러 역할의 병렬 실행으로 구성합니다.
- leader pane이 작업을 분해합니다.
- worker pane이 역할별로 실행합니다.
- tmux session이 터미널 수준에서 작업을 유지합니다.
- 각 pane의 출력과 결정은 `.omx/logs/`와 `.omx/state/`로 이어집니다.
중요한 점은 병렬 실행이 “가상의 역할극”이 아니라 tmux pane 기반이라는 것입니다.

---

## `$team` 기본 구조

```bash
+---------------------+---------------------+
| leader              | architect           |
| - dispatch          | - design checks     |
| - summarize         | - acceptance gate   |
+---------------------+---------------------+
| implementer         | reviewer            |
| - edit files        | - inspect diff      |
| - run tests         | - find regressions  |
+---------------------+---------------------+
```
화면은 pane으로 나뉘고, 각 pane은 맡은 역할의 Codex 실행 컨텍스트를 가집니다.

---

## Role Assignment

역할 배정은 작업 품질을 높이기 위한 가장 단순한 분리 방법입니다.
| 역할 | 책임 | 주로 보는 것 |
|---|---|---|
| architect | 설계와 수용 기준 | 구조, 의존성, 완료 조건 |
| implementer | 구현 | 코드, 테스트, 명령 실행 |
| reviewer | 검토 | diff, 회귀, 누락 |
| researcher | 조사 | 관련 파일, 문서, 기존 패턴 |
한 에이전트가 모든 관점을 동시에 유지하기 어렵기 때문에 역할을 분리합니다.

---

## Inter-pane Communication

```bash
+----------+       tmux send-keys        +-------------+
| leader   | --------------------------> | worker pane |
+----------+                             +-------------+
     ^                                           |
     |                                           v
     +------------- read pane output ------------+
```
통신은 복잡한 메시지 버스보다 단순합니다. 터미널 pane에 명령을 보내고 출력 상태를 읽는 방식입니다.

---

## `$team`의 병렬화 방식

`$team`은 모든 작업을 무조건 병렬화하지 않습니다.
```bash
Can run in parallel:
  - file discovery
  - risk review
  - test strategy planning
  - documentation impact check
Must run sequentially:
  - edit same file
  - apply schema migration
  - commit or release operation
  - approval-gated decision
```
좋은 병렬화는 작업 속도를 높이지만, 같은 파일을 동시에 고치면 오히려 충돌을 늘립니다.

---

## `$team`과 상태 파일

`$team`이 의미 있으려면 worker들의 작업 결과가 한곳에 모여야 합니다.
```bash
.omx/state/
  active-task.json
  worker-assignments.json
  verification-status.json
.omx/logs/
  leader.log
  architect.log
  implementer.log
  reviewer.log
```
상태와 로그는 leader가 다음 지시를 내릴 때 사용하는 근거가 됩니다.

---

<!-- _class: lead -->

# `$ralph`

architect 검증이 통과할 때까지 작업을 지속하는 persistence loop

---

## `$ralph`의 목적

`$ralph`는 “한 번 시도하고 멈추는” 실행이 아니라 “완료 조건이 만족될 때까지 반복하는” 실행입니다.
- 계획과 acceptance criteria를 읽습니다.
- 구현을 진행합니다.
- 테스트와 검증 결과를 확인합니다.
- 실패하면 원인을 반영해 다시 시도합니다.
- architect agent가 완료를 승인할 때 루프를 종료합니다.
자율성이 높은 만큼 완료 기준이 명확해야 잘 작동합니다.

---

## `$ralph` 루프 구조

```bash
while not architect_approved:
    inspect current state
    execute next plan step
    run verification
    collect failures
    update state
    retry with adjusted strategy
```
이 구조에서 중요한 것은 “계속 실행”이 아니라 “검증 결과를 다음 실행의 입력으로 쓰는 것”입니다.

---

## Architect Verification Gate

Architect gate는 `$ralph`의 종료 조건입니다.
| 확인 항목 | 질문 |
|---|---|
| 요구사항 | spec의 목표를 만족했는가? |
| 계획 | plan의 필수 단계가 끝났는가? |
| 테스트 | 관련 검증이 통과했는가? |
| 회귀 | 기존 동작을 깨지 않았는가? |
| 범위 | 요청 밖의 변경을 하지 않았는가? |
게이트가 없으면 지속 루프는 오래 실행될 뿐 완료 판정이 약해집니다.

---

## Self-healing의 의미

`$ralph`의 self-healing은 마법 같은 자동 수정이 아닙니다.
```bash
Failure: unit test fails
  -> inspect failing assertion
  -> compare with acceptance criteria
  -> patch implementation
  -> rerun focused test
Failure: build command unavailable
  -> record blocker
  -> try documented alternative
  -> stop if prerequisite is missing
```
실패를 숨기는 것이 아니라 실패를 다음 시도의 방향으로 변환합니다.

---

## Retry Strategy 예시

```bash
Attempt 1: implement direct change
  -> compile error in view model
Attempt 2: inspect existing binding pattern
  -> patch using project convention
Attempt 3: run focused scenario
  -> UI state passes but copy mismatch
Architect review:
  -> request message update
Attempt 4:
  -> update copy and verify acceptance criteria
```
루프의 품질은 무작정 반복이 아니라 실패로부터 얼마나 구체적으로 배우는지에 달려 있습니다.

---

<!-- _class: lead -->

# `$ultragoal`

여러 목표를 세션 밖으로 지속시키는 durable multi-goal workflow

---

## `$ultragoal`의 목적

`$ultragoal`은 하나의 작업이 아니라 여러 목표를 장기적으로 추적하기 위한 명령입니다.
- 목표 목록을 유지합니다.
- 각 목표의 상태와 다음 행동을 기록합니다.
- 세션이 끊겨도 다시 이어갈 수 있게 합니다.
- 목표 간 우선순위와 의존성을 관리합니다.
- 상태를 `.omx/ultragoal/`에 저장합니다.
단발성 수정이 아니라 긴 프로젝트 흐름에 맞는 기능입니다.

---

## `$ultragoal` 흐름

```bash
Capture goals
    |
    v
Prioritize active goal
    |
    v
Link spec and plan
    |
    v
Execute via $team or $ralph
    |
    v
Update durable goal state
    |
    v
Resume next session
```
이 흐름은 여러 날에 걸친 작업에서 특히 유용합니다.

---

<!-- _class: lead -->

# `.omx/` 상태 관리

명세, 계획, 문맥, 로그, 장기 목표를 파일 시스템에 남기는 기억 장치

---

## `.omx/` 디렉토리 개요

```bash
.omx/
  state/
  context/
  specs/
  plans/
  logs/
  wiki/
  ultragoal/
```
이 디렉토리는 OMX의 작업 기억입니다. Codex가 세션 안에서만 기억하는 것을 파일 기반 상태로 옮깁니다.

---

## `.omx/state/`

`.omx/state/`는 현재 실행 상태를 담습니다.
```bash
.omx/state/
  active-command.json
  current-plan-step.json
  worker-status.json
  verification-status.json
```
- 지금 어떤 명령이 실행 중인지 기록합니다.
- 어느 계획 단계에 있는지 추적합니다.
- worker pane의 상태를 요약합니다.
- 검증 성공, 실패, 대기 상태를 남깁니다.
현재 시점의 스냅샷에 해당합니다.

---

## `.omx/context/`

`.omx/context/`는 작업 중 생긴 결정과 배경 정보를 저장합니다.
- 중요한 설계 결정
- 사용자와 합의한 제약
- 회의식 질문과 답변 요약
- 작업 중 발견한 코드베이스 규칙
- 다음 세션에서 잊으면 안 되는 문맥
```bash
.omx/context/
  decisions.md
  constraints.md
  open-questions.md
```
대화가 길어질수록 context 파일은 작업 품질을 유지하는 안전망이 됩니다.

---

## `.omx/specs/`

`.omx/specs/`는 `$deep-interview`의 결과가 쌓이는 곳입니다.
```bash
.omx/specs/
  combat-balancing.md
  save-system-migration.md
  ai-tooling-slide-expansion.md
```
각 spec은 보통 다음을 포함합니다.
- 문제 정의
- 목표 사용자와 사용 시나리오
- 범위와 제외 범위
- acceptance criteria
- 아직 결정되지 않은 질문

---

## `.omx/plans/`

`.omx/plans/`는 `$ralplan`과 `$plan`의 결과가 저장되는 곳입니다.
```bash
.omx/plans/
  combat-balancing.plan.md
  save-system-migration.plan.md
```
계획 파일은 실행 단위의 기준점입니다.
- worker pane이 같은 작업 순서를 공유합니다.
- `$ralph`가 다음 단계와 완료 조건을 확인합니다.
- reviewer가 실제 diff와 계획을 비교합니다.
- 중단 후 재개할 때 현재 단계가 명확해집니다.

---

## 상태 흐름

```bash
$deep-interview
    -> .omx/specs/
$ralplan / $plan
    -> .omx/plans/
$team
    -> .omx/state/
    -> .omx/logs/
    -> .omx/context/
$ralph
    -> .omx/state/
    -> .omx/logs/
    -> verification result
$ultragoal
    -> .omx/ultragoal/
```
명령마다 남기는 상태가 다르기 때문에 `.omx/`는 자연스럽게 작업의 타임라인이 됩니다.

---

<!-- _class: lead -->

# tmux 런타임

터미널 세션, pane, 로그를 이용해 Codex 작업을 오래 유지하는 실행 기반

---

## 왜 tmux인가?

OMX가 tmux를 쓰는 이유는 단순합니다. tmux는 터미널 작업을 오래 유지하고 여러 pane으로 나눌 수 있습니다.
- SSH나 터미널이 끊겨도 세션이 남습니다.
- 여러 Codex 실행을 한 화면에서 관리할 수 있습니다.
- pane 단위로 역할을 분리할 수 있습니다.
- `send-keys`로 자동 입력을 보낼 수 있습니다.
- 로그와 출력 추적이 쉽습니다.
AI 워크플로우에서 tmux는 가벼운 런타임 컨테이너처럼 동작합니다.

---

## Disconnect Recovery

터미널 연결이 끊겼을 때 tmux 기반 OMX는 다음 방식으로 복구할 수 있습니다.
```bash
# reconnect to existing work session
tmux attach -t omx-project
```
복구 후 확인할 것들은 다음과 같습니다.
- leader pane이 마지막으로 내린 지시
- worker pane의 현재 출력
- `.omx/state/`의 active step
- `.omx/logs/`의 실패 또는 완료 기록
세션 유지와 파일 상태가 함께 있어야 복구가 안정적입니다.

---

<!-- _class: lead -->

# OMX와 Codex 직접 사용 비교

같은 엔진을 얼마나 긴 작업 흐름으로 운영할 수 있는가의 차이

---

## OMX vs 직접 Codex 사용

| 기준 | Codex CLI 직접 사용 | OMX |
|---|---|---|
| 시작 방식 | 목표를 바로 입력 | interview 또는 plan으로 시작 |
| 작업 단위 | 현재 대화 중심 | spec, plan, state 중심 |
| 병렬성 | 사용자가 직접 여러 터미널 관리 | `$team`이 pane 구성 |
| 지속성 | CLI 세션과 사용자 메모 | `.omx/`와 tmux session |
| 검증 | 사용자가 명령 지시 | `$ralph` 루프와 architect gate |
| 장기 목표 | 별도 문서 필요 | `$ultragoal` 추적 |
OMX는 Codex를 더 강한 모델로 바꾸는 것이 아니라 더 강한 프로세스 안에 넣습니다.

---

## OMX가 더 나은 경우

OMX는 다음 조건에서 가치가 커집니다.
- 사용자의 목표가 크고 모호합니다.
- 구현 전에 질문과 합의가 필요합니다.
- 여러 역할의 검토가 필요합니다.
- 장기 작업을 여러 세션에 걸쳐 이어가야 합니다.
- 실패 후 자동 재시도와 완료 판정이 중요합니다.
즉 “Codex가 코드를 잘 쓰는가”보다 “작업을 끝까지 운영할 수 있는가”가 문제일 때 OMX가 적합합니다.

---

<!-- _class: lead -->

# OMX 실전 워크플로우

모호한 목표를 명세, 계획, 팀 실행, 검증으로 통과시키는 예시

---

## 실전 예시: 작업 목표

사용자 목표가 다음과 같다고 가정합니다.
```text
인벤토리 UI가 느리고 상태가 꼬이는 문제를 고쳐줘.
세이브 데이터 형식은 바꾸면 안 돼.
```
이 목표에는 여러 모호성이 있습니다.
- “느리다”의 기준이 불명확합니다.
- “상태가 꼬인다”의 재현 조건이 없습니다.
- UI 변경 범위와 데이터 형식 제약이 충돌할 수 있습니다.
- 검증 방법이 아직 없습니다.

---

## 1단계: `$deep-interview`

```bash
$deep-interview "인벤토리 UI가 느리고 상태가 꼬이는 문제를 고쳐줘"
```
OMX가 물어볼 수 있는 질문은 다음과 같습니다.
- 느림은 초기 열기, 스크롤, 아이템 이동 중 어디에서 발생하나요?
- 상태 꼬임은 어떤 순서로 재현되나요?
- 세이브 데이터 형식은 어떤 파일이나 API를 의미하나요?
- 성공 기준은 프레임, 응답 시간, 테스트 중 무엇으로 볼까요?
질문 결과는 `.omx/specs/inventory-ui-state.md`로 정리됩니다.

---

## 3단계: `$ralplan`

```bash
$ralplan .omx/specs/inventory-ui-state.md
```
계획은 다음처럼 나뉠 수 있습니다.
```bash
1. Inspect inventory state model and UI binding path.
2. Reproduce stale state scenario with focused test or driver.
3. Patch state update ordering without changing save schema.
4. Verify existing save files still load.
5. Run UI interaction scenario and regression tests.
```
계획은 `.omx/plans/inventory-ui-state.plan.md`에 저장됩니다.

---

## 4단계: `$team`

```bash
$team .omx/plans/inventory-ui-state.plan.md
```
역할 분배 예시는 다음과 같습니다.
```bash
architect:
  confirm save schema boundary and state ownership
implementer:
  patch UI binding and state update path
reviewer:
  compare diff against acceptance criteria
researcher:
  find existing tests and reproduction paths
```
작업은 tmux pane에서 병렬로 진행됩니다.

---

## 5단계: `$ralph`

```bash
$ralph .omx/plans/inventory-ui-state.plan.md
```
`$ralph`는 완료 조건이 만족될 때까지 실행합니다.
```bash
run focused reproduction
  -> fail: duplicated item
patch update ordering
  -> pass focused reproduction
run save compatibility check
  -> pass
architect review
  -> asks for UI open-time evidence
run performance scenario
  -> pass
architect approved
```
중요한 것은 실패가 루프를 멈추지 않고 다음 시도의 입력이 된다는 점입니다.

---

<!-- _class: lead -->

# OMX 설치와 실행

Codex CLI, Node.js, tmux 위에 oh-my-codex를 설치하고 고자율 모드로 실행

---

## 설치 전제 조건

OMX를 쓰려면 몇 가지 전제 조건이 필요합니다.
- Node.js와 npm
- OpenAI Codex CLI
- `oh-my-codex` npm 패키지
- tmux가 동작하는 터미널 환경
- 작업할 Git 저장소
- Codex CLI 인증과 사용 권한
브라우저 기반 도구가 아니라 터미널 기반 작업 흐름이므로 로컬 CLI 환경이 중요합니다.

---

## 설치 명령

```bash
npm install -g @openai/codex oh-my-codex
```
설치 후 실행 예시는 다음과 같습니다.
```bash
omx --madmax --high
```
`--madmax`와 `--high`는 높은 자율성과 강한 추론 설정을 기대하는 실행 방식으로 이해할 수 있습니다.

---

## 기본 실행 흐름

```bash
# 1. 프로젝트 루트로 이동
cd my-project
# 2. OMX 실행
omx --madmax --high
# 3. 요구사항 분석
$deep-interview "Refactor checkout error handling"
# 4. 계획 수립
$ralplan .omx/specs/checkout-error-handling.md
# 5. 팀 실행 또는 지속 루프
$team .omx/plans/checkout-error-handling.plan.md
$ralph .omx/plans/checkout-error-handling.plan.md
```
명령은 프로젝트 루트와 `.omx/` 상태를 기준으로 이어집니다.

---

<!-- _class: lead -->

# OMX 한계와 주의점

강한 워크플로우 도구일수록 환경, 상태, 승인 경계 관리가 중요하다

---

## OMX의 주요 한계

OMX는 모든 문제를 자동으로 해결하는 도구가 아닙니다.
- Codex CLI의 능력과 권한에 의존합니다.
- tmux가 없는 환경에서는 핵심 병렬 런타임 가치가 줄어듭니다.
- 요구사항이 계속 바뀌면 spec과 plan도 계속 갱신해야 합니다.
- worker pane이 많아질수록 상태 조율 비용이 증가합니다.
- 로그와 상태 파일 관리 정책이 필요합니다.
하네싱은 복잡도를 제거하지 않고 구조화합니다.

---

## 상태 파일의 위험

`.omx/`는 유용하지만 주의가 필요합니다.
- 로그에 내부 경로와 오류 메시지가 남을 수 있습니다.
- context에 민감한 의사결정이나 고객 정보가 들어갈 수 있습니다.
- spec과 plan이 오래되면 오히려 잘못된 기준이 됩니다.
- 여러 브랜치에서 같은 `.omx/`를 공유하면 혼란이 생길 수 있습니다.
상태를 남긴다는 것은 정리와 보안 책임도 함께 생긴다는 뜻입니다.

---

## 좋은 사용 습관

- 먼저 `$deep-interview`로 모호성을 제거합니다.
- `$ralplan`으로 검증 단계가 포함된 계획을 남깁니다.
- 위험한 결정은 `$plan` approval gate로 멈춥니다.
- `$team`에서는 worker별 파일 소유권을 분명히 합니다.
- `$ralph`는 acceptance criteria가 명확할 때 사용합니다.
- `.omx/logs/`와 `.omx/state/`의 보안 정책을 정합니다.
도구보다 중요한 것은 도구가 따를 기준을 명확하게 주는 것입니다.

---

<!-- _class: lead -->

# OMX를 쓰는 이유

Codex의 코드 생성 능력을 반복 가능한 개발 운영 루프로 바꾸기 위해

---

## OMX의 핵심 가치

OMX는 Codex에 다음 네 가지를 더합니다.
- **명료화**: `$deep-interview`로 요구사항을 구현 가능한 spec으로 만듭니다.
- **합의**: `$ralplan`과 `$plan`으로 단계와 승인 지점을 정합니다.
- **병렬 실행**: `$team`으로 leader/worker 기반 실행을 구성합니다.
- **지속성**: `$ralph`, `$ultragoal`, `.omx/`로 중단되지 않는 흐름을 만듭니다.
결과적으로 사용자는 “프롬프트 운영자”보다 “목표와 기준을 정하는 감독자”에 가까워집니다.

---

## OMX가 팀에게 주는 효과

| 효과 | 설명 |
|---|---|
| 요구사항 품질 상승 | 구현 전 질문이 누락을 줄임 |
| 계획 재사용 | plan 파일이 다음 작업의 기준이 됨 |
| 검증 강화 | architect gate가 완료 판정을 구조화 |
| 맥락 보존 | `.omx/`가 세션 밖 기억을 제공 |
| 병렬 처리 | tmux worker가 역할별 작업을 분산 |
AI 도구의 생산성은 모델 성능뿐 아니라 작업을 어떻게 묶느냐에서 나옵니다.

---

## OMX 핵심 요약

OMX의 본질은 Codex CLI를 위한 command-driven workflow harness입니다.

- `$deep-interview`는 모호한 목표를 구조화된 spec으로 바꿉니다.
- `$ralplan`과 `$plan`은 합의 가능한 계획과 승인 게이트를 만듭니다.
- `$team`은 tmux leader/worker 모델로 실제 병렬 실행을 구성합니다.
- `$ralph`는 architect 검증이 통과할 때까지 지속 실행합니다.
- `$ultragoal`은 여러 목표를 `.omx/ultragoal/`에 장기 보존합니다.
- `.omx/`는 state, context, specs, plans, logs, wiki를 통해 작업 기억을 파일 시스템에 남깁니다.

---
<!-- _class: lead -->

# Part 4. OMC

## oh-my-claudecode

Claude Code용 멀티 에이전트 오케스트레이션 플러그인

---

## OMC 개요

**OMC(oh-my-claudecode)**는 Claude Code를 팀형 작업 시스템으로 확장하는 멀티 에이전트 오케스트레이션 플러그인입니다.

- GitHub: `Yeachan-Heo/oh-my-claudecode`
- HEAD SHA: `aacde3e1`
- 작성자: Yeachan-Heo
- 대상 도구: Claude Code
- 핵심 철학: Zero learning curve
- 핵심 가치: 자연어 기반 팀 오케스트레이션

---

## OMC가 보는 문제

Claude Code 자체도 강력하지만, 팀형 개발 워크플로우에는 운영 문제가 남습니다.

- 큰 작업은 계획, 구현, 검증, 수정이 반복됩니다.
- 여러 하위 작업은 동시에 진행할 수 있지만 한 세션은 순차화됩니다.
- 실패한 시도를 사람이 기억하고 다시 지시해야 합니다.
- 모델 선택, 토큰 관리, 컨텍스트 정리가 사용자의 부담으로 남습니다.

OMC는 이 운영 부담을 Claude Code 안쪽에서 자동화합니다.

---

## Zero Learning Curve 철학

OMC의 철학은 **Zero learning curve**입니다.

- 새 DSL을 먼저 배우지 않아도 됩니다.
- 자연어에 포함된 의도와 키워드를 감지합니다.
- 복잡한 팀 실행을 하나의 목표 문장으로 시작합니다.
- Claude Code를 쓰던 감각을 최대한 유지합니다.

강력한 오케스트레이션을 주되, 사용자에게 새 조작법을 강요하지 않습니다.

---

## OMC와 OMO/OMX의 차이

| 도구 | 기반 | 주된 사용 감각 |
|---|---|---|
| OMO | OpenCode | Hook, Agent, Skill, MCP를 통한 런타임 확장 |
| OMX | Codex CLI | 명령형 계획 루프와 Codex 실행 엔진 강화 |
| OMC | Claude Code | 자연어 기반 모드 선택과 Claude Code 경험 유지 |

OMC의 차별점은 강한 구조를 자연어 뒤에 숨기는 것입니다.

---

## OMC 4계층 아키텍처

```bash
+------------------------------+
| Hooks                        | lifecycle interception
+--------------+---------------+
               v
| Skills                       | reusable workflow patterns
+--------------+---------------+
               v
| Agents                       | delegated roles and routing
+--------------+---------------+
               v
| State                        | .claude persistent memory
+------------------------------+
```

네 계층이 합쳐져 자연어 요청을 팀 작업으로 바꿉니다.

---

## 4계층 책임 분리

| 계층 | 담당 책임 |
|---|---|
| Hooks | 도구 실행, 메시지, 오류, 위임 규칙을 생명주기에서 가로챔 |
| Skills | 반복 가능한 워크플로우 패턴을 `SKILL.md`로 로딩 |
| Agents | 역할 기반 작업자와 모델 라우팅을 연결 |
| State | `.claude/` 아래에 작업 기억과 진행 상태를 유지 |

이 분리가 있어야 자연어 한 줄이 안정적인 팀 작업으로 바뀝니다.

---

## 4계층 실행 흐름

사용자가 “autopilot으로 이 기능 완성해줘”라고 말하면 네 계층이 순서대로 개입합니다.

```bash
1. Hooks  -> magic keyword 감지, routing rule 주입
2. Skills -> autopilot workflow pattern 로딩
3. Agents -> plan / exec / verify 역할 배정
4. State  -> 진행 상황, 실패, 결정 기록
```

사용자 입장에서는 자연어 요청이지만 내부적으로는 구조화된 워크플로우입니다.

---

## Hooks 계층의 역할

Hooks는 Claude Code의 실행 흐름에서 정책을 주입하는 계층입니다.

- 도구 실행 전후에 guard를 적용합니다.
- 사용자 메시지를 작업 의도에 맞게 변환합니다.
- 실패와 오류를 복구 가능한 상태로 분류합니다.
- 에이전트 위임과 모델 라우팅 규칙을 자동 주입합니다.

OMC에서 자동화가 실제로 개입하는 지점은 대부분 Hooks입니다.

---

## Tool Guard Hooks

Tool guard는 도구 실행 전에 위험하거나 비효율적인 호출을 조정합니다.

```bash
Tool Request
    |
    v
+-------------------------+
| Tool Guard Hook         |
| - allow?                |
| - rewrite?              |
| - require delegation?   |
+-----------+-------------+
            v
       Tool Execution
```

---

## Message Transform Hooks

Message transform은 사용자 입력과 에이전트 메시지를 워크플로우가 이해하기 좋은 형태로 바꿉니다.

```bash
"autopilot으로 로그인 고쳐줘"
        |
        v
+----------------------------+
| Message Transform          |
| intent: autopilot          |
| goal: fix login            |
| stages: plan/exec/verify   |
+----------------------------+
```

사용자는 자연어를 쓰지만 OMC 내부에서는 구조화된 intent로 다룹니다.

---

## Error Recovery Hooks

Error recovery는 실패를 단순 종료로 보지 않고 다음 시도의 입력으로 바꿉니다.

- 도구 실패를 수집합니다.
- 실패가 환경 문제인지 구현 문제인지 구분합니다.
- `team-fix`나 autopilot fix stage로 전달합니다.
- 같은 실수를 반복하지 않도록 상태에 기록합니다.

자동화의 품질은 성공 경로보다 실패 경로에서 더 잘 드러납니다.

---

## Delegation Enforcer Hook

Delegation Enforcer는 OMC의 모델 라우팅과 역할 분리를 강제하는 핵심 Hook입니다.

```bash
Incoming Task
    |
    v
+--------------------------+
| Delegation Enforcer      |
| - classify complexity    |
| - inject routing rules   |
| - require right model    |
+------------+-------------+
      |      |      |
      v      v      v
   Haiku  Sonnet  Opus
```

---

## Skills 계층의 역할

Skills는 자주 반복되는 작업 절차를 재사용 가능한 패턴으로 묶는 계층입니다.

- 워크플로우 설명을 `SKILL.md` 형태로 제공합니다.
- 특정 작업에 필요한 단계, 규칙, 검증 기준을 포함합니다.
- 자연어 의도와 연결되어 자동 로딩될 수 있습니다.
- 팀 모드나 autopilot에서 실행 절차의 기준이 됩니다.

Skills는 OMC가 매번 새로 생각하는 것을 줄여줍니다.

---

## SKILL.md 로딩 흐름

```bash
User Intent
    |
    v
Skill Detection
    |
    v
Load SKILL.md
    |
    v
Apply Workflow Pattern
    |
    v
Agent Execution
```

`SKILL.md`는 “이 종류의 일은 이렇게 처리한다”는 작업 규약입니다.

---

## Agents 계층의 역할

Agents는 작업을 역할 단위로 분리합니다.

- 계획 담당과 구현 담당을 분리합니다.
- 검증 담당이 별도 관점으로 결과를 확인합니다.
- 복잡도에 따라 Haiku, Sonnet, Opus를 선택합니다.
- 팀 모드에서는 tmux worker와 연결됩니다.

OMC의 에이전트는 단순 persona가 아니라 실행 책임의 단위입니다.

---

## Role Assignment

큰 작업은 한 역할이 처음부터 끝까지 보는 것보다 역할을 나누는 편이 안정적입니다.

```bash
Goal
  |
  +--> Planner  : task breakdown / risk detection
  +--> Executor : file edits / command execution
  +--> Verifier : acceptance tests / failure report
```

OMC는 이 역할 분리를 자연어 모드 뒤에서 자동화합니다.

---

## Delegation Patterns

| 패턴 | 사용 상황 |
|---|---|
| Planner → Workers | 작업을 병렬 하위 과제로 나눌 때 |
| Worker → Verifier | 구현 결과를 별도 관점으로 검증할 때 |
| Verifier → Fixer | 실패 원인을 수정 루프로 넘길 때 |
| Enforcer → Model tier | 작업 난이도에 맞는 모델을 고를 때 |

Delegation은 “다른 AI에게 물어보기”가 아니라 작업 구조를 바꾸는 행위입니다.

---

## State 계층의 역할

State는 OMC가 장기 작업을 이어가기 위한 persistent working memory입니다.

- `.claude/` 디렉토리에 작업 상태를 저장합니다.
- 진행 중인 목표, 단계, 결정 사항을 기록합니다.
- 실패와 복구 시도를 다음 루프에 전달합니다.
- 컨텍스트 압축과 요약을 통해 긴 작업을 유지합니다.

State가 없으면 autopilot이나 Ralph는 매번 처음부터 다시 판단해야 합니다.

---

## State에 저장되는 정보

| 정보 | 목적 |
|---|---|
| Goal | 현재 해결해야 하는 최종 목표 유지 |
| Plan | 실행 단계와 병렬 작업 목록 유지 |
| Decisions | 왜 그렇게 바꾸기로 했는지 기록 |
| Failures | 실패 원인과 재시도 경로 기록 |
| Verification | 통과한 검증과 남은 검증 기록 |

상태는 모델의 기억을 보완하는 작업 기록입니다.

---

## 3-Tier 모델 라우팅

OMC는 Claude 모델을 세 단계로 나누어 사용합니다.

| Tier | 용도 |
|---|---|
| Haiku | 빠른 분류, 간단한 검색, 작은 수정 |
| Sonnet | 일반 구현, 표준 분석, 대부분의 작업 |
| Opus | 어려운 설계, 복잡한 디버깅, 고위험 판단 |

사용자가 매번 모델을 고르는 것이 아니라 작업 복잡도가 라우팅 기준이 됩니다.

---

## 자동 모델 선택 흐름

Delegation Enforcer는 작업을 분류하고 모델 선택 규칙을 주입합니다.

```bash
Task Complexity
      |
      +--> trivial / quick -----> Haiku
      +--> standard ------------> Sonnet
      +--> complex / risky -----> Opus
```

이 흐름은 비용 절감과 품질 보장을 동시에 노립니다.

---

## 모델 라우팅과 팀 모드

`omc team`에서는 역할별로 필요한 모델 수준이 달라질 수 있습니다.

| 역할 | 가능한 라우팅 |
|---|---|
| team-plan | Sonnet 또는 Opus |
| team-prd | Sonnet |
| team-exec | Sonnet 중심, 작은 작업은 Haiku |
| team-verify | Sonnet 또는 Opus |
| team-fix | 실패 난이도에 따라 Sonnet/Opus |

팀 전체가 같은 모델로만 움직인다고 가정하지 않습니다.

---

## `omc team`이란?

`omc team`은 OMC의 팀 기반 실행 모드입니다.

- tmux pane을 실제 worker 프로세스로 사용합니다.
- 계획, PRD, 실행, 검증, 수정 단계를 분리합니다.
- 병렬 가능한 작업을 여러 worker에게 나눕니다.
- 실패하면 fix 단계로 돌아가 루프를 이어갑니다.

핵심은 팀처럼 보이는 출력이 아니라 실제 프로세스 기반 병렬 실행입니다.

---

## tmux Real Process Model

```bash
+---------------- tmux session ----------------+
| pane 1: team-plan                            |
| pane 2: worker-auth                          |
| pane 3: worker-ui                            |
| pane 4: worker-tests                         |
| pane 5: team-verify                          |
+-----------------------------------------------+
```

각 pane은 독립된 작업 흐름을 가질 수 있고, 상위 오케스트레이션이 결과를 모읍니다.

---

## `omc team` 파이프라인

팀 모드는 다음 파이프라인으로 진행됩니다.

```bash
team-plan -> team-prd -> team-exec -> team-verify -> team-fix
                                         ^              |
                                         |              |
                                         +--- failed ---+
```

실패 시 `team-fix`가 루프에 포함되어 있다는 점이 중요합니다.

---

## Stage 1: team-plan

`team-plan`은 목표를 작업 가능한 하위 단위로 나눕니다.

- 목표와 제약을 정리합니다.
- 병렬화 가능한 하위 작업을 찾습니다.
- 작업 간 의존성을 표시합니다.
- 어떤 worker가 어떤 범위를 맡을지 결정합니다.

좋은 team-plan은 많이 나누기가 아니라 충돌 없이 나누기입니다.

---

## Stage 2: team-prd

`team-prd`는 실행 전에 요구사항을 문서화합니다.

- 성공 기준을 명시합니다.
- 범위 밖 항목을 정리합니다.
- 검증 방법을 미리 정의합니다.
- worker들이 같은 목표를 보도록 기준 문서를 만듭니다.

PRD가 있으면 병렬 worker가 서로 다른 해석으로 움직이는 위험이 줄어듭니다.

---

## Stage 3: team-exec

`team-exec`는 실제 worker 실행 단계입니다.

```bash
team-exec
  |
  +--> worker 1: feature slice A
  +--> worker 2: feature slice B
  +--> worker 3: tests / fixtures
  +--> worker 4: docs / integration
```

tmux pane들이 독립적으로 움직이며 각자의 결과를 남깁니다.

---

## Stage 4: team-verify

`team-verify`는 구현 결과를 acceptance criteria와 대조합니다.

- PRD의 성공 기준을 기준으로 확인합니다.
- 테스트, 빌드, 수동 확인 결과를 모읍니다.
- worker 간 충돌이나 누락을 찾습니다.
- 실패를 재현 가능한 fix 입력으로 정리합니다.

검증 단계가 별도로 있어야 병렬 실행의 품질을 회수할 수 있습니다.

---

## Stage 5: team-fix

```bash
Verify Failure
      |
      v
team-fix
  - identify failed criterion
  - assign fix worker
  - rerun verification
      |
      v
team-verify
```

OMC의 팀 모드는 실패를 먼저 자동 수정 루프로 보냅니다.

---

## Worker Lifecycle

| 단계 | 설명 |
|---|---|
| Spawn | tmux pane 또는 프로세스 생성 |
| Receive | 할당된 하위 작업과 제약 수신 |
| Execute | 파일 변경, 조사, 검증 수행 |
| Report | 결과, 실패, 남은 이슈 보고 |
| Close | 완료 후 세션 정리 또는 다음 루프 대기 |

worker 생명주기가 명확해야 병렬 실행이 통제됩니다.

---

## Inter-pane Communication

tmux 기반 팀 실행에서는 pane 간 직접 대화보다 상위 흐름을 통한 결과 취합이 중요합니다.

```bash
worker-auth ----+
worker-ui ------+--> team-verify --> team-fix
worker-tests ---+
```

verify 단계가 결과를 통합하고, fix 단계가 실패를 다시 작업 단위로 나눕니다.

---

## Autopilot이란?

Autopilot은 OMC의 5-stage full automation 모드입니다.

```bash
plan -> exec -> verify -> fix -> complete
```

- 사용자가 목표를 주면 자동으로 계획을 세웁니다.
- 계획에 따라 구현을 진행합니다.
- 검증 결과를 읽고 실패하면 수정합니다.
- 완료 기준을 만족하면 종료합니다.

Pipeline 모드는 이 autopilot 흐름으로 흡수된 것으로 봅니다.

---

## Autopilot 5단계

| 단계 | 역할 |
|---|---|
| plan | 목표, 범위, 실행 순서 정리 |
| exec | 실제 변경과 명령 실행 |
| verify | 테스트, 빌드, acceptance criteria 확인 |
| fix | 실패 원인 수정 후 재검증 |
| complete | 결과 요약과 상태 정리 |

사용자가 계속 “다음 단계 진행해”라고 말하지 않아도 루프를 이어갑니다.

---

## Autopilot Error Recovery

```bash
exec failed
   |
   +--> environment issue --> report blocker
   +--> code issue ---------> fix stage
   +--> unclear failure ----> re-plan or escalate model
```

Autopilot에서 오류는 종료 조건이 아니라 분기 조건입니다.

OMC는 오류를 분류한 뒤 가능한 경우 자체적으로 다음 시도를 수행합니다.

---

## Autopilot 사용 예

```bash
"autopilot으로 장바구니 할인 버그 고치고 테스트까지 돌려줘"
```

OMC 내부 흐름은 다음처럼 바뀝니다.

```bash
intent: autopilot
stages: plan / exec / verify / fix / complete
verification: tests + acceptance criteria
```

사용자 문장은 자연어지만 실행은 단계화됩니다.

---

## Ultrawork이란?

Ultrawork은 OMC의 parallel execution engine입니다.

- 큰 목표를 독립 하위 작업으로 나눕니다.
- 여러 worker가 동시에 실행합니다.
- 완료 결과를 다시 통합합니다.
- 병렬화로 처리 시간을 줄이는 데 초점을 둡니다.

Autopilot이 단계 자동화라면 Ultrawork은 병렬 실행 강화입니다.

---

## Ultrawork에 적합한 작업

| 작업 유형 | 예시 |
|---|---|
| 다중 파일 점검 | 여러 모듈의 동일 패턴 수정 |
| 독립 기능 조각 | UI, API, 테스트를 동시에 작성 |
| 조사 병렬화 | 여러 원인 후보를 worker별로 탐색 |
| 문서 확장 | 여러 섹션을 나누어 초안 작성 |

의존성이 낮은 하위 작업이 많을수록 효과가 큽니다.

---

## Ultrawork 실행 흐름

```bash
Goal
  |
  v
Split independent tasks
  |
  +--> worker A
  +--> worker B
  +--> worker C
  |
  v
Merge results -> Verify combined output
```

핵심은 병렬 실행 자체가 아니라 독립성을 먼저 판단하는 것입니다.

---

## Ralph란?

Ralph는 PRD 기반 persistence loop입니다.

- 요구사항 문서를 기준으로 작업합니다.
- acceptance criteria가 통과할 때까지 반복합니다.
- 실패하면 수정하고 다시 검증합니다.
- self-healing 성격의 장기 실행 모드입니다.

Ralph의 질문은 “답변을 했는가?”가 아니라 “완료 기준이 검증되었는가?”입니다.

---

## PRD와 Acceptance Criteria

```bash
PRD
  |
  +--> requirements
  +--> constraints
  +--> acceptance criteria
  +--> verification plan
```

| 상태 | 다음 행동 |
|---|---|
| 기준 통과 | complete |
| 일부 실패 | fix 후 재검증 |
| 기준 모호 | PRD 정리 또는 질문 |
| 환경 문제 | blocker 보고 |

---

## Self-healing Loop

Ralph는 실패를 스스로 복구하려는 루프를 가집니다.

```bash
execute
   |
   v
verify acceptance
   |
   +--> pass --> complete
   |
   +--> fail --> diagnose --> fix --> verify
```

이 구조는 한 번 시도하고 결과 보고하는 방식보다 장기 작업에 적합합니다.

---

## Natural Language Interface

OMC는 사용자가 복잡한 명령어를 외우지 않아도 되도록 설계되었습니다.

- “autopilot”을 말하면 자동 실행 루프를 감지합니다.
- “ralph”를 말하면 PRD 기반 persistence loop로 해석합니다.
- “ultrawork”을 말하면 병렬 실행 의도를 감지합니다.
- “search” 같은 키워드로 조사 흐름을 선택할 수 있습니다.

핵심은 문법이 아니라 의도입니다.

---

## Magic Keyword Detection

```bash
"ralph로 결제 실패 버그 끝까지 잡아줘"
        |
        v
mode: Ralph
basis: PRD / acceptance criteria
loop: verify -> fix -> verify
```

Magic keyword는 자연어 안에 섞여 있어도 작동하는 트리거입니다.

사용자는 Claude Code에게 말하듯 요청하고, OMC가 실행 모드를 잡습니다.

---

## Intent Detection과 Mode 선택

| 사용자 의도 | OMC가 고를 수 있는 흐름 |
|---|---|
| “처음부터 끝까지 알아서 해줘” | Autopilot |
| “여러 부분을 동시에 처리해줘” | Ultrawork 또는 team |
| “요구사항 만족할 때까지 반복해줘” | Ralph |
| “팀처럼 나눠서 검증까지 해줘” | `omc team` |

Zero learning curve는 이 intent detection에 의존합니다.

---

## HUD가 필요한 이유

자율 루프와 병렬 실행은 보이지 않으면 불안합니다.

- 지금 어떤 단계인지 알아야 합니다.
- 어떤 worker가 실행 중인지 확인해야 합니다.
- 토큰 사용량과 효율을 관찰해야 합니다.
- 실패가 fix loop로 들어갔는지 볼 수 있어야 합니다.

HUD는 자동화를 믿고 맡길 수 있는 상태로 만드는 가시성 계층입니다.

---

## HUD Presets

| Preset | 용도 |
|---|---|
| minimal | 필수 정보만 표시 |
| focused | 현재 작업과 상태 중심 |
| full | 전체 상태 개요 |
| dense | 많은 정보를 compact하게 표시 |
| analytics | 토큰 사용량과 성능 지표 확인 |
| opencode | OpenCode-compatible display |

같은 자동화라도 사용 상황에 따라 필요한 정보량이 다릅니다.

---

## Token Efficiency 목표

OMC는 30-50% token reduction을 목표로 합니다.

이 목표는 단순히 답변을 짧게 쓰는 것이 아닙니다.

- 쉬운 작업에는 가벼운 모델을 사용합니다.
- 반복 패턴은 Skill로 재사용합니다.
- 진행 상태는 State에 정리합니다.
- 긴 컨텍스트는 요약과 압축으로 관리합니다.
- HUD analytics로 사용량을 관찰합니다.

---

## Context Compression

장기 작업에서 모든 대화를 그대로 들고 가면 비용과 품질이 동시에 나빠집니다.

```bash
Conversation history
       |
       v
Extract decisions / failures / criteria
       |
       v
Compact state summary
       |
       v
Continue loop
```

OMC는 다음 단계에 필요한 형태로 기억하기를 지향합니다.

---

## 예시 목표

사용자가 Claude Code에서 다음처럼 요청한다고 가정합니다.

```bash
"omc team으로 로그인 리다이렉트 버그를 고치고,
테스트와 회귀 확인까지 끝내줘"
```

이 문장에는 세 가지 의도가 들어 있습니다.

- `omc team`: 팀 파이프라인 사용
- 로그인 리다이렉트 버그: 해결 목표
- 테스트와 회귀 확인: 검증 기준

---

## Step 1: Intent와 Mode 감지

Hooks 계층은 입력에서 실행 모드를 감지합니다.

```bash
input text
  |
  v
mode: omc team
workflow: team-plan -> team-prd -> team-exec -> team-verify -> team-fix
verification: tests + regression check
```

긴 설정 파일 없이 내부에는 구조화된 실행 계획이 생깁니다.

---

## Step 2-4: plan / prd / exec

팀 실행은 바로 수정하지 않고 기준과 작업 분해를 먼저 만듭니다.

| 단계 | 산출물 | 의미 |
|---|---|---|
| `team-plan` | worker 분해 | 인증 흐름, 수정, 테스트를 나눔 |
| `team-prd` | acceptance criteria | 완료 기준과 범위 밖 항목을 고정 |
| `team-exec` | worker 결과 | 각 pane이 자기 범위의 작업 수행 |

이후 `team-verify`가 결과를 통합합니다.

---

## Step 5: team-verify

검증 단계는 결과를 acceptance criteria에 맞춰 확인합니다.

- 테스트가 실제로 통과했는지 확인합니다.
- 로그인 실패 케이스가 깨지지 않았는지 확인합니다.
- 변경 범위가 인증 흐름 안에 머물렀는지 확인합니다.
- 누락이 있으면 `team-fix` 입력으로 정리합니다.

검증은 보고서가 아니라 다음 행동을 결정하는 단계입니다.

---

## Step 6: team-fix Loop

```bash
failed criterion:
  "로그아웃 후 보호 페이지 접근 시 redirect target 누락"

team-fix:
  - assign worker-fix
  - patch route guard
  - rerun verify
```

이 루프는 acceptance criteria가 통과하거나 blocker가 명확해질 때까지 이어집니다.

---

## 설치 전제 조건

OMC를 사용하려면 기본 실행 환경이 필요합니다.

- Claude Code
- Claude Max/Pro subscription
- tmux
- OMC plugin 또는 npm package 설치 권한

주의할 점은 OMC가 Claude Code를 대체하지 않는다는 것입니다. Claude Code가 먼저 있어야 그 위에서 작동합니다.

---

## Claude Code Plugin 설치

Claude Code plugin marketplace를 사용하는 설치 흐름입니다.

```bash
# Claude Code plugin marketplace
/plugin marketplace add https://github.com/Yeachan-Heo/oh-my-claudecode
/plugin install oh-my-claudecode
```

이 방식은 Claude Code 안에서 플러그인으로 OMC를 사용하는 경로입니다.

---

## npm 설치

전역 npm 패키지로 설치할 수도 있습니다.

```bash
npm i -g oh-my-claude-sisyphus@latest
```

설치 후에는 OMC가 제공하는 team, autopilot, ultrawork, ralph 흐름을 Claude Code 작업에 연결해 사용합니다.

---

## Claude 구독 의존성

OMC는 Claude Code 위에서 동작하므로 Claude 사용 조건의 영향을 받습니다.

- Claude Max/Pro subscription이 필요합니다.
- 사용량 제한이나 모델 접근 권한의 영향을 받습니다.
- 네트워크나 서비스 상태가 작업 안정성에 영향을 줍니다.
- 모델 비용과 토큰 사용량을 계속 관찰해야 합니다.

OMC의 자동화는 기반 모델 접근이 안정적일 때 가장 잘 작동합니다.

---

## tmux 의존성

팀 모드와 병렬 worker 흐름은 tmux 환경에 의존합니다.

```bash
OMC team mode
   |
   v
tmux session / panes
   |
   v
real worker processes
```

따라서 tmux가 익숙하지 않은 환경이나 제한된 터미널 환경에서는 설정과 운영을 확인해야 합니다.

---

## 자동화의 한계

Zero learning curve는 아무 판단도 필요 없다는 뜻이 아닙니다.

- 요구사항이 모호하면 PRD나 질문이 필요합니다.
- 병렬화가 부적절하면 충돌이 커질 수 있습니다.
- acceptance criteria가 없으면 Ralph의 완료 판정이 약해집니다.
- 실패가 외부 비밀값이나 권한 문제라면 자동 복구가 어렵습니다.

OMC는 사람의 판단을 없애기보다 반복 운영을 줄입니다.

---

## OMC의 핵심 가치

OMC는 Claude Code 사용자에게 가장 자연스러운 하네싱 경로를 제공합니다.

- Claude Code 사용 경험을 유지합니다.
- 자연어로 실행 모드를 선택합니다.
- tmux 기반 실제 worker로 병렬 실행을 제공합니다.
- Hooks, Skills, Agents, State로 자동화 구조를 나눕니다.
- 모델 라우팅과 token efficiency를 함께 고려합니다.

강한 구조를 낮은 학습 비용으로 제공하는 것이 핵심 가치입니다.

---

## OMC 핵심 요약

OMC의 본질은 **Claude Code 안에서 자연어 기반 팀 오케스트레이션을 제공하는 것**입니다.

- 4계층: Hooks → Skills → Agents → State
- 모델 라우팅: Haiku → Sonnet → Opus
- 팀 모드: `team-plan → team-prd → team-exec → team-verify → team-fix`
- 자율 모드: Autopilot, Ultrawork, Ralph
- 운영 목표: Zero learning curve와 30-50% token reduction

Claude Code를 대화형 AI에서 운영 가능한 개발 팀으로 확장하는 하네싱 툴입니다.

---

---

<!-- _class: lead -->

# Part 5. LUX

## Linalab Unity X

> 📘 **LUX 전용 가이드**: `slides/supplement-lux-guide.md`에서 60장 분량의 상세 슬라이드를 확인하세요.

Unity Editor를 AI 작업 허브로 만드는 Unity 특화 하네싱 툴킷

---

## LUX 개요

**LUX(Linalab Unity X)**는 Unity Editor를 AI 작업 허브로 만드는 AI adapter 및 automation toolkit입니다.

- 대상 환경: Unity Editor
- 회사: Linalab
- 라이선스: MIT 오픈소스
- 아키텍처: Unity Editor ↔ Rust Gateway (Axum) ↔ Web UI (React SPA) ↔ AI Tools

---

## LUX 핵심 기능

- **Multi-AI Terminal**: Claude Code, Codex, OpenCode를 Unity 안에서 통합
- **Skill Dispatch**: 작업 성격에 따라 AI별로 자동 분배
- **ReactFlow 파이프라인 에디터**: 작업 흐름을 시각화
- **WebRTC 원격 제어**: Editor 상태를 외부에서 확인하고 조작
- **Unity Git 통합**: Editor 안에서 버전 관리 흐름
- **6가지 코어 스킬**: compile, test, screenshot, logs, playmode, execute-code

---

## LUX를 쓰는 이유

LUX는 Unity 프로젝트에서 AI 작업의 중심을 터미널이 아니라 Editor로 옮깁니다.

- Unity 개발자가 익숙한 화면에서 AI 작업을 실행합니다.
- 여러 AI 도구를 한 터미널 경험으로 묶습니다.
- 시각적 파이프라인으로 반복 작업을 설계합니다.
- Git, 원격 제어, 도구 실행을 Unity 워크플로우에 연결합니다.

> 📘 LUX 설치, 아키텍처, 실전 워크플로우는 **`slides/supplement-lux-guide.md`**에서 상세히 다룹니다.

<!-- _class: lead -->

# Part 6. 비교 및 선택 가이드

OMO, OMX, OMC, LUX를 상황에 맞게 고르는 기준

---

## OMO vs OMX vs OMC vs LUX

| 도구 | 대상 | 강점 | 적합한 상황 |
|---|---|---|---|
| OMO | OpenCode | Hook, 에이전트, 모델 라우팅 | OpenCode를 팀형 런타임으로 확장 |
| OMX | Codex CLI | 요구사항 분석, 계획, tmux 루프 | Codex를 장기 작업 시스템으로 운용 |
| OMC | Claude Code | 자연어 팀 오케스트레이션 | Claude Code 경험을 유지하며 병렬화 |
| LUX | Unity Editor | 에디터와 시각 표면 통합 | Unity 씬, 에셋, UI까지 AI가 다룰 때 |

---

## 언제 어떤 툴을 쓸 것인가?

- OpenCode 기반 자동화와 Hook 제어가 중요하면 **OMO**가 어울립니다.
- Codex CLI를 계획 중심의 장기 실행기로 쓰고 싶으면 **OMX**가 어울립니다.
- Claude Code에서 자연어 기반 팀 실행을 원하면 **OMC**가 어울립니다.
- Unity Editor와 씬, 에셋까지 AI 루프에 넣고 싶으면 **LUX**가 어울립니다.

---

## 조합 추천

```bash
Text-heavy codebase automation
  -> OMO or OMX

Claude Code-first workflow
  -> OMC

Unity gameplay / scene / UI work
  -> LUX + one general code harness
```

하나의 도구가 모든 표면을 완벽히 다루기보다, 작업 표면에 맞는 하네스를 고르는 것이 중요합니다.

---

## 선택 기준 체크리스트

- 내가 주로 쓰는 기본 AI 코딩 도구는 무엇인가?
- 작업이 단발성인가, 여러 세션에 걸친 장기 작업인가?
- 병렬 실행과 검증 루프가 필요한가?
- Unity Editor처럼 코드 밖 작업 표면이 중요한가?
- 팀이 상태 파일과 로그를 저장소에 남겨도 되는가?

---

## 핵심 요약

AI 하네싱 툴은 모델을 바꾸는 것이 아니라 작업 방식을 바꿉니다.

- **OMO**는 OpenCode를 Hook 기반 멀티 에이전트 런타임으로 확장합니다.
- **OMX**는 Codex CLI에 분석, 계획, tmux 팀 실행, 지속 루프를 더합니다.
- **OMC**는 Claude Code 안에서 자연어 기반 팀 오케스트레이션을 제공합니다.
- **LUX**는 Unity Editor와 시각적 작업 표면을 AI 루프에 연결합니다.

좋은 선택 기준은 “어떤 모델이 제일 똑똑한가”가 아니라 “내 작업 표면과 검증 루프가 무엇인가”입니다.
