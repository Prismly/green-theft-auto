using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTruck : MonoBehaviour
{
    [Header("Handling")]
    [SerializeField] [Tooltip("The base speed, in units per second, of the player truck")] private float baseSpeed;
    [SerializeField] [Tooltip("Max angle from 0 (straight) at which the truck can be facing")] private float maxFacingAngle;
    [SerializeField] [Tooltip("Acceleration of the truck's turning, in degrees per second^2")] private float turnAccel;
    [SerializeField] [Tooltip("")] private float maxDeltaAngle;
    [SerializeField] [Tooltip("Deceleration of the truck's turning, in degrees per second^2")] private float turnDecel;
    private float facingAngle = 0; // Negative when facing left, positive when facing right. Zero is straight ahead
    private float deltaAngle = 0; // Saves the number of degrees rotated this frame, so the game object itself can be rotated later

    private KeyCode leftKey = KeyCode.LeftArrow;
    private KeyCode rightKey = KeyCode.RightArrow;
    private KeyCode gasKey = KeyCode.UpArrow;
    private KeyCode brakeKey = KeyCode.DownArrow;

    private void Update()
    {
        float timeScaledDeltaAngle = ProcessTurnInput();

        facingAngle += timeScaledDeltaAngle;
        transform.Rotate(new Vector3(0, timeScaledDeltaAngle, 0));
        transform.position += transform.forward * baseSpeed * Time.deltaTime;

        if (Input.GetKey(gasKey) && !Input.GetKey(brakeKey))
        {
            // Player is ACCELERATING

        }
        else if (!Input.GetKey(gasKey) && Input.GetKey(brakeKey))
        {
            // Player is BRAKING (moving backward)

        }

        Debug.Log("facingAngle: " + facingAngle + "   deltaAngle: " + deltaAngle + "   timeScaledDeltaAngle: " + timeScaledDeltaAngle);
    }

    private float ProcessTurnInput()
    {
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
        {
            // Player holding LEFT turn key
            deltaAngle -= turnAccel;
            if (deltaAngle < -maxDeltaAngle)
            {
                deltaAngle = -maxDeltaAngle;
            }
        }
        else if (!Input.GetKey(leftKey) && Input.GetKey(rightKey))
        {
            // Player holding RIGHT turn key
            deltaAngle += turnAccel;
            if (deltaAngle > maxDeltaAngle)
            {
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
        if (facingAngle + timeScaledDeltaAngle > maxFacingAngle)
        {
            deltaAngle = maxFacingAngle - facingAngle;
            timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        }
        else if (facingAngle + timeScaledDeltaAngle < -maxFacingAngle)
        {
            deltaAngle = -maxFacingAngle - facingAngle;
            timeScaledDeltaAngle = deltaAngle * Time.deltaTime;
        }
        
        return timeScaledDeltaAngle;
    }
}
