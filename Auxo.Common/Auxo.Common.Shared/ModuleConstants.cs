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
      public const string Accusative = "Accusative";
      
      public const string Dative = "Dative";
      
      public const string Prepositional = "Prepositional";
      
      public const string Genitive = "Genitive";
      
      public const string Ablative = "Ablative";
    }
    
    public static class ObjTypesNames
    {
      public const string JobTitles = "JobTitles";
      
      public const string Departments = "Departments";
      
      public const string People = "People";
      
      public const string User = "User";
      
      public const string Employee = "Employee";
    }
    
  }
}