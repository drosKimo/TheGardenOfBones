using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CS_TriggersScript : MonoBehaviour
{
    [SerializeField] TMP_Text tips;
    [SerializeField] Tilemap gardens;
    [SerializeField] TileBase targetSprite;
    [SerializeField] GameObject SpeechBox;
    [SerializeField] Animator playerAnim;

    int tipCode = 0, // код подсказки
        countCode = 0, // вспомогательный код для триггеров
        amountGround = 0; // текущее количество тайлов;
    bool triggered = true;

    CS_PlantBehaviour plant;
    Animator plantAnim;

    void Update()
    {
        if(!GameObject.Find("PFB_Plant(Clone)").IsUnityNull())
        {
            plant = GameObject.Find("PFB_Plant(Clone)").GetComponent<CS_PlantBehaviour>();
            plantAnim = GameObject.Find("PFB_Plant(Clone)").GetComponent<Animator>();
        }

        // каждый кадр считает сколько на поле тайлов земли
        GetTileCount();

        if (countCode == 4 && !SpeechBox.activeSelf)
            tips.text = "Press E to interact";
        else if (countCode == 5 && !SpeechBox.activeSelf)
            tips.text = "Press LMB on the excavated ground to plant";
        else if (countCode == 6 && !SpeechBox.activeSelf)
            tips.text = "Go to the plant and press E";

        if ((countCode == 1 && amountGround == 2) ||
            (countCode == 2 && amountGround == 1) ||
            (countCode == 3 && !triggered) ||
            (countCode == 4 && playerAnim.GetBool("Guid")) ||
            (countCode == 5 && plant.timeLeft == 0) ||
            (countCode == 6 && plantAnim.GetInteger("StateCounter") == 1))
        {
            tips.text = "";
            countCode++;
            SpeechBox.SetActive(true);
        }
    }

    void OnGUI()
    {
        var pushed = Event.current;

        if (Input.anyKeyDown)
        {
            if (pushed.isKey)
            {
                switch (pushed.keyCode)
                {
                    case KeyCode.Space:
                        // 0 = отключить подсказку
                        // 1 = ожидание и выборка подсказок
                        // постоянное добавление кода нужно для того, чтобы следить за триггерами в диалоге

                        switch (tipCode)
                        {
                            case 0:
                                tips.text = "";
                                tipCode = 1;
                                break;

                            case 1:
                                if (countCode == 0)
                                {
                                    countCode = 1;
                                    tips.text = "Press LMB to dig up the ground";
                                }
                                else if (countCode == 2)
                                    tips.text = "Press RMB to bury the excavated ground";
                                
                                break;
                        }
                        break;
                }
            }
        }
    }

    public void GetTileCount()
    {
        amountGround = 0; // сброс количества тайлов

        BoundsInt bounds = gardens.cellBounds; // получает полные координаты тайловой карты
        foreach (Vector3Int pos in bounds.allPositionsWithin) // проверка каждого тайла
        {
            TileBase tile = gardens.GetTile<TileBase>(pos);

            // проверяет, пустой ли тайл, и совпадает ли с контрольным значением
            if (tile != null && tile == targetSprite)
            {
                amountGround++;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && countCode == 3)
        {
            triggered = false;
        }
    }
}
