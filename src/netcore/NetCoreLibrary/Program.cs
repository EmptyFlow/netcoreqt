using System.IO;
using System.Threading.Tasks;

namespace NetCoreQtLibrary {

    public class Program {

        public static async Task Main ( string[] args ) {
            await Task.Delay ( 10000 );
            File.WriteAllText ( "C:/work/testlalala.txt", "!!!" );
        }

    }

}