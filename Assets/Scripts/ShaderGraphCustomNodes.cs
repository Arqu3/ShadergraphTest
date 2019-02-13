using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEditor;
using System.Reflection;

[Title("Custom", "Refract")]
public class Refract : CodeFunctionNode
{
    public Refract()
    {
        name = "Refract";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("refract", BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string refract(
        [Slot(0, Binding.None)] DynamicDimensionVector viewDirection,
        [Slot(1, Binding.None)] DynamicDimensionVector objectNormal,
        [Slot(2, Binding.None)] Vector1 scalar,
        [Slot(3, Binding.None)] out DynamicDimensionVector Out
        )
    {
        return @"
{ 
    Out = refract(normalize(viewDirection), normalize(objectNormal), 1 / scalar);
}";
    }
}
