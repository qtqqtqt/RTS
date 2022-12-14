using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] GameObject healthBarParent;
        [SerializeField] Image healthBarImage;

        private void Awake()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

    }
}
