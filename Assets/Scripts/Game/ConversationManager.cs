using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class ConversationManager : MonoBehaviour
{
    public static ConversationManager Instance { get; private set; }

    [SerializeField] GameObject conversation;
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Image leftPersonImage;
    [SerializeField] Image rightPersonImage;
    [SerializeField] Sprite emptySprite;

    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private Dialog dialog;

    private Character leftPerson;
    private Character rightPerson;

    public bool IsShowing { get; private set; }
    public bool ShowNext { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator StartConversation(Dialog dialog, Character leftPerson = null, Character rightPerson = null,
        List<string> choices = null, Action<int> onChoiceSelected = null, float autoSpeed = 0.0f, bool anotherCondition = false)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        this.leftPerson = leftPerson;
        this.rightPerson = rightPerson; 

        if (leftPerson == null)
            leftPersonImage.sprite = emptySprite;
        else
            leftPersonImage.sprite = leftPerson.Sprites[(int)SpriteState.Idle];

        if (rightPerson == null)
            rightPersonImage.sprite = emptySprite;
        else
            rightPersonImage.sprite = rightPerson.Sprites[(int)SpriteState.Idle];

        IsShowing = true;
        this.dialog = dialog;

        conversation.SetActive(true);

        for (int i = 0; i < dialog.Lines.Count; i++)
        {
            yield return TypeDialog(dialog.Lines[i], i);

            if (autoSpeed > 0) yield return new WaitForSeconds(autoSpeed);
            else if (anotherCondition)
            {
                yield return new WaitUntil(() => ShowNext == true);
                ShowNext = false;
            }
            else
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        conversation.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();
    }

    public IEnumerator EndConversation()
    {
        conversation.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();

        yield return null;
    }

    public void HandleUpdate()
    {
    }

    public IEnumerator TypeDialog(string line, int currentLine)
    {
        for (int i = 0; i < dialog.SwitchingDatas.Count; i++)
        {
             if (currentLine == dialog.SwitchingDatas[i].switchingLineNum)
            {
                if (leftPerson != null && leftPerson.Sprites[(int)dialog.SwitchingDatas[i].leftState] != null)
                    leftPersonImage.sprite = leftPerson.Sprites[(int)dialog.SwitchingDatas[i].leftState];
                if (rightPerson != null && rightPerson.Sprites[(int)dialog.SwitchingDatas[i].rightState] != null)
                    rightPersonImage.sprite = rightPerson.Sprites[(int)dialog.SwitchingDatas[i].rightState];
            }
        }

        dialogText.text = "";

        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void ChangeLeftPersonImage(Sprite sprite)
    {
        leftPersonImage.sprite = sprite;
    }

    public void ChangeRightPersonImage(Sprite sprite)
    {
        rightPersonImage.sprite = sprite;
    }
}
