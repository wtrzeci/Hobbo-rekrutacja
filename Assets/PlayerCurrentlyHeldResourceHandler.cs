using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCurrentlyHeldResourceHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string NoResourceString = "no resource :)";
    private void Awake()
    {
        text.text = NoResourceString;
        AgentMoveToResources.OnResourceChange += HandleResourceSwap;
    }
    private void HandleResourceSwap(GameResourceSO resourceSO)
    {
        if (resourceSO != null)
            text.text = resourceSO.resourceName;
        else text.text = NoResourceString;
    }
    private void OnDestroy()
    {
        AgentMoveToResources.OnResourceChange -= HandleResourceSwap;
    }
}
