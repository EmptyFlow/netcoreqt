using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace NetCoreQt.Generator.UnitTests {

    public record ConceptOfEvent {

        public int IntValue { get; init; }

    }

    public static class ConveptOfEventExternal {

        public static ConcurrentDictionary<int, ConceptOfEvent> m_events = new ();

        public static int m_counter;

        public delegate void FireEventDelegate ( IntPtr value );

        public static FireEventDelegate? m_fireEventDelegateHandler;

        public static IntPtr GetIntValue ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( m_events.TryGetValue ( id, out var value ) ) return value.IntValue;

            return -1;
        }

        public static void CompleteEvent ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( !m_events.ContainsKey ( id ) ) return;

            m_events.TryRemove ( id, out var _ );
        }

        public static void FireEventCallback ( IntPtr callback ) {
            m_fireEventDelegateHandler = Marshal.GetDelegateForFunctionPointer<FireEventDelegate> ( callback );
        }

        public static int Create ( ConceptOfEvent newEvent ) {
            var value = Interlocked.Increment ( ref m_counter );
            if ( !m_events.TryAdd ( value, newEvent ) ) throw new Exception ( $"Can't create event with index {value}" );

            m_fireEventDelegateHandler?.Invoke ( value );
            return value;
        }

    }

}
