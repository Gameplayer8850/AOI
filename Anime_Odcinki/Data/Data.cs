using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Anime_Odcinki.Data
{
    static class Data
    {
        public static String location;
        public static Boolean file_exist;
        public static String file_name = "Data.txt";
        public static String nick_MAL;
        public static List <Anime> list=new List<Anime>();
        public static List<Anime> new_episodes = new List<Anime>();
        public struct Anime {
            public int num_watched { get; set; }
            public String title { get; set; }
            public int num_epiosde { get; set; }
            public Boolean airing { get; set; }
            public String image_url { get; set; }
            public int year { get; set; }
            public Boolean saved_in_file { get; set; }
            public String link_to_anime { get; set; }
            public String link_to_epiosde { get; set; }

        }

    }
}
