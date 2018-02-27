using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

using Facebook.Unity;

public class NativeManager : Singleton<NativeManager>
{
    UI2DSprite Profile;
    UILabel MessageLbl;

    public void Init(UI2DSprite profile, UILabel messageLbl)
    {
        Profile = profile;
        MessageLbl = messageLbl;

        Profile.gameObject.SetActive(false);

        GoogleInit();
        FaceBookInit();
    }

    #region GOOGLE
    public bool IsGoogleLogin = false;
    void GoogleInit()
    {
        IsGoogleLogin = false;
        PlayGamesPlatform.Activate();
    }

    public void GoogleLogin()
    {
        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(GoogleLoginCallBack);
    }

    void GoogleLoginCallBack(bool result)
    {
        IsGoogleLogin = result;
    }

    public void GoogleLogout()
    {
        if (Social.localUser.authenticated)
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
            IsGoogleLogin = false;

            MessageLbl.text = "Logout";
            Profile.gameObject.SetActive(false);
        }
    }

    public void GetGoogleImage()
    {
        if (Social.localUser.authenticated)
        {
            MessageLbl.text = string.Format("User Id : {0}\nUser Name : {1}", GetGoogleId(), GetGoogleName());
            Profile.gameObject.SetActive(true);
            Profile.sprite2D = Sprite.Create(Social.localUser.image, new Rect(0, 0, 128, 128), new Vector2());
            Profile.MakePixelPerfect();
        }
    }

    public string GetGoogleId()
    {
        if (Social.localUser.authenticated)
            return Social.localUser.id;

        return null;
    }

    public string GetGoogleName()
    {
        if (Social.localUser.authenticated)
            return Social.localUser.userName;

        return null;
    }

    //// 리더보드 보여주기
    //public void GoogleShowLeaderBoard()
    //{
    //    Social.ShowLeaderboardUI();
    //}

    //// 리더보드 사용
    //public void GoogleUseLeaderBoard(long score, System.Action<string> callback)
    //{
    //    Social.ReportScore(score, GoogleManager.GoogleData.leaderboard_leaderboard_001, (result) =>
    //    {
    //        if (result)
    //            callback(string.Format("LEADERBOARD SCORE : {0} SUCCESS", score));
    //        else
    //            callback(string.Format("LEADERBOARD SCORE : {0} FAIL", score));
    //    });
    //}

    //// 리더보드 스코어 가져오기
    //public void GoogleGetLeaderBoardScore(System.Action<UnityEngine.SocialPlatforms.IScore[]> callback)
    //{
    //    Social.LoadScores(GoogleManager.GoogleData.leaderboard_leaderboard_001, callback);
    //}

    //// 업적 보기
    //public void GoogleShowAchievement()
    //{
    //    Social.ShowAchievementsUI();
    //}

    //public void GoogleUseAchievement(float progress, System.Action<string> callback)
    //{
    //    Social.ReportProgress(GoogleManager.GoogleData.achievement_1, progress, (result) => 
    //    {
    //        if (result)
    //            callback(string.Format("ACHIEVEMENT PROGRESS : {0} SUCCESS", progress));
    //        else
    //            callback(string.Format("ACHIEVEMENT PROGRESS : {0} FAIL", progress));
    //    });
    //}
    #endregion // GOOGLE

    #region FACEBOOK
    System.Action FaceBookLoginResult = null;
    void FaceBookInit()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    void InitCallBack()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("FACEBOOK INIT FAIL");
        }
    }

    void OnHideUnity(bool isGameShow)
    {
        if (!isGameShow)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void FaceBookLogin(System.Action resultCallback)
    {
        FaceBookLoginResult = resultCallback;

        List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallBack);
    }

    void AuthCallBack(ILoginResult result)
    {
        if (result.Error != null)
        {
            MessageLbl.text = string.Format("Auth Error : {0}", result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                MessageLbl.text = "FaceBook Login!!";
                AccessToken aToken = AccessToken.CurrentAccessToken;
                //MessageLbl.text += string.Format("\naToken string : {0}", aToken.TokenString);
                MessageLbl.text += string.Format("\nUser ID : {0}", aToken.UserId);

                foreach (string perm in aToken.Permissions)
                {
                    MessageLbl.text += string.Format("\npermissions : {0}", perm);
                }

                if (FaceBookLoginResult != null)
                    FaceBookLoginResult();
            }
            else
            {
                MessageLbl.text = "User cancelled login";
            }
        }

        DealWithFBMenus(FB.IsLoggedIn);
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
    }

    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            MessageLbl.text += string.Format("\n반가워요. {0}", result.ResultDictionary["first_name"]);
        }
        else
        {
            MessageLbl.text = string.Format("DisplayUsername Error {0}", result.Error);
        }
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Error == null)
        {
            MessageLbl.text += "\n나의 프로필 이미지 성공";
            Profile.gameObject.SetActive(true);
            Profile.sprite2D = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
            Profile.MakePixelPerfect();
        }
        else
        {
            MessageLbl.text = string.Format("DisplayProfilePic Error {0}", result.Error);
        }
    }

    ////친구초대
    //public void InviteFriends()
    //{
    //    // 순서 변경시 컴파일 에러 발생
    //    // message : 보낼 메시지
    //    // title : 메시지 보낼 친구목록 창의 타이틀
    //    FB.AppRequest(
    //        message: "This gmae is awesome, join me. now!",
    //        title: "Invite your firends to join you"
    //    );
    //}

    //점수 불러오기
    public void QueryScores()
    {
        FB.API("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, ScoresCallback);
    }

    //점수 Scores Console에 표시하기
    private void ScoresCallback(IResult result)
    {
        Debug.Log("Scores Callback : " + result.ResultDictionary.ToJson());
        MessageLbl.text = result.ResultDictionary.ToJson();
    }

    //점수 설정하기
    public void SetScores()
    {
        Dictionary<string, string> scoreData = new Dictionary<string, string>();
        scoreData["score"] = UnityEngine.Random.Range(10, 1000).ToString();
        FB.API("/me/scores", HttpMethod.POST, ScoreCallBack, scoreData);
    }

    void ScoreCallBack(IGraphResult result)
    {
        MessageLbl.text = MiniJSON.Json.Serialize(result.ResultDictionary);

        QueryScores();
    }

    public void FaceBookLogout()
    {
        FB.LogOut();

        MessageLbl.text = "Logout";
        Profile.gameObject.SetActive(false);
    }
    #endregion // FACEBOOK
}
