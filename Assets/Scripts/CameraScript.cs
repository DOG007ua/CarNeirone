using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float speed = 0;
    float dist = 0;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //KeyDown();
        //CalculationDistanceRay();
    }

    void KeyDown()
    {
        /*if (Input.GetKey(KeyCode.W))
        {
            Move(1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Move(-1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Rotation(1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotation(-1);
        }*/
    }

    public void Move(int acceleration)
    {
        //transform.position += transform.forward * speed * acceleration;
    }

    public void Rotation(float angle)
    {
        //transform.Rotate(Vector3.down * angle);
    }

    void CalculationDistanceRay()
    {
        /*RaycastHit rayHit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit) && rayHit.transform.gameObject.tag == "Let")
        {
            dist = rayHit.distance;
            //dist = Vector3.Distance(this.transform.position, rayHit.point);
        }*/
    }

    private void OnGUI()
    {
        /*GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.fontSize = 25;
        GUI.Label(new Rect(Screen.width / 2.0f, 120, 150, 40), dist.ToString(), style);*/

    }
}
