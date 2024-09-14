using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] int MaxHealth = 4;
    [SerializeField] GameObject GameOverPanel;
    private int currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentHealth);
    }
    public void TakeDamage(int damage, Vector2 damageOrigin){
        currentHealth--;

        if (currentHealth <= 0){
            GameOver();
        }
    }


    void GameOver(){
        Time.timeScale = 0;
        GameOverPanel.SetActive(true);
    }
}
