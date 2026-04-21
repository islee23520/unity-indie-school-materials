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
  .checklist {
    color: #16c79a;
  }
---

<!-- _class: lead -->

# 실무 아키텍처 & LitMotion

## Session 2: 실무 아키텍처 & LitMotion

클린 아키텍처부터 트윈 애니메이션까지

---

## 오늘의 학습 목표

**3시간 동안 배울 내용**

| 시간 | 주제 | 활동 |
|------|------|------|
| 0-30분 | Clean Architecture | 아키텍처 패턴 이해 |
| 30-60분 | 폴터 구조 | 프로젝트 조직화 |
| 60-90분 | VContainer DI | 의존성 주입 구현 |
| 90-120분 | R3, UniTask | 리액티브 프로그래밍 |
| 120-150분 | ScriptableObject | 데이터 관리 |
| 150-180분 | LitMotion 기초 | 트윈 애니메이션 |

---

## Claude Sonnet 4.6 활용 가이드

**AI 협업 핵심 원칙**

- **Claude Code** 또는 **claude.ai** 사용
- 학생이 **프롬프트**를 직접 작성하며 AI와 대화
- AI는 **코드 생성/리뷰/최적화** 보조 도구
- 모든 결과물은 **이해하고 설명**할 수 있어야 함

---

## 효과적인 프롬프트 작성법

**Claude에게 질문할 때**

```
"Unity Clean Architecture folder structure를
설계해줘. VContainer DI를 사용하는 구조로."

"PlayerDataSO ScriptableObject 설계해줘.
요구사항:
1) 체력, 공격력, 이동 속도 필드
2) 저장/불러오기 메서드
3) 이벤트 기반 변경 알림"

"LitMotion으로 기본 트윈 예제 코드 보여줘.
이동, 회전, 스케일 각각 구현."
```

---

## Clean Architecture 개요 (0-30분)

**왜 아키텍처가 중요한가?**

- **유지보수성**: 변경이 쉬운 코드 구조
- **테스트 용이성**: 독립적인 컴포넌트 테스트
- **협업 효율**: 명확한 책임 분리
- **확장성**: 새 기능 추가가 용이

---

## Clean Architecture 레이어

**핵심 원칙: Dependency Rule**

```
┌─────────────────────────────────────┐
│  Presentation Layer (UI, Input)     │
├─────────────────────────────────────┤
│  Application Layer (Use Cases)      │
├─────────────────────────────────────┤
│  Domain Layer (Entities, Services)  │
├─────────────────────────────────────┤
│  Infrastructure (Data, External)    │
└─────────────────────────────────────┘
       ↓ 의존 방향 (낮은 레벨로)
```

---

## Unity에서의 Clean Architecture

**실무 적용 패턴**

| 레이어 | Unity 구현 | 책임 |
|--------|-----------|------|
| **Presentation** | MonoBehaviour, UI | 입력 처리, 화면 표시 |
| **Application** | Presenter, Controller | 유스케이스 조율 |
| **Domain** | Service, Entity | 비즈니스 로직 |
| **Infrastructure** | Repository, Data | 데이터 접근 |

---

## AI 실습: Clean Architecture 설계

**Claude에게 요청할 프롬프트**

```
"Unity Clean Architecture folder structure를
설계해줘.

요구사항:
- Runtime/ 폴터 구조
- 각 레이어의 책임 설명
- VContainer DI 연동 방식
- Assembly Definition 구성"
```

**Claude가 생성한 구조를 분석하고 수정핵시오.**

---

## 프로젝트 폴터 구조 (30-60분)

**권장 폴터 구조**

```
Assets/
├── _Project/
│   ├── Runtime/
│   │   ├── Presentation/    # UI, Input, View
│   │   ├── Application/     # Use Cases
│   │   ├── Domain/          # Entities, Interfaces
│   │   └── Infrastructure/  # Data, External
│   ├── Editor/              # Editor Tools
│   └── Tests/               # Unit Tests
├── Plugins/                 # Third-party
└── StreamingAssets/         # Runtime Data
```

---

## 폴터 구조 실제 예시

**Runtime 낶부 구조**

```
Runtime/
├── Presentation/
│   ├── UI/                  # UI screens
│   ├── Input/               # Input handlers
│   └── Views/               # Visual components
├── Application/
│   ├── Game/                # Game flow
│   └── Services/            # App services
├── Domain/
│   ├── Entities/            # Player, Enemy, etc.
│   ├── Interfaces/          # Service contracts
│   └── ValueObjects/        # Data structures
└── Infrastructure/
    ├── Data/                # Save/Load
    └── External/            # API, Ads
```

---

## AI 실습: 폴터 구조 생성

**Claude에게 요청할 프롬프트**

```
"Unity 메트로배니아 게임의
Clean Architecture 폴터 구조를 만들어줘.

포함할 내용:
1) Presentation (UI, Input)
2) Application (GameManager)
3) Domain (Player, Score)
4) Infrastructure (SaveSystem)

각 폴터의 역할 설명도 함께."
```

**생성된 구조를 자신의 프로젝트에 적용하세요.**

---

## VContainer DI 기초 (60-90분)

**의존성 주입이란?**

- **Dependency**: 객체가 필요로 하는 다른 객체
- **Injection**: 필요한 객체를 외부에서 제공
- **Inversion of Control**: 객체가 의존성을 직접 생성하지 않음

```csharp
// 나쁜 예: 직접 생성
public class Player : MonoBehaviour {
    private Weapon weapon = new Weapon();  // 강한 결합
}

// 좋은 예: 주입받음
public class Player : MonoBehaviour {
    [Inject] private IWeapon weapon;  // 느슨한 결합
}
```

---

## VContainer 설치와 설정

**Package Manager 설치**

```json
// manifest.json
{
  "dependencies": {
    "jp.hadashikick.vcontainer": "1.16.0"
  }
}

// 또는 Scoped Registry
Name: package.openupm.com
URL: https://package.openupm.com
Scopes: jp.hadashikick.vcontainer
```

**LifetimeScope 설정**

```csharp
public class GameLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<PlayerService>(Lifetime.Singleton);
        builder.Register<ScoreManager>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GamePresenter>();
    }
}
```

---

## VContainer 사용 예시

**서비스 등록과 주입**

```csharp
// Domain Interface
public interface IScoreService {
    int CurrentScore { get; }
    void AddScore(int points);
    event Action<int> OnScoreChanged;
}

// Implementation
public class ScoreService : IScoreService {
    public int CurrentScore { get; private set; }
    public event Action<int> OnScoreChanged;
    
    public void AddScore(int points) {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}
```

---

## VContainer 주입 패턴

**Constructor Injection (권장)**

```csharp
public class PlayerPresenter : IStartable {
    private readonly IScoreService _scoreService;
    private readonly PlayerView _view;
    
    [Inject]
    public PlayerPresenter(IScoreService scoreService, PlayerView view) {
        _scoreService = scoreService;
        _view = view;
    }
    
    public void Start() {
        _scoreService.OnScoreChanged += _view.UpdateScore;
    }
}
```

---

## AI 실습: VContainer 설정

**Claude에게 요청할 프롬프트**

```
"package.json에 VContainer, R3, UniTask, LitMotion을
추가하는 방법을 알려줘.

요구사항:
- manifest.json 형식
- Scoped Registry 설정
- 버전 정보 포함"
```

**Claude의 제안을 하나씩 적용핵시오.**

---

## R3, UniTask 설정 (90-120분)

**리액티브 프로그래밍이란?**

- **R3**: Unity용 Reactive Extensions (Observable)
- **UniTask**: GC-free async/await
- **조합**: 비동기 + 리액티브 = 강력한 게임 로직

```json
// manifest.json dependencies
{
  "jp.hadashikick.vcontainer": "1.16.0",
  "com.cysharp.r3": "1.1.0",
  "com.cysharp.unitask": "2.5.0",
  "jp.ayutaz.litmotion": "1.0.0"
}
```

---

## R3 기본 사용법

**Observable 패턴**

```csharp
using R3;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    private ReactiveProperty<int> _health = new(100);
    
    public ReadOnlyReactiveProperty<int> Health => _health;
    
    private void Start() {
        // 구독
        Health
            .Where(h => h <= 0)
            .Subscribe(_ => Die())
            .AddTo(this);
            
        // 값 변경
        _health.Value -= 10;
    }
}
```

---

## UniTask 기본 사용법

**GC-free 비동기**

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameStarter : MonoBehaviour {
    private async UniTaskVoid Start() {
        // 딜레이
        await UniTask.Delay(1000);  // 1초
        
        // 조건 대기
        await UniTask.WaitUntil(() => _isReady);
        
        // 취소 토큰
        var cts = new CancellationTokenSource();
        await UniTask.Delay(5000, cancellationToken: cts.Token);
    }
}
```

---

## R3 + UniTask 조합

**강력한 비동기 리액티브**

```csharp
public class GameTimer : MonoBehaviour {
    [SerializeField] private float _gameTime = 60f;
    
    private async UniTaskVoid Start() {
        var timer = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .TakeUntilDestroy(this)
            .Subscribe(_ => {
                _gameTime--;
                UpdateUI(_gameTime);
            });
            
        await UniTask.WaitUntil(() => _gameTime <= 0);
        GameOver();
    }
}
```

---

## AI 실습: R3 UniTask 예제

**Claude에게 요청할 프롬프트**

```
"R3와 UniTask를 사용한
Unity 게임 타이머 예제를 작성해줘.

요구사항:
1) 60초 카운트다운
2) 1초마다 UI 업데이트 (R3)
3) 타이머 종료 시 이벤트 (UniTask)
4) 취소 가능한 구조"
```

**Claude의 코드를 분석하고 수정핵시오.**

---

## ScriptableObject 기초 (120-150분)

**ScriptableObject란?**

- **데이터 에셋**: 인스턴스가 아닌 에셋으로 저장
- **메모리 효율**: 여러 오브젝트가 공유 가능
- **런타임 수정**: 플레이 중 변경 가능 (에디터)
- **버전 관리**: 텍스트 기반, Git 친화적

```csharp
[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject {
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public Sprite characterSprite;
    public AnimationCurve expCurve;
}
```

---

## PlayerDataSO 설계

**실제 사용 예시**

```csharp
[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject {
    [Header("Base Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public float attackPower = 10f;
    
    [Header("Visual")]
    public Sprite characterSprite;
    public Color characterColor = Color.white;
}
```

---

## ScriptableObject 사용

**참조와 인스턴스**

```csharp
public class Player : MonoBehaviour {
    [SerializeField] private PlayerDataSO _data;
    
    private int _currentHealth;
    
    private void Start() {
        _currentHealth = _data.maxHealth;
        // _data는 에셋 참조, 복사하지 않음
    }
}
```

---

## AI 실습: PlayerDataSO 설계

**Claude에게 요청할 프롬프트**

```
"PlayerDataSO ScriptableObject를 설계해줘.

요구사항:
1) 체력, 공격력, 이동 속도 필드
2) 능력치 강화 메서드
3) 능력치 성장 곡선
4) 저장/불러오기 인터페이스

Unity 6.3 기준으로 작성."
```

**생성된 코드의 장단점을 분석하세요.**

---

## LitMotion 기초 (150-180분)

**LitMotion이란?**

- **경량 트윈 라이브러리**: DOTS 기반 고성능
- **GC-free**: 메모리 할당 최소화
- **체인 가능**: Fluent API
- **Unity 6.3 최적화**: Burst Compiler 지원

```csharp
using LitMotion;
using LitMotion.Extensions;

// 기본 이동
LMotion.Create(Vector3.zero, Vector3.one, 1f)
    .BindToPosition(transform);

// 체인
LMotion.Create(0f, 1f, 0.5f)
    .WithEase(Ease.OutQuad)
    .WithDelay(0.2f)
    .WithOnComplete(() => Debug.Log("Done!"))
    .BindToScale(transform);
```

---

## LitMotion 기본 트윈

**주요 트윈 타입**

```csharp
using LitMotion;
using LitMotion.Extensions;

public class TweenExamples : MonoBehaviour {
    void Start() {
        // 위치 이동
        LMotion.Create(transform.position, targetPos, 1f)
            .BindToPosition(transform);
            
        // 회전
        LMotion.Create(0f, 360f, 2f)
            .BindToLocalRotationY(transform);
            
        // 스케일
        LMotion.Create(Vector3.one, Vector3.one * 2f, 0.5f)
            .WithEase(Ease.OutBack)
            .BindToLocalScale(transform);
            
        // 값 변경
        LMotion.Create(0f, 100f, 1f)
            .Bind(x => Debug.Log($"Progress: {x}%"));
    }
}
```

---

## LitMotion 체이닝

**복합 애니메이션**

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ComplexTween : MonoBehaviour {
    void Start() => RunSequenceAsync().Forget();

    private async UniTask RunSequenceAsync() {
        // 순차 실행
        await LMotion.Create(Vector3.zero, Vector3.up, 0.5f)
            .BindToPosition(transform);
            
        await LMotion.Create(0f, 360f, 1f)
            .BindToLocalRotationY(transform);
            
        // 동시 실행
        var tween1 = LMotion.Create(transform.localScale, Vector3.one * 1.5f, 0.3f)
            .BindToLocalScale(transform);
            
        var tween2 = LMotion.Create(0f, 1f, 0.3f)
            .Bind(alpha => _spriteRenderer.color = new Color(1, 1, 1, alpha));
            
        await LMotion.WaitAll(tween1, tween2);
    }
}
```

---

## LitMotion Easing

**이징 함수 활용**

```csharp
LMotion.Create(startValue, endValue, duration)
    .WithEase(Ease.OutBack)      // 튕기는 효과
    .WithEase(Ease.InOutCubic)   // 부드러운 가감속
    .WithEase(Ease.OutElastic)   // 탄성 효과
    .WithEase(Ease.InExpo)       // 급가속
    
    // 커스텀 커브
    .WithEase(AnimationCurve.EaseInOut(0, 0, 1, 1))
    
    .BindToPosition(transform);
```

---

## AI 실습: LitMotion 예제

**Claude에게 요청할 프롬프트**

```
"LitMotion으로 기본 트윈 예제를 작성해줘.

요구사항:
1) 이동 트윈 (Vector3)
2) 회전 트윈 (float angle)
3) 스케일 트윈 (Vector3)
4) 각각 OutBack, OutElastic 이징 적용
5) 체이닝으로 순차 실행

Unity 6.3 + LitMotion 1.0.0 기준"
```

**Claude의 코드를 실행하고 결과를 확인하세요.**

---

## 오늘 배운 핵심 키워드

**Clean Architecture**
- Layer, Dependency Rule, Presentation, Domain, Infrastructure

**VContainer DI**
- LifetimeScope, Register, Inject, Constructor Injection

**R3, UniTask**
- Observable, ReactiveProperty, async/await, CancellationToken

**ScriptableObject**
- CreateAssetMenu, SerializeField, Shared Data

**LitMotion**
- LMotion, BindToPosition, Ease, Chain

---

## package.json 예시

**의존성 관리**

```json
{
  "dependencies": {
    "com.unity.modules.ui": "1.0.0",
    "jp.hadashikick.vcontainer": "1.16.0",
    "com.cysharp.r3": "1.1.0",
    "com.cysharp.unitask": "2.5.0",
    "jp.ayutaz.litmotion": "1.0.0"
  },
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "jp.hadashikick.vcontainer",
        "com.cysharp.r3",
        "com.cysharp.unitask",
        "jp.ayutaz.litmotion"
      ]
    }
  ]
}
```

---

## 체크리스트

**Session 2 완료 기준**

- [ ] Clean Architecture 4계층을 설명할 수 있다
- [ ] VContainer로 서비스를 등록하고 주입할 수 있다
- [ ] R3 Observable을 구독하고 이벤트를 처리할 수 있다
- [ ] UniTask로 비동기 작업을 작성할 수 있다
- [ ] PlayerDataSO를 생성하고 데이터를 관리할 수 있다
- [ ] LitMotion으로 기본 트윈을 구현할 수 있다

**자기 점검**
- [ ] AI에게 질문하고 답변을 코드로 구현했는가?
- [ ] 각 라이브러리의 역할을 설명할 수 있는가?
- [ ] 프로젝트에 패키지를 설치하고 설정했는가?

---

## 과제: 집에서 복습하기

**1. 아키텍처 실습**

```csharp
// 과제: 간단한 서비스를 DI로 구현
// - IGameService 인터페이스
// - GameService 구현체
// - VContainer 등록
// - MonoBehaviour에서 주입받아 사용
```

**2. AI 협업 과제**
- Claude에게 "Unity Clean Architecture vs MVVM" 비교 요청
- 예제 코드를 실행하고 결과 캡처

**3. LitMotion 과제**
- UI 버튼 클릭 시 스케일 애니메이션 구현
- 점수 변경 시 숫자 카운트업 애니메이션 구현

---

## AI 프롬프트 모음

**이번 세션에서 사용한 프롬프트**

```
1. "Unity Clean Architecture folder structure"
2. "package.json for VContainer, R3, UniTask, LitMotion"
3. "PlayerDataSO ScriptableObject design"
4. "LitMotion basic tween examples"
```

**확장 프롬프트**
```
5. "VContainer LifetimeScope best practices"
6. "R3 Observable vs Unity Event comparison"
7. "UniTask cancellation pattern examples"
8. "LitMotion sequence and parallel tweens"
```

---

<!-- _class: lead -->

## Session 2 완료!

**다음 시간 예고:** Spine 애니메이션 — SkeletonAnimation 상태 머신·LitMotion 블렌딩

### 질문 있으신가요?

Claude Sonnet 4.6과 함께 계속 공부합시다.
