using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.IO;

//things to add:
//a message noticing next trial (2s)
//a black screen for a few seconds between each trial (2s)
//A menu page to enter the experiment
//a better randomizer and counterbalance
//create log file

public class TrialController : MonoBehaviour
{
    //public GameObject nextTrialText;
    //private bool startNextTrial = false;

    //recording variables
    private string fileName;

    public int varDanger;

    public GameObject dangerousObs;
    public GameObject nondangerousObs;

    public TextMeshProUGUI countText;
    public GameObject endExpText;
    public GameObject pauseMenuUI;
    public GameObject halfWayUI;
    public GameObject AffordanceType;
    public GameObject AffordanceLimit;
    public GameOjbect StartMenu;

    private string endPos;

    public int count;
    private int totalCnt = 6;

    //variables for controlling controller
    private InputDevice targetDevice;

    // Start is called before the first frame update
    void Start()
    {
        //import all the controllers
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        if (devices.Count > 0) {
            targetDevice = devices[0];
        }

        //set the condition and heights
        varDanger = Random.Range(0,2);
        calculateHeights();

        //begin the first trial walking towards startPos2
        endPos = "startPos2";
        nextTrial();

        //initiate menus and text
        pauseMenuUI.SetActive(false);
        count = 0;
        SetCountText();
        endExpText.SetActive(false);


        //initiate recording devices
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Data_Logs/");
        CreateTextFile();
    }

    // Update is called once per frame
    void Update()
    {
        //recording the position according to time
        //Debug.Log(trialNum);
        if (transform.position.x < 0.01f && transform.position.x > -0.01f && count < totalCnt)
        {
            //Debug.Log("print");
            //Debug.Log(Time.time + ", " + prevTime);
            File.AppendAllText(fileName, count + "     " + record[record.Count - 1] + "       " + transform.position.ToString() + "\n");
        }
    }

    void SetCountText()
    {
        if (count == (totalCnt + 1)/2) {
            varDanger = Mathf.Abs(varDanger - 1);
            //recount and record
            record.Clear();
            //display half way panel
            StartCoroutine(WaitNextTrial(8f, halfWayUI));
        }

        if (count > totalCnt)
        {
            //pauseMenuUI.SetActive(false);
            endExpText.SetActive(true);
            Time.timeScale = 0f;
            //end game and terminate
        }
        else {
            countText.text = "Trial: " + (count+1).ToString() + "/" + totalCnt;
        }
    }


    public List<string> record = new List<string>(21);
    void nextTrial() {
        //generate random position of obstacle, make sure each position is only generated once
        int varType = Random.Range(0, 3);
        int varHeight = Random.Range(0, 7);
        string newCombo = varType + "," + varHeight;
        while (record.Contains(newCombo))
        {
            varType = Random.Range(0, 3);
            varHeight = Random.Range(0, 7);
            newCombo = varType + "," + varHeight;
        }

        record.Add(newCombo);

        //set the position for the trial
        setPosition(varType, varHeight);

        //transition between trials
        count++;
        //SetCountText();
        //StartCoroutine(WaitNextTrial(2f, pauseMenuUI));
        UIController();
        //update trial number on UI
    }

    private void UIController() {
        SetCountText();
        if (count != 0 && count != totalCnt)
        {
            StartCoroutine(WaitNextTrial(2f, pauseMenuUI));
        }

        AffordanceType.SetActive(true);
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue) {
            Debug.Log("PrimaryButton pressed");
            AffordanceType.SetActive(false);
            AffordanceLimit.SetActive(true);
            if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
            {
                Debug.Log("SecondaryButton pressed");
                AffordanceLimit.SetActive(false);
            }
        }

    }

    //set position of obstacle
    private void setPosition(int varType, int varHeight) {
        if (varDanger == 1)
        {
            dangerousObs.SetActive(true);
            nondangerousObs.SetActive(false);

            //head height type
            if (varType == 0)
            {
                dangerousObs.transform.position = new Vector3(0, heightsHead[varHeight], 0);
                Debug.Log("headHeightDanger");
            }
            //Waist height type
            else if (varType == 1)
            {
                dangerousObs.transform.position = new Vector3(0, heightsWaist[varHeight], 0);
                Debug.Log("waistHeightDanger");
            }
            //Knee height type
            else if (varType == 2)
            {
                dangerousObs.transform.position = new Vector3(0, heightsKnee[varHeight], 0);
                Debug.Log("kneeHeightDanger");
            }
        }
        else if (varDanger == 0)
        {
            nondangerousObs.SetActive(true);
            dangerousObs.SetActive(false);
            //head height type
            if (varType == 0)
            {
                nondangerousObs.transform.position = new Vector3(0, heightsHead[varHeight], 0);
                Debug.Log("headHeightNon");
            }
            //Waist height type
            else if (varType == 1)
            {
                nondangerousObs.transform.position = new Vector3(0, heightsWaist[varHeight], 0);
                Debug.Log("waistHeightNon");
            }
            //Knee height type
            else if (varType == 2)
            {
                nondangerousObs.transform.position = new Vector3(0, heightsKnee[varHeight], 0);
                Debug.Log("kneeHeightNon");
            }
        }
    }

    //pause menu for 2 seconds
    public IEnumerator WaitNextTrial(float t, GameObject uiPanel)
    {
        uiPanel.SetActive(true);
        yield return new WaitForSeconds(t);
        uiPanel.SetActive(false);
    }

    //input eyeHeight of participant before experimentation
    public float eyeHeight = 1.5f;
    public float offset = 0.15f;

    public float[] heightsHead = new float[7];
    public float[] heightsWaist = new float[7];
    public float[] heightsKnee = new float[7];
    private void calculateHeights() {
        // initialize height array
        eyeHeight = eyeHeight + offset;

        //calculate head heights, 7 values higher and lower than eye height
        //the center value is 1 eyeheight, with 0.05 increments above
        heightsHead[0] = eyeHeight * 1f;
        heightsHead[1] = eyeHeight * 1.05f;
        heightsHead[2] = eyeHeight * 1.1f;
        heightsHead[3] = eyeHeight * 1.15f;
        heightsHead[4] = eyeHeight * 0.95f;
        heightsHead[5] = eyeHeight * 0.90f;
        heightsHead[6] = eyeHeight * 0.85f;

        //calculate Waist heights, 7 values higher and lower than eye height
        //the center value is 0.6 eyeheight, with 0.05 increments above
        heightsWaist[0] = eyeHeight * 0.6f;
        heightsWaist[1] = eyeHeight * 0.70f;
        heightsWaist[2] = eyeHeight * 0.60f;
        heightsWaist[3] = eyeHeight * 0.65f;
        heightsWaist[4] = eyeHeight * 0.55f;
        heightsWaist[5] = eyeHeight * 0.50f;
        heightsWaist[6] = eyeHeight * 0.45f;

        //calculate knee heights, 7 values higher and lower than eye height
        //the center value is 0.25 eyeheight, with 0.05 increments above
        heightsKnee[0] = eyeHeight * 0.25f;
        heightsKnee[1] = eyeHeight * 0.30f;
        heightsKnee[2] = eyeHeight * 0.35f;
        heightsKnee[3] = eyeHeight * 0.40f;
        heightsKnee[4] = eyeHeight * 0.20f;
        heightsKnee[5] = eyeHeight * 0.15f;
        heightsKnee[6] = eyeHeight * 0.10f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (endPos == "startPos1" && other.tag == "startPos1")
        {
            //Debug.Log("startPos1");
            endPos = "startPos2";
            nextTrial();
        }

        if (endPos == "startPos2" && other.tag == "startPos2")
        {
            //Debug.Log("startPos2");
            endPos = "startPos1";
            nextTrial();
        }
    }

    //create file for recording
    public void CreateTextFile()
    {
        fileName = Application.streamingAssetsPath + "/Data_Logs/" + "Participant1" + ".txt";

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, "Participant 1 Log:" + "\n");
        }
    }
}
