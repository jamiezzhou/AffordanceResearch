using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;

public class FeedbackScript : MonoBehaviour
{
    DataRecord logScript;

    //recorded values
    public int varDanger = InfoLog.obstacleType;//0 is nondangerous, 1 is dangerous
    public int experimentPart = 0; //Block 1
    public int experimentPhase = 0; //1 adjustment phase, 0 feedback phase
    public int varHeight;
    public int judgeYN = -1;
    public float eyeHeight = InfoLog.eyeHeight;

    //UI values
    private GameObject obstacle;
    public GameObject dangerousObs;
    public GameObject nondangerousObs;
    private float obstacleOriginX;
    private float obstacleOriginY;
    private float obstacleOriginZ;

    public TextMeshProUGUI countText;
    public GameObject startMenuUI; //starting menu
    public GameObject endExpText; //end menu
    public GameObject makeJudgementText; //make judgement
    public GameObject makeAdjustmentText; //make adjustment
    public GameObject confirmUI; //confirm judgement
    public GameObject startActions; //start actions
    public GameObject pauseMenuUI; //nextTrial

    //variables for trigger events
    bool startSet = false;

    bool judgeSet = false;
    //bool adjustSet = false;
    bool confirmSet = false;
    bool pauseSet = false;
    bool updateStopper = false;

    public string endPos = "startPos1";

    public int count;
    private int totalCnt = 16;

    // Start is called before the first frame update
    void Start()
    {
        //initiate menus and text
        varDanger = InfoLog.obstacleType; //0 is nondangerous, 1 is dangerous
        eyeHeight = InfoLog.eyeHeight;

        //intialize menus
        startMenuUI.SetActive(true);
        endExpText.SetActive(false);
        pauseMenuUI.SetActive(false);
        makeJudgementText.SetActive(false);
        makeAdjustmentText.SetActive(false);
        confirmUI.SetActive(false);
        startActions.SetActive(false);

        nondangerousObs.SetActive(false);
        dangerousObs.SetActive(false);

        //set count text: 0/8
        count = 0;
        SetCountText();
        varHeight = 0;
        endPos = "startPos1";

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
        //begin experiment by pressing grip
        if (startMenuUI.activeSelf && !startSet && Input.GetKeyDown(KeyCode.G)
            || (RotateWithUser.initialized && startMenuUI.activeSelf && !startSet && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any)))
        {
            startSet = true;
            StartCoroutine(WaitStartTrial(2f));
            Debug.Log("Start");
            obstacleOriginX = obstacle.transform.position.x;
            obstacleOriginY = obstacle.transform.position.y;
            obstacleOriginZ = obstacle.transform.position.z;
        }

        //logic for recording yes and no answers for feedback phase
        if (startSet && pauseSet && !judgeSet && experimentPhase == 0)
        {
            //affordance judgement (Y Grip/N Pinch)
            if (Input.GetKey(KeyCode.N)
            || SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (updateStopper == false)
                {
                    updateStopper = true;
                    judgeYN = 0;
                    Debug.Log("affordance judgement: N");
                    makeJudgementText.SetActive(false);
                    judgeSet = true;
                    startActions.SetActive(true);

                    //record reading
                    string[] log = new string[7] {
                        count.ToString(),
                        experimentPart.ToString(),
                        experimentPhase.ToString(),
                        varHeight.ToString(),
                        varDanger.ToString(),
                        judgeYN.ToString(),
                        (-1).ToString()
                    };
                    logScript.AppendToReport(log);
                 }
                else
                {
                    updateStopper = false;
                }
            }
            if (Input.GetKey(KeyCode.Y)
            || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (updateStopper == false)
                {
                    updateStopper = true;
                    judgeYN = 1;
                    Debug.Log("affordance judgement: Y");
                    makeJudgementText.SetActive(false);
                    judgeSet = true;
                    startActions.SetActive(true);

                    //record reading
                    string[] log = new string[7] {
                        count.ToString(),
                        experimentPart.ToString(),
                        experimentPhase.ToString(),
                        varHeight.ToString(),
                        varDanger.ToString(),
                        judgeYN.ToString(),
                        (-1).ToString()
                    };
                    logScript.AppendToReport(log);
                }
                else
                {
                    updateStopper = false;
                }
            }

        }

        //logic for recording adjustment height for adjustment phase
        if (startSet && pauseSet && !confirmSet && !confirmUI.activeSelf && experimentPhase == 1)
        {
            if (Input.GetKey(KeyCode.P)
            || SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.Any))
            {
                //translates upwards for low trials
                if (varHeight == 0)
                {
                    if(obstacle.transform.position.y <= adjustmentHeights[1]){
                        obstacle.transform.position = obstacle.transform.position + new Vector3(0, 0.005f, 0);
                    }
                    else{
                        obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + adjustmentHeights[0], obstacleOriginZ);
                    }
                }
                else
                {
                    if (obstacle.transform.position.y >= adjustmentHeights[0]){
                        obstacle.transform.position = obstacle.transform.position + new Vector3(0, -0.005f, 0);
                    }
                    else{
                        obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + adjustmentHeights[1], obstacleOriginZ);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.G)
            || SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                //Debug.Log(updateStopper + "confirm menu");
                if (updateStopper == false)
                {
                    updateStopper = true;
                    confirmUI.SetActive(true);
                    makeAdjustmentText.SetActive(false);
                }
                else
                {
                    updateStopper = false;
                }
                //Debug.Log(updateStopper + "confirm menu");
            }

        }

        if (startSet && pauseSet && confirmUI.activeSelf && !confirmSet && experimentPhase == 1)
        {
            //No, do not confirm, return to previous
            if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.G))
            {
                //Debug.Log(updateStopper + "not confirmed");
                if (updateStopper == false)
                {
                    confirmUI.SetActive(false);
                    makeAdjustmentText.SetActive(true);
                }
                else
                {
                    updateStopper = false;
                }
                //Debug.Log(updateStopper + "not confirmed");
            }

            //Yes, confirm and proceed to next trial
            if (SteamVR_Actions._default.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.P))
            {
                //Debug.Log(updateStopper + "confirmed");
                if (updateStopper == false)
                {
                    updateStopper = true;
                    confirmSet = true;

                    confirmUI.SetActive(false);
                    makeAdjustmentText.SetActive(false);

                    //record reading

                    string[] log = new string[7] {
                        (count).ToString(),
                        experimentPart.ToString(),
                        experimentPhase.ToString(),
                        varHeight.ToString(),
                        varDanger.ToString(),
                        (-1).ToString(),
                        obstacle.transform.position.y.ToString()
                    };
                    logScript.AppendToReport(log);
                    nextTrial();
                }
                else
                {
                    updateStopper = false;
                }
                //Debug.Log(updateStopper + "confirmed");
            }
        }

        //manually remove and putback the obstacle when the participant asks
        if(Input.GetKeyDown(KeyCode.M)){
            Debug.Log("remove obstacle");
            obstacle.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.N)){
            Debug.Log("putback obstacle");
            obstacle.SetActive(true);
        }
    }

    //SetCount Text to trial/totalTrials
    void SetCountText()
    {
        if (count > totalCnt)
        {
            pauseMenuUI.SetActive(false);
            makeAdjustmentText.SetActive(false);
            makeJudgementText.SetActive(false);
            endExpText.SetActive(true);
            // StartCoroutine(WaitEnd(2f));
        }
        else
        {
            countText.text = "Trial: " + ((count-1)%4+1).ToString() + "/" + (totalCnt/4) 
            + "\nBlock: " + experimentPart;
        }
    }

    void nextTrial()
    {
        //set all menus to false, change set menus to true after each set
        judgeSet = false;
        confirmSet = false;
        pauseSet = false;
        updateStopper = false;

        //set the position for the trial
        //Debug.Log(varHeight);
        if (count <= totalCnt)
        {
            if(count%4 == 0){
                varHeight = Random.Range(0, 2);
                experimentPhase = 0;
                experimentPart++;
            }
            else if (count%4 == 1){
                varHeight = Mathf.Abs(varHeight - 1);
                experimentPhase = 0;
            }
            else if (count%4 == 2)
            {
                varHeight = Random.Range(0, 2);
                experimentPhase = 1;
            }
            else if (count%4 == 3){
                varHeight = Mathf.Abs(varHeight - 1);
                experimentPhase = 1;
            }
            setPosition(varHeight);
        }

        //transition between trials
        count++;
        //set UI menus
        UIController();
    }

    //controll the UI
    private void UIController()
    {
        SetCountText();
        startActions.SetActive(false);

        //initiate pause menu UI
        if (count == 1)
        {
            pauseSet = true;
            if (experimentPhase == 0){
                makeJudgementText.SetActive(true);
            }
            else{
                makeAdjustmentText.SetActive(true);
            }
        }
        //do not display pause menu after all trials end
        else if (count < totalCnt + 1)
        {
            StartCoroutine(WaitNextTrial(2f, pauseMenuUI));
        }
    }

    //set position of obstacle
    private void setPosition(int varHeight)
    {
        obstacle.SetActive(true);
        //feedback phase
        if (experimentPhase == 0){
            if(experimentPart == 1){
             obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[varHeight], obstacleOriginZ);
             Debug.Log("1" + obstacle.transform.position);
            }
            if(experimentPart == 2){
                obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[varHeight+2], obstacleOriginZ);
                Debug.Log("2" + obstacle.transform.position);
            }
            if(experimentPart == 3){
                obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[varHeight+4], obstacleOriginZ);
                Debug.Log("3" + obstacle.transform.position);
            }
            if(experimentPart == 4){
                obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + experimentHeights[varHeight+6], obstacleOriginZ);
                Debug.Log("4" + obstacle.transform.position);
            }
        }
        else if (experimentPhase == 1){
            obstacle.transform.position = new Vector3(obstacleOriginX, obstacleOriginY + adjustmentHeights[varHeight], obstacleOriginZ);
        }
    }

    //calculate heights
    private float[] experimentHeights = new float[8];
    private float[] adjustmentHeights = new float[2];
    private void calculateHeights()
    {
        float stepOverThreshold = eyeHeight * 0.52f;

        //the lower boundery
        experimentHeights[0] = stepOverThreshold * 0.7f;
        experimentHeights[1] = stepOverThreshold * 1.3f;
        experimentHeights[2] = stepOverThreshold * 0.8f;
        experimentHeights[3] = stepOverThreshold * 1.2f;
        experimentHeights[4] = stepOverThreshold * 0.9f;
        experimentHeights[5] = stepOverThreshold * 1.1f;
        experimentHeights[6] = stepOverThreshold * 0.95f;
        experimentHeights[7] = stepOverThreshold * 1.05f;

        adjustmentHeights[0] = stepOverThreshold * 0.6f;
        adjustmentHeights[1] = stepOverThreshold * 1.4f;
    }


    //pause for 2 seconds before starting the first trial
    public IEnumerator WaitStartTrial(float t)
    {
        yield return new WaitForSeconds(t);
        startMenuUI.SetActive(false);
        nextTrial();
    }

    // //pause for 2 seconds before confirming in confirm window
    // public IEnumerator WaitEnd(float t)
    // {
    //     yield return new WaitForSeconds(t);
    //     RotateWithUser.initialized = false;
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    // }

    //set the pause menu
    public IEnumerator WaitNextTrial(float t, GameObject uiPanel)
    {
        uiPanel.SetActive(true);
        yield return new WaitForSeconds(t);
        uiPanel.SetActive(false);
        if (experimentPhase == 0){
            makeJudgementText.SetActive(true);
        }
        else{
            makeAdjustmentText.SetActive(true);
        }
        pauseSet = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (judgeSet && endPos == "startPos1" && other.tag == "startPos1")
        {
            //Debug.Log("startPos1");
            endPos = "startPos2";
            nextTrial();
        }

        if (judgeSet && endPos == "startPos2" && other.tag == "startPos2")
        {
            //Debug.Log("startPos2");
            endPos = "startPos1";
            nextTrial();
        }
    }
}
