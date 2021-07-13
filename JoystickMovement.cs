/*
This script allows the user to move along the head forward vector even after the player prefab has done a snap turn or rotated another direction. Solves the tank movement issue of character control in VR. Now you will always move along the camera forward vector even if you have rotated multiple times.

Basically you have to get the player rotation before applying the movement vector along the camera forward
then you have to rotate that movement vector along the player prefab eulerAngle.y AGAIN to get the correct forward vector to move along, since you are now taking into consideration the rotation of the player and then rotating your direction vector around that.
But you have to use a rotation matrix to make sure that your direction vector is correct which is in the RotateVector() function.

Credit to https://leetdev.io/unity-vr-development/day-63-of-100-days-of-vr-rotating-a-vector-and-moving-in-the-direction-the-player-is-facing-in-unity for inspiration.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// Class to handle movement of the player with the touchpad
// Can also be re-written for Joystick movement, just need to set bindings in SteamVR  
public class JoystickMovement : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float maxSpeed = 1.0f;

    // Exposed for Debugging
    [ReadOnly] [SerializeField] private float currentSpeed = 0.0f;

    public SteamVR_Action_Vector2 touchpadValue = null;
    public SteamVR_Action_Boolean touchpadPress = null;

    public Transform head = null;

    public Player player = null;

    public CharacterController controller = null;


    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = Valve.VR.InteractionSystem.Player.instance;    
        }

        if (player != null)
        {
            HeadRotation();
            StickMovement();
        }
    }

    void Awake() 
    {
        controller = GetComponent<CharacterController>();
    }

    void HeadRotation()
    {
        // Store pos / rot before rotate
        Vector3 previousPos = player.transform.position;
        Quaternion previousRot = player.transform.rotation;

        // Rotate
        transform.eulerAngles = new Vector3(0.0f, player.transform.rotation.eulerAngles.y, 0.0f);

        // Back to original
        player.transform.position = previousPos;
        player.transform.rotation = previousRot;
    }

    // Given a direction and a degree, we'll rotate the direction vector by the given degree amount
    private Vector3 RotateVector(Vector3 direction, float degree)
    {
        float radian = Mathf.Deg2Rad * degree; // convert our degree to be in radians
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        float newX = direction.x * cos - direction.z * sin;
        float newZ = direction.x * sin + direction.z * cos;
        return new Vector3(newX, 0, newZ);
    }


    void StickMovement()
    {
        // Orientation
        //Vector3 orientationAngles = new Vector3(0, transform.eulerAngles.y, 0.0f);
        //Quaternion rotation = Quaternion.Euler(orientationAngles);
        Quaternion rotation = player.transform.rotation;
        Vector3 movement = Vector3.zero;

        // If not moving anymore
        if (touchpadValue.axis == Vector2.zero)
        {
            currentSpeed = 0;
        }

        // Check Input
        if (touchpadValue.axis != Vector2.zero)
        {
            if (touchpadValue.axis.y != 0f)
            {
                currentSpeed += touchpadValue.axis.y * sensitivity;

                // Clamp accleration and slow down backwards acceleration
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.75f, maxSpeed);

                movement = rotation * (currentSpeed * head.forward) * Time.deltaTime;
                movement = RotateVector(movement, player.transform.eulerAngles.y);

                Debug.Log("Moving Forward = " + touchpadValue.axis.y);
                Debug.Log("Movement Vector = " + movement);
            }

            // // Moving left
            // if (touchpadValue.axis.x < 0f)
            // {
            //     currentSpeed += touchpadValue.axis.x * sensitivity;

            //     // Clamp accleration and slow down backwards acceleration
            //     currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.75f, maxSpeed);

            //     movement = movement + (currentSpeed * -head.transform.right) * Time.deltaTime;

            //     Debug.Log("Moving Left = " + touchpadValue.axis.x);
            // }

            // // Moving right
            // if (touchpadValue.axis.x > 0f)
            // {
            //     currentSpeed += touchpadValue.axis.x * sensitivity;

            //     // Clamp accleration and slow down backwards acceleration
            //     currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.75f, maxSpeed);

            //     movement = movement + (currentSpeed * head.transform.right) * Time.deltaTime;
                
            //     Debug.Log("Moving Right = " + touchpadValue.axis.x);
            // }


        }

        controller.Move(movement);

        // Reset Camera Front when player pressed button
        if (touchpadPress.GetStateDown(SteamVR_Input_Sources.Any))
        {
            HeadRotation();
        }
    }
}
