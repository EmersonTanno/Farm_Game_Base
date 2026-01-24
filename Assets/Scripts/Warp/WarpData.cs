using UnityEngine;

[System.Serializable]
public class WarpData
{
    public SceneLocationEnum scene;
    public Vector2Int fromGridPosition;
    public SceneLocationEnum toScene;
    public Vector2Int toGridPosition;
    public TransitionType transitionType;
}
