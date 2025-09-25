using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    public Text resourceText; // assign in inspector

    private void Start()
    {
        if (EconomyManager.Instance != null)
        {
            UpdateText();
            EconomyManager.Instance.OnResourcesChanged += UpdateText;
        }
    }

    private void OnDestroy()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnResourcesChanged -= UpdateText;
    }

    private void UpdateText()
    {
        resourceText.text = $"Gold: {EconomyManager.Instance.gold}     " +
                            $"Stone: {EconomyManager.Instance.stone}     " +
                            $"Wood: {EconomyManager.Instance.wood}";
    }
}
