# Agent Instructions: ALWAYS READ _RULES.md and docs FILES

**CRITICAL**: Every time you start a new task or process a prompt, you MUST search for and read all `rules.md` or related docs files in the workspace (and specifically in the relevant directories you are modifying, such as `esecai.API/`, `esecai.Application/`, `esecai.Domain/`, `esecai.Infrastructure/`) before generating or modifying any code. These files contain essential, domain-specific architectural constraints and rules that supersede general best practices.

## Required Reading (Backend)

Before writing or modifying **any** backend code, you **MUST** read:

1. **`esecai.API/rules.md`** — Domain-based structure, naming conventions, and best practices.
2. **`docs/backend-guidelines.md`** — Reusable modules, authentication patterns, single-source-of-truth references, and common anti-patterns (do this, not that).
3. **`docs/dotnet-structure.md`** — .NET conventions for async/sync routes, Pydantic, dependencies, and database.

These documents reflect the actual codebase. Follow them strictly.