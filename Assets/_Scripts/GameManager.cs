using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private RoundData CurrentRoundData { get => rounds[roundIndex]; }

    [Header("---Game Manager---")]

    /// <summary>
    /// parent new rounds to this object
    /// </summary>
    [Header("---Round Info---")]
    [SerializeField]
    private Transform roundObjectsHandle;

    [SerializeField]
    private FloatVariable roundTimer;

    [SerializeField]
    private RoundData[] rounds;

    [SerializeField]
    private int roundIndex = 0;

    [Header("---Scene Management---")]
    [SerializeField]
    private int currentSceneIndex = 0;

    [SerializeField]
    private int mainMenuSceneIndex = 0;

    [SerializeField]
    private int gameSceneIndex = 1;

    private const int SCENE_COUNT = 2;


    //private runtime data
    private GameObject currentRoundInstance;
    
    //coroutines
    private Coroutine roundTimerRoutine;
    
    void Awake()
    {
        InitSingleton(this);
    }

    // Update is called once per frame
    void Update()
    {
        //get inputs
        var pressed_enter =
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter);

        //handle inputs
        if (pressed_enter)
        {
            if (CheckEntries())
            {
                //TODO YOU WIN!
                //on to the next level
                StartNewRound(roundIndex + 1);//load next round assets
            }
            else
            {
                //TODO 1 or more is incorrect
            }
        }
    }

    private static void InitSingleton(GameManager instance)
    {
        if (!Instance)
        {   //all good
            Instance = instance;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {   //singleton exception
            Debug.LogError("[GameManager] singleton error");
            instance.gameObject.SetActive(false);
        }
    }

    private bool CheckEntries()
    {
        var entriesAreAccepted = false; //pessimistic

        //for all input fields, compare with data answers
        //track which 

        return entriesAreAccepted;
    }

    private void StartNewRound(int newRoundIndex)
    {
        roundIndex = newRoundIndex; //catch arg

        var data = CurrentRoundData;//cache calculation
        //TODO load data from round and set in scene

        //instantiate objects for the round
        currentRoundInstance = Instantiate(data.RoundPrefab, roundObjectsHandle);
        var roundObjectsTransform = currentRoundInstance.GetComponent<Transform>();
        roundObjectsTransform.localPosition = Vector3.zero;//reset position
        roundObjectsTransform.localRotation = Quaternion.identity;//reset rotation

        //play music

        //set sounds

        //set timer
        roundTimerRoutine = StartCoroutine(CountRoundTimer(data.RoundTime));

        //play animations
    }

    private void OnRoundTimerElapsed()
    {
        //TODO explode bomb
        //game over
        //restart after time
        //etc
    }

    private IEnumerator CountRoundTimer(float roundLength)
    {
        yield return new WaitForSecondsRealtime(roundLength);

        OnRoundTimerElapsed();
    }

    public void GoToScene(int newSceneIndex)
    {
        if(newSceneIndex < 0 || newSceneIndex >= SCENE_COUNT)
        {
            Debug.LogError("FFFFFFUUUUUUCCCCKKKKK scene index out of array bounds! " +
                newSceneIndex + " / " + SCENE_COUNT);
        }
        else
        {
            currentSceneIndex = newSceneIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}
