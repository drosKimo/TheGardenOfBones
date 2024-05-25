using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_AnyButton : MonoBehaviour
{
    void OnGUI()
    {
        var pushed = Event.current;

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}