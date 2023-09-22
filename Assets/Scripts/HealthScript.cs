using System;
using System.Collections;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public static HealthScript Instance;
    public GameObject LastSaveObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    private void Awake()
    {
        Instance = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            StartCoroutine(ResetStates());
        }
        if(collision.CompareTag("Checkpoint"))
        {
            LastSaveObject = collision.gameObject;
            collision.gameObject.SetActive(false);
        }
    }

    IEnumerator ResetStates()
    {
        animator.SetTrigger("OnDeath");
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1f);
        Transform[] gameObjects = LastSaveObject.GetComponentsInChildren<Transform>(true);
        foreach(Transform transform in gameObjects)
        {
            transform.gameObject.SetActive(true);
        }
        yield return null;
    }
}
