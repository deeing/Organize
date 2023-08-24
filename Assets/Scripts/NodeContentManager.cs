using System.Collections.Generic;
using UnityEngine;

public class NodeContentManager : MonoBehaviour
{
    [SerializeField]
    private FileReader fileReader;
    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private Transform container;

    private void Awake()
    {
        fileReader.Read(Init);
    }

    public void Init()
    {
        // by this point the file should be loaded
        string[] lines = fileReader.fileContents.Split("\n");
        List<GameObject> nodeList = new();

        for(int i=0; i < lines.Length; i++)
        {
            string line = lines[i];
            GameObject newNode = Instantiate(nodePrefab);
            NodeContent nodeContent = newNode.GetComponent<NodeContent>();
            nodeContent.Init(line, i, this);
            nodeList.Add(newNode);
        }

        // randomize nodes
        nodeList = ShuffleList(nodeList);

        // add nodes to list in random order
        foreach(GameObject node in nodeList)
        {
            node.transform.SetParent(container);
        }
    }

    public void CheckAnswer()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Transform child = container.GetChild(i);
            NodeContent childContent = child.GetComponent<NodeContent>();

            if (childContent.nodeOrder != i)
            {
                // order is not correct
                return;
            }
        }

        Success();
    }

    private void Success()
    {
        foreach(Transform child in container)
        {
            NodeDragHandler nodeDrag = child.GetComponent<NodeDragHandler>();
            NodeContent content = child.GetComponent<NodeContent>();

            nodeDrag.enabled = false;
            content.End();
        }
    }

    List<T> ShuffleList<T>(List<T> originalList)
    {
        List<T> shuffledList = new List<T>(originalList);
        System.Random random = new System.Random();

        for (int i = shuffledList.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = shuffledList[i];
            shuffledList[i] = shuffledList[j];
            shuffledList[j] = temp;
        }

        // Ensure that the shuffled list is not in the same order as the original list
        bool areListsEqual = true;
        for (int i = 0; i < shuffledList.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(shuffledList[i], originalList[i]))
            {
                areListsEqual = false;
                break;
            }
        }

        if (areListsEqual)
        {
            // Swap the first two elements
            if (shuffledList.Count > 1)
            {
                T temp = shuffledList[0];
                shuffledList[0] = shuffledList[1];
                shuffledList[1] = temp;
            }
        }

        return shuffledList;
    }
}
