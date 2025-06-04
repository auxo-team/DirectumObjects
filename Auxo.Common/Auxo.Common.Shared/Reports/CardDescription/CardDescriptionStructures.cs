using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Auxo.Common.Structures.CardDescription
{

  partial class DescriptionLine
  {
    /// <summary>
    /// Guid для отчета
    /// </summary>
    public string ReportSessionId { get; set; }
    
    /// <summary>
    /// № группы
    /// </summary>
    public int GroupNum { get; set; }
    
    /// <summary>
    /// Имя группы
    /// </summary>
    public string GroupName { get; set; }
    
    /// <summary>
    /// Имя свойства
    /// </summary>
    public string PropertyName { get; set; }
    
    /// <summary>
    /// Тип свойства
    /// </summary>
    public string TypeInfo { get; set; }   
    
    /// <summary>
    /// Обязательное
    /// </summary>
    public string IsRequired { get; set; } 
    
    /// <summary>
    /// Редактируемое
    /// </summary>
    public string IsEnabled { get; set; } 
    
    /// <summary>
    /// Вынесено на форму/ленту
    /// </summary>
    public string IsVisibility { get; set; } 
    
    /// <summary>
    /// Отображать в диалоге поиска
    /// </summary>
    public string CanBeSearch { get; set; } 
    
    /// <summary>
    /// Значения
    /// </summary>
    public string ValuesInfo { get; set; }     
  }
    
}