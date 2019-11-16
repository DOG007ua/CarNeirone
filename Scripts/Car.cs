using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Car : MonoBehaviour, IComparable
{
    public int id;

    public float[] distanceRay = new float[3];
    public float distance = 0;
    float speed;
    float rotation;
    public float[] neironParam = new float[2];
    public int numberLap = 0;

    float minSpeed = 0.3f;
    float maxSpeed = 1.0f;
    
    float maxAcceleretion = 0.05f;
    float maxAngleRotation = 0.2f;
    
    public bool crash = false;
    public NeironNetWork neironNetWork;
    Vector3[] vector = new Vector3[3];
    Color[] colorRay = new Color[3];

    public int coefSpeedGame = 1;
    public bool cameraOncar = false;

    public Light ligthStop;
    public Light ligthRigth;
    public Light ligthLeft;
    public Light ligthRigthFront;
    public Light ligthLeftFront;
    public Light ligthMove;
    public GameObject cameraPosition;

    public event MyClass.CollisionCar eventCollisionCar;

    public float RotationReturn { get { return rotation; } }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
            if (speed < minSpeed) speed = minSpeed;
            if (speed > maxSpeed) speed = maxSpeed;
        }
    }

    public int CompareTo(object obj)
    {
        Car car = obj as Car;

        if (car != null)
        {
            if (this.distance < car.distance) return 1;
            else if (this.distance > car.distance) return -1;
            else return 0;
        }
        else
            throw new Exception("Невозможно сравнить два объекта");
    }


    void Start()
    {
        colorRay[0] = new Color(255, 0, 0);
        colorRay[1] = new Color(0, 255, 0);
        colorRay[2] = new Color(0, 0, 255);
        speed = minSpeed;
        neironNetWork = new NeironNetWork();
    }

    void Update()
    {
        if (crash) return;
        //CoefSpeedGame();
        CalculationDistanceRay();
        WorkNeiron();
        Move();
        CalculationDistanceMove();

        //delta = neironNetWork.delta;
    }

    void CoefSpeedGame()
    {
        minSpeed = 0.3f * coefSpeedGame;
        maxSpeed = 1.5f * coefSpeedGame;

        maxAcceleretion = 0.03f * coefSpeedGame;
        maxAngleRotation = 0.2f * coefSpeedGame;
    }
    /// <summary>
    /// работа нейрона
    /// </summary>
    void WorkNeiron()
    {        
        float[] massRez = neironNetWork.Start(distanceRay);
        ControlNeiron(massRez[0] - massRez[1], massRez[2] - massRez[3]);
        neironParam[0] = massRez[0] - massRez[1];
        neironParam[1] = massRez[3] - massRez[2];
    }

    void CalculationDistanceRay()
    {
        RaycastHit[] hit = new RaycastHit[3];
        Ray[] ray = new Ray[3];
        float maxD = 30.0f;

        vector[0] = (Quaternion.AngleAxis(-30, Vector3.down) * transform.forward);
        vector[1] = (Quaternion.AngleAxis(0, Vector3.down) * transform.forward); 
        vector[2] = (Quaternion.AngleAxis(30, Vector3.down) * transform.forward) ;


        for (int i = 0; i < 3; i++)
        {
            ray[i] = new Ray(transform.position, vector[i]);
            if (Physics.Raycast(ray[i], out hit[i]) && hit[i].transform.gameObject.tag == "Let")
            {
                Debug.DrawRay(transform.position, vector[i] * hit[i].distance, colorRay[i]);                
                distanceRay[i] = hit[i].distance;
                if (distanceRay[i] > maxD) distanceRay[i] = maxD;
            }
        }
    }

    public void Move()
    {
        transform.position += transform.forward * Speed;
    }

    /// <summary>
    /// Пройденное расстояние
    /// </summary>
    void CalculationDistanceMove()
    {
        distance += speed * Time.deltaTime;
    }

    /// <summary>
    /// Ускорение
    /// </summary>
    /// <param name="acceleretion"></param>
    public void SetSpeed(float acceleretion)
    {
        Speed += acceleretion * Time.deltaTime;
        if (acceleretion > 0)
        {
            ligthMove.enabled = true;
            ligthStop.enabled = false;
        }
        else
        {
            ligthMove.enabled = false;
            ligthStop.enabled = true;
        }
    }

    public void Rotation(float angle)
    {
        rotation = angle;
        transform.Rotate(Vector3.down * angle);
        if(angle > 0)
        {
            ligthLeft.enabled = true;
            ligthLeftFront.enabled = true;
            ligthRigth.enabled = false;
            ligthRigthFront.enabled = false;
        }
        else
        {
            ligthLeft.enabled = false;
            ligthLeftFront.enabled = false;
            ligthRigth.enabled = true;
            ligthRigthFront.enabled = true;
        }            
    }

    public void SetCameraOnCar(bool status)
    {
        cameraOncar = status;
    }

    /// <summary>
    /// Сюда нейрон передаёт ускорение скорости и поворота
    /// </summary>
    /// <param name="speed">скорость от -1 до 1</param>
    /// <param name="rotation">ускорение от -1 до 1</param>
    void ControlNeiron(float koefSpeed, float koefRotation)
    {
        SetSpeed(koefSpeed * maxAcceleretion);
        Rotation(koefRotation * maxAngleRotation);
    }

    void KeyDown()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetSpeed(maxAcceleretion);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SetSpeed(-maxAcceleretion);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Rotation(maxAngleRotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotation(-maxAngleRotation);
        }
    }

    void OnTriggerEnter(Collider myCollision)
    {
        if(myCollision.tag == "Let")
        {
            Crash();
        }
        
    }

    void Crash()
    {
        speed = 0;
        crash = true;
        //this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        //carPrefab.GetComponent<MeshRenderer>().enabled = false;
        //carPrefab.GetComponent<BoxCollider>().enabled = false;
        eventCollisionCar(this);
    }

    void OnGUI()
    {
        if(cameraOncar)
        {
            GUI_CarInformation();
        }
    }

    void GUI_CarInformation()
    {
        GUIStyle smallFont = new GUIStyle();
        smallFont.fontSize = 30;
        smallFont.normal.textColor = Color.yellow;
        int top = Screen.height - 300;
        int width = 200;
        int left = (int)(Screen.width / 2.0 - width / 2.0);
        GUI.Box(new Rect(left, top, 300, width), "Машина №" + id);
        GUI.Label(new Rect(left, top + 35, width, 40), "Скорость: " + Math.Round(speed, 3), smallFont);
        GUI.Label(new Rect(left, top + 70, width, 40), "Поворот: " + Math.Round(RotationReturn, 3), smallFont);
        GUI.Label(new Rect(left, top + 105, width, 40), "Растояние: " + Math.Round(distance, 3), smallFont);
        GUI.Label(new Rect(left, top + 140, width, 40), "Круг: " + numberLap, smallFont);
    }
}

public class NeironParam
{
    public float koefSpeed;
    public float koefRotation;
}

