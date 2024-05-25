using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Menu
{
    public GameObject MenuPrefab;
    public GameObject SpeechPrefab;
}

public class CS_HotKeys : MonoBehaviour
{
    // Перепривязка клавиш:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    [SerializeField] public GameObject plant;
    [SerializeField] TMP_Text daytimeText;
    [HideInInspector] public Object obj;

    CS_PlayerController controller;
    CS_SettingGround settingGround;
    CS_SpiritController spirit;
    CS_DaytimeTimer daytimeTimer;
    CS_SpeechBoxScript boxScript;
    CS_SpawnCreature spawnCreature;
    public Menu menu = new Menu();

    int useCode = 0, // код для взаимодействия
        speechCode = 0, // код диалога
        counterGround = 0, // счетчик тайлов земли
        counterSpirits = 0, // счетчик поднятых призраков (прямо сейчас)
        counterHappySpirits = 0, // счетчик полных циклов у растений
        counterTotalSpirits = 0, // счетчик сколько ВСЕГО было поднято и посажено призраков
        counterDays = 0; // счетчик дней 
    bool stopped = false, moving = false;
    string collName;
    float finalPercent;

    Component[] components;
    Animator spiritAnim, plantAnim, playerAnim;
    TMP_Text textSpeech;
    public List<Sprite> dayKeys = new List<Sprite>(); // список спрайтов дней

    private void Awake()
    {
        controller = GetComponent<CS_PlayerController>(); // для взаимодействий

        GameObject ground = GameObject.Find("GroundTest");

        settingGround = ground.GetComponent<CS_SettingGround>(); // для садовой земли
        playerAnim = controller.gameObject.GetComponent<Animator>();
        daytimeTimer = GameObject.Find("Slider").GetComponent<CS_DaytimeTimer>();
        boxScript = GameObject.Find("SpeechBoxScripter").GetComponent<CS_SpeechBoxScript>(); // для работы с временем
        textSpeech = GameObject.Find("SP_text").GetComponent<TMP_Text>(); // для работы с текстом в диалоговом окне

        GameObject speech = GameObject.Find("Speech");
        
        if (SceneManager.GetActiveScene().name != "Tutorial")
            speech.SetActive(false);
    }

    void OnGUI()
    {
        if (playerAnim.GetInteger("AngrySpirit") == 1)
        {
            counterSpirits--;
            playerAnim.SetInteger("AngrySpirit", 0);
        }

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
                            // 3 = поговорить
                            // 4 = ухаживать за садом

                            case 1:
                                // проверяет, есть ли на карте свободная земля для посадки
                                // запрещает поднимать призраков, если рабочее время прошло 
                                if (counterGround > counterSpirits && daytimeText.text == "work time") 
                                {
                                    counterSpirits++;
                                    counterTotalSpirits++;
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

                            case 3:
                                if (daytimeTimer.timeLeft > 0)
                                {
                                    menu.SpeechPrefab.SetActive(true);
                                    Time.timeScale = 0f; // остановить время
                                    speechCode = 0;

                                    textSpeech.text = "Что такое? Солнце ещё высоко! Возвращайся к работе";
                                }
                                else
                                {
                                    menu.SpeechPrefab.SetActive(true);
                                    speechCode = 1;
                                    Time.timeScale = 0f; // остановить время

                                    textSpeech.text = "Уже так поздно? А я и не заметил!";
                                }
                                break;

                            case 4:
                                // Получает доступ к объекту-растению
                                // plantAnim = компонент-аниматор, так что сначала нужно обратиться к его игровому объекту
                                CS_PlantBehaviour plantBH = plantAnim.gameObject.GetComponent<CS_PlantBehaviour>();
                                if (plantBH.timeLeft <= 0 && daytimeTimer.timeLeft > 0) // когда таймер вышел и день все еще идет
                                {
                                    plantBH.animHelp.SetTrigger("Left"); // убирает пометку "помощь" над растением

                                    plantAnim.SetBool("TimeRun", true); // разрешает снова запустить таймер
                                    plantAnim.SetInteger("StateCounter", plantAnim.GetInteger("StateCounter") + 1); // добавляет стадию роста
                                    if (plantAnim.GetInteger("StateCounter") == 3)
                                    {
                                        counterSpirits--;
                                        counterHappySpirits++;
                                        Destroy(plantAnim.gameObject, 1.5f);
                                    }
                                }
                                break;
                        }
                        break;

                    case KeyCode.Space: // закрыть/переключить диалоговое окно
                        if (SceneManager.GetActiveScene().name == "Tutorial")
                        {
                            switch (speechCode)
                            {
                                // 0 = основы
                                // 1 = закрыть диалог
                                // 2 = проверка диалога
                                // 3 = подойти к зверьку
                                // 4 = о посадке
                                // 5 = о готовности
                                // 6 = конец обучения

                                case 0:
                                    textSpeech.text = "Давай начнём с основ. Вскопай землю дважды";
                                    speechCode = 1;
                                    break;

                                case 1:
                                    menu.SpeechPrefab.SetActive(false);
                                    speechCode = 2;
                                    break;

                                case 2:
                                    switch (boxScript.counter)
                                    {
                                        case 3:
                                            textSpeech.text = "Надеюсь, тебя не нужно учить ходить";
                                            speechCode = 3;
                                            break;
                                        case 5:
                                            textSpeech.text = "Но ты не сможешь работать пока он это делает...";
                                            speechCode = 4;
                                            break;
                                        case 7:
                                            textSpeech.text = "Растение исчезнет, если ты не будешь ухаживать за ним";
                                            speechCode = 5;
                                            break;
                                        default:
                                            menu.SpeechPrefab.SetActive(false);
                                            break;
                                    }
                                    break;

                                case 3:
                                    textSpeech.text = "Итак! Видишь этого зверька рядом? Подойди к нему";
                                    speechCode = 1;
                                    break;

                                case 4:
                                    textSpeech.text = "Попробуй посадить его";
                                    speechCode = 1;
                                    break;

                                case 5:
                                    textSpeech.text = "Ну вот и всё, теперь ты готов к работе!";
                                    speechCode = 6;
                                    break;

                                case 6:
                                    SceneManager.LoadScene("MainScene");
                                    break;
                            }
                        }
                        else
                        {
                            switch (speechCode)
                            {
                                // 0 = закрыть диалог
                                // 1 = продолжение диалога
                                // 2 = смена дня
                                // 3 = выход из игры
                                // 4 = результат выработки (реакция)
                                // 5 = результат садоводства (реакция)

                                case 0: // день еще не прошел
                                    menu.SpeechPrefab.SetActive(false);
                                    Time.timeScale = 1f;
                                    break; 

                                case 1:
                                    if (counterDays < 6) // если дневное время вышло и прошло меньше 7 дней
                                    {
                                        textSpeech.text = "Ладно, за работу!";
                                        Time.timeScale = 1f;
                                        speechCode = 2;

                                        // все триггеры объектов-призраков под удаление
                                        GameObject[] wolf = new GameObject[GameObject.FindGameObjectsWithTag("SpiritTrigger").Length];
                                        wolf = GameObject.FindGameObjectsWithTag("SpiritTrigger");

                                        foreach (GameObject sp in wolf)
                                        {
                                            // находит каждого волка и удаляет
                                            Destroy(sp.transform.parent.gameObject);
                                        }

                                        // а затем спавнит новых
                                        GameObject[] spawn = new GameObject[GameObject.FindGameObjectsWithTag("Respawn").Length];
                                        spawn = GameObject.FindGameObjectsWithTag("Respawn");

                                        foreach(GameObject sp in spawn)
                                        {
                                            spawnCreature = sp.GetComponent<CS_SpawnCreature>();
                                            spawnCreature.Spawn();
                                        }
                                    }
                                    else if (counterDays == 6) // конец игры
                                    {
                                        // НЕ РАБОТАЕТ С DOUBLE
                                        // осталось посчитать выработку игрока
                                        finalPercent = ((float)counterTotalSpirits / (float)playerAnim.GetInteger("SpawnedCreatures")) * 100;
                                        finalPercent = (float)System.Math.Round(finalPercent, 2); // округление до 2 знаков после запятой

                                        textSpeech.text = $"Ты достиг выработки в {finalPercent}%";
                                        speechCode = 4;
                                    }
                                    break;

                                case 2:
                                    menu.SpeechPrefab.SetActive(false);

                                    Image dayImg = GameObject.Find("DaytimeImage").GetComponent<Image>();

                                    counterDays++;
                                    dayImg.sprite = dayKeys[counterDays];
                                    daytimeText.text = "work time";

                                    StartCoroutine(daytimeTimer.StartTimer()); // перезапуск таймера дня
                                    break;

                                case 3:
                                    Application.Quit();
                                    break;

                                case 4:
                                    if (finalPercent < 25) // 0 - 24.99 %
                                        textSpeech.text = "Я правда должен комментировать это?..";
                                    else if (finalPercent >= 25 && finalPercent < 50) // 25 - 49.99 %
                                        textSpeech.text = "Могло быть и лучше...";
                                    else if (finalPercent >= 50 && finalPercent < 75) // 50 - 74.99 %
                                        textSpeech.text = "Неплохо.";
                                    else if (finalPercent >= 75 && finalPercent < 100) // 75 - 99.99 %
                                        textSpeech.text = "Отличная работа!";
                                    else if (finalPercent == 100) // 100 %
                                        textSpeech.text = "Ничего себе! Ты успел(а) даже больше, чем могу я сам!";

                                    speechCode = 5;
                                    break;

                                case 5:
                                    float happyPercent = ((float)counterHappySpirits / (float)counterTotalSpirits) * 100;

                                    if (counterTotalSpirits - counterHappySpirits > 0)
                                    {
                                        if (happyPercent == 0) // 0 %
                                            textSpeech.text = "Это ведь просто ужас! Проваливай!";
                                        else if (happyPercent < 45) // 0.01 - 44.99 %
                                            textSpeech.text = $"Бедные души... Из {counterTotalSpirits} очистилось всего {counterHappySpirits}. Прошу, уйди.";
                                        else if (happyPercent >= 45 && happyPercent < 75) // 45 - 74.99 %
                                            textSpeech.text = $"По крайней мере, ты старался(ась). Из {counterTotalSpirits} очистилось только {counterHappySpirits}. Можешь идти.";
                                        else if (happyPercent >= 75 && happyPercent < 100) // 75 - 99.99 %
                                            textSpeech.text = $"Чудненько! Ты смог спасти целых {counterHappySpirits} душ из {counterTotalSpirits}. Спасибо, прощай!";
                                    }
                                    else if (counterTotalSpirits == 0)
                                        textSpeech.text = "Ты даже не попытался(ась)! Проваливай!";
                                    else
                                        textSpeech.text = "Все души, которые ты посадил, спаслись. Спасибо, прощай!";

                                    speechCode = 3;
                                    break;
                            }
                        }
                        break;

                    case KeyCode.Escape: // пауза, остановить анимации

                        // Специально для туториала, свойства немного другие
                        if (SceneManager.GetActiveScene().name == "Tutorial")
                        {
                            SceneManager.LoadScene("MainScene");
                        }
                        else
                        {
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
                        }
                        break;

                    case KeyCode.L: // пропуск дня для дебага
                        daytimeTimer.timeLeft = 0;
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
                        else if (hit.collider.IsUnityNull() && settingGround.tilemap.GetTile(settingGround.tilePos) == null && useCode != 2 && !stopped) // вторая проверка нужна для запрета размещения замли, пока мы ведем призрака
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

            case "Reaper":
                if (useCode == 0)
                {
                    useCode = 3;
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

            case "Reaper":
                if (playerAnim.GetBool("Guid") == false)
                    useCode = 0;
                else
                    useCode = 2;
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
