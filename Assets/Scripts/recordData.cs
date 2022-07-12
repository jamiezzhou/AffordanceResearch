using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class recordData : MonoBehaviour
{
    private string fileName;
    private int trialNum = 1;
    TrialController trialController;


    // Start is called before the first frame update
    void Start()
    {
        trialController = GetComponent<TrialController>();
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Data_Logs/");
        CreateTextFile();
    }

    // Update is called once per frame
    void Update()
    {
        //recording the position according to time
        if (transform.position.x < 0.05f && transform.position.x > -0.05f && trialNum < 42)
        {
            //Debug.Log("print");
            //Debug.Log(Time.time + ", " + prevTime);
            File.AppendAllText(fileName, trialNum + "     " + trialController.record[trialNum] + "       " + transform.position.ToString() + "\n");
            trialNum++;
        }
    }

    public void CreateTextFile()
    {
        fileName = Application.streamingAssetsPath + "/Data_Logs/" + "Participant1" + ".txt";

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, "Participant 1 Log:" + "\n");
        }
    }
}
