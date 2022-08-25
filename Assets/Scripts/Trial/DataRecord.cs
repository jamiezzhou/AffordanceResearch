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
    private string[] reportHeaders = new string[6] {
        "Trial Number",
        "Experiment Part (0 Pretest/1 Feeback/ 2 Posttest)",
        "Avatar (0 Avatar/ 1 No avatar)",
        "Obstacle Type (0 Non dangerous/ 1 Dangerous)",
        "Obstacle Height (Odd Min Height/ Even Max Height)",
        "Affordance Height (Meters)"
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
