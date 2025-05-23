using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting) return null;

            lock (_lock)
            {
                if(_instance == null)
                {
                    // 씬에서 이미 존재하는 오브젝트 검색
                    _instance = (T)FindObjectOfType(typeof(T));

                    if(_instance == null)
                    {
                        // 새 gameobject 생성 후 컴포넌트 부착
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 중복 방지
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 중복 시 삭제
        }
    }

    /// <summary>
    /// 앱 종료 시 인스턴스 재생성 방지
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}
