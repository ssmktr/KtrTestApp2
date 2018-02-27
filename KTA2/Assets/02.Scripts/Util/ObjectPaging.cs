using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPaging : MonoBehaviour {

    public enum eScrollType
    {
        Vertical,   //< 위,아래
        Horizontal, //< 좌,우
    }

    GameObject CreateObj;               //< 생성할 오브젝트
    Transform NowGridPanel;            //< 해당 UI Grid 패널
    Transform NowListPanel;            //< 해당 UI패널
    float cellHeight = 0;               //< 오브젝트 높이값
    float cellWidth = 0;                //< 오브젝트 넓이값
    int NowIndex = 1;                   //< 현재 페이지
    int MaxIndex = 0;                   //< 맥스 페이지
    int CreateCount = 1;                //< 생성시 몇개를 생성할것인지 카운트
    Vector2 DragPos = new Vector2();    //< 페이징할시 기준 높이값
    float BottomLimit = 0;              //< 아래 위치 리밋값
    int FirstCreateCount = 0;

    eScrollType ScrollType;
    UIScrollView scrollView;

    public System.Action<int, GameObject> CreateCallBack;
    public System.Action<GameObject> OffCallBack;

    GameObject UpCreateObject;
    GameObject DownCreateObject;

    public static ObjectPaging CreatePagingPanel(GameObject Uipanel, GameObject Uigrid, GameObject CreateObj, int _CreateCount, int FirstCreateCount, int MaxCreateCount, float BottomLimit, System.Action<int, GameObject> _CallBack, eScrollType _ScrollType = eScrollType.Vertical, bool _NotUpdate = false)
    {
        ObjectPaging objectPaging = Uipanel.AddComponent<ObjectPaging>();
        objectPaging.Setup(Uipanel, CreateObj, Uigrid, _CreateCount, FirstCreateCount, MaxCreateCount, BottomLimit, _CallBack, _ScrollType, _NotUpdate);
        return objectPaging;
    }

    public List<GameObject> GetCreateObjList(ref List<GameObject> Returnlist)
    {
        for (int i = 0; i < RecycleObjectList.Count; i++)
        {
            if (RecycleObjectList[i].activeSelf)
                Returnlist.Add(RecycleObjectList[i]);
        }
        return Returnlist;
    }

    bool Live = false;
    bool NotUpdate = false;
    public void Setup(GameObject _UIListPanel, GameObject _CreateObj, GameObject _UIGridPanel, int _CreateCount, int _FirstCreateCount, int _CreateMaxCount, float _BottomLimit, System.Action<int, GameObject> _CreateCallBack, eScrollType _ScrollType, bool _NotUpdate)
    {
        if (_UIListPanel.transform.localPosition != Vector3.zero)
            Debug.LogError("페이징을 사용하려면 ListPanel의 localPosition이 zero여야 함");

        CreateObj = _CreateObj;
        NowGridPanel = _UIGridPanel.transform;
        NowListPanel = _UIListPanel.transform;
        CreateCallBack = _CreateCallBack;

        CreateCount = _CreateCount;
        cellHeight = NowGridPanel.GetComponent<UIGrid>().cellHeight;
        cellWidth = NowGridPanel.GetComponent<UIGrid>().cellWidth;
        BottomLimit = _BottomLimit;

        ScrollType = _ScrollType;
        NotUpdate = _NotUpdate;

        scrollView = NowListPanel.GetComponent<UIScrollView>();

        //< 총 몇개까지 페이징 될것인가
        MaxIndex = _CreateMaxCount - 1;

        //< 처음에 몇개를 생성할것인가.
        _FirstCreateCount--;

        //< 맥스카운트 보정
        if (MaxIndex < _FirstCreateCount)
            MaxIndex = _FirstCreateCount;

        //< 현재 인덱스
        NowIndex = FirstCreateCount = _FirstCreateCount;

        //< 오브젝트를 미리생성해둠
        FirstCreateObj();

        //< 처음 한번 생성
        FirstCreate();
        DeleteUpObjectCount = 0;

        Live = true;
    }

	void Update() 
    {
        if (NowListPanel == null || !Live || !NowListPanel.gameObject.activeSelf)
            return;

        if (NotCreatePanel > 0)
        {
            NotCreatePanel -= Time.deltaTime;
            SpringPanel panel = NowListPanel.GetComponent<SpringPanel>();
            if (panel != null)
                DestroyImmediate(panel);
        }

        //< 업데이트를 아예 안한다.
        if (NotUpdate)
            return;

        if(ScrollType == eScrollType.Vertical)
        {
            float center = (NowListPanel.localPosition.y + NowGridPanel.localPosition.y);
            if (center < DragPos.x)         //< 위로 올렸을시
                CreateUpObject();
            else if (center > DragPos.y)    //< 아래로 내렸을시
                CreateDownObject();
        }
        else if(ScrollType == eScrollType.Horizontal)
        {
            
            float center = (NowListPanel.localPosition.x + NowGridPanel.localPosition.x);
            if (center > DragPos.x)         //< 왼쪽으로 이동했을시
                CreateUpObject();
            else if (center < DragPos.y)    //< 오른쪽으로 이동했을시
                CreateDownObject();

            //Debug.Log(center + " , " + DragPos);
        }
	}

    //< 처음 생성해준다.
    void FirstCreate()
    {
        for (int i = 0; i < FirstCreateCount + 1; i++)
        {
            CreateObj = RecycleObject(true, true);
            CreateObj.transform.parent = NowGridPanel.transform;
            CreateObj.transform.localScale = new Vector3(1, 1, 1);

            if (ScrollType == eScrollType.Vertical)
                CreateObj.transform.localPosition = new Vector3((i % CreateCount) * cellWidth, -((i / CreateCount) * cellHeight), 0);
            else if (ScrollType == eScrollType.Horizontal)
                CreateObj.transform.localPosition = new Vector3((i / CreateCount) * cellWidth, -((i % CreateCount) * cellHeight), 0);

            if (i < (MaxIndex + 1))
            {
                if (CreateCallBack != null)
                    CreateCallBack(i, CreateObj);
            }
            else
                CreateObj.SetActive(false);
        }

        //< 마지막 스크롤 위치값을 대입해준다.
        if (ScrollType == eScrollType.Vertical)
        {
            DragPos.y = ((CreateObj.transform.localPosition.y + NowGridPanel.transform.localPosition.y) * -1);
            DragPos.y -= BottomLimit;
        }
        else if (ScrollType == eScrollType.Horizontal)
        {
            DragPos.y = ((CreateObj.transform.localPosition.x + NowGridPanel.transform.localPosition.x) * -1);
            DragPos.y -= BottomLimit;
        }
    }

    int DeleteUpObjectCount = 0;
    float NotCreatePanel = 0;
    float NotCreatePanelTime = 1;
    void CreateUpObject()
    {
        if (UpCreateObject == null)
            return;

        if ((NowIndex) <= FirstCreateCount)
        {
            NotCreatePanel = 0;
            scrollView.MaxLineSize = Vector3.zero;

            //< 갱신하기위함
            if (!scrollViewUpdate)
            {
                scrollViewUpdate = true;
                scrollView.Press(false);
            }
            
            return;
        }
        else
            scrollViewUpdate = false;

        float LastCreatePos = UpCreateObject.transform.localPosition.y;
        if (ScrollType == eScrollType.Horizontal)
            LastCreatePos = UpCreateObject.transform.localPosition.x;

        for (int i = 0; i < CreateCount; i++)
        {
            NowIndex--;

            CreateObj = RecycleObject(false);
            CreateObj.transform.parent = NowGridPanel.transform;
            CreateObj.transform.localScale = new Vector3(1, 1, 1);

            if(ScrollType == eScrollType.Vertical)
                CreateObj.transform.localPosition = new Vector3(((CreateCount - 1) - i) * cellWidth, LastCreatePos + cellHeight, 0);
            else
                CreateObj.transform.localPosition = new Vector3(LastCreatePos - cellWidth, -(((CreateCount - 1) - i) * cellHeight), 0);

            //< 생성했던 클래스로 셋업하도록 보냄
            if (CreateCallBack != null)
            {
                if ((NowIndex) - FirstCreateCount >= 0)
                    CreateCallBack((NowIndex) - FirstCreateCount, CreateObj);
            }
        }

        //< 스크롤 범위를 늘려줌
        if (ScrollType == eScrollType.Vertical)
        {
            scrollView.MaxLineSize = new Vector3(0, (((MaxIndex / CreateCount)) * cellHeight), 0);

            //< 위 스크롤 기준을 만들어준다
            DragPos.x = ((CreateObj.transform.localPosition.y - NowGridPanel.transform.localPosition.y) * -1);

            //< 마지막 스크롤 위치값을 대입해준다.
            DragPos.y = ((DownCreateObject.transform.localPosition.y + NowGridPanel.transform.localPosition.y) * -1);
            DragPos.y -= BottomLimit;
        }
        else
        {
            scrollView.MaxLineSize = new Vector3((((MaxIndex / CreateCount)) * cellWidth), 0, 0);

            //< 위 스크롤 기준을 만들어준다
            DragPos.x = ((CreateObj.transform.localPosition.x - NowGridPanel.transform.localPosition.x) * -1);

            //< 마지막 스크롤 위치값을 대입해준다.
            DragPos.y = ((DownCreateObject.transform.localPosition.x + NowGridPanel.transform.localPosition.x) * -1);
            DragPos.y -= BottomLimit;
        }

        NotCreatePanel = NotCreatePanelTime;
    }

    bool scrollViewUpdate = false;
    void CreateDownObject()
    {
        
        if (DownCreateObject == null)
            return;

        if (NowIndex >= MaxIndex)
        {
            NotCreatePanel = 0;
            scrollView.MaxLineSize = Vector3.zero;

            //< 갱신하기위함
            if (!scrollViewUpdate)
            {
                scrollViewUpdate = true;
                scrollView.Press(false);
            }
            return;
        }
        else
            scrollViewUpdate = false;

        float LastCreatePos = DownCreateObject.transform.localPosition.y;
        if (ScrollType == eScrollType.Horizontal)
            LastCreatePos = DownCreateObject.transform.localPosition.x;

        //< 해당 카운트만큼 생성해둠
        for (int i = 0; i < CreateCount; i++ )
        {
            NowIndex++;

            CreateObj = RecycleObject(true);
            CreateObj.transform.parent = NowGridPanel.transform;
            CreateObj.transform.localScale = new Vector3(1, 1, 1);

            if(ScrollType == eScrollType.Vertical)
                CreateObj.transform.localPosition = new Vector3(i * cellWidth, LastCreatePos - cellHeight, 0);
            else
                CreateObj.transform.localPosition = new Vector3(LastCreatePos + cellWidth, -(i * cellHeight), 0);

            //< 생성했던 클래스로 셋업하도록 보냄
            if (CreateCallBack != null)
                CreateCallBack(NowIndex, CreateObj);
        }

        //< 스크롤 범위를 늘려줌
        if (ScrollType == eScrollType.Vertical)
        {
            scrollView.MaxLineSize = new Vector3(0, (((MaxIndex / CreateCount)) * cellHeight), 0);

            //< 위 스크롤 기준을 만들어준다
            DragPos.x = ((UpCreateObject.transform.localPosition.y - NowGridPanel.transform.localPosition.y) * -1);

            //< 마지막 스크롤 위치값을 대입해준다.
            DragPos.y = ((CreateObj.transform.localPosition.y + NowGridPanel.transform.localPosition.y) * -1);
            DragPos.y -= BottomLimit;
        }
        else
        {
            scrollView.MaxLineSize = new Vector3((((MaxIndex / CreateCount)) * cellWidth), 0, 0);

            //< 위 스크롤 기준을 만들어준다
            DragPos.x = ((UpCreateObject.transform.localPosition.x - NowGridPanel.transform.localPosition.x) * -1);

            //< 마지막 스크롤 위치값을 대입해준다.
            DragPos.y = ((CreateObj.transform.localPosition.x + NowGridPanel.transform.localPosition.x) * -1);
            DragPos.y -= BottomLimit;
        }
        
        NotCreatePanel = NotCreatePanelTime;
    }

    //< 현재 맥스치에 따라 정보를 갱신하는 용도
    public void NowCreate(int _CreateMaxCount)
    {
        //< 맥스 카운트 대입
        MaxIndex = _CreateMaxCount - 1;

        //< 맥스카운트 보정
        if (MaxIndex < FirstCreateCount)
            MaxIndex = FirstCreateCount;

        //if (MaxIndex < NowIndex)
        //    NowIndex = MaxIndex;

        //< 현재 생성되어있는 녀석들의 정보를 갱신
        int count = 0;
        int StartCount = (NowIndex - FirstCreateCount);
        if (StartCount < 0)
            StartCount = 0;

        //< 정보 갱신
        for (int i = 0; i < RecycleObjectList.Count; i++)
        {
            if (RecycleObjectList[i].activeSelf && CreateCallBack != null)
            {
                CreateCallBack(StartCount + count, RecycleObjectList[i]);
                count++;
            }
        }
    }

    public void NowCreate2(int _CreateMaxCount)
    {
        //< 맥스 카운트 대입
        MaxIndex = _CreateMaxCount - 1;

        //< 맥스카운트 보정
        if (MaxIndex < FirstCreateCount)
            MaxIndex = FirstCreateCount;

        //if (MaxIndex < NowIndex)
        //    NowIndex = MaxIndex;

        //< 현재 생성되어있는 녀석들의 정보를 갱신
        int count = 0;
        int StartCount = (NowIndex - FirstCreateCount);
        if (StartCount < 0)
            StartCount = 0;

        //< 정보 갱신
        for (int i = 0; i < RecycleObjectList.Count; i++)
        {
            if (CreateCallBack != null)
            {
                CreateCallBack(StartCount + count, RecycleObjectList[i]);
                count++;
            }
        }
    }

    public void SetIndex(int index)
    {
        NowIndex = index;
        Update();
        NowCreate(MaxIndex + 1);
    }
    public void SetMaxCount(int _maxcount)
    {
        MaxIndex = _maxcount - 1;
    }

    public void SetPanelActive(bool active)
    {
        NowListPanel.gameObject.SetActive(active);
    }
    //<==================================================
    //<         오브젝트 재활용을 위한 클래스
    //<==================================================
    void FirstCreateObj()
    {
        for (int i = 0; i < FirstCreateCount + 1; i++)
        {
            RecycleObjectList.Add(Instantiate(CreateObj) as GameObject);
            RecycleObjectList[RecycleObjectList.Count - 1].transform.parent = NowGridPanel.transform;
            RecycleObjectList[RecycleObjectList.Count - 1].transform.localPosition = Vector3.zero;
            RecycleObjectList[RecycleObjectList.Count - 1].transform.localScale = Vector3.one;
            RecycleObjectList[RecycleObjectList.Count - 1].SetActive(false);
            RecycleObjectList[RecycleObjectList.Count - 1].name = RecycleObjectList[RecycleObjectList.Count - 1].name.Replace("(Clone)", string.Format("_{0}", i));
        }
    }

    List<GameObject> RecycleObjectList = new List<GameObject>();
    GameObject RecycleObject(bool DownChange, bool First = false)
    {
        //< 현재 맨 처음 생성되어있는 위치를 얻음
        int StartCreateIdx = 0;
        for (int i = 0; i < RecycleObjectList.Count; i++)
        {
            if (RecycleObjectList[i] != null && RecycleObjectList[i].activeSelf)
            {
                StartCreateIdx = i;
                UpCreateObject = RecycleObjectList[i];
                break;
            }
        }

        //< 현재 맨 마지막까지 생성되어있는 위치를 얻음
        int EndCreateIdx = 0;
        for (int i = 0; i < RecycleObjectList.Count; i++)
        {
            if (RecycleObjectList[i] != null && RecycleObjectList[i].activeSelf)
            {
                EndCreateIdx = i;
            }
        }

        //< 아래에 생성할시 처리
        if (DownChange)
        {
            DeleteUpObjectCount++;

            //< 만약 마지막 위치에 있는 녀석이라면
            if ((EndCreateIdx + 1) >= RecycleObjectList.Count)
            {
                //< 첫번째 녀석을 맨 밑으로 넣고 빼줌
                GameObject obj = RecycleObjectList[0];
                RecycleObjectList.RemoveAt(0);
                RecycleObjectList.Add(obj);
                obj.SetActive(true);

                UpCreateObject = RecycleObjectList[0];
                DownCreateObject = obj;
                
                return obj;
            }
            //< 그게 아닐경우
            else
            {
                //< 위에있던 녀석은 안드로메다로 보내버림
                if (!First)
                {
                    RecycleObjectList[StartCreateIdx].gameObject.SetActive(false);

                    if (OffCallBack != null)
                        OffCallBack(RecycleObjectList[StartCreateIdx]);
                }
                
                //< 마지막 녀석을 리턴해줌
                RecycleObjectList[EndCreateIdx + 1].SetActive(true);
                UpCreateObject = RecycleObjectList[StartCreateIdx];
                DownCreateObject = RecycleObjectList[EndCreateIdx + 1];

                return RecycleObjectList[EndCreateIdx + 1];
            }
        }
        //< 위에 생성할시 처리
        else
        {
            //< 만약 맨 위에 있는 녀석이라면
            if (StartCreateIdx <= 0)
            {
                //< 맨밑에 녀석을 위로 넣어주고 밑에서 빼줌
                GameObject obj = RecycleObjectList[RecycleObjectList.Count - 1];
                RecycleObjectList.RemoveAt(RecycleObjectList.Count - 1);
                RecycleObjectList.Insert(0, obj);
                obj.SetActive(true);

                UpCreateObject = RecycleObjectList[0];
                DownCreateObject = RecycleObjectList[EndCreateIdx];
                return obj;
            }
            //< 그게 아닐경우
            else
            {
                //< 맨 아래에 있던 녀석은 비활성화
                if (!First)
                {
                    RecycleObjectList[EndCreateIdx].gameObject.SetActive(false);

                    if (OffCallBack != null)
                        OffCallBack(RecycleObjectList[EndCreateIdx]);
                }

                //< 첫번째 녀석을 리턴해줌
                RecycleObjectList[StartCreateIdx - 1].SetActive(true);
                UpCreateObject = RecycleObjectList[StartCreateIdx - 1];
                DownCreateObject = RecycleObjectList[EndCreateIdx - 1];

                return RecycleObjectList[StartCreateIdx - 1];
            }
        }
    }

    void OnDestroy()
    {
        for(int i=0; i<RecycleObjectList.Count; i++)
        {
            if (RecycleObjectList[i] != null)
                Destroy(RecycleObjectList[i]);
        }
    }

}
