using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---Game Manager---")]
    [SerializeField]
    private RoundData[] rounds;

    [SerializeField]
    private int roundIndex = 0;

    private RoundData CurrentRoundData { get => rounds[roundIndex]; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartNewRound(int newRoundIndex)
    {
        roundIndex = newRoundIndex; //catch arg

        var data = CurrentRoundData;//cache calculation
        //TODO load data from round and set in scene
    }
}
