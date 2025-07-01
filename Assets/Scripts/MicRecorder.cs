using System;
using UnityEngine;
using System.Threading.Tasks;

public class MicRecorder : MonoBehaviour
{

    public AudioClip recordedClip;
    public int sampleDurationSeconds = 20;
    public int sampleRate = 16000;

    private string micDevice;
    private AudioClip tempClip;
    private bool isRecording;

    //Used for Async Recording -- will see how using input action system works instead.
    public async Task StartRecordingAsync()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.Log("No microphone devices available");
            return;
        }
        
        micDevice = Microphone.devices[0];
        Debug.Log("Recording from device: " + micDevice);
        
        tempClip = Microphone.Start(micDevice, false, sampleDurationSeconds, sampleRate);
        Debug.Log("Recording started");
        
        await Task.Delay(sampleDurationSeconds * 1000);
        
        Microphone.End(micDevice);
        Debug.Log("Recording finished");

        int sampleCount = Mathf.Min(sampleRate * sampleDurationSeconds, tempClip.samples);
        float[] samples = new float[sampleCount];
        tempClip.GetData(samples, 0);
        
        recordedClip = AudioClip.Create("Recorded", sampleCount, tempClip.channels, sampleRate, false);
        recordedClip.SetData(samples, 0);
        
        Debug.Log("Recording: " + recordedClip);
    }
    
}
