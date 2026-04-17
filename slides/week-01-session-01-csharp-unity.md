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

# 개발 환경 입문

## Session 1: 개발 환경 입문

Unity 실무 중심 메트로배니아 게임 개발

---

## 오늘의 학습 목표

**3시간 동안 배울 내용**

| 시간 | 주제 | 활동 |
|------|------|------|
| 0-20분 | 진단 평가 | Entry Test (C#, Unity, Git 기초) |
| 20-50분 | 해설 설명 | 테스트 문제 풀이 및 핵심 개념 정리 |
| 50-80분 | C# 기초 문법 | 변수, 타입, 연산자, 조건문 |
| 80-110분 | 클래스와 메서드 | 클래스 구조, 배열 |
| 110-140분 | Unity 에디터 | 에디터 워크플로우, Git 기초 |
| 140-170분 | PlayerController 실습 | WASD 이동 구현 |
| 170-180분 | Git 커밋 및 마무리 | 버전 관리, 푸시 |

---

## 진단 평가 (Entry Test)

**현재 나의 수준을 확인해봅시다**

- **시간**: 20분
- **목적**: C# 기초, Unity 기초, Git 기초 수준 파악
- **안내**: 성적에 반영되지 않으니 모르는 문제는 "모름"으로 표기해주세요.
- **참조**: `docs/entry-test-session-01.md`

---

## 해설 설명 (Debrief)

**핵심 개념 다시 짚어보기**

- **C# 기초**: 정수(`int`)와 실수(`float`)의 연산, 조건문(`if/else`) 흐름
- **클래스**: 접근 제한자(`public`, `private`)의 역할
- **Unity**: 생명주기(`Update`, `FixedUpdate`)와 물리(`Rigidbody2D`)
- **Git**: 로컬 저장소와 원격 저장소의 연결

---

## Claude Sonnet 4.6 활용 가이드

**AI 협업 핵심 원칙**

- **Claude Code** 또는 **claude.ai** 사용
- 학생이 **프롬프트**를 직접 작성하며 AI와 대화
- AI는 **코드 생성/리뷰/최적화** 보조 도구
- 모든 결과물은 **이해하고 설명**할 수 있어야 함

---

## 효과적인 프롬프트 작성법

**Claude에게 질문할 때**

```
"C#에서 클래스와 구조체의 차이점을 
예제 코드와 함께 설명해줘."

"Unity에서 Rigidbody2D를 사용한 기본 이동 코드를 
작성해줘. 
조건: 
1) WASD 입력 
2) FixedUpdate 사용 
3) velocity 기반"

"위 코드를 리뷰해줘. 
개선점 3가지를 제시해줘."
```

---

## C# 기초: 변수와 타입

**기본 데이터 타입**

```csharp
// 숫자 타입
int score = 100;           // 정수
float speed = 5.5f;        // 실수

// 논리 타입
bool isAlive = true;       // 참/거짓

// 문자열 타입
string playerName = "Hero";

// Unity 전용
Vector2 position = new Vector2(0, 0);
```

---

## C# 기초: 연산자와 조건문

**기본 연산**

```csharp
// 산술 연산
int a = 10 + 5;    // 15
int b = 10 - 5;    // 5
int c = 10 * 5;    // 50
int d = 10 / 5;    // 2

// 조건문
if (score > 100) {
    Debug.Log("High score!");
} else {
    Debug.Log("Keep trying!");
}
```

---

## AI 실습: C# 기초 코드 생성

**Claude에게 요청할 프롬프트**

```
"Unity에서 플레이어의 점수를 관리하는 
기본 C# 코드를 작성해줘.

요구사항:
- int 타입 score 변수
- 점수를 추가하는 AddScore(int points) 메서드
- 현재 점수를 출력하는 PrintScore() 메서드"
```

**Claude가 생성한 코드를 분석하고 수정해보세요.**

---

## 클래스와 메서드

**기본 클래스 구조**

```csharp
public class Player {
    // 필드 (변수)
    public string name;
    public int health;
    
    // 메서드 (함수)
    public void TakeDamage(int damage) {
        health -= damage;
        if (health < 0) health = 0;
    }
    
    public bool IsAlive() {
        return health > 0;
    }
}
```

---

## 배열과 컬렉션

**배열 사용법**

```csharp
// 배열 선언
int[] scores = new int[5];
scores[0] = 100;
scores[1] = 200;

// 초기화와 함께 선언
string[] enemies = { "Slime", "Goblin", "Dragon" };

// 배열 순회
foreach (string enemy in enemies) {
    Debug.Log(enemy);
}
```

---

## AI 실습: 클래스 설계

**Claude에게 요청할 프롬프트**

```
"RPG 게임의 적(Enemy) 클래스를 설계해줘.

요구사항:
- 이름(name), 체(health), 공격력(attack) 필드
- 데미지를 받는 TakeDamage(int damage) 메서드
- 공격하는 Attack(Player target) 메서드
- 배열로 여러 적을 관리하는 예제"
```

**생성된 코드의 UML 다이어그램을 그려보세요.**

---

## Unity 에디터 기초

**핵심 창들**

| 창 | 역할 | 단축키 |
|----|------|--------|
| **Scene** | 게임 월드 편집 | - |
| **Game** | 게임 실행 화면 | - |
| **Hierarchy** | 오브젝트 목록 | - |
| **Inspector** | 컴포넌트 편집 | - |
| **Project** | 에셋 관리 | Ctrl+9 |
| **Console** | 로그 출력 | Ctrl+Shift+C |

---

## Unity 에디터 핵심 워크플로우

**1. 오브젝트 생성**

```
Hierarchy > 우클릭 > Create Empty
또는
GameObject > Create Empty
```

**2. 컴포넌트 추가**

```
Inspector > Add Component > 
Rigidbody2D, Sprite Renderer 등
```

**3. 스크립트 연결**

```
Inspector > Add Component > New Script
또는 기존 스크립트 드래그
```

---

## Git 기초 개념

**버전 관리란?**

- 코드 변경사항을 저장하고 추적
- 이전 버전으로 되돌리기 가능
- 팀원과 협업 시 필수

**핵심 명령어**

```bash
# 저장소 초기화
git init

# 변경사항 추가
git add .

# 커밋 (버전 저장)
git commit -m "메시지"

# 원격 저장소에 푸시
git push origin main
```

---

## Git 실습: 첫 커밋

**터미널에서 실행**

```bash
# 프로젝트 폴더로 이동
cd MyUnityProject

# Git 초기화
git init

# 모든 파일 추가
git add .

# 첫 커밋
git commit -m "Initial commit: Session 1 setup"

# 상태 확인
git log
```

---

## PlayerController 실습

**WASD 이동 구현**

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate() {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(moveX, moveY);
        rb.velocity = movement * moveSpeed;
    }
}
```

---

## PlayerController 분석

**핵심 키워드 이해하기**

| 키워드 | 의미 |
|--------|------|
| `class` | 객체 설계도 |
| `void` | 반환값 없음 |
| `Start()` | 초기화 (처음 1회) |
| `FixedUpdate()` | 물리 업데이트 (고정 주기) |
| `Rigidbody2D` | 2D 물리 컴포넌트 |
| `velocity` | 속도 벡터 |

---

## AI 실습: PlayerController 개선

**Claude에게 요청할 프롬프트**

```
"아래 PlayerController 코드를 개선해줘.

개선사항:
1) 대각선 이동 시 속도가 너무 빨라지는 문제 수정
2) New Input System 사용
3) 이동 방향에 따라 스프라이트 플립

[기존 코드 붙여넣기]"
```

**Claude의 제안을 하나씩 적용해보세요.**

---

## 오늘 배운 핵심 키워드

**C# 기초**
- `class`, `void`, `int`, `float`, `bool`
- `if`, `else`, `for`, `foreach`
- 배열 `[]`, `new`

**Unity**
- `Update()`, `FixedUpdate()`
- `Rigidbody2D`, `velocity`
- `Input.GetAxis()`, `Vector2`

**Git**
- `git init`, `git add`, `git commit`, `git push`

---

## 체크리스트

**Session 1 완료 기준**

- [ ] Claude Code로 3개 이상 질문하고 답변 이해
- [ ] AI 제안 코드를 수정하여 자신의 코드로 작성
- [ ] GitHub에 직접 커밋하고 푸시

**자기 점검**
- [ ] PlayerController가 WASD로 이동하는가?
- [ ] Git log에서 커밋 기록이 보이는가?
- [ ] AI에게 "이 코드를 설명해줘"라고 물었을 때 설명할 수 있는가?

---

## 과제: 집에서 학습하기

**1. C# 연습 문제**

```csharp
// 과제: 플레이어의 체력 시스템 구현
// - 현재 체력(hp)과 최대 체력(maxHp) 변수
// - 데미지를 입는 TakeDamage(int amount) 메서드
// - 사망 조건: hp <= 0
```

**2. AI 협업 과제**
- Claude에게 "C# 상속에 대해 설명해줘"라고 물어보기
- 예제 코드를 실행하고 결과 캡처

**3. Git 과제**
- 오늘 작성한 코드를 GitHub에 푸시
- README.md 파일 생성

---

<!-- _class: lead -->

## Session 1 완료!

**다음 시간 예고:** 실무 아키텍처 & LitMotion — Clean Architecture·VContainer DI·R3·UniTask

### 질문 있으신가요?

Claude Sonnet 4.6과 함께 계속 공부합시다.
