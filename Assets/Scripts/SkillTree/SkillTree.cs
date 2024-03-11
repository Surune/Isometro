using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public GameObject skillNodePrefab;
    public Transform nodeContainer;

    private Dictionary<string, SkillNode> nodes;

    void Start()
    {
        nodes = new Dictionary<string, SkillNode>();
        GenerateSkillTree();
    }

    void GenerateSkillTree()
    {
        string[] skills = { "Skill1", "Skill2", "Skill3", "Skill4", "Skill5", "Skill6", "Skill7" };

        foreach (string skill in skills)
        {
            GameObject nodeGO = Instantiate(skillNodePrefab, nodeContainer);
            SkillNode node = nodeGO.GetComponent<SkillNode>();
            node.Initialize(skill);
            nodes[skill] = node;
        }

        // Randomly connect nodes
        System.Random rand = new System.Random();
        foreach (var node in nodes.Values)
        {
            foreach (var otherNode in nodes.Values)
            {
                if (node != otherNode && rand.NextDouble() < 0.3) // Adjust the probability of connection
                {
                    node.AddConnection(otherNode);
                }
            }
        }
    }

    public void UnlockNode(string skillName)
    {
        nodes[skillName].Unlock();
    }

    public List<SkillNode> GetUnlockedNodes()
    {
        List<SkillNode> unlockedNodes = new List<SkillNode>();
        foreach (var node in nodes.Values)
        {
            if (node.Unlocked)
            {
                unlockedNodes.Add(node);
            }
        }
        return unlockedNodes;
    }
}