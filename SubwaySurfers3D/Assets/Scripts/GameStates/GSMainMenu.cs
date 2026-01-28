using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameStateDummy", menuName = "GameStates/GSMainMenu", order = 1)]
public class GSMainMenu : GameState
{
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("spmap_tiling");
        SceneManager.sceneLoaded += SceneLoaded;
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameStateManager.Instance.ChangeGameState(GameState.StateType.GAMEPLAY);
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}
