﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class AccessoryPool
    {
        private Dictionary<string, Dictionary<string, string>> pool;

        public AccessoryPool()
        {
            pool = new Dictionary<string, Dictionary<string, string>>
            {
                { "Hair", new Dictionary<string, string>() }
            };
        }

        public string GetId(string slotId, string accessoryKey)
        {
            pool[slotId].TryGetValue(accessoryKey, out string id);
            return id;
        }

        public void AddId(string slotId, string accessoryKey, string accessoryId)
        {
            pool[slotId][accessoryKey] = accessoryId;
        }
    }
}
