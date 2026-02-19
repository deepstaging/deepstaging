#!/bin/bash
# SPDX-FileCopyrightText: 2024-present Deepstaging
# SPDX-License-Identifier: RPL-1.5
# Build and package all Deepstaging NuGet packages
#
# Usage:
#   ./build/pack.sh                    # Build Release with dev version suffix
#   ./build/pack.sh --configuration Debug
#   ./build/pack.sh --version-suffix dev.42
#   ./build/pack.sh --no-version-suffix  # Pack without version suffix (release)
#
# Output: ../../artifacts/packages/
#   - Deepstaging.{version}.nupkg
#   - Deepstaging.Testing.{version}.nupkg

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Default values
CONFIGURATION="Release"
OUTPUT_DIR="$REPO_ROOT/../../artifacts/packages"
UPDATE_VERSIONS=false

# Version suffix: local uses timestamp, CI uses git commit count
if [ -n "${CI:-}" ]; then
    VERSION_SUFFIX="dev.$(git -C "$REPO_ROOT" rev-list --count HEAD)"
else
    VERSION_SUFFIX="local.$(date -u +%Y%m%d%H%M%S)"
fi

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --version-suffix)
            VERSION_SUFFIX="$2"
            shift 2
            ;;
        --no-version-suffix)
            VERSION_SUFFIX=""
            shift
            ;;
        --update-versions)
            UPDATE_VERSIONS=true
            shift
            ;;
        --ci)
            VERSION_SUFFIX="dev.$(git -C "$REPO_ROOT" rev-list --count HEAD)"
            UPDATE_VERSIONS=true
            shift
            ;;
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  -c, --configuration <config>  Build configuration (default: Release)"
            echo "  --version-suffix <suffix>     Version suffix (default: local.TIMESTAMP or dev.N in CI)"
            echo "  --no-version-suffix           Pack without version suffix (for release)"
            echo "  --update-versions             Update committed Versions.props (used by CI)"
            echo "  --ci                          Shorthand for dev.N suffix + --update-versions"
            echo "  -o, --output <dir>            Output directory (default: ../../artifacts/packages)"
            echo "  -h, --help                    Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Build common pack args
PACK_ARGS=(
    "$REPO_ROOT/Deepstaging.slnx"
    --configuration "$CONFIGURATION"
    --output "$OUTPUT_DIR"
    --no-build
)
if [[ -n "$VERSION_SUFFIX" ]]; then
    PACK_ARGS+=(--version-suffix "$VERSION_SUFFIX")
fi

echo "Building Deepstaging ($CONFIGURATION)..."
dotnet build "$REPO_ROOT/Deepstaging.slnx" --configuration "$CONFIGURATION"

echo ""
echo "Syncing package icons..."
"$SCRIPT_DIR/sync-icons.sh" || echo "Warning: Icon sync failed (icons may be missing from packages)"

echo ""
echo "Creating output directory..."
mkdir -p "$OUTPUT_DIR"

echo ""
echo "Packing all packable projects..."
dotnet pack "${PACK_ARGS[@]}"

# Clean up old package versions (keep only last 3 per package)
for prefix in $(ls "$OUTPUT_DIR"/*.nupkg 2>/dev/null | xargs -n1 basename | sed 's/\.[0-9][0-9]*\..*//' | sort -u); do
    ls -t "$OUTPUT_DIR/$prefix".[0-9]*.nupkg 2>/dev/null | tail -n +4 | xargs rm -f
done

# Update centralized version props file
VERSIONS_FILE="$REPO_ROOT/Deepstaging.Versions.props"
VERSIONS_LOCAL_FILE="$REPO_ROOT/Deepstaging.Versions.local.props"
PACK_VERSION=""
VERSIONS_TARGET=""

# Extract the full version from a generated .nupkg filename
NUPKG=$(ls "$OUTPUT_DIR"/Deepstaging.1*.nupkg 2>/dev/null | tail -1)
if [[ -n "$NUPKG" ]]; then
    PACK_VERSION=$(basename "$NUPKG" | sed 's/^Deepstaging\.\(.*\)\.nupkg$/\1/')

    if [[ "$UPDATE_VERSIONS" == "true" ]]; then
        VERSIONS_TARGET="$VERSIONS_FILE"

        cat > "$VERSIONS_FILE" << EOF
<!-- SPDX-FileCopyrightText: 2024-present Deepstaging -->
<!-- SPDX-License-Identifier: RPL-1.5 -->
<!--
  Deepstaging.Versions.props

  Centralized version definitions for Deepstaging NuGet packages.
  This file is automatically updated by CI after publishing to nuget.org.

  Consumer repos import this conditionally in their Directory.Packages.props:
    <Import Project="../deepstaging/Deepstaging.Versions.props" Condition="Exists('...')" />

  For local development, pack.sh writes Deepstaging.Versions.local.props
  (gitignored) which overrides the version below with a local.TIMESTAMP suffix.
-->
<Project>
  <PropertyGroup>
    <DeepstagingVersion>$PACK_VERSION</DeepstagingVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Deepstaging" Version="\$(DeepstagingVersion)" />
    <PackageVersion Include="Deepstaging.Testing" Version="\$(DeepstagingVersion)" />
  </ItemGroup>
</Project>
EOF
    else
        VERSIONS_TARGET="$VERSIONS_LOCAL_FILE"

        cat > "$VERSIONS_LOCAL_FILE" << EOF
<!-- Generated by pack.sh — overrides Versions.props for local development -->
<!-- This file is gitignored. Do not commit. -->
<Project>
  <PropertyGroup>
    <DeepstagingVersion>$PACK_VERSION</DeepstagingVersion>
  </PropertyGroup>
</Project>
EOF
    fi
fi

# Local only: clean, restore, and build sample projects to verify the pack
SAMPLE_RESULTS=()
if [ -z "${CI:-}" ]; then
    SAMPLES_DIR="$REPO_ROOT/../samples"
    if [ -d "$SAMPLES_DIR" ]; then
        while IFS= read -r slnx; do
            SAMPLE_NAME=$(basename "$(dirname "$slnx")")
            echo ""
            echo "Verifying sample: $SAMPLE_NAME..."
            dotnet clean "$slnx" -v q --nologo > /dev/null 2>&1
            dotnet restore "$slnx" --no-cache -q
            if dotnet build "$slnx" -nologo; then
                SAMPLE_RESULTS+=("$SAMPLE_NAME|OK")
            else
                SAMPLE_RESULTS+=("$SAMPLE_NAME|FAILED")
            fi
        done < <(find "$SAMPLES_DIR" -name '*.slnx' -o -name '*.sln' | sort)
    fi
fi

# Summary
BOLD="\033[1m"
DIM="\033[2m"
GREEN="\033[32m"
RED="\033[31m"
CYAN="\033[36m"
RESET="\033[0m"

echo ""
echo -e "${BOLD}Deepstaging Pack Summary${RESET}"
echo ""
if [[ -n "$PACK_VERSION" ]]; then
    DISPLAY_OUTPUT=$(cd "$OUTPUT_DIR" 2>/dev/null && pwd -P || echo "$OUTPUT_DIR")
    PARENT_PREFIX=$(cd "$REPO_ROOT/../.." 2>/dev/null && pwd -P)
    DISPLAY_OUTPUT="${DISPLAY_OUTPUT/#$PARENT_PREFIX\//}"

    echo -e "  ${DIM}Version${RESET}   ${CYAN}${PACK_VERSION}${RESET}"
    echo -e "  ${DIM}Config${RESET}    ${CONFIGURATION}"
    echo -e "  ${DIM}Output${RESET}    ${DISPLAY_OUTPUT}"
    echo -e "  ${DIM}Versions${RESET}  $(basename "$VERSIONS_TARGET")"
else
    echo -e "  ${RED}Could not determine package version${RESET}"
fi

PACKAGES=($(ls "$OUTPUT_DIR"/*."$PACK_VERSION".nupkg 2>/dev/null | xargs -n1 basename | sed "s/\.$PACK_VERSION\.nupkg//"))
if [[ ${#PACKAGES[@]} -gt 0 ]]; then
    echo ""
    echo -e "  ${DIM}Packages${RESET}"
    for pkg in "${PACKAGES[@]}"; do
        echo "    $pkg"
    done
fi

if [[ ${#SAMPLE_RESULTS[@]} -gt 0 ]]; then
    echo ""
    echo -e "  ${DIM}Samples${RESET}"
    for result in "${SAMPLE_RESULTS[@]}"; do
        name="${result%%|*}"
        status="${result##*|}"
        if [[ "$status" == "OK" ]]; then
            echo -e "    ${name}  ${GREEN}${status}${RESET}"
        else
            echo -e "    ${name}  ${RED}${status}${RESET}"
        fi
    done
fi
echo ""
