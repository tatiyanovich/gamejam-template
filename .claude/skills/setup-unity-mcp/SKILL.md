---
name: setup-unity-mcp
description: Diagnose and fix Unity MCP connection for Claude Code. Use when Unity MCP tools fail to connect or need initial setup.
user_invocable: true
---

# Setup / Fix Unity MCP Connection

Diagnose and configure the Unity MCP connection so Claude Code can communicate with the Unity Editor.

## Prerequisites

The user must have:
- Unity 6 open with `com.unity.ai.assistant` package installed
- The Unity Bridge running (Edit > Project Settings > AI > Unity MCP shows **Running**)

## Steps

### 1. Check if `.mcp.json` exists at the project root

Read `/Volumes/SSD_250_GB/ProjectsSSD/block-builders/.mcp.json`. If it doesn't exist or has wrong config, create/fix it with:

```json
{
  "mcpServers": {
    "unity-mcp": {
      "command": "/Users/<you>/.unity/relay/relay_mac_arm64.app/Contents/MacOS/relay_mac_arm64",
      "args": ["--mcp"]
    }
  }
}
```

Key details:
- **Transport:** stdio (NOT sse, NOT websocket)
- **Binary location:** `~/.unity/relay/relay_mac_arm64.app/Contents/MacOS/relay_mac_arm64` (installed automatically by the Unity AI Assistant package)
- **Required flag:** `--mcp` tells the relay to operate as an MCP server
- The relay binary communicates with the Unity Editor's bridge via WebSocket on port 9001

### 2. Verify the relay binary exists

```bash
ls -la ~/.unity/relay/relay_mac_arm64.app/Contents/MacOS/relay_mac_arm64
```

If missing, tell the user to open Unity — the binary is auto-installed when the editor loads the AI Assistant package.

### 3. Verify Unity's bridge is running

```bash
ps aux | grep relay_mac_arm64 | grep -v grep
```

Should show a process with `--relay --port 9001`. If not running, ask the user to check Unity's Project Settings > AI > Unity MCP and click **Start** on the Unity Bridge.

### 4. After config changes

Tell the user to restart Claude Code (`/exit` and relaunch) for the MCP server config to be picked up.

### 5. First connection approval

On first connect, Unity shows a **Pending Connection** in Edit > Project Settings > AI > Unity MCP. The user must click **Accept**. Previously approved clients reconnect automatically.

### 6. Test the connection

After restart, verify tools are available by trying:
- `Unity_ReadConsole` to read console output
- `Unity_RunCommand` to execute a simple script

## Platform-specific relay paths

| Platform | Path |
|----------|------|
| macOS (Apple Silicon) | `~/.unity/relay/relay_mac_arm64.app/Contents/MacOS/relay_mac_arm64` |
| macOS (Intel) | `~/.unity/relay/relay_mac_x64.app/Contents/MacOS/relay_mac_x64` |
| Windows | `%USERPROFILE%\.unity\relay\relay_win.exe` |
| Linux | `~/.unity/relay/relay_linux` |

## Common issues

- **"Unable to connect" / SSE auth failed** — Wrong transport type. Must use stdio command, NOT sse/url.
- **Port 8090/9001 not responding** — Don't connect to the WebSocket port directly. The relay binary handles the connection internally.
- **"Invalid host" on port 38000** — That's Unity's internal debugger port, not the MCP endpoint.
- **Tools not appearing after restart** — Check that the relay binary path is correct and the `--mcp` flag is present.
