---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
style: |
  section {
    font-family: 'Noto Sans KR', sans-serif;
  }
  h1 {
    color: #e94560;
    font-size: 2.5em;
  }
  h2 {
    color: #0f3460;
    font-size: 1.8em;
  }
  h3 {
    color: #16c79a;
    font-size: 1.4em;
  }
  code {
    background-color: #16213e;
    padding: 2px 6px;
    border-radius: 4px;
    font-family: 'Fira Code', monospace;
  }
  pre {
    background-color: #16213e;
    padding: 16px;
    border-radius: 8px;
  }
  .keyword {
    color: #e94560;
    font-weight: bold;
  }
  .accent {
    color: #16c79a;
    font-weight: bold;
  }
  table {
    font-size: 0.85em;
  }
  th {
    background-color: #0f3460;
    color: #fff;
  }
  td {
    background-color: #16213e;
  }
---

<!-- _class: lead -->

# Unity 실무 중심 메트로배니아 게임 개발

## Comprehensive Course Overview

**총 14세션, 주 2회, 세션당 3시간**

Steam 출시 품질의 횡스크롤 플랫포머 슈팅 메트로배니아 게임 제작

---

## 강의 개요

**코스 타임라인**

| 항목 | 내용 |
|------|------|
| **총 기간** | 7주 (14세션) |
| **주당 세션** | 2회 (화/목 또는 주말) |
| **세션당 시간** | 3시간 |
| **총 학습 시간** | 42시간 |
| **최종 목표** | Steam 출시 가능한 메트로배니아 게임 완성 |
| **AI 모델** | Claude Sonnet 4.6 |

---

## 학습 목표

**이 강의를 통해 다음을 달성합니다**

- **C# & Unity**: 실무 수준의 C# 프로그래밍과 Unity 엔진 활용
- **아키텍처**: Clean Architecture와 의존성 주입 (VContainer) 구현
- **애니메이션**: Spine 2D와 LitMotion을 활용한 부드러운 애니메이션
- **AI**: NavMesh 2D와 상태 머신 기반 적 AI 구현
- **UI**: UI Toolkit과 MVVM 패턴으로 현대적인 UI 시스템 구축
- **최적화**: Draw Call 최적화와 메모리 관리로 60FPS 유지
- **출시**: Steamworks 연동과 자동화된 빌드 파이프라인 구축

---

## 기술 스택 개요

**핵심 기술 스택**

| 영역 | 기술 | 용도 |
|------|------|------|
| **엔진** | Unity 6.3 LTS | 게임 엔진 |
| **아키텍처** | Clean Architecture + VContainer | 구조 설계 및 DI |
| **반응형** | R3 (Reactive Extensions) | 데이터 흐름 관리 |
| **비동기** | UniTask | 비동기 작업 처리 |
| **애니메이션** | Spine 4.2 + LitMotion | 2D 애니메이션 및 트윈 |
| **UI** | UI Toolkit + MVVM | 사용자 인터페이스 |
| **AI** | Unity NavMesh 2D | 경로 탐색 |
| **데이터** | ScriptableObject | 게임 데이터 관리 |
| **렌더링** | URP + Sprite Atlas | 그래픽 및 최적화 |
| **현지화** | Unity Localization | 다국어 지원 |
| **플랫폼** | Steamworks.NET | Steam 연동 |

---

## 14세션 로드맵

| 주차 | 세션 | 주제 | 핵심 기술 |
|------|------|------|-----------|
| **Week 1** | 1 | 개발 환경 입문 | 엔트리 테스트, C# 문법, PlayerController, Git |
| | 2 | 실무 아키텍처 & LitMotion | Clean Architecture, VContainer DI, R3, UniTask |
| **Week 2** | 3 | Spine 애니메이션 | SkeletonAnimation, 상태 머신, LitMotion 블렌딩 |
| | 4 | 플레이어 컨트롤러 | Input System, ReactiveProperty, LitMotion 이동 |
| **Week 3** | 5 | 타일맵 & 레벨 디자인 | Rule Tile, ScriptableObject, LitMotion 패럴랙스 |
| | 6 | NavMesh 2D AI | NavMeshAgent, 추격/패트롤 상태 머신 |
| **Week 4** | 7 | 전투 시스템 | R3 Health, 히트박스/허트박스, LitMotion 넉백 |
| | 8 | 적 AI & 오브젝트 풀링 | UnityEngine.Pool, EnemyFactory 패턴 |
| **Week 5** | 9 | 능력 시스템 | 더블 점프, LitMotion 대시, 벽 점프, 능력 해금 UI |
| | 10 | 세이브/로드 & 현지화 | JSON 직렬화, 체크포인트, Unity Localization |
| **Week 6** | 11 | UI Toolkit & MVVM | UXML, USS, R3 데이터 바인딩, LitMotion UI 트윈 |
| | 12 | 메뉴 시스템 & Animator | 화면 전환, Animator Override Controller, 스킨 변경 |
| **Week 7** | 13 | 최적화 & 폴리싱 | Sprite Atlas, Draw Call, Frame Debugger, 보스 AI (2페이즈) |
| | 14 | Steam 출시 & CI/CD | Steamworks.NET, GitHub Actions, steamcmd 업로드 |

---

## Week 1: 개발 환경 & 아키텍처

**Session 1: 개발 환경 입문**

- 엔트리 테스트(진단 평가) 및 해설 설명
- C# 기본 문법과 클래스 구조
- Unity 에디터 워크플로우
- PlayerController와 Rigidbody2D 기반 기본 이동 구현
- Git init, commit, push 실습

**Session 2: 실무 아키텍처 & LitMotion**

- Clean Architecture 개념과 폴 구조
- VContainer를 활용한 의존성 주입 (DI)
- R3와 UniTask 설치 및 기초
- ScriptableObject로 데이터 관리
- LitMotion 트윈 기초

---

## Week 2: 캐릭터 & 애니메이션

**Session 3: Spine 애니메이션**

- Spine Unity 런타임 설치
- SkeletonAnimation 상태 머신
- 애니메이션 블렌딩과 전환
- LitMotion을 활용한 부드러운 블렌딩
- SpineView 컴포넌트 모듈화

**Session 4: 플레이어 컨트롤러**

- Input System 설정
- Input Action과 R3 ReactiveProperty 연동
- LitMotion으로 부드러운 이동 구현
- Raycast2D로 바닥 체크 및 점프
- Clean Architecture로 리팩토링

---

## Week 3: 레벨 & 네비게이션

**Session 5: 타일맵 & 레벨 디자인**

- 메트로배니아 레벨 디자인 원칙
- Rule Tile 설정 및 타일맵 구성
- ScriptableObject로 레벨 데이터 관리
- LitMotion 패럴랙스 효과
- 레벨 디자인 폴리싱

**Session 6: NavMesh 2D AI**

- Unity NavMesh 2D 설정
- NavMeshAgent로 경로 탐색
- NavMeshAgent 경로 탐색
- 추격/패트롤 AI 행동 구현
- 상태 머신 기반 적 AI 설계

---

## Week 4: 전투 & 오브젝트 관리

**Session 7: 전투 시스템**

- R3 Health 시스템
- DamageCalculator로 데미지 계산
- 히트박스/허트박스 구현
- LitMotion 넉백 효과
- CombatService DI 연동

**Session 8: 적 AI & 오브젝트 풀링**

- 적 종류 설계 (근접/원거리)
- UnityEngine.Pool로 오브젝트 풀링
- EnemySpawner와 풀링 연동
- EnemyFactory 패턴
- Profiler로 GC.Alloc 확인

---

## Week 5: 게임 시스템 & 현지화

**Session 9: 능력 시스템**

- 능력 시스템 설계
- 더블 점프 구현
- LitMotion 대시 공격
- 벽 점프 구현
- 능력 해금 UI

**Session 10: 세이브/로드 & 현지화**

- JSON 직렬화/역직렬화
- 체크포인트 자동 저장
- Unity Localization 패키지 설정
- 한국어/영어 다국어 지원
- 데이터 암호화 (XOR/AES-256)

---

## Week 6: UI & 애니메이션 심화

**Session 11: UI Toolkit & MVVM**

- UXML로 레이아웃 작성
- USS로 스타일링
- R3 데이터 바인딩
- LitMotion UI 트윈

**Session 12: 메뉴 시스템 & Animator**

- 화면 전환 관리
- Animator Override Controller로 스킨 변경
- 메뉴 시스템 구성
- 스킨 변경과 캐릭터 표현 확장

---

## Week 7: 최적화 & Steam 출시

**Session 13: 최적화 & 폴리싱**

- 렌더링 파이프라인 이해
- Sprite Atlas로 Draw Call 최적화
- Frame Debugger로 병목 분석
- SRP Batcher와 GPU Instancing
- 보스 AI 구현 (2페이즈)

**Session 14: Steam 출시 & CI/CD**

- Steamworks.NET 설정
- GitHub Actions CI/CD 구축
- steamcmd 업로드
- Steam 출시 자동화 파이프라인

---

## AI 협업 가이드

**Claude 3.5 Sonnet 4.6 활용**

이 강의는 AI 협업을 핵심으로 합니다. Claude는 다음 역할을 담당합니다.

| 역할 | 설명 | 예시 |
|------|------|------|
| **코드 생성** | 기능 구현 코드 작성 | "PlayerController를 작성해줘" |
| **코드 리뷰** | 개선점 제시 | "이 코드를 리뷰해줘" |
| **디버깅** | 오류 해결 | "에러 원인과 해결책 알려줘" |
| **설명** | 개념 설명 | "DI가 뭔지 설명해줘" |
| **최적화** | 성능 개선 | "이 코드를 최적화해줘" |

---

## AI 협업 핵심 원칙

**올바른 AI 협업 방법**

**✅ 해야 할 것**

- AI 코드를 **항상 테스트**하며 동작 검증
- AI에게 **코드 설명**을 요구하며 이해 확인
- **대안을 요청**하며 다양한 접근법 학습
- **에러 발생 시 충분한 컨텍스트** 제공

**❌ 하지 말아야 할 것**

- AI 코드를 **그대로 복사**하지 말고 이해하고 수정
- AI에게 **모든 것을 맡기지** 말고 핵심 로직은 직접 작성
- AI 답변을 **비판적으로 검토** (오류 가능성 있음)
- **복잡한 로직을 한 번에** 요청하지 말고 단계적으로 나누기

---

## 효과적인 프롬프트 패턴

**1. 코드 생성 프롬프트**

```
"[기능]를 구현하는 코드를 작성해줘.
조건: 
1) [조건1]
2) [조건2] 
3) [기술 스택] 사용"
```

**2. 코드 리뷰 프롬프트**

```
"이 코드를 리뷰해줘:
[코드 붙여넣기]

개선점을 3가지 제시해줘."
```

**3. 디버깅 프롬프트**

```
"[오류 메시지] 에러가 발생해.
[코드 붙여넣기]

원인과 해결책을 알려줘."
```

---

## 사전 준비사항

**필수 요구사항**

| 항목 | 사양 |
|------|------|
| **운영체제** | Windows 10/11, macOS 12+, Ubuntu 20.04+ |
| **CPU** | Intel i5-8400 / AMD Ryzen 5 2600 이상 |
| **RAM** | 16GB 이상 (32GB 권장) |
| **GPU** | DirectX 11/OpenGL 4.5 지원 |
| **저장공간** | 50GB 이상 여유 공간 |

**설치 소프트웨어**

- Unity Hub + Unity 6.3 LTS
- Visual Studio 2022 또는 JetBrains Rider
- Git
- Claude Code (CLI) 또는 claude.ai 계정

---

## 권장 선수지식

**필수는 아니지만 도움이 되는 내용**

- **프로그래밍**: 기본적인 프로그래밍 개념 (변수, 함수, 조걸문)
- **수학**: 중학교 수준의 좌표계와 벡터 개념
- **게임**: 메트로배니아/플랫포머 게임 경험
- **영어**: 기본적인 프로그래밍 용어 이해

**초보자도 가능합니다**

이 강의는 C#과 Unity를 처음 접하는 분들도 따라갈 수 있도록 설계되었습니다.

---

## 수료 기준 및 성과물

**수료 기준**

| 항목 | 기준 |
|------|------|
| **출석률** | 80% 이상 (12/14 세션) |
| **과제 제출** | 주당 실습 과제 80% 이상 완료 |
| **프로젝트** | Steam 출시 가능한 게임 완성 |
| **발표** | 최종 프로젝트 시연 및 발표 |

**최종 성과물**

- Steam 출시 품질의 메트로배니아 게임
- GitHub 포트폴리오 (완성된 프로젝트)
- Clean Architecture 기반의 확장 가능한 코드베이스
- AI 협업 능력 (Claude 3.5 Sonnet 활용)

---

## 세션별 핵심 키워드

| 세션 | 핵심 키워드 |
|------|------------|
| 1 | `class`, `Rigidbody2D`, `velocity`, `git commit` |
| 2 | `VContainer`, `ScriptableObject`, `LitMotion`, `DI` |
| 3 | `Spine`, `SkeletonAnimation`, `LitMotion 블렌딩` |
| 4 | `New Input System`, `ReactiveProperty`, `LitMotion Move` |
| 5 | `Tilemap`, `ScriptableObject`, `LitMotion 패럴택스` |
| 6 | `NavMesh`, `NavMeshAgent`, `LitMotion 회전` |
| 7 | `Health`, `ReactiveProperty`, `LitMotion 넉백` |
| 8 | `ObjectPool`, `IObjectPool`, `Factory Pattern` |
| 9 | `Ability`, `LitMotion Dash`, `AbilityManager` |
| 10 | `JsonConvert`, `Localization`, `SaveService` |
| 11 | `UI Toolkit`, `MVVM`, `LitMotion UI` |
| 12 | `ScreenManager`, `LitMotion 전환`, `Animator Override` |
| 13 | `Sprite Atlas`, `Draw Call`, `Frame Debugger` |
| 14 | `Steamworks`, `BuildPipeline`, `CLI`, `CI/CD` |

---

## 강의 진행 방식

**세션 구성 (3시간)**

| 시간 | 활동 | 비고 |
|------|------|------|
| 0-30분 | 이론 강의 | 개념 설명 및 데모 |
| 30-60분 | AI 협업 실습 | Claude와 함께 코드 작성 |
| 60-90분 | 개별 실습 | 학생 직접 구현 |
| 90-150분 | 심화 실습 | 고급 기능 구현 |
| 150-180분 | 리뷰 및 과제 | 코드 리뷰, 다음 세션 예고 |

---

<!-- _class: lead -->

# 시작하기

## Unity 실무 중심 메트로배니아 게임 개발

**Claude 3.5 Sonnet과 함께하는 AI 협업 개발 여정**

Steam 출시를 향해 함께 성장합시다!

---

## 연락처 및 리소스

**강의 자료 및 커뮤니티**

- **강의 자료**: GitHub Repository
- **질문/답변**: Discord 커뮤니티
- **AI 도구**: Claude Code / claude.ai
- **버전 관리**: GitHub

**참고 자료**

- Unity Documentation: docs.unity3d.com
- Spine Documentation: esotericsoftware.com/spine-unity
- VContainer: github.com/hadashiA/VContainer
- R3: github.com/Cysharp/R3
- LitMotion: github.com/AnnulusGames/LitMotion

---

**문서 버전**: 1.0

**최종 업데이트**: 2026-03-04

**만든이**: Unity 실무 중심 메트로배니아 게임 개발팀
