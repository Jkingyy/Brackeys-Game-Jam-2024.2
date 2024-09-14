using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float KnockbackTime = 0.2f;
    public float hitDirectionForce = 10f;
    public float constForce = 5f;
    public float inputForce = 5f;
    public AnimationCurve knockbackForceCurve;
    float time = 0;

    private Rigidbody2D rb;

    private Coroutine knockbackCoroutine;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsBeingKnockedBack {get; private set;}

    public IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection){
        IsBeingKnockedBack = true;

        Vector2 _hitforce;
        Vector2 _constantForce;
        Vector2 _knockbackForce;
        Vector2 _combinedForce;

        
        _constantForce = constantForceDirection * constForce;
        

        float _elapsedTime = 0f;
        while(_elapsedTime < KnockbackTime){
            _elapsedTime += Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;

            _hitforce = hitDirection * hitDirectionForce;

            _knockbackForce = _hitforce + _constantForce;

            if(inputDirection != 0){
                _combinedForce = _knockbackForce + new Vector2(inputDirection * inputForce, 0);
            } else {
                _combinedForce = _knockbackForce;
            }

            rb.velocity = _combinedForce;

            yield return new WaitForFixedUpdate();
        }

        IsBeingKnockedBack = false;
    }

    public void CallKnockback(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection){
        knockbackCoroutine = StartCoroutine(KnockbackAction(hitDirection,constantForceDirection,inputDirection));
    }
}
