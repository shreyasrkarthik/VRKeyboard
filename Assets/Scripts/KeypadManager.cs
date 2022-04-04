using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class KeypadManager : MonoBehaviour
{
    public TMP_Text inputText;
    public TMP_Text followText;
    public TMP_Text wpmText;
    public TMP_Text errorRateText;
    bool flagTimer = true;
    DateTime startTime;
    DateTime endTime;
    string startStr;
    int startInt;

    // Start is called before the fiorst frame update
    void Start()
    {

    }

    //Update is called once per frame
    void Update()
    {

    }

    // This method updates the text in the input 
    public void UpdateText(string character)
    {
        if(flagTimer) {
            flagTimer = false;
            startTime = DateTime.Now;
            startStr = startTime.ToString("HHmmss");
            startInt = Int32.Parse(startStr);
        }

        if (character == "<-") {
            if(inputText.text.Length > 0) {
                int string_length = inputText.text.Length;
                inputText.text = inputText.text.Remove(string_length - 1); 
            }
        }
        else {
            inputText.text += character;
        }
    }

    public void ComputeMetricsOnSubmit() {

        // Compute Error Rate
        string errorRate = getErrorRateAsString(followText.text.ToUpper(), inputText.text.ToUpper());
        errorRateText.text = "Error Rate: " + errorRate + " %";

        Debug.Log("Follow text: " + followText.text);
        Debug.Log("Input text: " + inputText.text);
        Debug.Log("Error Rate: " + errorRate);

        // Calculate words per minute
        endTime = DateTime.Now;
        int temp = Int32.Parse(endTime.ToString("HHmmss")) - startInt;

        int length = inputText.text.Length;
        float wpmVal = ((length * 60 * 0.2f) / temp);
        wpmText.text =  "WPM: " + wpmVal.ToString();
        Debug.Log("WPM: " + errorRate);

    }

    private string getErrorRateAsString(string s, string t)
    {
        int errors = LevenshteinDistance(s, t);
        float error_rate = 100 * ((float) errors / s.Length);
        return error_rate.ToString("0.00");
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}

