using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace fyp1_prototype
{
	//	Class for table data / table columns
	//	Player class
	public class Player
	{
		[Key]
		[Column("ID")]
		public int Id { get; set; }

		[Column("username")]
		public string Username { get; set; }

		[Column("score")]
		public int Score { get; set; }

		[Column("date")]
		public string Date { get; set; }

		[Column("Password")]
		public string Password { get; set; }
	}

	//	Game class
	public class Game
	{
		[Key]
		[Column("ID")]
		public int Id { get; set; }

		[Column("lives")]
		public int Lives{ get; set; }

		[Column("playerGame")]
		public int PlayerGame{ get; set; }

		[Column("datetime")]
		public string DateTime { get; set; }

		[Column("time")]
		public string Time { get; set; }

		[Column("score")]
		public int Score { get; set; }

		[Column("itemGame")]
		public int ItemGame { get; set; }
	}

	//	Score class
	public class Score
	{
		[Key]
		[Column("ID")]
		public int Id { get; set; }

		[Column("value")]
		public int Value { get; set; }

		[Column("datetime")]
		public string DateTime { get; set; }

		[Column("playerScore")]
		public int PlayerScore { get; set; }
	}

	//	Item class
	public class Items
	{
		[Column("ID")]
		public int Id { get; set; }

		[Column("item_image_link")]
		public string ItemImageLink { get; set; }

		[Column("item_type")]
		public int ItemType { get; set; }
	}

	//	DbContext class to access tables
	public class DatabaseContext : DbContext
	{
		public DbSet<Player> Players { get; set; }
		public DbSet<Game> Game { get; set; }
		public DbSet<Score> Score { get; set; }
		public DbSet<Item> Item { get; set; }
	}

	//	Player class with public functions to interact with DbContext
	public class PlayersRepository
	{
		//	Constructor
		public PlayersRepository()
		{

		}

		//	Data transfer object (To save all column data)
		public class PlayerNameDto
		{
			public int Id { get; set; }
			public string Username { get; set; }
			public int Score { get; set; }
			public string Date { get; set; }
		}

		//	Get all players
		public List<Player> GetAllPlayers()
		{
			DatabaseContext dc = new DatabaseContext();
			dc.Database.Log = Console.WriteLine;
			return dc.Players.ToList();
		}

		//	Get player with specific ID
		public List<Player> GetPlayerWithId(int id)
		{
			DatabaseContext dc = new DatabaseContext();
			return dc
				.Players
				.Where(player => player.Id == 123)
				.ToList();
		}

		//	Get specific data of players with Data Transfer Object
		public List<PlayerNameDto> GetAllPlayersName()
		{
			DatabaseContext dc = new DatabaseContext();
			return dc.Players
				.Select(player => new PlayerNameDto()
				{
					Id = player.Id,
					Username = player.Username,
					Score = player.Score
				})
				.ToList();
		}

		//	Add player to table
		public void AddPlayer(string name)
		{
			DatabaseContext dc = new DatabaseContext();
			dc.Players.Add(new Player()
			{
				Username = name,
				Date = "2018-01-15",
				Password = "123456"
			});
			dc.SaveChanges();
		}

		//	Modify a player's data
		public void ModifyPlayer()
		{
			DatabaseContext dc = new DatabaseContext();
			var user = dc.Players
				.FirstOrDefault(u => u.Id == 1);
			user.Score = 100;
			dc.SaveChanges();
		}

		//	Modify players' data
		public void ModifyPlayers()
		{
			DatabaseContext dc = new DatabaseContext();
			var players = dc.Players
				.Where(u => u.Score > 0)
				.ToList();

			foreach (var player in players)
			{
				player.Score = 99999;
			}
			dc.SaveChanges();
		}
	}

	//	Game class with public functions to interact with DbContext
	public class GameRepository
	{

	}

	//	Score class with public functions to interact with DbContext
	public class ScoreRepository
	{

	}

	//	Item class with public functions to interact with DbContext
	public class ItemsRepository
	{

	}
}
