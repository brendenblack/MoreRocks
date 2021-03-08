using Assets.Features.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Player
{
    public class PlayerResourceManager : MonoBehaviour
    {
        [SerializeField] List<ResourceType> supportedResourceTypes;

        public PlayerResourceChangeEvent OnResourceChanged;

        private Dictionary<ResourceType, float> resourceHoldings = new Dictionary<ResourceType, float>();
        private Dictionary<ResourceType, float> resourceCapacities = new Dictionary<ResourceType, float>();

        public IReadOnlyDictionary<ResourceType, float> ResourceCapacities => resourceCapacities;
        public IReadOnlyDictionary<ResourceType, float> ResourceHoldings => resourceHoldings;

        private void Start()
        {
            foreach (var resourceType in supportedResourceTypes)
            {
                resourceHoldings.Add(resourceType, 0);
                resourceCapacities.Add(resourceType, resourceType.DefaultMaxCapacity);

                if (OnResourceChanged)
                {
                    Debug.Log($"Starting {resourceType.DisplayName} with {resourceHoldings[resourceType]} out of {resourceCapacities[resourceType]}");
                    OnResourceChanged.Raise(new PlayerResourceChangeEventArgs(0, resourceHoldings[resourceType], resourceType));
                }
            }
        }

        public float GetMaxForResource(ResourceType resourceType)
        {
            float capacity;
            if (!resourceCapacities.TryGetValue(resourceType, out capacity))
            {
                resourceCapacities[resourceType] = resourceType.DefaultMaxCapacity;
            }

            return capacity;
        }

        public float GetAmountHeldForResource(ResourceType resourceType)
        {
            float amount;
            if (!resourceHoldings.TryGetValue(resourceType, out amount))
            {
                resourceHoldings[resourceType] = 0;
            }

            return amount;
        }

        public bool IsResourceAtCapacity(ResourceType resourceType)
        {
            return GetAmountHeldForResource(resourceType) >= GetMaxForResource(resourceType);
        }

        public bool TryDepositResource(float amount, ResourceType resourceType)
        {
            if (IsResourceAtCapacity(resourceType))
            {
                return false;
            }

            Debug.Log($"Received deposit of {amount}x {resourceType.DisplayName}", this);

            var currentResourceAmount = GetAmountHeldForResource(resourceType);
            Debug.Log($"Currently holding {currentResourceAmount} {resourceType.DisplayName}", this);

            var updatedResourceAmount = Mathf.Min(currentResourceAmount + amount, GetMaxForResource(resourceType));

            resourceHoldings[resourceType] = updatedResourceAmount;
            Debug.Log($"New holding of {updatedResourceAmount} {resourceType}", this);

            if (OnResourceChanged)
            {
                OnResourceChanged.Raise(new PlayerResourceChangeEventArgs(currentResourceAmount, updatedResourceAmount, resourceType));
            }

            return true;
        }

    }
}
