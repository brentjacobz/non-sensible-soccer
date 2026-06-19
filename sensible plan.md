Yes, I know it.
Sensible Soccer is the classic fast-paced, top-down arcade football game with simple controls, quick matches, and lots of emphasis on ball feel and momentum rather than simulation realism.

## Plan: 2-Hour Sensible Soccer-Style Prototype (WinForms, Windows)

Goal: build a tiny playable prototype, not a full game.
Success = one human team vs CPU, passing/shooting, goals, scoring, restart loop.

**Steps**
1. Scope lock (10 min): fixed screen, 1 human team vs CPU, first to 3 goals, no menus, no assets.
2. Project setup (15 min): new WinForms app, enable double buffering, add a timer-driven game loop.
3. Core model (15 min): Player, Ball, Team, MatchState classes with position, velocity, and score.
4. Input and movement (20 min): move active player, switch to nearest teammate to ball, clamp speed.
5. Ball mechanics (25 min): pass, shoot, pickup radius, friction, boundaries, simple player-ball collisions.
6. AI and rules (20 min): CPU chases ball, basic defend/attack behavior, goal detection, kickoff reset.
7. Rendering and UI (10 min): draw pitch/goals/players/ball/score with GDI+, plus restart text.
8. Tuning pass (5 min): adjust speeds, ball friction, pass/shot power so it feels snappy.

**Out of scope for this 2-hour version**
1. Online multiplayer
2. Career/league systems
3. Animation/audio
4. Team management and tactics screens

**Verification checklist**
1. Controls respond instantly with no obvious stutter.
2. Passing and shooting are consistent and readable.
3. Goals increment score correctly.
4. Kickoff reset always returns game to valid state.
5. Three short matches are playable end-to-end without crashes.

I also tried to persist this plan in session memory, but that is currently unavailable because no workspace is open. If you open a folder/workspace, I can save and maintain the plan there and then break this into an even more detailed task-by-task execution checklist.