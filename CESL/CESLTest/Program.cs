#region vertex shader parsing testing

/*string vertexShaderSource = @"#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec4 aTangent;

namespace TestNameSpace;

class TestVertex
{
    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;

    out vec3 Normal;
    out vec3 FragPos;

    void main()
    {
        FragPos = vec3(model * vec4(aPosition, 1.0));
        Normal = mat3(transpose(inverse(model))) * aNormal;

        gl_Position = projection * view * vec4(FragPos, 1.0);
    }
}";

var vert = CESL.CESL.ParseVertexShader(vertexShaderSource);

Console.WriteLine($"namespace: {vert.Namespace}");
Console.WriteLine($"class: {vert.ClassName}");

Console.WriteLine($"GLSL:\n{vert.GLSL}\n");*/

#endregion

#region fragment shader parsing testing

string fragmentShaderSource = @"
namespace TestNameSpace;

class TestClass
{
    /*
    private int testPrivateInt;

    comment body test
    */

    [Attribute]
    /*
    attribute spacing text
    */
    // another attribute spacing text
    private float testPrivateFloat; /*
    
    private sampler2D testPrivateImage;

    another comment body test
    */

    // single line comment test
    [Header(Test Header)]
    [Range(0, 1, 1)]
    public int testPublicInt; // multi line comment test

    void main()
    {
        // main body
    }

    void main12()
    {
        // main body
    }
}";

var frag = CESL.CESL.ParseFragmentShader(fragmentShaderSource);

Console.WriteLine($"namespace: {frag.Namespace}");
Console.WriteLine($"class: {frag.ClassName}");

foreach (var field in frag.Fields)
{
    Console.WriteLine(field.ToString());

    foreach (var attr in field.Attributes)
    {
        var attrib = CESL.CESL.ParseAttribute(attr.Line);

        var attrargs = string.Join(", ", attrib.args);

        Console.WriteLine($"Attribute: {attr.Name}, args: {attrargs}");
    }
}

Console.WriteLine($"GLSL:\n{frag.GLSL}\n");

#endregion

#region to CESL convertion testing

/*
// make a small test vertex shader with glsl
var glslVertexSource = @"#version 330 core

layout (location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
}";

var ceslVertex = CESL.CESL.ToCESLVertexShader(glslVertexSource);

Console.WriteLine($"CESL: {ceslVertex}");

Console.WriteLine("-----------------------------");

var vert = CESL.CESL.ParseVertexShader(ceslVertex);

Console.WriteLine($"namespace: {vert.Namespace}");
Console.WriteLine($"class: {vert.ClassName}");

Console.WriteLine($"GLSL:\n{vert.GLSL}\n");
*/

#endregion

// dont close immediately
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
