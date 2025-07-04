using System;
using Sungero.Core;

namespace Auxo.Common.Constants
{
  public static class Module
  {

    public static readonly Guid UserGuid = Guid.Parse("243c2d26-f5f7-495f-9faf-951d91215c77");
    
    public static readonly Guid RecipientGuid = Guid.Parse("c612fc41-44a3-428b-a97c-433c333d78e9");
    
    public const string ClosedNullTypesParamName = "ClosedNullTypesIsWorking";
    
    public const int DelayedRetryMinutes = 5;
    
    public const int MaxRetryIteration = 30;    
    
  }
}