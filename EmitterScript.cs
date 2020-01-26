using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterScript : MonoBehaviour
{
    public GameObject [] asteroids; // массив астероидов и важеских истребителей
    public GameObject shield; // щиты
    public GameObject downedPilot; // сбитый летчик
    public GameObject ammo; // дополнительные снаряды
    public GameObject wingSupport; // бонус дополнительной эскадрильи
    public GameObject boss; // босс уровня

    public float minDelay, maxDelay; // Минимальная и максимальная задержка при эмиссии астероидов
    public float minShieldDelay, maxShieldDelay; // миниммальная и максимальная задержка при эмисиии щитов
    //
    // для хранения первоначальных значений эмисии асетроидов и щитов
    public float memoryStartMinDelay, memoryStartMaxDelay;
    public float memoryStartMinShieldDelay, memoryStartMaxShieldDelay;
    //
    public float delayCorrector; // коэффициент, уменьшающий время задержки эмиссии астероида
    public float scoreStep; // Шаг изменения интеснивности эмиссии астероидов
    private float memoryScoreStep; // Запоминаем scoreStep

    private float nextLaunch; // следующий запуск астероида
    private float nextShieldLaunch; // следующий запуск щита
    private float nextDownedPilotLaunch; // следующий запуск подбитого истребителя
    private float nextAmmoLaunch; // Запуск следующего блока со снарядами
    private float nextWingSupportLaunch; // запуск следующего блока с бонусом WingSupport

    public float minAmmoLaunchDelay, maxAmmoLaunchDelay; // Минимальный и максимальный интервал запуска боекомплектов

    public float minWingSupportDelay, maxWingSupportDelay;

    private bool IsBossEmitted; // босс эмитирован?



    protected GameControllerScript gameControllerScript;

    // Start is called before the first frame update
    void Start()
    {
    
        gameControllerScript =
            GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();

        memoryScoreStep = scoreStep;

        //
        // сохраняем первоначальные значения задержки эмиссии объектов
        memoryStartMinDelay = minDelay;
        memoryStartMaxDelay = maxDelay;
        memoryStartMinShieldDelay = minShieldDelay;
        memoryStartMaxShieldDelay = maxShieldDelay;
        //

        // ставим позицию эмиссии босса в положение false

        IsBossEmitted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Если игра не началась, то ничего не происходит
        if(!gameControllerScript.GetIsStarted())
        {
            return;
        }

        if (!IsBossEmitted)
        {
            //Запуск вражеских астероидов и истритбителей
            if (Time.time > nextLaunch)
            {
                int asteroidIndex = Random.Range(0, asteroids.Length);
                nextLaunch = Time.time + Random.Range(minDelay, maxDelay);

                float halfWidth = transform.localScale.x / 2;
                float positionX = Random.Range(-halfWidth, halfWidth);

                Vector3 newAsteroidPosition = new Vector3(positionX, transform.position.y,
                    transform.position.z);
                Instantiate(asteroids[asteroidIndex], newAsteroidPosition, Quaternion.Euler(0, 180, 0));
            }

            //запуск корабля подбитого пилота
            if (Time.time > nextDownedPilotLaunch)
            {
                nextDownedPilotLaunch = Time.time + Random.Range(8f, 14f);

                float halfWidth = transform.localScale.x / 2;
                float positionX = Random.Range(-halfWidth, halfWidth);

                Vector3 newdownedPilotPosition = new Vector3(positionX, transform.position.y,
                    transform.position.z);
                Instantiate(downedPilot, newdownedPilotPosition, Quaternion.Euler(0, 180, 0));
                Debug.Log("Downed Pilot Launch");
            }


        }


        if(gameControllerScript.menu.activeInHierarchy == false)
        {
            // Периодический запуск щита
            if (Time.time > nextShieldLaunch)
            {
                nextShieldLaunch = Time.time + Random.Range(minShieldDelay, maxShieldDelay);

                float halfWidth = transform.localScale.x / 2;
                float positionX = Random.Range(-halfWidth, halfWidth);

                Vector3 newShieldPosition = new Vector3(positionX, transform.position.y,
                    transform.position.z);
                Instantiate(shield, newShieldPosition, Quaternion.Euler(0, 180, 0));
            }


            // Запуск нового блока снарядов
            if (Time.time > nextAmmoLaunch)
            {
                nextAmmoLaunch = Time.time + Random.Range(minAmmoLaunchDelay, maxAmmoLaunchDelay);

                float halfWidth = transform.localScale.x / 2;
                float positionX = Random.Range(-halfWidth, halfWidth);

                Vector3 newAmmoPosition = new Vector3(positionX, transform.position.y,
                    transform.position.z);
                Instantiate(ammo, newAmmoPosition, Quaternion.Euler(0, 180, 0));
            }

            // запуск нового комплекта бонусов Wing Support

            if (Time.time > nextWingSupportLaunch)
            {
                nextWingSupportLaunch = Time.time + Random.Range(minWingSupportDelay, maxWingSupportDelay);

                float halfWidth = transform.localScale.x / 2;
                float positionX = Random.Range(-halfWidth, halfWidth);

                Vector3 newWSPosition = new Vector3(positionX, transform.position.y,
                    transform.position.z);
                Instantiate(wingSupport, newWSPosition, Quaternion.Euler(0, 180, 0));
            }

        }

        // Увеличиваем уровень сложности (волну)
        if (scoreStep <= gameControllerScript.ShowScore())
        {
            DelayCorrectionLow();
            scoreStep += memoryScoreStep;
            Debug.Log("new wave " + scoreStep);
            gameControllerScript.IncreaseWave();
        }

        // Восстанавливаем первичные показатели задержки при эмиссии объектов

        gameControllerScript.startButton.onClick.AddListener(delegate
        {
            minDelay = memoryStartMinDelay;
            maxDelay = memoryStartMaxDelay;
            minShieldDelay = memoryStartMinShieldDelay;
            maxShieldDelay = memoryStartMaxShieldDelay;
        });


        // запускаем босса уровня
        if (gameControllerScript.GetIsBossReady() && !IsBossEmitted)
        {
            StartCoroutine(LaunchBoss());
            IsBossEmitted = true;
        }

        // переключаем статус эмисии босса при нажатии кновки старт в положение выкл
        gameControllerScript.startButton.onClick.AddListener(delegate
        {
            IsBossEmitted = false;
        });

    }

    /// <summary>
    /// Снижает интервал между эмиссией игровых ообъектов, между австероидами уменьшается, между щитами увеличивается
    /// </summary>
    private void DelayCorrectionLow()
    {
        minDelay *= delayCorrector;
        maxDelay *= delayCorrector;
        minShieldDelay /= delayCorrector;
        maxShieldDelay /= delayCorrector;
    }


    /// <summary>
    /// Эмиссия босса с задержкой
    /// </summary>
    /// <returns></returns>
    IEnumerator LaunchBoss()
    {
        gameControllerScript.getReadyTxt.text = "Get Ready For Royale Rumble";

        
        yield return new WaitForSeconds(4.0f);

        gameControllerScript.getReadyTxt.text = "";

        float halfWidth = transform.localScale.x / 2;
        float positionX = Random.Range(-halfWidth, halfWidth);
        Vector3 newBossPosition = new Vector3(positionX, transform.position.y,
               transform.position.z);
        Instantiate(boss, newBossPosition, Quaternion.Euler(0, 180, 0));
        


    }



}
