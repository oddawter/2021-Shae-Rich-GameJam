using UnityEngine;
using TMPro;

public class TextHintController : MonoBehaviour
{
    [Header("---Resources---")]
    [SerializeField]
    private TextMeshProUGUI hintTMP;

    public string HintText { get => hintTMP.text; set => hintTMP.text = value; }
}
