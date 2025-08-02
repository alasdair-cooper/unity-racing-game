using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [Header("Inputs")]
    public InputActionAsset InputActions;

    [Header("Screens")]
    public GameObject HudScreen;
    public GameObject RaceCompletedScreen;

    void Start()
    {
        Instance = this;
        SetScreen(GameScreen.Hud);
    }

    public void SetScreen(GameScreen screen)
    {
        switch (screen)
        {
            case GameScreen.Hud:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                InputActions.FindActionMap("Player").Enable();
                InputActions.FindActionMap("UI").Disable();
                HudScreen.SetActive(true);
                RaceCompletedScreen.SetActive(false);
                break;
            case GameScreen.RaceCompleted:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InputActions.FindActionMap("Player").Disable();
                InputActions.FindActionMap("UI").Enable();
                RaceCompletedScreen.SetActive(true);
                HudScreen.SetActive(false);
                break;
        }
    }
}

public enum GameScreen
{
    Hud,
    RaceCompleted
}
