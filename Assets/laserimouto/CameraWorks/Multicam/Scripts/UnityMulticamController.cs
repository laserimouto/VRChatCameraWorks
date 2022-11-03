
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UnityMulticamController : UdonSharpBehaviour
{
    [Header("Cameras")]
    public Camera[] cameras;

    [Header("Controls")]
    public KeyCode enableOutputKey = KeyCode.Alpha0;
    public KeyCode nextCameraKey = KeyCode.N;
    public KeyCode prevCameraKey = KeyCode.B;

    private int _active;
    private bool cameraOutputEnabled;

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

            EnableOutput(_active);
        }
    }

    void Start()
    {
        cameraOutputEnabled = false;
        Active = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(nextCameraKey))
            OnNextCamera();
        else if (Input.GetKeyDown(prevCameraKey))
            Active--;

        if (Input.GetKeyDown(enableOutputKey))
        {
            cameraOutputEnabled = !cameraOutputEnabled;
            EnableOutput(Active);
        }
    }

    private void EnableOutput(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].enabled = cameraOutputEnabled && (i == index);
    }

    public void OnNextCamera()
    {
        Active++;
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

        Active = nextCamera;
    }
}
