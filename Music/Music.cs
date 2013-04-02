using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Music
{
    class Song
    {
        private static int idIncrement = -1;
        public enum AllGenres
        {
            Rock, Indie, Pop, Metal, Folk, Country, Jazz, Electronic, Blues, Classical, HipHop
        }
        public int Id { get; private set; }
        public StringBuilder Name { get; private set; }
        private List<AllGenres> genres;
        public TimeSpan Duration { get; private set; }
        public string Location { get; private set; }

        public Song(string name, TimeSpan duration, string location, params AllGenres[] genres)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or empty","name");
            }
            if (name.Length > 256)
            {
                throw new ArgumentOutOfRangeException("name","Cannot be more than 256 symbols. ");
            }
            if (duration.TotalSeconds==0)
            {
                throw new ArgumentNullException("length","Length cannot be 0. ");
            }
            if (String.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentNullException("location","Cannot be null or empty. ");
            }
            if (genres.Count()==0)
            {
                throw new ArgumentNullException("genres","The count must be one at least. ");
            }
            Name = new StringBuilder() {Capacity = 256};
            Name.Append(name);
            Id = ++idIncrement;
            this.genres = genres.ToList();
            Duration = duration;
            Location = location;
        }
        public void ChangeName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Name.Clear();
            this.Name.Append(name);
        }
        public List<AllGenres> GetGenres()
        {
            return genres;
        }
        public void AddGenres(params AllGenres[] newGenres)
        {
            if (newGenres == null)
            {
                throw new ArgumentNullException("newGenres");
            }
            for (var i =0; i < newGenres.Count(); i++)
            {
                this.genres.Add(newGenres[i]);
            }
        }
        public void ClearGenres(params AllGenres[] newGenres)
        {
            this.genres.Clear();
            AddGenres(newGenres);
        }
        public void ChangeLocation(string newLocation)
        {
            if (String.IsNullOrWhiteSpace(newLocation))
            {
                throw new ArgumentException("The file`s location cannot be null or empty. ");
            }
            Location = newLocation;
        }
    }

    class Playlist
    {
        private static int idIncrement = -1;
        public int Id { get; private set; }
        public StringBuilder Name { get; private set; }
        public TimeSpan Duration { get; private set; }
        public List<Song> Songs { get; private set; }

        public Playlist(string name,List<Song> songs)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name", "Cannot be null or empty. ");
            }
            if (name.Length > 256)
            {
                throw new ArgumentOutOfRangeException("name", "Cannot be more than 256 symbols. ");
            }
            if (songs.Count == 0)
            {
                throw new ArgumentNullException("songs", "The count must be more than 0. ");
            }
            Id = ++idIncrement;
            Name = new StringBuilder() { Capacity = 256 };
            Name.Append(name);
            Songs = songs.Distinct().ToList();
            Duration = Songs.Aggregate(new TimeSpan(), (current, song) => current + song.Duration);
        }
        public List<Song> GetAllSongs()
        {
            return Songs;
        }
        public void ChangeName(string newName)
        {
            if (String.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException("name", "Cannot be null or empty. ");
            }
            Name.Clear();
            Name.Append(newName);
        }
        public void ShowSongsInfo()
        {
            foreach (var s in Songs)
            {
                Console.WriteLine(s.Id+" "+s.Name+" "+s.Duration+" "+s.Location);
            }
        }
        public void ShowSongInfo(Song song)
        {
            Console.WriteLine(song.Id + " " + song.Name + " " + song.Duration + " " + song.Location);
        }
        public void AddSong(Song newSong)
        {
            if (Songs.FindAll(s => s.Id == newSong.Id).Count != 0)
            {
                Console.WriteLine("This song wasn`t added because it`s always in playlist. ");
                return;
            }
            Songs.Add(newSong);
            Duration+=newSong.Duration;
        }
        public void AddSongs(List<Song> newSongs)
        {
            if (newSongs.Count == 0)
            {
                throw new ArgumentNullException("newSongs", "The count must be more than 0. ");
            }
            if (Songs.Intersect(newSongs).Count() != 0)
            {
                Console.WriteLine("Some songs weren`t added because they`re always in playlist. ");
            }
            Songs.AddRange(newSongs.Except(Songs.Intersect(newSongs)).Distinct());
            Duration = Songs.Aggregate(new TimeSpan(), (current, song) => current + song.Duration);
        }
        public void DeleteSong(int id)
        {
            if (Songs.Exists(s => s.Id == id))
            {
                Duration -= Songs.Single(s => s.Id == id).Duration;
                Songs.RemoveAt(id);
            }
            else
            {
                Console.WriteLine("This song doesn`t exist. ");
            }
        }
        public void DeleteSongs(string name)
        {
            if (Songs.Exists(s => s.Name.ToString().Trim() == name.Trim()))
            {
                Songs.RemoveAll(s => s.Name.ToString().Trim() == name.Trim());
                Duration = Songs.Aggregate(new TimeSpan(), (current, song) => current + song.Duration);
            }
            else
            {
                Console.WriteLine("This song doesn`t exist. ");
            }
        }
        public void ClearPlaylist()
        {
            Songs.Clear();
            Duration = TimeSpan.Zero;
        }
        public List<Song> FindSongs(string songName)
        {
            return Songs.Where(s=>s.Name.ToString().Trim()==songName.Trim()).ToList();
        }
        public List<Song> OrderEnumeration()
        {
            return Songs.OrderBy(x => x.Id).ToList();
        }
        public List<Song> ShuffleEnumeration()
        {
            return Songs.OrderBy(song=>Guid.NewGuid()).ToList();
        }
    }
    
    class Player
    {
        public List<Playlist> AddedPlaylists { get; private set; }

        public Player()
        {
            AddedPlaylists = new List<Playlist>();
        }
        public void AddPlaylist(Playlist playlist)
        {
            AddedPlaylists.Add(playlist);
        }
        public void DeletePlaylist(string name)
        {
            if (AddedPlaylists.Exists(p => p.Name.ToString().Trim() == name.Trim()))
            {
                AddedPlaylists.RemoveAll(p => p.Name.ToString().Trim() == name.Trim());
            }
            else
            {
                Console.WriteLine("This playlist doesn`t exist. ");
            }
        }
        public void CombinePlaylists(Playlist firstPlaylist,Playlist secondPlaylist,string newPlaylistName)
        {
            if (String.IsNullOrWhiteSpace(newPlaylistName))
            {
                throw new ArgumentNullException("newPlaylistName", "Cannot be null or empty. ");
            }
            var combinedPlaylist =  new Playlist(newPlaylistName,firstPlaylist.OrderEnumeration().Union(secondPlaylist.OrderEnumeration()).ToList());
            AddPlaylist(combinedPlaylist);
        }
    }

    internal class Music
    {
        static void Main(string[] args)
        {
            var obj = new Song("Riders on the storm", new TimeSpan(0,0,0,67), @"D:\", Song.AllGenres.Blues, Song.AllGenres.Classical);
            var objj = new Song("Spiders", new TimeSpan(0, 0, 0, 67), @"D:\", Song.AllGenres.Blues, Song.AllGenres.Classical);
            var obj2 = new Song("Riders ", new TimeSpan(0, 0, 0, 67), @"D:\", Song.AllGenres.Blues, Song.AllGenres.Classical);
            var obj3 = new Song("Riders ", new TimeSpan(0, 0, 0, 67), @"D:\", Song.AllGenres.Blues, Song.AllGenres.Classical);

            Console.WriteLine("Введите имя новой песни");
            string name = Console.ReadLine();
            Console.WriteLine("Введите продолжительность новой песни");
            var duration = TimeSpan.Parse(Console.ReadLine());
            Console.WriteLine("Введите расположение новой песни");
            string location = Console.ReadLine();
            Console.WriteLine("Введите жанры новой песни");
            Enum.Parse(Console, Console.ReadLine());

            //var l = new List<Song> {obj, objj,obj3,obj2};
            //var l2 = new List<Song> {obj2, obj3};
            //var p = new Playlist("PL1",l);
            //var p2 = new Playlist("PL2", l2);

            //var pl = new Player();
            //pl.CombinePlaylists(p,p2,"PL3");
            //foreach (var pk in pl.AddedPlaylists)
            //{
            //    pk.ShowSongsInfo();
            //}


            //Console.WriteLine();
            //p.DeleteSong("Riders");
            //Console.WriteLine();
            //Console.WriteLine(p.Duration);
            //var pl = new Player(p);
           
            
            //Console.WriteLine();
            //p.AddSongs(l2);
            //p.ShowSongs();
            //Console.WriteLine();
            //Console.WriteLine(p.Duration);
            Console.ReadKey();
        }
    }
}
