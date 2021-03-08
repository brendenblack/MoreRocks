using Assets.Code.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Player
{
    [CreateAssetMenu(menuName = "Events/Selection changed")]
    public class SelectionChangedEvent : GameEvent<SelectionChangedEventArgs>
    {
    }
}
