using System;
using Sungero.Core;

namespace Auxo.Common.Constants
{
  public static class ObjectChanges
  {
    
    /// <summary>
    /// Имя таблицы в БД
    /// </summary>
    public const string SourceTableName = "Auxo_ReportChanges";
    
    /// <summary>
    /// Разделы объектов.
    /// </summary>
    public static class ChaptersNames
    {
      public const string HandledEvents = "HandledEvents";
      
      public const string Properties = "Properties";
      
      public const string Actions = "Actions";
      
      public const string Forms = "Forms";
      
      public const string RibbonActions = "RibbonActions";
    }
    
  }
}