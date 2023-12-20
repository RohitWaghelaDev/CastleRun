using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrickSpawner : MonoBehaviour
{
    public static BrickSpawner Instance;

    private MinMax xRange;
    private MinMax zRange;

    private IEnumerator spawnBricks;

    private Vector3 brickPosition = new Vector3(0,0,0);
    private float groundYPos = 0.3f;

    private const float spawnInterval = 1f;

    [SerializeField] private List<Vector3> vacantPositions= new List<Vector3>();
    [SerializeField]private List<Vector3> occupiedPositions= new List<Vector3>();

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = GetComponent<BrickSpawner>();
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameState;

       
    }

   

    private void Start()
    {
        xRange = Map.Instance.XRange;
        zRange = Map.Instance.ZRange;
        spawnBricks = SpawnBricks();
        FillVacantPositions();
        SpawnInitialBricks();


    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameState;
    }

    private IEnumerator SpawnBricks()
    {
        while (true)
        {
            SpawnBrick(BrickType.BlueBrick);
            SpawnBrick(BrickType.RedBrick,spawnAboveTheGround: true);
            SpawnBrick(BrickType.GreenBrick,spawnAboveTheGround: true);
            SpawnBrick(BrickType.YellowBrick,spawnAboveTheGround: true);


            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBrick(BrickType brickType,bool spawnAboveTheGround=true)
    {
        if (vacantPositions.Count<=0)
        {
           // Debug.Log("vacant position not available");
            return;
        }
        GameObject tempBrick = ObjectPool.Instance.GetBrick();
        tempBrick.transform.position = GetBrickPosition(spawnAboveTheGround);
        Brick b = tempBrick.GetComponent<Brick>();
        if (b!=null)
        {
            b.SetupBrick(brickType);
        }
    }

    private Vector3 GetBrickPosition(bool spawnAboveTheGround = true)
    {
       // int xPos = Random.Range(xRange.min,xRange.max);
        //int zPos = Random.Range(zRange.min,zRange.max);


        if (spawnAboveTheGround)
        {
            int i = Random.Range(0, vacantPositions.Count);
            // brickPosition = new Vector3(xPos, transform.position.y, zPos);
            brickPosition = vacantPositions[i];
            occupiedPositions.Add(brickPosition);
            vacantPositions.RemoveAt(i);
        }
        else
        {
            // brickPosition = new Vector3(xPos, groundYPos, zPos);
            int i = Random.Range(0, vacantPositions.Count);
            brickPosition = vacantPositions[i];
            occupiedPositions.Add(brickPosition);
            vacantPositions.RemoveAt(i);

        }

        return brickPosition;
    }

    private void GameManager_OnGameState(Gamestate gamestate)
    {
        switch (gamestate)
        {
            case Gamestate.LoadState:
                break;
            case Gamestate.RoundStart:
                
                StartCoroutine(spawnBricks);
                break;
            case Gamestate.RoundOverState:
                StopCoroutine(spawnBricks);
                break;
            case Gamestate.MetagameSate:
                break;
            case Gamestate.GameoverState:
                break;
            default:
                break;
        }
    }

    private void SpawnInitialBricks()
    {
        for (int i = 0; i < 50; i++)
        {
            SpawnBrick(BrickType.BlueBrick, spawnAboveTheGround: false);
            SpawnBrick(BrickType.RedBrick, spawnAboveTheGround: false);
            SpawnBrick(BrickType.GreenBrick, spawnAboveTheGround: false);
            SpawnBrick(BrickType.YellowBrick, spawnAboveTheGround: false);
        }
    }
    private void FillVacantPositions()
    {
        
        for (int i = Map.Instance.XRange.min; i < Map.Instance.XRange.max; i++)
        {
           
            
            for (int j = Map.Instance.ZRange.min; j < Map.Instance.ZRange.max; j++)
            {
                Vector3 vacantPos = new(i, groundYPos,j);
               // Debug.Log(vacantPos);
                if (!vacantPositions.Contains(vacantPos))
                {
                    vacantPositions.Add(vacantPos);
                }
                j += 5;
            }
            i += 5;
        }   
    }

    public void EmptyOccupiedPosition(Vector3 position)
    {
        if (occupiedPositions.Contains(position))
        {
            if (!vacantPositions.Contains(position))
            {
                vacantPositions.Add(position);
            }
            occupiedPositions.Remove(position);
        }
    }
}
