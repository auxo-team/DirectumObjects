using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjType;

namespace Auxo.Common.Client
{
  partial class ObjTypeActions
  {
    
    public virtual void CardDescription(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = Reports.GetCardDescription();
      report.Entity = _obj;
      var entityGenitiveName = Functions.ObjType.Remote.GetGenitiveName(_obj);
      report.ExportToFile(Reports.Resources.CardDescription.ReportNameFormat(entityGenitiveName));
    }

    public virtual bool CanCardDescription(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    
    public virtual void TypeProperties(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Common.PublicFunctions.ObjProperty.Remote.GetPropertiesByEntityType(_obj).ShowModal();
    }

    public virtual bool CanTypeProperties(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Type == Common.ObjType.Type.Assignement ||
        _obj.Type == Common.ObjType.Type.Databook ||
        _obj.Type == Common.ObjType.Type.Document ||
        _obj.Type == Common.ObjType.Type.Task;
    }

  }

  partial class ObjTypeCollectionActions
  {

    public virtual void ChangesReport(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      foreach (var entity in _objs)
      {
        var report = Reports.GetObjectChanges();
        report.Entity = entity;
        report.TypeName = entity.Info.Properties.Type.GetLocalizedValue(entity.Type);
        report.ExportToFile(entity.Name);
      }
    }
    
    public virtual bool CanChangesReport(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    
  }
}