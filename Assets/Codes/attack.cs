using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damageAble damageable = collision.GetComponent<damageAble>();

        if (damageable != null)
        {
            Vector2 deliveredknockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            bool gotHit = damageable.Hit(attackDamage, deliveredknockback);

            if (gotHit)
                Debug.Log(collision.name + "hit for " + attackDamage);
        }
    }
}
