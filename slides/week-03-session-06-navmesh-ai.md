---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
---

<!-- _class: lead -->

# Session 6: NavMesh 2D AI
## Unity 고급 AI 시스템 구축

**Week 03** | AI 기반 게임플레이 시스템

---

## 목차

1. NavMesh 개요 및 2D 설정
2. AI 상태 머신 (State Machine)
3. LitMotion으로 부드러운 회전
4. AI 프롬프트 설계
5. 실전 예제: 추격 AI

---

<!-- _class: lead -->
# 1. NavMesh 개요 및 2D 설정

---

## NavMesh란?

**Navigation Mesh**의 약자로, AI 캐릭터가 이동 가능한 영역을 나타내는 데이터 구조

- **장점**: 자동 경로 탐색, 장애물 회피, 동적 경로 재계산
- **Unity**: 기본 3D NavMesh + 2D 확장 패키지 지원

```csharp
// NavMeshAgent 기본 사용
using UnityEngine;
using UnityEngine.AI;

public class SimpleAI : MonoBehaviour
{
    private NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(targetPosition);
    }
}
```

---

## NavMesh 2D 패키지 설치

**Package Manager**에서 설치:

```
com.unity.ai.navigation
```

**NavMeshComponents** (GitHub):
```
https://github.com/h8man/NavMeshPlus
```

필요 컴포넌트:
- `NavMeshSurface` - 2D 타일맵에서 NavMesh 생성
- `NavMeshModifier` - 특정 영역 수정
- `NavMeshModifierVolume` - 영역별 속성 변경

---

## 2D NavMesh 설정

```csharp
using NavMeshPlus.Components;

public class NavMesh2DSetup : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surface;
    
    void Start()
    {
        // 2D 타일맵에서 NavMesh 빌드
        surface.BuildNavMesh();
    }
    
    // 런타임에 NavMesh 재빌드
    public void RebuildNavMesh()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }
}
```

---

## NavMeshSurface 설정

**Inspector 설정:**
- `Collect Objects` → `Children` 또는 `Volume`
- `Use Geometry` → `Physics Colliders` (2D 권장)
- `Agent Radius` → 캐릭터 크기에 맞게 조정
- `Agent Height` → 2D에서는 1~2 권장

```csharp
// 동적으로 장애물 추가
public void AddObstacle(Collider2D collider)
{
    var modifier = collider.gameObject.AddComponent<NavMeshModifier>();
    modifier.overrideArea = true;
    modifier.area = 1; // Not Walkable
    surface.BuildNavMesh();
}
```

---

<!-- _class: lead -->
# 2. AI 상태 머신 (State Machine)

---

## 상태 패턴 기본 구조

```csharp
public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Return
}

public interface IAIState
{
    void Enter(AIController ai);
    void Update(AIController ai);
    void Exit(AIController ai);
}
```

---

## 상태 구현 예제

```csharp
public class ChaseState : IAIState
{
    public void Enter(AIController ai)
    {
        ai.SetSpeed(ai.ChaseSpeed);
        ai.PlayAnimation("Run");
    }
    
    public void Update(AIController ai)
    {
        float distance = Vector2.Distance(
            ai.transform.position, 
            ai.Target.position
        );
        
        if (distance > ai.DetectionRange)
        {
            ai.ChangeState(AIState.Return);
            return;
        }
        
        if (distance <= ai.AttackRange)
        {
            ai.ChangeState(AIState.Attack);
            return;
        }
        
        ai.MoveTo(ai.Target.position);
    }
    
    public void Exit(AIController ai) { }
}
```

---

## AI 컨트롤러

```csharp
public class AIController : MonoBehaviour
{
    [Header("Detection")]
    public float DetectionRange = 8f;
    public float AttackRange = 1.5f;
    public float LoseInterestRange = 12f;
    
    [Header("Movement")]
    public float PatrolSpeed = 2f;
    public float ChaseSpeed = 5f;
    
    private NavMeshAgent agent;
    private IAIState currentState;
    private Dictionary<AIState, IAIState> states;
    
    public Transform Target { get; set; }
    public Vector3 StartPosition { get; private set; }
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // 2D 수동 회전
        agent.updateUpAxis = false;
        
        StartPosition = transform.position;
        
        states = new Dictionary<AIState, IAIState>
        {
            { AIState.Idle, new IdleState() },
            { AIState.Patrol, new PatrolState() },
            { AIState.Chase, new ChaseState() },
            { AIState.Attack, new AttackState() },
            { AIState.Return, new ReturnState() }
        };
        
        ChangeState(AIState.Idle);
    }
```

---

## 상태 변경 및 이동

```csharp
    void Update()
    {
        currentState?.Update(this);
        UpdateRotation();
    }
    
    public void ChangeState(AIState newState)
    {
        currentState?.Exit(this);
        currentState = states[newState];
        currentState.Enter(this);
    }
    
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    
    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }
    
    public void Stop()
    {
        agent.isStopped = true;
    }
    
    public void Resume()
    {
        agent.isStopped = false;
    }
}
```

---

<!-- _class: lead -->
# 3. LitMotion으로 부드러운 회전

---

## LitMotion 소개

**LitMotion**: Unity용 고성능 트위닝 라이브러리

```csharp
// LitMotion 설치 (Package Manager)
// https://github.com/annulusgames/LitMotion

using LitMotion;
using LitMotion.Extensions;
```

**장점**:
- Zero allocation 트위닝
- DOTS/ECS 호환
- 체이닝 지원
- 커스텀 이징 함수

---

## 기본 트윈 예제

```csharp
using LitMotion;
using LitMotion.Extensions;

public class MotionExample : MonoBehaviour
{
    void Start()
    {
        // 위치 이동
        LMotion.Create(Vector3.zero, Vector3.one, 1f)
            .BindToPosition(transform);
        
        // 회전
        LMotion.Create(0f, 360f, 2f)
            .BindToEulerAnglesZ(transform);
        
        // 스케일
        LMotion.Create(Vector3.zero, Vector3.one, 0.5f)
            .WithEase(Ease.OutBack)
            .BindToLocalScale(transform);
    }
}
```

---

## AI 부드러운 회전 구현

```csharp
public class SmoothRotationAI : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minMoveThreshold = 0.1f;
    
    private NavMeshAgent agent;
    private MotionHandle rotationHandle;
    private Vector2 lastPosition;
    
    void Update()
    {
        Vector2 currentPos = transform.position;
        Vector2 moveDirection = (currentPos - lastPosition).normalized;
        
        if (moveDirection.magnitude > minMoveThreshold)
        {
            RotateTowards(moveDirection);
        }
        
        lastPosition = currentPos;
    }
    
    void RotateTowards(Vector2 direction)
    {
        // 현재 각도
        float currentAngle = transform.eulerAngles.z;
        
        // 목표 각도
        float targetAngle = Mathf.Atan2(direction.y, direction.x) 
            * Mathf.Rad2Deg - 90f;
        
        // 부드러운 회전
        LMotion.Create(currentAngle, targetAngle, 0.2f)
            .WithEase(Ease.OutQuad)
            .BindToEulerAnglesZ(transform);
    }
}
```

---

## LitMotion + NavMesh 통합

```csharp
public class AdvancedAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private MotionHandle rotateHandle;
    private MotionHandle scaleHandle;
    
    [Header("Motion Settings")]
    public float RotateDuration = 0.15f;
    public Ease RotateEase = Ease.OutQuad;
    
    void Update()
    {
        if (agent.hasPath && agent.remainingDistance > 0.1f)
        {
            SmoothRotateTo(agent.desiredVelocity);
        }
    }
    
    void SmoothRotateTo(Vector3 velocity)
    {
        if (velocity.sqrMagnitude < 0.01f) return;
        
        Vector2 dir = velocity.normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) 
            * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        
        // 기존 모션 취소 후 새로 시작
        rotateHandle.TryCancel();
        
        rotateHandle = LMotion.Create(currentAngle, targetAngle, RotateDuration)
            .WithEase(RotateEase)
            .BindToEulerAnglesZ(transform);
    }
    
    public void PlayAttackMotion()
    {
        // 공격 시 튕기는 효과
        scaleHandle.TryCancel();
        scaleHandle = LMotion.Sequence()
            .Append(LMotion.Create(1f, 1.2f, 0.1f))
            .Append(LMotion.Create(1.2f, 1f, 0.2f))
            .WithEase(Ease.OutBack)
            .BindToLocalScaleX(transform)
            .AddTo(this);
    }
}
```

---

## 체이닝과 복합 모션

```csharp
public void PlayDamageReaction()
{
    // 색상 + 위치 + 회전 동시 적용
    var sprite = GetComponent<SpriteRenderer>();
    
    // 빨갛게 깜빡임
    LMotion.Create(Color.white, Color.red, 0.1f)
        .WithEase(Ease.OutQuad)
        .BindToColor(sprite)
        .AddTo(this);
    
    // 뒤로 밀림
    Vector2 knockback = -transform.up * 0.5f;
    LMotion.Create(transform.position, 
                   transform.position + (Vector3)knockback, 
                   0.2f)
        .WithEase(Ease.OutCubic)
        .BindToPosition(transform);
    
    // 복귀
    LMotion.Create(Color.red, Color.white, 0.3f)
        .WithDelay(0.1f)
        .BindToColor(sprite);
}
```

---

<!-- _class: lead -->
# 4. AI 프롬프트 설계

---

## AI 행동 프롬프트 구조

**NPC 프롬프트 템플릿:**

```markdown
# 역할
당신은 {게임 이름}의 {NPC 이름}입니다.

# 성격
- {성격 특성 1}
- {성격 특성 2}
- {성격 특성 3}

# 배경
{NPC의 배경 스토리}

# 행동 규칙
1. 플레이어가 범위 내에 있을 때: 추격
2. 처음 공격받았을 때: 경계 상태
3. 처치당하면: 마지막 대사 출력

# 대사 예시
- 발견 시: "거기 서!"
- 공격 시: "받아라!"
- 처치 시: "크윽..."
```

---

## 동적 AI 프롬프트 코드

```csharp
[System.Serializable]
public class AIBehaviorPrompt
{
    [TextArea(3, 10)]
    public string RolePrompt;
    
    [TextArea(2, 5)]
    public string OnDetectPlayer;
    
    [TextArea(2, 5)]
    public string OnAttack;
    
    [TextArea(2, 5)]
    public string OnDeath;
    
    public float ResponseDelay = 1f;
}

public class AIPromptController : MonoBehaviour
{
    [SerializeField] private AIBehaviorPrompt promptData;
    [SerializeField] private bool useLLM = false;
    
    public void OnDetectPlayer()
    {
        if (useLLM)
        {
            // LLM API 호출
            StartCoroutine(CallLLM(promptData.OnDetectPlayer));
        }
        else
        {
            // 프리셋 메시지
            ShowDialogue(promptData.OnDetectPlayer);
        }
    }
    
    IEnumerator CallLLM(string context)
    {
        // OpenAI/Claude API 호출
        // ... API 구현
        yield return null;
    }
}
```

---

## 행동 트리 + 프롬프트 통합

```csharp
public class SmartAI : MonoBehaviour
{
    [Header("AI Prompt")]
    public string Personality = "aggressive"; // aggressive, defensive, coward
    
    private IAIState currentState;
    
    void Update()
    {
        // 프롬프트 기반 의사결정
        var decision = EvaluateSituation();
        ExecuteDecision(decision);
    }
    
    AIDecision EvaluateSituation()
    {
        float healthRatio = currentHealth / maxHealth;
        float playerDistance = Vector2.Distance(
            transform.position, 
            player.position
        );
        
        // 프롬프트 기반 로직
        return Personality switch
        {
            "aggressive" => EvaluateAggressive(healthRatio, playerDistance),
            "defensive" => EvaluateDefensive(healthRatio, playerDistance),
            "coward" => EvaluateCoward(healthRatio, playerDistance),
            _ => AIDecision.Idle
        };
    }
    
    AIDecision EvaluateAggressive(float health, float distance)
    {
        if (health < 0.2f) return AIDecision.Retreat;
        if (distance < attackRange) return AIDecision.Attack;
        if (distance < detectionRange) return AIDecision.Chase;
        return AIDecision.Patrol;
    }
}
```

---

<!-- _class: lead -->
# 5. 실전 예제: 추격 AI

---

## 완전한 추격 AI 구현

```csharp
using UnityEngine;
using UnityEngine.AI;
using LitMotion;
using LitMotion.Extensions;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaserAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float loseRange = 12f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float rotationSmoothing = 0.15f;
    
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    
    private NavMeshAgent agent;
    private Transform target;
    private MotionHandle rotateHandle;
    private int currentPatrolIndex = 0;
    
    private enum State { Patrol, Chase, Search }
    private State currentState = State.Patrol;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
```

---

## 추격 AI (계속)

```csharp
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                DetectPlayer();
                break;
                
            case State.Chase:
                UpdateChase();
                CheckLostPlayer();
                break;
                
            case State.Search:
                UpdateSearch();
                break;
        }
        
        UpdateRotation();
    }
    
    void UpdatePatrol()
    {
        agent.speed = patrolSpeed;
        
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }
    
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
```

---

## 추격 AI (계속)

```csharp
    void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(
            transform.position, 
            detectionRange, 
            playerLayer
        );
        
        if (player != null)
        {
            target = player.transform;
            ChangeState(State.Chase);
        }
    }
    
    void UpdateChase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
    }
    
    void CheckLostPlayer()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        
        if (distance > loseRange)
        {
            ChangeState(State.Search);
        }
    }
```

---

## 부드러운 회전 구현

```csharp
    void UpdateRotation()
    {
        if (!agent.hasPath) return;
        
        Vector2 velocity = agent.desiredVelocity;
        if (velocity.sqrMagnitude < 0.01f) return;
        
        // 이동 방향 계산
        float targetAngle = Mathf.Atan2(velocity.y, velocity.x) 
            * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        
        // 각도 보정 (-180~180)
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        targetAngle = currentAngle + angleDiff;
        
        // LitMotion으로 부드러운 회전
        rotateHandle.TryCancel();
        rotateHandle = LMotion.Create(currentAngle, targetAngle, rotationSmoothing)
            .WithEase(Ease.OutQuad)
            .BindToEulerAnglesZ(transform);
    }
    
    void ChangeState(State newState)
    {
        currentState = newState;
        
        // 상태 변경 시 시각적 피드백
        PlayStateChangeEffect(newState);
    }
```

---

## 시각적 피드백

```csharp
    void PlayStateChangeEffect(State state)
    {
        var sprite = GetComponent<SpriteRenderer>();
        Color targetColor = state switch
        {
            State.Patrol => Color.white,
            State.Chase => Color.red,
            State.Search => Color.yellow,
            _ => Color.white
        };
        
        // 색상 트윈
        LMotion.Create(sprite.color, targetColor, 0.3f)
            .BindToColor(sprite);
        
        // 크기 튕김
        LMotion.Create(1f, 1.1f, 0.1f)
            .WithEase(Ease.OutBack)
            .BindToLocalScaleX(transform)
            .AddTo(this);
        
        LMotion.Create(1f, 1.1f, 0.1f)
            .WithEase(Ease.OutBack)
            .BindToLocalScaleY(transform)
            .AddTo(this);
    }
    
    void OnDrawGizmosSelected()
    {
        // 탐지 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
```

---

<!-- _class: lead -->
# 핵심 키워드 체크리스트

---

## 오늘 배운 핵심 개념

| 키워드 | 설명 | 체크 |
|--------|------|------|
| NavMesh | AI 이동 가능 영역 메시 | [ ] |
| NavMeshSurface | 2D 타일맵에서 NavMesh 생성 | [ ] |
| NavMeshAgent | 경로 탐색 및 이동 컴포넌트 | [ ] |
| State Machine | AI 행동 상태 관리 패턴 | [ ] |
| LitMotion | 고성능 트위닝 라이브러리 | [ ] |
| LMotion.Create() | 트윈 생성 메서드 | [ ] |
| BindToEulerAnglesZ() | Z축 회전 바인딩 | [ ] |
| SetDestination() | 목적지 설정 | [ ] |
| desiredVelocity | 에이전트의 희망 이동 속도 | [ ] |
| MotionHandle | 트윈 제어 핸들 | [ ] |

---

## 중요 메서드/클스 정리

```csharp
// NavMesh
NavMeshSurface.BuildNavMesh();     // NavMesh 빌드
agent.SetDestination(Vector3);      // 목적지 설정
agent.remainingDistance;            // 남은 거리
agent.desiredVelocity;              // 희망 이동 방향

// LitMotion
LMotion.Create(start, end, duration);  // 트윈 생성
.BindToPosition(transform);             // 위치 바인딩
.BindToEulerAnglesZ(transform);         // 회전 바인딩
.WithEase(Ease.OutQuad);                // 이징 적용
.WithDelay(0.5f);                       // 지연 실행

// State Pattern
ChangeState(AIState newState);          // 상태 변경
Enter/Update/Exit                       // 상태 메서드
```

---

## 과제: AI 적 완성하기

**목표**: 플레이어를 추격하는 AI 적 3종 구현

**요구사항**:
1. **근접 AI**: 빠르게 접근하여 공격
2. **원거리 AI**: 거리 유지하며 투사체 발사
3. **회피 AI**: 플레이어 공격 피하면서 반격

**필수 요소**:
- [ ] NavMesh 2D 설정
- [ ] 상태 머신 구현 (Idle, Chase, Attack, Flee)
- [ ] LitMotion으로 부드러운 회전
- [ ] 공격/피격 시 시각적 피드백
- [ ] AI 프롬프트 시스템 (선택사항)

---

<!-- _class: lead -->

## Q&A

**질문이 있으신가요?**

---

## 참고 자료

- **NavMesh Plus**: https://github.com/h8man/NavMeshPlus
- **LitMotion**: https://github.com/annulusgames/LitMotion
- **Unity AI Navigation**: https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/index.html
- **State Pattern**: https://refactoring.guru/design-patterns/state

---

<!-- _class: lead -->

# 다음 세션 예고

## Session 7: 전투 시스템 & LitMotion 넉백

**R3 ReactiveProperty로 Health 시스템 및 전투 구현**

---

<!-- _class: lead -->

# 감사합니다!

## 질문 환영합니다
