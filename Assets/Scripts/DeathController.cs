using System;
using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using Kryz.CharacterStats;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeathController : MonoBehaviour {

    private static readonly int ANIM_ATTACK = Animator.StringToHash("attack");
    private static readonly int ANIM_RUNNING = Animator.StringToHash("running");

    public bool canAttack = true;
    public GameObject shopMenu;

    public CharacterStat moveSpeed = new CharacterStat(20);
    public CharacterStat bulletSpeed = new CharacterStat(5);
    public CharacterStat reloadAmount = new CharacterStat(4, true);
    public CharacterStat maxAmmoAmount = new CharacterStat(6, true);
    public CharacterStat rangedCooldown = new CharacterStat(0.5f);

    public float minorSouls {
        get => _minorSouls;
        set {
            _minorSouls = value;
            UpdateStatText();
        }
    }

    public void UpdateStatText() {
        statText.SetText(minorSouls, moveSpeed.Value,
            reloadAmount.Value, maxAmmoAmount.Value, rangedCooldown.Value);
    }

    public SmartText statText;

    public Damager meleeDamager;
    public GameObject bulletPrefab;
    public GameObject enableOnDeath;

    private Camera cam;
    private Rigidbody2D rb;
    public new SpriteRenderer renderer;
    private Animator animator;
    private new ParticleSystem particleSystem;
    private ParticleSystemRenderer particleSystemRenderer;
    private ParticleSystem attackParticleSystem;
    private Orbital orbital;
    private float nextRangedAttack;

    [Header("Backing Fields")]
    private float _minorSouls;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip onTakeDamage;
    public AudioClip onReload;
    public AudioClip onDie;
    public AudioClip onPickup;
    public AudioClip onEmptyClip;
    public AudioClip onShoot;
    public bool inputEnabled = true;
    private bool dead;

    void Start() {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystemRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        Assert.IsNotNull(statText);
        minorSouls = 10;
        orbital = GetComponentInChildren<Orbital>();
        audioSource = GetComponent<AudioSource>();

//        StartCoroutine(UpdateStatTextAsync());
    }

    private IEnumerator UpdateStatTextAsync() {
        // oh my god, this is so ugly, but i cant bother with doing this the nice way for now...
        while (true) {
            UpdateStatText();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    void Update() {
        var moveVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveVec = moveVec.normalized * moveSpeed;
        var moving = moveVec != Vector2.zero;
        if (moving) {
            rb.AddForce(moveVec, ForceMode2D.Force);
        }
        animator.SetBool(ANIM_RUNNING, rb.velocity.sqrMagnitude > 0.1f);

        var mouseInWorld = cam.ScreenToWorldPoint(Input.mousePosition).to2D();
        var aimVec = mouseInWorld - rb.position;
        var flipX = aimVec.x < 0;
        renderer.flipX = flipX;
        var particleSystemShape = particleSystem.shape;
        var shapeScale = particleSystemShape.scale;
        shapeScale.x = flipX ? -1f : 1f;
        particleSystemShape.scale = shapeScale;

        if (inputEnabled && Input.GetButtonDown("Fire2")) {
            StartCoroutine(reload());
        }

        if (inputEnabled && Input.GetButtonDown("Fire1") && Time.time > nextRangedAttack) {
            rangedAttack(mouseInWorld);
            nextRangedAttack = Time.time + rangedCooldown;
        }
//        if (Input.GetButtonDown("Fire2")) {
//            StartCoroutine(meleeAttack());
//        }

        if (minorSouls <= 0 && !dead) {
            // DIE!!!
            StartCoroutine(Die());
        }
    }

    private IEnumerator reload() {
        var curAmmo = orbital.transform.childCount;
        var ammoAmount = (reloadAmount + curAmmo) > maxAmmoAmount
            ? maxAmmoAmount - curAmmo
            : reloadAmount;

        if (ammoAmount > 0) {
            minorSouls--;
            for (int i = 0; i < ammoAmount; i++) {
                var newBullet = Instantiate(bulletPrefab, orbital.transform);
                newBullet.transform.position = transform.position;
                audioSource.PlayOneShot(onReload);
                yield return new WaitForSeconds(0.1f);
            }

        } else {
            audioSource.PlayOneShot(onEmptyClip);
        }
    }

    private IEnumerator Die() {
        dead = true;
        animator.SetBool("death", true);
        audioSource.PlayOneShot(onDie);
        var coll = GetComponent<CircleCollider2D>();
        if (coll) {
            coll.enabled = false;
        }
        yield return new WaitForSeconds(1f);
        particleSystem.Stop();
        enableOnDeath.SetActive(true);
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        SceneManager.LoadScene(Scenes.MAIN_MENU);
    }

    private void rangedAttack(Vector2 mouseInWorld) {
        if (orbital.transform.childCount <= 0) {
            audioSource.PlayOneShot(onEmptyClip);
            return;
        }
        audioSource.PlayOneShot(onShoot);
        animator.SetTrigger(ANIM_ATTACK);
//        yield return new WaitForSeconds(0.3f);
//        var newBullet = Instantiate(bulletPrefab);
//        newBullet.transform.position = bulletStartLocation.position;
//        newBullet.transform.up = -aimVec;
        if (orbital.transform.childCount <= 0) {
            return;
        }
        var newBullet = orbital.transform.GetChild(0);
        if (!newBullet) {
            return;
        }
        newBullet.SetParent(null, true);
        var aim = (mouseInWorld - newBullet.position.to2D()).normalized;
        newBullet.up = aim;
        var bulletRb = newBullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(aim * bulletSpeed, ForceMode2D.Impulse);
        bulletRb.freezeRotation = true;
    }

    private IEnumerator meleeAttack() {
        animator.SetTrigger(ANIM_ATTACK);
        yield return new WaitForSeconds(0.3f);
        meleeDamager.gameObject.SetActive(false);
        meleeDamager.gameObject.SetActive(true);
    }

    public void TakeDamage() {
        StartCoroutine(TakeDamageLater());
    }

    private IEnumerator TakeDamageLater() {
        minorSouls--;
        audioSource.PlayOneShot(onTakeDamage);
        enableOnDeath.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        enableOnDeath.SetActive(false);
    }

    public void PickupSoul() {
        minorSouls++;
        audioSource.PlayOneShot(onPickup);
    }
}
