using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObDrag : MonoBehaviour
{
    private string parentName;
    private string index;
    private int numberMeetCenterCircle;
    private bool isMeetCenterCircle;
    private int countScore;
    private long totalScore;
    private List<string> lstOtherCircles = new List<string>();
    public Action actionWrong;
    public Action<long> actionSetTotalScore;
    public Action actionActiveLine;
    public Action<GameObject> actionRecreateCircles;
    private List<GameObject> lstObs = new List<GameObject>();
    private bool isSelected = false;
    private bool isDragging = false;
    [SerializeField]
    private Canvas canvas;

    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        isSelected = false;
        isDragging = false;
        numberMeetCenterCircle = 0;
        isMeetCenterCircle = false;
        lstOtherCircles.Clear();
        lstObs.Clear();
        countScore = 1;
        totalScore = 0;
        transform.localPosition = Vector3.zero;
        if (canvas != null)
            canvas.sortingOrder = 0;
    }

    private void DestroyOtherCircles()
    {
        if (lstObs == null || lstObs.Count == 0) return;
        for (int i = 0; i < lstObs.Count; i++)
        {
            if (lstObs[i] != null)
            {
                lstObs[i].GetComponent<Animator>().enabled = false;
                lstObs[i].transform.localScale = Vector3.zero;
                actionRecreateCircles?.Invoke(lstObs[i]);
            }
        }
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<Animator>().enabled = false;
        parent.transform.localScale = Vector3.zero;
        actionRecreateCircles?.Invoke(parent);
    }

    protected virtual void Update()
    {
        if (isSelected)
        {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(cursorPos.x, cursorPos.y);
        }
    }

    private void OnMouseDrag()
    {
        isDragging = true;
    }

    private void OnMouseDown()
    {
        Reset();
        isDragging = false;
        isSelected = true;
        if (canvas != null)
            canvas.sortingOrder = 2;
    }

    private void OnMouseUp()
    {
        isSelected = false;
        actionSetTotalScore?.Invoke(totalScore);
        if (totalScore == 0)
            Reset();
        else
            DestroyOtherCircles();
    }

    public void SetParentName(string parent_name, int index)
    {
        parentName = parent_name;
        this.index = index.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || collision.CompareTag("Untagged") || string.IsNullOrEmpty(collision.tag)) return;
        if (collision.name.Equals(parentName) && collision.CompareTag(index)) return;
        if (collision.CompareTag("center") && isMeetCenterCircle == false)
        {
            isMeetCenterCircle = true;
            numberMeetCenterCircle++;
            actionActiveLine?.Invoke();
            return;
        }
        if (collision.name.Equals(parentName) && !collision.CompareTag(index))
        {
            string otherName = collision.name;
            string otherIndexLine = collision.tag;
            if (!lstOtherCircles.Contains(string.Format("{0}_{1}", otherName, otherIndexLine)) && isMeetCenterCircle == true)
            {
                lstOtherCircles.Add(string.Format("{0}_{1}", otherName, otherIndexLine));
                countScore++;
                long score = (long)Math.Pow(2, countScore);
                totalScore += score;
                isMeetCenterCircle = false;
                if (!lstObs.Contains(collision.transform.parent.gameObject))
                    lstObs.Add(collision.transform.parent.gameObject);
                Circle otherCircle = collision.transform.parent.gameObject.GetComponent<Circle>();
                if (otherCircle != null)
                {
                    otherCircle.InvokeActiveLine();
                    otherCircle.SetScore(score);
                }
            }
        }
        else
        {
            if (isMeetCenterCircle && collision.CompareTag("center") || isMeetCenterCircle == false) return;
            for (int i = 0; i < lstObs.Count; i++)
            {
                if (lstObs[i] != null)
                {
                    Circle otherCircle = lstObs[i].GetComponent<Circle>();
                    if (otherCircle != null)
                        otherCircle.SetScore(0);
                }
            }
            Reset();
            actionWrong?.Invoke();
        }
    }
}
