using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TinyJSON;

public class MainPanel : MonoBehaviour {

    private void Awake()
    {
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
}
