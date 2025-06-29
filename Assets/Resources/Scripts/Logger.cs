using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    internal class AOBLogger
    {
        string path;
        public AOBLogger() 
        {
            string directory = Directory.GetCurrentDirectory(); //D:\UnityProjects\AgeOfBattleships
            path = directory + @"\Assets\Resources\Logger.txt";
            

            if (!File.Exists(path))
                File.Create(path).Dispose();

        }

        public void Log(string message) 
        {
            using (TextWriter tw = new StreamWriter(path,true))
            {   
                tw.WriteLine(message);
            }

        }
    }
}
