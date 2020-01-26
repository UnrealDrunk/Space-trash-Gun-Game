using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuidedLazerScript : MonoBehaviour
{
    public float speed;

    public GameObject asteroidExplosion;
    public GameObject playerExplosion;
    protected GameControllerScript gameControllerScript;
    protected GameObject player;

    public GameObject downedAlly; // Сбитый летчик




    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().velocity = Vector3.back * speed;
        //transform.position = transform.forward * speed;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        player = GameObject.FindGameObjectWithTag("Player");
     
     

        gameControllerScript =
            GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();

     


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Ammo" || other.tag == "WingSupport" || other.tag == "Shield")
        {
            return;
        }

        if (other.tag == "Player")
        {
            //Destroy(other.gameObject);
            Destroy(this.gameObject);
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);
            gameControllerScript.DecreaseLives();
        }

        if (other.tag == "Asteroid")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            Instantiate(asteroidExplosion, transform.position, Quaternion.identity);

        }

        if (other.tag == "PlayerShield")
        {
            gameControllerScript.LoseShields();
            Instantiate(asteroidExplosion, transform.position, Quaternion.identity);
            gameControllerScript.LazerManfunction();
            Destroy(this.gameObject);
        }

        if (other.tag == "Ally")
        {
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            Instantiate(downedAlly, other.transform.position, Quaternion.identity);
        }



    }


}
