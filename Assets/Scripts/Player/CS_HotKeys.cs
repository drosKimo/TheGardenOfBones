using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CS_HotKeys : MonoBehaviour
{
    // Перепривязка клавиш:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    CS_PlayerController controller;

    int useCode = 0;
    bool stopped = false;
    string collName;

    Component[] components;

    private void Awake()
    {
        // для взаимодействий
        controller = GetComponent<CS_PlayerController>();
    }

    void OnGUI()
    {
        // использование switch-case сегмента невозможно
        if (Input.anyKeyDown)
        {
            var pushed = Event.current;
            if (pushed.isKey)
            {
                switch (pushed.keyCode)
                {
                    case KeyCode.E: // использование                      
                        switch (useCode)
                        {
                            // 1 = поднять призрака

                            case 1:
                                Object obj = GameObject.Find(collName); // поиск объекта по имени 
                                Animator spiritAnim = obj.GetComponent<Animator>();

                                spiritAnim.SetTrigger("Arised"); // сначала включает триггер
                                spiritAnim.SetInteger("SetSpirit", 1); // дальнейшая работа в CS_SpiritController.cs

                                StartCoroutine(player_use()); // корутина с таймингами анимации использования
                                break;
                        }
                        break;

                    case KeyCode.Escape: // пауза, остановить анимации
                        components = FindObjectsOfType<Animator>(); // получает все компоненты типа Animator на сцене

                        if (stopped)
                            PlayAnimations();
                        else
                            StopAnimations();
                        break;

                    default:
                        break;
                }
            }
            else if (pushed.isMouse)
            {

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "SpiritTrigger":
                useCode = 1;
                collName = collision.transform.parent.name; // получаем имя родительского объекта из триггера, в котором стоим
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "SpiritTrigger":
                useCode = 0;
                collName = null; // обнуляет имя объекта
                break;
        }
    }

    IEnumerator player_use()
    {
        // остановка игрока
        controller.speed = 0f;
        controller.used = true;
        controller.player_anim.SetBool(name: "isUse", value: true);

        yield return new WaitForSeconds(1.5f); // время обездвиживания

        // возвращение возможности двигаться
        controller.player_anim.SetBool(name: "isUse", value: false);
        controller.speed = controller.current_speed;
        controller.used = false;

        yield return null;
    }

    void StopAnimations()
    {
        stopped = true;
        Time.timeScale = 0f; // остановить время

        foreach (Component component in components) // перебирает для каждого
        {
            Animator animator = component as Animator;
            animator.speed = 0f;
        }

        // запрещает игроку двигаться и отключает его аниматор
        controller.speed = 0f;
        controller.player_anim.enabled = false;
    }

    void PlayAnimations()
    {
        stopped = false;
        Time.timeScale = 1f; // возобновить время

        foreach (Component component in components) // перебирает для каждого
        {
            Animator animator = component as Animator;
            animator.speed = 1f;
        }

        // разрешает игроку двигаться и включает его аниматор
        controller.speed = controller.current_speed;
        controller.player_anim.enabled = true;
    }
}
