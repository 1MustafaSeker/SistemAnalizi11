namespace SistemAnalizi.Models
{
    public enum TurkishDayOfWeek
    {
        Pazar = 0,
        Pazartesi = 1,
        Salı = 2,
        Çarşamba = 3,
        Perşembe = 4,
        Cuma = 5,
        Cumartesi = 6
    }

    public static class DayOfWeekHelper
    {
        public static readonly Dictionary<int, string> DayNames = new Dictionary<int, string>
    {
        { 0, "Pazar" },
        { 1, "Pazartesi" },
        { 2, "Salı" },
        { 3, "Çarşamba" },
        { 4, "Perşembe" },
        { 5, "Cuma" },
        { 6, "Cumartesi" }
    };

        public static string GetDayName(int day)
        {
            return DayNames.ContainsKey(day) ? DayNames[day] : "Bilinmeyen";
        }
    }
}