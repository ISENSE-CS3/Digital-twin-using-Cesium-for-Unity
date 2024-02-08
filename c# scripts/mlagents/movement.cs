//Script for movement of vehicles
//To be applied to each vehicle
//Anton Rajko FAU

using UnityEngine;

public class movement : MonoBehaviour
{
    //Speed of movement and rotation (can be adjusted in the inspector)
    [SerializeField] private float speed = 10;
    [SerializeField] private float rotationSpeed = 20;

    //Variables for movement
    private float forwardAmount;
    private float turnAmount;
    private float translation;
    private bool canTurn = false;

    void Update()
    {

        //Used to rotate vehicle during each frame update
        float rotation = 0.0f;

        //Translates car forward on the relative positive Z axis
        translation = Time.deltaTime * Mathf.Abs(forwardAmount) * speed;
        transform.Translate(0, 0, translation);

        //Ensures car doesn't turn without moving first
        if (translation != 0)
        {

            canTurn = true;

        }

        //If car is moving, it can turn
        if (canTurn)
        {

            rotation = rotationSpeed * turnAmount * Time.deltaTime;
            transform.Rotate(0, rotation, 0);

        }

    }

    //Function that handles movement and rotation
    public void SetInputs(float forward, float turn)
    {

        forwardAmount = forward; turnAmount = turn;

    }

}