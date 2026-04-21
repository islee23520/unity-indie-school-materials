# Session 4: 플레이어 컨트롤러 & LitMotion 이동

## Unity 실무 중심 메트로배니아 - Week 2

---

## 📋 이번 세션 목표

1. **New Input System** 설정하고 사용하기
2. **Physics-based** 캐릭터 이동 구현
3. **LitMotion**으로 부드러운 이동 구현
4. 실전 예제: 플레이어 컨트롤러 완성

---

## 🔧 New Input System

### 왜 New Input System인가?

| 기능 | Old Input | New Input |
|------|-----------|-----------|
| 키 리맵핑 | 수동 구현 필요 | 내장 지원 |
| 게임패드 | 별도 처리 | 자동 지원 |
| 이벤트 기반 | 평문 검사 | 이벤트 콜백 |

---

## New Input System 설치

```bash
# Package Manager에서 설치
Window → Package Manager → Unity Registry
→ "Input System" 검색 → Install
```

**설치 후 팝업:**
- "Yes" 클릭 → 프로젝트가 새로고침됨
- 이제 Legacy Input Manager 비활성화됨

---

## Input Action Asset 생성

```
Project 창 → 우클릭 → Create →
  Input Actions
→ 이름: "PlayerInputActions"
```

**기본 설정:**
- Action Maps → "Player" 생성
- Actions → "Move", "Jump" 추가

---

## Input Actions 설정 예시

```yaml
# PlayerInputActions.asset 남부
Action Maps:
  - Name: Player
    Actions:
      - Name: Move
        Type: Value
        Control Type: Vector2
      - Name: Jump
        Type: Button

# Move 바인딩
- WASD 키보드
- Arrow keys 키보드
- 게임패드 왼쪽 스틱

# Jump 바인딩  
- Spacebar 키보드
- A 버튼 (Xbox)
```

---

## PlayerInput 코드 예제

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    // Input System에서 호출
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnJump(InputValue value)
    {
        if (isGrounded && value.isPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private void FixedUpdate()
    {
        // Physics-based 이동
        Vector2 velocity = rb.velocity;
        velocity.x = moveInput.x * moveSpeed;
        rb.velocity = velocity;
    }
}
```

---

## 🎯 Physics-based Movement

### 왜 Physics-based?

| 방식 | 장점 | 단점 |
|------|------|------|
| transform.position | 단순함 | 충돌 무시 |
| rb.MovePosition() | 충돌 감지 | 약간 복잡 |
| rb.velocity | 직관적 | 관성 있음 |
| rb.AddForce() | 현실적 | 미세조정 어려움 |

**메트로배니아에서:** 캐릭터 이동은 물리 기반이 필수!

---

## Physics 기반 이동 패턴

```csharp
public class PhysicsMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float friction = 5f;
    
    private Rigidbody2D rb;
    private Vector2 inputDirection;
    
    private void FixedUpdate()
    {
        // 가속
        if (inputDirection != Vector2.zero)
        {
            rb.AddForce(inputDirection * acceleration);
        }
        
        // 마찰력 (자연스러운 감속)
        rb.velocity *= (1f - friction * Time.fixedDeltaTime);
        
        // 최대 속도 제한
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
```

---

## 🌊 LitMotion 소개

### LitMotion이란?

**고성능 C# 트위닝 라이브러리**

```
NuGet/Git에서: "LitMotion" 검색
Unity용: OpenUPM 또는 Git URL
```

**특징:**
- Zero-allocation (GC 없음)
- Burst Compiler 지원
- UniTask와 통합
- 실무용 경량 트윈 워크플로우 구성에 적합

---

## LitMotion 설치

```json
// manifest.json
{
  "dependencies": {
    "com.annulusgames.lit-motion": "https://github.com/AnnulusGames/LitMotion.git?path=src/LitMotion/Assets/LitMotion"
  }
}
```

또는 OpenUPM:
```
Window → Package Manager → + → Add package from git URL
```

---

## LitMotion 기본 사용법

```csharp
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class LitMotionExample : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    void Start()
    {
        // 기본 트윈
        LMotion.Create(Vector3.zero, Vector3.one, 1f)
            .BindToPosition(target);
        
        // 이asing 적용
        LMotion.Create(0f, 100f, 2f)
            .WithEase(Ease.OutBounce)
            .BindToLocalPositionX(target);
        
        // 체이닝
        LMotion.Create(transform.position, targetPosition, 0.5f)
            .WithEase(Ease.InOutQuad)
            .WithOnComplete(() => Debug.Log("Done!"))
            .Preserve()
            .RunWithoutBinding();
    }
}
```

---

## LitMotion으로 부드러운 이동

```csharp
public class SmoothPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.3f;
    
    private Vector3 targetPosition;
    private MotionHandle currentMotion;
    
    void Update()
    {
        // 입력 받기
        if (Input.GetKeyDown(KeyCode.A))
            MoveTo(transform.position + Vector3.left);
        if (Input.GetKeyDown(KeyCode.D))
            MoveTo(transform.position + Vector3.right);
    }
    
    void MoveTo(Vector3 destination)
    {
        // 이전 모션 취소
        if (currentMotion.IsActive())
            currentMotion.Cancel();
        
        // 새로운 부드러운 이동
        currentMotion = LMotion
            .Create(transform.position, destination, moveDuration)
            .WithEase(Ease.OutCubic)
            .BindToPosition(transform);
    }
}
```

---

## ⚡ 실전: 완전한 플레이어 컨트롤러

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;
using LitMotion.Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class CompletePlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    
    [Header("Visual")]
    [SerializeField] private float squashStretch = 0.3f;
    [SerializeField] private float squashDuration = 0.15f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private MotionHandle squashMotion;
    
    void Awake() => rb = GetComponent<Rigidbody2D>();
    
    void Update()
    {
        // Timer 업데이트
        coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
        
        // 버퍼된 점프 체크
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            PerformJump();
            jumpBufferTimer = 0;
        }
        
        isGrounded = CheckGrounded();
        if (isGrounded) coyoteTimer = coyoteTime;
    }
    
    void FixedUpdate()
    {
        // 부드러운 가속
        Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }
    
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpBufferTimer = jumpBuffer;
        }
    }
    
    void PerformJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        SquashStretchEffect();
    }
    
    void SquashStretchEffect()
    {
        if (squashMotion.IsActive()) squashMotion.Cancel();
        
        // 점프 시 스쿼시 효과
        transform.localScale = new Vector3(1 + squashStretch, 1 - squashStretch, 1);
        
        squashMotion = LMotion
            .Create(transform.localScale, Vector3.one, squashDuration)
            .WithEase(Ease.OutElastic)
            .BindToLocalScale(transform);
    }
    
    bool CheckGrounded() => Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
}
```

---

## 🎨 AI 프롬프트: Input System

```markdown
"Unity New Input System으로 2D 플랫포머 컨트롤러를 
만들어줘. WASD로 이동하고 Space로 점프해야 해. 
Coyote time과 jump buffer 기능도 포함해줘."
```

**추가 프롬프트 예시:**

```markdown
"Unity Input System에서 다음 기능을 구현해줘:
- 게임패드 지원
- 키 리맵핑 UI
- 콤보 시스템 (연속 입력)  
- 입력 버퍼"
```

---

## 🎨 AI 프롬프트: Movement

```markdown
"LitMotion을 사용해서 Unity 2D 캐릭터의 부드러운 
이동을 구현해줘. dash 기능이 있고, dash할 때 
잔상 효과도 함께 만들어줘."
```

**추가 프롬프트 예시:**

```markdown
"2D 플랫포머 물리 시스템을 구현해줘:
- 가변 점프 높이
- 벽 슬라이딩
- 벽 점프  
- 대시
- LitMotion으로 부드러운 칸라 변경"
```

---

## 📊 LitMotion Easing 참조

| Ease | 효과 |
|------|------|
| Linear | 일정한 속도 |
| InQuad | 천천히 시작 |
| OutQuad | 천천히 끝 |
| InOutQuad | 양쪽 부드럽게 |
| OutBounce | 튕기며 끝 |
| OutElastic | 고무줄처럼 튕김 |
| OutBack | 살짝 넘쳤다가 돌아옴 |

---

## ✅ 핵심 키워드 체크리스트

- [ ] **Input Action Asset** 생성
- [ ] **PlayerInput** 컴포넌트 설정
- [ ] **OnMove**, **OnJump** 콜백 구현
- [ ] **Rigidbody2D.velocity**로 이동
- [ ] **AddForce**로 점프
- [ ] **Coyote Time** 구현
- [ ] **Jump Buffer** 구현
- [ ] **LitMotion** 설치
- [ ] **LMotion.Create** 사용
- [ ] **WithEase**로 이asing 적용
- [ ] **BindToPosition**으로 연결
- [ ] **MotionHandle**로 제어

---

## 🎮 실습 과제

### 미션 1: 기본 이동
```
New Input System 설정하고
WASD로 캐릭터 이동하기
```

### 미션 2: 점프 개선
```
Coyote Time + Jump Buffer 추가
더 반응 좋은 점프 만들기
```

### 미션 3: LitMotion 적용
```
이동 시 부드러운 보간
점프/착지 시 스쿼시 효과
```

---

## 🔗 참고 자료

- [Unity Input System Docs](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [LitMotion GitHub](https://github.com/AnnulusGames/LitMotion)
- 예제 프로젝트: `Examples/02-PlayerController/`

---

## 질문 있으신가요?

### 다음 세션 예고
**Session 5: 타일맵 & ScriptableObject & LitMotion**
- 메트로배니아 레벨 디자인 원칙
- Tilemap 설정 및 규칙 타일
- ScriptableObject로 레벨 데이터 관리

---

## 📝 요약

| 개념 | 핵심 포인트 |
|------|-------------|
| Input System | Action-based, 이벤트 콜백 |
| Physics Movement | velocity로 즉각적, AddForce로 자연스럽 |
| LitMotion | Zero-allocation 트위닝 |
| Smooth Movement | LMotion + Ease로 부드럽게 |

**핵심:** 입력 → 물리 계산 → 시각적 피드백 (LitMotion)
