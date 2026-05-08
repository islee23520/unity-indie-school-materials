---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# OMX Guide

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
