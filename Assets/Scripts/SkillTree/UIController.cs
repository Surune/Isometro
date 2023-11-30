using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public SkillTree skillTree;
    public PlayerProgress playerProgress;
    public TextMeshProUGUI skillTreeText;
    public TextMeshProUGUI playerProgressText;

    public void DisplaySkillTree()
    {
        skillTreeText.text = "Skill Tree:\n";
        foreach (var node in skillTree.GetUnlockedNodes())
        {
            skillTreeText.text += node.SkillName + " (Unlocked)\n";
        }
    }

    public void DisplayPlayerProgress()
    {
        playerProgressText.text = "Player Progress:\n";
        foreach (var skillName in playerProgress.playerProgress)
        {
            playerProgressText.text += skillName + "\n";
        }
    }
}
