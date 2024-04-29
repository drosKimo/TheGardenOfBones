using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CS_PlantBehaviour : MonoBehaviour
{
    // здесь нужен будет таймер о обработчик системы ухода за садом

    [SerializeField] public GameObject help;
    [HideInInspector] public float timeLeft, angry_timeLeft; // время в секундах
    [HideInInspector] public Animator animHelp;

    TMP_Text text;
    Animator animObj, animPlayer;
    // даст возможность приостановить или ускорить "злое" время
    int angryMult = 1;

    private void Awake()
    {
        animHelp = help.GetComponent<Animator>();
        animObj = GetComponent<Animator>();
        text = GameObject.Find("DaytimeText").GetComponent<TMP_Text>();
        animPlayer = GameObject.Find("Player").GetComponent<Animator>();

        animObj.SetBool("TimeRun", true);
    }

    void LateUpdate()
    {
        switch (text.text)
        {
            case "night time": // ночью таймер приостанавливается
                animObj.SetInteger("TimeUp", 0);
                break;

            case "work time": // днем таймер идет
                if (animObj.GetBool("TimeRun"))
                {
                    angryMult = 0;
                    timeLeft = 5; // если не задать время здесь, таймер не получится запустить заново
                    animObj.SetBool("TimeRun", false); // запрет на перезапуск таймера
                    StartCoroutine(StartTimer()); // таймер
                }

                animObj.SetInteger("TimeUp", 1); // устанавливает, что днем таймер идет со скоростью 1

                break;
        }
    }

    private IEnumerator StartTimer()
    {
        // каждый кадр тикает время
        while (timeLeft > 0)
        {
            //Debug.Log(timeLeft);

            // TimeUp = множитель течения таймера
            timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp");
            UpdateTime();
            yield return null;
        }
    }

    private IEnumerator AngryTimer()
    {
        // каждый кадр тикает время
        // если не успеть до конца, растение пропадет
        while (angry_timeLeft > 0)
        {            
            // TimeUp = множитель течения таймера
            angry_timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp") * angryMult;
            UpdateAngryTime();

            yield return null;
        }
    }

    private void UpdateTime()
    {
        if (timeLeft < 0) // когда таймер вышел
        {
            animHelp.SetTrigger("Triggered"); // включает подсказку "помощь"
            timeLeft = 0;
            angry_timeLeft = 10;
            angryMult = 1;
            StartCoroutine(AngryTimer()); // "злой" таймер
        }
    }

    private void UpdateAngryTime()
    {
        if (angry_timeLeft < 0) // когда таймер вышел
        {
            animHelp.SetTrigger("Left"); // выключает "помощь"
            angry_timeLeft = 0;
            animPlayer.SetInteger("AngrySpirit", 1);
            Destroy(gameObject, 0.5f);
        }
    }
}
