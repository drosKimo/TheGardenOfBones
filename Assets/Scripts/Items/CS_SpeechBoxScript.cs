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
                speechBox.text = "Welcome to the Garden of bones, follower!";
                break;

            case 2:
                speechBox.text = "Great! Now try to bury one of them";
                break;

            case 3:
                speechBox.text = "Okay, now you know how to work with the earth";
                break;

            case 4:
                speechBox.text = "Do you see a cloud? That means it needs your help.";
                break;

            case 5:
                speechBox.text = "Now this buddy will follow you!";
                break;

            case 6:
                speechBox.text = "Like all plants, it needs care";
                break;

            case 7:
                speechBox.text = "It's a completely different matter! Don't forget to take care of the garden";
                break;
        }
    }
}
