using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    // Start is called before the first frame update
    public void UpdateHeathBar(float maxHealth,float currentHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        _healthBar.fillAmount = fillAmount;
    }
}
