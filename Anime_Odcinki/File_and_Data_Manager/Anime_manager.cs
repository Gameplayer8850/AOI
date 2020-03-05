using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anime_Odcinki.File_and_Data_Manager
{
    class Anime_manager
    {
        Web.Web wb;
        public Anime_manager()
        {
            wb = new Web.Web();
        }
        public void Load_anime_list(String htmlcode) {
            Boolean end_list=false;
            do {
                if (htmlcode.Contains("\"status\""))
                {
                    try
                    {
                        Data.Data.Anime am = new Data.Data.Anime();

                        htmlcode=htmlcode.Substring(htmlcode.IndexOf("num_watched_episodes"));
                        am.num_watched = int.Parse((htmlcode.Substring(htmlcode.IndexOf(":") + 1, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 1)).Replace(" ", "").Replace("\"", ""));
                        htmlcode=htmlcode.Substring(htmlcode.IndexOf("anime_title"));
                        am.title = htmlcode.Substring(htmlcode.IndexOf(":") + 2, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 3);
                        htmlcode = htmlcode.Substring(htmlcode.IndexOf("anime_num_episodes"));
                        am.num_epiosde= int.Parse((htmlcode.Substring(htmlcode.IndexOf(":") + 1, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 1)).Replace(" ", "").Replace("\"", ""));
                        htmlcode = htmlcode.Substring(htmlcode.IndexOf("anime_airing_status"));
                        am.airing = (htmlcode.Substring(htmlcode.IndexOf(":") + 1, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 1)).Replace(" ", "").Replace("\"", "") == "1" ? true : false;
                        htmlcode = htmlcode.Substring(htmlcode.IndexOf("anime_image_path"));
                        am.image_url = htmlcode.Substring(htmlcode.IndexOf(":") + 2, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 3).Replace("\\", "");
                        htmlcode = htmlcode.Substring(htmlcode.IndexOf("anime_start_date_string"));
                        am.year= int.Parse(htmlcode.Substring(htmlcode.IndexOf(":") + 2, htmlcode.IndexOf(",") - htmlcode.IndexOf(":") - 3).Substring(6));
                        am.saved_in_file = false;
                        am.link_to_anime = "";
                        am.link_to_epiosde = "";
                        Data.Data.list.Add(am);
                    }
                    catch{
                        if (htmlcode.Contains("\"status\"")) htmlcode = htmlcode.Substring(htmlcode.IndexOf("\"status\""));
                        else end_list = true;
                    }
                }
                else end_list = true;
            } while (!end_list);
            
        }
        public void Load_anime_from_file(String text) {
            String[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                if (!lines[i].Contains("http")) continue;
                String[] line = new String[2];
                line[0] = lines[i].Substring(lines[i].IndexOf('{') + 1, lines[i].IndexOf('}') - lines[i].IndexOf('{')-1);
                line[1] = lines[i].Substring(lines[i].IndexOf('[') + 1, lines[i].IndexOf(']') - lines[i].IndexOf('[') - 1);
                foreach (Data.Data.Anime a1 in Data.Data.list) {
                    if (a1.title.Equals(line[0])) {
                        Data.Data.Anime a2 = a1;
                        a2.saved_in_file = true;
                        a2.link_to_anime = line[1];
                        Data.Data.list[Data.Data.list.IndexOf(a1)] = a2;
                        break;
                    }
                }
            }
        }
        public String Link_to_Anime(Data.Data.Anime a1)
        {
            String link = "";
            if (a1.airing) link=Get_Link_Airing(a1.title, a1.year, a1.num_epiosde);
            if (link.Equals("")) link=Get_Link_Search(a1.title, a1.year, a1.num_epiosde);
            return link;
        }
        private String Get_Link_Airing(String title, int year, int num_epiosde) {
            String code = wb.HtmlCode("https://www.animezone.pl/");
            code=code.Substring(code.IndexOf("Emitowane"));
            List<String[]> anime_to_check = new List<String[]>();
            List<String[]> anime_to_check2 = new List<String[]>();
            if (code.Contains("/anime/" + title.Substring(0, 2).ToLower().Replace(" ", ""))) {
                code = code.Substring(code.IndexOf("/anime/" + title.Substring(0, 2).ToLower().Replace(" ", "")));
                while (code.Contains("/anime/" + title.Substring(0, 2).ToLower().Replace(" ", ""))) {
                    code = code.Substring(code.IndexOf("/anime/" + title.Substring(0, 2).ToLower().Replace(" ", "")));
                    String link = code.Substring(code.IndexOf("/anime/" + title.Substring(0, 2).ToLower().Replace(" ", "")), code.IndexOf("\""));
                    if (!link.Contains("/favorite") && !link.Contains("/rating/") && !link.Contains("/watched/"))
                    {
                        String[] to_add = { link, "" };
                        anime_to_check.Add(to_add);
                    }
                    code = code.Substring(3);
                }
                String[] title_full = title.Split(' ');
                anime_to_check2 = new List<String[]>(anime_to_check);
                foreach (String[] anime in anime_to_check) {
                    int include = 0;
                    for (int i = 0; i < title_full.Length; i++) {
                       if (anime[0].Contains(title_full[i].ToLower())) include++;
                    }
                    String[] anime_with_num = { anime[0], include.ToString() };
                    anime_to_check2[anime_to_check2.IndexOf(anime)] = anime_with_num;                 
                }
                anime_to_check = anime_to_check2;
                anime_to_check.OrderByDescending(l => int.Parse(l[1]));
                
                foreach (String[] anime in anime_to_check)
                {
                    if (int.Parse(anime[1]) < title_full.Length / 2) continue;
                    if (Check_Anime(anime[0], year, num_epiosde, true)) return ("https://www.animezone.pl"+anime[0]);
                }
                    return "";
            }
            return "";
        }
        private String Get_Link_Search(String title, int year, int num_episode) {
            String[] title_full = title.Split(' ');
            List<String> wrong_anime = new List<String>();
            Boolean check_other_way = false;
            int start_index = 0, last_index = 0;

            for (int i = 0; i < title_full.Length; i++) {
                if (!check_other_way) last_index = i;
                else start_index++;
                if (check_other_way && i == title_full.Length - 1) break;
                String search = "";
                List<String> anime_to_check = new List<String>();
                    for (int k = 0+start_index; k < title_full.Length - last_index; k++) {
                        if (k != 0+start_index) search += "+";
                        search += title_full[k];
                    }
                String code=wb.HtmlCode("https://www.animezone.pl/szukaj?q="+search);
                code = code.Substring(code.IndexOf("site-main"), code.IndexOf("site-sidebar")- code.IndexOf("site-main"));
                while (code.Contains("/anime/")) {
                    code = code.Substring(code.IndexOf("/anime/"));
                    String link = code.Substring(0, code.IndexOf("\">"));
                    if (!wrong_anime.Contains(link)) {
                        wrong_anime.Add(link);
                        anime_to_check.Add(link);
                    }
                    code=code.Substring(3);
                }
                foreach (String link in anime_to_check) {
                    if (Check_Anime(link, year, num_episode)) return ("https://www.animezone.pl" + link);
                }
                if (i == title_full.Length - 1) {
                    check_other_way = true;
                    last_index = 0;
                    i = -1;
                }
            }
            return "";

        }
        private Boolean Check_Anime(String link, int year, int num_episode, Boolean airing=false) {
            try
            {
                String code = wb.HtmlCode("https://www.animezone.pl" + link);
                int year_a = -1;
                if (code.Contains("Rok produkcji"))
                {
                    code = code.Substring(code.IndexOf("Rok produkcji") + 24);
                    year_a = int.Parse(code.Substring(code.IndexOf("<td>")+4, code.IndexOf("</td>")- code.IndexOf("<td>")-4).Replace(" ", "").Substring(2));
                }
                if (!code.Contains("Numer")) return false;
                code = code.Substring(code.IndexOf("Numer"));
                int last_episode = int.Parse(code.Substring(code.IndexOf("<strong>") + 8, code.IndexOf("</strong>") - code.IndexOf("<strong>") - 8).Replace(" ", "").Replace("-", "-1"));
                if ((year_a == year || year_a == -1) && ((num_episode != 0 && last_episode == num_episode) || (num_episode == 0 && last_episode == -1)|| airing )) return true;
                else return false;
            }
            catch {
                return false;
            }

        }
        public int Last_Polish_Episode(String link) {
            if (link.Equals("")) return -1;
            String code = wb.HtmlCode(link);
            if (code.Contains("sprites PL")){
                code=code.Substring(code.IndexOf("<strong>"), code.IndexOf("sprites PL")- code.IndexOf("<strong>"));
                while (code.Substring(3).Contains("<strong>")) {
                    code = code.Substring(3);
                }
                return int.Parse(code.Substring(code.IndexOf("<strong>")+8, code.IndexOf("</strong>")- code.IndexOf("<strong>")-8).Replace(" ", ""));
            }else return -1;
        }
        public String Text_to_Save_in_File() {
            String text = Data.Data.nick_MAL + "\r\n";
            foreach (Data.Data.Anime a1 in Data.Data.list)
            {
                text += "{" + a1.title + "} [" + a1.link_to_anime + "]\r\n";
            }
            return text;
        }
        public Boolean Is_It_Link_to_Anime(String link) {
            if (!link.Contains("animezone.pl/anime/")) return false;
            else {
                if (wb.HtmlCode(link).Equals("")) return false;
                return true;
            }
        }
        public String Add_https(String link)
        {
            if (link.Contains("animezone.pl/anime/")) link = "https://www." + link.Substring(link.IndexOf("animezone.pl/anime/"));
            return link;
        }
    }
}
