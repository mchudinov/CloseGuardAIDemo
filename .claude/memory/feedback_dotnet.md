---
name: .NET / C# Conventions
description: Coding conventions and lessons learned for this .NET project
type: feedback
originSessionId: 031220a4-d066-4180-8228-30a3b7761532
---
- Use local `Random` instances per method call (not a shared static field) when methods need independent deterministic sequences — shared static `Random` causes cross-method state interference.
- Use deterministic seeds (e.g. `new Random(42)`) in data generators so tests are repeatable across runs.
- Inject anomalies at fixed array indices (not random selection) to guarantee deterministic anomaly counts in tests.
- Remote branch may already be deleted by GitHub after merge auto-delete — `git push origin --delete <branch>` returning "remote ref does not exist" is benign.

**Why:** Encountered during Steps 3–4 implementation.
**How to apply:** Always use local Random, not static, when seeding per-method. Always verify determinism of test data generators before writing count assertions.
