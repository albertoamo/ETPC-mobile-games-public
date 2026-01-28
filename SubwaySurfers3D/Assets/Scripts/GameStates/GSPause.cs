using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GSPause", menuName = "GameStates/GSPause", order = 1)]
public class GSPause: GameState
{
    public override void OnEnter()
    {
        Time.timeScale = 0.0f;
        UIPause pause = FindObjectOfType<UIPause>(true);
        pause.gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnGameplay();
        }
    }

    public override void OnExit()
    {
        Time.timeScale = 1.0f;
        UIPause pause = FindObjectOfType<UIPause>();
        pause.gameObject.SetActive(false);
    }
     
    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("spmap_mainmenu");
        GameStateManager.Instance.ChangeGameState(GameState.StateType.MAINMENU);
    }

    public void ReturnGameplay()
    {
        GameStateManager.Instance.ChangeGameState(StateType.GAMEPLAY);
    }
}
