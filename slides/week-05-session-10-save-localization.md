---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eaeaea
style: |
  section {
    font-family: 'Segoe UI', 'Noto Sans KR', sans-serif;
  }
  h1 {
    color: #00d4aa;
    border-bottom: 3px solid #00d4aa;
    padding-bottom: 10px;
  }
  h2 {
    color: #00d4aa;
  }
  code {
    background-color: #16213e;
    padding: 2px 6px;
    border-radius: 4px;
    font-family: 'Consolas', monospace;
  }
  pre {
    background-color: #16213e;
    padding: 15px;
    border-radius: 8px;
    border-left: 4px solid #00d4aa;
  }
  .small {
    font-size: 0.8em;
  }
---

<!-- _class: lead -->
# Session 10: 세이브/로드 & 현지화
## JSON 직렬화·체크포인트·Unity Localization

**게임 데이터 저장과 다국어 지원**

---

## 목차

1. JSON 직렬화 (Serialization)
2. 세이브/로드 시스템 구현
3. Unity Localization 패키지
4. AI를 활용한 현지화 워크플로우
5. 실습: 다국어 지원 메트로배니아

---

## 1. JSON 직렬화

### 왜 JSON인가?

- **가독성**: 사람이 읽고 수정할 수 있음
- **플랫폼 독립적**: Windows, Mac, Mobile 모두 호환
- **디버깅 용이**: 파일 내용 직접 확인 가능
- **Unity 지원**: `JsonUtility` 기본 제공

---

## 직렬화 기본 개념

```csharp
// 메모리의 객체를 문자열로 변환
PlayerData data = new PlayerData();
data.name = "Player1";
data.score = 1000;

// 객체 → JSON 문자열
string json = JsonUtility.ToJson(data);
// 결과: {"name":"Player1","score":1000}
```

**직렬화(Serialize)**: 객체 → 문자열  
**역직렬화(Deserialize)**: 문자열 → 객체

---

## [Serializable] 속성

```csharp
using System;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int highScore;
    public int currentLevel;
    public float playTime;
    
    // 복잡한 데이터도 저장 가능
    public List<LevelProgress> levelProgress;
    public Dictionary<string, bool> unlockedItems;
}

[Serializable]
public class LevelProgress
{
    public int levelId;
    public int stars;
    public bool isCompleted;
}
```

---

## JsonUtility 사용법

```csharp
using UnityEngine;

public class JsonExample : MonoBehaviour
{
    void SaveExample()
    {
        PlayerData data = new PlayerData();
        data.playerName = "김개발";
        data.highScore = 99999;
        
        // 객체를 JSON으로
        string json = JsonUtility.ToJson(data, true);
        // 두 번째 매개변수: prettyPrint (들여쓰기)
        
        Debug.Log(json);
    }
    
    void LoadExample(string json)
    {
        // JSON을 객체로
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        
        Debug.Log($"플레이어: {data.playerName}");
        Debug.Log($"최고 점수: {data.highScore}");
    }
}
```

---

## 리스트/배열 직렬화

```csharp
[Serializable]
public class SaveData
{
    // 배열은 직렬화 가능
    public int[] scores;
    public string[] playerNames;
    
    // List도 가능
    public List<InventoryItem> inventory;
}

// 저장 시
SaveData save = new SaveData();
save.scores = new int[] { 100, 200, 300 };
save.inventory = new List<InventoryItem>();

string json = JsonUtility.ToJson(save);
```

---

## Dictionary 직렬화 (주의)

```csharp
// JsonUtility는 Dictionary를 기본 지원하지 않음
// 해결책: 래퍼 클래스 사용

[Serializable]
public class StringBoolPair
{
    public string key;
    public bool value;
}

[Serializable]
public class SaveData
{
    // Dictionary 대신 List 사용
    public List<StringBoolPair> achievements;
}

// 변환 헬퍼 메서드
public Dictionary<string, bool> ToDictionary(List<StringBoolPair> list)
{
    return list.ToDictionary(p => p.key, p => p.value);
}
```

---

## 2. 세이브/로드 시스템 구현

### 파일 저장 경로

```csharp
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // 플랫폼 독립적인 저장 경로
    private string SavePath => Path.Combine(
        Application.persistentDataPath, 
        "savegame.json"
    );
    
    void Start()
    {
        Debug.Log($"저장 경로: {SavePath}");
        // Windows: C:\Users\[User]\AppData\LocalLow\[Company]\[Game]
        // Mac: ~/Library/Application Support/[Company]/[Game]
        // Android: /storage/emulated/0/Android/data/[package]/files
    }
}
```

---

## 완전한 세이브/로드 시스템

```csharp
using System.IO;
using VContainer;
using VContainer.Unity;

public class SaveManager : IStartable
{
    private readonly string _saveFileName = "metroidvania_save.json";
    private string _savePath;

    public void Start()
    {
        _savePath = Path.Combine(
            UnityEngine.Application.persistentDataPath,
            _saveFileName);
    }

    public string GetSavePath() => _savePath;
}

public class SaveLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SaveManager>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();
    }
}
```

---

## Save 메서드

```csharp
public void SaveGame(GameData data)
{
    try
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetSavePath();
        
        File.WriteAllText(path, json);
        
        Debug.Log($"게임 저장 완료: {path}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"저장 실패: {e.Message}");
    }
}
```

---

## Load 메서드

```csharp
public GameData LoadGame()
{
    string path = GetSavePath();
    
    if (!File.Exists(path))
    {
        Debug.Log("저장 파일 없음. 새 게임 시작");
        return new GameData(); // 기본 데이터 반환
    }
    
    try
    {
        string json = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(json);
        
        Debug.Log("게임 로드 완료");
        return data;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"로드 실패: {e.Message}");
        return new GameData();
    }
}
```

---

## 데이터 암호화 (기본)

```csharp
using System;
using System.Text;
using System.Security.Cryptography;

public class SaveEncryption
{
    private static string key = "YourSecretKey123"; // 16자리
    
    public static string Encrypt(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];
            
            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            
            return Convert.ToBase64String(encrypted);
        }
    }
    
    public static string Decrypt(string encryptedText)
    {
        byte[] bytes = Convert.FromBase64String(encryptedText);
        
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];
            
            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
```

---

## 메트로배니아 게임 데이터 구조

```csharp
[Serializable]
public class MetroidvaniaSaveData
{
    // 플레이어 정보
    public string playerName;
    public int playerLevel;
    public int currentHp;
    public int maxHp;
    
    // 진행 상황
    public string currentScene;
    public List<string> unlockedAbilities;
    public List<string> activatedCheckpoints;
    
    // 통계
    public float totalPlayTime;
    public int enemiesDefeated;
    public int deaths;
    
    // 설정
    public float musicVolume;
    public float sfxVolume;
    public string language;
    
    // 도전 과제
    public List<AchievementData> achievements;
}

[Serializable]
public class AchievementData
{
    public string id;
    public bool unlocked;
    public string unlockDate;
}
```

---

## 사용 예시

```csharp
using VContainer;

public class GameManager : MonoBehaviour
{
    [Inject] private SaveManager _saveManager;

    public MetroidvaniaSaveData CurrentData { get; private set; }

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        CurrentData.currentScene = SceneLoader.Instance.CurrentSceneName;
        CurrentData.totalPlayTime += Time.time;

        _saveManager.SaveGame(CurrentData);
    }

    public void LoadGame()
    {
        CurrentData = _saveManager.LoadGame();

        // UI 업데이트
        UIManager.Instance.UpdateSaveSlot(CurrentData.currentScene, CurrentData.totalPlayTime);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame(); // 백그라운드 진입 시 자동 저장
    }
}
```

---

## 3. Unity Localization 패키지

### 설치 방법

```
Window → Package Manager → Unity Registry
→ "Localization" 검색 → Install
```

**주요 기능**:
- 다국어 텍스트 관리
- 에셋 현지화 (스프라이트, 오디오 등)
- 런타임 언어 전환
- Google Sheets 연동

---

## Localization 설정

```
Edit → Project Settings → Localization
```

**1. Locales 설정**:
- Korean (ko)
- English (en)
- Japanese (ja)
- Chinese (zh)

**2. 기본 Locale**: Korean

**3. Asset Tables 생성**:
- UI_Text (텍스트)
- UI_Sprites (이미지)
- Audio_Clips (음성)

---

## String Table 생성

```
Window → Asset Management → Localization Tables
→ New Table Collection → "UI_Text"
```

| Key | Korean | English | Japanese |
|-----|--------|---------|----------|
| game_title | 네온 글리치 | Neon Glitch | ネオン・グリッチ |
| start_game | 게임 시작 | Start Game | ゲーム開始 |
| high_score | 최고 점수 | High Score | ハイスコア |
| pause | 일시정지 | Pause | 一時停止 |
| game_over | 게임 오버 | Game Over | ゲームオーバー |

---

## 코드에서 Localization 사용

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationExample : MonoBehaviour
{
    // String Table 참조
    public LocalizedString gameTitle;
    
    void Start()
    {
        // 동적 문자열 가져오기
        GetLocalizedStringAsync("UI_Text", "game_title").Forget();
        
        // 언어 변경
        ChangeLanguage("en");
    }
    
    public async UniTask GetLocalizedStringAsync(string table, string key)
    {
        var stringOperation = LocalizationSettings
            .StringDatabase
            .GetLocalizedStringAsync(table, key);
        
        string result = await stringOperation.Task;
        Debug.Log(result);
    }
}
```

---

## 런타임 언어 변경

```csharp
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    // 언어 변경
    public void SetLanguage(string localeCode)
    {
        // ko, en, ja, zh 등
        LocalizationSettings.SelectedLocale = 
            LocalizationSettings.AvailableLocales.GetLocale(localeCode);
    }
    
    // 현재 언어 확인
    public string GetCurrentLanguage()
    {
        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }
    
    // 저장된 언어 설정 로드
    void Start()
    {
        string savedLang = PlayerPrefs.GetString("Language", "ko");
        SetLanguage(savedLang);
    }
}
```

---

## LocalizedString 컴포넌트

```csharp
using UnityEngine;
using UnityEngine.Localization.Components;

public class UILocalization : MonoBehaviour
{
    // Inspector에서 설정
    public LocalizeStringEvent titleText;
    public LocalizeStringEvent scoreText;
    
    void SetLocalizedStrings()
    {
        // 코드에서 String Reference 설정
        titleText.StringReference.TableReference = "UI_Text";
        titleText.StringReference.TableEntryReference = "game_title";
        
        // 변수가 있는 문자열
        scoreText.StringReference.SetReference("UI_Text", "score_display");
        // score_display: "점수: {0}"
    }
    
    // 변수 전달
    public void UpdateScore(int score)
    {
        scoreText.StringReference.Arguments = new object[] { score };
    }
}
```

---

## UI Toolkit Label 자동 현지화

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class AutoLocalizedLabel : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string localizationKey;

    private async UniTaskVoid Start()
    {
        await LocalizationSettings.InitializationOperation.ToUniTask();

        var titleLabel = uiDocument.rootVisualElement.Q<Label>("title-label");
        string localizedText = await LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync("UI_Text", localizationKey)
            .Task;

        titleLabel.text = localizedText;
    }
}
```

---

## Asset Table (이미지/오디오)

```csharp
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizedAssetExample : MonoBehaviour
{
    public SpriteRenderer flagRenderer;
    public AudioSource voiceSource;
    
    void LoadLocalizedAssets()
    {
        // 현지화된 스프라이트 로드
        var spriteOp = LocalizationSettings
            .AssetDatabase
            .GetLocalizedAssetAsync<Sprite>("UI_Sprites", "flag_icon");
        
        flagRenderer.sprite = spriteOp.Result;
        
        // 현지화된 오디오 로드
        var audioOp = LocalizationSettings
            .AssetDatabase
            .GetLocalizedAssetAsync<AudioClip>("Audio_Clips", "welcome_voice");
        
        voiceSource.clip = audioOp.Result;
        voiceSource.Play();
    }
}
```

---

## 4. AI를 활용한 현지화 워크플로우

### 기본 번역 프롬프트

```
역할: 전문 게임 현지화 번역가

원본 텍스트 (한국어):
- 게임 시작
- 최고 점수를 달성했습니다!
- 대시하려면 Shift 키를 누르세요

번역 대상 언어: 영어, 일본어, 중국어(간체)

요구사항:
1. 게임 컨텍스트에 맞는 자연스러운 표현 사용
2. 각 언어별 문자 수 제한 고려 (UI 공간 제한)
3. 게임 용어의 일관성 유지

출력 형식:
한국어 | 영어 | 일본어 | 중국어
---|---|---|---
[텍스트] | [텍스트] | [텍스트] | [텍스트]
```

---

## 톤 앤 매너 유지 프롬프트

```
게임 정보:
- 장르: 횡스크롤 메트로배니아 액션
- 타겟 연령: 전연령
- 톤: 밝고, 에너지 넘치는, 격려하는

원본 문구:
"와우! 대단해요! 숨겨진 구역을 발견했어요!"

번역 지시:
1. 원문의 흥분과 격려하는 톤 유지
2. 각 언어의 감탄사와 강조 표현 활용
3. '~요' 체의 정중함과 친근함의 균형 유지

영어: "Wow! Amazing! You discovered a hidden area!"
일본어: 「すごい！隠しエリアを見つけました！」
중국어: "太棒了！你发现了隐藏区域！"
```

---

## UI 공간 제한 고려 프롬프트

```
UI 텍스트 현지화 요청

원본 (한국어): "최고 점수"
최대 글자 수: 
- 영어: 10자
- 일본어: 6자  
- 중국어: 4자

번역 및 대안 제시:

영어:
- "High Score" (권장, 10자)
- "Hi-Score" (대안, 8자)
- "Best" (축약, 4자)

일본어:
- 「ハイスコア」(권장, 5자)
- 「最高得点」(대안, 4자)

중국어:
- "最高分" (권장, 3자)
- "最佳分数" (대안, 4자)
```

---

## 대량 번역 자동화 프롬프트

```
CSV 형식의 게임 텍스트 번역

입력 데이터:
Key,Korean
GAME_START,게임 시작
PAUSE_MENU,일시정지 메뉴
SCORE_DISPLAY,점수: {0}
LEVEL_COMPLETE,레벨 {0} 완료!
ABILITY_UNLOCK,능력 해금!

번역 요구사항:
1. {0} 같은 플레이스홀더 유지
2. 게임 컨텍스트에 맞는 번역
3. 각 언어별 자연스러운 표현

출력 형식 (CSV):
Key,Korean,English,Japanese,Chinese
GAME_START,게임 시작,Start Game,ゲーム開始,开始游戏
...
```

---

## 문화권별 적응 프롬프트

```
문화 현지화 (Culturalization) 검토

원본 콘텐츠:
- 지역 테마: "버려진 연구소"
- 특수 아이템: "십자가", "묘비"
- 배경 음악: 교회 오르간

대상 시장별 검토 요청:

일본 시장:
- 할로윈 컨셉의 수용도
- 종교적 이미지(십자가) 수정 필요성
- 대안: "유령", "호박" 요소로 대체

중동 시장:
- 종교적 상징물 완전 제거 권장
- 묘비 → 복숭아 나무로 변경
- 십자가 → 별 모양으로 변경

중국 시장:
- 귀신/유령 컨셉 검열 고려
- 밝은 색상 팔레트 권장
- 축제 분위기 유지
```

---

## 품질 검수 프롬프트

```
번역 품질 검수 (QA)

검수할 번역:
한국어: "대시로 좁은 통로를 통과하세요!"
영어: "Use dash to pass through the narrow corridor!"
일본어: 「ダッシュで狭い通路を通り抜けましょう！」

검수 항목:
1. 의미 정확성: 원문과 의미 일치 여부
2. 자연스러움: 원어민이 사용할 표현인지
3. 게임 컨텍스트: 게임 상황에 적합한지
4. 길이 적절성: UI에 표시 가능한 길이인지
5. 일관성: 다른 문구와의 스타일 일치 여부

검수 결과:
영어: ✅ 적절함
일본어: ✅ 적절함
```

---

## 5. 실습: 다국어 지원 메트로배니아

### 실습 목표

1. 세이브/로드 시스템 구현
2. Unity Localization 설정
3. 4개 언어 지원 (한/영/일/중)
4. AI로 번역 데이터 생성

---

## 실습 단계

**단계 1: 세이브 시스템**
```csharp
// SaveManager.cs 생성
// - MetroidvaniaSaveData 클래스 정의
// - Save/Load 메서드 구현
// - 점수, 설정, 진행상황 저장
```

**단계 2: Localization 설정**
```
// Package Manager에서 Localization 설치
// 4개 Locale 추가 (ko, en, ja, zh)
// String Table "UI_Text" 생성
```

**단계 3: 번역 데이터**
```
// AI 프롬프트로 번역 CSV 생성
// String Table에 데이터 입력
// 키: game_title, start_button, score_label 등
```

---

## 실습 체크리스트

- [ ] SaveManager VContainer 등록
- [ ] 게임 데이터 JSON 저장 확인
- [ ] Localization 패키지 설치
- [ ] 4개 언어 Locale 설정
- [ ] String Table에 텍스트 입력
- [ ] LocalizedStringEvent 컴포넌트 적용
- [ ] 언어 전환 UI 구현
- [ ] 선택한 언어 저장 (PlayerPrefs)

---

## 과제

### 과제 1: 세이브 시스템 확장
- 여러 슬롯 저장 기능 (3개 슬롯)
- 저장 날짜/시간 표시
- 저장 파일 삭제 기능

### 과제 2: 완전한 다국어 지원
- AI로 20개 이상 문구 번역
- 스프라이트 현지화 (시작 버튼 등)
- 언어 변경 시 실시간 적용

### 과제 3: Google Sheets 연동
- 스프레드시트에서 번역 데이터 가져오기
- 자동 동기화 스크립트 작성

---

## 유용한 리소스

**Unity Documentation**:
- Unity Localization: https://docs.unity3d.com/Packages/com.unity.localization@latest

**JSON 유틸리티**:
- Newtonsoft.Json: 더 강력한 JSON 라이브러리

**번역 관리**:
- Google Sheets + Unity 연동 튜토리얼
- CSV import/export 스크립트

**Q&A 시간**

---

<!-- _class: lead -->
# 감사합니다!

**Session 10 완료**

질문 있으신가요?
