using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameControllerScript : MonoBehaviour
{
    /* 0. Закодить возврат уровня на первоначальные значения параметров сложности логику кодить в эмиттере
     3. Лист для записи Хайскоре
     4. Механика увеличения скороти - пожертвуй щитом, увеличь скорость( ???) Пока вопрос
     5. Неожиданный варинант!. подумать на тему авианосца, с которого будут стартовать корабли противника и свои корабли тоже
     6. Сделать вариативный темп стрельбы противников
     8. Сделать варианит противника - enemy2 - двухпушечного, и Enemy 3 - 3 пушечного
     12. Эффект подбитого игрока после поражения
     13. Сделать эмиссию своей эскадрильи в зависимости от набранных бонусов Wing Support, только идти не через лист!
     14. Большие боссы по окончании уровня
     15. сделать отдельный префаб и скрипт для Управляемого лазера (для работы с корабля босса и даллее)
     10 */


    public Text scoreTextElement;
    public Text highScoreTxtElem;
    public Text shieldTxtElem;
    public Text startBtnTxtElem;
    public Text waveTxtElem;
    public Text wpnStatusTxt;
    public Text getReadyTxt;
    public Text largeLaserTxt;
    public Text smallLaserTxt;
    public Text wingSupportTxt;
    public Text playerLivesTxt;
    public Text destroyerSupportTxt;
    public Text bossHullTxt;


    public Button startButton;
    public Button exitButton;
    public Button continueButton;
    public Button goToControlsMenuButton;
    public Button goBackToMainMenuButton;



    public GameObject menu;
    public GameObject controlsMenu;
    public GameObject player;
    public GameObject background;
    public GameObject asteroidEmitter;



    public GameObject playerShield;
    public GameObject[] largeLazers; // массив главных орудий

    protected int shieldPower =0;// мощность щита, на старте щит выключен
    protected int wingSupportCount = 0; // Счетчик бонусов Wing Support, на старте равен 0

    private int wave; //номер волны- для поднятия сложности
    private int playerLives; // Жизни игрока

    protected int score = 0;
    public int scoreToEmitBoss; //количество очков, необходимое для появления босса
    protected int highScore = 0;


    private bool IsStarted = false; //Игра начата?
    internal bool IsPlayerDown = true; // игрок уничтожен?

    internal bool IsPaused = false;
    internal bool IsWingLaunched = false; //если активирован ПауэрАп Wing Support, на старте равен 0
    private bool IsLivesLimit = false; //лимит по увеличению жизней
    private bool IsDestroyerReady; // готовность Эсминца (супер оружие 2)
    private bool IsItsTimeToBoss; // появление босса

    public float destroyerShotDelay; // время на подготовку выстрела эсминца


    Vector3 currentPlayerPosition;


    private void Awake()
    {
        background.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        //scoreTextElement.text = "Score: 0";
        //startButton.onClick.AddListener(delegate
        //{
        //    IsStarted = true;
        //    menu.SetActive(false);
        //    background.SetActive(true);
        //});
        //playerShield.SetActive(false);


        controlsMenu.SetActive(false); // отключаем меню
        score = 0; // обнуляем счетчик очков
        scoreTextElement.text = "Score: " + score;

        IsDestroyerReady = false;
        destroyerSupportTxt.text = "Destroyer is prepearing";
        destroyerSupportTxt.color = Color.red;

        wpnStatusTxt.text = "All weapons in charge";

        wingSupportTxt.text = "Wing Support: 0";

        playerLives = 2; // 2 жизни на старте
        playerLivesTxt.text = "LIVES: " + playerLives;

        //босс уровня неактивен
        IsItsTimeToBoss = false;

        // убираем текст статуса босса

        bossHullTxt.text = "";
     

    }


    

    

    private void Update()
    {
        if (menu.activeInHierarchy == false)
        {
            if (player.activeInHierarchy == false)
            {

                IsStarted = false;
                //menu.SetActive(true);
                //background.SetActive(false);



                //Invoke("MenuSetActive", 2.0f);

            }
        }


        startButton.onClick.AddListener(delegate
        {
            if(IsPlayerDown == false)
            {
                startBtnTxtElem.text = "RESTART";
            }
            else if(IsPlayerDown == true)
            {
                startBtnTxtElem.text = "Start Game";
            }
            StartCoroutine(GetReady());

            IsItsTimeToBoss = false;
            bossHullTxt.text = "";

            IsPlayerDown = false;
            shieldPower = 0;
            shieldTxtElem.text = "Shields: " + shieldPower;
            wingSupportCount = 0;
            wingSupportTxt.text = "Wing Support " + wingSupportCount;

            IsDestroyerReady = false;
            destroyerSupportTxt.color = Color.red;

            wpnStatusTxt.text = "All weapons in charge";

            player.SetActive(true);
            IsStarted = true;
            menu.SetActive(false);
            background.SetActive(true);
            IsPlayerDown = false;
            SetFirstWave();
            Debug.Log("Start button pressed");

            playerLives = 2;
            playerLivesTxt.text = "LIVES: " + playerLives;


            player.transform.position = new Vector3(0, 0, -6.8f);
            SetHighScore();
            score = 0;
            scoreTextElement.text = "Score: " + score;

           
        });

        continueButton.onClick.AddListener(delegate
        {
            if(IsPlayerDown ==false)
            {
                //player.SetActive(true);
                //getReadyTxt.text = "";
                //player.transform.position = currentPlayerPosition;
                //IsStarted = true;
                menu.SetActive(false);
                IsPaused = false;
                getReadyTxt.text = "";
                Time.timeScale =1; //Снимаем игру с паузы, игра продолжается
                //IsGamePaused();
                //background.SetActive(true);
            }
            else
            {
                return;
            }
      

        });
       


     

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //IsStarted = false;
            //currentPlayerPosition = player.transform.position;
            //player.SetActive(false);
            //background.SetActive(false);

            if(menu.activeInHierarchy == false)
            {
                IsGamePaused();
                menu.SetActive(true);
            }
            else
            {
                IsGamePaused();
                menu.SetActive(false);
            }
         
        }

        exitButton.onClick.AddListener(delegate
        {
            Application.Quit();
        });



        if (shieldPower > 0)
            playerShield.SetActive(true);
        else if (shieldPower == 0)
            playerShield.SetActive(false);



        // Ставим игру на паузу
        if (Input.GetKeyDown(KeyCode.P))
        {
            IsGamePaused();
            //if(IsPaused == false)
            //{
            //    getReadyTxt.text = "PAUSE";
            //    Time.timeScale = 0;
            //    IsPaused = true;
            //}
            //else
            //{
            //    Time.timeScale = 1;
            //    IsPaused = false;
            //    getReadyTxt.text = "";

            //}
        }

        // проходим в меню Controls

        goToControlsMenuButton.onClick.AddListener(delegate
        {
            //menu.SetActive(false);
            controlsMenu.SetActive(true);
        });

        //Переходим обратно в главное меню
        goBackToMainMenuButton.onClick.AddListener(delegate
        {
            //menu.SetActive(true);
            controlsMenu.SetActive(false);
        });


        // босс входит в игру

        if(score >= scoreToEmitBoss)
        {
            //IsStarted = false;
            IsItsTimeToBoss = true;
        }



    }


    /// <summary>
    /// Метод постановки игры на паузу
    /// </summary>
    private void IsGamePaused()
    {
        if (IsPaused == false)
        {
            getReadyTxt.text = "PAUSE";
            Time.timeScale = 0;
            IsPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            IsPaused = false;
            getReadyTxt.text = "";

        }
    }

    /// <summary>
    /// Снятие игры с паузы после нажатия на кнопку Continue в игровом меню
    /// </summary>
    private void LetsContinueTheGame()
    {
        Time.timeScale = 1;

    }


    /// <summary>
    /// Увеличение количества набранных очков
    /// </summary>
    /// <param name="increment"> покзатель роста очков (на сколько увеличить)</param>
    public void IncreaseScore(int increment)
    {
        score += increment;
        scoreTextElement.text = "Score: " + score;
    }

    /// <summary>
    /// Выводит на экран наибольшее количество очков
    /// </summary>
    private void SetHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            highScoreTxtElem.text = "High Score: " + highScore;
        }
    }

    /// <summary>
    /// Возвращает текущее значение набранных очков
    /// </summary>
    /// <returns></returns>
    public int ShowScore()
    {
        return score;
    }

    /// <summary>
    /// Метод возвращает статус начала игры
    /// </summary>
    /// <returns></returns>
    public bool GetIsStarted()
    {
        return IsStarted;
    }

    /// <summary>
    /// Возвращает количество заработанных бонусов Wing Support
    /// </summary>
    /// <returns></returns>
    public int ShowWingSupportCount()
    {
        return wingSupportCount;
    }




    /// <summary>
    /// Увеличение мощности щитов и повышения жизней в зависимости от мощности щитов
    /// </summary>
    public void RiseShields()
    {
        if(shieldPower < 4)
        {
            shieldPower++;
        }
        else
        {
            IncreaseScore(75);
            if (playerLives > 5)
            {
                IsLivesLimit = true;
            }
            else
            {
                IsLivesLimit = false;
            }

            if(IsLivesLimit == false)
            {
                IncreaseLives();
            }
            else
            {
                return;
            }
        }
        ShowShields();
    }

    /// <summary>
    /// Возвращает значение показателя мощности щитов
    /// </summary>
    /// <returns></returns>
    public int ShowShieldPower()
    {
        return shieldPower;
    }

    /// <summary>
    /// снижение мощности щитов
    /// </summary>
    public void LoseShields()
    {
        if(shieldPower > 0)
        {
            shieldPower--;
        }
        ShowShields();
    }

    /// <summary>
    /// Метод, выводящий показания мощности щита на экран (UI)
    /// </summary>
    private void ShowShields()
    {
        shieldTxtElem.text = "Shields: " + shieldPower;

    }

    /// <summary>
    /// Вызов активации меню после проигрыша или выигрыша игрока через определенное время
    /// </summary>
    public void InvokeMenu()
    {
        if(player.activeInHierarchy == false)
        {
            getReadyTxt.text = "GAME OVER!";

        }
        else
        {
            getReadyTxt.text = "LEVEL COMPLETED!";

        }
        Invoke("MenuSetActive", 4.0f);
    }


    /// <summary>
    /// Активация меню игры
    /// </summary>
    private void MenuSetActive()
    {
        menu.SetActive(true);
        background.SetActive(false);
        getReadyTxt.text = "";
        if(player.activeInHierarchy == true)
        {
            IsPlayerDown = true;
            player.SetActive(false);
        }
    }

    /// <summary>
    /// Повышение уровня сложности и отображение его в UI
    /// </summary>
    public void IncreaseWave()
    {
        wave++;
        waveTxtElem.text = "Wave: " + wave;
    }

    /// <summary>
    /// Устанавливает счетччик вол противников в позицию 1
    /// </summary>
    private void SetFirstWave()
    {
        wave = 1;
        waveTxtElem.text = "Wave: 1";

    }

    /// <summary>
    /// Вызывает повреждение одного из главных орудий
    /// </summary>
    public void LazerManfunction()
    {
        int lazerDamageIndex = Random.Range(0, largeLazers.Length);
        largeLazers[lazerDamageIndex].SetActive(false);
        wpnStatusTxt.text = "WEAPON LOST!!!";
        wpnStatusTxt.color = Color.red;
        StartCoroutine(RestoreLazer(lazerDamageIndex));
    }

    /// <summary>
    /// восстанваливает орудие после повреждения
    /// </summary>
    /// <param name="index">индекс орудия</param>
    /// <returns></returns>
    IEnumerator RestoreLazer(int index)
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        largeLazers[index].SetActive(true);
        foreach(GameObject lazerGun in largeLazers)
        {
            if(lazerGun.activeInHierarchy == true)
            {
                wpnStatusTxt.text = "All weapons in charge";
                wpnStatusTxt.color = Color.white;
            }

        }

    }

    /// <summary>
    /// Убирает текст Get ready, выключает и затем включает астероид эмиттер
    /// </summary>
    /// <returns></returns>
    IEnumerator GetReady()
    {
        
        asteroidEmitter.SetActive(false);
        yield return new WaitForSeconds(3.0f);
        asteroidEmitter.SetActive(true);
        getReadyTxt.text = " ";
    }


    /// <summary>
    /// Запись и вывод на экран состояния большого лазера
    /// </summary>
    /// <param name="qauntity"></param>
    public void SetLargeLaserTxt(int qauntity)
    {
        largeLaserTxt.text = "Large Laser " + qauntity;
        if(qauntity < 1)
        {
            largeLaserTxt.text = "LL CHARGING!";
            largeLaserTxt.color = Color.red;

        }
        else
        {
            largeLaserTxt.color = Color.white;

        }
    }

    /// <summary>
    /// Запись и вывод на экран состояния малого лазера
    /// </summary>
    /// <param name="qauntity"></param>
    public void SetSmallLaserTxt(int qauntity)
    {
        smallLaserTxt.text = "Small Laser " + qauntity;
        if (qauntity < 1)
        {
            smallLaserTxt.text = "SL CHARGING!";
            smallLaserTxt.color = Color.red;

        }
        else
        {
            smallLaserTxt.color = Color.white;

        }
    }

    /// <summary>
    /// Метод увеличения бонуса Wing Support
    /// </summary>
    public void AddWingSupport()
    {
        if(wingSupportCount < 5)
        {
            wingSupportCount++;
        }
        else
        {
            return;
        }

        if (IsWingLaunched == false)
        {
            wingSupportTxt.text = "Wing Support " + wingSupportCount + " press numpad_1 to use";
            wingSupportTxt.color = Color.yellow;

        }
        else
        {
            wingSupportTxt.color = Color.red;

            wingSupportTxt.text = "Career vessel is prepearing";
            StartCoroutine(ControlAddWingSupportText());
        }
    }

    /// <summary>
    /// Изменение текста статуса буонуса Wing Support 
    /// </summary>
    /// <returns></returns>
    IEnumerator ControlAddWingSupportText()
    {
        wingSupportTxt.color = Color.cyan;

        wingSupportTxt.text = "Wing Support " + wingSupportCount;

        yield return new WaitForSeconds(1.5f);

        wingSupportTxt.color = Color.red;
        wingSupportTxt.text = "Career vessel is prepearing";


    }

    /// <summary>
    /// Устанавливает значение переменной wingSupportCount для регулирования количества бонусов Wing Support
    /// </summary>
    /// <param name="newCount">требуемое количество бонусов Wing Support</param>
    public void SetWingSupportCount(int newCount)
    {
        wingSupportCount = newCount;
    }

    //манипуляции с жизнями игрока

    /// <summary>
    /// Уменьшение жизни игрока
    /// </summary>
    public void DecreaseLives()
    {
        if(playerLives > 0)
        {
            IsStarted = false;
            IsPlayerDown = true;
            player.SetActive(false);
            playerLives--;
            playerLivesTxt.text = "LIVES: " + playerLives;
            StartCoroutine(ContinueGame());
            IsDestroyerReady = false;
            destroyerSupportTxt.text = "Destroyer is prepearing";
            destroyerSupportTxt.color = Color.red;
            StartCoroutine(ChargingDestroyer(destroyerShotDelay));



        }
        else
        {
            IsStarted = false;
            IsPlayerDown = true;
            player.SetActive(false);
            IsItsTimeToBoss = false;

            InvokeMenu();
        }
    }

    /// <summary>
    /// Увеличение жизни игрока
    /// </summary>
    public void IncreaseLives()
    {
        playerLives++;
        playerLivesTxt.text = "LIVES: " + playerLives;

    }

    /// <summary>
    /// Возвращает количество жизней игрока
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLives()
    {
        return playerLives;
    }

    /// <summary>
    /// Продолжение игры после потери жизни
    /// </summary>
    /// <returns></returns>
    IEnumerator ContinueGame()
    {
        getReadyTxt.text = "GET READY FOR NEXT ATTEMPT";
        yield return new WaitForSeconds(2.0f);
        getReadyTxt.text = "";
        IsPlayerDown = false;
        shieldPower = 0;
        shieldTxtElem.text = "Shields: " + shieldPower;
        wingSupportCount = 0;
        wingSupportTxt.text = "Wing Support " + wingSupportCount;
        wingSupportTxt.color = Color.yellow;

        player.SetActive(true);
        IsStarted = true;
        menu.SetActive(false);
        background.SetActive(true);
        IsPlayerDown = false;
        
        player.transform.position = new Vector3(0, 0, -6.8f);
        scoreTextElement.text = "Score: " + score;

    }


    // методы управления переменно IsDestroyerReady

    /// <summary>
    /// Возвращает статус готовоности эсминца (супер оружие 2)
    /// </summary>
    /// <returns></returns>
    public bool GetIsDestroyerReady()
    {
        return IsDestroyerReady;
    }

    /// <summary>
    /// ставит статус готовности эсминца в положение true
    /// </summary>
    public void SetIsDestoyerReady()
    {
        IsDestroyerReady = true;
    }

    /// <summary>
    /// ставит статус готовности эсминца в положение false
    /// </summary>
    public void SetIsDestroyerCharging()
    {
        IsDestroyerReady = false;
    }



    /// <summary>
    /// Супрограмма - Перезарядка орудий эсимнца (супероружие 2)
    /// </summary>
    /// <param name="chargingDelay">время на перезарядку орудий эсминца</param>
    /// <returns></returns>
    IEnumerator ChargingDestroyer(float chargingDelay)
    {
        destroyerSupportTxt.text = "";
        destroyerSupportTxt.text = "Destroyer is prepearing";
        destroyerSupportTxt.color = Color.red;
        yield return new WaitForSeconds(chargingDelay);

        SetIsDestoyerReady();
        destroyerSupportTxt.text = "Destroyer is ready to punish, press numpad_2";
        destroyerSupportTxt.color = Color.cyan;

    }

    /// <summary>
    /// метод для вызова сопрограммы ChargingDestroyer из других скриптов кром Game Controller
    /// </summary>
    /// <param name="chargingDelay">время на перезарядку орудий эсминца</param>
    public void ChargeDestroyer(float chargingDelay)
    {
        StartCoroutine(ChargingDestroyer(chargingDelay));
    }

    //



    // обработка событий, свщязанных с боссом

    /// <summary>
    /// Возвращает состояние о готовности босса
    /// </summary>
    /// <returns>готовность босса</returns>
    public bool GetIsBossReady()
    {
        return IsItsTimeToBoss;
    }


    /// <summary>
    /// Активация босса
    /// </summary>
    public void SetBossIsReady()
    {
        IsItsTimeToBoss = true;
    }

    /// <summary>
    /// Деактивация босса
    /// </summary>
    public void SetBossIsDefeated()
    {
        IsItsTimeToBoss = false;

    }

    



}
