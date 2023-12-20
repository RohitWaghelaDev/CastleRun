using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gamestate : int {LoadState,RoundStart,RoundOverState,MetagameSate,GameoverState}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameLevels gameLevels;
    [SerializeField] private GameObject bluePlayer, YellowPlayer, redPlayer, greenPlayer;
    private Gamestate currentGameState;

    public delegate void GameStateChange(Gamestate gamestate);
    public static event GameStateChange OnGameStateChanged;

    private void Awake()
    {
        /*if (Instance != null && Instance != this)
        {
            Destroy(this);
        }*/
        if (Instance==null)
        {
            Instance = GetComponent<GameManager>();
        }

        if (!PlayerPrefs.HasKey(ConstStrings.CURRENTLEVEL))
        {
            PlayerPrefs.SetInt(ConstStrings.CURRENTLEVEL, 0);
        }
    }

    private void OnEnable()
    {
        SpawnLevel();
        SpawnPlayers();
    }

    private void Start()
    {
        
        ChangeGameState(Gamestate.LoadState);
        //Debug.LogError("changing state to  load state");
    }

    public void ChangeGameState(Gamestate state)
    {
        
        currentGameState = state;
        OnGameStateChanged?.Invoke(state);
    }


    private void SpawnLevel()
    {
        int currenrLevel = PlayerPrefs.GetInt(ConstStrings.CURRENTLEVEL);
        GameObject currentlevel = gameLevels.GetLevel(currenrLevel);
        Instantiate(currentlevel,Vector3.zero,Quaternion.identity);
    }

    private void SpawnPlayers()
    {
        SpawnPlayer(bluePlayer, BrickType.BlueBrick);
        SpawnPlayer(YellowPlayer,BrickType.YellowBrick);
        SpawnPlayer(redPlayer, BrickType.RedBrick);
        SpawnPlayer(greenPlayer,BrickType.GreenBrick);

    }

    private void SpawnPlayer(GameObject player,BrickType brickType)
    {
        Vector3 spawnPosition = Map.Instance.GetSpawnPosition(brickType);
        GameObject tempPlayer = Instantiate(player,spawnPosition,Quaternion.identity);
        if (brickType==BrickType.BlueBrick)
        {
            CameraController.Instance.SetPlayer(tempPlayer.transform);
        }
    }
}
