using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kit;
using UnityEngine.UI;
using System;

public class UIManager :Singleton<UIManager>
{
    public Canvas UICanvas;
    public CanvasScaler UIScaler;
    public GameObject UICameraGo;
    public Camera UICamera;
    public RectTransform UIAdapterRoot;
    public RectTransform UIRoot;
    protected override void InitSingleton()
    {
        var uiViewTransform = GameObject.Find("UIView").transform;
        UIRoot = uiViewTransform.Find("UIContainer/UIRoot") as RectTransform;
        if (UIRoot.IsNotNull())
        {
            UICanvas = UIRoot.GetComponent<Canvas>();
            UIScaler = UIRoot.GetComponent<CanvasScaler>();

            UIAdapterRoot = UIRoot.transform.Find("UIAdapterRoot") as RectTransform;
        }
        var uiCameraTran = uiViewTransform.Find("UIContainer/UICamera");
        if (uiCameraTran.IsNotNull())
        {
            UICameraGo = uiCameraTran.gameObject;
            UICamera = UICameraGo.GetComponent<Camera>();
        }
        
    }
    public void OpenUI(string uiName,Action<GameObject> loadCallBacl=null)
    {
        AddressableManager.Instance.LoadGameObjectAsync(uiName, loadCallBacl, UIAdapterRoot);
    }
}
