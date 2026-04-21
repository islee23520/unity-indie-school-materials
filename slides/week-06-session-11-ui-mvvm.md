---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
---

<style>
section {
  font-family: 'Segoe UI', 'Noto Sans KR', sans-serif;
}
h1 {
  color: #4ecdc4;
}
h2 {
  color: #ff6b6b;
}
code {
  background-color: #2d2d44;
  padding: 2px 6px;
  border-radius: 3px;
}
pre {
  background-color: #16213e;
  border: 1px solid #4ecdc4;
  border-radius: 8px;
  padding: 16px;
}
blockquote {
  border-left: 4px solid #4ecdc4;
  background-color: #2d2d44;
  padding: 12px 16px;
  margin: 16px 0;
}
</style>

<!-- _class: lead -->
# 🎨 Session 11: UI Toolkit & MVVM
## UXML/USS·R3 데이터 바인딩·LitMotion UI 트윈

**Unity UI, MVVM 패턴, 그리고 부드러운 애니메이션**

---

# 📋 목차

1. UI Toolkit 소개
2. UI Toolkit 기초
3. MVVM 패턴 적용
4. LitMotion UI 애니메이션
5. AI 프롬프트 활용
6. 실전 예제

---

<!-- _class: lead -->
# 1️⃣ UI Toolkit 소개

**Unity의 현대적 UI 시스템**

---

# UI Toolkit이란?

**Unity의 새로운 UI 프레임워크**

- **UXML**: HTML/XML 스타일의 마크업
- **USS**: CSS 스타일의 스타일 시트
- **C# API**: 코드로 UI 제어

```csharp
// 권장 스택: UI Toolkit + MVVM + R3
// UI Toolkit: UIDocument + VisualElement 기반
// Gameplay UI도 가능하면 같은 바인딩 패턴으로 통일
```

**장점**
- 성능 (GPU 기반 렌더링)
- 유연한 스타일링
- 데이터 주도 UI

**세션 기준 스택**
- MonoBehaviour/Clean Architecture
- VContainer DI
- R3 Reactive Stream

---

# 왜 UI Toolkit인가?

| 구분 | uGUI | UI Toolkit |
|------|------|------------|
| 렌더링 | Canvas 오브젝트 | GPU 기반 |
| 스타일 | 개별 설정 | CSS-like USS |
| 데이터 바인딩 | 수동 | 자동 (with MVVM) |
| 복잡한 UI | 어려움 | 쉬움 |
| 런타임 수정 | 가능 | 가능 |

---

<!-- _class: lead -->
# 2️⃣ UI Toolkit 기초

**UXML, USS, C#으로 UI 만들기**

---

# 기본 구조

**UI Document (UIDocument)**

```csharp
// UI Document 컴포넌트
public class MyUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        var label = root.Q<Label>("my-label");
        label.text = "Hello UI Toolkit!";
    }
}
```

```uxml
<!-- MyUI.uxml -->
<ui:UXML>
    <ui:Label name="my-label" text="Hello" />
    <ui:Button name="my-button" text="Click" />
</ui:UXML>
```

---

# USS 스타일링

**CSS와 유사한 문법**

```uss
/* MyStyle.uss */
.main-container {
    background-color: #2d2d44;
    padding: 20px;
    border-radius: 8px;
}

.my-button {
    background-color: #4ecdc4;
    color: white;
    font-size: 16px;
    transition: all 0.3s ease;
}

.my-button:hover {
    background-color: #ff6b6b;
    scale: 1.1;
}
```

---

# 이벤트 처리

**C#에서 이벤트 연결**

```csharp
public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    private Button _startButton;
    private Label _scoreLabel;
    
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        
        _startButton = root.Q<Button>("start-button");
        _scoreLabel = root.Q<Label>("score-label");
        
        // 이벤트 등록
        _startButton.clicked += OnStartClicked;
        _startButton.RegisterCallback<ClickEvent>(OnButtonHover);
    }
    
    void OnStartClicked()
    {
        Debug.Log("게임 시작!");
        _scoreLabel.text = "점수: 100";
    }
}
```

---

# 자주 사용하는 컨트롤

```csharp
// 레이블
var label = new Label("텍스트");
label.AddToClassList("my-label");

// 버튼
var button = new Button();
button.text = "클릭";
button.clicked += () => Debug.Log("Clicked!");

// 텍스트 필드
var textField = new TextField("이름:");
textField.value = "기본값";

// 슬라이더
var slider = new Slider(0, 100);
slider.value = 50;

// 목록
var listView = new ListView();
listView.itemsSource = myDataList;
```

---

<!-- _class: lead -->
# 3️⃣ MVVM 패턴

**Model-View-ViewModel**

---

# MVVM이란?

**UI와 비즈니스 로직 분리**

```
┌─────────────┐     ┌──────────────┐     ┌──────────┐
│   Model     │────▶│  ViewModel   │────▶│   View   │
│  (데이터)    │     │  (중재자)    │     │  (UI)    │
└─────────────┘     └──────────────┘     └──────────┘
       ▲                                      │
       └──────────────────────────────────────┘
                  (사용자 입력)
```

**구성 요소**
- **Model**: 순수 데이터
- **ViewModel**: UI 로직, 데이터 변환
- **View**: UI Toolkit (UXML)

---

# Model 예제

```csharp
// Model - 순수 데이터
[Serializable]
public class PlayerData
{
    public string Name;
    public int Health;
    public int MaxHealth;
    
    public float HealthPercent => (float)Health / MaxHealth;
}
```

---

# ViewModel 예제

```csharp
// ViewModel - UI 로직
public class PlayerViewModel : INotifyPropertyChanged
{
    private PlayerData _data;
    
    public string DisplayName => _data.Name;
    public string HealthText => $"{_data.Health} / {_data.MaxHealth}";
    public float HealthPercent => _data.HealthPercent;
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    public void TakeDamage(int amount)
    {
        _data.Health -= amount;
        PropertyChanged?.Invoke(this, 
            new PropertyChangedEventArgs(nameof(HealthText)));
        PropertyChanged?.Invoke(this, 
            new PropertyChangedEventArgs(nameof(HealthPercent)));
    }
}
```

---

# View (UI Toolkit)

```uxml
<!-- PlayerStatus.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement class="player-panel">
        <ui:Label name="player-name" class="name-label" />
        <ui:ProgressBar name="health-bar" low-value="0" high-value="1" />
        <ui:Label name="health-text" class="health-label" />
        <ui:Button name="damage-btn" text="데미지 테스트" />
    </ui:VisualElement>
</ui:UXML>
```

---

# 바인딩 연결

```csharp
public class PlayerUIView : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerViewModel viewModel;
    
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        
        // 데이터 바인딩
        root.Q<Label>("player-name").BindTo(viewModel, 
            nameof(viewModel.DisplayName));
        root.Q<ProgressBar>("health-bar").BindTo(viewModel, 
            nameof(viewModel.HealthPercent));
        root.Q<Label>("health-text").BindTo(viewModel, 
            nameof(viewModel.HealthText));
        
        // 버튼 이벤트
        root.Q<Button>("damage-btn").clicked += () => {
            viewModel.TakeDamage(10);
        };
    }
}
```

---

# R3와 함께 사용

**Reactive Extensions(R3)로 더 강력하게**

```csharp
using R3;

public class ReactiveViewModel
{
    public ReactiveProperty<string> PlayerName { get; } = new();
    public ReactiveProperty<int> Score { get; } = new(0);
    public ReactiveProperty<bool> IsGameOver { get; } = new(false);
    
    public IObservable<string> ScoreText => 
        Score.Select(s => $"점수: {s}");
        
    public void AddScore(int points)
    {
        Score.Value += points;
    }
}

// View에서 구독
viewModel.ScoreText.Subscribe(text => {
    scoreLabel.text = text;
}).AddTo(this);
```

---

<!-- _class: lead -->
# 4️⃣ LitMotion UI 애니메이션

**고성능 UI 애니메이션**

---

# LitMotion 소개

**Unity용 고성능 트윈 라이브러리**

```csharp
// 설치 (Package Manager)
// https://github.com/AnnulusGames/LitMotion

// 기본 사용법
using LitMotion;
using LitMotion.Extensions;

// UI 요소 이동
LMotion.Create(Vector2.zero, Vector2.one * 100f, 0.5f)
    .BindToAnchoredPosition(rectTransform);

// 알파값 변경
LMotion.Create(0f, 1f, 0.3f)
    .BindToGraphicAlpha(image);
```

---

# UI 애니메이션 기초

```csharp
using LitMotion;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    void AnimatePanel()
    {
        VisualElement panel = uiDocument.rootVisualElement.Q<VisualElement>("main-panel");

        LMotion.Create(0f, 1f, 0.5f)
            .WithEase(Ease.OutQuad)
            .BindWithState(panel, (value, element) =>
            {
                element.style.opacity = value;
            });

        LMotion.Create(-500f, 0f, 0.5f)
            .WithEase(Ease.OutBack)
            .BindWithState(panel, (value, element) =>
            {
                element.style.translate = new Translate(value, 0);
            });
    }
}
```

---

# Easing 함수

```csharp
// 다양한 easing 옵션
LMotion.Create(start, end, duration)
    .WithEase(Ease.Linear)      // 일정한 속도
    .WithEase(Ease.InQuad)      // 느리게 시작
    .WithEase(Ease.OutQuad)     // 느리게 끝
    .WithEase(Ease.InOutQuad)   // 양쪽에서 느리게
    .WithEase(Ease.OutBack)     // 약간 튀는 효과
    .WithEase(Ease.OutElastic)  // 탄성 효과
    .WithEase(Ease.OutBounce);  // 튀는 효과
```

---

# 체이닝과 딜레이

```csharp
// 순차 애니메이션
var sequence = LSequence.Create()
    // 1. 페이드 인
    .Append(LMotion.Create(0f, 1f, 0.3f)
        .BindToGraphicAlpha(image))
    
    // 2. 0.2초 대기
    .AppendInterval(0.2f)
    
    // 3. 위치 이동
    .Append(LMotion.Create(Vector2.zero, targetPos, 0.5f)
        .WithEase(Ease.OutBack)
        .BindToAnchoredPosition(transform))
    
    // 4. 동시에 회전
    .Join(LMotion.Create(0f, 360f, 0.5f)
        .BindToEulerAnglesZ(transform))
    
    .Run();
```

---

# UI Toolkit + LitMotion

```csharp
// UI Toolkit 요소 애니메이션
public class UIToolkitAnimator : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    void AnimatePanel()
    {
        var panel = uiDocument.rootVisualElement
            .Q<VisualElement>("game-panel");
        
        // 시작 상태
        panel.style.opacity = 0;
        panel.style.translate = new Translate(-200, 0);
        
        // 페이드 인
        LMotion.Create(0f, 1f, 0.5f)
            .WithEase(Ease.OutQuad)
            .BindWithState(panel, (value, element) => {
                element.style.opacity = value;
            });
        
        // 슬라이드 인
        LMotion.Create(-200f, 0f, 0.5f)
            .WithEase(Ease.OutBack)
            .BindWithState(panel, (value, element) => {
                element.style.translate = new Translate(value, 0);
            });
    }
}
```

---

# 버튼 클릭 효과

```csharp
public static class ButtonExtensions
{
    public static void AddClickAnimation(this Button button)
    {
        button.RegisterCallback<ClickEvent>(evt => {
            // 클릭 시 스케일 애니메이션
            LMotion.Create(1f, 0.9f, 0.05f)
                .WithEase(Ease.OutQuad)
                .BindWithState(button, (v, b) => {
                    b.style.scale = new Scale(new Vector2(v, v));
                })
                .AddTo(button);
            
            // 원래 크기로 복귀
            LMotion.Create(0.9f, 1f, 0.1f)
                .WithDelay(0.05f)
                .WithEase(Ease.OutBack)
                .BindWithState(button, (v, b) => {
                    b.style.scale = new Scale(new Vector2(v, v));
                })
                .AddTo(button);
        });
    }
}
```

---

# 팝업 애니메이션

```csharp
public class PopupController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement _popupPanel;
    
    void ShowPopup()
    {
        _popupPanel = uiDocument.rootVisualElement.Q<VisualElement>("popup");
        _popupPanel.style.display = DisplayStyle.Flex;
        
        // 배경 어둡게
        var overlay = uiDocument.rootVisualElement.Q<VisualElement>("overlay");
        LMotion.Create(0f, 0.7f, 0.3f)
            .BindWithState(overlay, (v, e) => {
                e.style.backgroundColor = new Color(0, 0, 0, v);
            });
        
        // 팝업 등장 (스케일 + 페이드)
        _popupPanel.style.scale = new Scale(Vector2.zero);
        _popupPanel.style.opacity = 0;
        
        LMotion.Create(0f, 1f, 0.4f)
            .WithEase(Ease.OutBack)
            .BindWithState(_popupPanel, (v, e) => {
                e.style.scale = new Scale(new Vector2(v, v));
                e.style.opacity = v;
            });
    }
}
```

---

<!-- _class: lead -->
# 5️⃣ AI 프롬프트 활용

**AI로 UI 구현 가속화**

---

# 효과적인 AI 프롬프트

## 기본 구조

```
역할 + 맥락 + 요구사항 + 출력 형식
```

**예시 프롬프트:**

```
Unity UI Toolkit 전문가로서,
RPG 게임의 플레이어 상태창 UI를 만들어줘.

요구사항:
- UXML + USS + C# 코드
- MVVM 패턴 적용
- 반응형 레이아웃
- 다크 테마

포함할 요소:
- 캐릭터 이름과 레벨
- HP/MP 바
- 경험치 바
- 장비 슬롯 6개

출력: 각 파일별 코드
```

---

# 프롬프트 예제 1: 전체 UI 시스템

```
Unity UI Toolkit으로 인벤토리 시스템을 만들어줘.

[요구사항]
1. 그리드 기반 아이템 슬롯 (8x5)
2. 아이템 툴팁 (호버 시 표시)
3. 아이템 드래그 앤 드롭
4. 아이템 정렬 버튼
5. MVVM 패턴으로 데이터 바인딩

[구조]
- Model: ItemData (id, name, icon, count, rarity)
- ViewModel: InventoryViewModel
- View: Inventory.uxml

[스타일]
- 다크 판타지 테마
- 희귀도별 테두리 색상 (일반-흰, 희귀-파랑, 전설-주황)
- 선택된 슬롯 강조 효과

각 파일의 완전한 코드를 제공해줘.
```

---

# 프롬프트 예제 2: 애니메이션

```
LitMotion을 사용한 Unity UI 애니메이션 코드를 작성해줘.

[요구사항]
1. 메인 메뉴 버튼들이 순차적으로 등장
2. 각 버튼마다 0.1초 간격
3. 등장 애니메이션: 아래에서 위로 + 페이드 인
4. 호버 시: 스케일 1.1배 + 색상 변경
5. 클릭 시: 스케일 0.9배로 눌렸다가 복귀

[코드 구조]
- MenuAnimationController 클래스
- Button hover/click 이벤트 연결
- LSequence 사용한 순차 애니메이션
- LitMotion 체이닝 메서드 활용

[추가]
- 애니메이션 재생 중 버튼 비활성화
- ESC 키로 역순 퇴장 애니메이션
```

---

# 프롬프트 예제 3: 스타일 시트

```
Unity UI Toolkit용 USS 파일을 만들어줘.

[디자인 시스템]
- 게임 장르: 사이버펑크 RPG
- 주요 색상: 네옴 블루 (#00d4ff), 핑크 (#ff00ff)
- 폰트: 퓨처리스틱한 무결체

[컴포넌트 스타일]
1. Primary Button: 그라데이션 배경, 네옴 테두리, 호버 시 글로우
2. Panel: 반투명 다크 배경, 블러 효과
3. Health Bar: HP 감소 시 붉은색 깜빡임
4. Text: 계층별 크기 (Title, Body, Caption)
5. ScrollView: 커스텀 스크롤바, 투명 트랙

[추가 요구사항]
- 변수 사용 (--primary-color 등)
- 반응형 breakpoint
- 애니메이션 keyframes

완성된 USS 코드를 보여줘.
```

---

# 프롬프트 팁

**좋은 결과를 위한 팁**

1. **구체적 제시**: "예쁘게" 대신 "네옴 글로우 효과"
2. **맥락 제공**: 게임 장르, 타겟 플랫폼
3. **구조 명시**: 원하는 클래스/메서드명
4. **단계별 요청**: 복잡한 UI는 나눠서
5. **레퍼런스 제공**: 비슷한 게임 UI 예시

```
❌ "RPG UI 만들어줘"

✅ "다크 소울스 스타일의 아이템 툴팁 UI를
    UI Toolkit으로 만들어줘.
    UXML 구조와 USS 스타일링 코드를
    분리해서 보여줘."
```

---

<!-- _class: lead -->
# 6️⃣ 실전 예제

**완성된 코드로 실습**

---

# 예제: 게임 HUD

**PlayerHUD.uxml**

```uxml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="hud-root" class="hud-container">
        <!-- HP Bar -->
        <ui:VisualElement name="hp-container" class="bar-container">
            <ui:Label text="HP" class="bar-label" />
            <ui:VisualElement name="hp-bg" class="bar-bg">
                <ui:VisualElement name="hp-fill" class="hp-fill" />
            </ui:VisualElement>
            <ui:Label name="hp-text" class="bar-text" />
        </ui:VisualElement>
        
        <!-- Score -->
        <ui:VisualElement name="score-container">
            <ui:Label name="score-label" class="score-text" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

---

# 예제: 스타일링

**HUDStyles.uss**

```uss
.hud-container {
    position: absolute;
    top: 20px;
    left: 20px;
    right: 20px;
    height: 100px;
}

.bar-container {
    width: 300px;
    flex-direction: column;
}

.bar-bg {
    height: 24px;
    background-color: #2d2d44;
    border-radius: 12px;
    overflow: hidden;
}

.hp-fill {
    height: 100%;
    background-color: #ff6b6b;
    width: 100%;
    transition: width 0.3s ease;
}

.score-text {
    font-size: 32px;
    color: #4ecdc4;
    -unity-font-style: bold;
}
```

---

# 예제: ViewModel

```csharp
using R3;

public class HUDViewModel
{
    public ReactiveProperty<int> CurrentHP { get; } = new(100);
    public ReactiveProperty<int> MaxHP { get; } = new(100);
    public ReactiveProperty<int> Score { get; } = new(0);
    
    public IObservable<float> HPPercent => 
        CurrentHP.CombineLatest(MaxHP, (cur, max) => (float)cur / max);
    
    public void TakeDamage(int damage)
    {
        CurrentHP.Value = Mathf.Max(0, CurrentHP.Value - damage);
    }
    
    public void AddScore(int points)
    {
        Score.Value += points;
    }
}
```

---

# 예제: View + Animation

```csharp
public class HUDView : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private HUDViewModel _viewModel;
    
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        _viewModel = new HUDViewModel();
        
        // 요소 참조
        var hpFill = root.Q<VisualElement>("hp-fill");
        var hpText = root.Q<Label>("hp-text");
        var scoreLabel = root.Q<Label>("score-label");
        
        // HP 바 바인딩 + 애니메이션
        _viewModel.HPPercent.Subscribe(percent => {
            // LitMotion으로 부드럽게
            var targetWidth = percent * 100;
            LMotion.Create(hpFill.style.width.value.value, targetWidth, 0.3f)
                .WithEase(Ease.OutQuad)
                .BindWithState(hpFill, (v, e) => {
                    e.style.width = Length.Percent(v);
                });
            
            // 색상 변경 (위험 시)
            if (percent < 0.3f)
            {
                hpFill.style.backgroundColor = new Color(1f, 0.2f, 0.2f);
            }
        }).AddTo(this);
        
        // 점수 애니메이션
        _viewModel.Score.Subscribe(score => {
            scoreLabel.text = score.ToString("N0");
            
            // 점수 획득 효과
            LMotion.Create(1f, 1.3f, 0.1f)
                .WithEase(Ease.OutBack)
                .BindWithState(scoreLabel, (v, e) => {
                    e.style.scale = new Scale(new Vector2(v, v));
                });
        }).AddTo(this);
    }
}
```

---

<!-- _class: lead -->
# 🎯 과제

**실습 미션**

---

# 과제 안내

## 미션 1: 기본 HUD 만들기
- HP/MP 바가 있는 기본 HUD
- MVVM 패턴 적용
- LitMotion으로 데미지 표시 애니메이션

## 미션 2: 팝업 시스템
- 설정 창, 퍼즈 메뉴
- 열기/닫기 애니메이션
- ESC 키로 닫기

## 미션 3: AI 활용
- 위 프롬프트 예제를 AI에 입력
- 생성된 코드 분석 및 개선
- 자신만의 UI 제작

---

# Q&A

**질문 있으신가요?**

---

<!-- _class: lead -->
# 감사합니다

## 다음 세션: Session 12 - 메뉴 시스템 & Animator — 화면 전환·Animator Override Controller·스킨 변경
**Session 12: UI 고급 테크닉 + 최적화**

**참고 자료:**
- [UI Toolkit 공식 문서](https://docs.unity3d.com/Manual/UIElements.html)
- [LitMotion GitHub](https://github.com/AnnulusGames/LitMotion)
- [MVVM Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
