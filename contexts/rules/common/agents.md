# Agent Orchestration

## Available Agents

Located in `contexts/agents/`:

| Agent | Purpose | When to Use |
|-------|---------|-------------|
| planner | Implementation planning | Complex features, refactoring |
| architect | System design | Architectural decisions |
| tdd-guide | Test-driven development | New features, bug fixes |
| code-reviewer | Code review | After writing code |
| security-reviewer | Security analysis | Before commits |
| build-error-resolver | Fix build/type errors | When build fails |
| refactor-cleaner | Dead code cleanup | Code maintenance |
| doc-updater | Documentation | Updating docs |
| docs-lookup | Documentation lookup via Context7 | API/docs questions |
| python-reviewer | Python code review | Python projects |
| typescript-reviewer | TypeScript/JavaScript code review | TypeScript/JavaScript projects |
| code-architect | Code architecture analysis | Structural decisions |
| code-explorer | Codebase exploration and navigation | Understanding unfamiliar code |
| code-simplifier | Code simplification and clarity | Reducing complexity |
| comment-analyzer | Code comment quality analysis | Documentation review |
| performance-optimizer | Performance profiling and tuning | Performance bottlenecks |
| seo-specialist | SEO analysis and optimization | Search visibility |
| silent-failure-hunter | Detect silent failures and swallowed errors | Reliability audits |

## Immediate Agent Usage

No user prompt needed:
1. Complex feature requests - Use **planner** agent
2. Code just written/modified - Use **code-reviewer** agent
3. Bug fix or new feature - Use **tdd-guide** agent
4. Architectural decision - Use **architect** agent

## Parallel Task Execution

ALWAYS use parallel Task execution for independent operations:

```markdown
# GOOD: Parallel execution
Launch 3 agents in parallel:
1. Agent 1: Security analysis of auth module
2. Agent 2: Performance review of cache system
3. Agent 3: Type checking of utilities

# BAD: Sequential when unnecessary
First agent 1, then agent 2, then agent 3
```

## Multi-Perspective Analysis

For complex problems, use split role sub-agents:
- Factual reviewer
- Senior engineer
- Security expert
- Consistency reviewer
- Redundancy checker
