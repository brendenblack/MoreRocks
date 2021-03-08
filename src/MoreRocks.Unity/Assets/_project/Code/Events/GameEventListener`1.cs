using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Code.Events
{
    //public abstract class GameEventListener<TEvent, TArgs> : MonoBehaviour, IGameEventListener<TArgs>
    //    where TEvent : GameEvent<TArgs>
    //{
    //    [Tooltip("What event this listener responds to")]
    //    public TEvent Event;

    //    [Tooltip("Response to invoke when Event is raised.")]
    //    public UnityEvent<TArgs> Response;

    //    private void OnEnable()
    //    {
    //        Event.RegisterListener(this);
    //    }
    //    private void OnDisable()
    //    {
    //        Event.UnregisterListener(this);
    //    }
    //    public void OnEventRaised(TArgs args)
    //    {
    //        Response.Invoke(args);
    //    }
    //}

    public abstract class GameEventListener<TArgs> : MonoBehaviour, IGameEventListener<TArgs>
    {
        [Tooltip("What event this listener responds to")]
        public GameEvent<TArgs> Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<TArgs> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }
        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }
        public void OnEventRaised(TArgs args)
        {
            Response.Invoke(args);
        }
    }
}
