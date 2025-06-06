﻿using UnityEngine;

public class PersistentMonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public static T Get()
	{
	    return instance;
	}

	public virtual void Awake()
	{
	    if (instance == null)
	    {
	        instance = this as T;
	        DontDestroyOnLoad(this);
	    }
	    else
	    {
	        Destroy(gameObject);
	    }
	}
}