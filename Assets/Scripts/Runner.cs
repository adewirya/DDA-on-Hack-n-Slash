using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class Runner : MonoBehaviour
{
    Process pythonProcess;
    double positiveMult = 1;
    double negativeMult = 1;
    int heartRate = -1;
    private int oldValue = 0;
    private int newValue = 0;
    private bool increaseInHeartRate = false;
    void Start()
    {
        StartPythonScript();
        StartHeartRateScript();
    }

    void StartHeartRateScript(){
        string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "heartratedetector.py");

        pythonProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = pythonScriptPath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Application.streamingAssetsPath
            },
            EnableRaisingEvents = true
        };

        pythonProcess.OutputDataReceived += HeartRate_OutputDataReceived;

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
    }

    void HeartRate_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            heartRate = int.Parse(e.Data);
            while (true)
            {
                newValue = int.Parse(e.Data);

                if (oldValue != newValue)
                {
                    if (Mathf.Abs(newValue - oldValue) > 5)
                    {
                        increaseInHeartRate = true;
                    }
                    oldValue = newValue;
                }
                else
                {
                    increaseInHeartRate = false;
                }
            }
        }
        else
        {
            heartRate = -1;
        }
    }

    void StartPythonScript()
    {
        string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "facerecog2.py");

        pythonProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = pythonScriptPath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Application.streamingAssetsPath
            },
            EnableRaisingEvents = true
        };

        pythonProcess.OutputDataReceived += PythonProcess_OutputDataReceived;

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
    }

    void PythonProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            string emotions = e.Data;


            // 0 -> user is satisfied
            // 1 -> user feels like the game is too hard
            // -1 -> user feels that the game is too easy / bored
            if (emotions.Equals("-1") )
            {
                if (heartRate == -1 || (increaseInHeartRate == true && heartRate != -1)){
                    positiveMult -= 0.1;
                    negativeMult += 0.1;
                }
            }
            else if (emotions.Equals("1") )
            {
                if (heartRate == -1 || (increaseInHeartRate == true && heartRate != -1)){
                    positiveMult += 0.1;
                    negativeMult -= 0.1;
                }
            }

            if(positiveMult <= 0)
            {
                positiveMult = 0.1;
            }

            if(negativeMult <= 0)
            {
                negativeMult = 0.1;
            }
            //Code for values that the smaller the easier the game.
            UnityEngine.Debug.Log("Emotion: " + e.Data);
            DynamicBalancer.BalanceGame(positiveMult, negativeMult);
        }
    }

    void OnDestroy()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.CancelOutputRead();
            pythonProcess.Kill();
            pythonProcess.Close();
        }
    }
}
