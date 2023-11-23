using System; // Required if running in .NET 4.x mode
using System.Collections;
using UnityEngine;
using UniRx;

// Implemented in singleton class
public class NotifyValueStream : MonoBehaviour
{
    private static NotifyValueStream _singleInstance = new NotifyValueStream();

    public static NotifyValueStream GetInstance() 
    { return _singleInstance; }

    private NotifyValueStream() {}

    // the instance that emits the event
    private Subject<string> notifyValueSubject = new Subject<string>();
   

    // Used when intentionally discarding
    public static IDisposable iDisposable;

    void Start()
    {
        // Subject is disposed at the timing of OnDestroy
        notifyValueSubject.AddTo(this);
    }

    // Publish event subscribers
    public IObservable<string> OnValueChanged
    { get { return notifyValueSubject; } }

   

    // publish event
    public void SetValue(string value)
    { notifyValueSubject.OnNext(value); }

  

    // Discard to prevent updates after stop
    public void OnDispose() {
        iDisposable.Dispose();
    }
}
