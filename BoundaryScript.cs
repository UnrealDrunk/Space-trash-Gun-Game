using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour
{
    // Выход объекта за границы коллайдера
    private void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }

}
