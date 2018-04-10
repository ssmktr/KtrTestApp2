using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TinyJSON;

public class MainPanel : MonoBehaviour {

    public GameObject FacebookLoginBtn, FacebookLogoutBtn;
    public UI2DSprite Profile;
    public UILabel MessageLbl;

    private void Awake()
    {
        UIEventListener.Get(FacebookLoginBtn).onClick = OnClickFacebookLoginBtn;
        UIEventListener.Get(FacebookLogoutBtn).onClick = OnClickFacebookLogoutBtn;

        NativeManager.Instance.Init();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnClickFacebookLoginBtn(GameObject sender)
    {
    }

    void OnClickFacebookLogoutBtn(GameObject sender)
    {
    }
}
