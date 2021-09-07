using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedsComponentAttribute : Attribute
{
   public Type type;
   public NeedsComponentAttribute(Type type)
   {
      this.type = type;
   }
}

