using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;

namespace Recaps
{
    public class HelperFunctions
    {
        public static DateTime ConvertFromEpoch(int time)
        {
            TimeSpan thisSpan = new TimeSpan(0, 0, time);
            DateTime realTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, new System.Globalization.GregorianCalendar(), DateTimeKind.Utc) + thisSpan;
            return realTime.ToLocalTime();

        }
        public static Hashtable LuaToHash(string fileToRead)
        {
            StreamReader thisReader = new StreamReader(fileToRead);
            string thisLine;
            Hashtable returnHash = new Hashtable();
            string currKey = "";
            string prevKey = "";

            while ((thisLine = thisReader.ReadLine()) != null)
            {
                if (thisLine.Contains('{'))
                {
                    if (currKey == "")
                    {
                        currKey = thisLine.Split('=')[0].Trim();
                    }
                    else if (thisLine.Contains('='))
                    {
                        string key = thisLine.Split('=')[0];
                        key = key.Trim(' ', '\t', '[', ']', '"');
                        currKey += "/" + key;
                        prevKey = currKey;
                    }
                    else
                    {
                        try
                        {
                            string[] prevKeyNumber = prevKey.Split('/');
                            int index = Convert.ToInt32(prevKeyNumber[prevKeyNumber.Length - 1]);
                            currKey += "/" + (index + 1);
                        }
                        catch (Exception e)
                        {
                            currKey += "/" + 0;
                        }
                    }
                }
                else if (thisLine.Contains('}'))
                {
                    string[] keys = currKey.Split('/');
                    prevKey = currKey;
                    currKey = "";

                    for (int j = 0; j < keys.Length - 1; j++)
                    {
                        currKey += keys[j];
                        if (j != keys.Length - 2)
                        {
                            currKey += "/";
                        }
                    }
                }
                else
                {
                    if (thisLine.Contains('='))
                    {
                        string[] keysAndValues = thisLine.Split('=');
                        string key = keysAndValues[0].Trim(' ', '\t', '[', ']', '"');
                        returnHash[currKey + "/" + key] = keysAndValues[1].Trim(',', ' ', '"');
                    }
                }
            }
            thisReader.Close();
            return returnHash;
        }
    }
}