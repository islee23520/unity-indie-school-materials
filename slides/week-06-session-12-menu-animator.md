# Session 12: 메뉴 시스템 & Animator

## 화면 전환·Animator Override Controller·스킨 변경

# Menu System & Animator Workflow

---

# 학습 목표

- UI Toolkit 기반 메뉴 화면을 VContainer DI로 조립
- R3 ViewModel로 현재 화면과 뒤로가기 상태 관리
- LitMotion 트윈으로 화면 전환 효과 구현
- Animator Override Controller 활용
- 런타임 본 조작으로 동적 애니메이션
- AI 프롬프트로 애니메이션 에셋 생성

---

# 1. 메뉴 시스템 설계

---

## 메뉴 시스템 아키텍처

```text
┌──────────────────────────────────────┐
│      MenuLifetimeScope (VContainer)  │
│  ┌────────────┐  ┌───────────────┐  │
│  │ MenuManager│  │ MenuViewModel │  │
│  │ (Presenter)│  │ (R3 State)    │  │
│  └─────┬──────┘  └───────┬───────┘  │
│        │                 │          │
│   ┌────┼─────────┐       │          │
│   ▼    ▼         ▼       │          │
│ ┌───┐┌───┐┌───┐   │          │
│ │Main││Shop││Pause│ ◄─┘          │
│ └───┘└───┘└───┘              │
└──────────────────────────────────────┘
```

---

## MenuViewModel + MenuManager (MVVM)

```csharp
// MenuViewModel: R3 반응형 상태 관리
public sealed class MenuViewModel : IDisposable
{
    private readonly Stack<string> _history = new();
    private readonly ReactiveProperty<int> _historyCount = new(0);

    public ReactiveProperty<string> CurrentScreenName { get; } = new(string.Empty);
    public ReadOnlyReactiveProperty<bool> CanGoBack { get; }

    public MenuViewModel()
    {
        CanGoBack = _historyCount
            .Select(count => count > 0)
            .ToReadOnlyReactiveProperty();
    }

    public void RequestOpen(string screenName)
    {
        // 히스토리 push + 상태 변경
    }

    public void RequestBack()
    {
        // 히스토리 pop + 상태 변경
    }
}
```

---

## MenuManager Presenter

```csharp
// MenuManager: VContainer IStartable Presenter
public sealed class MenuManager : IStartable, IDisposable
{
    [Inject]
    public MenuManager(
        UIDocument uiDocument,
        IReadOnlyList<IMenuScreen> menuScreens,
        ScreenTransition defaultTransition,
        MenuViewModel viewModel)
    {
        // DI로 UI 문서, 화면 목록, 전환, ViewModel 주입
    }

    public void Start()
    {
        // 화면 바인딩 + ViewModel 구독
        _viewModel.CurrentScreenName
            .Subscribe(screenName => OpenScreenAsync(screenName).Forget())
            .AddTo(ref _disposables);
    }
}
```

---

## IMenuScreen + MenuScreenDefinition

```csharp
// 화면 인터페이스 (VContainer 등록용)
public interface IMenuScreen
{
    string ScreenName { get; }
    string ContentElementName { get; }
    VisualElement Content { get; }
    void Bind(VisualElement content);
    void Show();
    void Hide();
}

// 직렬화 가능한 화면 정의 (LifetimeScope에 [SerializeField]로 등록)
[Serializable]
public sealed class MenuScreenDefinition : IMenuScreen
{
    [SerializeField] private string screenName;
    [SerializeField] private string contentElementName;

    public string ScreenName => screenName;
    public string ContentElementName => contentElementName;
    public VisualElement Content { get; private set; }

    public void Bind(VisualElement content)
    {
        Content = content;
    }

    public void Show()
    {
        Content.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Content.style.display = DisplayStyle.None;
    }
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

    public abstract UniTask AnimateInAsync(VisualElement screen);
    public abstract UniTask AnimateOutAsync(VisualElement screen);
}
```

---

## 페이드 전환 구현

```csharp
[CreateAssetMenu(fileName = "FadeTransition", menuName = "Menu/Fade Transition")]
public class FadeTransition : ScreenTransition
{
    public override async UniTask AnimateInAsync(VisualElement screen)
    {
        screen.style.display = DisplayStyle.Flex;
        screen.style.opacity = 0f;

        await LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.OutQuad)
            .BindWithState(screen, (value, element) => element.style.opacity = value)
            .ToUniTask();
    }

    public override async UniTask AnimateOutAsync(VisualElement screen)
    {
        await LMotion.Create(1f, 0f, duration)
            .WithEase(Ease.OutQuad)
            .BindWithState(screen, (value, element) => element.style.opacity = value)
            .ToUniTask();

        screen.style.display = DisplayStyle.None;
    }
}
```

---

## 슬라이드 전환 구현

```csharp
[CreateAssetMenu(fileName = "SlideTransition", menuName = "Menu/Slide Transition")]
public sealed class SlideTransition : ScreenTransition
{
    [SerializeField] private SlideDirection direction = SlideDirection.Right;

    public override async UniTask AnimateInAsync(VisualElement screen)
    {
        screen.style.display = DisplayStyle.Flex;
        screen.style.translate = new Translate(GetStartOffset(), 0);

        await LMotion.Create(GetStartOffset(), 0f, duration)
            .WithEase(Ease.OutQuad)
            .BindWithState(screen, (value, element) =>
                element.style.translate = new Translate(value, 0))
            .ToUniTask();
    }

    public override async UniTask AnimateOutAsync(VisualElement screen)
    {
        await LMotion.Create(0f, GetStartOffset(), duration)
            .WithEase(Ease.OutQuad)
            .BindWithState(screen, (value, element) =>
                element.style.translate = new Translate(value, 0))
            .ToUniTask();

        screen.style.display = DisplayStyle.None;
        screen.style.translate = new Translate(0, 0);
    }

    private float GetStartOffset()
    {
        return direction switch
        {
            SlideDirection.Left => -640f,
            SlideDirection.Right => 640f,
            _ => 0f
        };
    }
}
```

---

## MenuManager 전환 메서드

```csharp
public sealed class MenuManager : IStartable, IDisposable
{
    private readonly Dictionary<string, IMenuScreen> _lookup = new();
    private readonly DisposableBag _disposables = new();

    [Inject]
    public MenuManager(
        UIDocument uiDocument,
        IReadOnlyList<IMenuScreen> menuScreens,
        ScreenTransition defaultTransition,
        MenuViewModel viewModel)
    {
        // DI로 필요한 의존성을 받는다
    }

    public void Start()
    {
        _root = _uiDocument.rootVisualElement;

        foreach (IMenuScreen screen in _menuScreens)
        {
            VisualElement content = _root.Q<VisualElement>(screen.ContentElementName);
            screen.Bind(content);
            screen.Hide();
            _lookup[screen.ScreenName] = screen;
        }

        // ViewModel 상태 변화 → 화면 전환
        _viewModel.CurrentScreenName
            .Subscribe(screenName => OpenScreenAsync(screenName).Forget())
            .AddTo(ref _disposables);
    }
}
```

---

## MenuLifetimeScope 설정

```csharp
public sealed class MenuLifetimeScope : LifetimeScope
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private ScreenTransition defaultTransition;
    [SerializeField] private MenuScreenDefinition[] menuScreens;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(uiDocument);
        builder.RegisterInstance(defaultTransition);

        foreach (MenuScreenDefinition screen in menuScreens)
        {
            builder.RegisterInstance(screen).As<IMenuScreen>();
        }

        builder.RegisterInstance<IReadOnlyList<IMenuScreen>>(menuScreens);
        builder.Register<MenuViewModel>(Lifetime.Singleton);
        builder.RegisterEntryPoint<MenuManager>();
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
public sealed class CharacterSkinChanger : MonoBehaviour
{
    private Animator _animator;
    private CharacterSkinSettings _settings;
    private AnimatorOverrideController _overrideController;

    [Inject]
    public void Construct(Animator animator, CharacterSkinSettings settings)
    {
        _animator = animator;
        _settings = settings;
    }

    private void Start()
    {
        _overrideController = new AnimatorOverrideController(_settings.BaseController);
        _animator.runtimeAnimatorController = _overrideController;
    }

    public void ChangeSkin(AnimationClip idleClip, AnimationClip walkClip, AnimationClip attackClip)
    {
        _overrideController["Idle"] = idleClip;
        _overrideController["Walk"] = walkClip;
        _overrideController["Attack"] = attackClip;
    }
}
```

---

## 무기별 애니메이션 교체

```csharp
public sealed class WeaponAnimationSystem : MonoBehaviour, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private Animator _animator;
    private WeaponAnimationSettings _settings;
    private AnimatorOverrideController _overrideController;

    public ReactiveProperty<WeaponType> CurrentWeapon { get; } = new(WeaponType.None);

    [Inject]
    public void Construct(Animator animator, WeaponAnimationSettings settings)
    {
        _animator = animator;
        _settings = settings;
    }

    private void Start()
    {
        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;

        CurrentWeapon
            .Where(type => type != WeaponType.None)
            .Subscribe(ApplyWeaponAnimation)
            .AddTo(_disposables);
    }

    public void EquipWeapon(WeaponType type)
    {
        CurrentWeapon.Value = type;
    }

    private void ApplyWeaponAnimation(WeaponType type)
    {
        WeaponData weapon = Array.Find(_settings.Weapons, item => item.Type == type);
        if (weapon == null)
        {
            return;
        }

        _overrideController["Attack"] = weapon.AttackClip;
        _overrideController["Idle_Combat"] = weapon.IdleClip;
        _overrideController["Reload"] = weapon.ReloadClip;
    }
}
```

---

## ScriptableObject로 무기 데이터 관리

```csharp
public enum WeaponType
{
    None,
    Sword,
    Dagger,
    Bow,
    Staff,
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon")]
public sealed class WeaponData : ScriptableObject
{
    public string WeaponName;
    public WeaponType Type;
    public float Damage;
    public float AttackSpeed;

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
public sealed class BoneController : MonoBehaviour
{
    private readonly int _headHash = Animator.StringToHash("Head");
    private readonly int _rightHandHash = Animator.StringToHash("RightHand");
    private Animator _animator;

    public Transform HeadBone { get; private set; }
    public Transform RightHandBone { get; private set; }
    public Transform SpineBone { get; private set; }

    [Inject]
    public void Construct(Animator animator)
    {
        _animator = animator;
    }

    private void Start()
    {
        CacheBones();
    }

    private void CacheBones()
    {
        HeadBone = _animator.GetBoneTransform(HumanBodyBones.Head);
        RightHandBone = _animator.GetBoneTransform(HumanBodyBones.RightHand);
        SpineBone = _animator.GetBoneTransform(HumanBodyBones.Spine);
    }
}
```

---

## 머리 방향 제어 (Look At)

```csharp
public sealed class HeadTracking : MonoBehaviour
{
    private Transform _target;
    private HeadTrackingSettings _settings;
    private Transform _headBone;
    private Quaternion _initialRotation;
    private MotionHandle _rotationHandle;

    [Inject]
    public void Construct(Animator animator, Transform target, HeadTrackingSettings settings)
    {
        _target = target;
        _settings = settings;
        _headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        _initialRotation = _headBone.rotation;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 direction = _target.position - _headBone.position;
        direction.y = 0f;

        // 각도 제한
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle = Mathf.Clamp(angle, -_settings.MaxRotationAngle, _settings.MaxRotationAngle);

        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f) * _initialRotation;
        if (_rotationHandle.IsActive())
        {
            _rotationHandle.Cancel();
        }

        _rotationHandle = LMotion.Create(_headBone.rotation, targetRotation, _settings.RotationDuration)
            .WithEase(Ease.OutQuad)
            .WithOnUpdate(value => _headBone.rotation = value)
            .RunWithoutBinding();
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
    
    // 런타임에 위치 미세 조정
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
public sealed class FootIK : MonoBehaviour
{
    private Animator _animator;
    private FootIKSettings _settings;

    [Inject]
    public void Construct(Animator animator, FootIKSettings settings)
    {
        _animator = animator;
        _settings = settings;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        AdjustFootIK(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee);
        AdjustFootIK(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee);
    }

    private void AdjustFootIK(AvatarIKGoal foot, AvatarIKHint knee)
    {
        Vector3 footPosition = _animator.GetIKPosition(foot);
        Vector3 rayOrigin = footPosition + Vector3.up;

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
            _settings.RayDistance, _settings.GroundLayer))
        {
            _animator.SetIKPositionWeight(foot, 0f);
            _animator.SetIKRotationWeight(foot, 0f);
            _animator.SetIKHintPositionWeight(knee, 0f);
            return;
        }

        Vector3 targetPosition = hit.point + Vector3.up * _settings.FootOffset;
        Quaternion targetRotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(transform.forward, hit.normal),
            hit.normal);

        _animator.SetIKPosition(foot, targetPosition);
        _animator.SetIKRotation(foot, targetRotation);
        _animator.SetIKPositionWeight(foot, 1f);
        _animator.SetIKRotationWeight(foot, 1f);
        _animator.SetIKHintPositionWeight(knee, 0.5f);
    }
}
```

---

# 5. 고급 애니메이션 기법

---

## 애니메이션 이벤트

```csharp
public sealed class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _attackEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _attackSound;
    [SerializeField] private AudioClip[] _footstepSounds;

    public void OnAttackHit()
    {
        _attackEffect.Play();
        _audioSource.PlayOneShot(_attackSound);
        DealDamageToTarget();
    }

    public void OnFootstep()
    {
        AudioClip footstep = _footstepSounds[Random.Range(0, _footstepSounds.Length)];
        _audioSource.PlayOneShot(footstep, 0.5f);
    }

    public void OnAnimationComplete()
    {
        _animator.SetTrigger("ActionFinished");
    }

    private void DealDamageToTarget()
    {
        // 공격 판정 시스템에 데미지 적용
    }
}
```

---

## 애니메이션 블렌딩

```csharp
public sealed class AnimationBlending : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private Animator _animator;
    private AnimationInputSource _inputSource;
    private AnimationBlendingSettings _settings;
    private int _locomotionBlendHash;

    public ReactiveProperty<float> LocomotionSpeed { get; } = new(0f);

    [Inject]
    public void Construct(Animator animator, AnimationInputSource inputSource, AnimationBlendingSettings settings)
    {
        _animator = animator;
        _inputSource = inputSource;
        _settings = settings;
        _locomotionBlendHash = Animator.StringToHash(settings.LocomotionBlendParameter);
    }

    private void Start()
    {
        _inputSource.MoveInput
            .Select(moveInput => Mathf.Clamp01(moveInput.magnitude))
            .Subscribe(speed => LocomotionSpeed.Value = speed)
            .AddTo(_disposables);

        LocomotionSpeed
            .Subscribe(speed => _animator.SetFloat(
                _locomotionBlendHash,
                speed,
                _settings.DampTime,
                Time.deltaTime))
            .AddTo(_disposables);
    }
}
```

---

## 애니메이션 레이어 활용

```csharp
public sealed class LayeredAnimation : MonoBehaviour
{
    private Animator _animator;
    private LayeredAnimationSettings _settings;
    private MotionHandle _layerWeightHandle;

    [Inject]
    public void Construct(Animator animator, LayeredAnimationSettings settings)
    {
        _animator = animator;
        _settings = settings;
    }

    private void Start()
    {
        _animator.SetLayerWeight(_settings.UpperBodyLayerIndex, 1f);
    }

    public void PlayUpperBodyAction(string actionName)
    {
        _animator.Play(actionName, _settings.UpperBodyLayerIndex, 0f);
    }

    public async UniTask SetAimingAsync(bool isAiming)
    {
        float targetWeight = isAiming ? 1f : 0f;
        int layerIndex = _settings.AimingLayerIndex;
        float currentWeight = _animator.GetLayerWeight(layerIndex);

        if (_layerWeightHandle.IsActive())
        {
            _layerWeightHandle.Cancel();
        }

        _layerWeightHandle = LMotion.Create(currentWeight, targetWeight, _settings.BlendDuration)
            .WithEase(Ease.OutQuad)
            .WithOnUpdate(value => _animator.SetLayerWeight(layerIndex, value))
            .RunWithoutBinding();

        await _layerWeightHandle.ToUniTask();
    }
}
```

---

## AnimationLifetimeScope 구성

```csharp
public sealed class AnimationLifetimeScope : LifetimeScope
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _headTrackingTarget;
    [SerializeField] private CharacterSkinChanger _characterSkinChanger;
    [SerializeField] private WeaponAnimationSystem _weaponAnimationSystem;
    [SerializeField] private BoneController _boneController;
    [SerializeField] private HeadTracking _headTracking;
    [SerializeField] private FootIK _footIK;
    [SerializeField] private AnimationBlending _animationBlending;
    [SerializeField] private LayeredAnimation _layeredAnimation;
    // ... settings fields

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_animator);
        builder.RegisterInstance(_headTrackingTarget);
        builder.Register<AnimationInputSource>(Lifetime.Singleton);

        builder.RegisterComponent(_characterSkinChanger);
        builder.RegisterComponent(_weaponAnimationSystem);
        builder.RegisterComponent(_boneController);
        builder.RegisterComponent(_headTracking);
        builder.RegisterComponent(_footIK);
        builder.RegisterComponent(_animationBlending);
        builder.RegisterComponent(_layeredAnimation);
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
□ MenuLifetimeScope (VContainer) 구현
□ MenuViewModel로 R3 반응형 상태 관리
□ 최소 3개의 화면 (Main, Settings, Game)
□ 히스토리 관리 (뒤로 가기 기능, CanGoBack 파생 속성)
□ Fade + Slide 전환 (LitMotion 트윈)

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
□ ReactiveProperty<WeaponType>으로 무기 상태 관리
□ Animator Override로 클립 교체
□ VContainer DI로 Animator 주입

채점 기준:
- 무기 교체가 자연스러운가?
- Override가 정상 적용되는가?
- 애니메이션 이벤트가 작동하는가?
```

---

## 과제 3: IK 머리 추적

```
요구사항:
□ VContainer [Inject]로 Animator + 타겟 주입
□ LitMotion으로 부드러운 회전 (Slerp 대신)
□ 각도 제한 (±60도)
□ 타겟 전환 시 MotionHandle 취소

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
