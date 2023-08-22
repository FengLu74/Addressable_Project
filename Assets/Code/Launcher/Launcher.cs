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
        yield return HotUpdateManager.Instance.Launch();
        //yield return new WaitForSeconds(1f);
        
        UIManager.Instance.OpenUI("HomeWindow", (go) =>
        {
            Debug.Log("Load over HomeWindow !");
        });

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
