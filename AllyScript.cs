using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyScript : MonoBehaviour
{
    public float minSpeed, maxSpeed; // Максимальная и минимальные скорости движения корабля
    public float minStrifeSpeed, maxStrifeSpeed; // Максимальная и минимальная скорости бокового смещения корабля

    public float strifeDecisionDelay;// время задержки до первого маневра

    public float minShootDelay, maxShootDelay; // Минимальное и максимальное время задержки между выстрелами
    public float firstShootDelay; // Время задержки до первого выстрела

    public float armour; // Броня корабля (если == , то корабль погибает)

    Rigidbody ally;
    public GameObject Gun_1, Gun_2; // Орудия корабля
    public GameObject lazer; //лазеры (выстрелы, заряды)
    public GameObject player; // Корабль игрока

    protected GameControllerScript gameControllerScript; //подключаем скрипт гейм контроллера


    // Start is called before the first frame update
    void Start()
    {
        //Подключаем геймконтроллер скрипт
        gameControllerScript =
            GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();

        ally = GetComponent<Rigidbody>();
        ally.velocity = Vector3.forward * Random.Range(minSpeed, maxSpeed);

        if(gameControllerScript.GetIsStarted() == true)
        {
            // стрельба корабля
            InvokeRepeating("Shoot", firstShootDelay, Random.Range(minShootDelay, maxShootDelay));
            //меняем направление движения
            InvokeRepeating("StrifeShip", 2.0f, strifeDecisionDelay);
        }

       

        player = GameObject.FindGameObjectWithTag("Player");

      

    }

    

    /// <summary>
    /// Стрельба корабля
    /// </summary>
    private void Shoot()
    {
        if(gameControllerScript.menu.activeInHierarchy == false)
        {
            Instantiate(lazer, Gun_1.transform.position, Quaternion.identity);
            Instantiate(lazer, Gun_2.transform.position, Quaternion.identity);
        }


    }


    private void StrifeShip()
    {
       
        if(gameControllerScript.IsPlayerDown == false)
        {
            if (player.transform.position.x < transform.position.x)
            {
                ally.velocity = new Vector3(-1, 0, 1) * Random.Range(minSpeed, maxSpeed);
                ally.rotation = Quaternion.Euler(0, 0, 30);
            }
            else if (player.transform.position.x > transform.position.x)
            {
                ally.velocity = new Vector3(1, 0, 1) * Random.Range(minSpeed, maxSpeed);
                ally.rotation = Quaternion.Euler(0, 0, -30);
            }

        }
        else
        {
            ally.velocity = Vector3.forward * Random.Range(minSpeed, maxSpeed);

        }



    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo" || other.tag == "Shield" || other.tag == "WingSupport" || other.tag == "Ally" ||
            other.tag =="PlayerShield" || other.tag == "Player")
        {
            return;
        }
    }



}
