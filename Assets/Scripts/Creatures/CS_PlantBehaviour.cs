using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PlantBehaviour : MonoBehaviour
{
    CS_SettingGround settingGround;

    private void Awake()
    {
        GameObject ground = GameObject.Find("GroundTest");
        settingGround = ground.GetComponent<CS_SettingGround>(); // для садовой земли
    }
}
