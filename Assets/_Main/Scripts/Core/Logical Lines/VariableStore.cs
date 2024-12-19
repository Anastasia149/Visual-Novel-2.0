using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class VariableStore
{
    private const string default_database_name = "Default";
    public const char database_variable_relational_id = '.';
    public static readonly string regex_variable_ids = @"[!]?\$[a-zA-Z0-9_.]+";
    public const char variable_id = '$';
    public const char update_id = '+';

    #region Variables
    public class Database
    {
        public Database(string name)
        {
            this.name = name;
            variables = new Dictionary<string, Variable>();
        }

        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
    }

    public abstract class  Variable
    {
        public abstract object Get();
        public abstract void Set(object value);

    }

    public class Variable<T> : Variable
    {
        private T value;

        private Func<T> getter;
        private Action<T> setter;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if (getter == null)
                this.getter = () => value;
            else
                this.getter = getter;


            if (setter == null)
                this.setter = newValue => value = newValue;
            else
                this.setter = setter;
        }
        public override object Get() => getter();

        public override void Set(object newValue) => setter((T)newValue);
    }
    #endregion

    public static Dictionary<string, Database> databases = new Dictionary<string, Database>() { { default_database_name, new Database(default_database_name) } };

    private static Database defaultDatabase => databases[default_database_name];

    #region Create database

    public static bool CreateDatabase(string name)
    {
        if (!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }

        return false;
    }

    #endregion

    #region Get Database
    public static Database GetDatabase(string name)
    {
        if (name == string.Empty)
            return defaultDatabase;

        if (!databases.ContainsKey(name))
            CreateDatabase(name);

        return databases[name];
    }

    #endregion

    #region Create Variable 
    public static bool CreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName] = new Variable<T>(defaultValue, getter, setter);

        return true;
    }
    #endregion

    #region Try Get Value
    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }
        variable = db.variables[variableName].Get();

        return true;
    }
    #endregion

    #region Try Set Value
    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName].Set(value);
        return true;
    }
    #endregion

    #region Remove Variables
    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(name))
            db.variables.Remove(variableName);
    }
    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[default_database_name] = new Database(default_database_name);
    }
    #endregion

    #region Extract Value
    private static (string[], Database, string) ExtractInfo(string name)
    {
        string[] parts = name.Split(database_variable_relational_id);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }
    #endregion

    public static bool HasVariable(string name)
    {
        string[] parts = name.Split(database_variable_relational_id);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return db.variables.ContainsKey(variableName);
    }

    #region Printing
    public static void PrintAllDatabases()
    {
        foreach (KeyValuePair<string, Database> dbEntry in databases)
        {
            Debug.Log($"Database: '<color=#FFB145>{dbEntry.Key}</color>'");
        }
    }

    public static void PrintAllVariables(Database database = null)
    {
        if (database != null)
        {
            PrintAllDatabaseVariables(database);
            return;
        }

        foreach (var dbEntry in databases)
        {
            PrintAllDatabaseVariables(dbEntry.Value);
        }
    }

    private static void PrintAllDatabaseVariables(Database database)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Database: <color=#F38544>{database.name}</color>");

        foreach (KeyValuePair<string, Variable> variablePair in database.variables)
        { 
            string variableName = variablePair.Key;
            object variableValue = variablePair.Value.Get();
            sb.AppendLine($"\t<color=#FFB145>Variable [{variableName}]</color> = <color=#FFD22D>{variableValue}</color>");
        }
        Debug.Log(sb.ToString());
    }
    #endregion
}
