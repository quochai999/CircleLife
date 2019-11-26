using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{
    [SerializeField]
    private Image imgCircle;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Text txScore;
    [SerializeField]
    private ObDrag obDrag;
    [SerializeField]
    private GameObject rigOb;
    private string imgName;
    private int indexLine;
    public Action<int> actionActiveLine;
    public Action<long> actionSetTotalScore;
    public Action actionWrong;
    public Action<GameObject> actionRecreateCircles;


    public void SetImage(string img_name, int index_line, Action action)
    {
        indexLine = index_line;
        imgName = img_name;
        SetScore(0);
        if (obDrag != null)
        {
            obDrag.transform.localPosition = Vector3.zero;
            obDrag.SetParentName(img_name, indexLine);
            obDrag.actionActiveLine = delegate { actionActiveLine?.Invoke(indexLine); };
            obDrag.actionSetTotalScore = actionSetTotalScore;
            obDrag.actionWrong = actionWrong;
            obDrag.actionRecreateCircles = actionRecreateCircles;
        }

        if (imgCircle != null)
        {
            imgCircle.sprite = Resources.Load<Sprite>(string.Format("texts/{0}", img_name));
            imgCircle.preserveAspect = true;
            gameObject.SetActive(true);
            animator.enabled = true;
            animator.SetTrigger("scale_0_1");
        }

        if (rigOb != null)
        {
            rigOb.name = img_name;
            rigOb.tag = indexLine.ToString();
        }

    }

    public async void Destroy()
    {
        if (animator != null)
            animator.SetTrigger("scale_1_0");
        await Task.Delay(1000);
        Destroy(gameObject);
    }

    public string GetName()
    {
        return imgName;
    }

    public int GetIndexLine()
    {
        return indexLine;
    }

    public void InvokeActiveLine()
    {
        actionActiveLine?.Invoke(indexLine);
    }

    public void SetScore(long score)
    {
        if (txScore != null)
            txScore.text =  score > 0 ? score.ToString() : string.Empty;
    }
}
