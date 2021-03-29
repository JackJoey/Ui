using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInGame : UiBehaviour
{
    public override UiTabGroup TabGroup => UiTabGroup.BigGameScreens;

    public void OnClick_X() {
        Ui.Get<UiQuestion>().Show("Are you sure?",
            () => {
                Ui.Show<UiMainMenu>();
            }, null
        );
    }
}
