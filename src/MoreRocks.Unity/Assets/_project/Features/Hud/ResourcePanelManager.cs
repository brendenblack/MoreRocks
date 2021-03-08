using Assets.Features.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Hud
{
public class ResourcePanelManager : MonoBehaviour
{
    [SerializeField] ResourceTypeTextboxMap[] resourceTextboxes;

        public void HandleResourceChange(PlayerResourceChangeEventArgs args)
        {
            if (resourceTextboxes != null & resourceTextboxes.Length > 0)
            {
                var textbox = resourceTextboxes
                    .Where(m => m.ResourceType == args.ResourceType)
                    .Select(m => m.Textbox)
                    .FirstOrDefault();

                if (textbox != null)
                {
                    textbox.text = Mathf.Min(args.CurrentAmount, 9999).ToString("N0");
                }
            }
        }
    }
}
