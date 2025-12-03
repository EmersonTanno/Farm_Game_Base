using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapController : MonoBehaviour
{
    public new TileMapRenderer renderer;

    private TileMap tileMap;

    void Start()
    {
        tileMap = new TileMap(20, 10, 1f, Vector3.zero);

        renderer.Init(tileMap);
    }
}
