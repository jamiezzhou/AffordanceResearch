using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    public int varDanger;

    public GameObject dangerousObs;
    public GameObject nondangerousObs;

    public TextMeshProUGUI countText;
    public GameObject endExpText;
    public GameObject pauseMenuUI;
    private int count;

    //input eyeHeight of participant before experimentation
    public float eyeHeight = 1.5f;
    public float offset = 0.15f;
    private string endPos;
    public float[] heights = new float[7];

    private int totalCnt = 6;

    // Start is called before the first frame update
    void Start()
    {
        varDanger = Random.Range(0,2);
        //initialize height array
        eyeHeight = eyeHeight + offset;
        heights[0] = eyeHeight * 0.15f;
        heights[1] = eyeHeight * 0.35f;
        heights[2] = eyeHeight * 0.55f;
        heights[3] = eyeHeight * 0.75f;
        heights[4] = eyeHeight * 0.95f;
        heights[5] = eyeHeight * 0.115f;
        heights[6] = eyeHeight * 0.135f;

        //begin the first trial walking towards startPos2
        endPos = "startPos2";
        nextTrial();

        count = 0;
        SetCountText();

        endExpText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetCountText()
    {
        if (count == (totalCnt + 1)/2) {
            varDanger = Mathf.Abs(varDanger - 1);
        }

        if (count > totalCnt)
        {
            endExpText.SetActive(true);
            Time.timeScale = 0f;
            //end game and terminate
        }
        else {
            countText.text = "Trial: " + count.ToString() + "/" + totalCnt;
        }
    }

    void nextTrial() {
        //generate random position of obstacle
        int varHeight = Random.Range(1, 7);

        if (varDanger == 1)
        {
            dangerousObs.SetActive(true);
            nondangerousObs.SetActive(false);
            dangerousObs.transform.position = new Vector3(0, heights[varHeight], 0);
        }
        else if (varDanger == 0)
        {
            nondangerousObs.SetActive(true);
            dangerousObs.SetActive(false);
            nondangerousObs.transform.position = new Vector3(0, heights[varHeight], 0);
        }

        //transition between trials
        count++;
        SetCountText();
        StartCoroutine(WaitNextTrial(1f));
        //update trial number on UI
    }

    public IEnumerator WaitNextTrial(float t)
    {
        pauseMenuUI.SetActive(true);
        yield return new WaitForSeconds(t);
        pauseMenuUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (endPos == "startPos1" && other.tag == "startPos1")
        {
            Debug.Log("startPos1");
            endPos = "startPos2";
            nextTrial();
        }

        if (endPos == "startPos2" && other.tag == "startPos2")
        {
            Debug.Log("startPos2");
            endPos = "startPos1";
            nextTrial();
        }
    }
}
