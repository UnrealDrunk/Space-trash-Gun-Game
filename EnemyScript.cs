using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float minSpeed, maxSpeed;
    public GameObject asteroidExplosion;
    public GameObject playerExplosion;

    public GameObject lazerGun_1, lazerGun_2;

    public GameObject lazerShot;
    //public float nextShot;

    private Rigidbody enemy;

    private GameObject player;
    public GameObject downedAlly; // Сбитый летчик



    protected GameControllerScript gameControllerScript;

    public float posMinX, posMaxX; // ограничения полетов по оси X
    public float minStrifeDelay, maxStrifeDelay; //Минимальная и максимальная задержки вызова страйфа противника



    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Rigidbody>();
       
        enemy.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);

        InvokeRepeating("Shot", 2f, Random.Range(minStrifeDelay, maxStrifeDelay)); // Начинаем стрелять через две секунды

        gameControllerScript =
       GameObject.FindGameObjectWithTag("GameController").
       GetComponent<GameControllerScript>();

        player = GameObject.FindGameObjectWithTag("Player");

        InvokeRepeating("StrifePosition", 1.5f, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GameBoundary" || other.tag == "EnemyLazer" ||
            other.tag =="Shield" || other.tag == "Ammo" || other.tag == "WingSupport")
        {
            return;
        }
        if (other.tag == "Asteroid" && gameObject.transform.position.z > 10f)
        {
            return;
        }

        if (other.tag != "Player" && other.tag !="PlayerShield")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            gameControllerScript.DecreaseLives();


        }
        else if (other.tag == "PlayerShield")
        {
            gameControllerScript.LoseShields();
            gameControllerScript.LazerManfunction();
            Destroy(this.gameObject);
        }


        if (other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);
        }

        if (other.tag == "Lazer")
        {
            gameControllerScript.IncreaseScore(15);

        }


        if (other.tag == "Ally")
        {
            Instantiate(playerExplosion, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            Instantiate(downedAlly, other.transform.position, Quaternion.identity);
        }


        Destroy(this.gameObject);
        Instantiate(asteroidExplosion, transform.position, Quaternion.identity);


       


    }

    private void Update()
    {
        
        //if(Time.time > nextShot)
        //{
        //    Instantiate(lazerShot, lazerGun.transform.position, Quaternion.Euler(0,180,0));
        //    nextShot = Time.time + nextShot;
        //}

      
        if(gameControllerScript.player.activeInHierarchy == false)
        {
            Destroy(this.gameObject);
        }

        
    }

    /// <summary>
    /// Выстрел из орудий противника
    /// </summary>
    private void Shot()
    {
        Instantiate(lazerShot, lazerGun_1.transform.position, Quaternion.Euler(0, 180, 0));
        //если у корабля предусмотрена вторая пушка, то стрелять
        if (lazerGun_2.activeInHierarchy == true)
        {
            Instantiate(lazerShot, lazerGun_2.transform.position, Quaternion.Euler(0, 180, 0));

        }

    }

    /// <summary>
    /// Смещение позиции противника в зависимости от передвижений игрока
    /// </summary>
    private void StrifePosition()
    {
        float decisionPoint = transform.position.z - player.transform.position.z;// точка принятия решения о маневре

        //если корабль противника достаточно далеко от игрока, то он может принять решение о маневре
        if(decisionPoint > 10)
        {

            if (transform.position.x > player.transform.position.x || transform.position.x >= posMaxX)
            {
                enemy.velocity = new Vector3(-1, 0, -1) * Random.Range(minSpeed, maxSpeed);
                enemy.rotation = Quaternion.Euler(0, 180, -30);
            }
            else if (transform.position.x < player.transform.position.x || transform.position.x <= posMinX)
            {
                enemy.velocity = new Vector3(1, 0, -1) * Random.Range(minSpeed, maxSpeed);
                enemy.rotation = Quaternion.Euler(0, 180, +30);

            }
            else
            {
                enemy.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);

            }
        }
        else
        {
            enemy.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);
        }


        //var xPosition = Mathf.Clamp(enemy.position.x, posMinX, posMaxX);
        //var zPosition = Mathf.Clamp(enemy.position.z, -100, +100); // TO:DO переделать логику принятия решения на контроль левой и правой границы
        //enemy.position = new Vector3(xPosition, 0, zPosition);

    }


}
