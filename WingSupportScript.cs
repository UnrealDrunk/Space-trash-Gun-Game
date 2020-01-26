using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingSupportScript : MonoBehaviour
{
    public float minSpeed, maxSpeed; // Максимальная и минимальные скорости движения корабля
    public float minStrifeSpeed, maxStrifeSpeed; // Максимальная и минимальная скорости бокового смещения корабля

    public float strifeDecisionDelay;// время задержки до первого маневра

    public float minShootDelay, maxShootDelay; // Минимальное и максимальное время задержки между выстрелами
    public float firstShootDelay; // Время задержки до первого выстрела

    public float leftXBorder, rightXBorder; // Левая и правая грагица экрана, чтобы корабли не вылетали за экран

    Rigidbody ally;
    public GameObject Gun_1, Gun_2; // Орудия корабля
    public GameObject lazer; //лазеры (выстрелы, заряды)

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

        if (gameControllerScript.GetIsStarted() == true)
        {
            // стрельба корабля
            InvokeRepeating("Shoot", firstShootDelay, Random.Range(minShootDelay, maxShootDelay));
            //меняем направление движения
            InvokeRepeating("StrifeShip", 2.0f, Random.Range(strifeDecisionDelay, strifeDecisionDelay+1.5f));
        }

    }

    private void Update()
    {
        // Если корабль пытается покинуть пределы экрана
        if (transform.position.x >= rightXBorder)
        {
            ally.velocity = new Vector3(-1, 0, 1) * Random.Range(minSpeed, maxSpeed);
            ally.rotation = Quaternion.Euler(0, 0, 30);
        }

        if (transform.position.x <= leftXBorder)
        {
            ally.velocity = new Vector3(1, 0, 1) * Random.Range(minSpeed, maxSpeed);
            ally.rotation = Quaternion.Euler(0, 0, -30);
        }
        //

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

    /// <summary>
    /// метод бокового смещения корабля
    /// </summary>
    private void StrifeShip()
    {
        int strifeIndex = Random.Range(1,3);

        if (strifeIndex ==1)
        {
            ally.velocity = new Vector3(-1, 0, 1) * Random.Range(minSpeed, maxSpeed);
            ally.rotation = Quaternion.Euler(0, 0, 30);
        }
        else if (strifeIndex ==2)
        {
            ally.velocity = new Vector3(1, 0, 1) * Random.Range(minSpeed, maxSpeed);
            ally.rotation = Quaternion.Euler(0, 0, -30);
        }
        else
        {
            ally.velocity = Vector3.forward * Random.Range(minSpeed, maxSpeed);
        }


    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ammo" || other.tag == "Shield" || other.tag == "WingSupport" || other.tag == "Ally" ||
            other.tag == "PlayerShield" || other.tag == "Player")
        {
            return;
        }
    }




}
