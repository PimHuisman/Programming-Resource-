using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    [SerializeField] float healthFallRate;

    public Slider hungerSlider;
    public int maxHunger;
    [SerializeField] float hungerFallRate;

    public Slider thirstSlider;
    public int maxThirst;
    [SerializeField] float thirstFallRate;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        hungerSlider.maxValue = maxHunger;
        hungerSlider.value = maxHunger;

        thirstSlider.maxValue = maxThirst;
        thirstSlider.value = maxThirst;
    }
    void Update()
    {
        CheckHealth();
    }

    void CheckHealth()
    {
        if (hungerSlider.value <= 0 && thirstSlider.value <= 0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }

        else if (hungerSlider.value <= 0 || thirstSlider.value <= 0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }

        #region//HUNGER CONTROLLER
        if (hungerSlider.value > 0)
        {
            hungerSlider.value -= Time.deltaTime / hungerFallRate;
        }
        else if (hungerSlider.value <= 0)
        {
            hungerSlider.value = 0;
        }
        else if (hungerSlider.value >= maxHunger)
        {
            hungerSlider.value = maxHunger;
        }
        #endregion

        #region//THIRST CONTROLLER
        if (thirstSlider.value > 0)
        {
            thirstSlider.value -= Time.deltaTime / thirstFallRate;
        }
        else if (thirstSlider.value <= 0)
        {
            thirstSlider.value = 0;
        }
        else if (thirstSlider.value >= maxThirst)
        {
            thirstSlider.value = maxThirst;
        }
        #endregion

        if (healthSlider.value <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        anim.SetBool("Death", true);
        GetComponent<NPC>().newPosition = transform.position;
        GetComponent<NPC>().dead = true;
    }
}
