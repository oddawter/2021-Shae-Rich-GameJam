using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldController : MonoBehaviour
{
    [Header("---Resources---")]
    [SerializeField]
    private TMP_InputField fieldOne;

    [SerializeField]
    private TMP_InputField fieldTwo;

    [SerializeField]
    private TMP_InputField fieldThree;
    
    /// <summary>
    /// Extract input text from each field.
    /// </summary>
    public string[] Answers { get => new string[]
    {fieldOne.text, fieldTwo.text, fieldThree.text};}

    private void Start()
    {
    }
}
