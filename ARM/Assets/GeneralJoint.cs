using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralJoint : MonoBehaviour {

    private string objectName;
    private int joint_number;
    GameObject camera;
    Main mainScript;

    float[] temp_angle = { 0, 0, 0 };
    int axis_number;//该关节的旋转轴，0代表x轴，1代表y轴，2代表z轴，均为本体轴

    public int Axis_number
    {
        get
        {
            return axis_number;
        }
        set
        {
            axis_number = Axis_number;
        }

    }

    //读取该关节的当前关节转角
    public float read_tempangle()
    {
        return temp_angle[Axis_number];
    }
    //改变该关节的当前关节转角
    public void changetemp_angle(float angle)
    {
        temp_angle[Axis_number] = -angle;
        transform.localRotation = Quaternion.Euler(temp_angle[0], temp_angle[1], temp_angle[2]);

    }

    //通过控制函数控制该关节的转角
    public void angleMove(float ang)
    {
        if (ang > 180)
        {
            while (ang > 180)
            {
                ang -= 360;
            }
        }
        else if (ang < -180)
        {
            while (ang < -180)
            {

                ang += 360;
            }

        }
        temp_angle[Axis_number] = ang;

        transform.localRotation = Quaternion.Euler(temp_angle[0], temp_angle[1], temp_angle[2]);
    }

    void angle_change(ref float temp, float step)
    {
        temp = temp + step;
    }


    // Use this for initialization
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        objectName = gameObject.name;
        joint_number = int.Parse(objectName.Substring(5, 1));
        
        mainScript = camera.GetComponent<Main>();
        axis_number = mainScript.axis_number[joint_number];

    }

    // Update is called once per frame
    void Update()
    {

    }
}
