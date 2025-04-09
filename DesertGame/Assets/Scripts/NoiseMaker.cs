using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    [SerializeField] string ListenerTag;
    [SerializeField] List<AIEnemy> enemies = new List<AIEnemy>(); //si hay algo mas que atrape sonido que no sea un enemigo se puede cambiar por una interface INoiseCatcher y checkear por interface en vez de tag
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ListenerTag))
        {
            AIEnemy enemy = collision.gameObject.GetComponent<AIEnemy>();
            if (enemy)
            {
                enemies.Add(enemy);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ListenerTag))
        {
            AIEnemy enemy = collision.gameObject.GetComponent<AIEnemy>();
            if (enemy)
            {
                enemies.Remove(enemy);
            }
        }
    }
    public void AlertCloseNoiseCatchers(Noise noise)
    {
        foreach (AIEnemy enemy in enemies)
        {
            enemy.ReceiveNoise(noise);
        }
    }


}
