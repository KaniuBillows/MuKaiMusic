using System;

namespace DataAbstract.Attributes
{
    /// <summary>
    /// Body加密，被此特性标注的Action的Body会进行尝试解密
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EncryptAttribute : Attribute
    {

    }

}