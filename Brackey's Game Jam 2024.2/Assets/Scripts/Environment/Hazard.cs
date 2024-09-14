using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int Damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "Player"){
            Damage player = other.gameObject.GetComponent<Damage>();
            if(player != null){
                player.TakeDamage(Damage, GetHitDirection(player.transform.position));
            }
        }
    }

    private Vector2 GetHitDirection(Vector2 playerPosition){
        return (playerPosition - (Vector2)transform.position).normalized;
    }
}
