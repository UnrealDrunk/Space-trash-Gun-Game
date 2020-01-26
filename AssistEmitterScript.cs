using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistEmitterScript : MonoBehaviour
{
    public GameObject allyFighter; // Пилот союзников (периодически появляется на поле боя)
    public GameObject wingFighter; // Пилот эскадрильи (появляется на поле боя по команде)
    public GameObject lazerOfDestroyer; // Лазеры для поддержки (супер оружие залп эскадренного миноносца)

    public float wingStartTimeReload; // время перезагрузки супероружия Wing Support
    public float delayFirstAllyTakeOff;

    private int countOfDestroyerShots;// количество выстрелов в одном залпе эсминца
    public float destroyerShotDelay; // время на подготовку выстрела эсминца

    public float minTimeTakeOffDelay, maxTimeTakeOffDelay; // Минимальное и максимальное время между вылетами союзников
    protected GameControllerScript gameControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        //Вылет истребителей союзника
        InvokeRepeating("TakeOff", delayFirstAllyTakeOff, Random.Range(minTimeTakeOffDelay, maxTimeTakeOffDelay));

        // Подключаем GameControllerScript
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").
            GetComponent<GameControllerScript>();

       
    }


    private void Update()
    {
        // Запуск эскадрильи поддержки
        if(gameControllerScript.IsWingLaunched == false && gameControllerScript.ShowWingSupportCount() >0)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //ChargeWing(5);
                WingAttack(gameControllerScript.ShowWingSupportCount());
                gameControllerScript.IsWingLaunched = true;
                StartCoroutine(StartSupportWingMessage());
                StartCoroutine(ReloadWing());
            }
        }

        // атака эсминца

        if(gameControllerScript.GetIsDestroyerReady() == true)
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //AttackOfDestroyer();
                gameControllerScript.getReadyTxt.text = "Destroyer discharges guns!";
                StartCoroutine(DestroyerDischarge(0.1f));
                StartCoroutine(DestroyerDischarge(0.5f));
                StartCoroutine(DestroyerDischarge(0.9f));
                gameControllerScript.SetIsDestroyerCharging();
                gameControllerScript.ChargeDestroyer(destroyerShotDelay);
            }

        }

        // при нажатии кнопки старт запускается процесс заряда орудий
        gameControllerScript.startButton.onClick.AddListener(delegate
        {
            gameControllerScript.ChargeDestroyer(destroyerShotDelay);

        });
        

    }
        


    /// <summary>
    /// Вылет в бой самолетов союзника
    /// </summary>
    private void TakeOff()
    {
        if(gameControllerScript.menu.activeInHierarchy == false)
        {
            float halfWidth = transform.localScale.x / 2;
            float positionX = Random.Range(-halfWidth, halfWidth);

            Vector3 newAllyPosition = new Vector3(positionX, transform.position.y,
                transform.position.z);
            Instantiate(allyFighter, newAllyPosition, Quaternion.identity);
        }
       
    }

    

    /// <summary>
    /// Запуск эскадрильи в атаку
    /// </summary>
    /// <param name="count">количество машин</param>
    private void WingAttack(int count)
    {

        float halfWidth = transform.localScale.x / 2;
      
        for (int i =0; i < count; i++)
        {
            float positionX = Random.Range(-halfWidth, halfWidth);
            Vector3 newFighterPosition = new Vector3(positionX, transform.position.y, transform.position.z);
            Instantiate(wingFighter, newFighterPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Активация таймера перезапуска бонуса Wing Support
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadWing()
    {
        gameControllerScript.wingSupportTxt.text = "Career vessel is prepearing";
        gameControllerScript.wingSupportTxt.color = Color.red;
        gameControllerScript.SetWingSupportCount(0);

        yield return new WaitForSeconds(wingStartTimeReload);
        gameControllerScript.wingSupportTxt.color = Color.yellow;
        // Поменять моделки истребителей поддержки... чтобы красиво отличались
        gameControllerScript.IsWingLaunched = false;

    }

    /// <summary>
    /// Вывод на экран сооббщения об использовании бонуса Wing Support
    /// </summary>
    /// <returns></returns>
    IEnumerator StartSupportWingMessage()
    {
        if(gameControllerScript.ShowWingSupportCount() == 1)
        {
            gameControllerScript.getReadyTxt.text = "WING SUPPORT!!!" + " " + 
                gameControllerScript.ShowWingSupportCount()+" Fighter assist" ;
        }else if(gameControllerScript.ShowWingSupportCount() > 1)
        {
            gameControllerScript.getReadyTxt.text = "WING SUPPORT!!!" + " " +
               gameControllerScript.ShowWingSupportCount() + " Fighters assist";
        }

        yield return new WaitForSeconds(1.5f);
        gameControllerScript.getReadyTxt.text = "";


    }

    /// <summary>
    /// Залп эсминца (супер оружие)
    /// </summary>
    private void AttackOfDestroyer()
    {
        //Instantiate(lazer, Gun_1.transform.position, Quaternion.identity);
        int Xstep = (int)transform.localScale.x / countOfDestroyerShots; // шаг выстрела
        float firstShotPositionXR = Xstep; // координата по Х первого выстрела по правому борту
        float firstShotPositionXL = -Xstep; // координата по Х первого выстрела по левому борту

        countOfDestroyerShots = Random.Range(10, 21);

        for (int i = 0; i < countOfDestroyerShots; i++)
        {
            //стрельба справа
            Vector3 firstShotPosititonR = new Vector3(firstShotPositionXR, transform.position.y,
        transform.position.z);

            Instantiate(lazerOfDestroyer, firstShotPosititonR, Quaternion.identity);

            firstShotPositionXR += Xstep;

            //стрельба слева
            Vector3 firstShotPosititonL = new Vector3(firstShotPositionXL, transform.position.y,
                transform.position.z);

            Instantiate(lazerOfDestroyer, firstShotPosititonL, Quaternion.identity);

            firstShotPositionXL -= Xstep;



        }


    }

    /// <summary>
    /// Залп Эсминца (супер оружие)
    /// </summary>
    /// <param name="delay">задержка залпа</param>
    /// <returns></returns>
    IEnumerator DestroyerDischarge(float delay)
    {
        yield return new WaitForSeconds(delay);

        countOfDestroyerShots = Random.Range(10, 21);

        //Instantiate(lazer, Gun_1.transform.position, Quaternion.identity);
        int Xstep = (int)transform.localScale.x / countOfDestroyerShots; // шаг выстрела
        float firstShotPositionXR = Xstep; // координата по Х первого выстрела по правому борту
        float firstShotPositionXL = -Xstep; // координата по Х первого выстрела по левому борту


        for (int i = 0; i < countOfDestroyerShots; i++)
        {
            //стрельба справа
            Vector3 firstShotPosititonR = new Vector3(firstShotPositionXR, transform.position.y,
        transform.position.z);

            Instantiate(lazerOfDestroyer, firstShotPosititonR, Quaternion.identity);

            firstShotPositionXR += Xstep;

            //стрельба слева
            Vector3 firstShotPosititonL = new Vector3(firstShotPositionXL, transform.position.y,
                transform.position.z);

            Instantiate(lazerOfDestroyer, firstShotPosititonL, Quaternion.identity);

            firstShotPositionXL -= Xstep;



        }

    }


    


}
