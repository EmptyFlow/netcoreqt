using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using static NetCoreQtLibrary.ConceptOfClassExternal;

namespace NetCoreQtLibrary {

    public static class ConceptOfClassExternal {

        public static Dictionary<int, ConceptOfClass> m_events = new ();

        public static Dictionary<int, SetIdDelegate> m_delegates = new ();

        [UnmanagedCallersOnly]
        public static IntPtr GetId ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( !m_events.ContainsKey ( id ) ) return -1;

            return m_events[id].Id;
        }

        [UnmanagedCallersOnly]
        public static void SetId ( IntPtr index, IntPtr value ) {
            var id = index.ToInt32 ();
            if ( !m_events.ContainsKey ( id ) ) return;

            m_events[id].Id = value.ToInt32 ();
        }

        public delegate void SetIdDelegate ( IntPtr value );

        [UnmanagedCallersOnly]
        public static void SetIdCallback ( IntPtr index, IntPtr callback ) {
            var identifier = index.ToInt32 ();
            if ( m_delegates.ContainsKey ( identifier ) ) return;

            m_delegates.Add ( identifier, Marshal.GetDelegateForFunctionPointer<SetIdDelegate> ( callback ) );
        }

        public static void PerformIdCallback ( IntPtr index, IntPtr value ) {
            var identifier = index.ToInt32 ();
            m_delegates[identifier] ( value );
        }

    }

    public class ConceptOfClass {

        private readonly int m_index;

        public ConceptOfClass ( int index ) => m_index = index;

        private int m_id = 110;

        public int Id {
            get {
                return m_id;
            }
            set {
                value = m_id;
                PerformIdCallback ( m_index, m_id );
            }
        }

    }

    public static class ConceptOfEventExternal {

        private static ConcurrentDictionary<int, ConceptOfEvent> m_events = new ();

        private static int m_counter = 0;

        [UnmanagedCallersOnly]
        public static IntPtr GetId ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( m_events.TryGetValue ( id, out var value ) ) return value.Id;

            return -1;
        }

        [UnmanagedCallersOnly]
        public static void CompleteEvent ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( !m_events.ContainsKey ( id ) ) return;

            m_events.TryRemove ( id, out var _ );
        }

        public delegate void FireEventDelegate ( IntPtr value );

        public static FireEventDelegate? fireEventDelegateHandler = null;

        [UnmanagedCallersOnly]
        public static void FireEventCallback ( IntPtr callback ) {
            fireEventDelegateHandler = Marshal.GetDelegateForFunctionPointer<FireEventDelegate> ( callback );
        }

        public static int Create ( ConceptOfEvent newEvent ) {
            var value = Interlocked.Increment ( ref m_counter );
            if ( !m_events.TryAdd ( value, newEvent ) ) throw new Exception ( $"Can't create event with index {value}" );

            fireEventDelegateHandler?.Invoke ( value );
            return value;
        }

    }

    public record ConceptOfEvent {

        private readonly int m_index;

        public ConceptOfEvent ( int index ) => m_index = index;

        public int Id { get; init; }

    }


    public static class NetCoreQtEntryPoint {

        [UnmanagedCallersOnly]
        public static void Initialize () {
            // make preparation
        }

        [UnmanagedCallersOnly]
        public static void Deinitialize () {
            // remove all unmanaged
        }

    }

}