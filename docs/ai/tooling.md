# AI Tooling Context (Rider + CLI Agents)

Summary of local IDE and AI coding-agent setup for `SmartERP.Backend` on Windows (JetBrains Rider, non-commercial license).

> **Security rule**: Never store API keys, passwords, connection strings, or tokens in this file or in prompts.

## Goal
Rotate smoothly between free AI coding agents as quotas reset, while keeping context consistent via `AGENTS.md` and `docs/ai/`.

## Rotation & Resume Cheatsheet
- **Preferred rotation**: `agy` (Antigravity CLI) → OpenCode → Kilo (Codex CLI as extra pool).
- **Resume chats from project folder**:
  - `opencode --continue`
  - `codex resume`
  - `agy --help` (check session/resume flags)

## Current Agent Status (as of 2026-11-26)
| Agent | Where | Status | Notes |
| :--- | :--- | :--- | :--- |
| `agy` (Antigravity CLI) | Terminal | Works | Signed in with Google account. |
| Antigravity in Rider (ACP) | Rider chat pane | Needs auth switch | Registered in `%USERPROFILE%\.jetbrains\acp.json` → `agy.exe ["acp"]`. Currently reading `gemini-api-key` from `%USERPROFILE%\.gemini\antigravity-acp\settings.json` instead of Google account. |
| Junie | Rider | Works via Google AI Studio key | Do not use `/login` or "Check Sign-In" (non-commercial license has no JetBrains AI cloud and pane can hang). |
| Kilo | Rider + CLI | Works | Use Kilo Gateway "Auto Free". |
| OpenCode | Terminal | PATH check pending | Installed via curl, but `opencode`/`kilo` not yet on PATH in shell (check `$HOME\.opencode\bin` or fallback `npm install -g opencode-ai`). |
| Codex CLI | Terminal | Not installed yet | Included in free ChatGPT plan (`/status` to check limit). |
| Cursor, Qoder, GitHub Copilot, Claude Code, Gemini CLI, Qwen Code | — | Unavailable / limited | Free tiers ended or limits hit. |

## Antigravity in Rider (ACP): Troubleshooting Notes
1. `notebooks` / `visualization` MCP servers failed with ENOENT named-pipe errors when Antigravity IDE wasn't installed; fix in `%USERPROFILE%\.gemini\config\mcp_config.json` is `{ "mcpServers": {} }`, then restart Rider and start a new session.
2. Installing the full Antigravity IDE is not needed for quota.
3. Next step to switch Rider ACP from API key to Google account login:
   - Close Rider, rename `%USERPROFILE%\.gemini\antigravity-acp\settings.json` to `settings.json.bak`, reopen Rider, start a new Antigravity session, and select **Log in with Google** (also remove any `GEMINI_API_KEY` env var if it still falls back to key auth).

## Open Tooling Items
- Switch Antigravity in Rider (ACP) to Google account login.
- Fix `opencode` / `kilo` command recognition in terminal PATH.
- Optionally install Codex CLI.
- Optional: if opening `docs/ai` as an Obsidian vault, `.obsidian/` is already ignored in `.gitignore`.
