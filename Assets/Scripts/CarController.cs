using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum Axel { Front, Rear }
    public enum DriveType { Front, Rear, All }
    public enum ControlScheme { WASD, ArrowKeys }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
        public bool isDrive;
        public bool isSteer;
    }

    [Header("Drive Settings")]
    public DriveType driveType = DriveType.Rear;
    public ControlScheme controlScheme = ControlScheme.WASD;

    [Header("Car Physics")]
    public float maxAcceleration = 100f;   // motor torque multiplier
    public float brakeTorque = 600f;   // max brake torque (per wheel)
    public float maxVelocity = 25f;    // m/s

    [Header("Steering")]
    public float turnSensitivity = 1f;
    public float maxSteerAngle = 30f;

    [Header("Wheels & Physics")]
    public Vector3 _centerOfMass = new Vector3(0, -0.5f, 0);
    public List<Wheel> wheels = new List<Wheel>();

    [Header("Respawn Settings")]
    public float respawnRadius = 3f;
    public float respawnHeight = 2f;

    [Header("Race Progress")]
    public int currentCheckpointIndex = 0;
    public int currentLap = 0;
    [HideInInspector]
    public float raceProgress = 0f;

    private float moveInput;
    private float steerInput;
    private Rigidbody carRb;
    private Vector3 lastCheckPointPos;
    private Quaternion lastCheckPointRot;

    void Awake()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        currentCheckpointIndex = RaceManager.Instance.startCheckpointId;
    }

    void Update()
    {
        ReadInput();
        AnimateWheels();
    }

    void LateUpdate()
    {
        HandleMotorAndBrakes();
        HandleSteering();
    }

    void ReadInput()
    {
        moveInput = 0f;
        steerInput = 0f;

        switch (controlScheme)
        {
            case ControlScheme.WASD:
                if (Input.GetKey(KeyCode.W)) moveInput = 1f;
                if (Input.GetKey(KeyCode.S)) moveInput = -1f;
                if (Input.GetKey(KeyCode.A)) steerInput = -1f;
                if (Input.GetKey(KeyCode.D)) steerInput = 1f;
                break;

            case ControlScheme.ArrowKeys:
                if (Input.GetKey(KeyCode.UpArrow)) moveInput = 1f;
                if (Input.GetKey(KeyCode.DownArrow)) moveInput = -1f;
                if (Input.GetKey(KeyCode.LeftArrow)) steerInput = -1f;
                if (Input.GetKey(KeyCode.RightArrow)) steerInput = 1f;
                break;
        }

        bool respawn = false;
        if (controlScheme == ControlScheme.WASD && Input.GetKeyDown(KeyCode.R))
            respawn = true;
        if (controlScheme == ControlScheme.ArrowKeys && Input.GetKeyDown(KeyCode.P))
            respawn = true;

        if (respawn)
            RespawnAtLastCheckpoint();
    }

    // Motor and brakes
    void HandleMotorAndBrakes()
    {
        // Forward speed (+) = moving forward, (-) = moving backward
        float forwardSpeed = Vector3.Dot(carRb.linearVelocity, transform.forward);

        bool isMovingForward = forwardSpeed > 0.1f;
        bool isMovingBackward = forwardSpeed < -0.1f;

        bool braking =
              (isMovingForward && moveInput < 0f)   // S or Down while moving forward
           || (isMovingBackward && moveInput > 0f);   // W or Up   while moving backward

        foreach (Wheel wheel in wheels)
        {
            // Apply brake torque
            if (wheel.wheelCollider == null) continue;

            if (braking)
            {
                // Cut engine torque and apply brake torque
                wheel.wheelCollider.motorTorque = 0f;
                if (!wheel.isSteer)
                    wheel.wheelCollider.brakeTorque = brakeTorque;
                else
                    wheel.wheelCollider.brakeTorque = 0f; // No brakes on steering wheels
            }
            else
            {
                // Remove braking
                wheel.wheelCollider.brakeTorque = 0f;

                // Apply engine torque
                if (wheel.isDrive && Mathf.Abs(carRb.linearVelocity.magnitude) < maxVelocity)
                    wheel.wheelCollider.motorTorque = moveInput * maxAcceleration * 10f;
                else
                    wheel.wheelCollider.motorTorque = 0f;
            }
        }
    }

    void HandleSteering()
    {
        float speedFactor = carRb.linearVelocity.magnitude / maxVelocity;

        foreach (Wheel wheel in wheels)
        {
            if (wheel.isSteer && wheel.wheelCollider != null)
            {
                float targetSteer =
                    steerInput * maxSteerAngle * (speedFactor < 0.3f ? 2f : 1.5f) * turnSensitivity;

                wheel.wheelCollider.steerAngle = Mathf.Lerp(
                    wheel.wheelCollider.steerAngle,
                    targetSteer,
                    0.6f);
            }
        }
    }

    void AnimateWheels()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.wheelCollider == null || wheel.wheelModel == null) continue;

            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelModel.transform.SetPositionAndRotation(pos, rot);
        }
    }

    public void SetLastCheckPoint(Transform cp)
    {
        int newIndex = cp.GetComponent<CheckPoint>().checkPointId;
        // Debug.Log($"new index: {newIndex}, RaceManager.Instance.startCheckpointId: {RaceManager.Instance.startCheckpointId}, currentCheckpointIndex: {currentCheckpointIndex}");
        if (newIndex == RaceManager.Instance.startCheckpointId
            && currentCheckpointIndex != newIndex)
        {
            currentLap+=1;
        }

        currentCheckpointIndex = newIndex;
        lastCheckPointPos = cp.position;
        lastCheckPointRot = cp.rotation;
    }

    public void RespawnAtLastCheckpoint()
    {
        // Pick a random horizontal offset within the radius
        Vector2 offset2D = UnityEngine.Random.insideUnitCircle * respawnRadius;
        Vector3 spawnPos = lastCheckPointPos + new Vector3(offset2D.x, respawnHeight, offset2D.y);

        // Raycast downward to find the ground
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, respawnHeight + 1f))
        {
            spawnPos.y = hit.point.y + 0.2f; // raise a bit to avoid clipping into ground
        }
        else
        {
            spawnPos.y = lastCheckPointPos.y + 0.5f; // fallback height
        }

        // Set rotation to match checkpoint’s forward direction
        Quaternion spawnRot = Quaternion.LookRotation(lastCheckPointRot * Vector3.left, Vector3.up);

        // Teleport and reset velocity
        transform.SetPositionAndRotation(spawnPos, spawnRot);
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;
    }
    
    public void Stop()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.isDrive)
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = brakeTorque;
            }
            if (wheel.isSteer)
            {
                wheel.wheelCollider.steerAngle = 0f;
            }
        }
    }

}
 