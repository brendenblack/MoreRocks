using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Events
{
    public abstract class GameEvent<TArgs> : ScriptableObject
    {
        protected List<IGameEventListener<TArgs>> listeners = new List<IGameEventListener<TArgs>>();

        public TArgs EventArgs { get; private set; }

        public virtual void Raise(TArgs args)
        {
            EventArgs = args;

            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(args);
            }
        }

        public virtual void RegisterListener(IGameEventListener<TArgs> listener)
        {
            listeners.Add(listener);
        }

        public virtual void UnregisterListener(IGameEventListener<TArgs> listener)
        {
            listeners.Remove(listener);
        }
    }
}
