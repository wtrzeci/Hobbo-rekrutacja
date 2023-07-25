using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : MonoBehaviour
{
    public static Action<StorageBuilding> AddToList;
    private GameResourcesList resourcesList;
    private void Awake()
    {
        AddToList?.Invoke(this);
        resourcesList = GetComponent<GameResourcesList>();
    }
    public void AddResources(GameResourceSO resourceSO)
    {
        resourcesList.Add(resourceSO, 1);
    }
}
