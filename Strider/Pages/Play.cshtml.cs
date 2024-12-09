using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Security.Cryptography;

public class PlayModel : PageModel //set playmodel class
   {
    //creates instances of classes
       public List<SelectListItem> PlayerList { get; set; }
       public List<SelectListItem> EnemyList { get; set; }
       public Player SelectedPlayer { get; set; }
       public Enemy SelectedEnemy {get; set;}
       public pAbility SelectedAbility {get; set;}
       public pAbility SelectedAbility2 {get; set;}
       public pStat SelectedpStat {get; set;}
       public mStat SelectedmStat {get; set;}

       public void Versus() //generates a random enemy opponent 
       {
            Random rnd = new Random();
            int x = rnd.Next(1,8); //set variable x = random number from 1-8
            SelectedEnemy = GetEnemyByMon_ID(x); //calls function using x as id value parameter
       }

        public void LoadEnemyList() //loads enemies from the monsters table on strider database
       {
           EnemyList = new List<SelectListItem>();
           using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT Mon_ID, Mon_Type FROM Monsters";
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       EnemyList.Add(new SelectListItem
                       {
                           Value = reader.GetInt32(0).ToString(),
                           Text = reader.GetString(1)
                       });
                   }
               }
           }
       }

    public Enemy GetEnemyByMon_ID(int id) //gets enemy info based on int value id
       {
           using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT * FROM Monsters WHERE Mon_ID = @Mon_ID";
               command.Parameters.AddWithValue("@Mon_ID", id);
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new Enemy
                       {
                           Mon_ID = reader.GetInt32(0),
                           Mon_Type = reader.GetString(1),
                           Mon_Description = reader.GetString(2),
                           ImageFileName = reader.GetString(3)
                       };
                   }
               }
           }
           return null;
       }
   

       public void OnGet() //used to load class options
       {
           LoadPlayerList();
       }

       public void OnPost(string selectedPlayer) //run when player chooses a class
       {
           LoadPlayerList();
           if (!string.IsNullOrEmpty(selectedPlayer))
           {
               SelectedPlayer = GetPlayerByadv_ID(int.Parse(selectedPlayer)); //set Selected player instance to chosen class
               SelectedAbility = GetAbilityByadv_ID(int.Parse(selectedPlayer)); //set ability to chosen class
               SelectedAbility2 = GetAbility2Byadv_ID(int.Parse(selectedPlayer)); //set ability2 to chosen class
           }
           Versus(); //run versus function to populate opponent
       }

       public void LoadPlayerList() //loads class options for player from Adventurers table in the strider database
       {
           PlayerList = new List<SelectListItem>();
           using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT adv_ID, adv_Type FROM Adventurers";
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       PlayerList.Add(new SelectListItem
                       {
                           Value = reader.GetInt32(0).ToString(),
                           Text = reader.GetString(1)
                       });
                   }
               }
           }
       }

       public Player GetPlayerByadv_ID(int id) //gets the class info based on unique adventurer id found from player list
       {
           using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT * FROM Adventurers WHERE adv_ID = @adv_ID";
               command.Parameters.AddWithValue("@adv_ID", id);
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new Player
                       {
                           adv_ID = reader.GetInt32(0),
                           adv_Type = reader.GetString(1),
                           adv_Race = reader.GetString(2),
                           ImageFileName = reader.GetString(3)
                       };
                   }
               }
           }
           return null;
       }
       public pAbility GetAbilityByadv_ID(int id) //sets players first ability
       {
            using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               //inner join statement using adventurer ids from both tables
               command.CommandText = "SELECT a.* FROM Abilities AS a INNER JOIN Adventurers as t ON a.adv_ID = t.adv_ID WHERE t.adv_ID = @adv_ID";
               command.Parameters.AddWithValue("@adv_ID", id);
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new pAbility
                       {
                           Ability_ID = reader.GetInt32(0),
                           Action = reader.GetString(1),
                           Damage = reader.GetInt32(2),
                           adv_ID = reader.GetInt32(3),
                           Mon_ID = reader.GetInt32(4)
                       };
                   }
               }
           }
           return null;
       }

        public pAbility GetAbility2Byadv_ID(int id) //sets players second ability
       {
            using (var connection = new SqliteConnection("Data Source=Strider.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT a.* FROM Abilities AS a INNER JOIN Adventurers as t ON a.adv_ID = t.adv_ID WHERE t.adv_ID = @adv_ID ORDER BY a.Ability_ID DESC";
               command.Parameters.AddWithValue("@adv_ID", id);
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new pAbility
                       {
                           Ability_ID = reader.GetInt32(0),
                           Action = reader.GetString(1),
                           Damage = reader.GetInt32(2),
                           adv_ID = reader.GetInt32(3),
                           Mon_ID = reader.GetInt32(4)
                       };
                   }
               }
           }
           return null;
       }
       public void OnAttack() //used when first ability is clicked
       {

       }

       public void OnAttack2() //used when second ability is clicked
       {

       }
   }

   public class Player
   {
       public int adv_ID { get; set; }
       public string adv_Type { get; set; }
       public string adv_Race { get; set; }
       public string ImageFileName { get; set; }
   }

   public class Enemy
   {
        public int Mon_ID { get; set; }
       public string Mon_Type { get; set; }
       public string Mon_Description { get; set; }
       public string ImageFileName { get; set; }
   }

   public class pAbility
   {
        public int Ability_ID {get; set;}
        public string Action {get; set;}
        public int Damage {get; set;}
        public int adv_ID {get;set;}
        public int Mon_ID {get; set;}
   }

      public class mAbility
   {
        public int Ability_ID {get; set;}
        public string Action {get; set;}
        public int Damage {get; set;}
        public int adv_ID {get;set;}
        public int Mon_ID {get; set;}
   }

   public class pStat
   {
        public int Stat_ID {get; set;}
        public int Health {get; set;}
        public int Speed {get; set;}
        public int Exp {get; set;}
        public int adv_ID {get; set;}
        public int Mon_ID {get; set;}
   }

   public class mStat
   {
        public int Stat_ID {get; set;}
        public int Health {get; set;}
        public int Speed {get; set;}
        public int Exp {get; set;}
        public int adv_ID {get; set;}
        public int Mon_ID {get; set;}
   }

