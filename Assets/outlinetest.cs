using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class outlinetest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("ÄÑ±â")]
    void TurnOn()
    {
        GetComponent<Outline>().enabled = true;
    }

    [ContextMenu("²ô±â")]
    void TurnOff()
    {
        GetComponent<Outline>().enabled = false;
    }
}
