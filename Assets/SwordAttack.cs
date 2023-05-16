using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour {
    Vector2 rightAttackOffset;
    Collider2D swordCollider;
    public float damage = 25;
    public float knockbackForce = 100f;

    private void Start() {
        swordCollider = GetComponent<Collider2D>();
        rightAttackOffset = transform.position;
    }

    public void AttackRight() {
        print("Attack right.");
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft() {
        print("Attack left.");
        swordCollider.enabled = true;
        transform.localPosition = new Vector2(-1 * rightAttackOffset.x, rightAttackOffset.y);
    }
    
    public void StopAttack() {
        swordCollider.enabled = false;
    }

    // If sword collides with an enemy give damage
    public void OnTriggerEnter2D(Collider2D other) {
        // Calculate knockback
        Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;

        print("enemy pos: " + other.gameObject.transform.position);

        Vector2 direction = (Vector2) (parentPos - other.gameObject.transform.position).normalized;
        Vector2 knockback = direction * knockbackForce;

        // Deal with enemy damage
        if (other.tag == "Enemy") {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null) {
                enemy.TakeDamage(damage, knockback);
            } 
        }
    }
}
