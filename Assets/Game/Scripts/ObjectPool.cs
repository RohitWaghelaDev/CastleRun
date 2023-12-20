using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public GameObject brick;

    public List<GameObject> brickPool;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<ObjectPool>();
        }
    }
    private void OnEnable()
    {
        CreatePool(brick, brickPool, 3000);

    }

    // Start is called before the first frame update
    void Start()
    {
    }



    private void CreatePool(GameObject obj, List<GameObject> objPoolList, int count)
    {

        for (int i = 0; i < count; i++)
        {
            GameObject tempObj = Instantiate(obj, Vector3.zero, Quaternion.identity);
            objPoolList.Add(tempObj);
            tempObj.SetActive(false);
        }
    }


    public GameObject GetBrick()
    {
        GameObject tempBrick = null;
        bool foundBullet = false;
        for (int i = 0; i < brickPool.Count; i++)
        {
            if (!brickPool[i].activeInHierarchy)
            {
                brickPool[i].SetActive(true);
                foundBullet = true;
                tempBrick= brickPool[i];
                break;
                
            }
        }

        if (!foundBullet)
        {
            GameObject tempObj = Instantiate(brick);
            brickPool.Add(tempObj);
        }
        return tempBrick;
    }
}
