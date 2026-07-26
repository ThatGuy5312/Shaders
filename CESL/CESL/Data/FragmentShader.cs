using OpenTK.Mathematics;
using System;

namespace CESL.Data;

public struct FragmentShader
{
    public string GLSL { get; set; }

    public string Namespace { get; set; }
    public string ClassName { get; set; }

    public List<ShaderField> Fields;

    public readonly ShaderField FindShaderField(string name) => Fields.FirstOrDefault(f => f.Name == name);

    public static void SetUniform(CEShader shader, ShaderField field, object value, int index = 0)
    {
        if (value == null)
            return;

        switch (field.Type)
        {
            case "bool":
                shader.SetBool(field.Name, (bool)value);
                break;
            case "int":
                shader.SetInt(field.Name, (int)value);
                break;
            case "float":
                shader.SetFloat(field.Name, (float)value);
                break;
            case "vec2":
                shader.SetVector2(field.Name, (Vector2)value);
                break;
            case "vec3":
                shader.SetVector3(field.Name, (Vector3)value);
                break;
            case "vec4":
                shader.SetVector4(field.Name, (Vector4)value);
                break;
            case "mat4":
                shader.SetMatrix4(field.Name, (Matrix4)value);
                break;
            case "sampler2D":
                //((Texture2D)material.Values[field.name]).Bind(GetTextureUnit(index));
                shader.SetInt(field.Name, index);
                break;
        }
    }
}
