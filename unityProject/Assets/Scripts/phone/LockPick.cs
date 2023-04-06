using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LockPick : MonoBehaviour
{
    //images for the turning linked
    [SerializeField]
    private Transform innerLock;
    [SerializeField]
    private Transform pickPosition;

    //unlock mechanism variables
    [SerializeField]
    private float lockSpeed = 5f;
    [SerializeField]
    private float lockRange = 10;
    [SerializeField]
    private float unlockAngle = 15;
    private float differenceAngle;
    private float eulerAngle;
    private Vector3 pickRotation;

    //stuff
    [SerializeField]
    private int timesTurned = 0;
    private bool movePick = true;

    //timer variables
    [SerializeField]
    private float timerTime;
    private float countdownTime = 1f;
    private bool isRunning;

    void Start()
    {
        NewLock();
        pickRotation = Vector3.zero;
        Input.gyro.enabled = true;
    }

    void Update()
    {
        //set the rotation of the pick to the rotation of the gyro
        transform.localPosition = pickPosition.position;
        pickRotation.z = Input.gyro.rotationRateUnbiased.z;

        //when not checking the position by touching the pick is movable
        if (movePick)
        {
            //give eulerAngle the gyro information to make calculations
            eulerAngle = Input.gyro.attitude.eulerAngles.z;

            transform.rotation = Quaternion.Euler(0, 0, eulerAngle);

            //calculate the difference between the unlockable angle and the angle the pick is in
            differenceAngle = eulerAngle - unlockAngle;

            //unity works with 360 degrees. when the angle of the pick is over 180, reverse the numbers so the difference can be calculated correctly
            if (differenceAngle > 180)
            {
                differenceAngle = Mathf.Abs(360 - differenceAngle);
            }
        }

        //touching the phone to check the lock position
        if (Input.touchCount > 0)
        {
            movePick = false;

            if (differenceAngle < lockRange)
            {
                Debug.Log("unlock");
                ActivateTimer();

            }
        }
        else
        {
            movePick = true;
        }

        //timer stuff
        if (isRunning)
        {
            countdownTime -= Time.deltaTime;
            movePick = false;
            RotateInner();

            if (countdownTime <= 0)
            {
                timesTurned++;
                NewLock();
                countdownTime = timerTime;
                isRunning = false;
                movePick = true;
            }
        }

        if(timesTurned == 3)
        {
            GameDone();
        }
    }

    void GameDone()
    {
        Debug.Log("game finished, yay");
        //switch scenes here
    }

    void ActivateTimer()
    {
        isRunning = true;
    }

    void RotateInner()
    {
        innerLock.transform.Rotate(new Vector3(0, 0, -10 * lockSpeed) * Time.deltaTime);
    }

    void NewLock()
    {
        if (timesTurned == 0)
        {
            unlockAngle = 90;
        }
        if (timesTurned == 1)
        {
            unlockAngle = 15;
        }
        if (timesTurned == 2)
        {
            unlockAngle = 90;
        }
    }
}
