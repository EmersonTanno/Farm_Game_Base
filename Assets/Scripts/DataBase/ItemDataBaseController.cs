using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ItemDataBaseController : MonoBehaviour
{
    public static ItemDataBaseController Instance;

    [SerializeField] private ItemDataBase itemDB;

    void Awake()
    {
        Instance = this;
    }

    public Item GetItemById(int id)
    {
        return itemDB.GetItem(id);
    }
}