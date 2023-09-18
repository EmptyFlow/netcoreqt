using System.Reflection;
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

    }

}
