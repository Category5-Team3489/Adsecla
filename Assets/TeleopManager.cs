using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleopManager : MonoBehaviour
{
    [SerializeField] private Robot robot;

    [SerializeField] private float rtzLerp = 0.15f;

    private float leftThrottle = 0;
    private float rightThrottle = 0;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        ManualInput();
        robot.SetMotors(leftThrottle, rightThrottle);
    }

    private void ManualInput()
    {
        if (Input.GetKey(KeyCode.Q))
            leftThrottle += 0.0175f;
        else if (Input.GetKey(KeyCode.A))
            leftThrottle -= 0.0175f;
        else
            leftThrottle = Mathf.Lerp(leftThrottle, 0, rtzLerp);
        leftThrottle = Mathf.Clamp(leftThrottle, -1, 1);

        if (Input.GetKey(KeyCode.E))
            rightThrottle += 0.0175f;
        else if (Input.GetKey(KeyCode.D))
            rightThrottle -= 0.0175f;
        else
            rightThrottle = Mathf.Lerp(rightThrottle, 0, rtzLerp);
        rightThrottle = Mathf.Clamp(rightThrottle, -1, 1);
    }
}
