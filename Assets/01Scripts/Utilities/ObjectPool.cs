using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Func<T> _createFunc;
    private readonly Stack<T> _pool;

    public ObjectPool(Func<T> createFunc, int initialSize = 0)
    {
        if (createFunc == null)
            throw new ArgumentNullException(nameof(createFunc));

        _createFunc = createFunc;
        _pool = new Stack<T>();

        for (int i = 0; i < initialSize; i++)
        {
            _pool.Push(_createFunc());
            _pool.Peek().gameObject.SetActive(false);
        }
    }

    public T Get()
    {
        T returning = _pool.Count > 0 ? _pool.Pop() : _createFunc();
        returning.gameObject.SetActive(true);

        return returning;
    }

    public void Return(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        obj.transform.DOKill();
        obj.gameObject.SendMessage("ResetObject", options: SendMessageOptions.DontRequireReceiver);
        obj.gameObject.SetActive(false);
        _pool.Push(obj);
    }

    public int Count => _pool.Count;
}
