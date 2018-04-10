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
        MessageLbl.text = "INIT";
        Profile.gameObject.SetActive(false);
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
        NativeManager.Instance.FaceBookLogin(()=> 
        {
            NativeManager.Instance.GetUserFirstName((userName) => 
            {
                MessageLbl.text = userName;
            });

            NativeManager.Instance.GetProfile((sprite) => 
            {
                Profile.gameObject.SetActive(true);
                Profile.sprite2D = sprite;
                Profile.MakePixelPerfect();
            });
        });
    }

    void OnClickFacebookLogoutBtn(GameObject sender)
    {
        NativeManager.Instance.FaceBookLogout(()=> 
        {
            MessageLbl.text = "Logout";
            Profile.gameObject.SetActive(false);
        });
    }
}
