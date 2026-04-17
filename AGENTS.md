# COURSE-MATERIALS KNOWLEDGE BASE

## OVERVIEW
Private repository that bundles the Unity metroidvania course slide decks (`slides/`) and the canonical C# teaching samples (`sample-code/`). Mounted as a submodule at `course-materials/` of the parent [`unity-indie-school`](https://github.com/islee23520/unity-indie-school) repository.

## SCOPE
- 이 저장소는 **강의 자료 원본**만 담는다 — 슬라이드 + 샘플 코드.
- 학생 게임 프로젝트, 웹 포트폴리오, 슬라이드 생성 CLI, 관리 행정 자료는 모두 **부모 저장소 쪽에 있으므로 여기로 가져오지 않는다**.
- 슬라이드에서 참조되는 C# 샘플 코드만이 `sample-code/`의 대상이다.

## LANGUAGE
- 별도 요청이 없으면 문서와 설명은 한국어를 기본으로 유지한다.
- 코드 식별자, 클래스명, 함수명은 기존 C# 네이밍과 `sample-code/README.md` 규칙을 따른다.

## STRUCTURE
```text
unity-indie-school-materials/
├── slides/                  # Marp markdown slide source (14 sessions + course overview)
├── sample-code/             # Canonical C# teaching samples, session-scoped
│   ├── README.md            # Authoring contract + session-to-keyword matrix
│   ├── AGENTS.md            # Local authoring rules for samples
│   └── session1/ ... session14/
├── AGENTS.md                # This file
├── README.md
├── LICENSE
└── .gitignore
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Repo overview | `README.md` | Tech stack + session matrix |
| Slide source | `slides/*.md` | Marp markdown per session |
| Canonical C# samples | `sample-code/sessionN/` | Referenced by slides |
| Sample authoring rules | `sample-code/README.md` | Allman + 4 spaces + session matrix |
| Sample local rules | `sample-code/AGENTS.md` | Session-scoping discipline |

## TECH STACK (AUTHORITATIVE)
모든 샘플 코드와 슬라이드 내 코드 블록은 아래 스택에 정렬되어야 한다.

| Domain | Tech |
|---|---|
| Engine | Unity 6.3 LTS |
| Architecture | Clean Architecture + **VContainer** (DI) |
| Reactive | **R3** (Reactive Extensions) |
| Async | **UniTask** |
| Animation | **Spine 4.2** + **LitMotion** |
| UI | **UI Toolkit + MVVM** |
| AI | Unity NavMesh 2D |
| Data | ScriptableObject |
| Localization | Unity Localization |
| Steam | Steamworks.NET |
| CI/CD | GitHub Actions |

> 위 스택에서 벗어난 코드(Zenject, DOTween, UGUI 기반 MVVM, Unity 2022 전용 API 등)는 표준 스택으로 업데이트 대상이다.

## CONVENTIONS
- 슬라이드는 **단일 진실 공급원**이 아니다. 코드 원본은 항상 `sample-code/sessionN/`에 있고 슬라이드는 이를 참조/발췌한다.
- `sample-code/` 수정 시 해당 session의 슬라이드에서 참조 경로/스니펫이 여전히 유효한지 확인한다.
- C# 스타일: **Allman 중괄호**, **스페이스 4칸 들여쓰기**, 컴파일 가능한 형태 유지 (스니펫이어도).
- session 경계를 넘어 코드를 재사용하거나 공유 모듈로 뽑지 않는다 (교육 목적상 세션 독립).
- Marp front matter / theme 설정은 기존 슬라이드와 통일되도록 유지한다.

## ANTI-PATTERNS
- slides에서 `sample-code/` 외부의 경로(예: 부모 repo의 임의 파일)를 참조하지 않는다.
- `sample-code/` 내부에서 상대 경로로 부모 Unity 프로젝트 폴더(`Assets/`, `Packages/`)가 있는 것처럼 가정하지 않는다.
- 스택과 다른 기술(UGUI, Zenject, DOTween 등)을 "더 쉬우니까" 도입하지 않는다 — 스택 유지가 커리큘럼의 핵심.
- 여러 session을 동시에 리팩터링해 공유 추상을 만들지 않는다.
- 생성물/빌드 산출물(PPTX, HTML 렌더, PDF)을 저장소에 커밋하지 않는다 — 소스만 유지.

## UNIQUE STYLES
- 세션 번호 축(1~14)이 모든 구조의 1차 정렬 키이다.
- 스택 motif 반복: VContainer DI → R3 ReactiveProperty → UniTask 비동기 → LitMotion 트윈 → UI Toolkit 뷰 바인딩.
- 주석/설명은 한국어가 기본, 식별자만 영어 유지.

## COMMANDS
```bash
# Slide preview / PDF export
npx @marp-team/marp-cli slides/course-overview.md -o course-overview.pdf
npx @marp-team/marp-cli slides/week-04-session-07-combat.md -o session07.pdf

# C# 샘플 빠른 문법 확인 (원한다면 로컬 .csproj에 임시로 포함)
dotnet build <optional temp csproj>
```

## NOTES
- 부모 repo 클론 시 반드시 `--recurse-submodules` 또는 `git submodule update --init` 필요.
- 본 저장소는 private이다 — PR/이슈 공유 시 소스 노출에 주의한다.
- 슬라이드 생성 CLI(`tools/slide-generator/`)는 부모 저장소에 있으며 이 저장소에는 두지 않는다.
