using UnityEngine;
using System.Diagnostics;
using System.IO;

public class DDAScript : MonoBehaviour
{
    Process pythonProcess;
    double positiveMult = 1;
    double negativeMult = 1;
    int heartRate = -1;
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

    void HeartRate_OutputDataReceived(object sender, DataReceivedEventArgs e){
         if (!string.IsNullOrEmpty(e.Data))
        {
            heartRate = int.Parse(e.Data);
        }
        else {
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
            if (emotions.Equals("0"))
            {
                positiveMult = 1;
                negativeMult = 1;
            }
            else if (emotions.Equals("-1"))
            {
                positiveMult -= 0.1;
                negativeMult += 0.1;
            }
            else if (emotions.Equals("1") )
            {
                positiveMult += 0.3;
                negativeMult -= 0.1;
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
            Enemy.maxHealth = 100 * negativeMult;

            //Code for values that the bigger the easier the game.
            PlayerController.playerMaxHealth = 100 * positiveMult;
            PlayerController.attackDamage1 = 25 * positiveMult;
            PlayerController.attackDamage2 = 50 * positiveMult;
            PotionSpawner.spawnTime = 45 * (float)negativeMult;

            UnityEngine.Debug.Log("Emotion: " + e.Data);

            UnityEngine.Debug.Log("maxHealth(" + PlayerController.playerMaxHealth + ", " + Enemy.maxHealth+")");
            UnityEngine.Debug.Log("dmg(" + PlayerController.attackDamage1 + ", " + PlayerController.attackDamage2 + ")");
            UnityEngine.Debug.Log("PSpawnTime(" + PotionSpawner.spawnTime + ")");

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
