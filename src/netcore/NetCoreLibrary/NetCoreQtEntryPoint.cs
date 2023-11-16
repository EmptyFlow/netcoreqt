using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        public static SetIdDelegate? setIdDelegateHandler = null;

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
                ConceptOfClassExternal.PerformIdCallback ( m_index, m_id );
            }
        }

    }

    public static class ConceptOfEventExternal {

        public static Dictionary<int, ConceptOfEvent> m_events = new ();

        [UnmanagedCallersOnly]
        public static IntPtr GetId ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( !m_events.ContainsKey ( id ) ) return -1;

            return m_events[id].Id;
        }

        [UnmanagedCallersOnly]
        public static void CompleteEvent ( IntPtr index ) {
            var id = index.ToInt32 ();
            if ( m_events.ContainsKey ( id ) ) m_events.Remove ( id );
        }

        public static void Create ( ConceptOfEvent newEvent ) {
            m_events.Add ( 1, newEvent );
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

    public static class NetCoreQtImportGlobal {

        private static GlobalObject? m_globalObject = null;

        public static void SetGlobalObject ( GlobalObject globalObject ) => m_globalObject = globalObject;

        [UnmanagedCallersOnly]
        public static void SetGlobalInt32 ( int objectId, int value ) => m_globalObject!.SetGlobalInt32 ( objectId, value );

        [UnmanagedCallersOnly]
        public static void SetGlobalDouble ( int objectId, double value ) => m_globalObject!.SetGlobalDouble ( objectId, value );

        [UnmanagedCallersOnly]
        public static void SetGlobalString ( int objectId, nint value ) => m_globalObject!.SetGlobalString ( objectId, value );

        [UnmanagedCallersOnly]
        public static int GetGlobalInt32 ( int objectId ) => m_globalObject!.GetGlobalInt32 ( objectId );

        [UnmanagedCallersOnly]
        public static double GetGlobalDouble ( int objectId ) => m_globalObject!.GetGlobalDouble ( objectId );

        [UnmanagedCallersOnly]
        public static nint GetGlobalString ( int objectId ) => m_globalObject!.GetGlobalString ( objectId );

    }

    public static class NetCoreQtExportGlobal {

        [UnmanagedCallersOnly]
        public static void SetGlobalInt32Method ( nint value ) => SetGlobalInt32DelegatePointer = Marshal.GetDelegateForFunctionPointer<SetGlobalInt32Delegate> ( value );

        [UnmanagedCallersOnly]
        public static void SetGlobalDoubleMethod ( nint value ) => SetGlobalDoubleDelegatePointer = Marshal.GetDelegateForFunctionPointer<SetGlobalDoubleDelegate> ( value );

        [UnmanagedCallersOnly]
        public static void SetGlobalStringMethod ( nint value ) => SetGlobalStringDelegatePointer = Marshal.GetDelegateForFunctionPointer<SetGlobalStringDelegate> ( value );

        private delegate void SetGlobalInt32Delegate ( int objectId, int value );

        private delegate void SetGlobalDoubleDelegate ( int objectId, double value );

        private delegate void SetGlobalStringDelegate ( int objectId, nint value );

        private static SetGlobalInt32Delegate SetGlobalInt32DelegatePointer = SetGlobalInt32DelegatePointerEmpty;

        private static SetGlobalDoubleDelegate SetGlobalDoubleDelegatePointer = SetGlobalDoubleDelegatePointerEmpty;

        private static SetGlobalStringDelegate SetGlobalStringDelegatePointer = SetGlobalStringDelegatePointerEmpty;

        public static void SetGlobalInt32 ( int objectId, int value ) => SetGlobalInt32DelegatePointer ( objectId, value );

        public static void SetGlobalDouble ( int objectId, double value ) => SetGlobalDoubleDelegatePointer ( objectId, value );

        public static void SetGlobalString ( int objectId, nint value ) => SetGlobalStringDelegatePointer ( objectId, value );

        private static void SetGlobalInt32DelegatePointerEmpty ( int objectId, int value ) { }

        private static void SetGlobalDoubleDelegatePointerEmpty ( int objectId, double value ) { }

        private static void SetGlobalStringDelegatePointerEmpty ( int objectId, nint value ) { }

    }

}