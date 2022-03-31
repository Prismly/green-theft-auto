using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTruck : MonoBehaviour
{
    [Header("Driving")]
    [SerializeField] [Tooltip("The current speed, in units per second, of the player truck")] private float speed;
    [SerializeField] [Tooltip("The maximum speed, in units per second, of the player truck")] private float maxSpeed;
    [SerializeField] [Tooltip("The minimum speed, in units per second, of the player truck")] private float minSpeed;
    [SerializeField] [Tooltip("Acceleration/deceleration of the truck's speed, in units per second^2")] private float driveAccel;
    
    [SerializeField] private float facingAngle = 0; // The angle, from 0 (facing z+ axis) at which the truck is currently facing. Bounds are defined by angleHighBound and angleLowBound
    private Quaternion homeDirection; // The "straight ahead" direction, on which the player's maximum and minimum angles will be based
    [Header("Turning")]
    [SerializeField] [Tooltip("The maximum value currently allowed for facingAngle")] private float angleHighBound; 
    [SerializeField] [Tooltip("The minimum value currently allowed for facingAngle")] private float angleLowBound;
    [SerializeField] [Tooltip("Acceleration of the truck's turning, in degrees per second^2")] private float turnAccel;
    [SerializeField] [Tooltip("")] private float maxDeltaAngle;
    [SerializeField] [Tooltip("Deceleration of the truck's turning, in degrees per second^2")] private float turnDecel;
    private float deltaAngle = 0; // Saves the number of degrees rotated during the most recent frame, so accel/deceleration can be applied
    
    [Header("Keybinds")]
    [SerializeField] [Tooltip("Turns the truck left when held")] private KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] [Tooltip("Turns the truck right when held")] private KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] [Tooltip("Increases the truck's forward speed when held")] private KeyCode gasKey = KeyCode.UpArrow;
    [SerializeField] [Tooltip("Decreases the truck's forward speed when held")] private KeyCode brakeKey = KeyCode.DownArrow;

    private void Start()
    {
        // Initialize the angle the truck is facing as being between its two bounds.
        facingAngle = (angleHighBound + angleLowBound) / 2;
    }

    private void Update()
    {
        // Determine, from the player's left/right inputs and previous rotation speed, how much the truck should rotate this frame.
        float timeScaledDeltaAngle = ProcessTurnInput();
        // Rotate the truck according to the value determined by ProcessTurnInput().
        transform.Rotate(new Vector3(0, timeScaledDeltaAngle, 0));

        // Determine, from the player's gas/brake inputs and previous forward speed, how much the truck should move forward this frame.
        float timeScaledSpeed = ProcessForwardInput();
        // Move the truck according to the value determined by ProcessForwardInput().
        transform.position += transform.forward * timeScaledSpeed;
    }

    /*
     * Given the left/right input the player is holding, determines by how much the truck should be rotating this frame and returns that value as a float.
     */
    private float ProcessTurnInput()
    {
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
        {
            // Player holding LEFT turn key
            deltaAngle -= turnAccel;
            if (deltaAngle > 0)
            {
                // Player is trying to turn in the direction opposite to how the truck is rotating; accelerate more than usual ~~REWRITE~~
                deltaAngle = 0;
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
                // Player is trying to turn in the direction opposite to how the truck is rotating; accelerate more than usual ~~REWRITE~~
                deltaAngle = 0;
            }
            if (deltaAngle > maxDeltaAngle)
            {
                // Rotation speed has accelerated past what is allowed; cap it
                deltaAngle = maxDeltaAngle;
            }
        }
        else
        {
            // Player IS NOT holding a turning key (or holding both, which has the same result)
            if (deltaAngle != 0)
            {
                // Some amount of rotation is happening on this frame, so we should decelerate the rotation.
                if (deltaAngle > 0)
                {
                    deltaAngle -= turnDecel;
                    if (deltaAngle < 0)
                    {
                        // We've decelerated so much, we've begun turning in the other direction! Stop rotation entirely.
                        deltaAngle = 0;
                    }
                }
                else
                {
                    deltaAngle += turnDecel;
                    if (deltaAngle > 0)
                    {
                        // We've decelerated so much, we've begun turning in the other direction! Stop rotation entirely.
                        deltaAngle = 0;
                    }
                }
            }
        }

        // Clamp deltaAngle if its value is high/low enough to turn the truck past the set max/min turn angle.
        float timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        if (facingAngle + timeScaledDeltaAngle > angleHighBound)
        {
            if (facingAngle < angleHighBound)
            {
                deltaAngle = angleHighBound - facingAngle;
                timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
            }
            else if (timeScaledDeltaAngle >= 0)
            {
                deltaAngle = 0;
                timeScaledDeltaAngle = 0;
            }
        }
        else if (facingAngle + timeScaledDeltaAngle < angleLowBound)
        {
            if (facingAngle > angleLowBound)
            {
                deltaAngle = angleLowBound - facingAngle;
                timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
            }
            else if (timeScaledDeltaAngle <= 0)
            {
                deltaAngle = 0;
                timeScaledDeltaAngle = 0;
            }
        }

        // Update the variable tracking the truck's current angle, and return by how much it should rotate this frame.
        facingAngle += timeScaledDeltaAngle;
        return timeScaledDeltaAngle;
    }

    private float ProcessForwardInput()
    {
        if (Input.GetKey(gasKey) && !Input.GetKey(brakeKey))
        {
            // Player is SPEEDING UP
            speed += driveAccel * Time.deltaTime;
            if (speed > maxSpeed)
            {
                // Speed has accelerated past what is allowed; cap it
                speed = maxSpeed;
            }
        }
        else if (!Input.GetKey(gasKey) && Input.GetKey(brakeKey))
        {
            // Player is SLOWING DOWN
            speed -= driveAccel * Time.deltaTime;
            if (speed < minSpeed)
            {
                // Speed has decelerated past what is allowed; cap it
                speed = minSpeed;
            }
        }

        return speed * Time.deltaTime;
    }

    /*
     * Rather than maintain a homeAngle variable, which won't be useful later, immediately update
     * the facingAngle's high and low bounds to reflect the new home angle, which defines where is "straight ahead".
     */
    public void SetHomeAngle(float newHomeAngle)
    {
        float halfOfRange = (angleHighBound - angleLowBound) / 2;
        angleHighBound = newHomeAngle + halfOfRange;
        angleLowBound = newHomeAngle - halfOfRange;
    }
}
