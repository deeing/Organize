using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeContent : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nodeText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Color successColor;

    public int nodeOrder { get; private set; }

    private NodeContentManager manager;

    public void Init(string text, int order, NodeContentManager manager)
    {
        nodeText.text = text;
        nodeOrder = order;
        this.manager = manager;
    }

    public void CheckAnswer()
    {
        manager.CheckAnswer();
    }

    public void End()
    {
        image.color = successColor;
    }
}
