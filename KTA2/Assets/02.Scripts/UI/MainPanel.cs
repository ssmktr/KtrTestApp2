using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TinyJSON;

public class MainPanel : MonoBehaviour {

    public GameObject GoogleLoginBtn, GoogleLogoutBtn, GoogleProfileBtn, GoogleShowAchievementBtn, GoogleUseAchievementBtn, GoogleShowLeaderBoardBtn, GoogleUseLeaderBoardBtn, GoogleGetLeaderBoardScoreBtn;
    public GameObject FaceBookLoginBtn, FaceBookLogoutBtn, FaceBookSetScoreBtn, FaceBookShowScoreBtn;
    public UI2DSprite Profile;
    public UILabel MessageLbl;

    private void Awake()
    {
        UIEventListener.Get(GoogleLoginBtn).onClick = (sender) =>
        {
            if (!NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleLogin();
            }
        };

        UIEventListener.Get(GoogleLogoutBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleLogout();
            }
        };

        UIEventListener.Get(GoogleProfileBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GetGoogleImage();
            }
        };

        UIEventListener.Get(GoogleShowAchievementBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleShowAchievement();
            }
        };

        UIEventListener.Get(GoogleUseAchievementBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleUseAchievement(100f, (result) =>
                {
                    MessageLbl.text = result;
                });
            }
        };

        UIEventListener.Get(GoogleShowLeaderBoardBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleShowLeaderBoard();
            }
        };

        UIEventListener.Get(GoogleUseLeaderBoardBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                NativeManager.Instance.GoogleUseLeaderBoard((long)Random.Range(1, 100000), (result) =>
                {
                    MessageLbl.text = result;
                });
            }
        };

        UIEventListener.Get(GoogleGetLeaderBoardScoreBtn).onClick = (sender) =>
        {
            if (NativeManager.Instance.IsGoogleLogin)
            {
                MessageLbl.text = "";
                NativeManager.Instance.GoogleGetLeaderBoardScore(result =>
                {
                    foreach (UnityEngine.SocialPlatforms.IScore score in result)
                    {
                        MessageLbl.text += string.Format("User Id : {0}, Value : {1}, Rank : {2}\n\n", score.userID, score.value, score.rank);
                    }
                    //UnityEngine.SocialPlatforms.IScore[] scoreArray = result;
                    //MessageLbl.text = scoreArray.Length.ToString() + "개 데이터 있음";
                    //if (scoreArray.Length > 0)
                    //{
                    //    for (int i = 0; i < scoreArray.Length; ++i)
                    //    {
                    //        MessageLbl.text += string.Format("User Id : {0}, Value : {1}, Rank : {2}\n\n", scoreArray[i].userID, scoreArray[i].value, scoreArray[i].rank);
                    //    }
                    //}
                });
            }
        };

        UIEventListener.Get(FaceBookLoginBtn).onClick = (sender) =>
        {
            Debug.Log("FACEBOOK LOGIN...");

            NativeManager.Instance.FaceBookLogin(null);
        };

        UIEventListener.Get(FaceBookLogoutBtn).onClick = (sender) =>
        {
            Debug.Log("FACEBOOK LOGOUT...");

            NativeManager.Instance.FaceBookLogout();
        };

        UIEventListener.Get(FaceBookSetScoreBtn).onClick = (sender) =>
        {
            Debug.Log("FACEBOOK TEST BTN");

            NativeManager.Instance.SetScores();
        };

        UIEventListener.Get(FaceBookShowScoreBtn).onClick = (sender) =>
        {
            Debug.Log("FACEBOOK SHOW TEST BTN");

            NativeManager.Instance.QueryScores();
        };
    }

    private void Start()
    {
        NativeManager.Instance.Init(Profile, MessageLbl);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
