#!/bin/bash
set -euo pipefail

# Only run in Claude Code on the web; locally, devs already have their own SDK.
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

PROJECT_DIR="${CLAUDE_PROJECT_DIR:-$(cd "$(dirname "$0")/../.." && pwd)}"
DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
DOTNET_CHANNEL="10.0"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

install_dotnet() {
  echo "Installing .NET SDK channel ${DOTNET_CHANNEL} into ${DOTNET_ROOT}..." >&2
  mkdir -p "$DOTNET_ROOT"
  local script="$DOTNET_ROOT/dotnet-install.sh"
  local url
  for url in \
    "https://dot.net/v1/dotnet-install.sh" \
    "https://builds.dotnet.microsoft.com/dotnet/scripts/v1/dotnet-install.sh" \
    "https://raw.githubusercontent.com/dotnet/install-scripts/main/src/dotnet-install.sh"; do
    if curl -fsSL "$url" -o "$script"; then
      echo "Fetched dotnet-install.sh from $url" >&2
      break
    fi
    echo "Failed to fetch $url, trying next..." >&2
  done
  [ -s "$script" ] || { echo "Could not download dotnet-install.sh" >&2; exit 1; }
  chmod +x "$script"
  "$script" --channel "$DOTNET_CHANNEL" --install-dir "$DOTNET_ROOT" --no-path
}

if [ ! -x "$DOTNET_ROOT/dotnet" ]; then
  install_dotnet
fi

export PATH="$DOTNET_ROOT:$PATH"

if ! dotnet --list-sdks | grep -q '^10\.'; then
  install_dotnet
fi

echo "dotnet $(dotnet --version) ready" >&2

# Persist environment for the session so subsequent tool calls find dotnet.
if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
  {
    echo "export DOTNET_ROOT=\"$DOTNET_ROOT\""
    echo "export PATH=\"$DOTNET_ROOT:\$PATH\""
    echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
    echo "export DOTNET_NOLOGO=1"
    echo "export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1"
  } >> "$CLAUDE_ENV_FILE"
fi

# Restore NuGet packages so build/test/lint are warm on first use.
cd "$PROJECT_DIR"
dotnet restore CloseGuardAIDemo.slnx >&2
