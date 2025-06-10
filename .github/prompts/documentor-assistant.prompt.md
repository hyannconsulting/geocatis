---
mode: documentor
description: Guide an AI agent to identify undocumented code and behavior, and generate clear documentation for both code and product features.
author: Daniel Meppiel
mcp:
  - ghcr.io/github/github-mcp-server
---

# Documentor: Documentation Coverage Assistant

This workflow helps you systematically find undocumented functions, modules, and behaviors, and generate high-quality documentation for both code and product features.

## 1. Code Documentation Analysis Phase

- Scan the codebase for public functions, classes, and modules lacking docstrings or comments
- Identify complex logic or APIs without adequate explanation

**Guiding Questions:**
- Which parts of the code are hard to understand without comments?
- Are there public APIs or modules missing usage examples?

## 2. Product Documentation Gap Analysis Phase

- Review product docs (README, PRD, etc.)
- Identify features or behaviors not documented for end users or contributors
- Compare actual implementation with documented features

## 3. Documentation Generation Phase

- Write clear, concise docstrings or comments for undocumented code
- Generate or update product documentation for missing features or behaviors
- Provide usage examples where helpful

**Output Creation:**
- List of updated code comments/docstrings
- List of new or improved product documentation sections

## 4. Review & Integration Phase

- Review all generated documentation for clarity and accuracy
- Integrate documentation into the codebase and product docs
- Suggest improvements for ongoing documentation practices

**Remember:**
- Prioritize clarity and usefulness
- Keep documentation up to date with code changes
- Encourage contributions to documentation from all team members
