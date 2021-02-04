using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureAtlasUnity.Utility
{
    /*
        <summary>
            Contains Unity String Utility functions
        </summary>
    */
    static class UnityStringUtilities
    {
        /*
        <summary>
            returns the number of occurances of a target char.
        </summary>
        */
        public static int CountOccurances(string str, char target)
        {
            int num = 0;

            foreach (char c in str)
            {
                if (c == target)
                {
                    num++;
                }
            }

            return num;
        }

        /*
        <summary>
            Removes Filename from the end of the path string. It simply looks
            for the most occurances of forward and backward slashes and uses which ever is the most.
            Then looks for the ending slash removes what is after it.
        </summary>
        */
        public static string RemoveFileName(string path)
        {
            string newPath = "";
            char directoryChar = '/';
            int endOfPath = 0;

            int numBackSlashes = 0;
            int numForwardSlashes = 0;

            numBackSlashes = CountOccurances(path, '\\');
            numForwardSlashes = CountOccurances(path, '/');

            if (numBackSlashes > numForwardSlashes)
            {
                directoryChar = '\\';
            }

            endOfPath = path.LastIndexOf(directoryChar);

            newPath = path.Substring(0, endOfPath + 1);

            return newPath;
        }
    }
}
