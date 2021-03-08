using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Resources
{
    [CreateAssetMenu(menuName = "Resource type")]
    public class ResourceType : ScriptableObject
    {
        [SerializeField] string key;
        [SerializeField] string displayName;
        [SerializeField] float defaultMaxCapacity;

        public string Key => key;

        public string DisplayName => displayName;

        public float DefaultMaxCapacity => defaultMaxCapacity;
    }
}
