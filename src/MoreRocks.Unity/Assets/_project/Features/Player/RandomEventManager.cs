//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets._project.Features.Player
//{
    //public class RandomEventManager : MonoBehaviour
    //{
    //    [SerializeField] RandomEvent[] randomEvents;


    //    private RandomEvent activeEvent;
    //    private float eventSelectionDelay = 300;

    //    void Start()
    //    {
    //        StartCoroutine(EventSpawner());
    //    }

    //    IEnumerator EventSpawner()
    //    {
    //        while (true)
    //        {
    //            yield return new WaitForSeconds(eventSelectionDelay);
    //            if (activeEvent == null)
    //            {
    //                var randomEvent = randomEvents.First(); // whatever
    //                randomEvent.gameObject.SetActive(true);
    //                eventSelectionDelay = 300;
    //            }
    //            else
    //            {
    //                eventSelectionDelay = 1;
    //            }
    //        }
    //    }
    //}
//}
