using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;

public class PrePostTest : MonoBehaviour
{
    DataRecord logScript;

    public int varDanger = InfoLog.obstacleType;//0 is nondangerous, 1 is dangerous
    public int experimentPart = 1; //0 Pretest, 1 calibration, 2 posttest
    public int avatar = InfoLog.avatar;

    private GameObject obstacle;
    public GameObject dangerousObs;
    public GameObject nondangerousObs;
    private float obstacleOriginX;
    private float obstacleOriginY;
    private float obstacleOriginZ;

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
    bool updateStopper = false;

    public int count;
    private int totalCnt = 2;

    // Start is called before the first frame update
    void Start()
    {
        //initiate menus and text
        varDanger = InfoLog.obstacleType;//0 is nondangerous, 1 is dangerous
        avatar = InfoLog.avatar;
        eyeHeight = InfoLog.eyeHeight;

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
        else
        {
            obstacle = nondangerousObs;
        }
        calculateHeights();

        logScript = GetComponent<DataRecord>();
    }


    // Update is called once per frame
    void Update()
    {
        //only trigger the close of the menu if the menu is active, it has not been set before, and the trigger is pressed
        if (RotateWithUser.initialized && startMenuUI.activeSelf && !startSet && Input.GetKeyDown(KeyCode.G)
            || (RotateWithUser.initialized && startMenuUI.activeSelf && !startSet && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any)))
        {
            startSet = true;
            StartCoroutine(WaitStartTrial(2f));
            Debug.Log("Start");
            obstacleOriginX = obstacle.transform.position.x;
            obstacleOriginY = obstacle.transform.position.y;
            obstacleOriginZ = obstacle.transform.position.z;
        }

        //when the pause menu disappears, initiate affordance type menu
        if (startSet && pauseSet && !confirmSet && !confirmUI.activeSelf)
        {
            if (Input.GetKey(KeyCode.P)
            || SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.Any))
            {
                //translates upwards for low trials
                if (varHeight == 0)
                {
                    if(obstacle.transform.position.y <= experimentHeights[1]){
                        obstacle.transform.position = obstacle.transform.position + new Vector3(0, 0.005f, 0);
                    }
                    else{
                        obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[0], obstacleOriginZ);
                    }
                }
                else
                {
                    if (obstacle.transform.position.y >= experimentHeights[0]){
                        obstacle.transform.position = obstacle.transform.position + new Vector3(0, -0.005f, 0);
                    }
                    else{
                        obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[1], obstacleOriginZ);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.G)
            || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log(updateStopper + "confirm menu");
                if (updateStopper == false)
                {

                    updateStopper = true;
                    confirmUI.SetActive(true);
                    startAdjustmentText.SetActive(false);
                }
                else
                {
                    updateStopper = false;
                }
                Debug.Log(updateStopper + "confirm menu");
            }

        }

        if (startSet && confirmUI.activeSelf && !confirmSet)
        {
            //No, do not confirm, return to previous
            if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log(updateStopper + "not confirmed");
                if (updateStopper == false)
                {
                    confirmUI.SetActive(false);
                    startAdjustmentText.SetActive(true);
                }
                else
                {
                    updateStopper = false;
                }
                Debug.Log(updateStopper + "not confirmed");
            }

            //Yes, confirm and proceed to next trial
            if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log(updateStopper + "confirmed");
                if (updateStopper == false)
                {
                    updateStopper = true;
                    confirmUI.SetActive(false);
                    confirmSet = true;
                    startAdjustmentText.SetActive(false);

                    //record reading

                    string[] log = new string[5] {
                        (count+1).ToString(),
                        experimentPart.ToString(),
                        varDanger.ToString(),
                        varHeight.ToString(),
                        obstacle.transform.position.y.ToString()
                    };
                    logScript.AppendToReport(log);
                    nextTrial();
                }
                else
                {
                    updateStopper = false;
                }
                Debug.Log(updateStopper + "confirmed");
            }
        }
    }

    //pause for 2 seconds before starting the first trial
    public IEnumerator WaitStartTrial(float t)
    {
        yield return new WaitForSeconds(t);
        startMenuUI.SetActive(false);
        //so that the first trial is only calle once
        //startSet = false;
        nextTrial();
    }

    //pause for 0.5 seconds before confirming in confirm window
    public IEnumerator WaitEnd(float t)
    {
        yield return new WaitForSeconds(t);
        RotateWithUser.initialized = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void SetCountText()
    {
        if (count > totalCnt)
        {
            pauseMenuUI.SetActive(false);
            startAdjustmentText.SetActive(false);
            endExpText.SetActive(true);
            if(experimentPart == 0)
            {
                StartCoroutine(WaitEnd(2f));
            }
            //Time.timeScale = 0f;
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
        updateStopper = false;

        if (count == 0)
        {
            varHeight = 0;
            //varHeight = Random.Range(0, 2);
        }
        else
        {
            varHeight = 1;
            //varHeight = Mathf.Abs(varHeight - 1);
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
        obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[varHeight], obstacleOriginZ);
    }

    //input eyeHeight of participant before experimentation
    public float eyeHeight = InfoLog.eyeHeight;
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
