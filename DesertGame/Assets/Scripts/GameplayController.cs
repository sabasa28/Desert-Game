using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    static GameplayController instance = null;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this); //this o gameobject aca?
    }
    public static GameplayController Get()
    {
        return instance;
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
    public PlayerController GetCurrentPlayer()
    {
        return player;
    }
}
