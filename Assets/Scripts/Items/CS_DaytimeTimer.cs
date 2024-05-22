using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_DaytimeTimer : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    // время в UnityEditor ставить в секундах
    [HideInInspector] public Slider slider;
    [HideInInspector] public float timeLeft;
    Animator animPlayer;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        animPlayer = GameObject.Find("Player").GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name != "Tutorial")
            StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
        timeLeft = slider.maxValue;

        // каждый кадр тикает время
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
    }

    private void UpdateTimeText()
    {
        slider.value = timeLeft;
        if (timeLeft < 0)
        {
            timeLeft = 0;
            text.text = "night time";
        }
    }
}
