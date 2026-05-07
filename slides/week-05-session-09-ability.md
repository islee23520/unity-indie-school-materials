---
marp: true
theme: default
color: dark
class:
  - invert
paginate: true
style: |
  section {
    font-size: 28px;
  }
  h1 {
    font-size: 48px;
    color: #61dafb;
  }
  h2 {
    font-size: 36px;
    color: #ff6b6b;
  }
  code {
    font-size: 18px;
  }
  pre {
    font-size: 16px;
  }
---

# Session 9: 능력 시스템

**VContainer + R3 + MonoBehaviour/Clean Architecture**

![bg right:40%](https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800)

---

## 목차

1. 능력 시스템 아키텍처
2. 더블 점프 구현
3. LitMotion 대시 구현
4. AI 프롬프트 예시

---

## 능력 시스템 개요

메트로배니아 능력 시스템의 핵심:

- **능력(Ability)**: 플레이어가 해금해 사용하는 액션
- **상태 체크**: 잠금/해금, 사용 가능 여부 판단
- **쿨타임**: 재사용 대기 시간 관리
- **연계(Combo)**: 점프 + 대시 같은 조합 설계

```csharp
public enum AbilityType
{
    DoubleJump,
    Dash,
    WallJump,
    DashAttack
}
```

---

## 기준 아키텍처 (세션 2와 동일)

기본 경로는 아래 조합으로 통일합니다.

- **MonoBehaviour + ScriptableObject**: 런타임 로직 + 데이터 자산
- **VContainer**: 서비스/유스케이스 DI
- **R3**: UI/상태 반응형 바인딩
- **Clean Architecture**: Domain / Application / Presentation 분리

```csharp
public sealed class AbilityLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<AbilityService>(Lifetime.Singleton);
        builder.Register<AbilityCooldownService>(Lifetime.Singleton);
        builder.RegisterEntryPoint<PlayerAbilityPresenter>();
    }
}
```

---

## 데이터 모델 구성

```csharp
[CreateAssetMenu(menuName = "Game/Ability/AbilityConfig")]
public class AbilityConfig : ScriptableObject
{
    public AbilityType abilityType;
    public float cooldown;
    public float duration;
    public bool unlockedByDefault;
}

[Serializable]
public class PlayerAbilityState
{
    public AbilityType type;
    public bool unlocked;
    public float lastUsedTime;
}
```

---

## AbilityService (Application 레이어)

```csharp
using R3;

public sealed class AbilityService
{
    private readonly Dictionary<AbilityType, PlayerAbilityState> _states;
    public ReactiveProperty<AbilityType?> CurrentAbility { get; } = new(null);

    public AbilityService(IEnumerable<AbilityConfig> configs)
    {
        _states = configs.ToDictionary(
            c => c.abilityType,
            c => new PlayerAbilityState {
                type = c.abilityType,
                unlocked = c.unlockedByDefault,
                lastUsedTime = -999f
            });
    }

    public bool IsUnlocked(AbilityType type) => _states[type].unlocked;
    public void Unlock(AbilityType type) => _states[type].unlocked = true;
}
```

---

## 더블 점프 설계

요구사항:

- 기본 점프 1회 + 공중 점프 1회
- 착지 시 점프 횟수 리셋
- 능력 미해금 상태에서는 단일 점프만 허용

```csharp
public class PlayerJumpController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;

    private int _jumpCount;
    private int _maxJumpCount = 1;
}
```

---

## 더블 점프 구현 코드

Update 기반 구현과 R3 구독 기반 구현을 함께 비교합니다.

### Update() 기반

```csharp
using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    // 필드 생략

    private int _jumpCount;

    public void SetDoubleJumpUnlocked(bool unlocked)
    {
        _maxJumpCount = unlocked ? 2 : 1;
    }

    private void Update()
    {
        if (IsGrounded())
        {
            _jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount < _maxJumpCount)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        _jumpCount++;
    }

    private bool IsGrounded()
        => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
}
```

### R3 구독 기반

```csharp
using R3;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    // 필드 생략

    private enum JumpState
    {
        Grounded,
        Airborne
    }

    private readonly ReactiveProperty<JumpState> _jumpState = new(JumpState.Grounded);
    private readonly ReactiveProperty<int> _jumpCount = new(0);

    public void SetDoubleJumpUnlocked(bool unlocked)
    {
        _maxJumpCount = unlocked ? 2 : 1;
    }

    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => TransitionTo(IsGrounded() ? JumpState.Grounded : JumpState.Airborne))
            .AddTo(this);

        _jumpState
            .DistinctUntilChanged()
            .Where(state => state == JumpState.Grounded)
            .Subscribe(_ => _jumpCount.Value = 0)
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Where(_ => CanJump())
            .Subscribe(_ => Jump())
            .AddTo(this);
    }

    private bool CanJump()
    {
        if (_jumpState.Value == JumpState.Grounded)
        {
            return true;
        }

        return _jumpState.Value == JumpState.Airborne && _jumpCount.Value < _maxJumpCount;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        _jumpCount.Value++;
        TransitionTo(JumpState.Airborne);
    }

    private void TransitionTo(JumpState nextState)
    {
        _jumpState.Value = nextState;
    }

    private bool IsGrounded()
        => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
}
```

---

## 대시 설계

핵심 규칙:

- Shift 입력으로 대시
- 대시 중 입력 잠금(짧은 시간)
- 쿨타임 경과 시 재사용
- 필요 시 대시 공격으로 확장

```csharp
[Serializable]
public class DashSettings
{
    public float distance = 5f;
    public float duration = 0.3f;
    public float cooldown = 2f;
}
```

---

## LitMotion 대시 구현

https://annulusgames.github.io/LitMotion/

```csharp
using LitMotion;
using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    [SerializeField] private DashSettings dash;
    private bool _isDashing;
    private float _lastDashTime = -999f;

    public float distance = 5f;
    public float duration = 0.3f;
    public float cooldown = 2f;

    public void Start()
    {
        TryDash(Vector2.right); // 예시: 오른쪽으로 대시
    }

    public bool TryDash(Vector2 direction)
    {
        if (_isDashing) return false;
        if (Time.time - _lastDashTime < cooldown) return false;

        var start = (Vector2)transform.position;
        var end = start + direction.normalized * distance;
        _isDashing = true;
        _lastDashTime = Time.time;

        LMotion.Create(start, end, duration)
            .WithOnComplete(() => _isDashing = false)
            .WithEase(Ease.OutCubic)
            .Bind(value => transform.position = value)
            .AddTo(gameObject);

        return true;
    }
}
```

---

## R3로 UI 상태 연결

```csharp
using R3;

public sealed class PlayerAbilityPresenter : IStartable, IDisposable
{
    private readonly AbilityService _abilityService;
    private readonly CompositeDisposable _disposables = new();

    public PlayerAbilityPresenter(AbilityService abilityService)
    {
        _abilityService = abilityService;
    }

    public void Start()
    {
        _abilityService.CurrentAbility
            .Subscribe(ability => Debug.Log($"현재 능력: {ability}"))
            .AddTo(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}
```

---

## 능력 해금 UX (메트로배니아)

- 보스 처치/유물 획득 시 능력 해금
- 해금 연출: 팝업 + 입력 잠금 + 카메라 포커스
- 저장 데이터와 즉시 동기화

```csharp
public void OnRelicCollected(AbilityType unlocked)
{
    _abilityService.Unlock(unlocked);
    _saveService.MarkAbilityUnlocked(unlocked);
    _uiService.ShowAbilityUnlock(unlocked);
}
```

---

## AI 프롬프트: 더블 점프

```
Unity 6.3 LTS에서 MonoBehaviour 기반 더블 점프를 구현해줘.

기술 스택:
- VContainer + R3 + Clean Architecture

요구사항:
1. 착지 시 점프 카운트 리셋
2. 능력 해금 전 1회, 해금 후 2회 점프
3. Rigidbody2D 기반 점프
4. 지면 체크는 OverlapCircle 사용
5. 테스트 가능한 메서드 분리

출력:
- PlayerJumpController
- AbilityService 연동 코드
```

---

## AI 프롬프트: LitMotion 대시

```
LitMotion으로 메트로배니아 플레이어 대시를 구현해줘.

조건:
1. Shift 입력으로 대시
2. 쿨타임 2초
3. 대시 거리/지속시간은 ScriptableObject 설정값 사용
4. 대시 중 피격 판정 끄기(옵션)
5. 완료 시 원래 상태 복구

출력:
- PlayerDashController (MonoBehaviour)
- DashSettings (ScriptableObject)
- 간단한 단위 테스트 아이디어
```

---

## AI 프롬프트: 능력 해금 플로우

```
메트로배니아의 능력 해금 플로우를 설계해줘.

아키텍처:
- Domain: AbilityType, PlayerAbilityState
- Application: AbilityService, SaveService
- Presentation: AbilityUnlockView, HUDPresenter (R3)

요구사항:
1. 유물 획득 시 능력 해금
2. 저장/로드 시 해금 상태 복원
3. UI에 "NEW" 배지 표시
4. 코드 책임 분리를 명확히 설명
```

---

## 과제

1. **기본 과제**: 더블 점프 구현
   - MaxJumpCount = 2 (해금 후)
   - 두 번째 점프는 JumpForce * 0.8

2. **심화 과제**: LitMotion 대시
   - 거리: 5f
   - 지속시간: 0.3f
   - 쿨타임: 2f

3. **도전 과제**: 능력 조합
   - 대시 + 점프 = 슈퍼 점프
   - 대시 + 공격 = 대시 어택

---

## 정리

이번 세션에서 배운 내용:

- MonoBehaviour/Clean Architecture 기반 능력 시스템 설계
- VContainer로 AbilityService 구성
- R3로 능력 상태를 UI에 반영
- LitMotion으로 부드러운 대시 연출
- AI 프롬프트로 구현 가속화

**핵심**: 능력 로직은 서비스로 분리하고, 캐릭터 제어는 MonoBehaviour로 단순하고 명확하게 유지한다.

---

## 참고 자료

- VContainer: https://github.com/hadashiA/VContainer
- R3: https://github.com/Cysharp/R3
- LitMotion: https://github.com/AnnulusGames/LitMotion

---

# Q&A

**질문 있으신가요?**

![bg right:40%](https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800)

---

## 다음 세션 예고

**Session 10: 세이브/로드 + Unity Localization**

- 체크포인트 저장
- JSON 직렬화
- 다국어 UI 문자열 관리
- AI 현지화 워크플로우

**Coming Soon!**
