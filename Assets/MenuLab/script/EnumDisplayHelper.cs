using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Lê o texto de exibição (InspectorName, quando existir) de valores de enum,
/// incluindo enums [Flags] com múltiplos bits ativos ao mesmo tempo.
/// </summary>
public static class EnumDisplayHelper
{
    public static string GetDisplayName(Enum value)
    {
        Type type = value.GetType();
        string name = value.ToString();

        FieldInfo field = type.GetField(name);
        if (field != null)
        {
            InspectorNameAttribute attr = field.GetCustomAttribute<InspectorNameAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.displayName))
                return attr.displayName;
        }

        return name;
    }

    /// <summary>
    /// Para enums [Flags] com mais de um bit ativo, retorna os nomes de cada bit unidos por separator.
    /// </summary>
    public static string GetFlagsDisplayName(Enum flagsValue, string separator = "/")
    {
        Type type = flagsValue.GetType();
        long raw = Convert.ToInt64(flagsValue);

        if (raw == 0)
            return GetDisplayName(flagsValue);

        List<string> names = new List<string>();
        foreach (Enum bit in Enum.GetValues(type))
        {
            long bitValue = Convert.ToInt64(bit);
            if (bitValue == 0)
                continue;
            if ((raw & bitValue) == bitValue)
                names.Add(GetDisplayName(bit));
        }

        return names.Count > 0 ? string.Join(separator, names) : GetDisplayName(flagsValue);
    }

    /// <summary>
    /// Junta nomes no formato "A", "A e B" ou "A, B e C".
    /// </summary>
    public static string JoinNames(IReadOnlyList<string> names)
    {
        if (names == null || names.Count == 0)
            return string.Empty;
        if (names.Count == 1)
            return names[0];
        if (names.Count == 2)
            return $"{names[0]} e {names[1]}";

        return string.Join(", ", names.Take(names.Count - 1)) + " e " + names[names.Count - 1];
    }
}
