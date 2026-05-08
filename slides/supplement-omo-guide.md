---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# OMO Guide

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

