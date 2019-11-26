using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]
public class CirclesLife : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private Image[] lines;
    [SerializeField]
    private GameObject prefab;
    private string[] circlesNames = { "circle_blue", "circle_green", "circle_yellow", "circle_brown" };
    private int[] flagScores = { 50, 150, 300, 500, 750, 1050, 1400, 1800, 2250, 2750 };
    private Dictionary<string, Color32> dictColors;
    [SerializeField]
    private Text txPlusScore, txTotalScore, txBestScore;
    [SerializeField]
    private Animator txPlusScoreAnimator;
    [SerializeField]
    private Animator txTotalScoreAnimator;
    [SerializeField]
    private Animator imgRedWarning, imgFreeze;
    [SerializeField]
    private UIStart uiStart;
    [SerializeField]
    private GameObject[] circleLifes;
    [SerializeField]
    private Rotate bigCircle;
    [SerializeField]
    private Text txNumberFreeze;
    [SerializeField]
    private GameObject btFreeze, btBack;
    [SerializeField]
    private WishToStop wishToStop;
    [SerializeField]
    private AudioClip bubblePopSound;
    [SerializeField]
    private AudioSource audioSource;
    private long totalScore;
    private int lifes;
    private List<Circle> lstCircles = new List<Circle>();
    private List<int> lstPointsInUse = new List<int>();
    private bool isGameOver;
    private int indexNumberCircle;
    private int currentFlagScore;
    private int numberFreeze;
    private bool blockClickFreeze;

    private void Start()
    {
        ActiveUIStart();
    }

    private void ActiveUIStart()
    {
        SetActiveBtFreeze(false);
        if (uiStart != null)
        {
            uiStart.actionPlay = delegate { uiStart.SetActive(false); Setup(); };
            uiStart.Run();
        }
    }

    private void Setup()
    {
        CreateColors();
        CreateCircles();
        SetActiveLines(false);
        if (wishToStop != null)
        {
            wishToStop.actionYes = delegate
            {
                GameOver();
            };
        }
        Reset();
    }

    public void ClickStop()
    {
        if (wishToStop != null)
            wishToStop.Run();
    }

    private void Reset()
    {
        totalScore = 0;
        currentFlagScore = 0;
        if (txTotalScore != null)
            txTotalScore.text = totalScore.ToString();
        if (txPlusScore != null)
            txPlusScore.text = string.Empty;
        if (txBestScore != null)
            txBestScore.text = string.Empty;
        lifes = 2;
        if (circleLifes != null && circleLifes.Length > 1)
        {
            circleLifes[0].SetActive(true);
            circleLifes[1].SetActive(true);
        }
        if (imgFreeze != null)
            imgFreeze.enabled = false;
        isGameOver = false;
        blockClickFreeze = false;
        GetUsersNumberFreeze();
        if (btBack != null)
            btBack.SetActive(true);
    }

    private void SetTextNumberFreeze(int number)
    {
        if (txNumberFreeze != null)
            txNumberFreeze.text = number.ToString();
    }

    private void SetActiveBtFreeze(bool active)
    {
        if (btFreeze != null)
            btFreeze.SetActive(active);
    }

    private void GetUsersNumberFreeze()
    {
        int totalNumberFreeze = PlayerPrefs.GetInt("number_freeze", 0);
        if (totalNumberFreeze < 0)
            totalNumberFreeze = 0;
        numberFreeze = totalNumberFreeze;
        if (numberFreeze > 5)
            numberFreeze = 5;
        totalNumberFreeze -= numberFreeze;
        if (totalNumberFreeze < 0)
            totalNumberFreeze = 0;
        SaveTotalNumberFreeze(totalNumberFreeze);
        SetTextNumberFreeze(numberFreeze);
        SetActiveBtFreeze(true);
    }

    public async void ClickFreeze()
    {
        if (numberFreeze == 0 || blockClickFreeze == true) return;
        blockClickFreeze = true;
        numberFreeze--;
        SetTextNumberFreeze(numberFreeze);
        if (bigCircle != null)
        {
            float oldSpeed = bigCircle.speed;
            bigCircle.speed = 40;
            if (imgFreeze != null)
            {
                imgFreeze.enabled = true;
                imgFreeze.SetTrigger("freeze");
            }
            await Task.Delay(5000);
            bigCircle.speed = oldSpeed;
            blockClickFreeze = false;
            if (imgFreeze != null)
                imgFreeze.enabled = false;
        }
    }

    private void CreateColors()
    {
        dictColors = new Dictionary<string, Color32>();
        dictColors.Add("circle_blue", new Color32(0, 113, 187, 255));
        dictColors.Add("circle_green", new Color32(51, 153, 51, 255));
        dictColors.Add("circle_yellow", new Color32(255, 192, 0, 255));
        dictColors.Add("circle_brown", new Color32(150, 75, 0, 255));
    }

    private async void CreateCircles()
    {
        indexNumberCircle = 8;
        lstPointsInUse.Clear();
        for (int i = 0; i < indexNumberCircle; i++)
        {
            CreateSingleCircle(i);
            await Task.Delay(20);
        }
    }

    private async void ClearAndCheckRecreateCircles(GameObject ob)
    {
        if (isGameOver == true) return;
        if (ob == null) return;
        Circle circle = ob.GetComponent<Circle>();
        if (circle != null)
        {
            await Task.Delay(2000);
            string color = circlesNames[Random.Range(0, circlesNames.Length)];
            int indexLine = circle.GetIndexLine();
            circle.SetImage(color, indexLine, null);
            if (dictColors.ContainsKey(color))
                lines[indexLine].color = dictColors[color];
        }
    }

    private async void CheckRemainCirclesExit()
    {
        await Task.Delay(100);
        if (lstCircles.Count > 0)
        {
            Destroy(delegate
            {
                CheckRemainCirclesExit();
            });
        }
    }

    private void CreateSingleCircle(int i)
    {
        GameObject ob = Instantiate(prefab);
        ob.SetActive(false);
        ob.transform.SetParent(points[i]);
        ob.transform.localScale = Vector3.zero;
        ob.transform.localPosition = Vector3.zero;
        ob.name = i.ToString();
        if (!lstPointsInUse.Contains(i))
            lstPointsInUse.Add(i);
        Circle circle = ob.GetComponent<Circle>();
        if (circle != null)
        {
            string color = circlesNames[Random.Range(0, circlesNames.Length)];
            circle.actionSetTotalScore = (score) =>
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(bubblePopSound);
                }
                if (score > 0)
                    SetPlusScore(score);
                SetActiveLines(false);
            };
            circle.actionActiveLine = (index) =>
            {
                if (lines != null && lines.Length > index)
                    lines[index].gameObject.SetActive(true);
            };
            circle.actionWrong = delegate
            {
                if (imgRedWarning != null)
                    imgRedWarning.SetTrigger("red_warning");
                lifes--;
                if (circleLifes != null && circleLifes.Length > lifes)
                    circleLifes[lifes].SetActive(false);
                if (lifes <= 0)
                    GameOver();
            };
            circle.actionRecreateCircles = (circle_ob) => { ClearAndCheckRecreateCircles(circle_ob); };
            circle.SetImage(color, i, null);
            if (dictColors.ContainsKey(color))
                lines[i].color = dictColors[color];
        }
        lstCircles.Add(circle);
    }

    private async void SetPlusScore(long score)
    {
        if (txPlusScore == null) return;
        txPlusScore.text = string.Format("+{0}", score);
        if (txPlusScoreAnimator != null)
        {
            txPlusScoreAnimator.SetTrigger("fade_move_up");
        }
        await Task.Delay(1000);
        txPlusScore.text = string.Empty;
        totalScore += score;
        txTotalScore.text = totalScore.ToString();
        if (txTotalScoreAnimator != null)
        {
            if (txTotalScoreAnimator.enabled == false)
                txTotalScoreAnimator.enabled = true;
            txTotalScoreAnimator.SetTrigger("scale_1_12");
        }
        if (currentFlagScore >= flagScores.Length - 1) return;
        if (totalScore > flagScores[currentFlagScore])
        {
            CreateSingleCircle(indexNumberCircle);
            if (indexNumberCircle < points.Length - 1)
                indexNumberCircle++;
            if (currentFlagScore < flagScores.Length - 1)
                currentFlagScore++;
            if (bigCircle != null)
            {
                float speed = bigCircle.speed;
                speed += bigCircle.speedIncreaseByTime;
                if (speed > 260)
                    speed = 260;
                bigCircle.speed = speed;
            }
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        if (bigCircle != null)
            bigCircle.speed = bigCircle.GetDefaultSpeed();
        if (btBack != null)
            btBack.SetActive(false);
        if (circleLifes != null && circleLifes.Length > 1)
        {
            circleLifes[0].SetActive(false);
            circleLifes[1].SetActive(false);
        }
        if (uiStart != null)
        {
            uiStart.SetActive(true);
            SetActiveBtFreeze(false);
            int totalNumberFreeze = PlayerPrefs.GetInt("number_freeze", 0);
            totalNumberFreeze += numberFreeze;
            SaveTotalNumberFreeze(totalNumberFreeze);
            Destroy(delegate
            {
                uiStart.Run();
                ActiveBestScore();
            });
        }
    }

    private void SaveTotalNumberFreeze(int total)
    {
        PlayerPrefs.SetInt("number_freeze", total);
        PlayerPrefs.Save();
    }

    private void ActiveBestScore()
    {
        string score = PlayerPrefs.GetString("score", "0");
        long bestScore = int.Parse(score);
        if (totalScore > bestScore)
            bestScore = totalScore;
        if (txBestScore != null && bestScore > 0)
            txBestScore.text = string.Format("Best : {0}", bestScore);
        PlayerPrefs.SetString("score", bestScore.ToString());
        PlayerPrefs.Save();
    }

    private async void Destroy(Action action)
    {
        if (lstCircles.Count == 0) return;
        for (int i = 0; i < lstCircles.Count; i++)
        {
            if (lstCircles[i] != null)
            {
                lstCircles[i].Destroy();
                await Task.Delay(100);
            }
        }
        await Task.Delay(400);
        action?.Invoke();
    }

    private void SetActiveLines(bool active)
    {
        if (lines == null || lines.Length == 0) return;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != null)
                lines[i].gameObject.SetActive(active);
        }
    }

}
