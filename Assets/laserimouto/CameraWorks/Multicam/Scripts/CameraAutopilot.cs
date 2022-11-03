
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraAutopilot : UdonSharpBehaviour
{
    public UdonSharpBehaviour target;
    
    public bool autopilot = false;
    public bool randomOrder = false;
    public KeyCode enableAutopilotKey = KeyCode.H;
    public float speed = 10f;

    private float timer;

    void Start()
    {
        timer = 0f;       
    }

    private void Update()
    {
        if (Input.GetKeyDown(enableAutopilotKey))
        {
            autopilot = !autopilot;
            if (autopilot)
                timer = speed;
        }

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else if (autopilot)
        {
            if (randomOrder)
                target.SendCustomEvent("OnRandomCamera");
            else
                target.SendCustomEvent("OnNextCamera");

            timer = speed;
        }
    }
}
