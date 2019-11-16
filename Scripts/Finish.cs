using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour {

    public GameObject gameControlObject;
    int maxLap = 4;

    void OnTriggerEnter(Collider myCollision)
    {
        //Если проехала машина через финиш
        if (myCollision.tag == "Car")
        {
            Car car = myCollision.GetComponent<Car>();
            car.numberLap++;            //добавляю к этой машине пройденный круг
            if (car.numberLap >= maxLap && car.distance > 10)
            {
                gameControlObject.GetComponent<GameControl>().NewGeneration();      //Если одна из машин проехала максимальное колличество кругов. 10 - что бы машины на старте, которые развернулись, не накручивали круги
            }
            else                
            {
                //if(car.numberLap == 1) myCollision.GetComponent<Car>().stolknovenie = true;
            }

        }

    }
}
