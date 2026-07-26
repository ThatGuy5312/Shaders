using OpenTK.Mathematics;

namespace CESL;

public abstract class CEShader
{
    public abstract void SetMatrix4(string name, Matrix4 value);

    public abstract void SetVector3(string name, Vector3 value);

    public abstract void SetVector2(string name, Vector2 value);

    public abstract void SetVector4(string name, Vector4 value);

    public abstract void SetInt(string name, int value);

    public abstract void SetFloat(string name, float value);

    public abstract void SetBool(string name, bool value);
}
