---
name: Step Workflow Feedback
description: Rules for the step-by-step GitHub Projects + TDD workflow
type: feedback
originSessionId: 031220a4-d066-4180-8228-30a3b7761532
---
Always follow this exact workflow for each Step:

1. Create Git branch named after the step (e.g. `Step-5-Implement-ExceptionScorer`)
2. Move GitHub project item to **In-progress**
3. Write failing tests first (TDD — red before green, always)
4. Implement minimal production code to pass
5. Move item to **In-review**, create PR — never merge yourself
6. When user asks to check PR: `gh pr view <n> --json reviewDecision,mergeStateStatus,state`
7. If MERGED: checkout main, pull, delete local+remote branch, move item to **Done**, run `/compact`

**Why:** User enforced this after Step-1 was written without TDD and had to be deleted and restarted.
**How to apply:** No production code before a failing test. No merging PRs. No skipping branch cleanup. Run `/compact` after every completed step.
