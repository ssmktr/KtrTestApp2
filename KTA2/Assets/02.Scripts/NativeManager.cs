using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Facebook.Unity;

public class NativeManager : Singleton<NativeManager>
{
    public void Init()
    {
        FaceBookInit();
    }

    #region FACEBOOK
    System.Action FaceBookLoginCallBack = null;
    System.Action<string> GetFaceBookFirstNameResult = null;
    System.Action<Sprite> GetFaceBookProfileResult = null;

    void FaceBookInit()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(FaceBookInitCallBack, OnHideUnity);
        }
    }

    void FaceBookInitCallBack()
    {
        if (FB.IsInitialized)
            FB.ActivateApp();
        else
            Debug.Log("Failed to Initialize the Facebook SDK");
    }

    void OnHideUnity(bool isGameShown)
    {
        if (isGameShown)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    public void FaceBookLogout(System.Action callback)
    {
        if (FB.IsLoggedIn)
            FB.LogOut();

        if (callback != null)
            callback();
    }

    public void FaceBookLogin(System.Action faceBookLoginCallBack)
    {
        FaceBookLoginCallBack = faceBookLoginCallBack;
        List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCllBack);
    }

    void AuthCllBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken aToken = AccessToken.CurrentAccessToken;

            Debug.Log(aToken.UserId);
            foreach (string perm in aToken.Permissions)
                Debug.Log(perm);

            if (FaceBookLoginCallBack != null)
                FaceBookLoginCallBack();
        }
        else
            Debug.Log("User cancelled login");

        FaceBookLoginCallBack = null;
    }

    public void GetProfile(System.Action<Sprite> getFaceBookProfileResult)
    {
        GetFaceBookProfileResult = getFaceBookProfileResult;

        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Problem with getting user profile");
            return;
        }

        Sprite profile = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        if (GetFaceBookProfileResult != null)
            GetFaceBookProfileResult(profile);
        GetFaceBookProfileResult = null;
    }

    public void GetUserFirstName(System.Action<string> getFaceBookFirstNameResult)
    {
        GetFaceBookFirstNameResult = getFaceBookFirstNameResult;
        FB.API("/me?fields=id, first_name", Facebook.Unity.HttpMethod.GET, DealWithUserName);
    }

    void DealWithUserName(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Problem with getting user name");
            return;
        }

        if (GetFaceBookFirstNameResult != null)
            GetFaceBookFirstNameResult("UserFirstName");
        GetFaceBookFirstNameResult = null;
    }

    #endregion // FACEBOOK
}
