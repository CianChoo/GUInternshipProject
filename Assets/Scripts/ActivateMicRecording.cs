using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateMicRecording : MonoBehaviour
{
    public InputActionAsset inputActions;
    public MicRecorder micRecorder;
    public RunWhisper runWhisper;

    private InputAction startRecordingAction;

    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Microphone", true);
        startRecordingAction = actionMap.FindAction("StartRecording", true);
        startRecordingAction.Enable();
        
        startRecordingAction.performed += OnMicButtonPressed;
    }

    private void OnDisable()
    {
        startRecordingAction.performed -= OnMicButtonPressed;
        startRecordingAction.Disable();
    }

    private async void OnMicButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Button Pressed: Mic recording started");
        await micRecorder.StartRecordingAsync();

        if (micRecorder.recordedClip != null)
        {
            runWhisper.SetClipAndBeginTranscription(micRecorder.recordedClip);
        }
    }
}
