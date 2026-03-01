// ── Bootstrap.cs ──────────────────────────────────────────────────────────────
// Place in: Assets/Scripts/Core/Bootstrap.cs
//
// HOW TO USE
// ────────────────────────────────────────────────────────────────────────────
//  1. Open an empty Unity scene.
//  2. Create ONE empty GameObject (name it anything, e.g. "Bootstrap").
//  3. Attach ONLY this script to it.
//  4. Press Play — everything else is generated at runtime.
//
// INITIALIZATION ORDER
// ────────────────────────────────────────────────────────────────────────────
//  LayoutConfig.Compute()     ← MUST be first (all others depend on it)
//  Camera + AudioListener
//  DOTween
//  EventSystem
//  PoolManager → ScoreManager → GridManager → UIManager
//  BlockSpawner → InputHandler → AudioManager → GameManager
// ────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using DG.Tweening;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        // 1. Layout MUST be computed before Camera or anything else reads Constants
        LayoutConfig.Compute();

        SetupCamera();
        SetupDOTween();
        SetupEventSystem();

        // ── Create manager GameObjects ─────────────────────────────────────
        var poolMgr   = Spawn<PoolManager>   ("PoolManager");
        var scoreMgr  = Spawn<ScoreManager>  ("ScoreManager");
        var gridMgr   = Spawn<GridManager>   ("GridManager");
        var uiMgr     = Spawn<UIManager>     ("UIManager");
        var spawner   = Spawn<BlockSpawner>  ("BlockSpawner");
        var input     = Spawn<InputHandler>  ("InputHandler");
        var audioMgr  = Spawn<AudioManager>  ("AudioManager");
        var gameMgr   = Spawn<GameManager>   ("GameManager");

        // ── Initialize in dependency order ─────────────────────────────────
        poolMgr  .Initialize();
        scoreMgr .Initialize();
        gridMgr  .Initialize();
        uiMgr    .Initialize();
        spawner  .Initialize();
        input    .Initialize();
        audioMgr .Initialize();   // builds procedural audio clips
        gameMgr  .Initialize();
    }

    // ── Camera ────────────────────────────────────────────────────────────────

    private void SetupCamera()
    {
        var cam = GameObject.FindObjectOfType<Camera>() ?? gameObject.AddComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = LayoutConfig.OrthoSize;
        cam.backgroundColor  = Constants.BgColor;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.tag              = "MainCamera";
        transform.position   = new Vector3(0f, 0f, -10f);

        if (FindAnyObjectByType<AudioListener>() == null)
            gameObject.AddComponent<AudioListener>();
    }

    // ── DOTween ───────────────────────────────────────────────────────────────

    private void SetupDOTween()
    {
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true,
                     logBehaviour: LogBehaviour.ErrorsOnly);
        DOTween.SetTweensCapacity(350, 120);
    }

    // ── EventSystem (New Input System) ───────────────────────────────────────

    private void SetupEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;
        var esGO = new GameObject("EventSystem");
        DontDestroyOnLoad(esGO);
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>();
    }

    // ── Factory ───────────────────────────────────────────────────────────────

    private static T Spawn<T>(string name) where T : MonoBehaviour
    {
        var go = new GameObject(name);
        DontDestroyOnLoad(go);
        return go.AddComponent<T>();
    }
}
