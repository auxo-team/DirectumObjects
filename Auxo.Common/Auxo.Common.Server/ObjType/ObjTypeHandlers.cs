using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjType;

namespace Auxo.Common
{
  partial class ObjTypeServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var guids = new List<string>();
      guids.Add(_obj.EntityGuid);
      guids.AddRange(_obj.Parents.Select(p => p.EntityGuid));
      
      _obj.AllGuids = string.Join(", ", guids);
      _obj.IsOverridde = _obj.Parents.Where(_ => _.EntityGuid != Constants.Module.RecipientGuid.ToString()).Any();
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Status = Common.ObjType.Status.Active;
      _obj.TypeString = string.Empty;
    }

    public override void BeforeDelete(Sungero.Domain.BeforeDeleteEventArgs e)
    {
      bool canDelete;
      if (!e.Params.Contains(Constants.ObjType.CanDeleteParamName) || !(e.Params.TryGetValue(Constants.ObjType.CanDeleteParamName, out canDelete) && canDelete))
      {
        e.AddError(ObjTypes.Resources.DeleteErrorMessage);
        return;
      }
      
      var properties = Functions.ObjProperty.GetPropertiesByEntityType(_obj).ToList();
      foreach (var property in properties)
      {
        var extEntity = (Sungero.Domain.Shared.IExtendedEntity)property;
        extEntity.Params.Add(Constants.ObjProperty.CanDeleteParamName, true);
        ObjProperties.Delete(property);
      }
    }
    
  }
}