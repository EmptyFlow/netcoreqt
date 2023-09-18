using System.Runtime.InteropServices;

namespace NetCoreQtLibrary {

    public class Helpers {

        public delegate string? StringConversionDelegate ( nint ptr );

        public static readonly StringConversionDelegate StringConversion = RuntimeInformation.IsOSPlatform ( OSPlatform.Windows ) ? Marshal.PtrToStringUni : Marshal.PtrToStringUTF8;

    }

}
