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
    [ContextMenu("�ѱ�")]
    void TurnOn()
    {
        GetComponent<Outline>().enabled = true;
    }

    [ContextMenu("����")]
    void TurnOff()
    {
        GetComponent<Outline>().enabled = false;
    }
}
