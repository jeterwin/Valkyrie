using UnityEngine;
public class Springs : MonoBehaviour
{
    [SerializeField] private MovementScript movementScript;
    [SerializeField] private Animator animator;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float jumpStrength;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) { return; }
        
        movementScript.IsOnPad = true;
        movementScript.JumpPadStrength = jumpStrength;
        animator.Play("Work");
        //movementScript.Rigidbody.velocity = Vector2.zero;
        movementScript.Rigidbody.AddForce(jumpStrength * Vector2.up, ForceMode2D.Impulse);
    }
}
