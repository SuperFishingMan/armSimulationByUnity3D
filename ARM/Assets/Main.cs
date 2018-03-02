using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    #region definition
    //将机械臂的组件关联到下面这些数组中
    GameObject[] capsuleArray = new GameObject[3];
    GameObject[] sphereArray = new GameObject[2];
    GameObject[] cubeArray = new GameObject[2];   
    GameObject[] jointArray = new GameObject[6];

    //private Vector3[] capsuleScale = new Vector3[3];
    //private Vector3[] sphereScale = new Vector3[2];
    //private Vector3[] cubeScale = new Vector3[2];
    //private Vector3[] jointScale = new Vector3[6];
    int steps = 100;//每次运动的步数，这个步数乘以fixupdate每帧的时间就是一次运动需要的时间，可以通过调整这个步数来控制机械臂运动的时间
    int cnt = 0;//计数器
    bool moveTag = false;//运动标志
    //各关节转动轴编号，即关节0绕x轴转，关节1绕y轴转，关节2绕z轴转。。。 可调
    public int[] axis_number = new int[] { 0, 1, 2, 0, 1, 2 };
    private float[] jointAngleRealTime = new float[6];//存储实时的角度

    private float[] targetAngle = new float[6];//目标角度
    private float[] sliderAngle = new float[6] { 90,90,90,90,90,90};//滑条上的角度
    private float[] deltaAngle = new float[6];//单步变化角度
    private float[] currentAngle = new float[6];//当前步数的关节角度
    

    #endregion

    //将场景的物体关联到该脚本中
    void InitObject()
    {
        string objName;
        for (int i = 0; i < jointArray.Length; i++)
        {
            objName = string.Format("Joint{0}", i);
            jointArray[i] = GameObject.Find(objName);
        }

        for (int i = 0; i < capsuleArray.Length; i++)
        {
            objName = string.Format("Capsule{0}", i);
            capsuleArray[i] = GameObject.Find(objName);
        }
        for (int i = 0; i < sphereArray.Length; i++)
        {
            objName = string.Format("Sphere{0}", i);
            sphereArray[i] = GameObject.Find(objName);
        }
        for(int i = 0;i<cubeArray.Length;i++)
        {
            objName = string.Format("Cube{0}", i);
            cubeArray[i] = GameObject.Find(objName);
        }


    }

    //将代表关节的空物体位置和姿态固定在球形表示的关节处
    void FixJoints()
    {
        for(int i =0;i<3;i++)
        {
            jointArray[i].transform.position = sphereArray[0].transform.position;
            jointArray[i].transform.rotation = sphereArray[0].transform.rotation;
        }
        for(int i =3;i<6;i++)
        {
            jointArray[i].transform.position = sphereArray[1].transform.position;
            jointArray[i].transform.rotation = sphereArray[1].transform.rotation;
        }
    }

    //设置机械臂的各组件父子关系
    void SetPSRelationship()
    {
        cubeArray[0].transform.parent = capsuleArray[0].transform;
        capsuleArray[0].transform.parent = sphereArray[0].transform;
        sphereArray[0].transform.parent = jointArray[0].transform;
        jointArray[0].transform.parent = jointArray[1].transform;
        jointArray[1].transform.parent = jointArray[2].transform;
        jointArray[2].transform.parent = capsuleArray[1].transform;
        capsuleArray[1].transform.parent = sphereArray[1].transform;
        sphereArray[1].transform.parent = jointArray[3].transform;
        jointArray[3].transform.parent = jointArray[4].transform;
        jointArray[4].transform.parent = jointArray[5].transform;
        jointArray[5].transform.parent = capsuleArray[2].transform;
        capsuleArray[2].transform.parent = cubeArray[1].transform;


    }

    //各个物体的缩放最好都是1，1，1，不然可能由于缩放导致模型形状发生奇怪的变化
    //void RecordScale()
    //{
    //    for(int i=0;i<capsuleArray.Length;i++)
    //    {
    //        capsuleScale[i] = capsuleArray[i].transform.localScale;
    //    }
    //    for (int i = 0; i < sphereArray.Length; i++)
    //    {
    //        sphereScale[i] = sphereArray[i].transform.localScale;
    //    }
    //    for (int i = 0; i < cubeArray.Length; i++)
    //    {
    //        cubeScale[i] = cubeArray[i].transform.localScale;
    //    }
    //    for (int i = 0; i < jointArray.Length; i++)
    //    {
    //        jointScale[i] = jointArray[i].transform.localScale;
    //    }

    //}
    //void RemainScale()
    //{
    //    for (int i = 0; i < capsuleArray.Length; i++)
    //    {
    //        capsuleArray[i].transform.localScale = capsuleScale[i];
    //    }
    //    for (int i = 0; i < sphereArray.Length; i++)
    //    {
    //        sphereArray[i].transform.localScale = sphereScale[i];
    //    }
    //    for (int i = 0; i < cubeArray.Length; i++)
    //    {
    //        cubeArray[i].transform.localScale=cubeScale[i];
    //    }
    //    for (int i = 0; i < jointArray.Length; i++)
    //    {
    //        jointArray[i].transform.localScale=jointScale[i];
    //    }
    //}

    public void setAngle(float[] angle)//控制角度转动
    {


        jointArray[0].transform.GetComponent<GeneralJoint>().angleMove(angle[0]);

        jointArray[1].transform.GetComponent<GeneralJoint>().angleMove(angle[1]);
        jointArray[2].transform.GetComponent<GeneralJoint>().angleMove(angle[2]);

        jointArray[3].transform.GetComponent<GeneralJoint>().angleMove(angle[3]);
        jointArray[4].transform.GetComponent<GeneralJoint>().angleMove(angle[4]);
        jointArray[5].transform.GetComponent<GeneralJoint>().angleMove(angle[5]);

        updateJointAngle();

    }

    void updateJointAngle()//更新关节实时角度显示
    {
        for (int i = 0; i < 6; i++)
        {
            jointAngleRealTime[i] = jointArray[i].GetComponent<GeneralJoint>().read_tempangle();
        }
    }

    //设置各关节的目标角度
	void SetTargetAngle()
    {
        for(int i =0;i<targetAngle.Length;i++)
        {
            targetAngle[i] = sliderAngle[i] - 90;
        }
    }

    //设置关节单步变化角度
    void getDeltaAngle()
    {
        
        for(int i =0;i<deltaAngle.Length;i++)
        {
            deltaAngle[i] = (targetAngle[i] - jointAngleRealTime[i]) / steps;
        }
    }
    //获得当前该步关节的角度
    void getCurrentAngle()
    {
        for(int i = 0;i<6;i++)
        {
            currentAngle[i] = jointAngleRealTime[i] + deltaAngle[i];
        }
    }



    // Use this for initialization
    void Start()
    {
        InitObject();
        FixJoints();

        //RecordScale();
        SetPSRelationship();
        
        setAngle(new float[6] { 0, 0, 0, 0, 0, 0 });

    }
    void FixedUpdate()//这个函数的执行是固定间隔的，可以在Unity中设置
    {
        if(moveTag)
        {
            if(cnt<100)
            {
                
                getCurrentAngle();
                setAngle(currentAngle);
                //RemainScale();
                cnt++;
            }
            else
            {

                setAngle(targetAngle);
                //RemainScale();
                moveTag = false;
            }

        }



    }

    void OnGUI()
    {
        //简单的操作界面

        if(GUI.Button(new Rect(10,20,50,30),"Move"))
        {
            SetTargetAngle();
            getDeltaAngle();
            cnt = 0;
            moveTag = true;
            
        }

        GUI.Label(new Rect(55, 125, 150, 500), "操作说明：先通过滑条调整各个关节的目标角度，然后按“move”按钮开始运动，注意运动未结束不要再次点击“move”按钮");

        GUI.BeginGroup(new Rect(50, 20, 400, 200));
        sliderAngle[0] = GUI.HorizontalSlider(new Rect(55, 25, 100, 20), sliderAngle[0], 0, 180);
        GUI.Label(new Rect(55, 5, Screen.width, 20), "joint0 value: " + ((int)sliderAngle[0] - 90) + " degree");

        sliderAngle[1] = GUI.HorizontalSlider(new Rect(55, 55, 100, 20), sliderAngle[1], 0, 180);
        GUI.Label(new Rect(55, 35, Screen.width, 20), "joint1 value: " + ((int)sliderAngle[1] - 90) + " degree");

        sliderAngle[2] = GUI.HorizontalSlider(new Rect(55, 85, 100, 20), sliderAngle[2], 0, 180);
        GUI.Label(new Rect(55, 65, Screen.width, 20), "joint2 value: " + ((int)sliderAngle[2] - 90) + " degree");

        sliderAngle[3] = GUI.HorizontalSlider(new Rect(205, 25, 100, 20), sliderAngle[3], 0, 180);
        GUI.Label(new Rect(205, 5, Screen.width, 20), "joint3 value: " + ((int)sliderAngle[3] - 90) + " degree");

        sliderAngle[4] = GUI.HorizontalSlider(new Rect(205, 55, 100, 20), sliderAngle[4], 0, 180);
        GUI.Label(new Rect(205, 35, Screen.width, 20), "joint4 value: " + ((int)sliderAngle[4] - 90) + " degree");

        sliderAngle[5] = GUI.HorizontalSlider(new Rect(205, 85, 100, 20), sliderAngle[5], 0, 180);
        GUI.Label(new Rect(205, 65, Screen.width, 20), "joint5 value: " + ((int)sliderAngle[5] - 90) + " degree");

        GUI.EndGroup();



    }
}
