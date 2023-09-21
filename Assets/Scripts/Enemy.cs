using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
