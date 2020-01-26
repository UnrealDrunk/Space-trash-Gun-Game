using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody ship;
    public float speed;
    public float strife;
    public float tilt;
    public float xMin, xMax;
    public float zMin, zMax;

    public float lazerDelay; // задержка между выстрелами
    public float smallLazerDelay;
    private float nextShot; // время слудующего выстрела

    public GameObject statusSphere;

    public GameObject lazerGun;
    public GameObject lazerGun2;
    public GameObject lazerGun3;
    public GameObject smallLazerRight;
    public GameObject smallLazerLeft;

    public GameObject lazerShot;
    public GameObject rightShot;
    public GameObject leftShot;

    public int ammoLargeLazer; //боекомплект (количество снарядов большого лазера)
    public int ammoSmallLazer; // боекомплект (количество снарядов малого лазера)

    public int maxAmmoLR;
    public int maxAmmoSL;

    public float rechargeLaser; // Перезарядка лазера

    protected GameControllerScript gameControllerScript;


    // Start is called before the first frame update 
    void Start()
    {
        ship = GetComponent<Rigidbody>();
        gameControllerScript =
            GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();
        gameObject.SetActive(true);

        maxAmmoLR = ammoLargeLazer;
        maxAmmoSL = ammoSmallLazer;

        gameControllerScript.SetLargeLaserTxt(ammoLargeLazer);
        gameControllerScript.SetSmallLaserTxt(ammoSmallLazer);

    }

    // Update is called once per frame
    void Update()
    {
        // 0. Пока игра не начата, ничего неделать (код ниже не исполняется)
        if (!gameControllerScript.GetIsStarted())
        {
            return;
        }


        //1. Узнать куда хочет полететь игрок
        var moveHorizontal = Input.GetAxis("Horizontal"); // Куда хочет лететь игрок по горизонтали, -1...+1
        var moveVertical = Input.GetAxis("Vertical");

        //2. Полететь туда
        ship.velocity = new Vector3(moveHorizontal * strife, 0, moveVertical * speed);

        //3. Наклоняемся
        ship.rotation = Quaternion.Euler(moveVertical * tilt / 2, 0, -moveHorizontal * tilt); // костыль по оси Х(деление на 2), чтобы носом сильно не клевал, для красоты

        //4. Сковываем движения границами карты

        var xPosition = Mathf.Clamp(ship.position.x, xMin, xMax);
        var zPosition = Mathf.Clamp(ship.position.z, zMin, zMax);
        ship.position = new Vector3(xPosition, 0, zPosition);


        //5. Стреляем
        if (Input.GetButton("Fire1") && Time.time > nextShot && gameControllerScript.IsPaused == false)
        {
            if (ammoLargeLazer > 0)
            {
                if (lazerGun.activeInHierarchy == true)
                {
                    Instantiate(lazerShot, lazerGun.transform.position, Quaternion.identity);

                }

                if (lazerGun2.activeInHierarchy == true)
                {
                    Instantiate(lazerShot, lazerGun2.transform.position, Quaternion.identity);

                }

                if (lazerGun3.activeInHierarchy == true)
                {
                    Instantiate(lazerShot, lazerGun3.transform.position, Quaternion.identity);

                }
            }


            ReduceAmmoLargeLaser();


            //Instantiate(lazerShot, lazerGun2.transform.position, Quaternion.identity);
            //Instantiate(lazerShot, lazerGun3.transform.position, Quaternion.identity);
            nextShot = Time.time + lazerDelay;
            Debug.Log("Large laser ammo " + ammoLargeLazer);
        }

        if (ammoLargeLazer < 1)
            StartCoroutine(ChargeLargeLaser());



        if (Input.GetButton("Fire2") && Time.time > nextShot)
        {
            if (ammoSmallLazer > 0)
            {
                Instantiate(rightShot, smallLazerRight.transform.position, Quaternion.Euler(0, 45, 0));
                Instantiate(leftShot, smallLazerLeft.transform.position, Quaternion.Euler(0, -45, 0));
            }

            ReduceAmmoSmallLaser();


            nextShot = Time.time + smallLazerDelay;
            Debug.Log("Small laser ammo " + ammoSmallLazer);
        }


        if (ammoSmallLazer < 1)
            StartCoroutine(ChargeSmallLaser());

        //восстановление боекомплекта при рестарте игры
        gameControllerScript.startButton.onClick.AddListener(delegate
        {
            ammoSmallLazer = maxAmmoSL;
            ammoLargeLazer = maxAmmoLR;
            gameControllerScript.largeLaserTxt.text = "Large Laser " + ammoLargeLazer;
            gameControllerScript.largeLaserTxt.color = Color.white;
            gameControllerScript.smallLaserTxt.text = "Small Laser " + ammoSmallLazer;
            gameControllerScript.smallLaserTxt.color = Color.white;

            foreach (GameObject lazer in gameControllerScript.largeLazers)
            {
                lazer.SetActive(true);
            }
            gameControllerScript.wpnStatusTxt.text = "All weapons in charge";
            gameControllerScript.wpnStatusTxt.color = Color.white;
        });

    }

    private void OnTriggerEnter(Collider other)
    {
        // столконовение с дополнительным источником щита
        if (other.tag == "Shield")
        {
            gameControllerScript.RiseShields();
            Destroy(other.gameObject);
            StartCoroutine(ShowShieldsRiseStatus());
        }

        // Столкновение с дополнительным источником снарядов для главного и вспосомгательного калибров
        if (other.tag == "Ammo")
        {
            Destroy(other.gameObject);
            ammoLargeLazer += 10;
            gameControllerScript.largeLaserTxt.text = "Large Laser " + ammoLargeLazer;
            ammoSmallLazer += 10;
            gameControllerScript.smallLaserTxt.text = "Small Laser " + ammoSmallLazer;
            StartCoroutine(StatusSphereLaunch());

        }
        // Столкновение с бонусом Wing Support

        if (other.tag == "WingSupport")
        {
            Destroy(other.gameObject);
            if (gameControllerScript.ShowWingSupportCount() != 5 && gameControllerScript.ShowWingSupportCount() < 6)
            {
                gameControllerScript.AddWingSupport();
                StartCoroutine(AddWingSupportText());
            } else if (gameControllerScript.ShowWingSupportCount() == 5)
            {
                StartCoroutine(WingSupportIsFull());
                gameControllerScript.IncreaseScore(75);

            }

        }

    }

    /// <summary>
    /// Сокращение зарядов большого лазера
    /// </summary>
    private void ReduceAmmoLargeLaser()
    {
        if (ammoLargeLazer > 0)
        {
            ammoLargeLazer--;
            gameControllerScript.SetLargeLaserTxt(ammoLargeLazer);

        }
    }

    /// <summary>
    /// Сокращение зарядов малого лазера
    /// </summary>
    private void ReduceAmmoSmallLaser()
    {
        if (ammoSmallLazer > 0)
        {
            ammoSmallLazer--;
            gameControllerScript.SetSmallLaserTxt(ammoSmallLazer);
        }
    }

    /// <summary>
    /// Восстановление заряда большого лазера
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargeLargeLaser()
    {
        yield return new WaitForSeconds(rechargeLaser);

        ammoLargeLazer = maxAmmoLR;
        gameControllerScript.SetLargeLaserTxt(ammoLargeLazer);

        Debug.Log("Laser Charged" + " " + ammoLargeLazer);
    }

    /// <summary>
    /// Восстановление заряда малого лазера
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargeSmallLaser()
    {
        yield return new WaitForSeconds(rechargeLaser);
        ammoSmallLazer = maxAmmoSL;
        gameControllerScript.SetSmallLaserTxt(ammoSmallLazer);
        Debug.Log("Laser Charged" + " " + ammoSmallLazer);


    }

    /// <summary>
    /// Графическая обработка получения пауэр апов
    /// </summary>
    /// <returns></returns>
    IEnumerator StatusSphereLaunch()
    {
        statusSphere.SetActive(true);
        gameControllerScript.getReadyTxt.text = "ADDITIONAL AMMO!";
        yield return new WaitForSeconds(1.2f);
        statusSphere.SetActive(false);
        gameControllerScript.getReadyTxt.text = "";
    }

    /// <summary>
    /// Отображение на экране увеличения силы щитов
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowShieldsRiseStatus()
    {
        if(gameControllerScript.ShowShieldPower() < 4)
        {
            gameControllerScript.getReadyTxt.text = "SHIELD POWER UP!";
        }
        else
        {
            gameControllerScript.getReadyTxt.text = "ADDITIONAL LIFE COLLECTED!";
        }

        yield return new WaitForSeconds(1.2f);
        gameControllerScript.getReadyTxt.text = "";
    }

    /// <summary>
    /// Отображение сообщения о взятии бонуса Wing Support
    /// </summary>
    /// <returns></returns>
    IEnumerator AddWingSupportText()
    {
        statusSphere.SetActive(true);
        gameControllerScript.getReadyTxt.text = "Initiating Wing Support fighter preparation!";
        yield return new WaitForSeconds(1.5f);
        statusSphere.SetActive(false);
        gameControllerScript.getReadyTxt.text = "";

    }

    /// <summary>
    /// Сообщение о полной готовности истребительного крыла
    /// </summary>
    /// <returns></returns>
    IEnumerator WingSupportIsFull()
    {
        gameControllerScript.getReadyTxt.text = "All Wing Support Fighters are Ready!";
        yield return new WaitForSeconds(1.5f);
        gameControllerScript.getReadyTxt.text = "";

    }


}
