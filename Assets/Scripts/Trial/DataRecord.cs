using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataRecord : MonoBehaviour
{
    public string participantName = InfoLog.participantName;
    //public string prepostTrial = "Pre";
    private string reportDirectoryName = "Data_Logs";
    private string reportFileName;
    private string reportSeparator = ",";
    private string[] reportHeaders = new string[7] {
        "Trial_Num",
        "exp_block", //Experiment Block (0 block 0 pretest/1 block 1 30%/ 2 block 2 20%/ 3 block 3 10%/ 4 block 4 5%)
        "exp_phase", //Experiment Phase (0 adjustment phase/ 1 feedback phase)
        "start_pos", //Start Position(0 start low/ 1 start high)
        "obs_type", //obstacle type (0 Non dangerous/ 1 Dangerous)",
        "judge_yn", //affordance judgement (-1 n/a/ 0 no/ 1 yes)
        "aff_height" //affordance height(-1 n/a, in meters)
    };
    private string timeStampHeader = "time stamp";

    #region Interactions

    public void AppendToReport(string[] strings)
    {
        participantName = InfoLog.participantName;
        reportFileName = participantName +".csv";
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += strings[i];
            }
            finalString += reportSeparator + GetTimeStamp();
            sw.WriteLine(finalString);
        }
    }

    public void CreateReport()
    {
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            finalString += reportSeparator + timeStampHeader;
            sw.WriteLine(finalString);
        }
    }

    #endregion


    #region operations

    void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateReport();
        }
    }
    #endregion


    #region Queries
    string GetDirectoryPath()
    {
        return Application.streamingAssetsPath + "/" + reportDirectoryName;
    }

    string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    string GetTimeStamp()
    {
        return System.DateTime.Now.ToString();
    }
    #endregion

}
