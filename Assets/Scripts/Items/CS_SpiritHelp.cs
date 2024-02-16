using UnityEngine;

public class CS_ColorHelp : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anim;
    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // включает очередь анимаций
        if (collision.tag == "Player")
        {
            anim.SetTrigger("Triggered"); // сначала включает триггер
            anim.SetTrigger("Triggered"); // потом выключает. Иначе, багует
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // выключает
        anim.SetTrigger("Left");
    }
}
