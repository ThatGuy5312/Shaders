using CESL.Data;
using OpenTK.Mathematics;
using System.Text.RegularExpressions;

namespace CESL;

public static class CESL
{
    /// <summary>
    /// Parses a CESL vertex shader source code and converts it to GLSL format, returning a VertexShader struct containing the namespace, class name, and GLSL code.
    /// </summary>
    /// <param name="source">CESL vertex shader</param>
    /// <returns></returns>
    public static VertexShader ParseVertexShader(string source)
    {
        var vertex_data = new VertexShader();

        var lines = source.Split('\n');

        List<string> output = [];

        bool waitingForClassBrace = false;
        int classBraceDepth = 0;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            bool doNotAddLine = false;

            // check for namespace declaration
            if (line.StartsWith("namespace"))
            {
                var nsSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                vertex_data.Namespace = nsSplit[1].TrimEnd(';');
                doNotAddLine = true;
                continue;
            }

            // check for class declaration
            if (line.StartsWith("class"))
            {
                var classSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                vertex_data.ClassName = classSplit[1];
                doNotAddLine = true;
                waitingForClassBrace = true;
                continue;
            }

            // check for first brace after class declaration
            if (waitingForClassBrace && line == "{")
            {
                waitingForClassBrace = false;
                classBraceDepth = 1;
                continue;
            }

            // track brace depth
            if (classBraceDepth > 0)
            {
                foreach (char c in line)
                {
                    if (c == '{')
                        classBraceDepth++;
                    else if (c == '}')
                        classBraceDepth--;
                }

                // uninclude the last bracket
                if (classBraceDepth == 0 && line == "}")
                {
                    doNotAddLine = true;
                    continue;
                }
            }

            if (!doNotAddLine)
                output.Add(line);
        }

        vertex_data.GLSL = string.Join("\n", output);

        return vertex_data;
    }

    /// <summary>
    /// Parses a CESL fragment shader source code, extracts uniform fields and their attributes, 
    /// and converts it to GLSL format. Returns a FragmentShader struct containing the GLSL code and a list of 
    /// ShaderField structs representing the uniform fields and their attributes.
    /// </summary>
    /// <param name="source">CESL fragment shader</param>
    /// <returns></returns>
    public static FragmentShader ParseFragmentShader(string source)
    {
        var lines = source.Split('\n');

        var frag_data = new FragmentShader();
        frag_data.Fields = [];

        List<string> output = [];
        List<string> pendingAttributes = [];

        bool waitingForClassBrace = false;
        int classBraceDepth = 0;

        bool isInsideCommentBlock = false;
        bool stillCheckComment = false;

        // parse each line of the source code
        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.StartsWith("/*"))
                isInsideCommentBlock = true;

            if (line.Contains("/*") && !line.StartsWith("/*"))
            {
                stillCheckComment = true;
                isInsideCommentBlock = true;
            }

            if (line.EndsWith("*/"))
                isInsideCommentBlock = false;

            if (isInsideCommentBlock && !stillCheckComment)
            {
                output.Add(line);
                continue;
            }

            bool isComment = false;

            if (line.StartsWith("//"))
                isComment = true;

            if (line.Contains("//") && !line.StartsWith("//"))
            {
                stillCheckComment = true;
                isComment = true;
            }

            if (isComment && !stillCheckComment)
            {
                output.Add(line);

                continue;
            }

            bool doNotAddLine = false;

            // check for namespace declaration
            if (line.StartsWith("namespace"))
            {
                var nsSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                frag_data.Namespace = nsSplit[1].TrimEnd(';');
                doNotAddLine = true;
                stillCheckComment = false;
                continue;
            }

            // check for class declaration
            if (line.StartsWith("class"))
            {
                var classSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                frag_data.ClassName = classSplit[1];
                doNotAddLine = true;
                waitingForClassBrace = true;
                continue;
            }

            // check for first brace after class declaration
            if (waitingForClassBrace && line == "{")
            {
                waitingForClassBrace = false;
                classBraceDepth = 1;
                continue;
            }

            // track brace depth
            if (classBraceDepth > 0)
            {
                foreach (char c in line)
                {
                    if (c == '{')
                        classBraceDepth++;
                    else if (c == '}')
                        classBraceDepth--;
                }

                // uninclude the last bracket
                if (classBraceDepth == 0 && line == "}")
                {
                    doNotAddLine = true;
                    continue;
                }
            }

            // checking for a attribute
            if (line.StartsWith('['))
            {
                pendingAttributes.Add(line);

                stillCheckComment = false;

                continue;
            }

            bool skipUniform = false;

            // has to be done before its converted to uniform
            var isPrivate = line.StartsWith("private");

            // if it starts with uniform it wont contain public or private data
            if (line.StartsWith("uniform"))
                skipUniform = true;

            // convert the words public or private to uniform
            line = Regex.Replace(line, @"\b(public|private)\b", "uniform");

            // checking for a uniform
            if (line.StartsWith("uniform") && !skipUniform)
            {
                // changed this so it doesn't make a field for every line
                var field = new ShaderField();

                field.Attributes = [];

                field.IsPrivate = isPrivate;

                // process attributes before clearing
                if (pendingAttributes.Count > 0)
                {
                    foreach (var attr in pendingAttributes)
                    {
                        // glsl attribute indication
                        output.Add("// " + attr);

                        // create the attribute
                        var attribute = new FieldAttribute
                        {
                            Line = attr,
                            Name = ParseAttribute(attr).name
                        };

                        // add the attribute to the field
                        field.Attributes.Add(attribute);
                    }

                    pendingAttributes.Clear();
                }

                output.Add(line);

                var presplit = line;

                if (isComment)
                    presplit = line.Split("//")[0].Trim();

                // splits the field into segments
                var split = Regex.Split(presplit.Trim(), @"\s+|;");

                // finds the type
                field.Type = split[1];

                //if (!field.isPrivate)
                //field.value = CreateInstance(split[1]);

                // finds the name
                field.Name = split[2];

                // adds the field to the fragment data
                frag_data.Fields.Add(field);

                stillCheckComment = false;

                continue;
            }

            if (!doNotAddLine)
                output.Add(line);

            stillCheckComment = false;
        }

        frag_data.GLSL = string.Join("\n", output);

        return frag_data;
    }

    /// <summary>
    /// Parses a CESL attribute string (e.g. "[Range(0, 1, 0.1)]") and 
    /// extracts the attribute name and its arguments, 
    /// returning them as a tuple.
    /// </summary>
    /// <param name="attr">Attribute name and arguments</param>
    /// <returns></returns>
    public static (string name, List<string> args) ParseAttribute(string attr)
    {
        // remove [ and ]
        attr = attr.Trim('[', ']');

        var nameEnd = attr.IndexOf('(');

        if (nameEnd == -1)
        {
            return (attr.Trim(), new List<string>());
        }

        var name = attr[..nameEnd].Trim();

        var argsRaw = attr.Substring(nameEnd + 1, attr.LastIndexOf(')') - nameEnd - 1);

        var args = argsRaw
            .Split(',')
            .Select(a => a.Trim())
            .Where(a => a.Length > 0)
            .ToList();

        return (name, args);
    }

    /// <summary>
    /// Creates a default instance of a shader field based on its type.
    /// </summary>
    /// <param name="shaderType">shader type</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static object CreateInstance(string shaderType) => shaderType switch
    {
        "float" => 0f,
        "int" => 0,
        "bool" => false,
        "string" => string.Empty,
        "vec2" => new Vector2(0),
        "vec3" => new Vector3(0),
        _ => throw new ArgumentException($"Unsupported shader type: {shaderType}")
    };

    /// <summary>
    /// Converts a GLSL vertex shader to CESL format with new support for namespace and class declaration.
    /// </summary>
    /// <param name="glsl_source">glsl vertex source</param>
    /// <returns></returns>
    public static string ToCESLVertexShader(string glsl_source)
    {
        var lines = glsl_source.Split('\n');
        List<string> output = [];

        bool addedNamespaceAndClass = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            var isLastLine = Array.IndexOf(lines, rawLine) == lines.Length - 1;

            if (!isLastLine && !addedNamespaceAndClass)
            {
                var nextLine = lines[Array.IndexOf(lines, rawLine) + 1].Trim();

                if (nextLine.StartsWith("uniform"))
                {
                    var nameclass = @"namespace CESLNamespace;

class CESLVertex
{";

                    output.Add(nameclass);

                    addedNamespaceAndClass = true;
                }
            }

            output.Add(line);

            if (isLastLine)
                output.Add("}");
        }

        return string.Join("\n", output);
    }
}
