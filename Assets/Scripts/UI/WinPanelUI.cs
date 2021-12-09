using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WinPanelUI : MonoBehaviour
{
    // Start is called before the first frame update
    public string timeFilename = "timeLeaderBoard";
    public string deathFilename = "deathLeaderBoard";
    public GameObject timeSpentUI;
    public GameObject deathCountUI;
    public string timeJsonFile => Application.persistentDataPath + '/' + timeFilename + ".json";
    public string deathJsonFile => Application.persistentDataPath + '/' + deathFilename + ".json";

    public int timeRecordSize = 3;
    public GameObject[] timeLeaderboard = new GameObject[3];
    private List<TimeRecord> timeRecords = new List<TimeRecord>();

    public int deathRecordSize = 3;
    public GameObject[] deathLeaderboard = new GameObject[3];
    private List<DeathRecord> deathRecords = new List<DeathRecord>();


    private TimeSpan timeSpentSpan;
    private int deathCount;
    private bool fadeIn;

    private IEnumerator Animation()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        Transform[] things =
        {
            transform.Find("Completed"), timeSpentUI.transform,
            deathCountUI.transform, transform.Find("Others")
        };
        foreach (var thing in things)
        {
            thing.GetComponent<CanvasGroup>().alpha = 0;
        }

        yield return FadeIn(transform, 0f, 4f, 0.5f);
        yield return new WaitForSecondsRealtime(1f);
        foreach (var thing in things)
        {
            yield return FadeIn(thing, 0f, 6f, 3f);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void OnEnable()
    {
        PrepareWinPanel();
    }

    private void PrepareWinPanel()
    {
        timeSpentSpan = DateTime.Now - TimeManager.Instance.startTime;
        deathCount = LevelManager.Instance.fallTime;
        SetTimeSpent();
        SetDeathCount();
        ReadLeaderBoard();
        UpdateTimeLeaderBoard();
        UpdateDeathLeaderBoard();
        DisplayAndSaveTimeLeaderBoard();
        DisplayAndSaveDeathLeaderBoard();
        StartCoroutine(Animation());
    }

    private string ToTextFromTimeSpan(TimeSpan ts, bool isNull = false)
    {
        return isNull ? "XX:XX.XX" : ts.ToString().Substring(3, 8);
    }

    private void SetTimeSpent()
    {
        timeSpentUI.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Time";

        timeSpentUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ToTextFromTimeSpan(timeSpentSpan);
    }

    private void SetDeathCount()
    {
        deathCountUI.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Death";
        deathCountUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
            deathCount.ToString(CultureInfo.InvariantCulture);
    }

    private void ReadLeaderBoard()
    {
        try
        {
            string timeRawJson = File.ReadAllText(timeJsonFile);
            TimeRecord[] tmp = JsonHelper.FromJson<TimeRecord>(timeRawJson);
            timeRecords = new List<TimeRecord>(tmp);
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.ToString());
        }

        try
        {
            string deathRawJson = File.ReadAllText(deathJsonFile);
            DeathRecord[] tmp = JsonHelper.FromJson<DeathRecord>(deathRawJson);
            deathRecords = new List<DeathRecord>(tmp);
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void UpdateTimeLeaderBoard()
    {
        string now = DateTime.Now.ToString("MM/dd hh:mm");
        double timeSpentSeconds = timeSpentSpan.TotalSeconds;
        int count = timeRecords.Count;
        if (count < timeRecordSize)
        {
            timeRecords.Add(new TimeRecord
            {
                timeSpent = timeSpentSeconds,
                date = now
            });
        }
        else
        {
            for (int i = 0; i < timeRecordSize; i++)
            {
                if (timeSpentSeconds <= timeRecords[i].timeSpent)
                {
                    for (int j = timeRecordSize - 1; j > i; j--)
                    {
                        timeRecords[j] = timeRecords[j - 1];
                    }


                    timeRecords[i] = new TimeRecord
                    {
                        timeSpent = timeSpentSeconds,
                        date = now
                    };
                    break;
                }
            }
        }
    }

    private void UpdateDeathLeaderBoard()
    {
        string now = DateTime.Now.ToString("MM/dd hh:mm");
        int count = deathRecords.Count;
        if (count < deathRecordSize)
        {
            deathRecords.Add(new DeathRecord
            {
                deathCount = deathCount,
                date = now
            });
        }
        else
        {
            for (int i = 0; i < deathRecordSize; i++)
            {
                if (deathCount <= deathRecords[i].deathCount)
                {
                    for (int j = deathRecordSize - 1; j > i; j--)
                    {
                        deathRecords[j] = deathRecords[j - 1];
                    }


                    deathRecords[i] = new DeathRecord
                    {
                        deathCount = deathCount,
                        date = now
                    };
                    break;
                }
            }
        }
    }

    private void DisplayAndSaveTimeLeaderBoard()
    {
        for (int i = 0; i < timeRecordSize; i++)
        {
            var tmp = timeLeaderboard[i];
            if (i < timeRecords.Count)
            {
                tmp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
                    ToTextFromTimeSpan(TimeSpan.FromSeconds(timeRecords[i].timeSpent));
                tmp.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = timeRecords[i].date;
            }
            else
            {
                tmp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = "";
                tmp.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        string timeJsonData = JsonHelper.ToJson(timeRecords.ToArray());

        File.WriteAllText(timeJsonFile, timeJsonData);
    }

    private void DisplayAndSaveDeathLeaderBoard()
    {
        for (int i = 0; i < deathRecordSize; i++)
        {
            var tmp = deathLeaderboard[i];
            if (i < deathRecords.Count)
            {
                tmp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
                    deathRecords[i].deathCount.ToString(CultureInfo.InvariantCulture);
                tmp.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = deathRecords[i].date;
            }
            else
            {
                tmp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = "";
                tmp.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        string deathJsonData = JsonHelper.ToJson(deathRecords.ToArray());
        File.WriteAllText(deathJsonFile, deathJsonData);
    }

    IEnumerator FadeIn(Transform target, float wait, float duration, float powBase)
    {
        float start = 0f;
        float progress = (Time.time - start) / duration;

        while (progress < 1f)
        {
            start += Time.fixedDeltaTime;
            progress = start / duration;
            float alpha = Mathf.Pow(progress, powBase);

            target.GetComponent<CanvasGroup>().alpha = alpha;
            yield return null;
        }

        target.GetComponent<CanvasGroup>().alpha = 1f;
    }

    [System.Serializable]
    public class TimeRecord
    {
        public string date;
        public double timeSpent;
    }

    [System.Serializable]
    public class DeathRecord
    {
        public string date;
        public int deathCount;
    }
}