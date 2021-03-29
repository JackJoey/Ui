using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestion : UiBehaviour
{
    public override UiTabGroup TabGroup => UiTabGroup.DialogsLayer1;

    public Text txtQuestion;

    Action onYes = null;
    Action onNo = null;


    public void Show(string question, Action onYes, Action onNo) {
        txtQuestion.text = question;
        this.onYes = onYes;
        this.onNo = onNo;

        Ui.Show<UiQuestion>();
    }

    public void OnClick_Yes() {
        onYes?.Invoke();

        Ui.HideCurrentIn(this.TabGroup);
    }

    public void OnClick_No() {
        onNo?.Invoke();

        Ui.HideCurrentIn(this.TabGroup);
    }
}
