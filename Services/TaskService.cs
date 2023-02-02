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
    public class TaskService : ITaskService
    {
        List<MyTask>? tasks { get; }
        private IWebHostEnvironment webHost;
        private string filePath;
        public TaskService(IWebHostEnvironment webHost)
        {
            this.webHost = webHost;
            this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Task.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                tasks = JsonSerializer.Deserialize<List<MyTask>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(tasks));
        }

        public List<MyTask> GetAll(string token)
        {
            String idFromToken = TokenService.Decode(token);
            List<MyTask> tasksFilter = new List<MyTask>();
            foreach (MyTask task in tasks)
            {
                if (task.UserId.ToString() == idFromToken)
                    tasksFilter.Add(task);
            }
            return tasksFilter;
        }

        public MyTask Get(int id) => tasks.FirstOrDefault(t => t.Id == id);

        public void Add(MyTask task, string token)
        {
            String idFromToken = TokenService.Decode(token);

            foreach (MyTask myTask in this.tasks)
            {
                if (task.Id < myTask.Id)
                    task.Id = myTask.Id;
            }
            task.Id++;
            task.UserId = Convert.ToInt32(idFromToken);
            tasks.Add(task);
            saveToFile();
        }

        public void Delete(int id)
        {
            var task = Get(id);
            if (task is null)
                return;

            tasks.Remove(task);
            saveToFile();
        }

        public void Update(MyTask task)
        {
            var index = tasks.FindIndex(t => t.Id == task.Id);
            if (index == -1)
                return;

            tasks[index] = task;
            saveToFile();
        }

        public int Count => tasks.Count();
    }

}
