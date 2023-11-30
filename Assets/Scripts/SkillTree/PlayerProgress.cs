using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerProgress : MonoBehaviour
{
    public SkillTree skillTree;
    public TextMeshProUGUI playerProgressText;

    public List<string> playerProgress = new List<string>();

    public void UnlockSkill(string skillName)
    {
        playerProgress.Add(skillName);
        skillTree.UnlockNode(skillName);
        UpdatePlayerProgressText();
    }

    void UpdatePlayerProgressText()
    {
        playerProgressText.text = "Player Progress:\n";
        foreach (string skillName in playerProgress)
        {
            playerProgressText.text += skillName + "\n";
        }
    }
}
