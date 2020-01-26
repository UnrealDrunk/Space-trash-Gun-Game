using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLazerRScript : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(1,0,1) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
