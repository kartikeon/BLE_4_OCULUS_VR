using System.Collections;
using UnityEngine;
using UniRx;
using System; // .NET 4.x Required if running in mode

// Implemented as a singleton class
public sealed class FlagStream : MonoBehaviour
{
    private static FlagStream _singleInstance = new FlagStream();

    public static FlagStream GetInstance()
    {
        return _singleInstance;
    }

    // constructor
    private FlagStream() { }

    private void Start() 
    {
        // Subject is disposed at the timing of OnDestroy
        scanFlagSubject.AddTo(this);
        connectFlagSubject.AddTo(this);
    }

    // the instance that emits the event
    private Subject<bool> scanFlagSubject = new Subject<bool>();
    private Subject<bool> connectFlagSubject = new Subject<bool>();

    // Publish event subscribers
    public IObservable<bool> OnScanFlagChanged
    {
        get { return scanFlagSubject; }
    }
    public IObservable<bool> OnConnectFlagChanged
    {
        get { return connectFlagSubject; }
    }

    // publish event
    public void SetScanFlag(bool value) 
    {
        scanFlagSubject.OnNext(value);
    }
    public void SetConnectFlag(bool value) 
    {
        connectFlagSubject.OnNext(value);
    }
}