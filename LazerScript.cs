using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerScript : MonoBehaviour
{
    public float speed;

    Rigidbody lazer;

    public bool IsShot = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().velocity = Vector3.forward*speed;
        lazer = GetComponent<Rigidbody>();
        lazer.velocity = Vector3.forward * speed;

        //Блок кода на случай, если захочется позаниматься стрельбой с изменением угла (не только вперед)

        //if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        //{
        //    lazer.velocity = new Vector3(-1.5f, 0, 1) * speed / 1.5f;

        //}
        //else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        //{
        //    lazer.velocity = new Vector3(1.5f, 0, 1) * speed / 1.5f;
        //}
        //else
        //{
        //    lazer.velocity = Vector3.forward * speed;
        //}


    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Shield" || other.tag == "Ammo" || other.tag == "Player" || other.tag == "Ally" ||
            other.tag == "PlayerShield")
        {
            return;
        }
    }


}
