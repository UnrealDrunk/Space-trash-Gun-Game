using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public float rotation;
    public float minSpeed, maxSpeed;
    public GameObject asteroidExplosion;
    public GameObject playerExplosion;

    public GameObject downedAlly; // Сбитый летчик


    protected GameControllerScript gameControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody asteroid = GetComponent<Rigidbody>();
        asteroid.angularVelocity = Random.insideUnitSphere * rotation;
        asteroid.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);

        gameControllerScript = 
            GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();
    }

    //Столкневение с другим объектом
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GameBoundary" || other.tag == "Shield" || other.tag == "Ammo" || other.tag == "WingSupport") 
        {
            return;
        }
        if(other.tag == "Asteroid" && gameObject.transform.position.z > 10f)
        {
            return;
        }

        if(other.tag != "Player" && other.tag != "PlayerShield" && other.tag != "Boss")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag =="Player")
        {
            gameControllerScript.DecreaseLives();

        }

        if (other.tag == "PlayerShield")
        {
            Destroy(this.gameObject);
            gameControllerScript.LoseShields();
            gameControllerScript.LazerManfunction();
        }

     


        //Destroy(other.gameObject);

        Destroy(this.gameObject);

        Instantiate(asteroidExplosion, transform.position, Quaternion.identity);

        if(other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);

        }

        if(other.tag == "Lazer")
        {
            gameControllerScript.IncreaseScore(10);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        if(other.tag == "Ally")
        {
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            Instantiate(downedAlly, other.transform.position, Quaternion.identity);
        }


    }

    private void Update()
    {
        if (gameControllerScript.player.activeInHierarchy == false)
        {
            Destroy(this.gameObject);
        }
    }

}
