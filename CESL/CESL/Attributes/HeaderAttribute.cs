namespace CESL.Attributes;

public class HeaderAttribute(string name)
{
    public string Name { get; internal set; } = name;
}
