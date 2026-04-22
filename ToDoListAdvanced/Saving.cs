using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace ToDoListAdvanced
{
    public class Saving
    {
        public static async Task Save(ObservableCollection<ToDoTask> Tasks) 
        {
            string filename = Path.Combine(FileSystem.AppDataDirectory, "ToDoListAdvanced.json");
            await using FileStream createStream = File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, Tasks);
        }
        public static async Task<ObservableCollection<ToDoTask>?> Load()
        {
            string filename = Path.Combine(FileSystem.AppDataDirectory, "ToDoListAdvanced.json");

            if (!File.Exists(filename))
                return new ObservableCollection<ToDoTask>();

            try
            {
                using FileStream openStream = File.OpenRead(filename);
                ObservableCollection<ToDoTask>? Tasks = await JsonSerializer.DeserializeAsync<ObservableCollection<ToDoTask>>(openStream);
                return Tasks ?? new ObservableCollection<ToDoTask>();
            }
            catch
            {
                return new ObservableCollection<ToDoTask>();
            }
        }
    }
}
