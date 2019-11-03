using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {



    public float[] dist;
    Vector3[] vector;
    Color[] color = new Color[3];
    // Use this for initialization
    void Start () {
        dist = new float[3];
        vector = new Vector3[3];
        
         

        color[0] = new Color(255, 0, 0);
        color[1] = new Color(0, 255, 0);
        color[2] = new Color(0, 0, 255);
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit[] hit = new RaycastHit[3];
        Ray[] ray = new Ray[3];

        /*vector[0] =  new Vector3(0.707f, 0, 0.707f);
        vector[1] =  new Vector3(1, 0, 0);
        vector[2] =  new Vector3(0.707f, 0, -0.707f);*/


        vector[0] = Quaternion.AngleAxis(-30, Vector3.down) * transform.right;
        vector[1] =  Quaternion.AngleAxis(0, Vector3.down) * transform.right;
        vector[2] = Quaternion.AngleAxis(30, Vector3.down) * transform.right;


        for (int i = 0; i < 3; i++)
        {
            ray[i] = new Ray(transform.position, vector[i]);
            if (Physics.Raycast(ray[i], out hit[i]))
            {
                Debug.DrawRay(transform.position, vector[i] * hit[i].distance, color[i]);
                dist[i] = hit[i].distance;
            }
        }
    }
}
