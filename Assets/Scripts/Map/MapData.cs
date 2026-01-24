using UnityEngine;

[System.Serializable]
public class MapData
{
    public SceneLocationEnum scene;
    public int width;
    public int height;

    [SerializeField]
    public int[] map;

    public void Init(int width, int height)
    {
        this.width = width;
        this.height = height;
        map = new int[width * height];
    }

    public int Get(int x, int y)
    {
        return map[y * width + x];
    }

    public void Set(int x, int y, int value)
    {
        map[y * width + x] = value;
    }
}
