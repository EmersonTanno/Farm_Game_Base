using UnityEngine;

public class ShopObject : MonoBehaviour
{
    [SerializeField] public string shopID;
    [SerializeField] public int ownerNpcID;
    [SerializeField] public Vector2Int relativeNPCPosition;
    [SerializeField] public Vector2Int relativePlayerPosition;

    public ShopObject CheckShopAvalible(Vector3 playerPos)
    {
        Grid<WorldTileData> npcGrid = TileMapController.Instance.GetGrid().GetGrid();
        
        if(npcGrid.GetGridObject(transform.position + new Vector3(relativeNPCPosition.x, relativeNPCPosition.y, 0)).npcId == ownerNpcID && transform.position + new Vector3(relativePlayerPosition.x, relativePlayerPosition.y, 0) == new Vector3Int((int)playerPos.x, (int)playerPos.y))
        {
            return this;
        }
        else
        {
            return null;
        }
    }

    public virtual void OpenNPCShop()
    {
        DialogueManager.Instance.SetDialogue(ownerNpcID, shopID);
    }
}