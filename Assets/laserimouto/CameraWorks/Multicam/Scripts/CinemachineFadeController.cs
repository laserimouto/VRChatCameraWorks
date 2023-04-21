
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Cinemachine;

public class CinemachineFadeController : UdonSharpBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera[] cameras;
    public float fadeTime = 2f;

    [Header("Controls")]
    public KeyCode enableOutputKey = KeyCode.Alpha0;
    public KeyCode nextCameraKey = KeyCode.N;
    public KeyCode prevCameraKey = KeyCode.B;

    [Header("Advanced")]
    public Camera outputCamera;

    [Space(10)]
    public Camera activeCamera;
    public Camera fadeTargetCamera;
    public Material activeCameraMaterial;
    public Material fadeTargetMaterial;

    [Space(10)]
    public int activeLayer = 25;
    public int fadeTargetLayer = 26;

    [Space(10)]
    public GameObject[] disableAfterTransition;

    private float fadeAmount;
    private int _active;
    private int _fadeTarget;

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

    public int FadeTarget
    {
        get => _fadeTarget;
        set
        {
            _fadeTarget = value;
            if (_fadeTarget >= cameras.Length)
                _fadeTarget = 0;
            else if (_fadeTarget < 0)
                _fadeTarget = cameras.Length - 1;
        }
    }

    void Start()
    {
        // Layer changes persist in Play mode, so we reset to avoid weirdness
        foreach (CinemachineVirtualCamera camera in cameras)
            camera.gameObject.layer = activeLayer;

        // Automatically set up culling masks so we don't need to mess around in inspector
        activeCamera.cullingMask |= (1 << activeLayer);
        fadeTargetCamera.cullingMask |= (1 << fadeTargetLayer);

        outputCamera.enabled = false;

        FadeTarget = 0;
        StopFade();
    }

    private void Update()
    {
        if (Input.GetKeyDown(enableOutputKey))
            outputCamera.enabled = !outputCamera.enabled;

        if (fadeAmount > 0)
        {
            fadeAmount -= Time.deltaTime;
            float alpha = Mathf.Clamp(1f - fadeAmount / fadeTime, 0f, 1f);
            fadeTargetMaterial.color = new Vector4(1f, 1f, 1f, alpha);
            activeCameraMaterial.color = new Vector4(1f, 1f ,1f, 1 - alpha);
        }
        else if (Active != FadeTarget)
        {
            StopFade();
        }
        else if (Input.GetKeyDown(nextCameraKey))
        {
            StartFade(Active + 1);
        }
        else if (Input.GetKeyDown(prevCameraKey))
        {
            StartFade(Active - 1);
        }
    }

    private void StartFade(int target)
    {
        FadeTarget = target;
        fadeAmount = fadeTime;
        fadeTargetCamera.enabled = true;

        cameras[FadeTarget].enabled = true;
        cameras[FadeTarget].gameObject.layer = fadeTargetLayer;
    }

    private void StopFade()
    {
        fadeAmount = 0f;
        fadeTargetCamera.enabled = false;
        fadeTargetMaterial.color = new Vector4(1f, 1f, 1f, 0f);
        activeCameraMaterial.color = new Vector4(1f, 1f, 1f, 1f);

        foreach (GameObject gameObject in disableAfterTransition)
            gameObject.SetActive(false);

        foreach (CinemachineVirtualCamera camera in cameras)
            camera.enabled = false;

        Active = FadeTarget;
        // Explicitly re-enable to trigger Cinemachine OnCameraLive event
        cameras[Active].enabled = true;
        cameras[Active].gameObject.layer = activeLayer;
    }

    public void OnNextCamera()
    {
        StartFade(Active + 1);
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
        while (nextCamera == FadeTarget);

        StartFade(nextCamera);
    }
}
