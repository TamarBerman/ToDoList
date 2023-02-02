using System.Collections.Generic;
using System.Text.Json;
using TaskList.Interfaces;
using System.Linq;
using System.IO;
using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using TaskList.Models;



namespace TaskList.Services
{
    public class UserService : IUserService
    {
        List<User>? users { get; }
        private IWebHostEnvironment webHost;
        private string filePath;

        public UserService(IWebHostEnvironment webHost)
        {
            this.webHost = webHost;
            this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "User.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            }
        }
        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(users));
        }

        public List<User> GetAll() => users;

        public User Get(int id) => users.FirstOrDefault(t => t.Id == id);

        public User GetCurrentUser(string token)
        {
            var id = Convert.ToInt32(TokenService.Decode(token));
            return Get(id);
        }
        public void Add(User user)
        {
            foreach (User myUser in this.users)
            {
                if (user.Id < myUser.Id)
                    user.Id = myUser.Id;
            }
            user.Id ++;
            users.Add(user);
            saveToFile();
        }

        public void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                return;
            users.Remove(user);
            saveToFile();
        }
        public int Count => users.Count();

    }

}
