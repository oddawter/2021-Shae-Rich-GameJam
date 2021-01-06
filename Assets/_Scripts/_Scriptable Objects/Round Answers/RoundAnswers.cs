using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundAnswers",
	menuName = "ScriptableObjects/RoundAnswers")]
public class RoundAnswers : ScriptableObject
{
    [SerializeField]
    private List<string> field1Answers = new List<string>();
    public List<string> Field1Answers { get => field1Answers; }//readonly serialized field pattern

    [SerializeField]
    private List<string> field2Answers = new List<string>();
    public List<string> Field2Answers { get => field2Answers; }//readonly serialized field pattern

    [SerializeField]
    private List<string> field3Answers = new List<string>();
    public List<string> Field3Answers { get => field3Answers; }//readonly serialized field pattern
}
