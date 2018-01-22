using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public float maxHealth;
    public float maxShield;

    public float shieldRegenTimeout;
    public float shieldRegenRate;
    float lastHitTime;

    public GameObject deathFx;
    public AudioClip hurtNoise;
    AudioSource playerASS;

    float currentHealth;
    float currentShield;

    PlayerController controller;

    // HUD
    public Slider healthBar;
    public Slider shieldBar;
    public Image damagedEffect;
    Color damagedColor = new Color(0f, 0f, 0f, .5f);
    float smoothColor = 5f;

    bool damaged = false;

    // Use this for initialization
    void Start() {
        controller = GetComponent<PlayerController>();
        playerASS = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        currentShield = maxShield;

        healthBar.maxValue = maxHealth;
        shieldBar.maxValue = maxShield;
        updateHealthGUI();
    }

    // Update is called once per frame
    void Update() {
        if (currentShield < maxShield) {
            if (Time.time - lastHitTime > shieldRegenTimeout) {
                currentShield += shieldRegenRate * Time.deltaTime;
                if (currentShield > maxShield) {
                    currentShield = maxShield;
                } 
                updateHealthGUI();
            }
        }

        if (damaged) {
            damagedEffect.color = damagedColor;
        } else {
            damagedEffect.color = Color.Lerp(damagedEffect.color, Color.clear, smoothColor * Time.deltaTime);
        }
        damaged = false;
    }

    public void doDamage(float damage) {
        lastHitTime = Time.time;
        if (currentShield > damage) {
            currentShield -= damage;
        } else {
            currentHealth -= (damage - currentShield);
            currentShield = 0;
            

            damaged = true;
            updateHealthGUI();
            if (currentHealth <= 0) {
                kill();
            }
        }
        playerASS.clip = hurtNoise;
        playerASS.Play();
        updateHealthGUI();

        //if (damage > 0) {
        //    currentHealth -= damage;
        //    playerASS.clip = hurtNoise;
        //    playerASS.Play();
        //    damaged = true;
        //    updateHealthGUI();
        //    if (currentHealth <= 0) {
        //        kill();
        //    }
        //}
    }

    public void heal(float amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        updateHealthGUI();
    }

    public void updateHealthGUI() {
        healthBar.value = currentHealth;
        shieldBar.value = currentShield;
    }

    public void kill() {
        Instantiate(deathFx, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
