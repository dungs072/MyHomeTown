using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using static ManagerSingleton;
using static GameController;
using BaseEngine;
/// <summary>
/// GamePlay class to manage the game state.
/// This class is responsible for handling the game state and managing the game flow.
/// It only work in the game scene.
/// </summary>
public class GamePlay : MonoBehaviour
{
    private GamePlayState gameState = GamePlayState.NONE;
    public GamePlayState GameState => gameState;


    public void SwitchGameState(GamePlayState newState)
    {
        gameState = newState;
        Debug.Log($"Game state changed to: {gameState}");
    }
    void Start()
    {
        CreateMockAgents();
    }

    private void CreateMockAgents()
    {
        var agentManager = EmpireInstance.AgentManager;
        StartCoroutine(agentManager.SpawnAgents(35));
    }

}
