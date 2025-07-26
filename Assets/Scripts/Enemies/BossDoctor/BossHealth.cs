using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Image fillImage;
    
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        fillImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            GetComponentInParent<MadDoctor>().iAmDead = true;
        }
    }

   
}