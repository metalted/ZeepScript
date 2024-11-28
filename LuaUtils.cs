using MoonSharp.Interpreter;
using System;
using System.Linq;

namespace ZeepScript
{
    public static class LuaUtils
    {
        public static int[] ConvertLuaTableToIntArray(DynValue tableValue)
        {
            if (tableValue.Type != DataType.Table)
            {
                Plugin.Instance.Log("Expected a table for int array conversion.");
                return null;
            }

            Table luaTable = tableValue.Table;
            int[] intArray = luaTable.Values
                .Where(v => v.Type == DataType.Number && v.Number % 1 == 0) // Ensure values are integers
                .Select(v => (int)v.Number)
                .ToArray();

            if (intArray.Length != luaTable.Length)
            {
                Plugin.Instance.Log("Table contains non-integer or invalid values.");
                return null;
            }

            return intArray;
        }

        public static bool ValidateParameterType(DynValue arg, DataType expectedType, string parameterName)
        {
            if (arg.Type != expectedType)
            {
                Plugin.Instance.Log($"{parameterName}: Expected {expectedType}, but got {arg.Type}.");
                return false;
            }
            return true;
        }

        public static T ConvertParameter<T>(DynValue arg, DataType expectedType, string parameterName, Func<DynValue, T> conversion)
        {
            if (!ValidateParameterType(arg, expectedType, parameterName))
            {
                return default;
            }

            try
            {
                return conversion(arg);
            }
            catch (Exception e)
            {
                Plugin.Instance.Log($"{parameterName}: Conversion error - {e.Message}");
                return default;
            }
        }
    }
}
