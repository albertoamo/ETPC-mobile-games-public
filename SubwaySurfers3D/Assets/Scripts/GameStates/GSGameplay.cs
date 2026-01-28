using UnityEngine;

[CreateAssetMenu(fileName = "GameStateGameplay", menuName = "GameStates/GSGameplay", order = 1)]
public class GSGameplay: GameState
{
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStateManager.Instance.ChangeGameState(StateType.PAUSE);
        }
    }

    public override void OnExit()
    {

    }
}
