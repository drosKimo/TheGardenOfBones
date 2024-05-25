using TMPro;
using UnityEngine;

            // ТОЛЬКО ДЛЯ ТУТОРИАЛА
public class CS_SpeechBoxScript : MonoBehaviour
{
    public int counter = 0; // счетчик открытий диалогового окна

    TMP_Text speechBox; // текстовое поле

    private void Awake()
    {
        speechBox = GameObject.Find("SP_text").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        counter++;
        Time.timeScale = 0f;
        TutorialScene();
    }

    public void TutorialScene()
    {
        switch (counter)
        {
            case 1:
                speechBox.text = "Добро пожаловать в сад костей, ученик!";
                break;

            case 2:
                speechBox.text = "Отлично! Теперь попробуй закопать одну из них";
                break;

            case 3:
                speechBox.text = "Хорошо, теперь ты знаешь как работать с землёй";
                break;

            case 4:
                speechBox.text = "Видишь облачко? Это значит, что ему нужна твоя помощь";
                break;

            case 5:
                speechBox.text = "Теперь этот дружок будет ходить за тобой!";
                break;

            case 6:
                speechBox.text = "Как и всем растениям, ему нужен уход";
                break;

            case 7:
                speechBox.text = "Совсем другое дело! Не забывай ухаживать за садом";
                break;
        }
    }
}
