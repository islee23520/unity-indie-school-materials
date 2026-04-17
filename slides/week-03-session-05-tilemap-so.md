---
marp: true
theme: default
paginate: true
backgroundColor: #1a1a2e
color: #eee
style: |
  section {
    font-family: 'Noto Sans KR', sans-serif;
  }
  h1 {
    color: #e94560;
    font-size: 2.5em;
  }
  h2 {
    color: #0f3460;
    font-size: 1.8em;
  }
  code {
    background-color: #16213e;
    padding: 2px 6px;
    border-radius: 4px;
    font-family: 'Fira Code', monospace;
  }
  pre {
    background-color: #16213e;
    padding: 16px;
    border-radius: 8px;
  }
  .keyword {
    color: #e94560;
    font-weight: bold;
  }
  .checklist {
    color: #16c79a;
  }
---

<!-- _class: lead -->

# 타일맵 & 레벨 디자인

## Session 5: 타일맵 & 레벨 디자인

Unity 실무 중심 메트로배니아 게임 개발

---

## 오늘의 학습 목표

**3시간 동안 배울 내용**

| 시간 | 주제 | 활동 |
|------|------|------|
| 0-40분 | Unity Tilemap | 타일맵 생성, Rule Tile, 레이어 관리 |
| 40-80분 | ScriptableObject | 데이터 분리, 레벨 설정 관리 |
| 80-120분 | AI 실습 | Claude로 레벨 데이터 생성 |
| 120-150분 | LitMotion | 패럴랙스 배경, 침 침 효과 |
| 150-180분 | 통합 실습 | 타일맵 + 데이터 + 모션 조합 |

---

## Unity Tilemap이란?

**2D 레벨 디자인의 핵심 도구**

- 격자 기반(grid) 타일 배치 시스템
- 스프라이트를 타일 형태로 배치
- 충돌 처리, 레이어링 자동 지원
- Rule Tile로 자동 타일링 가능

**실무 활용**
- 지형(ground), 벽(wall), 장애물 배치
- 메트로배니아 맵 구성
- 프로시저럴 생성의 기초

---

## Tilemap 생성하기

**1. 타일맵 오브젝트 생성**

```
Hierarchy > 2D Object > Tilemap > Rectangular
```

**2. 자동 생성 구조**

```
Grid (부모)
  └─ Tilemap (자식)
       ├─ Tilemap Renderer
       └─ Tilemap Collider 2D (추가 가능)
```

**3. Tile Palette 열기**

```
Window > 2D > Tile Palette
```

---

## Tile Palette 설정

**스프라이트를 타일로 변환**

```
1. Project 창에서 스프라이트 선택
2. Slice로 개별 타일 분리 (Sprite Editor)
3. Tile Palette에 드래그
4. Create New Palette로 저장
```

**주요 도구**

| 도구 | 단축키 | 기능 |
|------|--------|------|
| Paint Brush | B | 타일 그리기 |
| Eraser | E | 타일 지우기 |
| Fill Bucket | G | 영역 채우기 |
| Pick | I | 타일 색상 선택 |

---

## Rule Tile로 스마트 타일링

**자동 연결 타일 시스템**

```csharp
// 2D Extras 패키지 필요
// Window > Package Manager > 2D Tilemap Extras
```

**Rule Tile 설정**

```
1. Project > Create > 2D > Tiles > Rule Tile
2. Default Sprite 설정
3. Tiling Rules 추가
4. Neighbors 설정 (상하좌우 연결 조건)
```

**자동 벽 타일 예시**
- 위쪽만 연결: 위쪽 벽
- 양옆 연결: 수평 벽
- 모서리: 모서리 스프라이트

---

## 레이어 시스템 구성

**메트로배니아 레이어 구조**

```
Grid
  ├─ BackgroundLayer (배경 장식, Z: 10)
  ├─ GroundLayer (지형, Z: 0)  
  ├─ WallLayer (벽, 충돌, Z: 0)
  ├─ PlatformLayer (플랫폼, Z: 0)
  └─ ForegroundLayer (전경, Z: -10)
```

**Sorting Layer 설정**

```
Edit > Project Settings > Tags and Layers
  ├─ Sorting Layers: Background, Ground, Foreground
  └─ Layer: Ground, Wall, Platform, Enemy, Player
```

---

## 타일맵 충돌 설정

**Tilemap Collider 2D**

```csharp
// Tilemap 게임오브젝트에 추가
// Inspector > Add Component > Tilemap Collider 2D

// Used by Composite 체크 후
// Add Component > Composite Collider 2D
// Rigidbody2D 자동 추가 (Static 설정)
```

**충돌이 필요 없는 타일**

```
1. Tile Palette에서 타일 선택
2. Inspector > Collider Type: None
3. 장식용 타일에 적용
```

---

## AI 실습: Tilemap 설정

**Claude에게 요청할 프롬프트**

```
"Unity 2D 메트로배니아 게임의 타일맵 레이어 구조를 설계해줘.

요구사항:
1) Background, Ground, Wall, Platform, Foreground 5개 레이어
2) 각 레이어의 Sorting Order와 Z-position
3) Collision 설정 (어떤 레이어에 Collider 추가)
4) Rule Tile을 활용한 자동 벽 타일링 방법"
```

**생성된 구조를 프로젝트에 적용하세요.**

---

## ScriptableObject란?

**데이터와 로직 분리**

- 에디터에서 데이터를 에셋 형태로 저장
- 인스펙터에서 직관적으로 편집 가능
- 런타임에 메모리에 한 번만 로드
- JSON/ScriptableObject 복제 없이 참조 공유

**vs MonoBehaviour**

| 특성 | MonoBehaviour | ScriptableObject |
|------|---------------|------------------|
| 존재 | Scene에 부착 | Project 에셋 |
| 생명주기 | Scene 로드 | 앱 실행 동안 |
| 용도 | 컴포넌트 로직 | 순수 데이터 |

---

## ScriptableObject 기본 구조

**레벨 데이터 정의**

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", 
    menuName = "Game/Level Data")]
public class LevelData : ScriptableObject {
    [Header("기본 정보")]
    public string levelName;
    public int levelIndex;
    
    [Header("플레이어 설정")]
    public Vector2 playerStartPosition;
    public float timeLimit = 300f;
    
    [Header("적 설정")]
    public EnemySpawnData[] enemySpawns;
    
    [Header("보상")]
    public int coinCount;
    public int secretCount;
}
```

---

## ScriptableObject 에셋 생성

**1. 메뉴에서 생성**

```
Project 창 > 우클릭 > Create > Game > Level Data
```

**2. 인스펙터에서 데이터 입력**

```csharp
// LevelData 에셋 클릭
// Inspector에서 직접 값 입력
// 드래그 앤 드롭으로 참조 연결
```

**3. 코드에서 로드**

```csharp
public class LevelManager : MonoBehaviour {
    [SerializeField] private LevelData currentLevel;
    
    void Start() {
        Debug.Log($"레벨: {currentLevel.levelName}");
        SpawnPlayer(currentLevel.playerStartPosition);
    }
}
```

---

## 데이터 구조 설계

**적 스폰 데이터**

```csharp
[System.Serializable]
public class EnemySpawnData {
    public EnemyType enemyType;
    public Vector2 spawnPosition;
    public float spawnDelay;
    public bool isPatrol;
}

public enum EnemyType {
    Slime,
    Goblin,
    Bat,
    Boss
}
```

**ScriptableObject 활용**

```csharp
[CreateAssetMenu(fileName = "EnemyDatabase", 
    menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject {
    public EnemyData[] enemies;
    
    public EnemyData GetEnemy(EnemyType type) {
        return System.Array.Find(enemies, e => e.type == type);
    }
}
```

---

## AI 실습: 레벨 데이터 관리

**Claude에게 요청할 프롬프트**

```
"Unity ScriptableObject를 사용한 메트로배니아 
레벨 데이터 관리 시스템을 설계해줘.

요구사항:
1) LevelData: 레벨 이름, 시작 위치, 적 목록
2) EnemySpawnData: 적 타입, 위치, 스폰 시간
3) TilemapData: 사용할 타일맵 프리팹 참조
4) LevelManager에서 데이터 로드하고 적 스폰하는 코드"
```

**생성된 ScriptableObject 구조를 구현하세요.**

---

## LitMotion 소개

**고성능 트위닝 라이브러리**

- Unity 최신 DOTS/ECS 기반
- 제로 할당(zero allocation)
- UniTask와 완벽 호환
- 체인 가능한 Fluent API

**설치**

```
Window > Package Manager > + > Add package from git URL
git@github.com:annulusgames/LitMotion.git
```

**네임스페이스**

```csharp
using LitMotion;
using LitMotion.Extensions;
```

---

## LitMotion 기본 사용법

**기본 트윈**

```csharp
// 위치 이동
LMotion.Create(Vector3.zero, Vector3.right * 5f, 1f)
    .BindToPosition(transform);

// 스케일 변경
LMotion.Create(Vector3.one, Vector3.one * 2f, 0.5f)
    .BindToLocalScale(transform);

// 회전
LMotion.Create(0f, 360f, 2f)
    .BindToLocalEulerAnglesY(transform);
```

**이징 함수**

```csharp
LMotion.Create(start, end, duration)
    .WithEase(Ease.OutBounce)  // 바운스 효과
    .BindToPosition(transform);
```

---

## 패럴랙스 배경 구현

**다중 레이어 패럴랙스**

```csharp
public class ParallaxBackground : MonoBehaviour {
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxSpeed = 0.5f;
    
    private Vector3 previousCameraPosition;
    
    void Start() {
        previousCameraPosition = cameraTransform.position;
    }
    
    void LateUpdate() {
        Vector3 delta = cameraTransform.position - previousCameraPosition;
        delta.y = 0; // 수직 패럴랙스 제거
        
        transform.position += delta * parallaxSpeed;
        previousCameraPosition = cameraTransform.position;
    }
}
```

---

## LitMotion으로 침 침 효과

**침 침 효과 (Floating Effect)**

```csharp
public class FloatingObject : MonoBehaviour {
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatDuration = 1.5f;
    
    void Start() {
        // 위아래 반복 이동
        LMotion.Create(-floatHeight, floatHeight, floatDuration)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .BindToLocalPositionY(transform);
    }
}
```

**곡선 이동**

```csharp
// 베지어 곡선으로 이동
LMotion.Create(startPos, endPos, 2f)
    .WithEase(Ease.InOutQuad)
    .WithOnComplete(() => Debug.Log("도착!"))
    .BindToPosition(transform);
```

---

## AI 실습: 패럴랙스 + LitMotion

**Claude에게 요청할 프롬프트**

```
"Unity에서 LitMotion을 사용한 패럴랙스 배경 시스템을 구현해줘.

요구사항:
1) 3개의 배경 레이어 (Far, Mid, Near)
2) 침 침 떠다니는 구름 (LitMotion)
3) 침 침 반짝이는 별 (스케일 트윈)
4) 침사 침사하는 물결 효과 (위치 트윈)
5) 플레이어 따라가는 침 침 침 침"

참고: 침 침 = floating/oscillating motion
```

**생성된 코드를 배경 오브젝트에 적용하세요.**

---

## 통합 실습: 타일맵 + 데이터 + 모션

**레벨 전체 구조**

```csharp
public class GameLevel : MonoBehaviour {
    [SerializeField] private LevelData levelData;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private ParallaxBackground[] backgrounds;
    
    void Start() {
        // 1. 레벨 데이터 로드
        SetupLevel(levelData);
        
        // 2. 적 스폰
        SpawnEnemies(levelData.enemySpawns);
        
        // 3. 배경 침 침 시작
        StartBackgroundAnimations();
    }
}
```

---

## 오늘 배운 핵심 키워드

**Tilemap**
- `Grid`, `Tilemap`, `Tile Palette`
- `Rule Tile`, `Tilemap Collider 2D`
- `Sorting Layer`, `Composite Collider`

**ScriptableObject**
- `[CreateAssetMenu]`, `[SerializeField]`
- `ScriptableObject.CreateInstance()`
- 데이터 에셋화, 로직 분리

**LitMotion**
- `LMotion.Create()`, `BindToPosition()`
- `Ease.InOutSine`, `LoopType.Yoyo`
- 제로 할당 트위닝

---

## 체크리스트

**Session 5 완료 기준**

- [ ] Tilemap으로 3개 이상 레이어 구성
- [ ] Rule Tile로 자동 타일링 적용
- [ ] ScriptableObject로 레벨 데이터 생성
- [ ] LitMotion으로 패럴랙스 배경 구현

**자기 점검**

- [ ] 타일맵 충돌이 정상 작동하는가?
- [ ] ScriptableObject 데이터가 인스펙터에 보이는가?
- [ ] 배경이 침 침 움직이는가?
- [ ] Claude에게 "ScriptableObject vs JSON"이라고 물었을 때 설명할 수 있는가?

---

## 과제: 집에서 복습하기

**1. 타일맵 과제**

```csharp
// 과제: 랜덤 레벨 생성기
// - ScriptableObject에 타일 배열 저장
// - 런타임에 타일맵에 타일 프로그래밍 배치
// - Tilemap.SetTile(position, tile) 사용
```

**2. AI 협업 과제**

```csharp
// Claude에게 질문:
// "ScriptableObject와 JSON 파일의 장단점을 비교해줘"
// "LitMotion vs DOTween 성능 비교"
```

**3. 데이터 관리 과제**

```csharp
// 3개의 레벨 데이터 ScriptableObject 생성
// 레벨 선택 화면에서 데이터 로드
```

---

<!-- _class: lead -->

## Session 5 완료!

**다음 시간 예고:** NavMesh 2D AI — NavMeshAgent 경로 탐색·추격/패트롤 상태 머신

### 질문 있으신가요?

Claude Sonnet 4.6과 함께 계속 공부합시다.

**주요 리소스:**
- [Unity 2D Tilemap Manual](https://docs.unity3d.com/Manual/Tilemap.html)
- [LitMotion GitHub](https://github.com/annulusgames/LitMotion)
- ScriptableObject Best Practices
