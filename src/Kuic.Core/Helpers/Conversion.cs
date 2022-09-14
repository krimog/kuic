using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Kuic.Core.Helpers
{
    public static class Conversion
    {
        private static readonly HashSet<string> _boolTrueValues = new(StringComparer.InvariantCultureIgnoreCase) { "1", "true", "yes" };
        private static readonly HashSet<string> _boolFalseValues = new(StringComparer.InvariantCultureIgnoreCase) { "0", "false", "no" };

        public static bool TryParse<T>(string input, out T? value)
        {
            var result = TryParse(input, typeof(T), out var objValue);
            value = (T?)objValue;
            return result;
        }

        public static bool TryParse<T>(string input, IFormatProvider formatProvider, out T? value)
        {
            var result = TryParse(input, typeof(T), formatProvider, out var objValue);
            value = (T?)objValue;
            return result;
        }

        public static bool TryParse(string? input, Type type, IFormatProvider formatProvider, out object? value)
        {
            value = type.IsValueType ? Activator.CreateInstance(type) : null;

            if (type == typeof(string))
            { value = input; return true; }
            if (input == string.Empty)
                input = null;
            if (input == null && type.IsValueType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return true;
                return false;
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                if (bool.TryParse(input, out var result))
                { value = result; return true; }
                if (_boolTrueValues.Contains(input ?? string.Empty))
                { value = true; return true; }
                if (_boolFalseValues.Contains(input ?? string.Empty))
                { value = false; return true; }
                return false;
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                if (byte.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                if (sbyte.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                if (short.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(ushort) || type == typeof(ushort?))
            {
                if (ushort.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                if (int.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(uint) || type == typeof(uint?))
            {
                if (uint.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                if (long.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(ulong) || type == typeof(ulong?))
            {
                if (ulong.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                if (float.TryParse(input, NumberStyles.Float, formatProvider, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                if (double.TryParse(input, NumberStyles.Float, formatProvider, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                if (decimal.TryParse(input, NumberStyles.Float, formatProvider, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                if (DateTime.TryParse(input, formatProvider, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                if (DateTimeOffset.TryParse(input, formatProvider, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                if (Guid.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type.IsEnum || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum))
            {
                try
                {
                    value = Enum.Parse(type.IsEnum ? type : type.GetGenericArguments()[0], input ?? string.Empty);
                    return true;
                }
                catch (Exception) { return false; }
            }
            if (TryParseWithRegex(/* {Prop1=val1,Prop2=val2} */ new Regex(@"^\{\w+=[\w\.]+(,\w+=[\w\.]+)*\}$"), new Regex(@"(?<Prop>\w+)=(?<Value>[\w\.]+)"), type, input ?? string.Empty, out var parsedVar))
            {
                value = parsedVar;
                return true;
            }

            return false;
        }

        public static bool TryParse(string? input, Type type, out object? value)
        {
            value = type.IsValueType ? Activator.CreateInstance(type) : null;

            if (type == typeof(string))
            { value = input; return true; }
            if (input == string.Empty)
                input = null;
            if (input == null && type.IsValueType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return true;
                return false;
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                if (bool.TryParse(input, out var result))
                { value = result; return true; }
                if (_boolTrueValues.Contains(input ?? string.Empty))
                { value = true; return true; }
                if (_boolFalseValues.Contains(input ?? string.Empty))
                { value = false; return true; }
                return false;
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                if (byte.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                if (sbyte.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                if (short.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(ushort) || type == typeof(ushort?))
            {
                if (ushort.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                if (int.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(uint) || type == typeof(uint?))
            {
                if (uint.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                if (long.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(ulong) || type == typeof(ulong?))
            {
                if (ulong.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                if (float.TryParse(input, out var result))
                { value = result; return true; }
                if (Thread.CurrentThread.CurrentCulture != new CultureInfo("en-US") && float.TryParse(input, NumberStyles.Float, new CultureInfo("en-US"), out var result2))
                { value = result2; return true; }
                return false;
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                if (double.TryParse(input, out var result))
                { value = result; return true; }
                if (Thread.CurrentThread.CurrentCulture != new CultureInfo("en-US") && double.TryParse(input, NumberStyles.Float, new CultureInfo("en-US"), out var result2))
                { value = result2; return true; }
                return false;
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                if (decimal.TryParse(input, out var result))
                { value = result; return true; }
                if (Thread.CurrentThread.CurrentCulture != new CultureInfo("en-US") && decimal.TryParse(input, NumberStyles.Float, new CultureInfo("en-US"), out var result2))
                { value = result2; return true; }
                return false;
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                if (DateTime.TryParse(input, out var result))
                { value = result; return true; }
                if (DateTime.TryParseExact(input, "yyyyMMddTHHmmss.fff GMT", new CultureInfo("en-US"), DateTimeStyles.None, out result))
                { value = result; return true; }
                return false;
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                if (Guid.TryParse(input, out var result))
                { value = result; return true; }
                return false;
            }
            if (type.IsEnum || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum))
            {
                try
                {
                    value = Enum.Parse(type.IsEnum ? type : type.GetGenericArguments()[0], input ?? string.Empty);
                    return true;
                }
                catch (Exception) { return false; }
            }
            if (TryParseWithRegex(/* {Prop1=val1,Prop2=val2} */ new Regex(@"^\{\w+=[\w\.]+(,\w+=[\w\.]+)*\}$"), new Regex(@"(?<Prop>\w+)=(?<Value>[\w\.]+)"), type, input ?? string.Empty, out var parsedVar))
            {
                value = parsedVar;
                return true;
            }

            return false;
        }

        private static bool TryParseWithRegex(Regex stringRegex, Regex propValueRegex, Type type, string input, out object? value)
        {
            value = type.IsValueType ? Activator.CreateInstance(type) : null;
            if (!stringRegex.IsMatch(input)) return false;
            var propsValues = propValueRegex.Matches(input).ToDictionary(m => m.Groups["Prop"].Value, m => m.Groups["Value"].Value, StringComparer.OrdinalIgnoreCase);

            var constructor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == propsValues.Count && c.GetParameters().All(p => propsValues.ContainsKey(p.Name!)));
            if (constructor is null) return false;

            object?[] parameters = new object[constructor.GetParameters().Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var currentParam = constructor.GetParameters()[i];
                if (TryParse(propsValues[currentParam.Name!], currentParam.ParameterType, out var parsedValue))
                {
                    parameters[i] = parsedValue;
                }
                else
                {
                    return false;
                }
            }

            value = constructor.Invoke(parameters);
            return true;
        }
    }
}
