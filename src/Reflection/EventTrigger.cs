using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DSP_EventSystem.Reflection
{
    public class RegisterEventTriggerAttribute : Attribute { }

    /// <summary>
    /// event chain
    /// <para>the first event will be popped when the event is triggered</para>
    /// </summary>
    public class EventChain : Queue<Event>
    {
        public int CurrentEventStage { get; private set; }

        public EventChain()
        {
            CurrentEventStage = 0;
        }

        [CanBeNull]
        public new Event Dequeue()
        {
            if (Count == 0) return null;
            var e = base.Dequeue();
            CurrentEventStage++;
            return e;
        }

        public new void Clear()
        {
            base.Clear();
            CurrentEventStage = 0;
        }
    }
}
