using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Metroidvania.Session10
{
    [Serializable]
    public sealed class MetroidvaniaSaveData
    {
        public string CurrentScene = "start_area";
        public string LastCheckpointId = "cp_start";
        public List<string> UnlockedAbilities = new();
        public List<string> ActivatedCheckpoints = new();
        public string LocaleCode = "ko";
        public float TotalPlayTime;
    }

    public interface ISaveService
    {
        void Save(MetroidvaniaSaveData data);
        MetroidvaniaSaveData Load();
        bool HasSave();
    }

    public sealed class MetroidvaniaJsonSaveService : ISaveService
    {
        private readonly string _savePath;

        public MetroidvaniaJsonSaveService(string fileName = "metroidvania_save.json")
        {
            _savePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public void Save(MetroidvaniaSaveData data)
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
        }

        public MetroidvaniaSaveData Load()
        {
            if (!HasSave())
            {
                return new MetroidvaniaSaveData();
            }

            var json = File.ReadAllText(_savePath);
            return JsonUtility.FromJson<MetroidvaniaSaveData>(json) ?? new MetroidvaniaSaveData();
        }

        public bool HasSave()
        {
            return File.Exists(_savePath);
        }
    }
}
