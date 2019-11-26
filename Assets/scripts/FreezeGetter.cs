using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class FreezeGetter : MonoBehaviour
{
    [SerializeField]
    private Text txFreeze;
    [SerializeField]
    private Button btGetMore;
    [SerializeField]
    private GameObject imgMask;
    [SerializeField]
    private AudioSource audioSourceEffect, audioSourceMusic;
    private int totalNumberFreeze;
    private string gameId = "3370714";
    private bool testMode = true;
    private string placementId = "rewardedVideo";
    private ShowAdPlacementContent ad = null;

    private void Start()
    {
        if (Monetization.isSupported)
            Monetization.Initialize(gameId, false);
    }

    public void Run()
    {
        totalNumberFreeze = PlayerPrefs.GetInt("number_freeze", 0);
        if (totalNumberFreeze < 0)
            totalNumberFreeze = 0;
        if (txFreeze != null)
            txFreeze.text = totalNumberFreeze.ToString();
        if (totalNumberFreeze > 0)
            SetBtGetMoreInteractable(false);
        else
            WaitForAds();
        gameObject.SetActive(true);
    }

    private async void WaitForAds()
    {
        while (true)
        {
            if (Monetization.IsReady(placementId)) break;
            await Task.Delay(100);
        }
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
        if (ad != null)
            SetBtGetMoreInteractable(true);
    }

    public void ClickGetMore()
    {
        if (totalNumberFreeze > 0 || !Monetization.IsReady(placementId)) return;
        if (ad != null)
         ad.Show(AdFinished);
    }

    private void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            totalNumberFreeze = 50;
            PlayerPrefs.SetInt("number_freeze", totalNumberFreeze);
            PlayerPrefs.Save();
            if (txFreeze != null)
                txFreeze.text = totalNumberFreeze.ToString();
            SetBtGetMoreInteractable(false);
        }
        else if(result == ShowResult.Failed)
        {
            Debug.Log("Watch Ads failed.");
        }else if(result == ShowResult.Skipped)
        {
            Debug.Log("Ads skipped.");
        }
    }

    public void ClickSoundEffect(GameObject mask)
    {
        if (mask == null || audioSourceEffect == null) return;
        if (mask.activeSelf == false)
        {
            audioSourceEffect.volume = 0;
            mask.SetActive(true);
        }
        else
        {
            audioSourceEffect.volume = 0.8f;
            mask.SetActive(false);
        }
    }

    public void ClickSoundMusic(GameObject mask)
    {
        if (mask == null || audioSourceMusic == null) return;
        if (mask.activeSelf == false)
        {
            audioSourceMusic.volume = 0;
            mask.SetActive(true);
        }
        else
        {
            audioSourceMusic.volume = 0.4f;
            mask.SetActive(false);
        }
    }

    public void ClickBack()
    {
        gameObject.SetActive(false);
    }

    private void SetBtGetMoreInteractable(bool interact)
    {
        if (btGetMore != null) btGetMore.interactable = interact;
        if (imgMask != null) imgMask.SetActive(!interact);
    }

}
