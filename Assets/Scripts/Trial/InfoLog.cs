using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoLog : MonoBehaviour
{
    public static int obstacleType = 1;
    public static int avatar = 1;
    public static float eyeHeight = 1.755f;
    public static string participantName = "Cal";

    public GameObject childAvatar;
    void Start() {
        if (avatar == 0){
            childAvatar.SetActive(false);
        }
        else
        {
            childAvatar.SetActive(true);
        }
    }
}
