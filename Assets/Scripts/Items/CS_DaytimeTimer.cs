using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CS_DaytimeTimer : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    // время в UnityEditor ставить в секундах
    public Slider slider;
    float timeLeft;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        timeLeft = slider.value;
        StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
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
