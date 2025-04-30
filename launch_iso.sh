#!/bin/bash

ISO_PATH="$1"
VNC_PORT=8      # QEMU will listen on localhost:5901
NOVNC_PORT=6088   # noVNC will serve on http://localhost:6080

# Check for required files
if [ ! -f "$ISO_PATH" ]; then
    echo "Error: '$ISO_PATH' not found!"
    exit 1
fi

# Clone noVNC and websockify if missing
if [ ! -d "noVNC" ]; then
    echo "[+] Cloning noVNC..."
    git clone https://github.com/novnc/noVNC.git
fi

if [ ! -d "noVNC/utils/websockify" ]; then
    echo "[+] Cloning websockify into noVNC/utils/..."
    git clone https://github.com/novnc/websockify.git noVNC/utils/websockify
fi

# Build ISO if not already built
PROJECT_DIR="$(dirname "$ISO_PATH")" | awk -F'/bin/cosmos'
dotnet clean $PROJECT_DIR
dotnet build $PROJECT_DIR


# Start QEMU in background with VNC output
echo "[+] Starting QEMU with VNC output..."
qemu-system-x86_64 \
  -cdrom "$ISO_PATH" \
  -m 512 \
  -boot d \
  -vnc :$VNC_PORT \
  -serial mon:stdio \
  &

QEMU_PID=$!

# Give QEMU a second to start
sleep 2

# Start noVNC WebSocket proxy
echo "[+] Starting noVNC WebSocket proxy on port $NOVNC_PORT..."
cd noVNC
./utils/novnc_proxy --vnc localhost:$((5900 + VNC_PORT)) &

NOVNC_PID=$!

# Give noVNC a second to start
sleep 2

# Open in browser
echo "[+] Opening browser to view VM..."
xdg-open "$NOVNC_PORT-kumja1-raptoros-r5unk8tlm4.app.codeanywhere.com/vnc.html" >/dev/null 2>&1 &

# Wait for QEMU to close
wait $QEMU_PID

# Cleanup: kill noVNC after QEMU exits
kill $NOVNC_PID 2>/dev/null
