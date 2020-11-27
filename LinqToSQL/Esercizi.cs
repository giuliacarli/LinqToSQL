using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSQL
{
    public class Esercizi
    {
        const string conn = @"Persist Security Info = False; Integrated Security = true; Initial Catalog = CinemaDb; Server = WINAP6A66HJRUSC\SQLEXPRESS";

        //Selezionare i film
        public static void SelectMovies()
        {
            CinemaDataContext db = new CinemaDataContext(conn);

            foreach(var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2}",movie.ID, movie.Titolo, movie.Genere);
            }

            Console.ReadKey();
        }

        //Filtrare i film
        public static void FilterMovieByGenere()
        {

            CinemaDataContext db = new CinemaDataContext(conn);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2}", movie.ID, movie.Titolo, movie.Genere);
            }

            //Query 
            Console.WriteLine("Genere: ");
            string Genere;
            Genere = Console.ReadLine();

            IQueryable<Movy> moviesFiltered =
                from m in db.Movies
                where m.Genere == Genere
                select m;

            foreach (var film in moviesFiltered)
            {
                Console.WriteLine("{0} - {1} - {2}", film.ID, film.Titolo, film.Genere);
            }

            Console.ReadKey();

        }


        //inserire record
        public static void InsertMovie()
        {
            CinemaDataContext db = new CinemaDataContext(conn);

            SelectMovies();

            var movieToInsert = new Movy();         // insert
            movieToInsert.Titolo = "Inception";
            movieToInsert.Genere = "Romantico";
            movieToInsert.Durata = 120;

            db.Movies.InsertOnSubmit(movieToInsert); // qui non mi riconcilia i dati col db, solo quando faccio submitchanges() lo fa

            var deleteMovie = db.Movies.SingleOrDefault(m => m.ID == 2); // delete

            if (deleteMovie != null)
            {
                db.Movies.DeleteOnSubmit(deleteMovie);
            }


            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            SelectMovies();

            Console.ReadKey();

        }


        //update movie
        public static void UpdateMovieByTitolo()
        {
            CinemaDataContext db = new CinemaDataContext(conn);

            Console.WriteLine("Dimmi il titolo del film da aggiornare: ");
            string titolo = Console.ReadLine();

            IQueryable<Movy> filmByTitolo =
                from film in db.Movies
                where film.Titolo == titolo
                select film;

            Console.WriteLine("I film trovati sono: {0}", filmByTitolo.Count());

            if (filmByTitolo.Count() == 0)
            {
                return;
            }

            if (filmByTitolo.Count() > 1)
            {
                return;
            }

            SelectMovies();

            Console.WriteLine("Scrivere i valori aggiornati ");
            Console.WriteLine("Titolo: ");
            string titolo2 = Console.ReadLine();

            Console.WriteLine("Genere: ");
            string genere = Console.ReadLine();

            Console.WriteLine("Durata: ");
            int durata = Convert.ToInt32(Console.ReadLine());

            foreach(var f in filmByTitolo)
            {
                f.Titolo = titolo2;
                f.Genere = genere;
                f.Durata = durata;

            }

            try
            {
                Console.WriteLine("Premi un tasto per mandare modifiche al db");
                Console.ReadKey();
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (ChangeConflictException e)
            {
                Console.WriteLine("Cuncurrency error");
                Console.WriteLine(e);

                Console.ReadKey();
                db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues); // qui aggiorno il mio datamodel

                db.SubmitChanges(); // qui è inutile perché appunto sto aggiornando il mio datamodel, negli altri due invece è indispensabile perché lui da solo non lo fa e mi deve andare ad aggiornare il db
            }

        }
    }
}
