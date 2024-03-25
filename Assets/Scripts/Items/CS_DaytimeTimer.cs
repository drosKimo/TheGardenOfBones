using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CS_DaytimeTimer : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    // ����� � UnityEditor ������� � ��������
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
        // ������ ���� ������ �����
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
