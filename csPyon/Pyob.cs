using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace csPyon
{
    public class Pyob
    {
        public string head;
        public object[] ordered;
        public Hashtable keyed;
        public Pyob(string head)
        {
            if (head == null || head.Length == 0)
                throw new Exception();
            this.head = head;
            this.ordered = new object[0];
            this.keyed = new Hashtable();
        }
        public Pyob(string head, object[] ordered, Hashtable keyed)
        {
            if (head == null || head.Length == 0)
                throw new Exception();
            this.head = head;
            this.ordered = ordered;
            this.keyed = keyed;
            if (this.ordered == null) this.ordered = new object[0];
            if (this.keyed == null) this.keyed = new Hashtable();
        }
        public static string Repr(object thing)
        {
            StringBuilder builder = new StringBuilder();
            AddRepr(thing, builder);
            return builder.ToString();
        }

        private static void AddRepr(object thing, StringBuilder builder)
        {
            bool hit = false;
            if (thing == null)
            {
                builder.Append("null");
            }
            else if (thing.GetType() == typeof(string))
            {
                builder.Append("'");
                builder.Append(thing);
                builder.Append("'");
            }
            else if (thing.GetType().IsArray)
            {
                object[] arr = (object[])thing;
                builder.Append("[");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (hit) builder.Append(",");
                    hit = true;
                    AddRepr(arr[i], builder);
                }
                builder.Append("]");
            }
            else if (thing.GetType() == typeof(Hashtable))
            {
                Hashtable hashTable = (Hashtable)thing;
                List<string> keys = new List<string>();
                foreach (object obj in hashTable.Keys)
                    keys.Add(obj.ToString());
                keys.Sort();
                builder.Append("{");
                foreach (string key in keys)
                {
                    if (hit) builder.Append(",");
                    hit = true;
                    builder.Append("'");
                    builder.Append(key);
                    builder.Append("':");
                    AddRepr(hashTable[key], builder);
                }
                builder.Append("}");
            }
            else if (thing.GetType().IsPrimitive)
            {
                builder.Append(thing.ToString());
            }
            else
                throw new Exception();
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.head);
            builder.Append("(");
            bool stuff = false;
            for (int i = 0; i < ordered.Length; i++)
            {
                if (stuff) builder.Append(",");
                builder.Append(Repr(ordered[i]));
                stuff = true;
            }
            foreach (DictionaryEntry entry in keyed)
            {
                if (stuff) builder.Append(",");
                builder.Append(entry.Key.ToString());
                builder.Append("=");
                builder.Append(Repr(entry.Value));
                stuff = true;
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}