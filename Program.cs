using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mapi_Decode_ConvId
{
    /// <summary>
    /// DISCLAIMER: This code is experimental. It comes with no warranty or support or expectation that it is accurate.
    /// </summary>
    /// 
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("Please provide an EntryID");
                return;
            }
            
            string sentryid = args[0];
 
            Console.WriteLine(string.Empty);
            Console.WriteLine("entryId input:");
            Console.WriteLine(sentryid);
            Console.WriteLine(string.Empty);

            byte[] arr = System.Text.Encoding.ASCII.GetBytes(sentryid);
            using (MemoryStream memory = new MemoryStream(arr))
            {
                
                var _decode = ExchangeStoreReference.DecodeEntryId(new System.IO.BinaryReader(memory), string.Empty);
            }
        }

    }
}
