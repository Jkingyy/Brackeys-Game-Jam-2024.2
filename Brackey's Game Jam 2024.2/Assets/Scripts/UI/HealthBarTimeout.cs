using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarTimeout : MonoBehaviour
{
    [SerializeField] float timeout;
    private float timeoutTimer;
    public bool hasChanged;
    // Start is called before the first frame update
    void Start()
    {
        timeoutTimer = timeout;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasChanged){
            timeoutTimer = timeout;
            hasChanged = false;
        } else {
            timeoutTimer -= Time.deltaTime;
        }

        if(timeoutTimer < 0){
            gameObject.SetActive(false);
        }
    }
}
