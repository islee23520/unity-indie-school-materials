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
# 🎨 Session 3: Spine 애니메이션 & LitMotion 블렌딩

## Spine + LitMotion으로 만드는 생동감 있는 캐릭터 애니메이션

---

# 📚 이번 세션 목표

1. **Spine Runtime 기초** - Unity에서 Spine 사용하기
2. **Animation State 관리** - 상태 머신 구현
3. **LitMotion 블렌딩** - 물리 기반 애니메이션 보간
4. **실전 적용** - 코드 예제와 AI 프롬프트

---

<!-- _class: lead -->
# Part 1: Spine 애니메이션 기초

---

# Spine이란?

**Spine**은 2D 스켈레톤 애니메이션 도구

- **Bone 기반 애니메이션** - 메시 변형 없이 자연스러운 움직임
- **메모리 효율적** - 스프라이트 시트보다 적은 리소스
- **실시간 블렌딩** - 런타임에서 애니메이션 믹싱

```csharp
// Spine Unity 패키지 설치
// Window → Package Manager → Add package from git URL
// https://github.com/EsotericSoftware/spine-runtimes.git?path=spine-unity
```

---

# Spine 런타임 설정

## 1. 프로젝트에 Spine 임포트

```csharp
using Spine;
using Spine.Unity;

public class SpineCharacter : MonoBehaviour
{
    [SpineAnimation] public string idleAnimation;
    [SpineAnimation] public string walkAnimation;
    
    private SkeletonAnimation skeletonAnimation;
    
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
    }
}
```

---

# SkeletonAnimation 구성 요소

| 컴포넌트 | 설명 |
|---------|------|
| `SkeletonDataAsset` | Spine 파일(.json/.skel) 참조 |
| `AnimationState` | 애니메이션 재생/전환 제어 |
| `Skeleton` | 뼈대(bone)와 슬롯 데이터 |
| `MeshRenderer` | 실제 렌더링 처리 |

```csharp
// 기본 애니메이션 재생
skeletonAnimation.AnimationState.SetAnimation(
    trackIndex: 0,           // 트랙 번호
    animationName: "idle",   // 애니메이션 이름
    loop: true               // 반복 여부
);
```

---

<!-- _class: lead -->
# Part 2: Animation State 관리

---

# Animation State 기초

**Track** 시스템으로 애니메이션 레이어 관리

```csharp
public class AnimationController : MonoBehaviour
{
    private SkeletonAnimation spine;
    
    // 트랙 상수 정의
    private const int TRACK_BASE = 0;      // 기본 (idle, walk)
    private const int TRACK_ACTION = 1;    // 액션 (attack, jump)
    private const int TRACK_FACIAL = 2;    // 표정 (blink, talk)
    
    void PlayAttack()
    {
        // 기본 트랙은 유지하고 액션 트랙에 겹쳐서 재생
        spine.AnimationState.SetAnimation(TRACK_ACTION, "attack", false);
    }
}
```

---

# 상태 머신 패턴

```csharp
public enum CharacterState
{
    Idle, Walk, Attack, Hit, Die
}

public class CharacterStateMachine : MonoBehaviour
{
    private CharacterState currentState;
    private SkeletonAnimation spine;
    
    public void ChangeState(CharacterState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        
        switch (newState)
        {
            case CharacterState.Idle:
                PlayAnimation("idle", true);
                break;
            case CharacterState.Walk:
                PlayAnimation("walk", true);
                break;
            case CharacterState.Attack:
                PlayAnimation("attack", false, OnAttackComplete);
                break;
        }
    }
    
    void PlayAnimation(string name, bool loop, System.Action callback = null)
    {
        var track = spine.AnimationState.SetAnimation(0, name, loop);
        if (callback != null)
            track.Complete += entry => callback();
    }
}
```

---

# 애니메이션 전환 블렌딩

```csharp
public void TransitionTo(string animationName, float blendDuration = 0.3f)
{
    var state = skeletonAnimation.AnimationState;
    
    // 현재 트랙의 애니메이션 가져오기
    var currentTrack = state.GetCurrent(0);
    
    if (currentTrack != null)
    {
        // 부드러운 전환을 위한 mixDuration 설정
        state.Data.SetMix(currentTrack.Animation.Name, animationName, blendDuration);
    }
    
    // 새 애니메이션으로 전환
    state.SetAnimation(0, animationName, true);
}
```

---

# 이벤트 기반 애니메이션

Spine 이벤트로 정확한 타이밍 제어

```csharp
void SetupEventListeners()
{
    var state = skeletonAnimation.AnimationState;
    
    // 애니메이션 시작
    state.Start += (trackEntry) => {
        Debug.Log($"Started: {trackEntry.Animation.Name}");
    };
    
    // 애니메이션 완료
    state.Complete += (trackEntry) => {
        Debug.Log($"Completed: {trackEntry.Animation.Name}");
    };
    
    // Spine 이벤트 (에디터에서 설정한 이벤트)
    state.Event += (trackEntry, e) => {
        if (e.Data.Name == "footstep")
            PlayFootstepSound();
        else if (e.Data.Name == "damage")
            ApplyDamage();
    };
}
```

---

<!-- _class: lead -->
# Part 3: LitMotion 블렌딩

---

# LitMotion 소개

**LitMotion**은 Unity용 고성능 트위닝 라이브러리

- **Zero Allocation** - GC 없는 애니메이션
- **Burst Compiler** 지원 - 최적화된 성능
- **Spine과 완벽 호환** - 런타임 수치 보간

```csharp
// LitMotion 설치
// Package Manager → Add package from git URL
// https://github.com/annulusgames/LitMotion.git?path=src/LitMotion/Assets/LitMotion
```

---

# LitMotion 기본 사용법

```csharp
using LitMotion;
using LitMotion.Extensions;

public class LitMotionExample : MonoBehaviour
{
    void ScaleBounce()
    {
        // 스케일 1 → 1.2 → 1 bounce
        LMotion.Create(1f, 1.2f, 0.1f)
            .WithEase(Ease.OutQuad)
            .BindToScale(transform)
            .AddTo(this);
        
        // 체이닝
        LMotion.Create(1.2f, 1f, 0.2f)
            .WithDelay(0.1f)
            .WithEase(Ease.OutElastic)
            .BindToScale(transform)
            .AddTo(this);
    }
}
```

---

# Spine + LitMotion 통합

```csharp
public class SpineLitMotionBlend : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spine;
    
    // Spine bone에 LitMotion 적용
    public void ShakeBone(string boneName, float intensity, float duration)
    {
        var bone = spine.Skeleton.FindBone(boneName);
        if (bone == null) return;
        
        var startX = bone.X;
        var startY = bone.Y;
        
        // 물리 기반 흔들림
        LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.OutElastic)
            .WithOnUpdate(t => {
                bone.X = startX + Random.Range(-intensity, intensity) * (1 - t);
                bone.Y = startY + Random.Range(-intensity, intensity) * (1 - t);
            })
            .RunWithoutBinding()
            .AddTo(this);
    }
}
```

---

# 실전 예제: 데미지 효과

```csharp
public class DamageEffect : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spine;
    [SerializeField] private Color hitColor = Color.red;
    
    public void PlayDamageEffect()
    {
        var skeleton = spine.Skeleton;
        var originalColor = skeleton.GetColor();
        
        // 1. 흰색 플래시
        LMotion.Create(Color.white, hitColor, 0.05f)
            .WithOnUpdate(color => skeleton.SetColor(color))
            .RunWithoutBinding();
        
        // 2. 뒤로 넉백 (LitMotion으로 spine 위치 보간)
        var startPos = spine.transform.position;
        var knockbackPos = startPos + Vector3.left * 0.5f;
        
        LMotion.Create(startPos, knockbackPos, 0.1f)
            .WithEase(Ease.OutQuad)
            .BindToPosition(spine.transform);
        
        // 3. 원위치 복귀
        LMotion.Create(knockbackPos, startPos, 0.3f)
            .WithDelay(0.1f)
            .WithEase(Ease.OutElastic)
            .BindToPosition(spine.transform);
        
        // 4. 색상 복귀
        LMotion.Create(hitColor, originalColor, 0.2f)
            .WithDelay(0.1f)
            .WithOnUpdate(color => skeleton.SetColor(color))
            .RunWithoutBinding();
    }
}
```

---

# Animation Mixing 심화

```csharp
public void SetupAdvancedMixing()
{
    var stateData = skeletonAnimation.AnimationState.Data;
    
    // 전역 기본 믹스 타임
    stateData.DefaultMix = 0.2f;
    
    // 특정 애니메이션 쌍별 믹스 설정
    stateData.SetMix("idle", "walk", 0.3f);
    stateData.SetMix("walk", "run", 0.15f);
    stateData.SetMix("any", "attack", 0.1f);  // 공격은 빠른 전환
    stateData.SetMix("attack", "idle", 0.4f);  // 공격 후 자연스럽게
}
```

---

# IK (Inverse Kinematics) 활용

```csharp
public void SetupIK()
{
    // IK 타겟 설정 (예: 총구 방향 조준)
    var ikConstraint = skeletonAnimation.Skeleton.FindIkConstraint("aim");
    
    if (ikConstraint != null)
    {
        // 타겟 위치 업데이트
        UpdateAimTarget();
        
        // LitMotion으로 부드러운 IK 전환
        LMotion.Create(0f, 1f, 0.3f)
            .WithEase(Ease.OutQuad)
            .WithOnUpdate(weight => {
                ikConstraint.Mix = weight;
            })
            .RunWithoutBinding();
    }
}
```

---

<!-- _class: lead -->
# Part 4: AI 프롬프트 가이드

---

# Spine 애니메이션 시스템 프롬프트

## 캐릭터 컨트롤러 생성

```
Unity Spine 캐릭터 컨트롤러를 만들어줘.

요구사항:
- 상태 머신 패턴 사용 (Idle, Walk, Run, Attack, Hit)
- 애니메이션 블렌딩 지원 (0.2초 기본 믹스)
- 이벤트 기반 공격 판정
- 공격 중 이동 불가 로직

Spine Runtime API 사용:
- SkeletonAnimation
- AnimationState
- TrackEntry
```

---

# 복합 애니메이션 프롬프트

```
Spine 캐릭터에 다음 기능을 추가해줘:

1. 상체/하체 분리 애니메이션
   - 하체: Walk/Run (Track 0)
   - 상체: Idle/Attack (Track 1)

2. 방향 전환시 부드러운 뒤집기
   - LitMotion으로 scale.x 1 → -1 보간
   - 0.15초 duration, Ease.OutQuad

3. 피격 반응
   - Spine SetColor로 흰색 플래시
   - 랜덤한 각도로 넉백
   - 0.3초 내 복귀
```

---

# 효과 시스템 프롬프트

```
Spine 애니메이션 이벤트 기반 효과 시스템을 만들어줘:

기능:
1. Spine 이벤트 리스너 등록
2. 이벤트 이름별 콜백 매핑
3. 이벤트 타임라인:
   - "swing_start": 검 휘두르기 시작
   - "hit_frame": 데미지 판정
   - "swing_end": 애니메이션 종료

4. LitMotion으로 동적 이펙트:
   - 검 궤적 잔상
   - 타격 지점 파티클
   - 화면 흔들림 (칵테일 효과)
```

---

<!-- _class: lead -->
# Part 5: 키워드 체크리스트

---

# 필수 키워드 정리

| 한글 | 영문 | 설명 |
|-----|-----|-----|
| 뼈대 | Skeleton | Spine의 bone 계층 구조 |
| 애니메이션 상태 | AnimationState | 애니메이션 재생/전환 관리 |
| 트랙 | Track | 애니메이션 레이어 |
| 블렌딩 | Mix/Blend | 애니메이션 간 부드러운 전환 |
| IK | Inverse Kinematics | 역욱법으로 끝점 기반 조작 |
| 슬롯 | Slot | attachment(이미지) 배치 위치 |

---

# 코드 키워드

```csharp
// Spine Core
SkeletonAnimation      // Spine Unity 컴포넌트
AnimationState         // 애니메이션 상태 머신
TrackEntry            // 특정 트랙의 애니메이션 인스턴스
Skeleton              // bone, slot 데이터
SetAnimation()        // 애니메이션 설정
SetMix()             // 블렌딩 설정
AddAnimation()       // 큐에 애니메이션 추가

// LitMotion
LMotion.Create()     // 모션 생성
BindToPosition()     // 위치 바인딩
BindToScale()        // 스케일 바인딩
WithEase()           // 이징 함수
WithDelay()          // 지연 시간
RunWithoutBinding()  // 직접 값 업데이트
```

---

# LitMotion 이징 함수

| 이징 | 용도 |
|-----|-----|
| `Linear` | 일정한 속도 |
| `OutQuad` | 자연스러운 감속 |
| `OutElastic` | 통통 튀는 효과 |
| `OutBounce` | 바운스 효과 |
| `InOutCubic` | 부드러운 가감속 |
| `Shake` | 진동/흔들림 효과 |

---

<!-- _class: lead -->
# 🎯 실습 과제

---

# 미션: 콤보 공격 시스템

**목표**: Spine + LitMotion으로 콤보 공격 구현

```csharp
// 요구사항
1. 3단 콤보 애니메이션 (attack_1 → attack_2 → attack_3)
2. 콤보 타이밍: 이전 공격 종료 0.3초 내 다음 입력
3. 각 공격마다 다른 히트스톱(얼림) 효과
4. LitMotion으로 칼날 궤적 그리기
5. 마지막 공격에 칵테일(화면 흔들림) 효과
```

**제출**: GitHub 레포 링크 + 짧은 시연 영상

---

<!-- _class: lead -->
# Q & A

질문 있으신가요?

---

# 참고 자료

- **Spine 문서**: http://esotericsoftware.com/spine-unity
- **LitMotion GitHub**: https://github.com/annulusgames/LitMotion
- **Spine 런타임 예제**: spine-runtimes/spine-unity/Examples

---

<!-- _class: lead -->
# 감사합니다!

## 다음 세션: Session 4 - 어드레서블 시스템
