using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using System.IO;


public class PrePost : MonoBehaviour
{
    //recording variables
    private string fileName;
    bool trial;
    int recordedHeight;

    DataRecord logScript;

    public int varDanger = 1;//0 is nondangerous, 1 is dangerous
    private GameObject obstacle;
    public GameObject dangerousObs;
    public GameObject nondangerousObs;

    public TextMeshProUGUI countText;
    public GameObject endExpText;
    public GameObject startAdjustmentText;

    public GameObject pauseMenuUI; //nextTrial
    public GameObject confirmUI; //confirm adjustment
    public GameObject startMenuUI; //starting menu

    //variables for trigger events
    //bool typeSet = false;
    bool confirmSet = false;
    bool startSet = false;
    bool pauseSet = false;

    public int count;
    private int totalCnt = 2;

    // Start is called before the first frame update
    void Start()
    {
        //initiate menus and text
        count = 0;
        SetCountText();
        startMenuUI.SetActive(true);
        endExpText.SetActive(false);
        pauseMenuUI.SetActive(false);
        startAdjustmentText.SetActive(false);
        confirmUI.SetActive(false);

        nondangerousObs.SetActive(false);
        dangerousObs.SetActive(false);

        //set the condition and heights
        if (varDanger == 1)
        {
            obstacle = dangerousObs;
        }
        else {
            obstacle = nondangerousObs;
        }
        calculateHeights();

        logScript = GetComponent<DataRecord>();
    }


    // Update is called once per frame
    void Update()
    {
        //only trigger the close of the menu if the menu is active, it has not been set before, and the trigger is pressed
        if (startMenuUI.activeSelf && !startSet && Input.GetKeyDown(KeyCode.B)
            || (startMenuUI.activeSelf && !startSet && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any)))
        {
            startSet = true;
            StartCoroutine(WaitStartTrial(2f));
            Debug.Log("Start");
        }

        //when the pause menu disappears, initiate affordance type menu
        if (pauseSet && !confirmSet && !confirmUI.activeSelf)
        {
            if (Input.GetKey(KeyCode.U)
                || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                obstacle.transform.position = obstacle.transform.position + new Vector3(0, 0.02f, 0);
            }

            if (Input.GetKey(KeyCode.I)
                || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                obstacle.transform.position = obstacle.transform.position + new Vector3(0, -0.02f, 0);
            }

            //if the trigger is pressed, open confirm menu
            //if (!confirmUI.activeSelf)
            //{
                if (Input.GetKeyDown(KeyCode.Y)
                    || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    //record result
                    Debug.Log("Confirm pressed");
                    confirmUI.SetActive(true);
                    startAdjustmentText.SetActive(false);
                }
            //}

        }

        if (confirmUI.activeSelf && !confirmSet)
        {
            //No, do not confirm, return to previous
            if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("Not confirmed");
                confirmUI.SetActive(false);
                startAdjustmentText.SetActive(true);
            }

            //Yes, confirm and proceed to next trial
            if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.M))
            {
                //StartCoroutine(WaitStartTrial(0.5f));
                Debug.Log("Confirmed");
                confirmUI.SetActive(false);
                confirmSet = true;
                startAdjustmentText.SetActive(false);

                //record reading

                string[] log = new string[4] {
                    count.ToString(),
                    varDanger.ToString(),
                    varHeight.ToString(),
                    obstacle.transform.position.y.ToString()
                };
                logScript.AppendToReport(log);

                nextTrial();
            }
        }

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

    //pause for 0.5 seconds before confirming in confirm window
    public IEnumerator WaitConfirm(float t)
    {
        yield return new WaitForSeconds(t);
    }

    void SetCountText()
    {
        if (count > totalCnt)
        {
            pauseMenuUI.SetActive(false);
            startAdjustmentText.SetActive(false);
            endExpText.SetActive(true);
            Time.timeScale = 0f;
            //end game and terminate
        }
        else
        {
            countText.text = "Trial: " + (count).ToString() + "/" + totalCnt;
        }
    }

    int varHeight;
    void nextTrial()
    {
        //generate random position of obstacle, make sure each position is only generated once
        //set all menus to false, change set menus to true after each set
        confirmSet = false;
        pauseSet = false;

        if (count == 0)
        {
            varHeight = Random.Range(0, 2);
            Debug.Log("Start" + varHeight);
        }
        else {
            varHeight = Mathf.Abs(varHeight - 1);
            Debug.Log("End" + varHeight);
        }

        //set the position for the trial
        setPosition(varHeight);

        //transition between trials
        count++;
        UIController();
    }

    //controll the appearing 
    private void UIController()
    {

        SetCountText();

        //initiate pause menu UI
        if (count == 1)
        {
            pauseSet = true;
            startAdjustmentText.SetActive(true);
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
        startAdjustmentText.SetActive(true);
        pauseSet = true;
    }

    //set position of obstacle
    private void setPosition(int varHeight)
    { 
        obstacle.SetActive(true);
        obstacle.transform.position = new Vector3(0, experimentHeights[varHeight], 0);
    }

    //input eyeHeight of participant before experimentation
    public float eyeHeight = 1.5f;
    public float offset = 0.15f;

    private float[] experimentHeights = new float[2];

    private void calculateHeights()
    {
        float stepOverThreshold = eyeHeight * 0.52f;

        //the lower boundery
        experimentHeights[0] = stepOverThreshold * 0.7f;
        experimentHeights[1] = stepOverThreshold * 1.4f;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "startPos1")
    //    {
    //        Debug.Log("in bounds");
    //    }
    //    else {
    //        //Debug.Log("out of bounds");
    //    }
    //}
}
