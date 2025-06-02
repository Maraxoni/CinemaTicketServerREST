using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CinemaTicketServerREST.Models
{
    public class Movie
    {
        private static int lastId = 0;

        [DataMember]
        public int MovieID { get; private set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Director { get; set; }

        [DataMember]
        public List<string> Actors { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public byte[] Poster { get; set; }
        public List<Link> Links { get; set; } = new();

        public Movie(Movie movie)
        {
            MovieID = movie.MovieID;
            Title = movie.Title;
            Director = movie.Director;
            Actors = movie.Actors;
            Description = movie.Description;
            Poster = movie.Poster;
        }
        public Movie(string title, string director, List<string> actors, string description, byte[] poster = null)
        {
            MovieID = ++lastId;
            Title = title;
            Director = director;
            Actors = actors;
            Description = description;
            Poster = poster;
        }

        [JsonConstructor]
        public Movie(int movieID, string title, string director, List<string> actors, string description, byte[] poster)
        {
            MovieID = movieID;
            Title = title;
            Director = director;
            Actors = actors;
            Description = description;
            Poster = poster;

            if (movieID > lastId)
            {
                lastId = movieID;
            }
        }
    }
}
