using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    public string SkillName { get; private set; }
    public bool Unlocked { get; private set; }
    public List<SkillNode> Connections { get; private set; }
    [SerializeField] private Image image;

    public void Initialize(string skillName)
    {
        SkillName = skillName;
        Unlocked = false;
        Connections = new List<SkillNode>();
    }

    public void Unlock()
    {
        Unlocked = true;
        image.color = Color.green; // Change color when unlocked
    }

    public void AddConnection(SkillNode node)
    {
        Connections.Add(node);
    }
}
