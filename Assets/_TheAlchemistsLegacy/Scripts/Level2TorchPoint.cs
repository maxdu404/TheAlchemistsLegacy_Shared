using UnityEngine;

public class Level2TorchPoint : MonoBehaviour
{
    [Header("Sequence")]
    [SerializeField] private int sequenceIndex;

    [Header("Lit Visuals")]
    [SerializeField] private GameObject pointLightObject;
    [SerializeField] private GameObject fireEffectObject;
    [SerializeField] private bool litAtStart;

    public int SequenceIndex => sequenceIndex;
    public bool IsLit { get; private set; }

    private void Reset()
    {
        AutoFindVisuals();
    }

    private void Awake()
    {
        AutoFindVisuals();
        SetLit(litAtStart);
    }

    public void SetLit(bool isLit)
    {
        IsLit = isLit;

        if (pointLightObject != null)
        {
            pointLightObject.SetActive(isLit);
        }

        if (fireEffectObject != null)
        {
            fireEffectObject.SetActive(isLit);
        }
    }

    private void AutoFindVisuals()
    {
        if (pointLightObject != null && fireEffectObject != null)
        {
            return;
        }

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            string normalizedName = NormalizeName(child.name);

            if (pointLightObject == null && normalizedName.Contains("pointlight"))
            {
                pointLightObject = child.gameObject;
            }

            if (fireEffectObject == null && normalizedName.Contains("fireeffect"))
            {
                fireEffectObject = child.gameObject;
            }
        }
    }

    private string NormalizeName(string value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.ToLowerInvariant().Replace(" ", string.Empty).Replace("_", string.Empty);
    }
}
