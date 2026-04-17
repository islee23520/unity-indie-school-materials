# Slide Code Sample Management

This directory contains the canonical C# code samples used in the 14-session Metroidvania course. These samples are the stable source referenced by the markdown-first course materials so session examples stay consistent across docs and slides.

## Tech Stack Context
All samples follow the course's production-ready tech stack:
- **Engine**: Unity 6.3 LTS
- **Architecture**: Clean Architecture + VContainer (DI)
- **Reactive/Async**: R3 + UniTask
- **Animation/Tween**: Spine 4.2 + LitMotion
- **UI**: UI Toolkit + MVVM

## Directory Structure
The `sample-code/` directory is organized by session (1-14). Each folder contains the session-scoped sample files that course docs and slide sources can reference directly.

```
sample-code/
├── session1/              # Week 1: 개발 환경 입문
├── session2/              # Week 1: 실무 아키텍처 & LitMotion
├── session3/              # Week 2: Spine 애니메이션
├── session4/              # Week 2: 플레이어 컨트롤러
├── session5/              # Week 3: 타일맵 & 레벨 디자인
├── session6/              # Week 3: NavMesh 2D AI
├── session7/              # Week 4: 전투 시스템
├── session8/              # Week 4: 적 AI & 오브젝트 풀링
├── session9/              # Week 5: 능력 시스템
├── session10/             # Week 5: 세이브/로드 & 현지화
├── session11/             # Week 6: UI Toolkit & MVVM
├── session12/             # Week 6: 메뉴 시스템 & Animator
├── session13/             # Week 7: 최적화 & 폴리싱
└── session14/             # Week 7: Steam 출시 & CI/CD
```

## Session-to-Keyword Coverage Matrix

| Session | Topic | Key Keywords / Technologies |
|:---|:---|:---|
| 1 | 개발 환경 입문 | `class`, `Rigidbody2D`, `velocity`, `git init`, `git commit`, `git push` |
| 2 | 실무 아키텍처 & LitMotion | `VContainer`, `R3`, `UniTask`, `Clean Architecture` |
| 3 | Spine 애니메이션 | `SkeletonAnimation`, `AnimationState`, `LitMotion 블렌딩` |
| 4 | 플레이어 컨트롤러 | `Input System`, `ReactiveProperty`, `LitMotion 이동` |
| 5 | 타일맵 & 레벨 디자인 | `Rule Tile`, `ScriptableObject`, `LitMotion 패럴랙스` |
| 6 | NavMesh 2D AI | `NavMeshAgent`, `경로 탐색`, `추격/패트롤 상태 머신` |
| 7 | 전투 시스템 | `R3 Health`, `히트박스`, `허트박스`, `LitMotion 넉백` |
| 8 | 적 AI & 오브젝트 풀링 | `UnityEngine.Pool`, `EnemyFactory`, `Object Pooling` |
| 9 | 능력 시스템 | `더블 점프`, `LitMotion 대시`, `벽 점프`, `능력 해금 UI` |
| 10 | 세이브/로드 & 현지화 | `JSON 직렬화`, `체크포인트`, `Unity Localization` |
| 11 | UI Toolkit & MVVM | `UXML`, `USS`, `R3 데이터 바인딩`, `LitMotion UI 트윈` |
| 12 | 메뉴 시스템 & Animator | `ScreenManager`, `Animator Override Controller`, `스킨 변경` |
| 13 | 최적화 & 폴리싱 | `Sprite Atlas`, `Draw Call`, `Frame Debugger`, `보스 AI (2페이즈)` |
| 14 | Steam 출시 & CI/CD | `Steamworks.NET`, `GitHub Actions`, `steamcmd 업로드` |

## Usage Guidelines

### 1. Authoring Samples
- Create `.cs` files in the corresponding `sessionX/` folder.
- Use **Allman style** (braces on new lines).
- Use **4-space indentation**.
- Ensure samples are valid C# (even if they are snippets) to leverage IDE tooling.

### 2. Integration
Keep session folders stable so curriculum docs and markdown slide sources can point to a single canonical path.
- **Canonical path**: `sample-code/sessionX/`
- **Reference target**: `slides/` and `docs/` should point here when they need session-specific Unity examples.

## Code Style Standards
```csharp
public class SampleComponent : MonoBehaviour
{
    [Inject] private readonly IMessageService _messageService;

    public void HandleAction()
    {
        if (isValid)
        {
            _messageService.Show("Action Triggered");
        }
    }
}
```
