using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Events
{
    [CreateAssetMenu(menuName = "Events/Game event (no args)")]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners = new List<GameEventListener>();

        public virtual void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public virtual void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public virtual void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
