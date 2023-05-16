using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    Animator animator;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    SpriteRenderer spriteRender;
    Rigidbody2D rb;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    public float moveSpeed = 1f;
    public float collisionOffset = 0.01f;
    public float Health {
        set {
            _health = value;
            if (_health <= 0) animator.SetBool("isAlive", false);;
        }
        get { return _health; }
    }
    private float _health = 100;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // If player is close to an enemy, trigger sword attack
    private void Update() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (Collider2D collider in colliders) {
            if (collider.tag == "Enemy") {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.Health > 0) {
                    animator.SetTrigger("swordAttack");
                    break;
                }
            }
        }
    }

    private void FixedUpdate(){
        if (animator.GetBool("isAlive")) {
            // Get the mouse position in pixels
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            movementInput = worldPos - transform.position * moveSpeed;

            // Calculate the distance to the mouse position
            float distanceToMouse = Vector2.Distance(transform.position, worldPos);
            // Check if the distance is smaller than a threshold
            bool hasReachedTarget = distanceToMouse < 0.1f;

            // If movement input is not 0 try to move
            if (movementInput != Vector2.zero && !hasReachedTarget) {
                // if move fails 
                bool success = TryMove(movementInput);
                
                if (!success) {
                    //tries to move on x coordinates
                    success = TryMove(new Vector2(movementInput.x, 0));
                    if(!success){
                        //tries to move on y coordinates
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
                animator.SetBool("isMoving",success);
            } else {
                animator.SetBool("isMoving",false);
            }

            // Set direction of sprite to movement direction
            if(movementInput.x < 0){
                spriteRender.flipX = true;
            }else if(movementInput.x > 0){
                spriteRender.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction){
        if (direction != Vector2.zero) {
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset // the amount to cast equal to the movement plus an offset
            );
            // if there is no collisin, player will move
            if (count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    void OnFire() {
        print("Attack! Trying to deal damage.");
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        if (spriteRender.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void StopAttack() {
        swordAttack.StopAttack();
    }

    // // If player collides with an enemy trigger sword attack
    // public void OnCollisionEnter2D(Collision2D collision) {
    //     Collider2D other = collision.collider;

    //     if (other.tag == "Enemy") {
    //         Enemy enemy = other.GetComponent<Enemy>();
    //         if (enemy != null && enemy.Health > 0) animator.SetTrigger("swordAttack");
    //     }
    // }

    public void TakeDamage(float damage) {
        Health -= damage;
        print("Damage by enemy: " + damage);
        print("Your HP: " + Health + "/" + 100);
    }

    // void OnMove(InputValue movementValue){
    //     movementInput = movementValue.Get<Vector2>();

    // }
}

