# Session 12: 메뉴 시스템 & Animator

## 화면 전환·Animator Override Controller·스킨 변경

# Menu System & Animator Workflow

---

# 학습 목표

- Unity UI 기반 메뉴 시스템 설계
- 화면 전환 효과 구현
- Animator Override Controller 활용
- 런타임 본 조작으로 동적 애니메이션
- AI 프롬프트로 애니메이션 에셋 생성

---

# 1. 메뉴 시스템 설계

---

## 메뉴 시스템 아키텍처

```
┌─────────────────────────────────────┐
│         MenuManager (Singleton)     │
│  - 현재 화면 추적                    │
│  - 화면 전환 관리                    │
│  - 히스토리 스택 관리                 │
└─────────────┬───────────────────────┘
              │
    ┌─────────┼─────────┐
    ▼         ▼         ▼
┌───────┐ ┌───────┐ ┌───────┐
│ Main  │ │ Shop  │ │Pause  │
│Menu   │ │Screen │ │Menu   │
└───────┘ └───────┘ └───────┘
```

---

## MenuManager 기본 구조

```csharp
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    [SerializeField] private MenuScreen[] menuScreens;
    [SerializeField] private ScreenTransition defaultTransition;
    
    private Stack<MenuScreen> history = new();
    private MenuScreen currentScreen;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
```

---

## MenuScreen 기본 클래스

```csharp
public abstract class MenuScreen : MonoBehaviour
{
    [SerializeField] protected RectTransform content;
    [SerializeField] private bool hideOnStart = true;
    
    public bool IsVisible => content.gameObject.activeSelf;
    
    public virtual void Show()
    {
        content.gameObject.SetActive(true);
        OnShow();
    }
    
    public virtual void Hide()
    {
        OnHide();
        content.gameObject.SetActive(false);
    }
    
    protected abstract void OnShow();
    protected abstract void OnHide();
}
```

---

# 2. 화면 전환 효과

---

## ScreenTransition 기본 클래스

```csharp
public abstract class ScreenTransition : ScriptableObject
{
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected AnimationCurve easeCurve = 
        AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    public float Duration => duration;
    
    public abstract IEnumerator AnimateIn(RectTransform screen);
    public abstract IEnumerator AnimateOut(RectTransform screen);
}
```

---

## 페이드 전환 구현

```csharp
[CreateAssetMenu(fileName = "FadeTransition", 
    menuName = "Menu/Fade Transition")]
public class FadeTransition : ScreenTransition
{
    public override IEnumerator AnimateIn(RectTransform screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = screen.gameObject.AddComponent<CanvasGroup>();
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
```

---

## 슬라이드 전환 구현

```csharp
[CreateAssetMenu(fileName = "SlideTransition", 
    menuName = "Menu/Slide Transition")]
public class SlideTransition : ScreenTransition
{
    [SerializeField] private SlideDirection direction = SlideDirection.Right;
    
    public override IEnumerator AnimateIn(RectTransform screen)
    {
        Vector2 startPos = GetStartPosition(screen);
        Vector2 endPos = Vector2.zero;
        
        screen.anchoredPosition = startPos;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / duration);
            screen.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        screen.anchoredPosition = endPos;
    }
    
    Vector2 GetStartPosition(RectTransform screen)
    {
        return direction switch
        {
            SlideDirection.Left => new Vector2(-screen.rect.width, 0),
            SlideDirection.Right => new Vector2(screen.rect.width, 0),
            SlideDirection.Up => new Vector2(0, screen.rect.height),
            SlideDirection.Down => new Vector2(0, -screen.rect.height),
            _ => Vector2.zero
        };
    }
}
```

---

## MenuManager 전환 메서드

```csharp
public class MenuManager : MonoBehaviour
{
    public void OpenScreen(string screenName, 
        ScreenTransition transition = null)
    {
        MenuScreen target = Array.Find(menuScreens, 
            s => s.name == screenName);
        if (target == null) return;
        
        transition ??= defaultTransition;
        
        if (currentScreen != null)
        {
            StartCoroutine(SwitchScreens(currentScreen, target, transition));
        }
        else
        {
            StartCoroutine(ShowScreen(target, transition));
        }
    }
    
    IEnumerator SwitchScreens(MenuScreen from, MenuScreen to, 
        ScreenTransition transition)
    {
        yield return StartCoroutine(transition.AnimateOut(from.Content));
        from.Hide();
        
        to.Show();
        yield return StartCoroutine(transition.AnimateIn(to.Content));
        
        history.Push(from);
        currentScreen = to;
    }
}
```

---

# 3. Animator Override Controller

---

## Animator Override란?

```
기본 Animator Controller를 유지하면서
특정 클립만 교체할 수 있는 시스템

┌─────────────────┐      ┌─────────────────────┐
│ Base Controller │      │ Override Controller │
│                 │      │                     │
│ Idle   ─────────┼──────┼───► New Idle Clip   │
│ Walk   ─────────┼──────┼───► Keep Original   │
│ Attack ─────────┼──────┼───► New Attack Clip │
│                 │      │                     │
└─────────────────┘      └─────────────────────┘
```

---

## 런타임 클립 교체

```csharp
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController baseController;
    
    private AnimatorOverrideController overrideController;
    
    void Start()
    {
        // Override Controller 생성
        overrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = overrideController;
    }
    
    public void ReplaceIdleAnimation(AnimationClip newIdleClip)
    {
        // "Idle" 상태의 클립을 교체
        overrideController["Idle"] = newIdleClip;
    }
}
```

---

## 무기별 애니메이션 교체

```csharp
public class WeaponAnimationSystem : MonoBehaviour
{
    [SerializeField] private WeaponData[] weapons;
    [SerializeField] private Animator animator;
    
    private AnimatorOverrideController overrideController;
    private Dictionary<string, AnimationClip> originalClips = new();
    
    void Start()
    {
        overrideController = new AnimatorOverrideController(
            animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        
        // 원본 클립 저장
        SaveOriginalClips();
    }
    
    public void EquipWeapon(WeaponType type)
    {
        WeaponData weapon = Array.Find(weapons, w => w.Type == type);
        if (weapon == null) return;
        
        // 각 상태별 클립 교체
        overrideController["Attack"] = weapon.AttackClip;
        overrideController["Idle_Combat"] = weapon.IdleClip;
        overrideController["Reload"] = weapon.ReloadClip;
    }
}
```

---

## ScriptableObject로 무기 데이터 관리

```csharp
[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType Type;
    public float damage;
    public float attackSpeed;
    
    [Header("Animations")]
    public AnimationClip IdleClip;
    public AnimationClip AttackClip;
    public AnimationClip ReloadClip;
    public AnimationClip EquipClip;
}
```

---

# 4. 런타임 본 조작

---

## 본 조작이란?

```
스켈레톤의 개별 본(Bone)을 런타임에
회전/이동시켜 동적 애니메이션 효과 구현

예시:
- 캐릭터가 타겟을 바라보기
- 무기가 손에 맞게 위치 조정
- IK(역욕학)로 발 위치 보정
- 망토나 머리칼 물리 시뮬레이션
```

---

## 본 캐싱 최적화

```csharp
public class BoneController : MonoBehaviour
{
    private Animator animator;
    private Transform headBone;
    private Transform rightHandBone;
    private Transform spineBone;
    
    // 본 해시 ID (성능 최적화)
    private readonly int headHash = Animator.StringToHash("Head");
    private readonly int rightHandHash = Animator.StringToHash("RightHand");
    
    void Start()
    {
        animator = GetComponent<Animator>();
        CacheBones();
    }
    
    void CacheBones()
    {
        // GetBoneTransform으로 주요 본 캐싱
        headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        rightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        spineBone = animator.GetBoneTransform(HumanBodyBones.Spine);
    }
}
```

---

## 머리 방향 제어 (Look At)

```csharp
public class HeadTracking : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxRotationAngle = 60f;
    
    private Transform headBone;
    private Quaternion initialRotation;
    
    void OnAnimatorIK(int layerIndex)
    {
        if (target == null) return;
        
        // 목표 방향 계산
        Vector3 direction = target.position - headBone.position;
        direction.y = 0; // Y축 회전만
        
        // 각도 제한
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);
        
        // 부드러운 회전
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0) * initialRotation;
        headBone.rotation = Quaternion.Slerp(
            headBone.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }
}
```

---

## 무기 위치 동적 조정

```csharp
public class WeaponAttachment : MonoBehaviour
{
    [SerializeField] private Transform weaponPrefab;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    
    private Transform rightHand;
    private Transform attachedWeapon;
    
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        AttachWeapon();
    }
    
    void AttachWeapon()
    {
        attachedWeapon = Instantiate(weaponPrefab, rightHand);
        attachedWeapon.localPosition = positionOffset;
        attachedWeapon.localRotation = Quaternion.Euler(rotationOffset);
        attachedWeapon.localScale = Vector3.one;
    }
    
    // 런타ime에 위치 미세 조정
    public void AdjustWeaponPosition(Vector3 newOffset)
    {
        positionOffset = newOffset;
        if (attachedWeapon != null)
            attachedWeapon.localPosition = positionOffset;
    }
}
```

---

## IK로 발 위치 보정

```csharp
public class FootIK : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f;
    
    private Animator animator;
    
    void OnAnimatorIK(int layerIndex)
    {
        // 왼발 IK
        AdjustFootIK(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee);
        
        // 오른발 IK
        AdjustFootIK(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee);
    }
    
    void AdjustFootIK(AvatarIKGoal foot, AvatarIKHint knee)
    {
        // 현재 발 위치에서 아래로 Raycast
        Vector3 footPos = animator.GetIKPosition(foot);
        
        if (Physics.Raycast(footPos + Vector3.up, Vector3.down, 
            out RaycastHit hit, rayDistance, groundLayer))
        {
            // 발 위치를 지면에 맞춤
            Vector3 targetPos = hit.point + Vector3.up * footOffset;
            animator.SetIKPosition(foot, targetPos);
            
            // 발 회전을 지면 기울기에 맞춤
            Quaternion targetRot = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, hit.normal), 
                hit.normal
            );
            animator.SetIKRotation(foot, targetRot);
            animator.SetIKPositionWeight(foot, 1f);
            animator.SetIKRotationWeight(oot, 1f);
        }
    }
}
```

---

# 5. 고급 애니메이션 기법

---

## 애니메이션 이벤트

```csharp
public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem attackEffect;
    [SerializeField] private AudioSource audioSource;
    
    // 애니메이션 클립에서 호출할 이벤트
    public void OnAttackHit()
    {
        // 공격 판정 시점
        attackEffect.Play();
        audioSource.PlayOneShot(attackSound);
        
        // 데미지 적용
        DealDamageToTarget();
    }
    
    public void OnFootstep()
    {
        // 발소리 재생
        AudioClip footstep = GetRandomFootstep();
        audioSource.PlayOneShot(footstep, 0.5f);
    }
    
    public void OnAnimationComplete()
    {
        // 애니메이션 종료 콜백
        OnActionFinished?.Invoke();
    }
}
```

---

## 애니메이션 블렌딩

```csharp
public class AnimationBlending : MonoBehaviour
{
    private Animator animator;
    private int locomotionBlendHash;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        locomotionBlendHash = Animator.StringToHash("LocomotionBlend");
    }
    
    void Update()
    {
        float speed = new Vector3(
            Input.GetAxis("Horizontal"), 
            0, 
            Input.GetAxis("Vertical")
        ).magnitude;
        
        // Blend Tree 파라미터 업데이트
        // 0 = Idle, 0.5 = Walk, 1 = Run
        animator.SetFloat(locomotionBlendHash, speed, 0.1f, Time.deltaTime);
    }
}
```

---

## 애니메이션 레이어 활용

```csharp
public class LayeredAnimation : MonoBehaviour
{
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        // 상체 레이어 설정 (무게 1)
        animator.SetLayerWeight(1, 1f);
    }
    
    public void PlayUpperBodyAction(string actionName)
    {
        // 레이어 1 (Upper Body)에서만 재생
        // 하체는 기존 애니메이션 유지
        animator.Play(actionName, 1, 0f);
    }
    
    public void SetAiming(bool isAiming)
    {
        // 조준 레이어 블렌딩
        float targetWeight = isAiming ? 1f : 0f;
        float currentWeight = animator.GetLayerWeight(2);
        
        // 부드러운 전환
        StartCoroutine(BlendLayerWeight(2, currentWeight, targetWeight));
    }
}
```

---

# 6. AI 프롬프트로 애니메이션 생성

---

## AI 애니메이션 생성 도구

```
사용 가능한 AI 도구:

1. Mixamo (Adobe)
   - 텍스트 입력으로 캐릭터 자동 리깅
   - 수천 개의 모션 캡처 클립

2. Cascadeur
   - AI 보조로 키프레임 애니메이션
   - 물리 기반 애니메이션 시뮬레이션

3. DeepMotion
   - 비디오를 3D 애니메이션으로 변환

4. Krikey AI
   - 텍스트 프롬프트로 애니메이션 생성
```

---

## Cascadeur 프롬프트 예시

```
목표: RPG 캐릭터 공격 애니메이션 생성

프롬프트 구조:
1. 캐릭터 타입: "Dual-wielding rogue character"
2. 액션: "Quick dagger slash combo, 3 hits"
3. 스타일: "Fast, agile, assassin-like movement"
4. 참조: "Similar to Dishonored combat style"
5. 기술적: "Root motion enabled, 60fps, 2 seconds"

전체 프롬프트:
"Create a fast-paced dual dagger attack combo 
for a rogue character. Three quick slashes with 
agile footwork. Style inspired by Dishonored. 
Include anticipation and recovery frames. 
Root motion, 60fps, 2 second duration."
```

---

## Krikey AI 프롬프트 예시

```
애니메이션 생성 프롬프트:

캐릭터: "Female warrior"
액션: "Victory celebration"
세부사항: "Raises sword above head, triumphant pose, 
          slight jump with fist pump"
스타일: "Energetic, heroic, fantasy RPG style"

생성된 결과를 FBX로 export 후
Unity에서 AnimationClip으로 import
```

---

## Mixamo 활용 워크플로우

```
1. 캐릭터 모델 준비
   └── T-포즈, 바이폴 메시

2. Mixamo Auto-Rigger
   └── "Create Custom Character" 업로드
   └── 자동으로 스켈레톤 생성

3. 애니메이션 검색/다운로드
   └── "idle", "walk", "run", "attack" 검색
   └── 필요한 클립 선택 후 다운로드

4. Unity Import
   └── FBX 파일 Import
   └── Rig 설정: Humanoid
   └── Animation 탭에서 클립 설정
```

---

## AI 생성 클립 후처리

```csharp
public class AnimationPostProcessor : AssetPostprocessor
{
    void OnPreprocessAnimation()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        
        // 모든 애니메이션 클립에 대한 설정
        ModelImporterClipAnimation[] clips = 
            modelImporter.defaultClipAnimations;
        
        foreach (var clip in clips)
        {
            // 루프 설정
            clip.loopTime = clip.name.Contains("Idle") || 
                           clip.name.Contains("Walk") ||
                           clip.name.Contains("Run");
            
            // 이벤트 추가
            if (clip.name.Contains("Attack"))
            {
                clip.events = new AnimationEvent[]
                {
                    new AnimationEvent()
                    {
                        functionName = "OnAttackHit",
                        time = clip.length * 0.5f
                    }
                };
            }
        }
        
        modelImporter.clipAnimations = clips;
    }
}
```

---

# 실습 과제

---

## 과제 1: 메뉴 시스템 구현

```
요구사항:
□ MenuManager 싱글톤 구현
□ 최소 3개의 화면 (Main, Settings, Game)
□ 히스토리 관리 (뒤로 가기 기능)
□ Fade + Slide 전환 효과

채점 기준:
- 화면 전환이 부드러운가?
- 히스토리가 정상 작동하는가?
- 전환 중 입력이 차단되는가?
```

---

## 과제 2: 무기 애니메이션 시스템

```
요구사항:
□ WeaponData ScriptableObject 생성
□ 3가지 이상의 무기 타입
□ Animator Override로 클립 교체
□ 무기 장착/해제 애니메이션

채점 기준:
- 무기 교체가 자연스러운가?
- Override가 정상 적용되는가?
- 애니메이션 이벤트가 작동하는가?
```

---

## 과제 3: IK 머리 추적

```
요구사항:
□ IK로 머리 방향 제어
□ 각도 제한 (±60도)
□ 부드러운 보간
□ 타겟 전환 기능

채점 기준:
- 자연스러운 머리 움직임
- 각도 제한이 적용되는가?
- 타겟 변경이 부드러운가?
```

---

## 과제 4: AI 애니메이션 활용

```
요구사항:
□ Mixamo 또는 Cascadeur에서 클립 다운로드
□ Unity에 Import 및 설정
□ 기존 Animator와 통합
□ 커스텀 이벤트 추가

채점 기준:
- 클립이 정상 재생되는가?
- Humanoid 설정이 올바른가?
- 루프/이벤트 설정이 되어있는가?
```

---

# 참고 자료

---

## 공식 문서

- [Unity UI System](https://docs.unity3d.com/Manual/UI-system.html)
- [Animator Override Controller](https://docs.unity3d.com/Manual/AnimatorOverrideController.html)
- [Inverse Kinematics](https://docs.unity3d.com/Manual/InverseKinematics.html)
- [Animation Scripting](https://docs.unity3d.com/Manual/AnimationScripting.html)

---

## AI 도구 링크

- **Mixamo**: https://www.mixamo.com
- **Cascadeur**: https://cascadeur.com
- **DeepMotion**: https://www.deepmotion.com
- **Krikey AI**: https://www.krikey.ai

---

# Q&A

---

## 질문 환영합니다

- 메뉴 시스템 구현의 어려운 점
- Animator Override 사용 시 주의사항
- IK 성능 최적화 방법
- AI 생성 애니메이션 품질 향상

---

# 다음 세션 예고

## Week 07 - Session 13

### Session 13: 최적화 & 폴리싱 — Sprite Atlas·Draw Call·Frame Debugger·보스 AI (2페이즈)

- 씬 로딩/언로딩
- Addressables 시스템
- 에셋 번들 빌드
- 동적 에셋 로드

---

# 수고하셨습니다!
