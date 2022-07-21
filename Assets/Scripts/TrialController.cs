using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using System.IO;

//things to add:
//create log file

public class TrialController : MonoBehaviour
{
    //recording variables
    private string fileName;
    bool questType;
    bool questLimit;

    public int varDanger;

    public GameObject dangerousObs;
    public GameObject nondangerousObs;

    public TextMeshProUGUI countText;
    public GameObject endExpText;

    public GameObject pauseMenuUI;
    public GameObject halfWayUI;
    public GameObject AffordanceType;
    public GameObject AffordanceLimit;
    public GameObject startMenuUI;

    //variables for trigger events
    bool typeSet = false;
    bool limitSet = false;
    bool startSet = false;
    bool pauseSet = false;

    private string endPos;

    public int count;
    private int totalCnt = 6;

    // Start is called before the first frame update
    void Start()
    {
        //change to start game after clicking primary button
        //StartCoroutine(WaitNextTrial(10f, startMenuUI));
        //initiate menus and text
        count = 0;
        SetCountText();
        startMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        AffordanceType.SetActive(false);
        AffordanceLimit.SetActive(false);
        halfWayUI.SetActive(false);
        endExpText.SetActive(false);

        //set the condition and heights
        varDanger = Random.Range(0,2);
        calculateHeights();

        //begin the first trial walking towards startPos2
        endPos = "startPos2";
        //nextTrial();


        //initiate recording devices
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Data_Logs/");
        CreateTextFile();
    }


    // Update is called once per frame
    void Update()
    {
        //recording the position according to time
        //Debug.Log(trialNum);
        if (transform.position.x < 0.01f && transform.position.x > -0.01f && nextTrialStart)
        {
            //Debug.Log("print");
            //Debug.Log(Time.time + ", " + prevTime);
            nextTrialStart = false;
            File.AppendAllText(fileName, count + "     " + record[record.Count - 1] + "       " + transform.position.ToString() + "\n");
        }

        //only trigger the close of the menu if the menu is active, it has not been set before, and the trigger is pressed
        if (startMenuUI.activeSelf && !startSet && Input.GetKeyDown(KeyCode.B)
            || (startMenuUI.activeSelf && !startSet && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any)))
        {
           startSet = true;
            StartCoroutine(WaitStartTrial(2f));
            Debug.Log("Start");
            //startMenuUI.SetActive(false);
        }

        ////initiate first trial when the start menu has been set and is now nonactive
        //if (startSet && !startMenuUI.activeSelf) {
        //}

        //when the pause menu disappears, initiate affordance type menu
        if (pauseSet)
        {
            AffordanceType.SetActive(true);
        }

        //when the affordance type menu dissappears, initiate affordance limit menu
        if (typeSet)
        {
            AffordanceLimit.SetActive(true);
        }

        if(AffordanceType.activeSelf && !typeSet)
        {
            if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.Y))
            {
                //record result
                Debug.Log("Trigger pressed");
                questType = true;

                pauseSet = false;
                typeSet = true;
                AffordanceType.SetActive(false);
            }
            else if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.N)) {
                //record result
                Debug.Log("Grip pressed");
                questType = false;

                pauseSet = false;
                typeSet = true;
                AffordanceType.SetActive(false);
            }

        }

        if (AffordanceLimit.activeSelf && !limitSet) {
            if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.Y))
            {
                //record result
                Debug.Log("Trigger pressed");
                questLimit = true;

                typeSet = false;
                limitSet = true;
                AffordanceLimit.SetActive(false);
            }
            else if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.N))
            {
                //record result
                Debug.Log("Grip pressed");
                questLimit = true;

                typeSet = false;
                limitSet = true;
                AffordanceLimit.SetActive(false);
            }
        }

        //if (AffordanceType.activeSelf && !typeSet && Input.GetKeyDown(KeyCode.N)
        //    || (AffordanceType.activeSelf && !typeSet && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any)))
        //{
        //    pauseSet = false;
        //    Debug.Log("Grip pressed");
        //    typeSet = true;
        //    AffordanceType.SetActive(false);
        //}

        //if (AffordanceLimit.activeSelf && !limitSet && Input.GetKeyDown(KeyCode.M)
        //    || (AffordanceLimit.activeSelf && !limitSet && SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any)))
        //{
        //    typeSet = false;
        //    Debug.Log("Trigger pressed");
        //    limitSet = true;
        //    AffordanceLimit.SetActive(false);
        //}
    }

    //pause for 2 seconds before starting the first trial
    public IEnumerator WaitStartTrial(float t)
    {
        yield return new WaitForSeconds(t);
        startMenuUI.SetActive(false);
        //so that the first trial is only calle once
        startSet = false;
        nextTrial();
    }

    void SetCountText()
    {
        if (count > totalCnt)
        {
            pauseMenuUI.SetActive(false);
            endExpText.SetActive(true);
            Time.timeScale = 0f;
            //end game and terminate
        }
        else {
            countText.text = "Trial: " + (count).ToString() + "/" + totalCnt;
        }
    }


    public List<string> record = new List<string>(21);
    private bool nextTrialStart = false;
    void nextTrial() {
        //generate random position of obstacle, make sure each position is only generated once
        //set all menus to false, change set menus to true after each set
        typeSet = false;
        limitSet = false;
        pauseSet = false;

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
        UIController();

        nextTrialStart = true;
    }

    //controll the appearing 
    private void UIController() {

        SetCountText();

        //initiate pause menu UI
        if (count == (totalCnt / 2) + 1)
        {
            varDanger = Mathf.Abs(varDanger - 1);
            //recount and record
            record.Clear();
            //display half way panel
            StartCoroutine(WaitNextTrial(8f, halfWayUI));
        }
        //do not display pause menu for first trial
        else if (count == 1)
        {
            pauseSet = true;
        }
        //do not display pause menu after all trials end
        else if (count < totalCnt + 1)
        {
            StartCoroutine(WaitNextTrial(2f, pauseMenuUI));
        }
    }

    //set the pause menu
    public IEnumerator WaitNextTrial(float t, GameObject uiPanel)
    {
        uiPanel.SetActive(true);
        yield return new WaitForSeconds(t);
        uiPanel.SetActive(false);
        pauseSet = true;
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
        if (limitSet && endPos == "startPos1" && other.tag == "startPos1")
        {
            //Debug.Log("startPos1");
            endPos = "startPos2";
            limitSet = false;
            nextTrial();
        }

        if (limitSet && endPos == "startPos2" && other.tag == "startPos2")
        {
            //Debug.Log("startPos2");
            endPos = "startPos1";
            limitSet = false;
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
