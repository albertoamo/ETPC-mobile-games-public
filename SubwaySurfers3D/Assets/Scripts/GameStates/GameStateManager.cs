using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    // GameState array
    public GameState[] gameStates;
    public GameState currentState;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    public void ChangeGameState(GameState.StateType type)
    {
        // Search through all gamestates listed
        // Exit current state
        // Enter new state

        // Search through all gamestates listed and return the target
        GameState targetState = GetState(type);

        // Exit current state, assign and start new state
        currentState.OnExit();
        currentState = targetState;
        currentState.OnEnter();
    }

    private GameState GetState(GameState.StateType type)
    {
        for (int i = 0; i < gameStates.Length; i++)
        {
            if(gameStates[i].type == type)
            {
                return gameStates[i];
            }
        }

        return null;
    }
}
