using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class GameManager : MonoBehaviour
{
    [Header("---Game Manager---")]
    [SerializeField]
    private RoundData[] rounds;

    [SerializeField]
    private int roundIndex = 0;

    private RoundData CurrentRoundData { get => rounds[roundIndex]; }

    /// <summary>
    /// parent new rounds to this object
    /// </summary>
    [Header("---Round Info---")]
    [SerializeField]
    private Transform roundObjectsHandle;

    [SerializeField]
    private FloatVariable roundTimer;

    [Header("---Scene Management---")]
    [SerializeField]
    private int currentSceneIndex = 0;

    [SerializeField]
    private int mainMenuSceneIndex = 0;

    [SerializeField]
    private int gameSceneIndex = 1;

    private const int SCENE_COUNT = 2;

    public static GameManager Instance { get; private set; }

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
