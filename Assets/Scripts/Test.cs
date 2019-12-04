using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        //g.GetComponent<Material>().color = value;
        //this.gameObject.transform.Find("Body").GetComponent<Material>().color = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
