using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackScript : MonoBehaviour
{
    [SerializeField] private MovementScript movementScript;
    [SerializeField] private InputHandler InputHandler;
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackSphere;
    private bool canAttack = true;
    public bool IsAttacking { get; private set; } = false;

    private void Update()
    {
        if(InputHandler.PressedAttack && canAttack)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        IsAttacking = true;
        canAttack = false;
        yield return new WaitForSeconds(0.7f);
        IsAttacking = false;
        var enemies = Physics2D.OverlapCircleAll(attackSphere.position, attackRange);

/*        foreach(var enemy in enemies)
        {
            if(enemy.TryGetComponent(out Enemy enemyCharacter)) { yield return null; }
            if(enemyCharacter != null)
                Debug.Log("Enemy found");
            //enemyCharacter.TakeDamage(damage);
        }*/
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackSphere.position, attackRange);
    }
}
