using CESL.Attributes;

namespace CESL.Data;

public struct FieldAttribute
{
    public string Name { get; internal set; }

    public string Line { get; internal set; }

    public object GetAttribute()
    {
        var attr = CESL.ParseAttribute(Line);

        if (attr.name == "Range")
        {
            var min = float.Parse(attr.args[0]);
            var max = float.Parse(attr.args[1]);
            var step = float.Parse(attr.args[2]);

            var range = new RangeValue(min, max, step);

            return range;
        }

        if (attr.name == "Header")
        {
            var header = new HeaderAttribute(attr.args[0]);
            return header;
        }

        return null;
    }
}
