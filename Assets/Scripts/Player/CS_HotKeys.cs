using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Menu
{
    public GameObject MenuPrefab;
}

public class CS_HotKeys : MonoBehaviour
{
    // Перепривязка клавиш:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    [SerializeField] public GameObject plant;
    [HideInInspector] public Object obj;

    CS_PlayerController controller;
    CS_SettingGround settingGround;
    CS_SpiritController spirit;
    public Menu menu = new Menu();

    int useCode = 0, // код для взаимодействия
        counterGround = 0, // счетчик тайлов земли
        counterSpirits = 0; // счетчик поднятых призраков
    bool stopped = false, moving = false;
    string collName;

    Component[] components;
    Animator spiritAnim, plantAnim;

    private void Awake()
    {
        controller = GetComponent<CS_PlayerController>(); // для взаимодействий

        GameObject ground = GameObject.Find("GroundTest");
        settingGround = ground.GetComponent<CS_SettingGround>(); // для садовой земли
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
                            // 2 = посадка (в землю); на мышь
                            // 3 = открытие двери (+ переход)
                            // 4 = ухаживать за садом

                            case 1:
                                if (counterGround > counterSpirits) // проверяет, есть ли на карте свободная земля для посадки
                                {
                                    counterSpirits++;
                                    useCode = 2;
                                    obj = GameObject.Find(collName); // поиск объекта по имени 
                                    spiritAnim = obj.GetComponent<Animator>();
                                    spirit = obj.GetComponent<CS_SpiritController>();

                                    spiritAnim.SetTrigger("Arised"); // сначала включает триггер
                                    spiritAnim.SetInteger("SetSpirit", 1); // дальнейшая работа в CS_SpiritController.cs

                                    StartCoroutine(player_use()); // корутина с таймингами анимации использования
                                }
                                else
                                {
                                    Debug.Log("Некуда посадить!");
                                }
                                break;
                            case 4:
                                plantAnim.SetInteger("StateCounter", plantAnim.GetInteger("StateCounter") + 1);
                                if (plantAnim.GetInteger("StateCounter") == 3)
                                {
                                    counterSpirits--;
                                    Destroy(plantAnim.gameObject, 1.5f);
                                }
                                break;
                        }
                        break;

                    case KeyCode.Tab: // пауза, остановить анимации
                        components = FindObjectsOfType<Animator>(); // получает все компоненты типа Animator на сцене

                        if (stopped)
                        {
                            menu.MenuPrefab.gameObject.SetActive(false);
                            PlayAnimations();
                        }
                        else
                        {
                            menu.MenuPrefab.gameObject.SetActive(true);
                            StopAnimations();
                        }
                        break;

                    case KeyCode.Escape:
                        Application.Quit();
                        break;
                    
                    case KeyCode.Q: // для дебага, чтобы знать координаты игрока
                        Debug.Log(gameObject.transform.position);
                        break;

                    case KeyCode.F: // перезагрузка сцены
                        SceneManager.LoadScene("Test Scene");
                        break;

                    default:
                        break;
                }
            }
            else if (pushed.isMouse)
            {
                settingGround.GetMousePosition(); // каждый раз, когда нажата мышь, получает её координаты на указанном тайлмапе (сейчас - земля для посадки)

                //обязательно нужно отдельно получить позицию мыши для проверки на наличие объектов, иначе работает неправильно
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                switch (pushed.button)
                {
                    case 0:
                        // строка settingGround.tilemap.GetTile(settingGround.tilePos) != null проверяет, есть ли под мышью тайл земли
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null && !stopped)
                        {
                            if (hit.collider.IsUnityNull() && useCode == 2) // первая проверка узнает, есть ли под ней коллайдер
                            {
                                StartCoroutine(spirit.GoToGround());
                                moving = spirit.moveToPoint;
                                StartCoroutine(player_use());

                                // обнуляем данные существа, с которым взаимодействовали
                                obj = null;
                                spirit = null;
                                spiritAnim = null;
                                useCode = 0;
                            }
                            else
                            {
                                Debug.Log("Занято");
                            }
                        }
                        else if (settingGround.tilemap.GetTile(settingGround.tilePos) == null && useCode != 2 && !stopped) // вторая проверка нужна для запрета размещения замли, пока мы ведем призрака
                        {
                            settingGround.tilemap.SetTile(settingGround.tilePos, settingGround.tile); // поставить тайл
                            counterGround++;
                        }
                        break;

                    case 1:
                        // удалить возможно только если под мышью есть тайл + запрет на удаление, пока:
                        // 1. За нами ходит призрак
                        // 2. Клетка уже занята растением или на нее идет призрак
                        
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null &&
                            settingGround.tilemap.GetTile(settingGround.tilePos).name == "RT_GardenGround" &&
                            useCode !=2 && hit.collider.IsUnityNull() && !moving)
                        {
                            settingGround.tilemap.SetTile(settingGround.tilePos, null); // удалить тайл
                            counterGround--;
                        }
                        break;

                    default:
                        break;
                }
                
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // хотя и вызывается каждый кадр нахождения в триггере, убирает баг, когда нельзя было
        // поднять другого призрака, если посадить предыдущего, находясь в радиусе триггера второго
        switch (collision.tag)
        {
            case "SpiritTrigger":
                if (useCode == 0)
                {
                    useCode = 1;
                    collName = collision.transform.parent.name; // получаем имя родительского объекта из триггера, в котором стоим
                }
                break;
            case "Garden":
                if (useCode == 0)
                {
                    useCode = 4;
                }
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Garden":
                plantAnim = collision.GetComponentInParent<Animator>();
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "SpiritTrigger":
                if (useCode == 1)
                {
                    useCode = 0;
                    collName = null; // обнуляет имя объекта
                }
                break;
            case "Garden":
                useCode = 0;
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

        yield return new WaitForSeconds(1.5f);
        moving = false; // чтобы можно было потом удалять землю

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
