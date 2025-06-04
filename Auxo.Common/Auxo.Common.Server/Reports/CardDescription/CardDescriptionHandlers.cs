using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Auxo.Common
{
  partial class CardDescriptionServerHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var objType = CardDescription.Entity;
      
      var reportSessionId = Guid.NewGuid().ToString();
      CardDescription.ReportSessionId = reportSessionId;
      CardDescription.TypeName = objType.Info.Properties.Type.GetLocalizedValue(objType.Type);
      
      var propertiesList = new List<Structures.CardDescription.DescriptionLine>();                  
      var properties = Common.PublicFunctions.ObjProperty.Remote.GetPropertiesByEntityType(objType).Where(_ => _.IsSystem != true).ToList();
      var groupNum = 1;
      
      foreach (var property in properties.Where(_ => _.Parent == null && _.PropertyType != ObjProperty.PropertyType.AxCollection))
      {
        var description = GetDescription(property);
        description.ReportSessionId = reportSessionId;
        description.GroupNum = groupNum;
        description.GroupName = Reports.Resources.CardDescription.MainGroupName;
        propertiesList.Add(description);
      }
      
      var collections = properties.Where(_ => _.PropertyType == ObjProperty.PropertyType.AxCollection);
      foreach (var collection in collections)
      {
        groupNum++;
        
        foreach (var property in properties.Where(_ => Equals(_.Parent, collection)))
        {
          var description = GetDescription(property);
          description.ReportSessionId = reportSessionId;
          description.GroupNum = groupNum;
          description.GroupName = Reports.Resources.CardDescription.CollectionNameFormat(collection.LocalizedName);
          propertiesList.Add(description);
        }
      }
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.CardDescription.SourceTableName, propertiesList);
    }
    
    /// <summary>
    /// Получить описание свойства.
    /// </summary>
    /// <param name="property">Свойство.</param>
    /// <returns>Структура с описанием свойства.</returns>
    public virtual Structures.CardDescription.DescriptionLine GetDescription(IObjProperty property)
    {
      var newLine = System.Environment.NewLine;
      var typeName = ObjProperties.Info.Properties.PropertyType.GetLocalizedValue(property.PropertyType);
      
      var valuesInfo = string.Empty;
      if (property.PropertyType == ObjProperty.PropertyType.AxEnumeration)
        valuesInfo = string.Join(string.Format(",{0}", newLine), property.EnumValues.Select(_ => string.Format("{1} ({0})", _.Name, _.LocalizedValue)));
      
      return Structures.CardDescription.DescriptionLine.Create(string.Empty,
                                                               0,
                                                               string.Empty,
                                                               string.Format("{0}{1}{2}", property.LocalizedName, newLine, property.Name),
                                                               property.PropertyType == ObjProperty.PropertyType.AxString ? string.Format("{0} ({1})", typeName, property.Length) : typeName,
                                                               property.IsRequired.GetValueOrDefault() ? Reports.Resources.CardDescription.Yes : Reports.Resources.CardDescription.No,
                                                               property.IsEnabled.GetValueOrDefault() ? Reports.Resources.CardDescription.Yes : Reports.Resources.CardDescription.No,
                                                               property.IsVisibility.GetValueOrDefault() ? Reports.Resources.CardDescription.Yes : Reports.Resources.CardDescription.No,
                                                               property.CanBeSearch.GetValueOrDefault() ? Reports.Resources.CardDescription.Yes : Reports.Resources.CardDescription.No,
                                                               valuesInfo);
    }

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.CardDescription.SourceTableName, CardDescription.ReportSessionId);
    }

  }
}