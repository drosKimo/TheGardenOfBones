using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_SceneChanger : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void WebPage()
    {
        Application.OpenURL("https://droskimo.ru");
    }

    public void About()
    {
        Application.OpenURL("https://www.furaffinity.net/user/droskimo");
    }

    public void Support()
    {
        Application.OpenURL("https://boosty.to/droskimo");
    }
}
