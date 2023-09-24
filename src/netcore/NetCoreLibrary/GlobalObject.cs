using System.Reflection;
using System.Runtime.InteropServices;
using static NetCoreQtLibrary.Helpers;

namespace NetCoreQtLibrary {

    public class GlobalObject {

        private readonly Dictionary<int, PropertyInfo> m_integers = new ();

        private readonly Dictionary<int, PropertyInfo> m_doubles = new ();

        private readonly Dictionary<int, PropertyInfo> m_strings = new ();

        private readonly object? m_globalObject;

        public GlobalObject ( object globalObject ) {
            if ( globalObject == null ) throw new ArgumentNullException ( nameof ( globalObject ) );

            m_globalObject = globalObject;
            var properties = globalObject.GetType ().GetProperties ();
            var iterator = -1; //TODO: supporting attributes for user defined order
            foreach ( var property in properties ) {
                if ( property.DeclaringType == typeof ( int ) ) m_integers.Add ( iterator++, property );
                if ( property.DeclaringType == typeof ( double ) ) m_doubles.Add ( iterator++, property );
                if ( property.DeclaringType == typeof ( string ) ) m_strings.Add ( iterator++, property );
            }
        }

        public void SetGlobalInt32 ( int objectId, int value ) {
            m_integers[objectId].GetSetMethod ()!.Invoke ( m_globalObject, new object[] { value } );
        }

        public void SetGlobalDouble ( int objectId, double value ) {
            m_doubles[objectId].GetSetMethod ()!.Invoke ( m_globalObject, new object[] { value } );
        }

        public void SetGlobalString ( int objectId, nint value ) {
            var stringValue = StringConversion ( value ) ?? "";

            m_strings[objectId].GetSetMethod ()!.Invoke ( m_globalObject, new object[] { stringValue } );
        }

        public int GetGlobalInt32 ( int objectId ) {
            var value = m_integers[objectId].GetGetMethod ()!.Invoke ( m_globalObject, null );
            return value is int result ? result : 0;
        }

        public double GetGlobalDouble ( int objectId ) {
            var value = m_integers[objectId].GetGetMethod ()!.Invoke ( m_globalObject, null );
            return value is double result ? result : 0.0;
        }

        public nint GetGlobalString ( int objectId ) {
            var value = m_strings[objectId].GetSetMethod ()!.Invoke ( m_globalObject, null );

            return Marshal.StringToHGlobalUni ( value is string result ? result : "" );
        }

    }

}
