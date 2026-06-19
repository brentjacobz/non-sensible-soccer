# Non-Sensible Soccer (WinForms Prototype)

A tiny Sensible Soccer-style prototype built with C# WinForms.

## Prerequisites (Windows)

1. Install .NET 8 SDK (x64):
   - https://dotnet.microsoft.com/en-us/download/dotnet/8.0
2. Optional but recommended: install Visual Studio 2022 Community with workload:
   - Desktop development with .NET
3. For VS Code debugging, install extensions:
   - C#
   - C# Dev Kit

## Verify .NET Is Installed

Open a new terminal and run:

```powershell
dotnet --info
```

If you still get "dotnet is not recognized", restart Windows (or sign out/in) after SDK install.

## Run In Visual Studio (Easiest)

1. Open [NonSensibleSoccer.sln](NonSensibleSoccer.sln).
2. Ensure configuration is Debug | Any CPU.
3. Press F5.

## Run In VS Code

1. Open this folder in VS Code.
2. Press F5.
3. Choose Launch NonSensibleSoccer.

This uses:
- Build task: [.vscode/tasks.json](.vscode/tasks.json)
- Launch profile: [.vscode/launch.json](.vscode/launch.json)

## Manual Build/Run (CLI)

From the workspace root:

```powershell
dotnet build NonSensibleSoccer.csproj
dotnet run --project NonSensibleSoccer.csproj
```

## Controls

- Move: WASD or Arrow keys
- Switch player: Q or Tab
- Pass: Space
- Shoot: Left Ctrl or Right Ctrl
- Restart after game over: Enter

## Project Layout

- Entry/form: [Program.cs](Program.cs), [MainForm.cs](MainForm.cs)
- Game world: [Game/GameWorld.cs](Game/GameWorld.cs)
- Models: [Game/Models/Player.cs](Game/Models/Player.cs), [Game/Models/Ball.cs](Game/Models/Ball.cs), [Game/Models/Team.cs](Game/Models/Team.cs), [Game/Models/MatchState.cs](Game/Models/MatchState.cs)
- Systems: [Game/Systems/InputController.cs](Game/Systems/InputController.cs), [Game/Systems/PhysicsGameplay.cs](Game/Systems/PhysicsGameplay.cs), [Game/Systems/CpuController.cs](Game/Systems/CpuController.cs), [Game/Systems/RulesSystem.cs](Game/Systems/RulesSystem.cs)
- Rendering: [Game/Rendering/GameRenderer.cs](Game/Rendering/GameRenderer.cs)
