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

    public GameObject dangerousObs;
    public GameObject nondangerousObs;

    public TextMeshProUGUI countText;
    public GameObject endExpText;
    private int count;

    //input eyeHeight of participant before experimentation
    public float eyeHeight = 1.5f;
    public float offset = 0.15f;
    private string endPos;
    public float[] heights = new float[7];

    private int dangerousCnt = 21;
    private int nondangerousCnt = 21;
    private int totalCnt = 3;

    // Start is called before the first frame update
    void Start()
    {
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
        //nextTrialText.SetActive(startNextTrial);

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

        if (count > totalCnt)
        {
            endExpText.SetActive(true);
            //end game and terminate
        }
        else {
            countText.text = "Trial: " + count.ToString() + "/" + totalCnt;
        }
    }

    void nextTrial() {
        //generate random position of obstacle
        int varDanger = Random.Range(0, 2);
        int varHeight = Random.Range(1, 7);

        if (dangerousCnt == 0)
        {
            varDanger = 0;
        }
        if (nondangerousCnt == 0)
        {
            varDanger = 1;
        }

        if (varDanger == 1)
        {
            dangerousCnt--;
            dangerousObs.SetActive(true);
            nondangerousObs.SetActive(false);
            dangerousObs.transform.position = new Vector3(0, heights[varHeight], 0);
        }
        else if (varDanger == 0)
        {
            nondangerousCnt--;
            nondangerousObs.SetActive(true);
            dangerousObs.SetActive(false);
            nondangerousObs.transform.position = new Vector3(0, heights[varHeight], 0);
        }

        //update trial number on UI
        count++;
        SetCountText();
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
