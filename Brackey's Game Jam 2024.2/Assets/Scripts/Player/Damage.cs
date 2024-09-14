using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] int MaxHealth = 4;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] float HitStopTime;
    [SerializeField] float InvincibilityTime;
    [SerializeField] float CameraShakeTime;
    [SerializeField] float VerticalKnockback;
    [SerializeField] float HorizontalKnockback;
    [SerializeField] AudioClip[] DamageSounds;

    private int currentHealth;

    private float invincibilityTimeCounter;
    private bool isWaiting;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Knockback knockback;
    private CameraShake cameraShake;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        knockback = GetComponent<Knockback>();

        cameraShake = GameObject.FindGameObjectWithTag("Virtual Camera").GetComponent<CameraShake>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        invincibilityTimeCounter -= Time.deltaTime;
    }
    public void TakeDamage(int damage, Vector2 direction){
        if(invincibilityTimeCounter > 0)return;

        currentHealth -= damage;
        SoundFXManager.Instance.PlayRandomSoundFXClip(DamageSounds,transform,1f);
        if (currentHealth <= 0){
            GameOver();
        }

        ApplyKnockback(direction);
    }


    private void ApplyKnockback(Vector2 direction){
        if(isWaiting) return;
        //do Knockback
        //Stop player inputs


        

        //start InvincibilityTime
        invincibilityTimeCounter = InvincibilityTime;

        IEnumerator coroutine = HitStop(direction);
        StartCoroutine(coroutine);
    }

    IEnumerator HitStop(Vector2 direction){
        isWaiting = true;
        Time.timeScale = 0;
        
        yield return new WaitForSecondsRealtime(HitStopTime);
        isWaiting = false;
        Time.timeScale = 1;  

        cameraShake.ShakeCamera(HitStopTime);

        if(direction.x < 0){
            knockback.CallKnockback(direction, Vector2.left, 0);   

        } else if(direction.x > 0){
            knockback.CallKnockback(direction, Vector2.right, 0);   
        } else {
            knockback.CallKnockback(direction, Vector2.up, 0);   
        }
        
    }

    void GameOver(){
        Time.timeScale = 0;
        GameOverPanel.SetActive(true);
    }
}
