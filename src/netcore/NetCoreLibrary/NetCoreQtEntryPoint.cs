using System.Runtime.InteropServices;

namespace NetCoreQtLibrary {

    public class Test {

        public int Id { get; set; } = 110;

        public string Value { get; set; } = "Lalala123Аргאתať";

        public double Double { get; set; } = 4.5;
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