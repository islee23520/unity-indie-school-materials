---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# AI Coding CLI Tools

## 왜 CLI 기반 AI 코딩 도구를 써야 하는가

터미널 중심 워크플로로 AI를 개발 루프 안에 자연스럽게 통합하기

---

# 오늘의 목표

**AI 코딩 도구를 GUI가 아니라 CLI로 다루는 이유를 이해합니다**

1. CLI가 반복 개발에 강한 이유
2. 주요 AI 코딩 CLI 도구의 특징과 설치 방법
3. 개인, 숙련자, 팀 환경별 선택 기준

---

# 전체 구성

| Part | 주제 | 핵심 질문 |
|---|---|---|
| **Part 1** | 왜 CLI인가 | GUI보다 CLI가 강한 상황은 무엇인가 |
| **Part 2** | 주요 AI 코딩 툴 | Codex, Claude Code, OpenCode, Gemini CLI는 어떻게 다른가 |
| **Part 3** | 선택 가이드 | 내 상황에는 어떤 도구가 맞는가 |

---

<!-- _class: lead -->

# Part 1: 왜 CLI인가

## Why CLI over GUI

---

# GUI AI 도구의 장점

**GUI는 시작 장벽이 낮습니다**

- 대화창이 익숙하고 접근성이 좋음
- 코드 설명, 오류 메시지 해석, 아이디어 브레인스토밍에 편함
- 비개발자나 초보자가 빠르게 질문하기 쉬움
- 화면 공유나 시각적 피드백에 강함

> GUI는 "질문하는 도구"로 훌륭하지만, 반복 가능한 개발 워크플로에는 한계가 있습니다.

---

# CLI AI 도구의 핵심 차이

**CLI는 AI를 개발 환경의 일부로 넣습니다**

- 현재 디렉터리, git 상태, 파일 구조를 직접 이해
- 명령 실행, 테스트, 빌드, 수정 루프를 한 자리에서 처리
- 터미널, 에디터, CI/CD, 스크립트와 연결 가능
- 사람이 복사해서 붙여넣는 단계를 줄임

```bash
codex "Refactor this module and run the related tests"
```

---

# 키보드 중심 반복 속도

**개발자는 대부분의 시간을 작은 반복에 씁니다**

- 검색하고 수정하기
- 테스트하고 실패 원인 확인하기
- diff를 보고 다시 고치기
- commit 단위로 정리하기

CLI에서는 이 반복이 모두 키보드 안에서 이어집니다.

```bash
rg "PlayerHealth"
npm test
git diff
```

---

# 컨텍스트 전환 비용 줄이기

**GUI 왕복은 생각보다 비쌉니다**

| 작업 | GUI 중심 | CLI 중심 |
|---|---|---|
| 코드 전달 | 복사/붙여넣기 | 파일 직접 읽기 |
| 결과 확인 | 사람이 명령 실행 | AI가 테스트 실행 |
| 수정 확인 | 화면 전환 | `git diff` 확인 |
| 반복 | 대화창 왕복 | 명령 히스토리 재사용 |

CLI의 장점은 한 번의 큰 작업보다 수십 번의 작은 반복에서 누적됩니다.

---

# 스크립트 자동화

**CLI는 자동화의 기본 단위입니다**

- 정해진 명령을 스크립트로 저장 가능
- CI/CD에서 동일한 명령 실행 가능
- 팀원 간 재현 가능한 절차 공유 가능
- 실패 로그를 그대로 AI에게 분석시킬 수 있음

```bash
npm run lint
npm test
npm run build
```

---

# CI/CD 연동

**CLI 도구는 로컬과 서버의 언어가 같습니다**

```bash
# local
npm test

# GitHub Actions
npm test
```

AI CLI는 로컬에서 같은 명령을 실행하며 실패를 고칠 수 있습니다.

```bash
claude "Fix the failing CI command: npm test"
```

---

# 파이프라인 구성

**Unix 철학: 작은 도구를 연결해 큰 일을 합니다**

- `stdin`으로 입력 받기
- `stdout`으로 결과 내보내기
- 로그, diff, 테스트 결과를 다른 명령과 연결하기

```bash
git diff | codex "Review this change for bugs"
```

파이프라인은 AI를 단독 앱이 아니라 개발 도구 체인의 한 단계로 만듭니다.

---

# 로그 분석 파이프라인

**실패 로그를 그대로 전달할 수 있습니다**

```bash
npm test 2>&1 | codex "Explain the root cause and propose a fix"
```

좋은 CLI 워크플로는 사람이 요약하지 않습니다.

- 원본 로그를 전달
- 원본 diff를 전달
- 원본 테스트 결과를 전달
- AI가 추측하지 않도록 함

---

# 버전 관리와 자연스러운 통합

**코딩 에이전트의 작업 단위는 git diff입니다**

```bash
git status
git diff
git add src/player.ts
git commit -m "Fix player jump buffering"
```

AI CLI는 다음을 직접 확인할 수 있습니다.

- 어떤 파일이 바뀌었는가
- 변경이 너무 넓지 않은가
- 테스트가 변경과 맞는가
- commit 가능한 상태인가

---

# 멀티 파일 편집

**현대 개발 작업은 한 파일에서 끝나지 않습니다**

- 타입 정의 변경
- 호출부 수정
- 테스트 업데이트
- 문서 보정
- import 정리

CLI 에이전트는 프로젝트 루트에서 여러 파일을 읽고 수정하며, 전체 diff를 기준으로 작업을 정리할 수 있습니다.

---

# Git Workflow와 AI

**AI에게도 작업 경계를 줘야 합니다**

```bash
git status
codex "Implement the requested change, keep the diff minimal"
git diff
npm test
```

좋은 습관:

- 시작 전 worktree 확인
- 작업 후 diff 확인
- 테스트와 빌드 실행
- 불필요한 파일 변경 제거

---

# 체크포인트와 감사 추적

**CLI 작업은 흔적을 남기기 쉽습니다**

- shell history
- git diff
- commit log
- CI log
- test report
- agent transcript

GUI 대화만으로는 "무엇이 실제로 바뀌었는가"를 추적하기 어렵습니다.

---

# 재현 가능한 프롬프트

**프롬프트도 명령처럼 관리할 수 있습니다**

```bash
codex "Update README according to the current CLI flags"
claude "Find dead code in src and propose removals"
gemini "Summarize this repository architecture"
```

팀에서는 자주 쓰는 요청을 문서화하거나 custom command로 만들 수 있습니다.

---

# 확장성: MCP

**MCP는 AI 도구가 외부 시스템과 대화하는 표준 인터페이스입니다**

- GitHub, Figma, Slack, Notion 같은 도구 연결
- 데이터베이스나 내부 API 조회
- 브라우저 자동화, 문서 검색, 디자인 시스템 탐색
- 프로젝트별 커스텀 도구 제공

CLI 도구는 MCP 서버 실행, 환경 변수, 권한 범위를 명확히 관리하기 좋습니다.

---

# 확장성: Hooks와 Custom Commands

**반복 규칙은 도구에 심을 수 있습니다**

- 작업 시작 전 `git status` 확인
- 파일 수정 후 formatter 실행
- 테스트 실패 시 로그 저장
- 특정 명령을 slash command로 등록
- 팀 규칙을 hook으로 강제

```bash
opencode run "check changed files and run related tests"
```

---

# GUI vs CLI 비교표

| 기준 | GUI AI 도구 | CLI AI 도구 |
|---|---|---|
| 시작 난이도 | 낮음 | 중간 |
| 반복 작업 속도 | 보통 | 빠름 |
| 자동화 | 제한적 | 강함 |
| git 연동 | 간접적 | 직접적 |
| CI/CD 연동 | 약함 | 강함 |
| 멀티 파일 작업 | 도구별 차이 큼 | 강함 |
| 감사 추적 | 대화 기록 중심 | diff, log, command 중심 |
| 확장성 | 앱 기능에 의존 | MCP, hooks, script로 확장 |

---

# CLI가 특히 강한 상황

**다음 작업에서는 CLI가 유리합니다**

- 테스트를 실행하며 고쳐야 하는 버그 수정
- 여러 파일을 함께 바꿔야 하는 리팩터링
- 빌드, lint, typecheck가 중요한 프로젝트
- git diff 기준으로 리뷰해야 하는 작업
- CI 실패를 재현하고 수정해야 하는 상황
- 팀 규칙을 자동으로 적용해야 하는 환경

---

# CLI 사용 시 주의점

**강력한 만큼 운영 습관이 필요합니다**

- 작업 전 `git status`로 변경 상태 확인
- AI가 만든 diff를 반드시 직접 검토
- destructive command는 명시적으로 통제
- secret, token, private data를 프롬프트에 넣지 않기
- 비용과 rate limit 확인
- 권한이 넓은 MCP 서버는 신중히 연결

---

<!-- _class: lead -->

# Part 2: 주요 AI 코딩 툴 소개

## Codex CLI, Claude Code, OpenCode, Gemini CLI

---

# OpenAI Codex CLI: 개요

**터미널에서 사용하는 OpenAI 코딩 에이전트**

- 로컬 프로젝트를 읽고 수정하는 CLI 기반 agent
- ChatGPT 계정 또는 OpenAI API key로 사용
- 코드 생성, 수정, 설명, 테스트 실행 루프 지원
- OpenAI 모델 생태계와 자연스럽게 연결

```bash
npm install -g @openai/codex
```

---

# OpenAI Codex CLI: 주요 기능

**ChatGPT 경험을 터미널 워크플로로 가져옵니다**

- 프로젝트 파일 기반 질의응답
- 파일 수정과 patch 적용
- shell command 실행 보조
- test/build 결과를 바탕으로 반복 수정
- OpenAI 모델의 코드 추론 능력 활용

```bash
codex "Explain the architecture of this repository"
codex "Fix the failing unit tests"
```

---

# OpenAI Codex CLI: 가격과 사용 이유

| 항목 | 내용 |
|---|---|
| **가격 모델** | ChatGPT Plus/Pro 또는 OpenAI API key 기반 |
| **강점** | OpenAI 모델 품질, ChatGPT와의 친숙함 |
| **적합한 사용자** | ChatGPT를 이미 쓰고 있고 터미널 작업을 시작하려는 개발자 |
| **주의점** | 계정 플랜, API 비용, 조직 정책 확인 필요 |

**왜 쓰는가**

- ChatGPT에서 하던 코딩 대화를 실제 파일 수정 루프로 옮기기 좋음
- OpenAI 생태계를 이미 쓰는 팀에 도입 장벽이 낮음

---

# Claude Code: 개요

**Anthropic의 터미널 기반 agentic coding tool**

- 프로젝트를 탐색하고 직접 수정하는 CLI 코딩 에이전트
- git workflow를 중심으로 작업 추적
- 대규모 코드베이스 이해와 리팩터링에 강점
- Claude 모델의 긴 컨텍스트와 코드 리뷰 능력 활용

```bash
curl -fsSL https://claude.ai/install.sh | bash
```

---

# Claude Code: 주요 기능

**코드베이스 안에서 계획, 수정, 검증을 반복합니다**

- 파일 검색과 구조 파악
- 멀티 파일 편집
- 테스트 실행과 실패 분석
- git diff 기반 리뷰
- 자연어로 작업 지시

```bash
claude "Find the root cause of this test failure and fix it"
claude "Refactor this service without changing behavior"
```

---

# Claude Code: 가격과 사용 이유

| 항목 | 내용 |
|---|---|
| **가격 모델** | Pro, Max, Team, Enterprise 플랜 중심 |
| **강점** | agentic workflow, 코드베이스 이해, git 중심 작업 |
| **적합한 사용자** | 복잡한 리팩터링과 리뷰를 자주 하는 개발자 |
| **주의점** | 조직별 보안 정책과 사용량 제한 확인 필요 |

**왜 쓰는가**

- 큰 변경을 작은 diff로 정리하는 작업에 잘 맞음
- 코드 리뷰, 테스트 기반 수정, 리팩터링 루프가 자연스러움

---

# OpenCode: 개요

**provider-agnostic 오픈소스 AI 코딩 에이전트**

- 특정 모델 벤더에 묶이지 않는 CLI/TUI 기반 도구
- 75+ provider 지원
- LSP 통합으로 코드 이해 품질 향상
- MCP, custom command, agent workflow 확장 가능

```bash
npm install -g opencode-ai
```

---

# OpenCode: 주요 기능

**모델 선택권과 개발자 워크플로 확장이 핵심입니다**

- OpenAI, Anthropic, Google 등 다양한 provider 연결
- LSP diagnostics, symbol 탐색 등 코드 인텔리전스 활용
- 프로젝트별 설정 파일로 동작 조정
- hooks와 custom command로 팀 규칙 자동화
- MCP로 외부 도구 연결

```bash
opencode
opencode run "summarize the current diff"
```

---

# OpenCode: 가격과 사용 이유

| 항목 | 내용 |
|---|---|
| **가격 모델** | 오픈소스 무료, Zen/Go 유료 옵션 가능 |
| **강점** | provider 독립성, 확장성, LSP 통합 |
| **적합한 사용자** | 여러 모델을 비교하거나 벤더 종속을 피하려는 개발자 |
| **주의점** | provider별 API key와 비용 구조를 별도로 관리해야 함 |

**왜 쓰는가**

- 한 벤더의 모델 정책이나 장애에 묶이지 않음
- 팀별 규칙, hook, MCP 기반 워크플로를 세밀하게 구성하기 좋음

---

# Gemini CLI: 개요

**Google의 오픈소스 AI 코딩 CLI**

- Gemini 모델을 터미널에서 사용하는 도구
- Google Search grounding 지원
- MCP와 tool 사용을 통한 확장
- Google 생태계와 자연스럽게 연결

```bash
npm install -g @google/gemini-cli
```

---

# Gemini CLI: 주요 기능

**검색 기반 최신 정보와 Google 연동이 강점입니다**

- 코드 작성과 설명
- repository 탐색
- Google Search grounding으로 최신 정보 보강
- MCP 서버 연결
- Google 계정 기반 사용 흐름

```bash
gemini "Explain this build error"
gemini "Search and summarize the latest API changes"
```

---

# Gemini CLI: 가격과 사용 이유

| 항목 | 내용 |
|---|---|
| **가격 모델** | 무료 티어 제공 |
| **무료 한도** | 60 req/min, 1000 req/day |
| **강점** | Google Search grounding, Google 생태계 연동 |
| **적합한 사용자** | 최신 문서 확인과 검색 기반 개발 보조가 필요한 사용자 |
| **주의점** | 무료 한도, 모델별 품질 차이, 조직 계정 정책 확인 필요 |

**왜 쓰는가**

- 실시간 검색이 중요한 라이브러리 조사나 오류 분석에 유용
- Google Workspace, Cloud 환경과 함께 쓰기 좋음

---

# 공통 설치 전 준비

**AI CLI 도구는 로컬 개발 환경 위에서 동작합니다**

```bash
git --version
node --version
npm --version
```

확인할 것:

- Git 설치 여부
- Node.js와 npm 버전
- 프로젝트 테스트 명령
- API key 또는 계정 로그인 방식
- 회사 보안 정책

---

# 공통 운영 습관

**AI CLI를 안전하게 쓰는 기본 루틴**

```bash
git status
# run AI coding tool
git diff
# run tests
git status
```

원칙:

- 작업 전후 상태를 비교
- 테스트가 없는 변경은 수동으로 확인
- AI가 만든 코드를 반드시 읽기
- commit은 사람이 의미를 확인한 뒤 수행

---

# 좋은 요청 예시

**작업 범위와 검증 기준을 함께 줍니다**

```bash
codex "Fix the login redirect bug. Keep the change minimal and run related tests."
```

```bash
claude "Refactor the inventory module for readability without changing behavior. Show the final diff."
```

```bash
opencode run "Review the current git diff for regression risks"
```

---

# 나쁜 요청 예시

**범위가 넓고 검증 기준이 없습니다**

```bash
codex "Make this project better"
```

```bash
claude "Rewrite the architecture"
```

```bash
gemini "Fix everything"
```

문제점:

- 변경 범위가 폭발함
- 테스트 기준이 없음
- 리뷰하기 어려운 diff가 생김
- 팀 규칙을 벗어나기 쉬움

---

<!-- _class: lead -->

# Part 3: 툴 선택 가이드

## 내 상황에 맞는 CLI 고르기

---

# 기능 비교 테이블

| 도구 | 모델/Provider | 오픈소스 | 강점 | 적합한 상황 |
|---|---|---|---|---|
| **Codex CLI** | OpenAI | 도구 정책 확인 필요 | ChatGPT 연동, OpenAI 모델 | ChatGPT 사용자, OpenAI 중심 팀 |
| **Claude Code** | Anthropic | 도구 정책 확인 필요 | agentic coding, git workflow | 리팩터링, 리뷰, 큰 코드베이스 |
| **OpenCode** | 75+ provider | 예 | provider 독립성, LSP, 확장성 | 모델 선택권, 커스텀 워크플로 |
| **Gemini CLI** | Google Gemini | 예 | Search grounding, Google 연동 | 최신 정보 조사, Google 생태계 |

---

# 가격 비교 테이블

| 도구 | 기본 과금 관점 | 메모 |
|---|---|---|
| **Codex CLI** | ChatGPT Plus/Pro 또는 API key | 개인 계정과 API 비용 구조 확인 |
| **Claude Code** | Pro/Max/Team/Enterprise | 플랜별 사용량과 조직 정책 확인 |
| **OpenCode** | 오픈소스 무료 + provider 비용 | Zen/Go 같은 유료 옵션 가능 |
| **Gemini CLI** | 무료 티어 + 유료 확장 가능 | 60 req/min, 1000 req/day 무료 한도 |

가격은 빠르게 바뀔 수 있으므로, 도입 전 공식 문서를 다시 확인해야 합니다.

---

# 초보자 추천

**처음에는 익숙한 모델 하나로 시작합니다**

추천 흐름:

1. 이미 쓰는 계정 기반 도구 선택
2. 작은 파일 수정부터 시작
3. `git diff` 읽는 습관 만들기
4. 테스트 실행을 요청에 포함
5. destructive command는 직접 실행

예시:

```bash
codex "Explain this file and suggest one safe improvement"
```

---

# 숙련자 추천

**숙련자는 도구보다 workflow를 설계합니다**

- task 유형별로 도구를 나누기
- custom command로 반복 요청 표준화
- MCP로 GitHub, browser, docs 연결
- LSP diagnostics와 test runner를 검증 루프에 포함
- provider별 모델 장단점 비교

```bash
opencode run "inspect current diff, run related checks, and report risks"
```

---

# 팀 환경 고려사항

**팀에서는 개인 생산성보다 통제가 중요합니다**

- 코드와 프롬프트에 secret이 들어가지 않도록 정책화
- 사용 가능한 provider와 모델 제한
- 저장소별 agent instruction 관리
- audit log와 비용 추적
- CI와 동일한 검증 명령 사용
- PR 리뷰에서 AI 생성 diff도 동일 기준 적용

---

# 기업 환경 고려사항

**Enterprise 도입은 보안과 거버넌스가 핵심입니다**

| 고려사항 | 질문 |
|---|---|
| **데이터 보존** | 코드가 학습에 사용되는가 |
| **접근 제어** | 누가 어떤 저장소에 접근할 수 있는가 |
| **감사 로그** | 누가 어떤 변경을 만들었는가 |
| **비용 관리** | 팀별 사용량을 추적할 수 있는가 |
| **모델 정책** | 허용된 provider와 region은 무엇인가 |
| **도구 확장** | MCP와 hook 권한을 어떻게 제한할 것인가 |

---

# 상황별 추천

| 상황 | 추천 |
|---|---|
| ChatGPT를 이미 적극적으로 사용 | **Codex CLI** |
| 큰 코드베이스에서 리팩터링과 리뷰 중심 | **Claude Code** |
| 여러 모델을 바꿔 쓰고 싶음 | **OpenCode** |
| 벤더 종속을 피하고 싶음 | **OpenCode** |
| 최신 문서와 검색 grounding이 중요 | **Gemini CLI** |
| Google Cloud/Workspace 중심 팀 | **Gemini CLI** |

---

# 실전 도입 순서

**한 번에 모든 것을 바꾸지 않습니다**

1. 개인 로컬에서 작은 작업에 사용
2. `git diff`와 테스트 검증 습관 정착
3. 팀 공통 프롬프트와 금지 규칙 정리
4. CI 명령과 동일한 검증 루프 연결
5. MCP, hooks, custom command를 단계적으로 추가
6. 비용과 보안 로그를 보고 운영 정책 조정

---

# 수업/프로젝트에서의 추천 기본값

**학습 환경에서는 재현성과 설명 가능성이 중요합니다**

- CLI 명령을 슬라이드와 문서에 그대로 남기기
- AI가 만든 변경은 반드시 diff로 설명하기
- 테스트와 빌드 명령을 함께 실행하기
- 팀원이 같은 명령을 따라 할 수 있게 만들기
- GUI는 아이디어 탐색, CLI는 실제 변경에 사용하기

---

# 핵심 요약

**CLI AI 도구의 가치는 자동화 가능한 개발 루프에 있습니다**

- GUI는 질문과 설명에 편리함
- CLI는 실제 코드 변경, 검증, git workflow에 강함
- 좋은 AI 코딩은 프롬프트보다 검증 루프가 중요함
- 도구 선택은 모델 취향보다 팀의 workflow와 보안 정책에 맞춰야 함

---

# 다음 실습 제안

**작은 저장소에서 안전하게 연습합니다**

```bash
git status
codex "Find one small documentation improvement and apply it"
git diff
```

확인 질문:

- 어떤 파일이 바뀌었는가
- 변경 이유가 설명 가능한가
- 테스트나 preview가 필요한가
- commit할 만한 단위인가

---

# 마무리

**AI 코딩 CLI는 개발자를 대체하는 도구가 아니라 개발 루프를 압축하는 도구입니다**

좋은 사용자는 AI에게 모든 것을 맡기지 않습니다.

- 범위를 정하고
- 명령을 실행하고
- diff를 읽고
- 테스트로 확인하고
- 책임 있게 merge합니다.
