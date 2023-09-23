using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    static DeathScript Instance;
    [SerializeField] private Animator animator;
    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void ResetState()
    {
        Debug.Log("Reseted");
    }
}
