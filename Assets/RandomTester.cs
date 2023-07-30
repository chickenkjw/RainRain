using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class RandomTester : MonoBehaviour
{
    public Text text;
    public void RandomCheck()
    {
        Debug.Log("random!");
        Random random = new Random(1);
        text.text = random.Next(0, 10).ToString();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
