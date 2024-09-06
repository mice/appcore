using System;
using System.Collections.Generic;
using System.Text;

namespace SignalsLib
{
    public static  class EventUtils
    {
        class EventSignal : AbstractDataSignal<String>
        {

        }

        class EventSignal<T> : AbstractDataSignal<String,T>
        {

        }

        class EventSignal<T1,T2> : AbstractDataSignal<String, T1,T2>
        {

        }

        public static void RemoveListener(Delegate listener)
        {
            Signals.RemoveHandler(listener);
        }

        public static void Dispatch(string eventName)
        {
            Signals.Get<EventSignal>().Dispatch(eventName);
        }

        public static void Dispatch<T>(string eventName,T param)
        {
            Signals.Get<EventSignal<T>>().Dispatch(eventName, param);
        }

        public static void Dispatch<T1,T2>(string eventName, T1 param1,T2 param2)
        {
            Signals.Get<EventSignal<T1, T2>>().Dispatch(eventName, param1, param2);
        }

        public static Delegate AddListener(string eventName, Action listener)
        {
            return Signals.Get<EventSignal>().AddListener(eventName, listener);
        }


        public static Delegate AddListener<T>(string eventName, Action<T> listener)
        {
            return Signals.Get<EventSignal<T>>().AddListener(eventName, listener);
        }

        public static Delegate AddListener<T1,T2>(string eventName, Action<T1,T2> listener)
        {
            return Signals.Get<EventSignal<T1, T2>>().AddListener(eventName, listener);
        }
    }
}
