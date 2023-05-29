using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public HealthBar healthBar;
    private float health;
    private float score=0;
    [SerializeField]  private float maxHealth=1000f;
    [SerializeField] private Text textScore;
    [SerializeField] private Text textHealth;
    public GameOver gameOver;
    public float Score { get => score; set => score = value; }

    private void Awake()
    {
        health = maxHealth;
        healthBar.UpdateHeathBar(maxHealth, health);
    }
    void Update()
    {
        healthBar.UpdateHeathBar(maxHealth, health);
        textHealth.text =" "+ health;
        textScore.text = "Score: "+Score;

    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHeathBar(maxHealth, health);
        if (health <= 0)
        {
           gameOver.ShowUI();
        };
    }
}
