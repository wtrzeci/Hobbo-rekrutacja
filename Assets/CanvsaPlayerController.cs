using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvsaPlayerController : MonoBehaviour
{
    [SerializeField] private Canvas playerCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject == gameObject)
            {
                playerCanvas.gameObject.SetActive(true);
            }
            else
            {
                playerCanvas.gameObject.SetActive(false);
            }
        }
        else
        {
            playerCanvas.gameObject.SetActive(false);
        }
    }
}
