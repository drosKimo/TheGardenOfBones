using System;
using UnityEngine;

public class CS_SpawnCreature : MonoBehaviour
{
    public GameObject creature;
    public int counterSpawned; // сколько всего заспавнилось волков за день

    GameObject wolf;
    Animator animPlayer;
    System.Random rand;

    void Start()
    {
        animPlayer = GameObject.Find("Player").GetComponent<Animator>();
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();

        sprite.enabled = false;
        rand = new System.Random();

        //StartCoroutine(Spawner()); // чуть более реалистично выгл€д€щий спавн. Ќеравномерный по времени
        Spawn();
    }

    public void Spawn()
    {
        if (rand.Next(0, 100) > 65)
        {
            counterSpawned = animPlayer.GetInteger("SpawnedCreatures") + 1;
            animPlayer.SetInteger("SpawnedCreatures", counterSpawned);

            wolf = Instantiate(creature, transform.position, Quaternion.identity);
            wolf.name = "Wolf" + Convert.ToString(counterSpawned);
        }
    }

    /*IEnumerator Spawner()
    {
        yield return new WaitForSeconds((float)rand.NextDouble());

        if (rand.Next(0, 100) > 65)
        {
            counterAnim = animPlayer.GetInteger("SpawnedCreatures") + 1;
            animPlayer.SetInteger("SpawnedCreatures", counterAnim);

            wolf = Instantiate(creature, transform.position, Quaternion.identity);
            wolf.name = "Wolf" + Convert.ToString(counterAnim);
        }
    }*/
}
