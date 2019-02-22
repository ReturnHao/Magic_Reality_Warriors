using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject bullet;
    public float speed = 5f;

    private int ballIndex = 0;
    private List<GameObject> objBall;

    private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro, lastgyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;

    // Start is called before the first frame update
    void Start()
    {
        objBall = new List<GameObject>();

        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];
            // GetButtonDown checks if a button has been pressed (not held)
            if (j.GetButtonDown(Joycon.Button.HOME))
            {
                ballIndex++;
                GameObject ball = Instantiate(bullet, transform.position, transform.rotation);
                ball.name = "Ball" + ballIndex;
                objBall.Add(ball);

                Rigidbody rig = ball.GetComponent<Rigidbody>();

                rig.velocity = transform.forward * speed;
                if (ballIndex > 5)
                {
                    Destroy(objBall[0]);
                    objBall.RemoveAt(0);
                }
            }
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 pressed");
                // GetStick returns a 2-element vector with x/y joystick components
                Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}", j.GetStick()[0], j.GetStick()[1]));

                // Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
                j.Recenter();
            }
            // GetButtonDown checks if a button has been released
            if (j.GetButtonUp(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 released");
            }
            // GetButtonDown checks if a button is currently down (pressed or held)
            if (j.GetButton(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 held");
            }

            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Debug.Log("Rumble");

                // Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
                // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

                j.SetRumble(160, 320, 0.6f, 200);

                // The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
                // (Useful for dynamically changing rumble values.)
                // Then call SetRumble(0,0,0) when you want to turn it off.
            }

            stick = j.GetStick();

            // Gyro values: x, y, z axis values (in radians per second)
            lastgyro = gyro;
            gyro = j.GetGyro();

            //Debug.Log(gyro.x);

            if ((gyro.x >= 0.5) && (lastgyro.x <= -0.5))
            {
                Debug.Log("Fire");
                j.SetRumble(160, 320, 0.6f, 200);
                ballIndex++;
                GameObject ball = Instantiate(bullet, transform.position, transform.rotation);
                ball.name = "Ball" + ballIndex;
                objBall.Add(ball);

                Rigidbody rig = ball.GetComponent<Rigidbody>();
                rig.velocity = transform.forward * speed;

                if (ballIndex > 5)
                {
                    Destroy(objBall[0]);
                    objBall.RemoveAt(0);
                }
            }

            // Accel values:  x, y, z axis values (in Gs)
            accel = j.GetAccel();

            orientation = j.GetVector();
            if (j.GetButton(Joycon.Button.DPAD_UP))
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            gameObject.transform.rotation = orientation;
        }
    }
}
