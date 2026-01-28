using UnityEngine;

public abstract class GameState : ScriptableObject
{
    public enum StateType
    {
        MAINMENU,
        PAUSE,
        GAMEPLAY,
        WIN,
        OVER
    }

    public StateType type;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
