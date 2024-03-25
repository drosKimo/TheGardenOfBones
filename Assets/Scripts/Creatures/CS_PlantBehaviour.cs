using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CS_PlantBehaviour : MonoBehaviour
{
    // здесь нужен будет таймер о обработчик системы ухода за садом

    [SerializeField] public GameObject help;
    TMP_Text text;

    float timeLeft = 5; // время в секундах
    Animator animHelp, animObj;

    private void Awake()
    {
        animHelp = help.GetComponent<Animator>();
        animObj = GetComponent<Animator>();
        text = GameObject.Find("DaytimeText").GetComponent<TMP_Text>();

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
                    animObj.SetBool("TimeRun", false);
                    StartCoroutine(StartTimer());
                }

                animObj.SetInteger("TimeUp", 1);
                break;
        }
    }

    private IEnumerator StartTimer()
    {
        // каждый кадр тикает время
        while (timeLeft > 0)
        {
            Debug.Log(timeLeft);
            // TimeUp = множитель течения таймера
            timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp");
            UpdateTimeText();
            yield return null;
        }
    }

    private void UpdateTimeText()
    {
        // когда таймер вышел
        if (timeLeft < 0)
        {
            animHelp.SetTrigger("Triggered");
            animObj.SetInteger("TimeUp", 1);

            timeLeft = 0;
        }
    }
}
