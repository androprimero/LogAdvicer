using System;
using System.Collections.Generic;
using System.Text;

namespace LogAdvicer
{
    public class MethodMetrics
    {
        bool HasTry;
        bool HasIf;
        Dictionary<String, bool> ifValues;
        Dictionary<String, bool> elseValues;
        Dictionary<String, int> tryValues;
        Dictionary<String, bool> catchValues;
        public MethodMetrics()
        {
            HasIf = false;
            HasTry = false;
            ifValues = new Dictionary<String, bool>();
            elseValues = new Dictionary<String, bool>();
            tryValues = new Dictionary<String, int>();
            catchValues = new Dictionary<String, bool>();
        }

        public void SetHasIf(bool hasif)
        {
            HasIf = hasif;
        }

        public void SetHasTry(bool hastry)
        {
            HasTry = hastry;
        }

        public bool MethodHasIf()
        {
            return HasIf;
        }

        public bool MethodHasTry()
        {
            return HasTry;
        }

        public void AddTryValues(String key)
        {
            if (tryValues.ContainsKey(key))
            {
                tryValues[key]++;
            }
            else
            {
                tryValues.Add(key, 1);
            }
        }

        public void AddTryValues(String key, int Value)
        {
            if (tryValues.ContainsKey(key))
            {
                tryValues[key] += Value;
            }
            else
            {
                tryValues.Add(key, Value);
            }
        }

        public void AddIfValues(String key)
        {
            if (!ifValues.ContainsKey(key)) // catch exceptions that are logged
            {
                ifValues.Add(key, true);
            }
        }

        public void AddIfValues(String key, bool Value)
        {
            if (!ifValues.ContainsKey(key))
            {
                ifValues.Add(key, Value);
            }
        }
        public void AddCatchValues(String key)
        {
            if (!catchValues.ContainsKey(key)) // catch exceptions that are logged
            {
                catchValues.Add(key, true);
            }
        }

        public void AddCatchValues(String key, bool Value)
        {
            if (!catchValues.ContainsKey(key))
            {
                catchValues.Add(key, Value);
            }
        }

        public void AddElseValues(String key)
        {
            if (!elseValues.ContainsKey(key))
            {
                elseValues.Add(key, true);
            }
        }
        public void AddElseValues(String key, bool Value)
        {
            if (elseValues.ContainsKey(key))
            {
                elseValues[key] = Value;
            }
            else
            {
                elseValues.Add(key, Value);
            }
        }

        public List<String> GetTryKeys()
        {
            List<String> keys = new List<String>();
            foreach(var key in tryValues.Keys)
            {
                keys.Add(key);
            }
            return keys;
        }
        public List<String> GetIfKeys()
        {
            List<String> keys = new List<String>();
            foreach (var key in ifValues.Keys)
            {
                keys.Add(key);
            }
            return keys;
        }
        public List<String> GetCatchKeys()
        {
            List<String> keys = new List<String>();
            foreach (var key in catchValues.Keys)
            {
                keys.Add(key);
            }
            return keys;
        }
        public List<String> GetElseKeys()
        {
            List<String> keys = new List<String>();
            foreach (var key in elseValues.Keys)
            {
                keys.Add(key);
            }
            return keys;
        }
        public int GetTryValue(String key)
        {
            return tryValues[key];
        }

        public bool GetCatchValue(String Key)
        {
            return catchValues[Key];
        }

        public bool GetIfValue(String Key)
        {
            return ifValues[Key];
        }
        public bool GetElseValue(String key)
        {
            return elseValues[key];
        }
    }
}
