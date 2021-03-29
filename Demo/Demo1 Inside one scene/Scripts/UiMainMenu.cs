using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMainMenu : UiBehaviour
{
    public override UiTabGroup TabGroup => UiTabGroup.BigGameScreens;

    public void OnClick_Play() {
        Ui.Show<UiInGame>();
    }

    public void OnClick_Options() {
        Ui.Show<UiOptions>();
    }
}
