# Agent Instructions: ALWAYS READ _RULES.md FILES
**CRITICAL**: Every time you start a new task or process a prompt, you MUST search for and read all rules.md files in the workspace (and specifically in the relevant directories you are modifying, such as src/components/, src/app/, etc.) before generating or modifying any code. These files contain essential, domain-specific architectural constraints and rules that supersede general best practices.

**CRITICAL**: Before applying any frontend changes, you MUST read `frontend-guidelines.md` and follow its requirements.

<!-- BEGIN:nextjs-agent-rules -->
# This is NOT the Next.js you know

This version has breaking changes — APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` before writing any code. Heed deprecation notices.
<!-- END:nextjs-agent-rules -->
