---
marp: true
theme: default
class: invert
paginate: true
header: 'Week 4 - Session 8'
footer: '적 AI & 오브젝트 풀링'
---

# Session 8: 적 AI & 오브젝트 풀링
## UnityEngine.Pool·EnemyFactory 패턴

**Unity 2D 메트로배니아 액션 게임 개발**

---

## 학습 목표

- 다양한 적 AI 타입 이해하기
- Unity Object Pooling 시스템 구현하기
- Factory 패턴으로 적 생성 관리하기
- 성능 최적화를 위한 오브젝트 재사용

---

## Part 1: 적 AI 타입

---

## 적 AI 설계 패턴

| AI 타입 | 동작 방식 | 사용 예시 |
|---------|----------|----------|
| **Melee (근접)** | 플레이어 추적 후 근접 공격 | 좀비, 슬라임 |
| **Ranged (원거리)** | 거리 유지하며 발사체 공격 | 마법사, 궁수 |
| **Patrol (순찰)** | 고정 경로 따라 이동 | 경비병 |
| **Boss (보스)** | 단계별 패턴 전환 | 보스 몬스터 |

---

## Finite State Machine (FSM)

```csharp
public enum EnemyState
{
    Idle,      // 대기
    Chase,     // 추격
    Attack,    // 공격
    Return,    // 복귀
    Dead       // 사망
}

public class EnemyAI : MonoBehaviour
{
    private EnemyState currentState;
    
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Attack: UpdateAttack(); break;
        }
    }
}
```

---

## 근접 AI (Melee Enemy)

```csharp
public class MeleeEnemyAI : MonoBehaviour
{
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float moveSpeed = 3f;
    
    private Transform player;
    private float distanceToPlayer;
    
    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
    }
    
    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}
```

---

## 원거리 AI (Ranged Enemy)

```csharp
public class RangedEnemyAI : MonoBehaviour
{
    [SerializeField] private float optimalRange = 4f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    
    private float lastFireTime;
    
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        
        // 거리 조절
        if (distance < optimalRange - 0.5f)
            MoveAway();
        else if (distance > optimalRange + 0.5f)
            MoveCloser();
        
        // 발사
        if (Time.time >= lastFireTime + fireRate)
        {
            FireProjectile();
            lastFireTime = Time.time;
        }
    }
}
```

---

## 보스 AI 패턴 시스템

```csharp
public class BossAI : MonoBehaviour
{
    [System.Serializable]
    public class BossPhase
    {
        public string phaseName;
        public float healthThreshold;
        public float attackCooldown;
        public BossAttack[] attacks;
    }
    
    [SerializeField] private BossPhase[] phases;
    private int currentPhase = 0;
    
    void Update()
    {
        CheckPhaseTransition();
        ExecuteCurrentPattern();
    }
    
    void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / maxHealth;
        
        if (currentPhase + 1 < phases.Length && 
            healthPercent <= phases[currentPhase + 1].healthThreshold)
        {
            currentPhase++;
            OnPhaseChanged();
        }
    }
}
```

---

## Part 2: Object Pooling

---

## Object Pooling이 필요한 이유

**Instantiate/Destroy의 문제점:**
- **메모리 할당**: 새 객체 생성 시 힙 메모리 할당
- **GC 부담**: 파괴 시 가비지 컬렉션 트리거
- **성능 저하**: 런타임 중 프레임 드롭 발생

**Pooling의 장점:**
- 메모리 할당 최소화
- GC 호출 감소
- 일정한 프레임레이트 유지

---

## 기본 Object Pool 구현

```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    void Start()
    {
        InitializePool();
    }
    
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        // 풀이 비면 새로 생성
        return Instantiate(prefab);
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

---

## Unity 6.3 LTS ObjectPool

```csharp
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    
    private ObjectPool<Bullet> bulletPool;
    
    void Awake()
    {
        bulletPool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 50
        );
    }
    
    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab);
        bullet.SetPool(bulletPool);
        return bullet;
    }
    
    void OnGetBullet(Bullet bullet) => bullet.gameObject.SetActive(true);
    void OnReleaseBullet(Bullet bullet) => bullet.gameObject.SetActive(false);
    void OnDestroyBullet(Bullet bullet) => Destroy(bullet.gameObject);
}
```

---

## 풀링된 오브젝트 예시 (Bullet)

```csharp
public class Bullet : MonoBehaviour
{
    private ObjectPool<Bullet> pool;
    [SerializeField] private float lifetime = 3f;
    
    public void SetPool(ObjectPool<Bullet> objectPool)
    {
        pool = objectPool;
    }
    
    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifetime);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 처리
        ReturnToPool();
    }
    
    void ReturnToPool()
    {
        pool?.Release(this);
    }
}
```

---

## Part 3: Factory Pattern

---

## Enemy Factory 구현

```csharp
public class EnemyFactory : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public EnemyType type;
        public GameObject prefab;
        public int poolSize;
    }
    
    [SerializeField] private EnemyData[] enemyData;
    private Dictionary<EnemyType, ObjectPool<GameObject>> pools;
    
    void Awake()
    {
        InitializePools();
    }
    
    void InitializePools()
    {
        pools = new Dictionary<EnemyType, ObjectPool<GameObject>>();
        
        foreach (var data in enemyData)
        {
            pools[data.type] = new ObjectPool<GameObject>(
                () => Instantiate(data.prefab),
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                Destroy,
                true,
                data.poolSize
            );
        }
    }
}
```

---

## 적 생성 메서드

```csharp
public class EnemyFactory : MonoBehaviour
{
    public GameObject SpawnEnemy(EnemyType type, Vector2 position)
    {
        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"Enemy type {type} not found!");
            return null;
        }
        
        GameObject enemy = pools[type].Get();
        enemy.transform.position = position;
        
        // AI 컴포넌트 초기화
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        ai?.Initialize();
        
        return enemy;
    }
    
    public void DespawnEnemy(EnemyType type, GameObject enemy)
    {
        if (pools.ContainsKey(type))
        {
            pools[type].Release(enemy);
        }
    }
}

public enum EnemyType
{
    Slime,
    Skeleton,
    Boss
}
```

---

## Spawn Manager 통합

```csharp
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 10;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }
    
    void SpawnEnemy()
    {
        if (activeEnemies.Count >= maxEnemies) return;
        
        Vector2 spawnPos = GetRandomSpawnPosition();
        EnemyType type = GetRandomEnemyType();
        
        GameObject enemy = enemyFactory.SpawnEnemy(type, spawnPos);
        activeEnemies.Add(enemy);
        
        // 적 사망 이벤트 구독
        enemy.GetComponent<Health>()?.OnDeath
            .Take(1)
            .Subscribe(_ => OnEnemyDeath(enemy));
    }
}
```

---

## Part 4: AI 프롬프트

---

## 프롬프트 1: Object Pool 시스템

```
Unity 2D 게임용 범용 Object Pool 시스템을 만들어줘.

요구사항:
1. 제네릭 타입 지원
2. 풀 크기 자동 확장
3. Unity 6.3 LTS 기본 ObjectPool 사용
4. MonoBehaviour가 아닌 클래스도 지원
5. 풀 통계 (활성/비활성 카운트) 제공

추가 기능:
- 풀 프리워밍 (Warmup) 기능
- 풀 크기 제한 및 우선순위 제거
- 오브젝트가 풀에 자동 반환되는 인터페이스
```

---

## 프롬프트 2: Enemy Spawner

```
메트로배니아 게임의 적 스폰 시스템을 만들어줘.

요구사항:
1. Wave 기반 스폰 (단계별 난이도 증가)
2. 여러 종류의 적 동시 스폰
3. 플레이어와 일정 거리 이상에서 스폰
4. Object Pooling 통합
5. 스폰 포인트 가중치 시스템

데이터 구조:
- WaveData: 적 타입, 수량, 간격
- SpawnPoint: 위치, 가중치
- DifficultyCurve: 시간에 따른 난이도 곡선
```

---

## 프롬프트 3: FSM AI 시스템

```
Unity용 유연한 Finite State Machine AI 프레임워크를 만들어줘.

요구사항:
1. ScriptableObject 기반 상태 정의
2. 상태 전환 조건 시스템
3. 애니메이션 연동
4. 디버그 시각화 (Gizmos)
5. 이벤트 기반 전환

구성 요소:
- State: 기본 상태 클래스
- Transition: 전환 조건
- Condition: 체크 로직
- StateMachine: 실행 엔진

예시 상태들:
- Idle, Patrol, Chase, Attack, Flee, Dead
```

---

## 체크리스트

- [ ] Melee AI 구현 (추격 + 근접 공격)
- [ ] Ranged AI 구현 (거리 유지 + 발사)
- [ ] Boss Phase 시스템 설계
- [ ] Object Pool 기본 구조
- [ ] Unity ObjectPool 적용
- [ ] Enemy Factory 구현
- [ ] Spawn Manager 통합
- [ ] 성능 프로파일링

---

## 핵심 키워드

| 한글 | 영문 | 설명 |
|------|------|------|
| 오브젝트 풀링 | Object Pooling | 객체 재사용 패턴 |
| 팩토리 패턴 | Factory Pattern | 객체 생성 캡슐화 |
| 유한 상태 기계 | Finite State Machine | AI 상태 관리 |
| 추격 | Chase | 플레이어 추적 행동 |
| 순찰 | Patrol | 고정 경로 이동 |
| 발사체 | Projectile | 원거리 공격물 |
| 스폰 | Spawn | 오브젝트 생성 |
| 디스폰 | Despawn | 오브젝트 반환 |
| 풀 크기 | Pool Size | 풀의 최대 용량 |
| GC | Garbage Collection | 메모리 정리 |

---

## 실습 과제

1. **Melee Enemy** 구현하기
   - 플레이어 감지 및 추격
   - 공격 범위 내에서 공격

2. **Object Pool** 적용하기
   - Bullet 오브젝트 풀링
   - Spawn/Despawn 테스트

3. **Factory Pattern** 적용하기
   - EnemyFactory 클래스
   - 여러 종류의 적 생성

---

## 다음 세션 예고

### Session 9: 능력 시스템 + LitMotion 대시

- 능력 시스템 설계
- 더블 점프 구현
- LitMotion 대시 공격

---

## Q&A

질문 있으신가요?

---

## 참고 자료

- [Unity Object Pool](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html)
- [Object Pool Pattern](https://gameprogrammingpatterns.com/object-pool.html)
- [Game AI Pro](http://www.gameaipro.com/)
