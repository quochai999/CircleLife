using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishToStop : MonoBehaviour
{
    public Action actionYes;

    public void Run()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClickYes()
    {
        ClickNo();
        actionYes?.Invoke();
    }

    public void ClickNo()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
