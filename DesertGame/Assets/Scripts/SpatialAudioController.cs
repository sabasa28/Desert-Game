using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAudioController : MonoBehaviour
{
    [SerializeField] List<AIEnemy> closeEnemies = new List<AIEnemy>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            closeEnemies.Add(collision.GetComponent<AIEnemy>());
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) closeEnemies.Remove(collision.GetComponent<AIEnemy>());
    }

    void MakeSound()//agregar parametros
    { 
        
        foreach (AIEnemy enemy in closeEnemies)
        { 
            enemy.TryListenToPlayer(transform.position,50);
        }
    }
}
