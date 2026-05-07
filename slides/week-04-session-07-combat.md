---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
---

<style>
section {
  font-family: 'Noto Sans KR', 'Segoe UI', sans-serif;
}
h1, h2, h3 {
  color: #ff6b6b;
}
code {
  background-color: #2d2d44;
  padding: 2px 6px;
  border-radius: 4px;
}
pre {
  background-color: #2d2d44;
  padding: 16px;
  border-radius: 8px;
}
.highlight {
  color: #ffd93d;
}
.keyword {
  color: #6bcf7f;
  font-weight: bold;
}
</style>

<!-- _class: lead -->
# ⚔️ Session 7: 전투 시스템

## Health system부터 넉백까지, 완성도 있는 전투 구현

---

# 📚 이번 세션 목표

1. **Health 시스템** - 체계적인 생명력 관리
2. **Damage 계산** - 데미지 공식과 방어력
3. **Hitbox 시스템** - 충돌 감지와 공격 판정
4. **LitMotion 넉백** - 물리 기반 반동 효과
5. **실전 적용** - 코드 예제와 AI 프롬프트

---

<!-- _class: lead -->
# Part 1: Health 시스템 설계

---

# Health 컴포넌트 기초

**단일 책임 원칙**: Health는 생명력만 관리

```csharp
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    // 이벤트: 다른 시스템이 구독
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action<float> OnDamaged;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
}
```

---

# 데미지 처리 메서드

```csharp
public class Health : MonoBehaviour
{
    // 데미지 받기
    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;
        if (currentHealth <= 0) return; // 이미 사망
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamaged?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 처치
    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} died!");
    }
    
    // 힐
    public void Heal(float amount)
    {
        if (amount <= 0) return;
        if (currentHealth >= maxHealth) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }
}
```

---

# Health UI 연동

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using R3;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Health health;
    
    private VisualElement _hpFill;
    private Label _hpText;
    private readonly CompositeDisposable _disposables = new();
    
    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _hpFill = root.Q<VisualElement>("hp-fill");
        _hpText = root.Q<Label>("hp-text");
        
        health.CurrentHealth
            .Subscribe(UpdateHealthBar)
            .AddTo(_disposables);
    }
    
    private void UpdateHealthBar(float currentHealth)
    {
        float percent = health.HealthPercent;
        _hpFill.style.width = Length.Percent(percent * 100f);
        _hpText.text = $"{currentHealth:0} / {health.MaxHealth:0}";
        
        _hpFill.style.backgroundColor = percent < 0.3f
            ? new StyleColor(Color.red)
            : new StyleColor(Color.green);
    }
    
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

---

# 무적(Invincibility) 시스템

```csharp
using Cysharp.Threading.Tasks;

public class Health : MonoBehaviour
{
    [SerializeField] private float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // 무적 중 데미지 무시
        
        currentHealth -= damage;
        
        if (currentHealth > 0)
        {
            InvincibilityAsync().Forget();
        }
    }
    
    private async UniTaskVoid InvincibilityAsync()
    {
        isInvincible = true;
        
        if (spriteRenderer != null)
        {
            int blinkCount = Mathf.CeilToInt(invincibilityDuration / 0.2f);
            for (int i = 0; i < blinkCount; i++)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                await UniTask.Delay(100);
                spriteRenderer.color = Color.white;
                await UniTask.Delay(100);
            }
        }
        else
        {
            await UniTask.Delay((int)(invincibilityDuration * 1000));
        }
        
        isInvincible = false;
    }
}
```

R3 예시
```csharp
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private readonly HashSet<PlayerHealthState> _healthStates = new();

    // Read-only access (replaces your original public HashSet)
    public IReadOnlyCollection<PlayerHealthState> CurrentHealthStates => _healthStates;

    // Reactive events - this is the "R3 way"
    private readonly Subject<PlayerHealthState> _onStateAdded = new();
    private readonly Subject<PlayerHealthState> _onStateRemoved = new();

    public Observable<PlayerHealthState> OnStateAdded => _onStateAdded;
    public Observable<PlayerHealthState> OnStateRemoved => _onStateRemoved;

    /// <summary>
    /// Add a health state. If it already exists, the duration is refreshed (standard behavior for buffs/debuffs).
    /// If duration <= 0, the state is permanent.
    /// </summary>
    public void AddHealthState(PlayerHealthState state, float duration = 0f)
    {
        bool wasNew = _healthStates.Add(state);

        if (wasNew)
        {
            _onStateAdded.OnNext(state);
        }

        // Cancel any existing timer for this state (refresh duration)
        if (_stateTimers.TryGetValue(state, out var oldTimer))
        {
            oldTimer.Dispose();
            _stateTimers.Remove(state);
        }

        // Only start a timer if the state has a finite duration
        if (duration > 0f)
        {
            var timer = Observable.Timer(TimeSpan.FromSeconds(duration))
                .Subscribe(_ => RemoveHealthState(state));

            _stateTimers[state] = timer;
        }
    }

    public void RemoveHealthState(PlayerHealthState state)
    {
        if (_healthStates.Remove(state))
        {
            _onStateRemoved.OnNext(state);

            if (_stateTimers.TryGetValue(state, out var timer))
            {
                timer.Dispose();
                _stateTimers.Remove(state);
            }
        }
    }

    // Optional helper
    public bool HasState(PlayerHealthState state) => _healthStates.Contains(state);

    private readonly Dictionary<PlayerHealthState, IDisposable> _stateTimers = new();

    private void OnDestroy()
    {
        foreach (var timer in _stateTimers.Values)
        {
            timer.Dispose();
        }
        _stateTimers.Clear();

        _onStateAdded.OnCompleted();
        _onStateRemoved.OnCompleted();
        _onStateAdded.Dispose();
        _onStateRemoved.Dispose();
    }
}
```
---

<!-- _class: lead -->
# Part 2: Damage 계산 시스템

---

# Damage 계산 공식

```csharp
// DamageInfo: 데미지의 모든 정보를 담는 구조체
public struct DamageInfo
{
    public float baseDamage;        // 기본 데미지
    public float criticalMultiplier; // 크리티컬 배율 (1.5f = 150%)
    public bool isCritical;         // 크리티컬 여부
    public DamageType damageType;   // 데미지 타입
    public GameObject attacker;     // 공격자
    
    // 최종 데미지 계산
    public float GetFinalDamage()
    {
        float final = baseDamage;
        if (isCritical)
        {
            final *= criticalMultiplier;
        }
        return final;
    }
}

public enum DamageType
{
    Physical,   // 물리
    Magical,    // 마법
    True        // 고정 (방어력 무시)
}
```

---

# 방어력과 데미지 감소

```csharp
public class Defense : MonoBehaviour
{
    [SerializeField] private float physicalDefense = 10f;
    [SerializeField] private float magicalDefense = 5f;
    
    // 데미지 감소율 계산 (방어력 공식)
    // 공식: damageReduction = defense / (defense + 100)
    // 예: 방어력 100 = 50% 감소, 200 = 66% 감소
    public float CalculateDamageReduction(DamageType type)
    {
        float defense = type switch
        {
            DamageType.Physical => physicalDefense,
            DamageType.Magical => magicalDefense,
            DamageType.True => 0f,
            _ => 0f
        };
        
        return defense / (defense + 100f);
    }
    
    // 데미지 적용
    public float ApplyDefense(DamageInfo damageInfo)
    {
        if (damageInfo.damageType == DamageType.True)
        {
            return damageInfo.GetFinalDamage();
        }
        
        float reduction = CalculateDamageReduction(damageInfo.damageType);
        return damageInfo.GetFinalDamage() * (1f - reduction);
    }
}
```

---

# DamageCalculator 중앙 관리

```csharp
// 전역 데미지 계산기
public static class DamageCalculator
{
    // 크리티컬 확률 계산
    public static bool RollCritical(float criticalChance)
    {
        return Random.value <= criticalChance;
    }
    
    // 최종 데미지 계산
    public static DamageInfo CalculateDamage(
        float baseDamage,
        float criticalChance,
        float criticalMultiplier,
        DamageType type,
        GameObject attacker)
    {
        bool isCritical = RollCritical(criticalChance);
        
        return new DamageInfo
        {
            baseDamage = baseDamage,
            criticalMultiplier = criticalMultiplier,
            isCritical = isCritical,
            damageType = type,
            attacker = attacker
        };
    }
    
    // 타격 결과 메시지 생성
    public static string GetDamageText(DamageInfo info)
    {
        string damage = Mathf.Round(info.GetFinalDamage()).ToString();
        return info.isCritical ? $"<color=red><b>{damage}!!</b></color>" : damage;
    }
}
```

---

<!-- _class: lead -->
# Part 3: Hitbox 시스템

---

# Hitbox와 Hurtbox 개념

| 컴포넌트 | 역할 | 설명 |
|---------|------|------|
| **Hitbox** | 공격 판정 | 데미지를 주는 영역 |
| **Hurtbox** | 피격 판정 | 데미지를 받는 영역 |

```csharp
// Hitbox: 공격자에게 부착
public class Hitbox : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float criticalChance = 0.2f;
    [SerializeField] private DamageType damageType = DamageType.Physical;
    [SerializeField] private LayerMask targetLayers; // 공격 가능한 레이어
    
    private GameObject attacker;
    
    public void SetAttacker(GameObject attacker)
    {
        this.attacker = attacker;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 자기 자신은 무시
        if (other.gameObject == attacker) return;
        
        // 대상 레이어 확인
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;
        
        // Hurtbox 찾기
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            hurtbox.TakeHit(CreateDamageInfo());
        }
    }
    
    private DamageInfo CreateDamageInfo()
    {
        return DamageCalculator.CalculateDamage(
            damage, criticalChance, 1.5f, damageType, attacker
        );
    }
}
```

---

# Hurtbox 구현

```csharp
// Hurtbox: 피격자에게 부착
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Defense defense;
    
    // 피격 이벤트
    public event Action<DamageInfo> OnHit;
    
    private void Start()
    {
        if (health == null)
            health = GetComponent<Health>();
        if (defense == null)
            defense = GetComponent<Defense>();
    }
    
    public void TakeHit(DamageInfo damageInfo)
    {
        // 최종 데미지 계산
        float finalDamage = damageInfo.GetFinalDamage();
        
        if (defense != null)
        {
            finalDamage = defense.ApplyDefense(damageInfo);
        }
        
        // Health에 데미지 적용
        if (health != null)
        {
            health.TakeDamage(finalDamage);
        }
        
        // 이벤트 발행
        OnHit?.Invoke(damageInfo);
        
        // 넉백 적용
        ApplyKnockback(damageInfo);
    }
    
    private void ApplyKnockback(DamageInfo damageInfo)
    {
        KnockbackReceiver knockback = GetComponent<KnockbackReceiver>();
        if (knockback != null && damageInfo.attacker != null)
        {
            Vector2 direction = (transform.position - damageInfo.attacker.transform.position).normalized;
            knockback.ApplyKnockback(direction, finalDamage * 0.1f);
        }
    }
}
```

---

# 공격 타이밍 제어

```csharp
using Cysharp.Threading.Tasks;

public class AttackController : MonoBehaviour
{
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float hitboxActiveDuration = 0.2f;
    [SerializeField] private float windupDuration = 0.1f;
    
    private float lastAttackTime;
    private bool canAttack = true;
    
    public void PerformAttack()
    {
        if (!canAttack) return;
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        AttackSequenceAsync().Forget();
    }
    
    public async UniTaskVoid AttackSequenceAsync()
    {
        canAttack = false;
        lastAttackTime = Time.time;
        
        // 1. 공격 시작 (애니메이션 트리거)
        // animator.SetTrigger("Attack");
        
        // 2. Hitbox 활성화 (애니메이션 이벤트로도 가능)
        await UniTask.Delay((int)(windupDuration * 1000));
        hitbox.gameObject.SetActive(true);
        
        await UniTask.Delay((int)(hitboxActiveDuration * 1000));
        hitbox.gameObject.SetActive(false);
        
        // 3. 쿨다운
        float remaining = attackCooldown - windupDuration - hitboxActiveDuration;
        if (remaining > 0)
        {
            await UniTask.Delay((int)(remaining * 1000));
        }

        canAttack = true;
    }
}
```

---

<!-- _class: lead -->
# Part 4: LitMotion 넉백 시스템

---

# LitMotion 넉백 기초

```csharp
using LitMotion;
using LitMotion.Extensions;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseKnockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;
    
    private MotionHandle currentKnockback;
    
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    
    public void ApplyKnockback(Vector2 direction, float damageAmount)
    {
        // 기존 넉백 취소
        if (currentKnockback.IsActive())
        {
            currentKnockback.Cancel();
        }
        
        // 데미지에 비례한 넉백 세기
        float force = baseKnockbackForce + (damageAmount * 0.05f);
        Vector2 targetPosition = rb.position + (direction * force);
        
        // LitMotion으로 부드러운 넉백
        currentKnockback = LMotion.Create(rb.position, targetPosition, knockbackDuration)
            .WithEase(Ease.OutQuad)
            .BindToPosition(rb.transform);
    }
}
```

---

# 고급 넉백: 방향과 회전

```csharp
public class AdvancedKnockback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    
    public void ApplyDirectionalKnockback(
        Vector2 hitDirection, 
        float force,
        float duration,
        bool addRotation = false)
    {
        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + (hitDirection.normalized * force);
        
        // 위치 넉백
        var positionMotion = LMotion.Create(startPos, endPos, duration)
            .WithEase(Ease.OutCubic)
            .BindToPosition(rb.transform);
        
        // 회전 효과 (선택적)
        if (addRotation)
        {
            float randomRotation = Random.Range(-30f, 30f);
            
            LMotion.Create(0f, randomRotation, duration * 0.5f)
                .WithEase(Ease.OutBack)
                .BindToRotationZ(rb.transform)
                .AddTo(this);
            
            // 복귀
            LMotion.Create(randomRotation, 0f, duration * 0.5f)
                .WithDelay(duration * 0.5f)
                .WithEase(Ease.InOutQuad)
                .BindToRotationZ(rb.transform)
                .AddTo(this);
        }
    }
    
    // 벽 반사 넉백
    public void ApplyBounceKnockback(Vector2 direction, float force, int bounceCount = 1)
    {
        Vector2 currentPos = rb.position;
        
        for (int i = 0; i < bounceCount; i++)
        {
            float segmentForce = force * Mathf.Pow(0.7f, i); // 감쇠
            float segmentDuration = 0.2f * Mathf.Pow(1.2f, i);
            
            Vector2 targetPos = currentPos + (direction * segmentForce);
            
            LMotion.Create(currentPos, targetPos, segmentDuration)
                .WithEase(Ease.OutQuad)
                .BindToPosition(rb.transform)
                .AddTo(this);
            
            currentPos = targetPos;
            direction = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * direction;
        }
    }
}
```

---

# 넉백 + 히트스톱(Hitstop) 조합

```csharp
using Cysharp.Threading.Tasks;

public class CombatEffects : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    
    // 히트스톱: 타격 순간 시간을 느리게
    public void ApplyHitstop(float duration = 0.1f, float timeScale = 0.1f)
    {
        timeManager?.SlowMotionAsync(timeScale, duration);
    }
    
    // 넉백 + 히트스톱 통합
    public void ApplyImpactFeedback(
        Rigidbody2D target,
        Vector2 direction,
        float knockbackForce,
        float hitstopDuration = 0.05f)
    {
        // 1. 히트스톱 시작
        ApplyHitstop(hitstopDuration);
        
        // 2. 화면 흔들림
        CameraShaker.Instance?.Shake(0.2f, 0.1f);
        
        // 3. 넉백 (히트스톱 후 시작)
        LMotion.Create(target.position, target.position, hitstopDuration)
            .WithOnComplete(() =>
            {
                var knockback = target.GetComponent<KnockbackReceiver>();
                knockback?.ApplyKnockback(direction, knockbackForce);
            })
            .RunWithoutBinding()
            .AddTo(this);
    }
}

// 간단한 TimeManager
public class TimeManager : MonoBehaviour
{
    public async UniTaskVoid SlowMotionAsync(float targetTimeScale, float duration)
    {
        Time.timeScale = targetTimeScale;
        await UniTask.Delay((int)(duration * 1000), ignoreTimeScale: true);
        Time.timeScale = 1f;
    }
}
```

---

# Spine과 LitMotion 통합 넉백

```csharp
using Spine;
using Spine.Unity;
using LitMotion;
using LitMotion.Extensions;

public class SpineKnockback : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spine;
    [SerializeField] private Rigidbody2D rb;
    
    private Vector2 originalScale;
    
    private void Start()
    {
        originalScale = spine.transform.localScale;
    }
    
    public void ApplySpineKnockback(Vector2 direction, float force)
    {
        // 1. Spine 애니메이션 트리거
        spine.AnimationState.SetAnimation(0, "hit", false);
        spine.AnimationState.AddAnimation(0, "idle", true, 0.3f);
        
        // 2. 색상 플래시
        FlashColor(Color.red, 0.1f);
        
        // 3. 위치 넉백
        Vector2 targetPos = rb.position + (direction * force);
        
        LMotion.Create(rb.position, targetPos, 0.2f)
            .WithEase(Ease.OutQuad)
            .BindToPosition(rb.transform);
        
        // 4. 스케일 압축 효과 (LitMotion)
        LMotion.Create(originalScale, originalScale * 0.8f, 0.05f)
            .WithEase(Ease.OutQuad)
            .BindToLocalScale(spine.transform)
            .AddTo(this);
        
        // 복귀
        LMotion.Create(originalScale * 0.8f, originalScale, 0.2f)
            .WithDelay(0.05f)
            .WithEase(Ease.OutElastic)
            .BindToLocalScale(spine.transform)
            .AddTo(this);
    }
    
    private void FlashColor(Color flashColor, float duration)
    {
        var skeleton = spine.Skeleton;
        Color original = skeleton.GetColor();
        
        LMotion.Create(original, flashColor, duration * 0.3f)
            .WithOnUpdate(c => skeleton.SetColor(c))
            .RunWithoutBinding();
        
        LMotion.Create(flashColor, original, duration * 0.7f)
            .WithDelay(duration * 0.3f)
            .WithOnUpdate(c => skeleton.SetColor(c))
            .RunWithoutBinding();
    }
}
```

---

# 넉백 방어 및 저항

```csharp
public class KnockbackResistance : MonoBehaviour
{
    [SerializeField] private float resistancePercent = 0f; // 0 ~ 1
    [SerializeField] private float maxKnockbackDistance = 3f;
    [SerializeField] private bool isImmune = false;
    
    public float ModifyKnockbackForce(float originalForce)
    {
        if (isImmune) return 0f;
        
        // 저항률 적용
        float modified = originalForce * (1f - resistancePercent);
        
        // 최대 거리 제한
        if (modified > maxKnockbackDistance)
        {
            modified = maxKnockbackDistance;
        }
        
        return modified;
    }
    
    // 슈퍼 아머: 넉백 무시하고 대신 특수 효과
    public void ApplySuperArmor()
    {
        isImmune = true;
        
        // 슈퍼 아머 시각 효과
        LMotion.Create(1f, 1.2f, 0.1f)
            .WithEase(Ease.OutQuad)
            .BindToLocalScaleX(transform)
            .AddTo(this);
        
        LMotion.Create(1.2f, 1f, 0.15f)
            .WithDelay(0.1f)
            .WithEase(Ease.OutElastic)
            .BindToLocalScaleX(transform)
            .AddTo(this);
    }
}
```

---

<!-- _class: lead -->
# Part 5: AI 프롬프트 가이드

---

# 전투 시스템 프롬프트

## 기본 Health 시스템 생성

```
Unity 2D 게임용 Health 시스템을 만들어줘.

요구사항:
- 최대 체, 현재 체 관리
- TakeDamage(float damage) 메서드
- Heal(float amount) 메서드
- OnHealthChanged, OnDeath 이벤트
- 무적(Invincibility) 기능 (0.5초)
- 체 0 이하 시 Die() 호출

코드 스타일:
- 단일 책임 원칙 준수
- Unity Events 사용
- SerializeField로 Inspector 노출
```

---

# Hitbox/Hurtbox 프롬프트

```
Unity 2D 충돌 기반 전투 시스템을 만들어줘.

Hitbox (공격자):
- LayerMask로 대상 레이어 지정
- DamageInfo 구조체로 데미지 정보 전달
- OnTriggerEnter2D로 충돌 감지

Hurtbox (피격자):
- Health 컴포넌트 참조
- Defense 적용 후 최종 데미지 계산
- TakeHit(DamageInfo) 메서드

추가 기능:
- DamageType enum (Physical, Magical, True)
- 방어력 공식: defense / (defense + 100)
- 크리티컬 시스템
```

---

# LitMotion 넉백 프롬프트

```
LitMotion을 사용한 넉백 시스템을 만들어줘.

요구사항:
1. Rigidbody2D 기반 넉백
2. 방향과 세기 파라미터
3. Ease.OutQuad로 부드러운 감속
4. 연속 넉백 시 기존 모션 취소
5. duration: 0.3초 기본값

고급 기능:
- 회전 효과 추가 (선택적)
- 반사 넉백 (벽 튕김)
- 넉백 저항 시스템 (resistance 0~1)

LitMotion API 사용:
- LMotion.Create()
- BindToPosition()
- WithEase()
- MotionHandle.IsActive()
```

---

# 전체 전투 시스템 통합 프롬프트

```
Spine 캐릭터용 통합 전투 시스템을 만들어줘.

구성 요소:
1. Health (생명력)
2. Hitbox/Hurtbox (공격/피격)
3. AttackController (공격 타이밍)
4. SpineKnockback (LitMotion 넉백)

특징:
- Spine 애니메이션 연동
- 히트스톱(시간 느리게)
- 화면 흔들림 연동
- 무적 시 깜빡임 효과
- 데미지 텍스트 표시

코드 구조:
- 각 기능별 별도 컴포넌트
- 이벤트 기반 통신
- Inspector에서 설정 가능
```

---

<!-- _class: lead -->
# Part 6: 키워드 체크리스트

---

# 필수 키워드 정리

| 한글 | 영문 | 설명 |
|-----|-----|-----|
| 생명력 | Health/HP | 캐릭터의 생명 |
| 데미지 | Damage | 입히는 피해 |
| 방어력 | Defense | 받는 데미지 감소 |
| 무적 | Invincibility | 일시적 피해 무시 |
| 공격 판정 | Hitbox | 데미지를 주는 영역 |
| 피격 판정 | Hurtbox | 데미지를 받는 영역 |
| 넉백 | Knockback | 공격에 의한 밀려남 |
| 히트스톱 | Hitstop | 타격 순간 시간 정지 |
| 크리티컬 | Critical | 추가 데미지 발생 |
| 쿨다운 | Cooldown | 스킬 재사용 대기 |

---

# 코드 키워드

```csharp
// Health System
TakeDamage()      // 데미지 받기
Heal()           // 회복
OnHealthChanged  // 체 변화 이벤트
OnDeath          // 사망 이벤트
isInvincible     // 무적 상태

// Combat
DamageInfo       // 데미지 정보 구조체
DamageType       // 데미지 타입 enum
Hitbox           // 공격 판정
Hurtbox          // 피격 판정
LayerMask        // 레이어 필터
OnTriggerEnter2D // 충돌 이벤트

// LitMotion Knockback
LMotion.Create()       // 모션 생성
BindToPosition()       // 위치 바인딩
WithEase()            // 이징 함수
MotionHandle          // 모션 핸들
IsActive()            // 활성화 확인
Cancel()              // 모션 취소
```

---

# LitMotion 이징 함수 참고

| 이징 | 용도 |
|-----|------|
| `Linear` | 일정한 속도 |
| `OutQuad` | 자연스러운 감속 (넉백 기본) |
| `OutElastic` | 통통 튀는 효과 (복귀용) |
| `OutCubic` | 강한 감속 |
| `OutBack` | 살짝 넘어갔다가 돌아오기 |
| `InOutQuad` | 부드러운 가감속 |

---

<!-- _class: lead -->
# 🎯 실습 과제

---

# 미션: 완전체 전투 시스템

**목표**: Health + Hitbox + Knockback 통합 시스템 구현

```csharp
// 요구사항
1. Player와 Enemy 각각 Health 컴포넌트
2. 공격 시 Hitbox 활성화 (0.2초)
3. 피격 시 LitMotion 넉백 적용
4. 무적 시간 중 깜빡임 효과
5. 체 UI (Slider) 연동

추가 과제:
- 크리티컬 시 빨간색 데미지 텍스트
- 히트스톱 0.05초 적용
- 슈퍼 아머 기능 (보스용)
```

**제출**: GitHub 레포 링크 + 전투 시연 영상

---

# 체크리스트

**Session 7 완료 기준**

- [ ] Health 컴포넌트가 데미지를 받는가?
- [ ] Hitbox가 Hurtbox와 충돌하는가?
- [ ] LitMotion 넉백이 적용되는가?
- [ ] 무적 시간 중 데미지를 무시하는가?
- [ ] 체 UI가 실시간으로 업데이트되는가?

**자기 점검**

- [ ] Defense 방어력 공식이 올바른가?
- [ ] 넉백 방향이 공격자 기준인가?
- [ ] 연속 공격 시 이전 넉백이 취소되는가?

---

<!-- _class: lead -->
# Q & A

질문 있으신가요?

---

# 참고 자료

- **LitMotion GitHub**: https://github.com/annulusgames/LitMotion
- **Unity 2D Physics**: https://docs.unity3d.com/Manual/Physics2D.html
- **Spine 문서**: http://esotericsoftware.com/spine-unity
- **Game Programming Patterns**: https://gameprogrammingpatterns.com/

---

<!-- _class: lead -->
# 감사합니다!

## 다음 세션: Session 8 - 적 AI & 오브젝트 풀링 — UnityEngine.Pool·EnemyFactory 패턴
