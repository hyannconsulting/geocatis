---
mode: agent
description: Guide an AI agent to plan, break down, and generate actionable GitHub issues for a software project using the Model Context Protocol (MCP).
author: Daniel Meppiel
mcp:
  - ghcr.io/github/github-mcp-server
---

# Planning Assistant: Issue Planner & MCP Generator

This workflow helps you systematically plan features, break them down, and generate well-structured GitHub issues using MCP best practices.

## 1. Planning Context Phase

- Review the project documentation and backlog
- Identify high-level goals and priorities
- Understand dependencies and constraints

**Guiding Questions:**
- What are the main deliverables for this cycle?
- What are the critical paths and blockers?
- Are there dependencies between features?

## 2. Feature Breakdown Phase

- Decompose high-level features into smaller, actionable tasks
- Ensure each task is independent and testable
- Map dependencies between tasks

**Output Creation:**
- List all tasks with clear descriptions
- For each, note dependencies and estimated effort

## 3. GitHub Issue Generation Phase

- For each task, create a GitHub issue with:
  - Clear title and description
  - Acceptance criteria
  - Implementation hints or references
  - Labels (e.g., enhancement, priority, MCP)
  - MCP metadata if relevant

**Issue Template Structure:**
```
# [Task Name]

## Description
[What needs to be done and why]

## Acceptance Criteria
- [ ] List of testable requirements

## Implementation Reference
[Links, code snippets, or related issues]

## Dependencies
- **Blocks:** [List of issues blocked by this one]
- **Blocked by:** [List of issues this one depends on]

## Effort Estimate
[Small/Medium/Large]
```

## 4. Review & Optimization Phase

- Review all generated issues for clarity and completeness
- Optimize for parallel work and minimal dependencies
- Ensure all issues are linked and labeled appropriately

**Remember:**
- Prioritize clarity and actionability
- Use MCP metadata for traceability
- Keep issues small and focused
