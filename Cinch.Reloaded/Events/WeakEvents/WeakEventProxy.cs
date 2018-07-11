using System;
using System.Diagnostics;
using System.Reflection;

namespace Cinch.Reloaded.Events.WeakEvents
{
    /// <summary>
    /// http://www.paulstovell.com/weakevents
    /// 
    /// An event handler wrapper used to create weak-reference event handlers, 
    /// so that event subscribers can be garbage collected without the event publisher 
    /// interfering. 
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments used in the event handler.
    /// </typeparam>
    /// <example>
    /// <![CDATA[
    /// 
    ///        When subscribing to events, instead of writing:
    ///        
    ///        alarm.Beep += Alarm_Beeped;
    ///        
    ///        Just write:
    ///        
    ///        alarm.Beeped += new WeakEventHandler<AlarmEventArgs>(Alarm_Beeped).Handler;
    /// ]]>
    /// </example>
    [DebuggerNonUserCode]
    public sealed class WeakEventProxy<TEventArgs> where TEventArgs : System.EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakEventProxy(EventHandler<TEventArgs> callback)
        {
            _method = callback.Method;
            _targetReference = new WeakReference(callback.Target, true);
        }

        [DebuggerNonUserCode]
        public void Handler(object sender, TEventArgs e)
        {
            var target = _targetReference.Target;
            if (target != null)
            {
                var callback = (Action<object, TEventArgs>)Delegate.CreateDelegate(typeof(Action<object, TEventArgs>), target, _method, true);
                if (callback != null)
                {
                    callback(sender, e);
                }
            }
        }
    }
}