using System;

public class NeedsComponentAttribute : Attribute
{
   public Type type;
   public NeedsComponentAttribute(Type type)
   {
      this.type = type;
   }
}

