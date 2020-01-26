using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public float rotationSpeed;


    private void Update()
    {
        Quaternion target = Quaternion.Euler(20, 10, 30);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
    }


}
