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
        //Debug.Log(trialNum);
        if (transform.position.x < 0.01f && transform.position.x > -0.01f && trialNum < 42)
        {
            //Debug.Log("print");
            //Debug.Log(Time.time + ", " + prevTime);
            File.AppendAllText(fileName, trialController.count + "     " + trialController.record[trialController.record.Count - 1] + "       " + transform.position.ToString() + "\n");
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
