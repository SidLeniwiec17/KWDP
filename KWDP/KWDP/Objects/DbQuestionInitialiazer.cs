using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP.Objects
{
    public static class DbQuestionInitialiazer
    {
        public static void InitializeQuestions()
        {
            if(!IsDbFilled())
            {
                List<DbQuestion> questions = GetQuestions();
                
                DBHandler conn = new DBHandler();
                conn.InitializeConnection();               

                for (int i = 0; i < questions.Count; i++)
                {
                    conn.AddQuestionToTable(questions.ElementAt(i));
                }
                
                conn.CloseConnection();
            }
        }

        public static bool IsDbFilled()
        {
            bool isFilled = false;
            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            var question = conn.GetAllQuestions();
            if(question.Count >= 1)
            {
                isFilled = true;
            }
            conn.CloseConnection();
            return isFilled;
        }

        public static List<DbQuestion> GetQuestions()
        {
            List<DbQuestion> questions = new List<DbQuestion>();

            questions.Add(new DbQuestion("Czy pacjent odczówa ból w klatce piersowiej ?",
                "Ból pojawia się zazwyczaj z przodu klatki piersiowej, czasem w okolicy góry brzucha. Może promieniować do karku, szczęki lub któregoś z ramion. Ból z natury jest ciasny i przygniatający.", 
                0));
            questions.Add(new DbQuestion("Czy pacjent cierpi na problemy oddechowe ?",
                "Czy wystepują duszności lub problemy oddechowe podczas średnio wymagającego wysiłku fizycznego ? Czy podczas snu istnieje potrzeba przyjęcia odpowiedniej pozycji w celu ułatwienia oddychania ? Czy zdaża się napadowa dusznośc z świszczącym oddechem, poceniem sie, bólem oraz kaszlem ?",
                0));
            questions.Add(new DbQuestion("Czy pacjent cierpi na palpitacje serca ?",
                "Czy pojawia się uczucie kołatania serca (pulsowanie, uderzanie, obijanie) ?",
                0));
            questions.Add(new DbQuestion("Czy pacjent cierpi na zaburzenia tętna ?",
                "Czy tętno znacznie przyspiesza zwalnia, gubi rytm ? Zdażenia takie mogą trwać którko lub długo.",
                0));
            questions.Add(new DbQuestion("Czy pacjent jest uzalezniony od narkotyków lub silnych lekarstw ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent ma objawy żołądkowo-jelitowe ?",
                "Czy pojawia się ból wątroby lub wzdęcie brzucha ?",
                0));
            questions.Add(new DbQuestion("Czy pacjent ma problem z oddawaniem moczu ?",
                "Czy oddawanie moczu zdarza się rzadko i w małych ilościach ?",
                0));
            questions.Add(new DbQuestion("Czy pacjent ma problemy ciśnieniowe ?",
                "Czy zdarzają się : zawroty głowy, bóle głowy, zmiany psychiczne ?",
                0));
            questions.Add(new DbQuestion("Czy w rodzinie wystepowały choroby serca ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy w dzieciństwie występowały problemy z sercem lub ciśnieniem ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent jest palaczem ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent cierpi na otyłość ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent zdrowo sie odzywia ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent prowadzi aktywny cz siedzacy tryb zycia ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent jest narażony na podniesiony poziom stresu ?",
                "",
                0));
            questions.Add(new DbQuestion("Czy pacjent zauważył siność skóry pod paznokciami, siność uszu lub nosa ?",
                "",
                0));
            return questions;
        }
    }
}
