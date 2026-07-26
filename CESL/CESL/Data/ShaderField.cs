namespace CESL.Data;

public struct ShaderField
{
    public string Name { get; internal set; }

    public string Type { get; internal set; }

    public List<FieldAttribute> Attributes { get; internal set; }

    public bool IsPrivate { get; internal set; }

    public bool HasAttribute(string attributeName) => Attributes.Any(attr => attr.Name == attributeName);

    public object GetAttribute(string attributeName)
    {
        var attr = Attributes.FirstOrDefault(attr => attr.Name == attributeName);
        if (attr.Line != null)
        {
            return attr.GetAttribute();
        }
        return null;
    }

    public override readonly string ToString() => $"Name: {Name}, Type: {Type}, Attributes: {Attributes.Count}, IsPrivate: {IsPrivate}";
}
