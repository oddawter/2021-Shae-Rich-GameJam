using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class InputFieldController : MonoBehaviour
{
    [Header("---Resources---")]
    [SerializeField]
    private TMP_InputField fieldOne;

    [SerializeField]
    private RectTransform fieldOneRect;

    public string FieldOneText { get => fieldOne.text.ToLower(); }

    [SerializeField]
    private TMP_InputField fieldTwo;
    public string FieldTwoText { get => fieldTwo.text.ToLower(); }
    
    [SerializeField]
    private RectTransform fieldTwoRect;

    [SerializeField]
    private TMP_InputField fieldThree;

    [SerializeField]
    private RectTransform fieldThreeRect;

    public string FieldThreeText { get => fieldThree.text.ToLower(); }

    [Header("---Animation---")]
    [SerializeField]
    private float shakeWrongDuration = 0.8f;

    [SerializeField]
    private float shakeStrength = 5;

    [SerializeField]
    private float shakeCorrectDuration = 1.0f;

    [SerializeField]
    private float jumpStr = 10;

    [SerializeField]
    private float jumpHeight = 25;

    [SerializeField]
    private int jumpCount = 1;

    //runtime values
    private Vector3 cachedField1Position;
    private Vector3 cachedField2Position;
    private Vector3 cachedField3Position;

    /// <summary>
    /// Extract input text from each field.
    /// </summary>
    public string[] Answers { get => new string[]
    {fieldOne.text.ToLower(), fieldTwo.text.ToLower(), fieldThree.text.ToLower()};}

    private void Start()
    {
        cachedField1Position = fieldOneRect.localPosition;
        cachedField2Position = fieldTwoRect.localPosition;
        cachedField3Position = fieldThreeRect.localPosition;
    }

    private void JumpField(RectTransform fieldRect)
    {
        var jumpPos = new Vector3(fieldRect.localPosition.x,
            fieldRect.localPosition.y + jumpHeight,
            fieldRect.localPosition.z
            );

        fieldRect.DOLocalJump(
            jumpPos, jumpStr, jumpCount, shakeCorrectDuration);
    }

    public void JumpField(int fieldNum)
    {
        if (fieldNum == 1 && fieldOne.interactable)
        {
            JumpField(fieldOneRect);
            fieldOne.DeactivateInputField();
            fieldOne.interactable = false;

        }
        else if (fieldNum == 2 && fieldTwo.interactable)
        {
            JumpField(fieldTwoRect);
            fieldTwo.DeactivateInputField();
            fieldTwo.interactable = false;

        }
        else if (fieldNum == 3 && fieldThree.interactable)
        {
            JumpField(fieldThreeRect);
            fieldThree.DeactivateInputField();
            fieldThree.interactable = false;

        }
        else
        {
            //Debug.LogError("dangit.");
        }
    }

    private void ShakeField(RectTransform field)
    {
        field.DOShakePosition(shakeWrongDuration, shakeStrength);
    }

    public void ShakeField(int fieldNum)
    {
        if(fieldNum == 1)
        {
            ShakeField(fieldOneRect);

        }
        else if (fieldNum == 2)
        {
            ShakeField(fieldTwoRect);

        }
        else if(fieldNum == 3)
        {
            ShakeField(fieldThreeRect);

        }
        else
        {
            Debug.LogError("dangit.");
        }
    }

    public void ResetGame()
    {
        fieldOneRect.DOLocalMove(cachedField1Position, 0.5f);
        fieldTwoRect.DOLocalMove(cachedField2Position, 0.5f);
        fieldThreeRect.DOLocalMove(cachedField3Position, 0.5f);

        fieldOne.interactable = true;
        fieldTwo.interactable = true;
        fieldThree.interactable = true;

        fieldOne.text = null;
        fieldTwo.text = null;
        fieldThree.text = null;
    }
}
