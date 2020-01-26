using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public float minSpeed, maxSpeed; // минимальная и максимальная скорость движения корабля
    public float leftXBorder, rightXBorder; // границы карты, за которые нельзя выходить
    private float shipRotateSpeed = 3.0f;
    public GameObject straightLazerGun1, straightLazerGun2; // неуправляемые лазерные пушки + управляемая
    public GameObject guidedGun1; // пушка с корректированным прицелом
    public GameObject lazerShot, guidedLazerShot; // префаб лазерного выстрела и направленного лазерного выстрела

    public GameObject asteroidExplosion; // префаб взрыва астероида (малый взрыв)
    public GameObject playerExplosion; // префаб большого взрыва
    private GameObject player; //игрок
    private Transform transformPlayer; // положение в пространстве игрока
    private Transform leftBossMarker, rightBossMarker; //маркеры, на которые ориентируется бос для поворота по оси Y


    public float turn90BorderZ; // Положение в пространстве по оси Z, где необходимо провести поворот на 90
    public float hullPower; // броня босса, если 0, то босс уничтожается

    protected GameControllerScript gameControllerScript;

    private Rigidbody boss;

    private bool IsBossAchivedAttackPoint; //Босс вышел на рубеж боя?
    private bool CanTurn; // кораблю разрешается поворачивать?
    private bool IsMoveLeft; //корабль летит на лево?
    private bool IsStrifeLeft; //корабль кренится влево?
    private bool IsAcceptedToShoot; // разрешено стрелять?

    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponent<Rigidbody>();

        //направляем движение коарбля после его появления
        boss.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);

        gameControllerScript =
          GameObject.FindGameObjectWithTag("GameController").
          GetComponent<GameControllerScript>();

        //выводим на экран мощность брони босса
        gameControllerScript.bossHullTxt.text = "Boss Hull: " + hullPower;

        //доступ к игроку
        player = GameObject.FindGameObjectWithTag("Player");

        //доступ к компоненту Transform игрока
        transformPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        //стрельба из орудий корабля неуправляемые

        InvokeRepeating("Shoot", 3.0f, Random.Range(1.5f, 2.0f));

        // стрельба из орудий корабля управляемые

        InvokeRepeating("GuidedShoot", 2.8f, Random.Range(1.1f, 1.7f));

        //босс не вышел на рубеж атаки
        IsBossAchivedAttackPoint = false;

        InvokeRepeating("TractionControl", 1.0f, Random.Range(3.5f, 4.5f));

        //обнаружение маркеров ориентира для левого и правого поворота по оси Y
        leftBossMarker = GameObject.FindGameObjectWithTag("LeftBossMarker").GetComponent<Transform>();
        rightBossMarker = GameObject.FindGameObjectWithTag("RightBossMarker").GetComponent<Transform>();

        //даем возможность кораблю поворачивать, т.к. корабль изначально не выходит за границы карты
        CanTurn = true;

        //Задаем изначальный крен корабля налево для красоты движения (не принципиально для логики, можно и направо задать)
        IsStrifeLeft = true;

        //заставляем корабль качаться на волнах для красоты
        InvokeRepeating("ShipWaving", 1.0f, Random.Range(2.5f, 2.9f));

        //разрешаем короаблю вести огонь
        IsAcceptedToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z <= turn90BorderZ)
        {
            IsBossAchivedAttackPoint = true;
            // добавить красивый эффект при переходе
        }

        // Если корабль пытается покинуть пределы экрана
        if (transform.position.x >= rightXBorder)
        {
            boss.velocity = new Vector3(-1, 0, 0) * Random.Range(minSpeed, maxSpeed);
            StartCoroutine(ICanTurn());
            IsMoveLeft = true;
            //плавные повороты
        }

        if (transform.position.x <= leftXBorder)
        {
            boss.velocity = new Vector3(1, 0, 0) * Random.Range(minSpeed, maxSpeed);

            StartCoroutine(ICanTurn());
            IsMoveLeft = false;
            // менее частые повороты
        }

        //плавный поворот корабля по направлению его движения

        if (IsBossAchivedAttackPoint)
        {
            if (IsMoveLeft)
            {
                Vector3 direction = leftBossMarker.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, shipRotateSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 direction = rightBossMarker.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, shipRotateSpeed * Time.deltaTime);
            }
        }


        //убираем геймобджект корабля в случае нажатия кнопки рестарт
        gameControllerScript.startButton.onClick.AddListener(delegate
        {
            if (gameObject.activeInHierarchy == true)
            {
                Destroy(gameObject);
            }
        });

        //третья пушка с управляемым выстрелом отслеживает движения игрока

        if (player.activeInHierarchy == true)
        {
            guidedGun1.transform.LookAt(player.transform.position);

        }


    }

    /// <summary>
    /// Логика перемещения босса в пространстве
    /// </summary>
    private void TractionControl()
    {
        if (IsBossAchivedAttackPoint)
        {


            boss.position = new Vector3(transform.position.x, transform.position.y, turn90BorderZ);
            int moveIndex = Random.Range(1, 3); // Здесь продолжить и смотреть на WingSupport
            if (moveIndex == 1 && CanTurn)
            {
                IsMoveLeft = true;
                boss.velocity = new Vector3(-1, 0, 0) * Random.Range(minSpeed, maxSpeed);


            }
            else if (moveIndex == 2 && CanTurn)
            {
                IsMoveLeft = false;
                boss.velocity = new Vector3(1, 0, 0) * Random.Range(minSpeed, maxSpeed);


            }
            else
            {
                return;
            }
        }
        else
        {
            boss.velocity = Vector3.back * Random.Range(minSpeed, maxSpeed);

        }




    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lazer" || other.tag == "Ally")
        {
            Instantiate(asteroidExplosion, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            DecreaseBossHull();
            Instantiate(asteroidExplosion, other.transform.position, Quaternion.identity);

        }

        if (other.tag == "PlayerShield")
        {
            gameControllerScript.LoseShields();
            gameControllerScript.LazerManfunction();
            DecreaseBossHull();
            Instantiate(asteroidExplosion, other.transform.position, Quaternion.identity);

        }

        if (other.tag == "Player")
        {
            gameControllerScript.DecreaseLives();
            DecreaseBossHull();
            Instantiate(asteroidExplosion, other.transform.position, Quaternion.identity);

        }

        if(other.tag == "Asteroid")
        {
            DecreaseBossHull();
            Destroy(other.gameObject);
            Instantiate(asteroidExplosion, other.transform.position, Quaternion.identity);
        }

        if (other.tag == "GameBoundary" || other.tag == "Shield" || other.tag == "Ammo" || other.tag == "WingSupport" ||
            other.tag == "EnemyLazer")
        {
            return;
        }


    }

    //Описать логику взрывов корабля (для начала сделаем просто взырыв, потом сделаем красиво
    /// <summary>
    /// повреедение корпусса корабля / уничтожения корабля
    /// </summary>
    private void DecreaseBossHull()
    {
        if (hullPower > 1)
        {
            hullPower--;
            gameControllerScript.bossHullTxt.text = "Boss Hull: " + hullPower;
            
            if(gameObject.activeInHierarchy == false)
            {
                gameControllerScript.bossHullTxt.text = "";

                gameControllerScript.InvokeMenu();
            }

        }
        else
        {
            Instantiate(playerExplosion, transform.position, Quaternion.identity);

            Destroy(gameObject);

            gameControllerScript.bossHullTxt.text = "";

            gameControllerScript.InvokeMenu();
        }
    }

    /// <summary>
    /// Выстрел орудий корабля
    /// </summary>
    private void Shoot()
    {
        //стреляем только в случае активности игрока с учетом требуемой задержки
        if (player.activeInHierarchy == true && gameControllerScript.GetPlayerLives() > 1 && IsAcceptedToShoot)
        {
            Instantiate(lazerShot, straightLazerGun1.transform.position, Quaternion.Euler(0, 180, 0));
            Instantiate(lazerShot, straightLazerGun2.transform.position, Quaternion.Euler(0, 180, 0));
            if(player.activeInHierarchy == false)
            {
                StartCoroutine(IStopShooting());
            }

        }


    }

    /// <summary>
    /// Вносим запрет на стрельбу после респауна игрока, чтобы босс не сбивал игрока сразу же и с ходу
    /// </summary>
    /// <returns></returns>
    IEnumerator IStopShooting()
    {
        float delay = 0;
        if(CheckDistance() < 30)
        {
            delay = 0.8f;
        }
        else
        {
            delay = 0.5f;
        }

        IsAcceptedToShoot = false;
        yield return new WaitForSeconds(delay);
        IsAcceptedToShoot = true;
    }

    /// <summary>
    /// проверка дистанции от корабля босса до игрока
    /// </summary>
    /// <returns></returns>
    private float CheckDistance()
    {
        float distance = 0;
        if (transformPlayer)
        {
            distance = Vector3.Distance(transformPlayer.position, transform.position);
        }
        return distance;
    }

    /// <summary>
    /// Выстрел из управляемого лазера с предварительным наведением
    /// </summary>
    private void GuidedShoot()
    {
        if (player.activeInHierarchy == true)
        {
            Instantiate(guidedLazerShot, guidedGun1.transform.position, guidedGun1.transform.rotation);
            Debug.Log("Guided Shoot");
        }

    }



    /// <summary>
    /// временная блокировка поворотов корабля после соприкосновения с границей сцены
    /// </summary>
    /// <returns></returns>
    IEnumerator ICanTurn()
    {
        CanTurn = false;
        yield return new WaitForSeconds(2.5f);
        CanTurn = true;
    }

    private void ShipWaving()
    {
        if (IsStrifeLeft)
        {
            boss.rotation = Quaternion.Euler(-5, transform.rotation.eulerAngles.y, -10);
            IsStrifeLeft = false;
        }
        else
        {
            boss.rotation = Quaternion.Euler(5, transform.rotation.eulerAngles.y, 10);
            IsStrifeLeft = true;

        }

    }

}
