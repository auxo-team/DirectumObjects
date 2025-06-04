using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Auxo.Common.Structures.ObjectChanges
{

  /// <summary>
  /// Данные по изменениям
  /// </summary>
  [Sungero.Core.PublicAttribute]
  partial class OverrideData
  {
    /// <summary>
    /// Guid для отчета
    /// </summary>
    public string ReportSessionId { get; set; }
    
    /// <summary>
    /// Имя группы объектов
    /// </summary>
    public string GroupName { get; set; }
    
    /// <summary>
    /// Имя объекта
    /// </summary>
    public string ObjectName { get; set; }
    
    /// <summary>
    /// Имя события
    /// </summary>
    public string EventName { get; set; }
    
    /// <summary>
    /// Тип изменения
    /// </summary>
    public string ActionName { get; set; }   
    
    /// <summary>
    /// Код компании
    /// </summary>
    public string CompanyCode { get; set; } 
  }
  
}