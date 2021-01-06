using UnityEngine;

[CreateAssetMenu(fileName = "RoundData",
	menuName = "ScriptableObjects/RoundData")]
public class RoundData : ScriptableObject
{
    [SerializeField]
    private RoundAnswers answers;

    public RoundAnswers Answers { get => answers; } //readonly serialized field pattern

    [SerializeField]
    private string[] hints;

    public string[] Hints { get => hints; }//readonly serialized field pattern

    [SerializeField]
    private float roundTime = 60f;
    public float RoundTime { get => roundTime; }//readonly serialized field pattern

}
