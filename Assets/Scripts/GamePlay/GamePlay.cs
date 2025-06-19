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
        SwitchGameState(GamePlayState.PLAYING);
    }

    void Update()
    {
        if (gameState == GamePlayState.NONE) return;
        if (gameState == GamePlayState.PLAYING)
        {
            UpdateGamePlaying();
        }
    }
    private void UpdateGamePlaying()
    {
        var agentManager = EmpireInstance.AgentManager;
        int shouldSpawnAgentCount = UnityEngine.Random.Range(1, 100);
        bool shouldSpawnAgent = agentManager.IsAllAgentsLessThan(shouldSpawnAgentCount);
        if (!shouldSpawnAgent) return;
        SpawnAgents();
    }
    private void SpawnAgents()
    {
        var agentManager = EmpireInstance.AgentManager;
        int agentCount = UnityEngine.Random.Range(1, 25);
        agentManager.SpawnAgents(AgentType.CUSTOMER, agentCount);

    }


}
