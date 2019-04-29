using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D {
    public class Damager : MonoBehaviour {
        [Serializable]
        public class DamagableEvent : UnityEvent<Damager, Damageable> {
        }
        [Serializable]
        public class NonDamagableEvent : UnityEvent<Damager> {
        }

        public int damage = 1;
        public float knockback;

        [Tooltip("If disabled, damager ignore trigger when casting for damage")]
        public bool canHitTriggers;
        public bool disableDamageAfterHit = false;
        public bool disableLoot = false;

        [Tooltip(
            "If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
        public bool ignoreInvincibility = false;

        public LayerMask hittableLayers;
        public DamagableEvent OnDamageableHit;
        public NonDamagableEvent OnNonDamageableHit;

        protected bool m_CanDamage = true;
        protected GameObject m_LastHit;

        public void EnableDamage() {
            m_CanDamage = true;
        }

        public void DisableDamage() {
            m_CanDamage = false;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            tryDamage(other.collider);
        }

        public void OnCollisionStay2D(Collision2D other) {
            tryDamage(other.collider);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            tryDamage(other);
        }

        public void OnTriggerStay2D(Collider2D other) {
            tryDamage(other);
        }

        private void tryDamage(Collider2D other) {
            if (!m_CanDamage) {
                return;
            }
            if (other.isTrigger && !canHitTriggers) {
                return;
            }
            if (hittableLayers != (hittableLayers | (1 << other.gameObject.layer))) {
                return;
            }

            m_LastHit = other.gameObject;
            Damageable damageable = other.gameObject.GetComponent<Damageable>();
            if (damageable) {
                if (!damageable.enabled) {
                    return;
                }
                OnDamageableHit.Invoke(this, damageable);
                damageable.TakeDamage(this, ignoreInvincibility);
                if (disableDamageAfterHit)
                    DisableDamage();
            } else {
                OnNonDamageableHit.Invoke(this);
            }
        }

        // region custom extensions

        public void DestroyYourself() {
            var sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr) {
                sr.enabled = false;
            }
            var c = gameObject.GetComponent<Collider2D>();
            if (c) {
                c.enabled = false;
            }
            var rb2d = gameObject.GetComponent<Rigidbody2D>();
            if (rb2d) {
                rb2d.simulated = false;
            }
            Destroy(gameObject, 0.5f);
        }

        // endregion

    }
}