using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiBehaviour : MonoBehaviour
{
    public abstract UiTabGroup TabGroup { get; }

    [System.NonSerialized]
    public bool isInited = false;

    // caching the gameObject
    private GameObject _gameObject = null;
    new public GameObject gameObject => _gameObject != null ? _gameObject : _gameObject = base.gameObject;

    // caching the transform
    private Transform _transform = null;
    new public Transform transform => _transform != null ? _transform : _transform = base.transform;

    public virtual void InitUi() {
        isInited = true;
    }

    public virtual void RequestShow(Action onFinish) {
        this.gameObject.SetActive(true);
        onFinish?.Invoke();
    }

    public virtual void RequestHide(Action onFinish) {
        this.gameObject.SetActive(false);
        onFinish?.Invoke();
    }

    public virtual void ForceShow() {

    }

    public virtual void ForceHide() {

    }

    // todo think about it or 
    public virtual void OnShowBegin() { }
    public virtual void OnShowEnd() { }
    public virtual void OnHideBegin() { }
    public virtual void OnHideEnd() { }
}
