using Assets.Features.Player;
using Assets.Features.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._project.Features.Buildings
{
    public class ResourceHub : MonoBehaviour
    {
        private Dictionary<ResourceType, float> gatheredResources = new Dictionary<ResourceType, float>();

        public IReadOnlyDictionary<ResourceType, float> GatheredResources => gatheredResources;

        [SerializeField] PlayerResourceManager playerResourceManager;

        public bool TryMakeDeposit(LabourerUnit depositor, float amount, ResourceType resourceType)
        {
            if (playerResourceManager.TryDepositResource(amount, resourceType))
            {
                if (!gatheredResources.TryGetValue(resourceType, out float currentAmount))
                {
                    currentAmount = 0.0f;
                }

                gatheredResources[resourceType] = currentAmount + amount;

                return true;
            }
            else
            {
                depositor.Idle();
                return false;
            }
        }

    }
}
