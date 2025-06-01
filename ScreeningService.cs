using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using System.Collections.Generic;
using System.Linq;

namespace CinemaTicketServerREST.Services
{
    public static class ScreeningService
    {
        private static readonly string FilePath = "Data/screenings.json";
        private static List<Screening> _screenings = JsonStorage.LoadFromFile<Screening>(FilePath);

        public static List<Screening> GetAllScreenings()
        {
            return _screenings;
        }

        public static Screening? GetById(int id)
        {
            return _screenings.FirstOrDefault(s => s.ScreeningID == id);
        }

        public static void SaveAllScreenings(List<Screening> screenings)
        {
            _screenings = screenings;
            JsonStorage.SaveToFile(FilePath, _screenings);
        }
    }
}
