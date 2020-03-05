using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anime_Odcinki.File_and_Data_Manager
{
    class Files_manager
    {
        public Boolean File_Exist(String location) {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\AOI");
            return File.Exists(location);
        }
        public String File_Read(String location) {
            using (StreamReader sr = new StreamReader(location))
            {
                return sr.ReadToEnd();
            }
        }
        public String Nick_MAL(String location) {
            using (StreamReader sr = new StreamReader(location))
            {
                if (!(sr.Peek() < 0))  return sr.ReadLine();
                else return "";
            }
        }
        public void Save_to_File(String text) {
            File.WriteAllText(Data.Data.location , text);
        }

    }
}
