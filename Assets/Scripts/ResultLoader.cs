using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.IO;
using System.Globalization;

#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ResultLoader : MonoBehaviour
{
    public TMP_Text sentence1;
    public TMP_Text wpm1;
    public TMP_Text error_rate1;

    public TMP_Text sentence2;
    public TMP_Text wpm2;
    public TMP_Text error_rate2;

    public TMP_Text sentence3;
    public TMP_Text wpm3;
    public TMP_Text error_rate3;

    public TMP_Text sentence4;
    public TMP_Text wpm4;
    public TMP_Text error_rate4;

    public TMP_Text sentence5;
    public TMP_Text wpm5;
    public TMP_Text error_rate5;

    public TMP_Text average_wpm;
    public TMP_Text average_error;

    double avg_error;
    double avg_wpm;
    // Start is called before the first frame update
    void Start()
    {
        loadResults();
    }

    void loadResults() {
        string[] lines = System.IO.File.ReadAllLines(getPath());
        int lineCount = lines.Length;
        int val = lineCount - 5;
        for(int i = lineCount - 5; i < lineCount; i++) {
            string[] data = lines[i].Split(',');
            if(data[0] == AnalyticsSessionInfo.sessionId.ToString()) {
                string sentence = data[5];
                string wpm = data[6];
                string error_rate = data[7];
                avg_wpm += Convert.ToDouble(wpm);
                avg_error += Convert.ToDouble(error_rate);
                addToScreen(sentence, wpm, error_rate, i - val);
            }
        }
        avg_error /= 5.0;
        average_error.text = avg_error.ToString();
        average_wpm.text = avg_wpm.ToString();
    }

    void addToScreen(string sentence, string wpm, string error_rate, int num) {
        if(num == 0) {
            sentence1.text = sentence;
            wpm1.text = wpm;
            error_rate1.text = error_rate;
        }
        else if(num == 1) {
            sentence2.text = sentence;
            wpm2.text = wpm;
            error_rate2.text = error_rate;
        }
        else if(num == 2) {
            sentence3.text = sentence;
            wpm3.text = wpm;
            error_rate3.text = error_rate;
        }
        else if(num == 3) {
            sentence4.text = sentence;
            wpm4.text = wpm;
            error_rate4.text = error_rate;
        }
        else if(num == 4) {
            sentence5.text = sentence;
            wpm5.text = wpm;
            error_rate5.text = error_rate;
        }
    }

    private string getPath(){
        #if UNITY_EDITOR
        Debug.Log("Unity");
        Debug.Log(Application.dataPath.ToString());
        return Application.dataPath +"/CSV/"+"keyboardDataTeam4.csv";
        #elif UNITY_ANDROID
        return Application.persistentDataPath+"keyboardDataTeam4.csv";
        #elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"keyboardDataTeam4.csv";
        #else
        Debug.Log("else");
        Debug.Log(Application.dataPath.ToString());
        return Application.dataPath +"/"+"keyboardDataTeam4.csv";
        Debug.Log(Application.dataPath);
        #endif
    }
}

