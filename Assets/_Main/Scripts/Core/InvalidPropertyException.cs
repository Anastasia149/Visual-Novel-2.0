using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class InvalidPropertyException : Exception
{
   public InvalidPropertyException() : base() { }
   public InvalidPropertyException(string message) : base(message) { }
   public InvalidPropertyException(string message, Exception innerException) : base(message, innerException) { }
   protected InvalidPropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
