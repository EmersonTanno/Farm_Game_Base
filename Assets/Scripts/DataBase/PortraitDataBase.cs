using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Portrait Data Base")]
public class PortraitDataBase : ScriptableObject
{
    public List<PortraitData> portraits = new List<PortraitData>();

    public Sprite GetPortrait(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (portraits == null)
        {
            Debug.LogError("Lista portraits está NULL no PortraitDataBase!");
            return null;
        }

        PortraitData portrait = portraits.Find(t => t.id.ToString() == id.ToString());

        if (portrait == null)
        {
            Debug.LogWarning($"Portrait ID não encontrado: {id}");
            return null;
        }

        return portrait.portrait;
    }
}
