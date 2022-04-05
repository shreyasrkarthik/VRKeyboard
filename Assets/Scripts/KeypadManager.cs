using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.IO;
using Random = UnityEngine.Random;

#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif
public class KeypadManager : MonoBehaviour
{
    public TMP_Text inputText;
    public TMP_Text followText;
    public TMP_Text wpmText;
    public TMP_Text errorRateText;
    public string ResultSceneName;
    public string KeyboardType;
    public string InputType;
    public bool AudioFeedbackFlag;
    public bool VisualFeedbackFlag;
    bool flagTimer = true;
    DateTime startTime;
    DateTime endTime;
    string startStr;
    int startInt;
    static string[] sentences = {"Good to know it exists", "Need to watch closely", "I am on a conference call", "Please revise accordingly", "I am glad you are involved",
     "Thanks again for your help", "It is not working very well"};
    int sceneNumber;
    int randomInt;
    List<string> wpmRates = new List<string>();
    List<string> errorRates = new List<string>();
    private System.Random rand;
    private List<string[]> rowData = new List<string[]>();
    int id;
    string filePath;


    // Start is called before the fiorst frame update
    void Start()
    {
        VisualFeedbackFlag = true;
        createCSV();
        id = 1;

        randomInt = Random.Range(0, sentences.Length);
        sceneNumber = 0;
        followText.text = sentences[randomInt];
        Debug.Log("Start:   " + AnalyticsSessionInfo.userId + " " + AnalyticsSessionInfo.sessionState + " " + AnalyticsSessionInfo.sessionId + " " + AnalyticsSessionInfo.sessionElapsedTime);
 
    }

    //Update is called once per frame
    void Update()
    {

    }

    // This method updates the text in the input 
    public void UpdateText(string character)
    {
        if(flagTimer == true) {
            flagTimer = false;
            startTime = DateTime.Now;
            startStr = startTime.ToString("HHmmss");
            startInt = Int32.Parse(startStr);
            inputText.text = "";
            Debug.Log("Input: "+ inputText.text);
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
        Debug.Log("WPM: " + wpmVal.ToString());

        randomInt += 1;
        randomInt = randomInt % 6;
        sceneNumber += 1;

        if (sceneNumber == 6) {
            // Load result Scene;
            SceneManager.LoadScene(ResultSceneName);
        }
        else {
            // Save wpm and error rates and then load new sentence.
            addDataToCSV(followText.text, wpmVal.ToString(), errorRate);
            
            // Set new sentence settings
            wpmRates.Add(wpmVal.ToString());
            errorRates.Add(errorRate);
            followText.text = sentences[randomInt];
            inputText.text = "";
        }

    }


    private void addDataToCSV(string sentence, string wpm, string error_rate) {
        
        string sb = AnalyticsSessionInfo.sessionId.ToString() + "," + AudioFeedbackFlag.ToString() + "," + VisualFeedbackFlag.ToString() + "," + KeyboardType + "," + InputType + "," + sentence + "," + wpm + "," + error_rate;
        StreamWriter outStream = System.IO.File.AppendText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        id += 1;
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

    //Credits: https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html
    void createCSV() {

        //AutoIncrementUserID,AudioFeedback,Hapticfeedback,KeyboardType,InputType,Sentence,WPM,ErrorRate
        // Creating First row of titles manually..

        Debug.Log("created csv");
        string[] rowDataTemp = new string[8];
        rowDataTemp[0] = "SessionID";
        rowDataTemp[1] = "AudioFeedback";
        rowDataTemp[2] = "VisualFeedback";
        rowDataTemp[3] = "KeyboardType"; // 3D or 2D
        rowDataTemp[4] = "InputType"; // Hand or Controller
        rowDataTemp[5] = "Sentence";
        rowDataTemp[6] = "WPM";
        rowDataTemp[7] = "ErrorRate";
        rowData.Add(rowDataTemp);

        // You can add up the values in as many cells as you want.
        // for(int i = 0; i < 10; i++){
        //     rowDataTemp = new string[3];
        //     rowDataTemp[0] = "Sushanta"+i; // name
        //     rowDataTemp[1] = ""+i; // ID
        //     rowDataTemp[2] = "$"+UnityEngine.Random.Range(5000,10000); // Income
        //     rowData.Add(rowDataTemp);
        // }

        string[][] output = new string[rowData.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        Debug.Log("before path");

        filePath = getPath();
        Debug.Log("cnnreated csv");
        StreamWriter outStream;
        if(!System.IO.File.Exists(filePath)) {
            outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }
    }

    private string getPath(){
        Debug.Log("im here");

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

