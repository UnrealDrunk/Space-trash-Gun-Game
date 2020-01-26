using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public float rotation;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody shield = GetComponent<Rigidbody>();
        shield.angularVelocity = Random.insideUnitSphere * rotation;
        shield.velocity = Vector3.back * speed;
    }



}
