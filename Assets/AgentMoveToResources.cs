using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class AgentMoveToResources : MonoBehaviour
{
    [SerializeField] private Transform TestGoal;
    private List<ProductionBuilding> ProductionBuildings;
    private List<ExtractionBuilding> ExtractionBuildings;
    private List<StorageBuilding> StorageBuildings;
    [SerializeField] private GameResourceSO RawResource;
    [SerializeField] private GameResourceSO ProcessedResource;
    public static Action<GameResourceSO> OnResourceChange;
    private GameResourceSO _ownedResource;
    int cachedIndex = 0;
    private GameResourceSO OwnedResource
    {
        get => _ownedResource;
        set
        {
            _ownedResource = value;
            OnResourceChange?.Invoke(value);
        }
    }
    [SerializeField]
    GameResourcesList Resources;
    private NavMeshAgent Agent;
    private TargetType Target;
    private float stoppingDistance =0.9f;
    private float interactionDistance = 4;
    private Animator Animator;
    
    // Start is called before the first frame update
    void Awake()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        ProductionBuildings = new List<ProductionBuilding>();
        ExtractionBuildings = new List<ExtractionBuilding>();
        ExtractionBuilding.AddToList += AddToExtractionBuildings;
        ProductionBuilding.AddToList += AddToProductionBuildings;
        StorageBuilding.AddToList += AddToStorageBuildings;
        Agent.stoppingDistance = stoppingDistance;
    }

    void AddToStorageBuildings(StorageBuilding building)
    {
        if (StorageBuildings == null)
        {
            StorageBuildings = new List<StorageBuilding>();
        }
        StorageBuildings.Add(building);
    }
    void AddToProductionBuildings(ProductionBuilding building)
    {
        if (ProductionBuildings == null)
        {
            ProductionBuildings = new List<ProductionBuilding>();
        }
        ProductionBuildings.Add(building);
    }

    void AddToExtractionBuildings(ExtractionBuilding building)
    {
      
        if (ExtractionBuildings == null)
        {
            ExtractionBuildings = new List<ExtractionBuilding>();
        }
        ExtractionBuildings.Add(building);
        StartMovingToExtractionBuilding();
    }

    private void StartMovingToExtractionBuilding()
    {
        Target = TargetType.extractor;
        cachedIndex = UnityEngine.Random.Range(0, ExtractionBuildings.Count);
        Agent.destination = ExtractionBuildings[cachedIndex].transform.position;
    }

    private void StartMovingToProcessingPlant()
    {
        
        Target = TargetType.manufacture;
        cachedIndex = UnityEngine.Random.Range(0, ProductionBuildings.Count);
        Agent.destination = ProductionBuildings[cachedIndex].transform.position;
    }

    private void OnDestroy()
    {
        ExtractionBuilding.AddToList -= AddToExtractionBuildings;
        ProductionBuilding.AddToList -= AddToProductionBuildings;
        StorageBuilding.AddToList -= AddToStorageBuildings;
    }

    private void Update()
    {
        Animator.SetBool("isMoving",Agent.velocity.magnitude>0.1f);
        Animator.SetBool("hasBalast",!(OwnedResource is null));
        if (Agent.remainingDistance > interactionDistance)
        {
            return;
        }
        else Agent.velocity = Vector3.zero;
        if (Target == TargetType.extractor)
        {
            if (OwnedResource is null)
            {
                if (!ExtractionBuildings[cachedIndex].TryUseBuilding(RawResource, 1)) { return; }
                    OwnedResource = RawResource;
            }
            if (ProductionBuildings is not null && ProductionBuildings.Count >0)
                StartMovingToProcessingPlant();
            return;
        }

        if (Target == TargetType.manufacture)
        {
            ProductionBuildings[cachedIndex].AddResources(OwnedResource);
            if(StorageBuildings is not null && StorageBuildings.Count != 0)
            {
                if (ProductionBuildings[cachedIndex].TryUseBuilding(1))
                {
                    GetItemFromProduction();
                    
                    return;
                }
            }
            OwnedResource = null;
            StartMovingToExtractionBuilding();
            return;
        }

        if (Target == TargetType.storage)
        {
            StorageBuildings[cachedIndex].AddResources(OwnedResource);
            OwnedResource = null;
            StartMovingToExtractionBuilding();
            
        }
    }
    private void GetItemFromProduction()
    {
        OwnedResource = ProcessedResource;
        cachedIndex = UnityEngine.Random.Range(0, StorageBuildings.Count);
        Target = TargetType.storage;
        Agent.destination = StorageBuildings[cachedIndex].transform.position;
    }
}


public enum TargetType
{
    none,
    extractor,
    manufacture,
    storage
}