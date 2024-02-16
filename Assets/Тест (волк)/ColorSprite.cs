using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSprite : MonoBehaviour
{
    SpriteRenderer a;
    private void Start()
    {
       a = this.GetComponentInParent<SpriteRenderer>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            a.color = Color.white;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //a.color = Color.
    }
}
