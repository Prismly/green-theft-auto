using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTruck : MonoBehaviour
{
    [Header("Driving")]
    [SerializeField] [Tooltip("The starting speed and current speed, in units per second, of the player truck")] private float speed;
    [SerializeField] [Tooltip("The maximum speed, in units per second, of the player truck")] private float maxSpeed;
    [SerializeField] [Tooltip("The minimum speed, in units per second, of the player truck")] private float minSpeed;
    [SerializeField] [Tooltip("Acceleration/deceleration of the truck's speed, in units per second^2")] private float driveAccel;
    [Header("Turning")]
    [SerializeField] [Tooltip("Max angle from 0 (straight) at which the truck can be facing")] private float maxFacingAngle;
    private float angleHighBound;
    private float angleLowBound;
    [SerializeField] [Tooltip("Acceleration of the truck's turning, in degrees per second^2")] private float turnAccel;
    [SerializeField] [Tooltip("")] private float maxDeltaAngle;
    [SerializeField] [Tooltip("Deceleration of the truck's turning, in degrees per second^2")] private float turnDecel;
    private float facingAngle = 0;
    [Header("Path")]
    [SerializeField] [Tooltip("")] private Transform pathTransform;
    private int pathIndex = 0;

    private Quaternion homeDirection; // The "straight ahead" direction, on which the player's maximum and minimum angles will be based
    private float deltaAngle = 0; // Saves the number of degrees rotated this frame, so the game object itself can be rotated later

    private KeyCode leftKey = KeyCode.LeftArrow;
    private KeyCode rightKey = KeyCode.RightArrow;
    private KeyCode gasKey = KeyCode.UpArrow;
    private KeyCode brakeKey = KeyCode.DownArrow;

    private void Start()
    {
        angleHighBound = maxFacingAngle;
        angleLowBound = -maxFacingAngle;
    }

    private void Update()
    {
        float timeScaledDeltaAngle = ProcessTurnInput();

        // Rotate the truck according to the value determined by ProcessTurnInput().
        facingAngle += timeScaledDeltaAngle;
        transform.Rotate(new Vector3(0, timeScaledDeltaAngle, 0));

        if (Input.GetKey(gasKey) && !Input.GetKey(brakeKey))
        {
            // Player is ACCELERATING
            speed += driveAccel;
            if (speed > maxSpeed)
            {
                // Speed has accelerated past what is allowed; cap it
                speed = maxSpeed;
            }
        }
        else if (!Input.GetKey(gasKey) && Input.GetKey(brakeKey))
        {
            // Player is BRAKING (moving backward)
            speed -= driveAccel;
            if (speed < minSpeed)
            {
                // Speed has decelerated past what is allowed; cap it
                speed = minSpeed;
            }
        }

        // Move the truck according to the current value of baseSpeed.
        transform.position += transform.forward * speed * Time.deltaTime;

        //Debug.Log("speed: " + speed);
        //Debug.Log("facingAngle: " + facingAngle + "   deltaAngle: " + deltaAngle + "   timeScaledDeltaAngle: " + timeScaledDeltaAngle);
    }

    /**
     * Given the left/right input the player is holding, determines by how much the truck should be rotating this frame.
     * @return the degrees by which the truck should rotate per second, multiplied by Time.deltaTime.
     */
    private float ProcessTurnInput()
    {
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
        {
            // Player holding LEFT turn key
            deltaAngle -= turnAccel;
            if (deltaAngle > 0)
            {
                // Player is trying to turn in the direction opposite to how the truck is rotating; accelerate more than usual
                deltaAngle -= turnDecel;
            }
            if (deltaAngle < -maxDeltaAngle)
            {
                // Rotation speed has accelerated past what is allowed; cap it
                deltaAngle = -maxDeltaAngle;
            }
        }
        else if (!Input.GetKey(leftKey) && Input.GetKey(rightKey))
        {
            // Player holding RIGHT turn key
            deltaAngle += turnAccel;
            if (deltaAngle < 0)
            {
                // Player is trying to turn in the direction opposite to how the truck is rotating; accelerate more than usual
                deltaAngle += turnDecel;
            }
            if (deltaAngle > maxDeltaAngle)
            {
                // Rotation speed has accelerated past what is allowed; cap it
                deltaAngle = maxDeltaAngle;
            }
        }
        else
        {
            // Player IS NOT holding a turning key
            if (deltaAngle != 0)
            {
                // Some amount of rotation is happening on this frame, so we should decelerate the rotation
                if (deltaAngle > 0)
                {
                    deltaAngle -= turnDecel;
                    if (deltaAngle < 0)
                    {
                        deltaAngle = 0;
                    }
                }
                else
                {
                    deltaAngle += turnDecel;
                    if (deltaAngle > 0)
                    {
                        deltaAngle = 0;
                    }
                }
            }
        }

        // Clamp deltaAngle if its value is high enough to turn the truck past the set max turn angle
        float timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        if (facingAngle + timeScaledDeltaAngle > angleHighBound)
        {
            deltaAngle = angleHighBound - facingAngle;
            timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        }
        else if (facingAngle + timeScaledDeltaAngle < angleLowBound)
        {
            deltaAngle = angleLowBound - facingAngle;
            timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        }

        return timeScaledDeltaAngle;
    }

    public void SetHomeAngle(float newHomeAngle)
    {
        angleHighBound = newHomeAngle + maxFacingAngle;
        angleLowBound = newHomeAngle - maxFacingAngle;
        Debug.Log("New angle bounds: " + angleLowBound + " to " + angleHighBound);
    }
}
