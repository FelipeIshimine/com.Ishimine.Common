using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Deconstructors
{
	public static void Deconstruct(this Vector3 @this, out float x, out float y, out float z)
	{
		x = @this.x;
		y = @this.x;
		z = @this.x;
	}
	
}
