using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collection : MonoBehaviour
{
    [SerializeField] TMP_Text _collectionText;
    [SerializeField] AudioClip _collectionFX;


    private int collectionCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Collect")){
            collectionCount++;
            UpdateUI();
            Destroy(other.gameObject);
            SoundFXManager.Instance.PlaySoundFXClipAtRandomPitch(_collectionFX, transform, 0.25f, 0.25f);
        }
    }

    private void UpdateUI(){
        _collectionText.text = collectionCount.ToString();
    }
}
