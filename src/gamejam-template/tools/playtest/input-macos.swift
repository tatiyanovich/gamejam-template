import AppKit
import CoreGraphics
import Foundation

let arguments = Array(CommandLine.arguments.dropFirst())
guard arguments.count >= 2, let process = pid_t(arguments[0]),
      let app = NSRunningApplication(processIdentifier: process),
      app.bundleIdentifier == "com.unity3d.UnityEditor5.x" else {
    print("Usage: input-macos UNITY_PID down:49 wait:0.1 up:49 (or click:X:Y)")
    exit(2)
}
guard CGPreflightPostEventAccess() else {
    print("Allow Accessibility for the terminal/application running this helper.")
    exit(2)
}
let keys: [CGKeyCode] = [0, 1, 2, 8, 11, 12, 15, 17, 18, 19, 20, 21, 32, 34, 35, 45, 46, 49, 53, 123, 124, 125, 126]
for argument in arguments.dropFirst() {
    guard NSWorkspace.shared.frontmostApplication?.processIdentifier == process else {
        for key in keys {
            CGEvent(keyboardEventSource: nil, virtualKey: key, keyDown: false)?.postToPid(process)
        }
        print("Stopped: Unity lost focus. No input will be sent to another application.")
        exit(3)
    }
    let parts = argument.split(separator: ":")
    if parts.count == 2, parts[0] == "wait", let seconds = Double(parts[1]), seconds >= 0, seconds <= 5 {
        Thread.sleep(forTimeInterval: seconds)
    } else if parts.count == 3, parts[0] == "click", let x = Double(parts[1]), let y = Double(parts[2]) {
        let point = CGPoint(x: x, y: y)
        CGEvent(mouseEventSource: nil, mouseType: .mouseMoved, mouseCursorPosition: point, mouseButton: .left)?.post(tap: .cghidEventTap)
        CGEvent(mouseEventSource: nil, mouseType: .leftMouseDown, mouseCursorPosition: point, mouseButton: .left)?.post(tap: .cghidEventTap)
        CGEvent(mouseEventSource: nil, mouseType: .leftMouseUp, mouseCursorPosition: point, mouseButton: .left)?.post(tap: .cghidEventTap)
    } else if parts.count == 2, ["down", "up"].contains(parts[0]), let key = CGKeyCode(parts[1]), keys.contains(key) {
        CGEvent(keyboardEventSource: nil, virtualKey: key, keyDown: parts[0] == "down")?.post(tap: .cghidEventTap)
    } else {
        print("Invalid action: \(argument)")
        exit(2)
    }
}
Thread.sleep(forTimeInterval: 0.05)
