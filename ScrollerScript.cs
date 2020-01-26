using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerScript : MonoBehaviour
{
    public float speed;
    public float maxZshift;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        var shift =  Mathf.Repeat(Time.time * speed, maxZshift); // Зацикливание переменной
        transform.position = startPosition + Vector3.back * shift;
    }
}
