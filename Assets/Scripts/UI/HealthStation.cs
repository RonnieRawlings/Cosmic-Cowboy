// Author - Ronnie Rawlings.

using TMPro;
using UnityEngine;

public class HealthStation : MonoBehaviour
{
    // Attatched text comp ref.
    private TextMeshProUGUI textComp;

    // Health increase amount.
    [SerializeField] private int healthIncrease;

    // Has health already been increased.
    private bool canIncreaseHealth = true;

    /// <summary> method <c>CheckForClick</c> check if the specificed key has been clicked. </summary>
    private void CheckForClick()
    {
        // Call method if key clicked.
        if (Input.GetKeyDown(KeyCode.E)) { IncreaseHealth(); }
    }

    /// <summary> method <c>IncreaseHealth</c> increase player health by specified amount, if still avaible. </summary>
    private void IncreaseHealth()
    {
        // Checks if increasing HP is available.
        if (BattleInfo.currentPlayerHealth < BattleValues.basePlayerHealth)
        {
            // Calculate the potential new health after the increase.
            BattleInfo.currentPlayerHealth += healthIncrease;

            // Ensure the new health doesn't exceed the basePlayerHealth.
            if (BattleInfo.currentPlayerHealth > BattleValues.basePlayerHealth)
            {
                BattleInfo.currentPlayerHealth = BattleValues.basePlayerHealth;
            }

            // Disable further health increases.
            canIncreaseHealth = false;
        }
        
        // Only prevent use if successfully gained hp.
        if (!canIncreaseHealth)
        {
            // Prevent re-use.
            textComp.enabled = false;
            gameObject.layer = 0;
            Destroy(this);
        }   
    }

    // Called once before first update.
    void Start()
    {
        // Find interact text comp.
        textComp = GameObject.Find("UICanvas").transform.Find("HealthInteract").
            GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame.
    void Update()
    {
        // Only check if text enabled.
        if (textComp.enabled) { CheckForClick(); }
    }
}
