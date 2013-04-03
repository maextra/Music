using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Music
{
    class Song
    {
        private static int idIncrement = -1;
        
        public enum AllGenres
        {
            Empty=0, Rock, Indie, Pop, Metal, Folk, Country, Jazz, Electronic, Blues, Classical, HipHop
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

        public Playlist(string playlistName,List<Song> playlistSongs)
        {
            if (String.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("name", "Cannot be null or empty. ");
            }
            if (playlistName.Length > 256)
            {
                throw new ArgumentOutOfRangeException("name", "Cannot be more than 256 symbols. ");
            }
            if (playlistSongs.Count == 0)
            {
                throw new ArgumentNullException("songs", "The count must be more than 0. ");
            }
            Id = ++idIncrement;
            Name = new StringBuilder() { Capacity = 256 };
            Name.Append(playlistName);
            Songs = playlistSongs.Distinct().ToList();
            Duration = Songs.Aggregate(new TimeSpan(), (current, song) => current + song.Duration);
        }
        public void ChangeName(string newPlaylistName)
        {
            if (String.IsNullOrWhiteSpace(newPlaylistName))
            {
                throw new ArgumentNullException("name", "Cannot be null or empty. ");
            }
            Name.Clear();
            Name.Append(newPlaylistName);
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
        public void DeleteSong(int songId)
        {
            if (Songs.Exists(s => s.Id == songId))
            {
                Duration -= Songs.Single(s => s.Id == songId).Duration;
                Songs.RemoveAt(songId);
            }
            else
            {
                Console.WriteLine("This song doesn`t exist. ");
            }
        }
        public void DeleteSongs(string songName)
        {
            if (Songs.Exists(s => s.Name.ToString().Trim() == songName.Trim()))
            {
                Songs.RemoveAll(s => s.Name.ToString().Trim() == songName.Trim());
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
        public List<Song> GetSongsOrderly()
        {
            return Songs.OrderBy(x => x.Id).ToList();
        }
        public List<Song> GetSongsRandomly()
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
            var combinedPlaylist =  new Playlist(newPlaylistName,firstPlaylist.GetSongsOrderly().Union(secondPlaylist.GetSongsOrderly()).ToList());
            AddPlaylist(combinedPlaylist);
        }

        public delegate void Playing(Song currentSong);
        public event Playing StartPlaying;
        public event Playing EndPlaying;

        public void PlayOrderly(Playlist playlist)
        {
            if (playlist.Songs.Count == 0)
            {
                Console.WriteLine("The playlist is empty. There is nothing to play. ");
                return;
            }
            PlayPlaylist(playlist.GetSongsOrderly(), playlist);
        }
        public void PlayRandomly(Playlist playlist)
        {
            if (playlist.Songs.Count == 0)
            {
                Console.WriteLine("The playlist is empty. There is nothing to play. ");
                return;
            }
            var playingThread = new Task(() => PlayPlaylist(playlist.GetSongsRandomly(), playlist));
            playingThread.RunSynchronously();
            Task.WaitAll(playingThread);
        }
        private void PlayPlaylist(List<Song> songs,Playlist currentPlaylist)
        {
            TimeSpan totalPlaylistDuration = currentPlaylist.Duration;
            StartPlaying += StartPlayingSong;
            EndPlaying += EndPlayingSong;
            while (totalPlaylistDuration != TimeSpan.Zero)
            {
                foreach (var song in songs)
                {
                    totalPlaylistDuration -= song.Duration;
                    PlaySong(song);
                }
            }
        }
        private void PlaySong(Song currentSong)
        {
            if (StartPlaying == null || EndPlaying == null)
            {
                throw new ArgumentNullException("StartPlaying,EndPlaying", "Events cannot be null. ");
            }
            var firstTask = new Task(()=>StartPlaying(currentSong));
            var secondTask =firstTask.ContinueWith(task => EndPlaying(currentSong));
            firstTask.Start();
            Task.WaitAll(firstTask,secondTask);
        }
        private void StartPlayingSong(Song song)
        {
            var playedSongDuration = (int) song.Duration.TotalSeconds;
            Console.WriteLine(song.Name+" is playing now. ");
            while (playedSongDuration != 0)
            {
                Console.Write("*"); 
                Thread.Sleep(100);
                playedSongDuration--;
            }
            Console.WriteLine();
        }
        private void EndPlayingSong(Song song)
        {
            Console.WriteLine(song.Name+" finishes playing. ");
        }
    }

    internal class Music
    {
        private static Song CreateSong()
        {
            Console.WriteLine("Enter the name of a new song. ");
            string name;
            while (true)
            {
                if (String.IsNullOrWhiteSpace(name = Console.ReadLine()))
                {
                    Console.WriteLine("A bad value was entered, enter it again, please. ");
                    continue;
                }
                break;
            }
            Console.WriteLine("Enter the duration (just minutes or minutes:seconds) of a new song. ");
            TimeSpan duration;
            while (true)
            {
                try
                {
                    duration = TimeSpan.Parse("00:" + Console.ReadLine());
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("A bad value was entered, enter it again, please. ");
                }
            }
            Console.WriteLine("Enter the location of a new song. ");
            string location;
            while (true)
            {
                if (!String.IsNullOrWhiteSpace(location=@Console.ReadLine()))
                {
                    break;
                }
                Console.WriteLine("A bad value was entered, enter it again, please. ");
            }
            Console.WriteLine("Enter the genres (separated by comma) of a new song. (");
            foreach (var g in Enum.GetValues(typeof(Song.AllGenres)))
            {
                Console.Write(g.ToString()+" ");
            }
            Console.Write(")");
            Console.WriteLine();
            while (true)
            {
                var inputString = Console.ReadLine().Trim().Split(',');
                for (int i = 0; i < inputString.Length;)
                {
                    try
                    {
                        var inputGenre = (Song.AllGenres)Enum.Parse(typeof(Song.AllGenres),inputString[i]);
                        string temporaryGenres = (i == inputString.Length - 1) ? inputString[i] + ", " : inputString[i];
                        i++;
                        if (i == inputString.Length)
                        {
                            var inputGenres = new Song.AllGenres[inputString.Length];
                            for (int j = 0; j < inputString.Length; j++)
                            {
                                inputGenres[j] = (Song.AllGenres)Enum.Parse(typeof(Song.AllGenres), inputString[j]);
                            }
                            var song = new Song(name, duration, location, inputGenres);
                            return song;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("A {0} value isn`t allowed, enter it again, please. Letters are case, spaces are ignored. ", i + 1);
                        inputString[i] = Console.ReadLine().Trim();
                        continue;
                    }
                }
                break;
            }
            return null;
        }
        private static Playlist CreatePlaylist(List<Song> songs)
        {
            Console.WriteLine("Enter the name of a new playlist. ");
            string name;
            while (true)
            {
                if (String.IsNullOrWhiteSpace(name = Console.ReadLine()))
                {
                    Console.WriteLine("A bad value was entered, enter it again, please. ");
                    continue;
                }
                break;
            }
            while (true)
            {
                if (songs == null)
                {
                    Console.WriteLine("A bad value was entered, enter it again, please. ");
                    continue;
                }
                break;
            }
            return new Playlist(name,songs);
        }

        static void Main(string[] args)
        {
            var allSongs = new List<Song>();
            var allPlaylists = new List<Playlist>();
            var player = new Player();
            while (true)
            {
                Console.WriteLine("Print 1 for creating a new song. ");
                Console.WriteLine("Print 2 for creating a new playlist. ");
                Console.WriteLine("Print 3 to see all the songs. ");
                Console.WriteLine("Print 4 to see all the songs of playlist. ");
                Console.WriteLine("Print 5 to play all the songs of playlist orderly. ");
                Console.WriteLine("Print 6 to play all the songs of playlist randomly. ");
                Console.WriteLine();
                int choice;
                Int32.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        {
                            allSongs.Add(CreateSong());
                            break;
                        }
                    case 2:
                        {
                            allPlaylists.Add(CreatePlaylist(allSongs));
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("id   name    duration  location");
                            foreach (var s in allSongs)
                            {
                                Console.WriteLine(s.Id+" "+s.Name+" "+s.Duration+" "+s.Location);
                            }
                            break;
                        }
                    case 4:
                        {
                            allPlaylists[0].ShowSongsInfo();
                            break;
                        }
                    case 5:
                        {
                            player.PlayOrderly(allPlaylists[0]);
                            break;
                        }
                    case 6:
                        {
                            player.PlayRandomly(allPlaylists[0]);
                            break;
                        }
                }
                Console.WriteLine("Press 'esc' to exit or ant key to continue. ");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
    }
}
