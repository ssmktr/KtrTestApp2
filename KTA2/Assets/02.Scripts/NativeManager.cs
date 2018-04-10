using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Facebook.Unity;

public class NativeManager : Singleton<NativeManager>
{
    public void Init()
    {
        FB.Init(FaceBookInit, OnHideUnity);
    }

    #region FACEBOOK
    System.Action FaceBookLoginCallBack = null;
    public void FaceBookLogin(System.Action faceBookLoginCallBack)
    {
        FaceBookLoginCallBack = faceBookLoginCallBack;
        
    }

    void FaceBookInit()
    {
        if (FB.IsLoggedIn)
        {

        }
        else
        {
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (isGameShown)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
        }
        else
        {
        }
    }

    #endregion // FACEBOOK
}
