using NetCoreQt.Generator.CodeSaver;
using System.Threading.Tasks;

namespace NetCoreQtLibrary {


    public class Program {

        public static async Task Main ( string[] args ) {
            await Task.Delay ( 10000 );
            MyEventExternal.Create ( new MyEvent ( 20, 100 ) );
            await Task.Delay ( 10000 );
            MyEventExternal.Create ( new MyEvent ( 98459345, 4332434 ) );
        }

    }

}