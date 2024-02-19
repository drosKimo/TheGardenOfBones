using System.Collections;
using UnityEngine;

public class CS_HotKeys : MonoBehaviour
{
    // Перепривязка клавиш:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    bool stopped = false;
    CS_PlayerController controller;

    [HideInInspector] public bool aaa; // тут долна быть буль для проверки нахождения в зоне взаимодействия

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
                    case KeyCode.F: // использование
                        StartCoroutine(player_use());
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
            if (pushed.isMouse)
            {

            }
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
