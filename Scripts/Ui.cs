using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UiTabGroup {
    BigGameScreens,
    DialogsLayer1,
    DialogsLayer2,
}

/*
 требования:
    1) окна лежат внутри канваса и не являются префабами 
        - для простоты работы с ними и свободы их визуальной настройки 
    2) у каждого окна должен быть свой класс потомок от абстрактного окна 
        - такие классы полезны они управляют его визуальными елементами
        - окно может содержать прямые public\SerializeField ссылки на собственные визуальные елементы
        это дает свободу "конструирования" и избавляет от грубых инициализаций в старте
    3) организовать работу с окном без прямой ссылки на это окно
        - ссылку на окно нужно доставать из менаджера окон
        прямые ссылки на окно не являются плохим стилем сами по себе
        плохо то что их может быть много в разных местах проекта и что они 
        искушают ломать логику переключения окон 
    4) менаджер окон учавствует в переключении окон.
        - менаджер - по сути машина состояний - он ждет пока спрячется старое и затем запускает новое
        особо полезно когда окна исчезают и появляются с анимацией
        однако сдесь нужно еще подумать как это лучше организорвать.
            Нужно ответить на вопросы:
            1) кто должен следить за анимацией само окно или его менаджер?
                а) окна могут иметь разные принципы анимации полезно чтоб окно это делало - но это может много чего усложнить
                б) можно упростить окно и вынести это в менаджер - это может уменьшить возможности окон - шаблонизация

*/


// 1) возможно стоит использовать другие имена - название окно мне нравится но слово менаджер мне не нравится
public static class Ui {
    private static bool isInited = false;
    private static string initedSceneName = null;
    private static Dictionary<UiTabGroup, TabGroup> tabGroups = null;
    private static Dictionary<Type, UiBehaviour> allUis = null; // idea: use the name of Type not Type

    private static void CheckAndInit() {
        if (!isInited) {
            //create TabGroups
            tabGroups = new Dictionary<UiTabGroup, TabGroup>();

            // create the enum vals
            Array enumValues = Enum.GetValues(typeof(UiTabGroup));
            int groupsCount = enumValues.Length;

            // fill groups by default
            foreach (UiTabGroup currVal in enumValues) {
                var group = new TabGroup();
                group.id = currVal;
                group.currUi = null;
                group.posibleUis = null;
                tabGroups.Add(currVal, group);
            }
            isInited = true;
        }

        string currSceneName = SceneManager.GetActiveScene().name; // todo paralel scenes support and DontDestoroyOnLoad windows

        if (initedSceneName != currSceneName) {
            ReInitUis();
            initedSceneName = currSceneName;
        }
    }

    private static void ReInitUis() {
        // todo prevent adding copies of ui

        allUis = new Dictionary<Type, UiBehaviour>();

        UiBehaviour[] findedUis = Resources.FindObjectsOfTypeAll<UiBehaviour>();

        Debug.Log($"findedUis.Length {findedUis.Length}");

        // clear posible of groups for writing it by new values (old DontDestoroyable will be re added)
        foreach (var kvp in tabGroups) {
            TabGroup tabGroup = kvp.Value;
            tabGroup.posibleUis = new List<UiBehaviour>();
        }

        foreach (UiBehaviour ui in findedUis) {
            UiTabGroup groupId = ui.TabGroup;
            TabGroup group = tabGroups[groupId];
            group.posibleUis.Add(ui);

            allUis.Add(ui.GetType(), ui);
        }

        foreach (UiBehaviour ui in findedUis) {
            if (!ui.isInited) {
                ui.gameObject.SetActive(false);
                ui.InitUi();
            }
        }
    }


    public static T Get<T>() where T : UiBehaviour  {
        CheckAndInit();

        allUis.TryGetValue(typeof(T), out UiBehaviour result);

        return (T)result;
    }

    // other name candidates ToggleOn, Show, SwitchTo
    public static void Show<T>() where T : UiBehaviour {
        CheckAndInit();

        UiBehaviour newUi = Get<T>();

        TabGroup group = tabGroups[newUi.TabGroup];

        group.SwitchTo(newUi);
    }

    public static void HideCurrentIn(UiTabGroup groupId) {
        CheckAndInit();

        TabGroup group = tabGroups[groupId];

        group.SwitchTo(null);
    }


    #region TabGroup

    // ideas:
    // use TabGroups like enum UiTabGroup
    // write custom TabGroups for controlling by switching animation
    // use internal TabGroups inside TabGroups
    private class TabGroup {
        public enum State {
            Idle, Switch
        }

        public UiTabGroup id;
        public State state;
        public UiBehaviour currUi = null;
        public UiBehaviour prevUi = null;
        public List<UiBehaviour> posibleUis = null;

        public void SwitchTo(UiBehaviour nextUi) {
            if (state != State.Switch) {

                state = State.Switch;

                if (currUi != null) {
                    currUi.RequestHide(() => { ShowNextUi(nextUi); });
                } else {
                    ShowNextUi(nextUi);
                }

            } else {
                Debug.LogError($"You try switch to ui '{nextUi.GetType().ToString()}' but group '{id.ToString()}' is alredy in switching state.");
            }
        }

        private void ShowNextUi(UiBehaviour nextUi) {
            prevUi = currUi;
            currUi = nextUi;
            if (nextUi != null) {
                nextUi.RequestShow(EndOfSwitching);
            } else {
                EndOfSwitching();
            }
        }

        private void EndOfSwitching() {
            state = State.Idle;
        }
    }

    #endregion TabGroup

}
