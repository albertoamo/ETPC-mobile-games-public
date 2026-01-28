using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GSWin", menuName = "GameStates/GSWin", order = 1)]
public class GSWin : GameState
{
    public override void OnEnter()
    {
        Time.timeScale = 0.0f;
        UIWin pause = FindObjectOfType<UIWin>(true);
        pause.gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        Time.timeScale = 1.0f;
        UIWin pause = FindObjectOfType<UIWin>();
        pause.gameObject.SetActive(false);
    }
     
    public void ReloadScene()
    {
        SceneManager.LoadScene("SampleScene");
        GameStateManager.Instance.ChangeGameState(GameState.StateType.GAMEPLAY);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        GameStateManager.Instance.ChangeGameState(GameState.StateType.MAINMENU);
    }
}
