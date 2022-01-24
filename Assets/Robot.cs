using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robot : MonoBehaviour
{
    [SerializeField] private Rigidbody robotRigidbody;
    [SerializeField] private GameObject leftWheels;
    [SerializeField] private GameObject rightWheels;

    [SerializeField] private Material red;
    [SerializeField] private Material green;
    [SerializeField] private Material gray;

    [SerializeField] private MeshRenderer leftThrottleRenderer;
    [SerializeField] private MeshRenderer rightThrottleRenderer;

    [SerializeField] private float throttleForce;

    [SerializeField] private float performanceRandomizer;

    [SerializeField] private Text motorPerformanceText;
    [SerializeField] private Text motorSpeedText;
    [SerializeField] private Text encoderClicksLeftText;
    [SerializeField] private Text encoderClicksRightText;

    public double leftClicks;
    public double rightClicks;

    public float leftMotor;
    public float rightMotor;

    private Vector3 lastLeftPos = Vector3.zero;
    private Vector3 lastRightPos = Vector3.zero;

    private void Start()
    {
        RandomizeMotorPerformance();
    }

    private void FixedUpdate()
    {
        Encoders();
        Motors();
    }

    private void Encoders()
    {
        float robotHeadingRad = gameObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 robotVector = new Vector3(Mathf.Sin(robotHeadingRad), 0, Mathf.Cos(robotHeadingRad));
        Vector3 leftTravelVector = leftWheels.transform.position - lastLeftPos;
        leftTravelVector.y = 0;
        leftClicks += leftTravelVector.magnitude * Mathf.Sign(Vector3.Dot(robotVector, leftTravelVector));
        Vector3 rightTravelVector = rightWheels.transform.position - lastRightPos;
        rightTravelVector.y = 0;
        rightClicks += rightTravelVector.magnitude * Mathf.Sign(Vector3.Dot(robotVector, rightTravelVector));
        encoderClicksLeftText.text = $"Encoder Clicks Left: {leftClicks}";
        encoderClicksRightText.text = $"Encoder Clicks Right: {rightClicks}";
        lastLeftPos = leftWheels.transform.position;
        lastRightPos = rightWheels.transform.position;
    }

    private void Motors()
    {
        if (leftMotor > 0.01f)
            leftThrottleRenderer.material = green;
        else if (leftMotor < -0.01f)
            leftThrottleRenderer.material = red;
        else
            leftThrottleRenderer.material = gray;
        if (rightMotor > 0.01f)
            rightThrottleRenderer.material = green;
        else if (leftMotor < -0.01f)
            rightThrottleRenderer.material = red;
        else
            rightThrottleRenderer.material = gray;

        motorSpeedText.text = $"Motor Speed: ({((int)(leftMotor * 1000)) / 10f}, {((int)(rightMotor * 1000)) / 10f})";

        float leftLerp = (leftMotor + 1f) / 2;
        float rightLerp = (rightMotor + 1f) / 2;

        leftThrottleRenderer.gameObject.transform.localPosition = new Vector3(-0.35f, 0.5f, Mathf.Lerp(-0.4f, 0.4f, leftLerp));
        rightThrottleRenderer.gameObject.transform.localPosition = new Vector3(0.35f, 0.5f, Mathf.Lerp(-0.4f, 0.4f, rightLerp));

        robotRigidbody.AddForceAtPosition(robotRigidbody.rotation * Vector3.forward * leftMotor * throttleForce * performanceRandomizer, leftWheels.transform.position, ForceMode.Force);
        robotRigidbody.AddForceAtPosition(robotRigidbody.rotation * Vector3.forward * rightMotor * throttleForce * performanceRandomizer, rightWheels.transform.position, ForceMode.Force);
    }

    public void RandomizeMotorPerformance()
    {
        performanceRandomizer = Random.Range(0.8f, 1.2f);
        motorPerformanceText.text = "Motor Performance: " + (int)(performanceRandomizer * 100) + "%";
    }

    public void SetMotors(float left, float right)
    {
        if (Mathf.Abs(left) < 0.01f) left = 0;
        if (Mathf.Abs(right) < 0.01f) right = 0;
        leftMotor = Mathf.Clamp(left, -1f, 1f);
        rightMotor = Mathf.Clamp(right, -1f, 1f);
    }

    public void ResetEncoders()
    {
        leftClicks = 0;
        rightClicks = 0;
    }
}
