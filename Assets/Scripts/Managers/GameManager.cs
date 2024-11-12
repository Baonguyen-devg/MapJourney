using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Setup,
    Playing,
    Finished,
}

public class GameManager : Singleton<GameManager>
{
    #region Game states
    [SerializeField] private GameState gameStatePresent = GameState.Setup;

    public GameState GameStatePresent => gameStatePresent;
    public void ChangeGameState(GameState gameState)
    {
        gameStatePresent = gameState;
        switch (gameStatePresent)
        {
            case GameState.Setup: break;
            case GameState.Playing: break;
            case GameState.Finished: break;
            default: break;
        }
    }
    #endregion
}
