# Unity Indie School — Course Materials

> **Private repository** containing the slide decks and canonical C# code samples for the 14-session Unity metroidvania course taught at Unity Indie School.

This repository is mounted into the parent course repository ([`islee23520/unity-indie-school`](https://github.com/islee23520/unity-indie-school)) as a submodule at `course-materials/`. It is separated out because it bundles instructor-facing slide material together with the production-style C# samples the slides reference, and is not intended for public distribution.

---

## Tech Stack (authoritative)

All slides and sample code target the following stack. Anything that deviates from this should be updated.

| Domain | Technology | Purpose |
|---|---|---|
| Engine | **Unity 6.3 LTS** | Game engine |
| Architecture | **Clean Architecture + VContainer** | Structure & DI |
| Reactive | **R3 (Reactive Extensions)** | Data flow |
| Async | **UniTask** | Async operations |
| Animation | **Spine 4.2 + LitMotion** | 2D animation & tweening |
| UI | **UI Toolkit + MVVM** | User interface |
| AI | **Unity NavMesh 2D** | Pathfinding |
| Data | **ScriptableObject** | Game data management |
| Rendering | **URP + Sprite Atlas** | Graphics & optimization |
| Localization | **Unity Localization** | Multi-language support |
| Platform | **Steamworks.NET** | Steam integration |
| CI/CD | **GitHub Actions** | Build automation |

---

## Repository Layout

```text
unity-indie-school-materials/
├── slides/                  # Marp markdown slide source (per session)
│   ├── course-overview.md
│   ├── week-01-session-01-csharp-unity.md
│   ├── ...
│   └── week-07-session-14-steam-build.md
├── sample-code/             # Canonical C# teaching samples, session-scoped
│   ├── README.md            # Authoring contract & session-to-keyword matrix
│   ├── AGENTS.md            # Local authoring rules
│   ├── session1/ ... session14/
├── AGENTS.md                # Repo-wide contributor guide
├── README.md                # This file
├── LICENSE                  # MIT
└── .gitignore
```

### slides ↔ sample-code relationship

`slides/*.md` are the single source of truth for course presentations and **reference `sample-code/sessionN/`** when they need to embed or link to concrete C# examples. When updating a code snippet in the slides, update the canonical file under `sample-code/` first, then reference it from the slide.

---

## Session Matrix

| Week | Session | Slide file | Topic |
|---|---|---|---|
| 1 | 1 | `slides/week-01-session-01-csharp-unity.md` | C# & Unity immersion |
| 1 | 2 | `slides/week-01-session-02-architecture-litmotion.md` | Architecture & LitMotion |
| 2 | 3 | `slides/week-02-session-03-spine-litmotion.md` | Spine animation |
| 2 | 4 | `slides/week-02-session-04-playercontroller.md` | Player controller |
| 3 | 5 | `slides/week-03-session-05-tilemap-so.md` | Tilemap & ScriptableObject |
| 3 | 6 | `slides/week-03-session-06-navmesh-ai.md` | NavMesh 2D AI |
| 4 | 7 | `slides/week-04-session-07-combat.md` | Combat system |
| 4 | 8 | `slides/week-04-session-08-enemy-pooling.md` | Enemy AI & pooling |
| 5 | 9 | `slides/week-05-session-09-ability.md` | Ability system |
| 5 | 10 | `slides/week-05-session-10-save-localization.md` | Save/load & localization |
| 6 | 11 | `slides/week-06-session-11-ui-mvvm.md` | UI Toolkit & MVVM |
| 6 | 12 | `slides/week-06-session-12-menu-animator.md` | Menu & Animator |
| 7 | 13 | `slides/week-07-session-13-optimization.md` | Optimization & polish |
| 7 | 14 | `slides/week-07-session-14-steam-build.md` | Steam release & CI/CD |

---

## Usage

### Inside the parent course repository (recommended)

```bash
# Clone the parent repo with submodules
git clone --recurse-submodules https://github.com/islee23520/unity-indie-school.git
cd unity-indie-school/course-materials
```

### Standalone clone

```bash
git clone https://github.com/islee23520/unity-indie-school-materials.git
cd unity-indie-school-materials
```

### Regenerate slide PDFs

```bash
npx @marp-team/marp-cli slides/course-overview.md -o course-overview.pdf
```

---

## Contribution Rules

- C# samples follow **Allman braces** + **4-space indentation** (see `sample-code/README.md`).
- Preserve session scoping under `sample-code/sessionN/`; do not refactor across sessions.
- Keep slide files self-contained Marp markdown; reference sample code by relative path.
- Any deviation from the tech stack table above (e.g., DOTween instead of LitMotion, Zenject instead of VContainer) must be updated to match the authoritative stack.

---

## License

Released under the [MIT License](./LICENSE) — same terms as the parent `unity-indie-school` project.
