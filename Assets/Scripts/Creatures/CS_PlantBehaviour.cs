using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CS_PlantBehaviour : MonoBehaviour
{
    // ����� ����� ����� ������ � ���������� ������� ����� �� �����

    [SerializeField] public GameObject help;
    TMP_Text text;

    float timeLeft = 5; // ����� � ��������
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
            case "night time": // ����� ������ ������������������
                animObj.SetInteger("TimeUp", 0);
                break;

            case "work time": // ���� ������ ����
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
        // ������ ���� ������ �����
        while (timeLeft > 0)
        {
            Debug.Log(timeLeft);
            // TimeUp = ��������� ������� �������
            timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp");
            UpdateTimeText();
            yield return null;
        }
    }

    private void UpdateTimeText()
    {
        // ����� ������ �����
        if (timeLeft < 0)
        {
            animHelp.SetTrigger("Triggered");
            animObj.SetInteger("TimeUp", 1);

            timeLeft = 0;
        }
    }
}
