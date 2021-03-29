using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiOptions : UiBehaviour {
    public override UiTabGroup TabGroup => UiTabGroup.BigGameScreens;

    public void OnClick_X() {
        Ui.Show<UiMainMenu>();
    }
}
