using Assets.Features.Resources;
using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] string displayName;
    [SerializeField] ResourceType resourceType;
    [SerializeField] int availableResource;
    [SerializeField] int resourceCapacity;
    [SerializeField] int activeGatherers;

    [SerializeField] GameObject sprite100percent;
    [SerializeField] GameObject sprite66Percent;
    [SerializeField] GameObject sprite33Percent;

    public string DisplayName => (string.IsNullOrWhiteSpace(displayName)) ? $"{resourceType.DisplayName} node" : displayName;

    public ResourceType ResourceType => resourceType;

    public (bool Success, int ReceivedAmount) TryGatherResource(int gatherAmount)
    {
        var result = (false, 0);

        if (availableResource > 0)
        {
            var gatheredAmount = (int)Mathf.Min(gatherAmount, availableResource);
            availableResource -= Mathf.Clamp(gatheredAmount, 0, resourceCapacity);
            result = (true, gatheredAmount);
        }

        UpdateSprite();
        return result;
    }

    private void UpdateSprite()
    {
        float resourcePercent = availableResource / (float)resourceCapacity;

        if (resourcePercent >= 0.66)
        {
            sprite100percent.SetActive(true);
            sprite66Percent.SetActive(false);
            sprite33Percent.SetActive(false);
        }
        else if (resourcePercent >= 0.33)
        {
            sprite100percent.SetActive(false);
            sprite66Percent.SetActive(true);
            sprite33Percent.SetActive(false);
        }
        else if (resourcePercent > 0)
        {
            sprite100percent.SetActive(false);
            sprite66Percent.SetActive(false);
            sprite33Percent.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
