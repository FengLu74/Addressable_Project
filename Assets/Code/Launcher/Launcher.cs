using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowUI());
    }
    IEnumerator ShowUI()
    {
        Debug.Log("Start!");
        yield return new WaitForSeconds(1f);


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}