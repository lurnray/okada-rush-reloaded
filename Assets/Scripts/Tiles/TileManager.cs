using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private List<GameObject> activeTiles = new List<GameObject>();
    public GameObject[] tilePrefabs;

    public float tileLength = 30;
    public int numberOfTiles = 3;
   // public int totalNumOfTiles = 5;

    public float zSpawn = 0;

    public Transform playerTransform;

    private int previousIndex;

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
           if (i == 0) 
           SpawnTile(0);
           else
           SpawnTile(Random.Range(0, tilePrefabs.Length));
        }

    }
    void Update()
    {
        if(playerTransform.position.z - 30 >= zSpawn - (numberOfTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteTile();
            
        }
            
    }

    public void SpawnTile(int tileIndex)
    {
        GameObject go = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);
        zSpawn += tileLength;
    }

    private void DeleteTile()
    {
        activeTiles[0].SetActive(false);
        activeTiles.RemoveAt(0);
        //PlayerManager.score += 3;
    }
}
