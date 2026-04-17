---
marp: true
theme: default
class: invert
paginate: true
backgroundColor: #1a1a2e
color: #eaeaea
style: |
  section {
    font-family: 'Noto Sans KR', sans-serif;
  }
  h1 {
    color: #00d4aa;
    font-size: 2.5em;
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
    padding: 16px;
    border-radius: 8px;
  }
  .highlight {
    color: #ff6b6b;
    font-weight: bold;
  }
  .tip {
    background-color: #16213e;
    border-left: 4px solid #00d4aa;
    padding: 12px;
    margin: 12px 0;
  }
---

<!-- _class: lead -->

# Session 13: 최적화 & 폴리싱
## Draw Call 최적화 + 폴리싱

Unity 2D 성능 마스터하기

---

# 오늘의 목표

1. **Draw Call**이 무엇인지 이해하기
2. **Sprite Atlas**로 텍스처 최적화하기
3. **SRP Batcher** 활용법 익히기
4. **Frame Debugger**로 병목 찾기
5. 보스 배틀에 폴리싱 적용하기

---

# Draw Call이란?

## GPU에게 "그려!"라고 외치는 순간

```csharp
// CPU가 GPU에게 별도의 명령을 내릴 때마다 발생
// 텍스처 바꾸기, 셰이더 변경, 메시 전환...

// 나쁜 예: 텍스처가 다른 오브젝트들
Sprite A (texture1.png)  -> Draw Call #1
Sprite B (texture2.png)  -> Draw Call #2  
Sprite C (texture1.png)  -> Draw Call #3
// 총 3번의 Draw Call!
```

<p class="highlight">Draw Call = 성능의 적! 줄이는게 핵심!</p>

---

# Sprite Atlas: 텍스처 합치기

## 여러 이미지를 하나로 뭉치자

```csharp
// Sprite Atlas 생성
// Window -> 2D -> Sprite Atlas

// 자동으로 포함될 폰더 설정
public class AtlasConfig : MonoBehaviour
{
    void Start()
    {
        // 런타임에 Atlas에서 Sprite 로드
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UI_Atlas");
        Sprite buttonSprite = atlas.GetSprite("button_start");
        GetComponent<Image>().sprite = buttonSprite;
    }
}
```

---

# Sprite Atlas 설정 팁

## 효율적인 Atlas 구성법

```csharp
// 폭발 효과 Atlas - 자주 같이 쓰이는 것들
// [explosion_01] [explosion_02] [explosion_03] [smoke_01]

// UI Atlas - 메뉴 화면용
// [button_start] [button_exit] [panel_bg] [icon_coin]

// 캐릭터 Atlas - 플레이어 관련
// [player_idle] [player_run] [player_attack] [player_hurt]
```

<div class="tip">
💡 팁: 2048x2048 이하로 유지. 큰 Atlas는 메모리 낭비!
</div>

---

# SRP Batcher (Scriptable Render Pipeline)

## 셰이더 변환 비용 줄이기

```csharp
// URP(Universal Render Pipeline)에서 자동 적용
// SRP Batcher는 "같은 셰이더"를 쓰는 오브젝트들을
// 한 번에 batching 처리

// 조건:
// 1. 같은 Shader 사용
// 2. Material 속성이 비슷
// 3. SRP Batcher compatible shader 사용
```

---

# SRP Batcher 확인하기

## 인스펙터로 체크

```csharp
// Scene 뷰 -> Overlay -> Stats 클릭
// 또는 Window -> Analysis -> Frame Debugger

// SRP Batcher가 활성화되면:
// - Batches 수가 크게 감소
// - SetPass calls 감소
// - "SRP Batcher"로 항목 표시됨
```

<div class="tip">
✅ URP 프로젝트면 SRP Batcher는 기본 ON!
Project Settings -> Graphics -> SRP Batcher 체크 확인
</div>

---

# Frame Debugger로 분석하기

## 병목의 원인을 찾아라

```csharp
// Window -> Analysis -> Frame Debugger
// 또는 단축키: Ctrl + Shift + F (Windows)

// 사용법:
// 1. Enable 버튼 클릭
// 2. 왼쪽 패널에서 각 Draw Call 확인
// 3. 왜 batching이 안됐는지 원인 파악
```

---

# Frame Debugger 해석법

## 빨간색 경고를 주목하라

```csharp
// Batching break 원인 예시:

// [X] Different texture (다른 텍스처)
// [X] Different material (다른 머티리얼)  
// [X] Different shader (다른 셰이더)
// [X] Z-write / Z-test mismatch

// 해결책:
// Texture -> Sprite Atlas 사용
// Material -> Material property block 사용
// Shader -> Shader variants 최소화
```

---

# 실전: 보스 배틀 최적화

## Before vs After

```csharp
// BEFORE: 각 효과마다 별도 Draw Call
Boss body        -> Draw Call (texture: boss.png)
Boss arm         -> Draw Call (texture: boss_arm.png)
Fire effect      -> Draw Call (texture: fire_01.png)
Smoke effect     -> Draw Call (texture: smoke_01.png)
// 총 4개 Draw Call

// AFTER: Atlas 하나로 통합
Boss + Effects   -> 1 Draw Call (texture: BossAtlas.png)
```

---

# 보스 배틀 Sprite Atlas 구성

## atlas_boss_battle 설정

```csharp
// 포함할 에셋들:
// - Sprites/Boss/body*.png
// - Sprites/Boss/arm_*.png
// - Sprites/Effects/fire_*.png
// - Sprites/Effects/smoke_*.png
// - Sprites/UI/boss_hp_bar.png

// Inspector 설정:
// Type: Tight packing (공간 효율적)
// Allow Rotation: ON (더 많이 채움)
// Read/Write: OFF (빌드 크기 감소)
```

---

# 보스 폴리싱: 파티클 시스템

## 최적화된 이펙트 만들기

```csharp
public class BossDeathEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathParticles;
    
    public void PlayDeathEffect()
    {
        // 파티클 수 제한으로 성능 보장
        var main = deathParticles.main;
        main.maxParticles = 50;  // 너무 많으면 렉!
        
        // Texture sheet animation (Atlas 활용)
        var texAnim = deathParticles.textureSheetAnimation;
        texAnim.enabled = true;
        texAnim.mode = ParticleSystemAnimationMode.Sprites;
        // Atlas의 sprite들을 순환 재생
        
        deathParticles.Play();
    }
}
```

---

# 코드 최적화: Object Pooling

## Instantiate는 비싸다, 재활용하자

```csharp
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    public GameObject GetProjectile()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(projectilePrefab);
    }
    
    public void ReturnProjectile(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

---

# Shader 최적화 팁

## Simple Lit vs Lit

```hlsl
// 2D 게임에는 Simple Lit 권장
// 불필요한 3D 라이팅 계산 제거

// Custom Shader 작성 시:
Shader "Custom/Fast2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        
        Pass
        {
            // 핵심: SRP Batcher compatible
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // ... (최소한의 연산만)
            ENDHLSL
        }
    }
}
```

---

# AI 프롬프트: 자동 최적화

## ChatGPT/Claude에게 물어보기

```
"Unity 2D 프로젝트의 Draw Call을 분석하고 
줄일 수 있는 방법을 제시해줘. 
현재 Frame Debugger 결과:
- SetPass calls: 45
- Batches: 52
- 사용 중인 텍스처: 23개"
```

```
"Sprite Atlas 구성을 최적화해줘.
다음 에셋들을 효율적으로 묶는 방법:
- 캐릭터 스프라이트 15개
- UI 요소 20개  
- 이펙트 30개
메모리는 512MB 제한"
```

---

# AI 프롬프트: 코드 리뷰

## 성능 병목 찾기

```
"다음 Unity C# 코드의 성능 문제를 찾아줘:
[코드 붙여넣기]

특히 GC allocation과 Draw Call 관련
문제점을 지적해줘."
```

```
"Object Pooling 패턴을 적용해서
다음 코드를 리팩토링해줘:
[Instantiate/Destroy 사용 코드]

풀 크기는 50개로 제한하고,
메모리 단편화 방지 로직도 추가해줘."
```

---

# AI 프롬프트: 셰이더 최적화

## ShaderLab 코드 개선

```
"다음 Unity Shader를 SRP Batcher 
호환으로 수정해줘:
[셰이더 코드 붙여넣기]

CBUFFER_START(UnityPerMaterial)
안에 필요한 프로퍼티만 포함시키고,
불필요한 연산은 제거해줘."
```

```
"2D 게임용으로 최적화된 
Unlit Shader 코드를 작성해줘.
요구사항:
- Sprite Tint 색상 지원
- Alpha Blending
- SRP Batcher 호환
- 모바일에서도 빠름"
```

---

# 체크리스트: 보스 배틀 최적화

## 배포 전 꼭 확인하자

- [ ] 모든 스프라이트가 Atlas에 포함됨
- [ ] Frame Debugger로 Batches 확인 (목표: <20)
- [ ] SRP Batcher 활성화됨
- [ ] Object Pooling 적용됨
- [ ] Particle max particles 제한 설정
- [ ] 모바일 기기에서 60fps 유지
- [ ] 메모리 프로파일러로 Atlas 크기 확인

---

<!-- _class: lead -->

# 정리

| 기술 | 효과 |
|------|------|
| Sprite Atlas | 텍스처 batching |
| SRP Batcher | 셰이더 batching |
| Frame Debugger | 병목 분석 |
| Object Pooling | GC 최적화 |

<p class="highlight">최적화는 끝이 없다. 프로파일러와 함께하자!</p>

---

<!-- _class: lead -->

# Q&A

## 질문 있으신가요?

다음 세션: **Session 14 - Steam 출시 & CI/CD — Steamworks.NET·GitHub Actions·steamcmd 업로드**

---

<!-- _class: lead -->

# 감사합니다!

**Happy Optimizing!** ⚡
