using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D {
    public class Damageable : MonoBehaviour {
        [Serializable]
        public class HealthEvent : UnityEvent<Damageable> {
        }

        [Serializable]
        public class DamageEvent : UnityEvent<Damager, Damageable> {
        }

        [Serializable]
        public class HealEvent : UnityEvent<int, Damageable> {
        }

        public int startingHealth = 5;
        public bool invulnerableAfterDamage = true;
        public float invulnerabilityDuration = 3f;
        public bool disableOnDeath = false;
        public GameObject lootPrefab;

        [Tooltip(
            "An offset from the obejct position used to set from where the distance to the damager is computed")]
        public Vector2 centreOffset = new Vector2(0f, 1f);

        public HealthEvent OnHealthSet;
        public DamageEvent OnTakeDamage;
        public DamageEvent OnDie;
        public HealEvent OnGainHealth;

        protected bool m_Invulnerable;
        protected float m_InulnerabilityTimer;
        [SerializeField, ShowOnly]
        protected int m_CurrentHealth;
        protected Vector2 m_DamageDirection;
        protected bool m_ResetHealthOnSceneReload;
        protected bool noHealth;

        public int CurrentHealth {
            get { return m_CurrentHealth; }
        }

        void OnEnable() {
            if (startingHealth < 0) {
                noHealth = true;
                m_CurrentHealth = 10000;
            } else {
                m_CurrentHealth = startingHealth;
            }

            OnHealthSet.Invoke(this);

            DisableInvulnerability();
        }

        void Update() {
            if (m_Invulnerable) {
                m_InulnerabilityTimer -= Time.deltaTime;

                if (m_InulnerabilityTimer <= 0f) {
                    m_Invulnerable = false;
                }
            }
        }

        public void EnableInvulnerability(bool ignoreTimer = false) {
            m_Invulnerable = true;
            //technically don't ignore timer, just set it to an insanly big number. Allow to avoid to add more test & special case.
            m_InulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
        }

        public void DisableInvulnerability() {
            m_Invulnerable = false;
        }

        public Vector2 GetDamageDirection() {
            return m_DamageDirection;
        }

        public void TakeDamage(Damager damager, bool ignoreInvincible = false) {
            if ((m_Invulnerable && !ignoreInvincible) || m_CurrentHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            if (!m_Invulnerable && !noHealth) {
                m_CurrentHealth -= damager.damage;
                OnHealthSet.Invoke(this);
            }

            m_DamageDirection =
                transform.position + (Vector3) centreOffset - damager.transform.position;

            OnTakeDamage.Invoke(damager, this);
            if (invulnerableAfterDamage) {
                EnableInvulnerability();
            }

            if (m_CurrentHealth <= 0) {
                OnDie.Invoke(damager, this);
                m_ResetHealthOnSceneReload = true;
                EnableInvulnerability(true);
                if (disableOnDeath) this.enabled = false;
            }
        }

        public void GainHealth(int amount) {
            m_CurrentHealth += amount;

            if (m_CurrentHealth > startingHealth)
                m_CurrentHealth = startingHealth;

            OnHealthSet.Invoke(this);

            OnGainHealth.Invoke(amount, this);
        }

        public void SetHealth(int amount) {
            m_CurrentHealth = amount;

            OnHealthSet.Invoke(this);
        }


        // region custom extensions

        public void ApplyKnockback(Damager damager, Damageable damageable) {
            var thisRb = gameObject.GetComponent<Collider2D>().attachedRigidbody;
            if (thisRb) {
                var hitVec = damager.gameObject.transform.up;
                thisRb.AddForce(hitVec * damager.knockback, ForceMode2D.Impulse);
            }
        }

        public void InstantiateHere(GameObject prefab) {
            var go = Instantiate(prefab);
            go.transform.position = transform.position;
        }

        public void DropLoot(Damager damager, Damageable damageable) {
            if (damager.disableLoot || !lootPrefab) {
                return;
            }
            InstantiateHere(lootPrefab);
        }

        public void Blink(SpriteRenderer spriteRenderer) {
            StartCoroutine(_Blink(spriteRenderer));
        }

        private IEnumerator _Blink(SpriteRenderer spriteRenderer) {
            spriteRenderer.material.SetColor("_FlashColor", Color.white);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.material.SetColor("_FlashColor", Color.clear);
        }

        // endregion
    }
}