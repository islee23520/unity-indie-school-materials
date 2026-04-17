# SAMPLE-CODE KNOWLEDGE BASE

## OVERVIEW
Canonical Unity C# teaching samples organized by session. These snippets are referenced by the markdown-first curriculum materials and are not standalone production systems.

## STRUCTURE
```text
sample-code/
├── session1/ ... session14/   # Session-aligned Unity examples
└── README.md                  # Session matrix + authoring rules
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Style rules | `README.md` | Allman + 4 spaces |
| Combat topics | `session7/`, `session8/` | Health, hurtbox, pooling |
| UI Toolkit topics | `session11/`, `session12/` | UXML/USS, MVVM, menu flow |
| Release/build topics | `session14/` | Steam build + CI/CD examples |
| Architecture intro | `session2/` | VContainer, settings, tween basics |

## CONVENTIONS
- Preserve session scoping; each folder teaches a specific milestone.
- Keep snippets valid C# shape even when simplified.
- Use Allman braces and 4-space indentation.
- Favor pedagogical clarity over abstraction-heavy reuse across sessions.

## ANTI-PATTERNS
- Do not refactor unrelated sessions together; they are not one shared runtime.
- Do not introduce repo-local dependencies or assumptions about missing Unity project folders.
- Do not optimize snippets at the expense of readability for teaching.

## UNIQUE STYLES
- Repeated stack motifs: VContainer, R3, UniTask, LitMotion, UI Toolkit MVVM.
- Session progression matters more than cross-file reuse.

## NOTES
- No Unity test framework here.
- `README.md` is the canonical authoring contract for this subtree.
- Top-level path is `sample-code/`; do not reintroduce parent-folder assumptions.
