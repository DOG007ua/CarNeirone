using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Car : MonoBehaviour, IComparable
{
    public int id;
    Color colorTeam;

    float[] distanceRay = new float[3];
    float distance = 0;
    float speed;
    float rotation;
    float[] neironParam = new float[2];      //выходные значения нейрона. Для отладки
    public int numberLap = 0;                       //номер круга
    Team team;
    float minSpeed = 0.4f;
    float maxSpeed = 1.0f;
    
    float maxAcceleretion = 1f;          //0,05
    float maxAngleRotation = 1.5f;          //0,2

    float maxDistanceRay = 50.0f;
    public bool crash = false;                  //столкнулась ли машина с препятствием
    public NeironNetWork neironNetWork;         //нейронная сеть машинки
    Vector3[] vectorColorRay = new Vector3[3];  //хранит цветные лучи
    Color[] colorRay = new Color[3];

    bool cameraOncar = false;            //находится ли камера на машинке. Перенести в скрипт камеры!!!!!!!!!
    public GameObject cameraPosition;           //куда будет ставиться камера
    public float Distance { get { return distance; } }
    public bool Crash { get { return crash; } set { crash = value; } }
    public bool CameraOncar { 
        get { return cameraOncar; } 
        set 
        { 
            cameraOncar = value;
            if (value && drawNeiron == null)
            {
                drawNeiron = new DrawNeiron(neironNetWork, maxDistanceRay);
                drawNeiron.SetNeironNetWork(neironNetWork);
            }
        } 
    }    

    public Team TeamObj
    {
        get { return team; }
        set { team = value; }
    }

    public Color ColorTeam
    {
        set
        {            
            colorTeam = value;
            this.gameObject.transform.Find("Body").GetComponent<Renderer>().material.color = value;
        }
    }

    Dictionary<string, Light> massLigth = new Dictionary<string, Light>();

    

    public event MyClass.CollisionCar eventCollisionCar;        //событие при столкновении
    DrawNeiron drawNeiron;
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
            else if (speed > maxSpeed) speed = maxSpeed;
        }
    }

    public int CompareTo(object obj)        //сортировка по расстоянию
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
        SetLigth();
        //SetRandomColor();
    }

    void SetRandomColor()
    {
        ColorTeam = new Color((float)UnityEngine.Random.Range(0, 1.0f), (float)UnityEngine.Random.Range(0, 1.0f), (float)UnityEngine.Random.Range(0, 1.0f));
    }


    void Update()
    {
        if(crash) return;
        //CoefSpeedGame();      //ускорение игры
        CalculationDistanceRay();   
        WorkNeiron();
        Move();
        CalculationDistanceMove();
    }

    void SetLigth()
    {

        massLigth.Add("Stop", this.gameObject.transform.Find("Ligth/Stop").GetComponent<Light>());
        massLigth.Add("RigthFront", this.gameObject.transform.Find("Ligth/RigthFront").GetComponent<Light>());
        massLigth.Add("LeftFront", this.gameObject.transform.Find("Ligth/LeftFront").GetComponent<Light>());
        massLigth.Add("RigthBack", this.gameObject.transform.Find("Ligth/RigthBack").GetComponent<Light>());
        massLigth.Add("LeftBack", this.gameObject.transform.Find("Ligth/LeftBack").GetComponent<Light>());
        massLigth.Add("Move", this.gameObject.transform.Find("Ligth/Move").GetComponent<Light>());
    }

    /*void CoefSpeedGame()
    {
        minSpeed = 0.3f * coefSpeedGame;
        maxSpeed = 1.5f * coefSpeedGame;

        maxAcceleretion = 0.03f * coefSpeedGame;
        maxAngleRotation = 0.2f * coefSpeedGame;
    }*/
    /// <summary>
    /// работа нейрона
    /// </summary>
    void WorkNeiron()
    {        
        float[] massRez = neironNetWork.Start(distanceRay);
        ControlNeiron(massRez[0], massRez[1]);
        neironParam[0] = massRez[0];       //укорение
        neironParam[1] = massRez[1];       //поворот
    }
    /// <summary>
    /// расчёт растояния до препятствия
    /// </summary>
    void CalculationDistanceRay()
    {
        RaycastHit[] hit = new RaycastHit[3];
        Ray[] ray = new Ray[3];

        vectorColorRay[0] = (Quaternion.AngleAxis(-30, Vector3.down) * transform.forward);
        vectorColorRay[1] = (Quaternion.AngleAxis(0, Vector3.down) * transform.forward); 
        vectorColorRay[2] = (Quaternion.AngleAxis(30, Vector3.down) * transform.forward) ;


        for (int i = 0; i < 3; i++)
        {
            ray[i] = new Ray(transform.position, vectorColorRay[i]);
            if (Physics.Raycast(ray[i], out hit[i]) && hit[i].transform.gameObject.tag == "Let")
            {
                Debug.DrawRay(transform.position, vectorColorRay[i] * hit[i].distance, colorRay[i]);                
                distanceRay[i] = hit[i].distance;
                if (distanceRay[i] > maxDistanceRay) distanceRay[i] = maxDistanceRay;
            }
        }
    }
    /// <summary>
    /// передвижение
    /// </summary>
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
            massLigth["Move"].enabled = true;
            massLigth["Stop"].enabled = false;
        }
        else
        {
            massLigth["Move"].enabled = false;
            massLigth["Stop"].enabled = true;
        }
    }
    /// <summary>
    /// поворот
    /// </summary>
    /// <param name="angle">угол поворота</param>
    public void Rotation(float angle)
    {
        rotation = angle;
        transform.Rotate(Vector3.down * angle);


        if (angle > 0)
        {
            massLigth["LeftBack"].enabled = true;
            massLigth["LeftFront"].enabled = true;
            massLigth["RigthBack"].enabled = false;
            massLigth["RigthFront"].enabled = false;
        }
        else
        {
            massLigth["LeftBack"].enabled = false;
            massLigth["LeftFront"].enabled = false;
            massLigth["RigthBack"].enabled = true;
            massLigth["RigthFront"].enabled = true;
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
    /// <summary>
    /// Нажатие кнопок. Это если я захочу поуправлять машинкой
    /// </summary>
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
    /// <summary>
    /// Если столкнулся с препятствием
    /// </summary>
    /// <param name="myCollision"></param>
    void OnTriggerEnter(Collider myCollision)
    {
        if(myCollision.tag == "Let")
        {
            CrashFunction();
        }        
    }

    public void NewGeneration(NeironNetWork massNeironNew)
    {
        neironNetWork = massNeironNew;
        distance = 0;
        numberLap = 0;
        crash = false;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        drawNeiron?.SetNeironNetWork(neironNetWork);
        drawNeiron?.SetMassColorNeironWeigth();
        //coefSpeedGame = 1;
    }

    void CrashFunction()
    {
        speed = 0;
        crash = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        eventCollisionCar(this);
    }

    void OnGUI()
    {
        if(cameraOncar)
        {
            GUI_CarInformation();
            GUI.Box(new Rect(drawNeiron.left - 30, drawNeiron.top + 30, drawNeiron.width + 50, drawNeiron.heigth - 60), "");
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.green;
            GUI.Label(new Rect(drawNeiron.outValuePosition[0].x + 35, drawNeiron.outValuePosition[0].y - 10, 80, 20), "Ускорение", style);
            GUI.Label(new Rect(drawNeiron.outValuePosition[1].x + 35, drawNeiron.outValuePosition[1].y - 10, 80, 20), "Поворот", style);

            drawNeiron.DrawResult();            
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
        GUI.Label(new Rect(left, top + 105, width, 40), "Растояние: " + Math.Round(distance, 1), smallFont);
        GUI.Label(new Rect(left, top + 140, width, 40), "Круг: " + numberLap, smallFont);
        GUI.Label(new Rect(left, top + 170, width, 40), Math.Round(neironParam[0], 3).ToString() + "||" + Math.Round(neironParam[1], 3).ToString(), smallFont);
    }
}

public class NeironParam
{
    public float koefSpeed;
    public float koefRotation;
}

public class DrawNeiron
{
    NeironNetWork neironNetWork;
    Vector2[][] massPositionNeiron;
    public static Color[] massGradient;

    public int left = 1400;
    public int top = 10;          //Screen.Heigth - 450

    public int width = 500;
    public int heigth = 400;

    int stepX;
    int amountLayer;
    float maxDistanceRay = 0;

    float sizeNeiron = 20;
    float widthWeigth = 5.0f;
    /// <summary>
    /// № слоя|№ нейрона|№ нейрона пред. слоя
    /// </summary>
    Color[][][] massColorWeigth;

    public Vector2[] outValuePosition;
    MyPoint pDelete;

    static DrawNeiron()
    {
        SetMassGradient();
    }

    public DrawNeiron(NeironNetWork neironNetWork, float maxDistanceRay)
    {
        this.neironNetWork = neironNetWork;
        amountLayer = neironNetWork.AmountNeironInLayer.Length;
        stepX = width / amountLayer;
        this.maxDistanceRay = maxDistanceRay;
        outValuePosition = new Vector2[neironNetWork.AmountNeironInLayer[neironNetWork.AmountNeironInLayer.Length - 1]];
        CalculationPositionNeiron();
        InicializatioMassColorNeiron();
        SetMassColorNeironWeigth();
    }

    public void SetNeironNetWork(NeironNetWork neironNetWork)
    {
        this.neironNetWork = neironNetWork;
    }

    void CalculationPositionNeiron()
    {
        massPositionNeiron = new Vector2[amountLayer][];

        for (int i = 0; i < amountLayer; i++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[i].Length;
            int stepY = heigth / (amountNeironInLayer + 1);
            massPositionNeiron[i] = new Vector2[amountNeironInLayer];
            for (int j = 0; j < amountNeironInLayer; j++)
            {
                massPositionNeiron[i][j].x = left + i * stepX;
                massPositionNeiron[i][j].y = top + (j + 1) * stepY;
                if(i == amountLayer - 1)
                {
                    outValuePosition[j] = new Vector2(left + i * stepX, top + (j + 1) * stepY);
                }
            }
        }
    }

    void InicializatioMassColorNeiron()
    {
        massColorWeigth = new Color[amountLayer - 1][][];
        for (int i = 0; i < amountLayer - 1; i++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[i].Length;
            massColorWeigth[i] = new Color[amountNeironInLayer][];
            for (int j = 0; j < amountNeironInLayer; j++)
            {
                massColorWeigth[i][j] = new Color[neironNetWork.MassNeiron[i][j].weight.Length];       //Колличество весов в нейроне
            }
        }
    }

    public void DrawResult()
    {
        PaintWeigth();
        PaintNeiron();
    }

    void PaintWeigth()
    {
        
        for (int x = 0; x < amountLayer - 1; x++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[x].Length;
            for (int y = 0; y < amountNeironInLayer; y++)
            {
                int amountW = neironNetWork.MassNeiron[x][y].weight.Length;
                for (int k = 0; k < amountW; k++)
                {
                    Vector2 p1 = new Vector2(massPositionNeiron[x][y].x, massPositionNeiron[x][y].y);
                    Vector2 p2 = new Vector2(massPositionNeiron[x + 1][k].x, massPositionNeiron[x + 1][k].y);                    
                    DrawLine(p1, p2, massColorWeigth[x][y][k], widthWeigth);                    
                }
            }
        }
    }


    //Вызывать при новом поколении. Может подписаться на событие???
    public void SetMassColorNeironWeigth()
    {
        float min = 1000;
        float max = -1000;
        FindMinMaxWeigth(ref min, ref max);

        for (int x = 0; x < amountLayer - 1; x++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[x].Length;
            for (int y = 0; y < amountNeironInLayer; y++)
            {
                for (int k = 0; k < neironNetWork.MassNeiron[x][y].weight.Length; k++)
                {
                    Color c = GetColor(min, max, neironNetWork.MassNeiron[x][y].weight[k]);                     //удалить
                    massColorWeigth[x][y][k] = GetColor(min, max, neironNetWork.MassNeiron[x][y].weight[k]);
                }
            }
        }
    }

    void FindMinMaxWeigth(ref float min, ref float max)
    {
        for (int x = 0; x < amountLayer - 1; x++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[x].Length;
            for (int y = 0; y < amountNeironInLayer; y++)
            {
                for (int k = 0; k < neironNetWork.MassNeiron[x][y].weight.Length; k++)
                {
                    if (neironNetWork.MassNeiron[x][y].weight[k]  > max) max = neironNetWork.MassNeiron[x][y].weight[k];
                    if (neironNetWork.MassNeiron[x][y].weight[k]  < min) min = neironNetWork.MassNeiron[x][y].weight[k];
                }
            }
        }
    }


    void PaintNeiron()
    {
        for (int x = 0; x < amountLayer; x++)
        {
            int amountNeironInLayer = neironNetWork.MassNeiron[x].Length;
            for (int y = 0; y < amountNeironInLayer; y++)
            {
                Color color;
                if (x == 0) color = GetColor(0, maxDistanceRay, neironNetWork.MassNeiron[x][y].OutValue);
                else color = GetColor(-1, 1, neironNetWork.MassNeiron[x][y].OutValue);

                Vector2 p1 = new Vector2(massPositionNeiron[x][y].x - 10, massPositionNeiron[x][y].y);
                Vector2 p2 = new Vector2(massPositionNeiron[x][y].x + 10, massPositionNeiron[x][y].y);

                DrawLine(p1, p2, color, sizeNeiron);
            }
        }
    }

    struct tRGB { public int R, G, B; }

    Color GetColor(float min, float max, float val)
    {
        float dev = (val - min) / (max - min);
        return massGradient[(int)((massGradient.Length - 1) * dev)];
    }

    static void SetMassGradient()
    {
        int numPoint = 1000;
        massGradient = new Color[numPoint];

        tRGB Color1 = new tRGB();
        Color1.R = 255;
        Color1.G = 0;
        Color1.B = 0;

        tRGB Color2 = new tRGB();          //Желтый
        Color2.R = 255;
        Color2.G = 255;
        Color2.B = 0;

        tRGB Color3 = new tRGB();
        Color3.R = 0;                       //синий
        Color3.G = 255;
        Color3.B = 0;

        int diapaz_1 = (int)(0.5 * numPoint);
        int diapaz_2 = numPoint - diapaz_1;

        double Rg = (Color2.R - Color1.R);
        double Gg = (Color2.G - Color1.G);
        double Bg = (Color2.B - Color1.B);

        Rg /= diapaz_1;
        Gg /= diapaz_1;
        Bg /= diapaz_1;

        for (int i = 0; i < diapaz_1; i++)
        {
            massGradient[i] = new Color((float)((Color1.R + Rg * (i))/255.0), (float)((Color1.G + Gg * (i)) / 255.0), (float)((Color1.B + Bg * (i))/255.0));
        }

        Rg = (Color3.R - Color2.R);
        Gg = (Color3.G - Color2.G);
        Bg = (Color3.B - Color2.B);

        Rg /= diapaz_2;
        Gg /= diapaz_2;
        Bg /= diapaz_2;



        for (int i = 0; i < diapaz_2; i++)
        {
            massGradient[diapaz_1 + i] = new Color((float)((Color2.R + Rg * (i)) / 255.0), (float)((Color2.G + Gg * (i)) / 255.0), (float)((Color2.B + Bg * (i)) / 255.0));
        }
    }

    public static Texture2D lineTex;
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;

        // Generate a single pixel texture if it doesn't exist
        if (!lineTex) { lineTex = new Texture2D(1, 1); }

        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;

        // Determine the angle of the line.
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);

        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) { angle = -angle; }

        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));


        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);

        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}

