CESL shader language project that I will be using in my game engine project [Chondria](https://github.com/ThatGuy5312/Chondria)

Functionality:

Adds csharp-like functionality like having public and private fields along with field attributes into GLSL.

-

Data types:

VertexShader:
Contains the Namespace, ClassName, and GLSL code.

FragmentShader:
Contains the Namespace, ClassName, GLSL code, and a list of found ShaderField's.
It also contains two functions.
FindShaderField(string name) - Finds a shader fields based off of name of field.
SetUniform(CEShader shader, ShaderField field, object value, int index = 0) - Sets the shader uniform of a specific field.
shader - pass on your shader class (more below on how that works)
field - the specific field being changed
value - the object value of the uniform being set
index - only for texture units

ShaderField:
Contains the Name, Type, Attributes, and IsPrivate data of the field.
There are also two functions for help with attributes.
HasAttribute(string attributeName) - return true or false depending on if the field has that attribute in it.
GetAttribute(string attributeName) - gets the attribute object based off of the name without needing to go inside the attribute.

FieldAttribute:
Contains the Name, and Line which is the full line found from the shader.
It also contains a single function.
GetAttribute() - gets the attribute object parsed from the Line.

–

Using ShaderField and Attributes:

The public and private member values will be stored as a bool in a ShaderField as IsPrivate.

This can be used in many ways like how I use it with having a private value not display on the
UI as a material property.

The attributes can also be used in many ways along with it being expandable.
It is stored as a list of FieldAttribute's in a ShaderField.
Attributes can go two ways, one is already known and parsed by the GetAttribute() function inside FieldAttribute, the second in a custom attribute.
A FieldAttribute holds just a Name (name of the attribute), and Line which is the full line of the attribute. It also has GetAttribute() which turns known attributes into a usable object.
There are only two known attributes which are RangeValue and Header. More may come in the future but for the user it takes a little but of work to add a custom complex attribute.
When making a custom complex attribute you can run the Line property through CESL.ParseAttribute(Line) to get the name of the attribute and a string list of arugments found while parsing.
When using attributes in code, using already known attributes is straight forward.

RangeValue:
[Range(#, #, #)] - This will be found and parsed when running GetAttribute(). Also when doing a int, use whole numbers. But when doing floats but a decimal.
RangeValue has a IsInt boolean for the user to either make it display differently on the UI (What I use it for). It also contains a Min, Max, and Step float for easy use with Dear ImGui.

HeaderAttribute:
[Header(Test Header)] - This will also be found and parsed when running GetAttribute(). Header attribute just contains a Name property which in this case with contain "Test Attribute".
You can you this any way you like, my intention was for a drop down header for grouping fields.

There are also 'simple' attributes which are defined by the user but they are standard attributes with no arguments.
A good example is [Color]. This isn't found while parsing but is found in the engine and can display a type like vec3 as a color instead as a drag float (if your using Dear ImGui).

–

CEShader:

CEShader is a simple 'shader' subclass that your real shader class inherits from. CEShader contains nearly nothing, theres just a important method that relies on it.

CEShader only contains some abstract members:
SetMatrix4(string name, Matrix4 value)
SetVector3(string name, Vector3 value)
SetVector2(string name, Vector2 value)
SetVector4(string name, Vector4 value)
SetInt(string name, int value)
SetFloat(string name, float value)
SetBool(string name, bool value)

You just need to implement the code for these when making a OpenGL shader program script.

-

For the actual C# functionality there are loads of helper functions to make implementaion easy.

CESL Functions:
ParseVertexShader(string source) - Returns a VertexShader holding the parsed GLSL code, Namespace, and ClassName.

ParseFragmentShader(string source) - Returns a FragmentShader holding the parsed GLSL code, Namespace, ClassName, and a list of ShaderFields.

ParseAttribute(string attr) - turns an attribute, ex [Range(0, 10, 1)] and returns a tuple with a string and string list holding the name of the attribute (Range), and the arguments (0, 10, 1).

CreateInstance(string shaderType) - Creates an instance of a uniform value type, 
ex vec3 -> new Vector3(0).

ToCESLVertexShader(string glsl_source) -  Turns a GLSL vertex shader into a working CESL vertex shader. All it adds is the bracket spacing and declaration for a namespace and class name.
