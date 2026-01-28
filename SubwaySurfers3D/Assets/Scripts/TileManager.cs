using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform[] tilePrefabs;
    public int tileCount = 15;

    private List<TileController> _tiles;
    private Vector3 spawnPosition = Vector3.zero;

    void Awake()
    {
        _tiles = new List<TileController>();

        AddTile(0);
        for (int i = 1; i < tileCount; i++)
        {
            AddTile();
        }
    }

    void Update() 
    {
        if (_tiles[0].pivotBack.position.z < Camera.main.transform.position.z)
        {
            RemoveTile();
            AddTile();
        }
    }

    private void RemoveTile()
    {
        Destroy(_tiles[0].gameObject);
        _tiles.RemoveAt(0);
    }

    private void AddTile()
    {
        int tileIndex = Random.Range(0, tilePrefabs.Length);
        AddTile(tileIndex);
    }

    private void AddTile(int tileIndex)
    {
        Transform currentTile = tilePrefabs[tileIndex];

        Transform tile = Instantiate(currentTile, transform);
        tile.transform.position = spawnPosition;

        TileController tileCtr = tile.GetComponent<TileController>();
        spawnPosition = tileCtr.pivotBack.position;
        _tiles.Add(tileCtr);
    }
}
