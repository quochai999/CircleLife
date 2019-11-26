using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStart : MonoBehaviour
{
    [SerializeField]
    private Animator btPlayAnimator, btGetMoreFreezingAnimator;
    public Action actionPlay;
    [SerializeField]
    private FreezeGetter freezeGetter;

    public void Run()
    {
        ScaleToZero();
        SetActive(true);
        SetTrigger();
    }

    private void ScaleToZero()
    {
        if (btPlayAnimator != null)
            btPlayAnimator.transform.localScale = Vector3.zero;
        if (btGetMoreFreezingAnimator != null)
            btGetMoreFreezingAnimator.transform.localScale = Vector3.zero;
    }

    private void SetTrigger()
    {
        if (btPlayAnimator != null)
            btPlayAnimator.SetTrigger("scale2_0_1");
        if (btGetMoreFreezingAnimator != null)
            btGetMoreFreezingAnimator.SetTrigger("scale2_0_1");
    }

    public void ClickPlay()
    {
        ScaleToZero();
        actionPlay?.Invoke();
    }

    public void ClickGetFreeze()
    {
        if (freezeGetter != null) freezeGetter.Run();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
