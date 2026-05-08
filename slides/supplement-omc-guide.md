---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# OMC Guide

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

