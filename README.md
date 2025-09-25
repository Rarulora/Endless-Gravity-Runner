# Gravity Runner

An endless 2D runner built in **1 day** where gravity is your greatest tool. Sprint forward, flip between floor and ceiling only when grounded, and survive as long as possible. Speed **and** gravity intensity scale with your score, pushing your reflexes to the limit.

---

## ✨ Features
- **Gravity flip** (floor/ceiling) only when touching a surface
- **Score-based scaling**: movement speed and gravity magnitude increase as you score
- **Obstacle spawner** with object pooling and anchor-aware placement (Top/Bottom Center)
- **Camera follow**: smooth X-only tracking
- **Minimal state machine**: `MainMenu`, `Gameplay`, `Pause`, `GameOver`
- **JSON save/load** (WebGL-ready) for `HighScore` and `LastScore`

---

## 🎮 Controls
- **Sprint / Move**: Hold **Space** or **Right Mouse Button**, or **D**
- **Flip Gravity** (while grounded): **Q** / **W** / **S** / **Left Mouse Button**
- **Pause / Resume**: **Esc**

> Tip: Master short, precise flips—stay grounded just long enough to flip again.

---

## 🧩 Project Structure (scripts)
```
Assets/
  _.Scripts/
    GameManager.cs         # Enum-based minimal state machine + JSON save/load integration
    SaveSystem.cs          # Cross-platform JSON persistence (with WebGL sync)
    ScoreManager.cs        # Simple score/highscore API (singleton)
    MovementController.cs  # Auto walk + sprint + gravity flip (with score-based scaling)
    Obstacle.cs            # World-size aware placement (width/height) + anchor align
    ObstacleSpawner.cs     # Camera-X based spawn, pooling, despawn behind camera
    CameraController.cs    # Smooth X-only follow
    GameUI.cs              # Pause & GameOver panels, shows Score/HighScore
    MenuUI.cs              # Play button + HighScore/LastScore display
  Plugins/
    WebGL/
      WebGLSyncFS.jslib      # WebGL IndexedDB sync for JSON saves
```

---

## 💾 Save / Load (JSON, WebGL-ready)

This project avoids `PlayerPrefs` and saves scores to `save.json` under `Application.persistentDataPath`.  
On **WebGL**, we sync that virtual file system with IndexedDB via a tiny JS plugin.

### JS plugin (required for WebGL)
Create **`Assets/Plugins/WebGL/WebGLSyncFS.jslib`** with:

```javascript
mergeInto(LibraryManager.library, {
  GR_SyncFS_In: function () {        // IndexedDB -> MEM (on start)
    FS.syncfs(true, function (err) {
      if (err) console.error('GR_SyncFS_In error:', err);
    });
  },
  GR_SyncFS_Out: function () {       // MEM -> IndexedDB (after write)
    FS.syncfs(false, function (err) {
      if (err) console.error('GR_SyncFS_Out error:', err);
    });
  }
});
```

> **Import Settings** for the jslib: enable **WebGL** platform (you can uncheck "Any Platform").

### SaveSystem.cs usage
- Call `SaveSystem.SyncInIfWebGL()` **once at startup** (GameManager does this in `Awake`).
- Call `SaveSystem.Save(data)` to write, which internally **SyncOut** on WebGL.
- Data class example:
```csharp
[Serializable]
public class SaveData
{
    public int HighScore;
    public int LastScore;
    public string Version = "1.0.0";
    public long LastUpdatedUnix;
}
```

---

## 🛠️ Unity Version
- Built and tested on **Unity 6000.0.58f1** (Unity 6)

---

---

## 🔧 Setup & Hooks

### GameManager (enum-only FSM)
- `ToMenu()`, `StartRun()`, `Pause()`, `Resume()`, `GameOver()`, `RestartRun()`
- `OnStateChanged` event for UI panels to react
- Applies `Time.timeScale = 0` for menus; slightly slows during `GameOver` (0.01)

### ScoreManager (singleton)
- `Score` / `HighScore`
- `AddScore(int)`, `SetScore(int)`, `ResetRun()`, `SubmitRun()` (optional)
- GameManager uses **JSON SaveSystem** to persist `HighScore`/`LastScore`

### MovementController
- Walks by default; **sprint-only** movement (Space / RMB / D) applies a multiplier
- Gravity flip allowed only if grounded (ray checks up/down)
- Speed and gravity **scale with score** toward `maxSpeed` & `maxGravity`

### ObstacleSpawner + Obstacle
- Camera-X-driven spawning, pools 3 per prefab by default
- Aligns Y by **TopCenter** when above 0, **BottomCenter** when below 0
- Optional `snapToYEdges` pins to `yMin` or `yMax`
- Keeps track of last rightmost X to space obstacles correctly

### CameraController
- SmoothDamp on **X only**, with optional look-ahead and clamps

### UI
- **GameUI**: Pause/GameOver panels; shows `Score` and `HighScore` (from GameManager/ScoreManager)
- **MenuUI**: Play button; shows `HighScore` and `LastScore` from `GameManager.Data`

---

## ▶️ How to Play (local)
1. Open the project in Unity **6000.0.58f1**
2. Open **MainMenu** scene and press **Play**
3. Build WebGL:
   - File → Build Settings → WebGL → Build
   - If using threads, configure your hosting (itch.io **SharedArrayBuffer** toggle)

---


---

## 🗺️ Roadmap / Ideas
- Difficulty curves / patterns for obstacle gaps
- Power-ups (slow time, invert controls, ghost)

---

## 📝 License
MIT — see `LICENSE` (add one if you need).

---

## 🙌 Credits
- Code & design by **Rarulora** (1-day)
- Thanks to open-source & Unity docs

---

## 🔍 Troubleshooting
- **WebGL linker error `undefined symbol: GR_SyncFS_In/Out`**  
  Ensure `WebGLSyncFS.jslib` exists at `Assets/Plugins/WebGL/`, platform set to **WebGL**, names match exactly, and rebuild. Clear `Library/` if needed.
- **SAB/Threads errors on itch.io**  
  Enable the **SharedArrayBuffer support** option; avoid external assets without CORS/CORP.

Enjoy running on the ceiling 🌀
