using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class NodeDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject placeholderPrefab;
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private float switchThreshold = 0.1f;
    [SerializeField]
    private NodeContent nodeContent;

    public bool isCoolingDown { get; private set; } = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalSiblingIndex;
    private GameObject placeholder;
    private WaitForSeconds switchCooldown;

    void Awake()
    {
        switchCooldown = new WaitForSeconds(slideDuration);
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        placeholder = Instantiate(placeholderPrefab, originalParent);
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);

        transform.SetParent(originalParent.parent);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

        List<Transform> siblings = GetSiblingList();
        int newSiblingIndex = GetNewSiblingIndex(siblings);

        if (newSiblingIndex != originalSiblingIndex && newSiblingIndex >= 0 && newSiblingIndex < siblings.Count)
        {
            UpdateSiblingIndexAndPlaceholder(newSiblingIndex);
            //SlideNodesSmoothly(siblings);
        }
    }

    private List<Transform> GetSiblingList()
    {
        List<Transform> siblings = new List<Transform>();
        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i) != placeholder.transform)
            {
                siblings.Add(originalParent.GetChild(i));
            }
        }
        siblings.Insert(placeholder.transform.GetSiblingIndex(), placeholder.transform);
        return siblings;
    }

    private int GetNewSiblingIndex(List<Transform> siblings)
    {
        int newSiblingIndex = originalSiblingIndex;
        for (int i = 0; i < siblings.Count; i++)
        {
            if (i != originalSiblingIndex && siblings[i] != placeholder.transform)
            {
                float currentDist = Vector3.Distance(rectTransform.position, siblings[i].position);
                float newSiblingDist = Vector3.Distance(rectTransform.position, siblings[newSiblingIndex].position);
                NodeDragHandler siblingNode = siblings[i].GetComponent<NodeDragHandler>();

                if (currentDist < newSiblingDist * (1 - switchThreshold) && !siblingNode.isCoolingDown)
                {
                    newSiblingIndex = i;
                    siblingNode.StartCooldown();
                }
            }
        }
        return newSiblingIndex;
    }

    public void StartCooldown()
    {
        StartCoroutine(CoolDownFromSwitching());
    }

    private IEnumerator CoolDownFromSwitching()
    {
        isCoolingDown = true;
        yield return switchCooldown;
        isCoolingDown = false;
    }

    private void UpdateSiblingIndexAndPlaceholder(int newSiblingIndex)
    {
        originalSiblingIndex = newSiblingIndex;
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    private void SlideNodesSmoothly(List<Transform> siblings)
    {
        for (int i = 0; i < siblings.Count; i++)
        {
            if (siblings[i] != placeholder.transform)
            {
                siblings[i].DOLocalMove(originalParent.GetChild(i).localPosition, slideDuration);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        canvasGroup.blocksRaycasts = true;
        DestroyImmediate(placeholder);

        nodeContent.CheckAnswer();
        /*rectTransform.DOLocalMove(placeholder.transform.localPosition, slideDuration).OnComplete(() => {
            Destroy(placeholder);
        });*/
    }
}
