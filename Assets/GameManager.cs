using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Robot robot;

    [SerializeField] private Text targetErrorText;
    [SerializeField] private Text errorToleranceText;
    [SerializeField] private Text speedClampText;
    [SerializeField] private Text integrationWindowSizeText;
    [SerializeField] private Text integrationWindupLimitText;

    [SerializeField] private Text pConstantText;
    [SerializeField] private Text iConstantText;
    [SerializeField] private Text dConstantText;
    [SerializeField] private Text pText;
    [SerializeField] private Text iText;
    [SerializeField] private Text dText;

    [SerializeField] private bool enable;
    [SerializeField] private float targetError;
    [SerializeField] private float errorTolerance;
    [SerializeField] private float speedClamp;
    [SerializeField] private int integrationWindowSize;
    [SerializeField] private float integrationWindupLimit;

    [SerializeField] private float p;
    [SerializeField] private float i;
    [SerializeField] private float d;

    private bool running = false;

    private float lastError;
    private Queue<float> integrationWindow = new Queue<float>();

    private void Start()
    {
        Set();
    }

    public void Reset()
    {
        Set();
        enable = false;
        running = false;
        integrationWindow.Clear();
        robot.transform.position = new Vector3(0, 1, 0);
        robot.RandomizeMotorPerformance();
    }

    public void Enable()
    {
        enable = true;
    }

    // later stop I term when motors are working as hard as they can, called anti-windup

    private void FixedUpdate()
    {
        if (!enable)
        {
            robot.ResetEncoders();
            robot.SetMotors(0, 0);
            return;
        }
        if (!running)
        {
            running = true;
            Set();
        }


        float error = targetError - (float)robot.leftClicks;
        targetErrorText.text = "Target Error: " + error + "m";

        if (Mathf.Abs(error) > errorTolerance)
        {
            integrationWindow.Enqueue(error);
            while (integrationWindow.Count > integrationWindowSize) integrationWindow.Dequeue();
            float kP = p * error;
            float kI = i * integrationWindow.Average() * Time.fixedDeltaTime;
            float kD = d * ((error - lastError) / Time.fixedDeltaTime);
            if (Mathf.Abs(kD) > integrationWindupLimit) kI = 0;
            pText.text = $"P: {kP}";
            iText.text = $"I: {kI}";
            dText.text = $"D: {kD}";
            lastError = error;
            float speed = Mathf.Clamp(kP + kI + kD, -speedClamp, speedClamp);
            robot.SetMotors(speed, speed);
        }
        else
        {
            robot.SetMotors(0, 0);
        }
    }

    private void Set()
    {
        lastError = targetError;
        pConstantText.text = "P Gain: " + p;
        iConstantText.text = "I Gain: " + i;
        dConstantText.text = "D Gain: " + d;
        targetErrorText.text = "Target Error: " + targetError + "m";
        errorToleranceText.text = "Error Tolerance: ±" + errorTolerance + "m";
        speedClampText.text = "Speed Clamp: " + ((int)(speedClamp * 1000)) / 10f;
        integrationWindowSizeText.text = "Integration Window Size: " + integrationWindowSize;
        integrationWindupLimitText.text = "Integration Windup Limit: " + integrationWindupLimit;
    }
}
