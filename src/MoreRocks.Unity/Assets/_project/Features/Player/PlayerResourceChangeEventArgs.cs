using Assets.Features.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Features.Player
{
    public class PlayerResourceChangeEventArgs
    {
        public PlayerResourceChangeEventArgs(float oldAmount, float newAmount, ResourceType resourceType)
        {
            PreviousAmount = oldAmount;
            CurrentAmount = newAmount;
            ResourceType = resourceType;
        }

        public float PreviousAmount { get; private set; }
        
        public float CurrentAmount { get; private set; }

        public ResourceType ResourceType { get; private set; }
    }
}
