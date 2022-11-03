
using Cinemachine;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CinemachineMulticamController : UdonSharpBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera[] cameras;

    [Header("Controls")]
    public KeyCode enableOutputKey = KeyCode.Alpha0;
    public KeyCode nextCameraKey = KeyCode.N;
    public KeyCode prevCameraKey = KeyCode.B;

    [Header("Advanced")]
    public Camera outputCamera;

    [Space(10)]
    public GameObject[] disableAfterTransition;

    private int _active;

    public int Active
    {
        get => _active;
        set
        {
            _active = value;
            if (_active >= cameras.Length)
                _active = 0;
            else if (_active < 0)
                _active = cameras.Length - 1;
        }
    }

    void Start()
    {
        outputCamera.enabled = false;

        SetActiveCamera(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(enableOutputKey))
            outputCamera.enabled = !outputCamera.enabled;

        if (Input.GetKeyDown(nextCameraKey))
            OnNextCamera();
        else if (Input.GetKeyDown(prevCameraKey))
            SetActiveCamera(Active - 1);
    }

    private void SetActiveCamera(int target)
    {
        foreach (GameObject gameObject in disableAfterTransition)
            gameObject.SetActive(false);

        foreach (CinemachineVirtualCamera camera in cameras)
            camera.enabled = false;

        Active = target;

        // Explicitly re-enable to trigger Cinemachine OnCameraLive event
        cameras[Active].enabled = true;
    }

    public void OnNextCamera()
    {
        SetActiveCamera(Active + 1);
    }

    public void OnRandomCamera()
    {
        if (cameras.Length < 2)
            return;

        int nextCamera;
        do
        {
            nextCamera = Random.Range(0, cameras.Length);
        }
        while (nextCamera == Active);

        SetActiveCamera(nextCamera);
    }
}
