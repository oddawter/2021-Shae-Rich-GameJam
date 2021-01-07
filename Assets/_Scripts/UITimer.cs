using System.Text;
using UnityEngine;
using ScriptableObjectArchitecture;
using TMPro;

public class UITimer : MonoBehaviour
{

    private const string COLON = ":";
    [Header("---Resources---")]
    [SerializeField]
    private TextMeshProUGUI TMPText;

    [SerializeField]
    private FloatReference timeValue;

    private StringBuilder myStringBuilder = new StringBuilder(4);

    private void OnEnable()
    {
        timeValue.AddListener(UpdateUI);
    }
    
    private void OnDisable()
    {
        timeValue.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        //format for time
        myStringBuilder.Clear();
        myStringBuilder.Append((int)(timeValue.Value / 60));//minutes
        myStringBuilder.Append(COLON);
        myStringBuilder.Append((int)(timeValue.Value % 60));//seconds

        TMPText.text = myStringBuilder.ToString();
    }


}
