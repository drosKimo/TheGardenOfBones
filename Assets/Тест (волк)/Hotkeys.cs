using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotkeys : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnGUI()
    {
        if (Input.anyKeyDown)
        {
            var pushed = Event.current;
            if (pushed.isKey)
            {
                switch (pushed.keyCode)
                {
                    case KeyCode.F:
                        {
                            anim.SetBool(name: "isUse", value: true);
                            break;
                        }
                    default:
                        break;
                }
            }
            if (pushed.isMouse)
            {

            }
        }
    }
}
