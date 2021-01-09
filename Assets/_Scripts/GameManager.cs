using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject explosion;

    private Transform canvas;

    private RoundData CurrentRoundData { get => rounds[roundIndex]; }

    [Header("---Game Manager---")]
    [SerializeField]
    private InputFieldController inputFields;

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

    private int NextIndex { get => (int)Mathf.Repeat(roundIndex + 1, rounds.Length); }

    [Header("---Audio---")]
    [SerializeField]
    private AudioClip explosionSound;

    //private runtime data
    private GameObject currentRoundInstance;
    
    //coroutines
    private Coroutine roundTimerRoutine;
    
    void Awake()
    {
        InitSingleton(this);
    }

    private void Start()
    {
        StartNewRound(0);

        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;

        if (!AudioManager.IsInitialized)
            AudioManager.Init();
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
                var seq = DOTween.Sequence();

                seq.AppendInterval(3)//wait some seconds
                    .AppendCallback(//then call the following function:
                        () => StartNewRound(NextIndex));//load next round assets
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

    private static bool ListContainsString(List<string> acceptedAnswers, 
        string playerAnswer)
    {
        //remove leading and following whitespace
        playerAnswer.Trim();

        var accepted = false;
        for(var i = 0; i < acceptedAnswers.Count; ++i)
        {
            var ans = acceptedAnswers[i];
            if(ans == playerAnswer)
            {
                accepted = true;
                break;
            }
        }
        return accepted;
    }

    private bool CheckEntries()
    {
        var entriesAreAccepted = true; //optimistic
        var roundData = CurrentRoundData;
        var answers = roundData.Answers;
        //for all input fields, compare with data answers
        
        //check field 1
        if (ListContainsString(answers.Field1Answers, inputFields.FieldOneText))
        {   //correct answer in field 1
            inputFields.JumpField(1);
        }
        else
        {
            inputFields.ShakeField(1);
            entriesAreAccepted = false;
        }

        //check field 2
        if (ListContainsString(answers.Field2Answers, inputFields.FieldTwoText))
        {   //correct answer in field 2
            inputFields.JumpField(2);
        }
        else
        {
            inputFields.ShakeField(2);
            entriesAreAccepted = false;
        }

        //check field 3
        if (ListContainsString(answers.Field3Answers, inputFields.FieldThreeText))
        {   //correct answer in field 3
            inputFields.JumpField(3);
        }
        else
        {
            inputFields.ShakeField(3);
            entriesAreAccepted = false;
        }

        return entriesAreAccepted;
    }

    private void StartNewRound(int newRoundIndex)
    {
        roundIndex = newRoundIndex; //catch arg

        var data = CurrentRoundData;//cache calculation
                                    //TODO load data from round and set in scene

        if (currentRoundInstance != null)
            Destroy(currentRoundInstance);

        if (data.RoundPrefab)
        {
            //instantiate objects for the round
            currentRoundInstance = Instantiate(data.RoundPrefab, roundObjectsHandle);
            var roundObjectsTransform = currentRoundInstance.GetComponent<Transform>();
            roundObjectsTransform.localPosition = Vector3.zero;//reset position
            roundObjectsTransform.localRotation = Quaternion.identity;//reset rotation

        }
        else
        {
            Debug.Log("No prefab for round " + data.name + " yet");
        }

        //play music

        //set sounds

        //set timer
        roundTimer.Value = data.RoundTime;
        roundTimerRoutine = StartCoroutine(CountRoundTimer(data.RoundTime));

        //play animations
        inputFields.ResetGame();
    }

    private void OnRoundTimerElapsed()
    {
        //explosion effect
        AudioManager.PlaySFX(explosionSound, pitchShift: false);
        Instantiate(explosion, canvas);

        //stop timer
        if(roundTimerRoutine != null)
            StopCoroutine(roundTimerRoutine);
        roundTimer.Value = 0;
    }

    private IEnumerator CountRoundTimer(float roundLength)
    {
        var finishTime = roundLength + Time.time;
        while (Time.time < finishTime)
        {
            var previousTIme = Time.time;
            yield return null;
            roundTimer.Value -= Time.time - previousTIme;
        }

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
