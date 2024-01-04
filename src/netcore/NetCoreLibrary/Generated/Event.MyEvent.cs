namespace NetCoreQt.Generator.CodeSaver {

        public record MyEvent (int Count, long Distance);

        public static class MyEventExternal {

            private static System.Collections.Concurrent.ConcurrentDictionary<int, MyEvent> m_events = new ();

            private static int m_counter;

            private delegate void FireEventDelegate ( nint value );

            private static FireEventDelegate? m_fireEventDelegateHandler;

            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static void CompleteEvent ( nint index ) {
                var id = index.ToInt32 ();
                if ( !m_events.ContainsKey ( id ) ) return;

                m_events.TryRemove ( id, out var _ );
            }

            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static void FireEventCallback ( nint callback ) {
                m_fireEventDelegateHandler = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<FireEventDelegate> ( callback );
            }

            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static nint GetCount ( nint index ) {
                if ( m_events.TryGetValue ( index.ToInt32 (), out var value ) ) {
                    return value.Count;
                }

                return 0;
            }
            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static nint GetDistance ( nint index ) {
                if ( m_events.TryGetValue ( index.ToInt32 (), out var value ) ) {
                    return (nint)value.Distance;
                }

                return 0;
            }


            public static int Create ( MyEvent newEvent ) {
                var value = System.Threading.Interlocked.Increment ( ref m_counter );
                if ( !m_events.TryAdd ( value, newEvent ) ) throw new System.Exception ( $"Can't create event with index {value}" );

                m_fireEventDelegateHandler?.Invoke ( value );
                return value;
            }

        }

}
