using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour {

    public GameObject gameControlObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider myCollision)
    {
        if (myCollision.tag == "Car")
        {
            if (myCollision.GetComponent<Car>().distance > 6)
            {
                gameControlObject.GetComponent<GameControl>().NewGeneration();
            }
            else                
            {
                myCollision.GetComponent<Car>().stolknovenie = true;
            }

        }

    }
}
