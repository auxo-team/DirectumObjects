using System;
using Sungero.Core;

namespace Auxo.Common.Constants
{
  public static class Module
  {

    public static readonly Guid UserGuid = Guid.Parse("243c2d26-f5f7-495f-9faf-951d91215c77");
    
    public static readonly Guid RecipientGuid = Guid.Parse("c612fc41-44a3-428b-a97c-433c333d78e9");
    
    public const string OnClosedNullTypesParamName = "OnClosedNullTypes";
    
    public const int DelayedRetryMinutes = 5;
    
    public const int MaxRetryIteration = 30;    
    
    public static class Declensions
    {
      [Sungero.Core.Public]
      public const string Accusative = "Accusative";
      
      [Sungero.Core.Public]
      public const string Dative = "Dative";
      
      [Sungero.Core.Public]
      public const string Prepositional = "Prepositional";
      
      [Sungero.Core.Public]
      public const string Genitive = "Genitive";
      
      [Sungero.Core.Public]
      public const string Ablative = "Ablative";
    }
    
    public static class ObjTypesNames
    {
      [Sungero.Core.Public]
      public const string JobTitles = "JobTitles";
      
      [Sungero.Core.Public]
      public const string Departments = "Departments";
      
      [Sungero.Core.Public]
      public const string People = "People";
      
      [Sungero.Core.Public]
      public const string User = "User";
      
      [Sungero.Core.Public]
      public const string Employee = "Employee";
    }
    
  }
}