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
      var isParents = _obj.Parents.Any();
      
      var entityGuids = new List<string>();
      entityGuids.Add(_obj.EntityGuid);
      if (isParents)
        entityGuids.AddRange(_obj.Parents.Select(_ => _.EntityGuid));
      
      _obj.AllGuids = string.Join(", ", entityGuids);
      _obj.IsOverridde = isParents ? _obj.Parents.Where(_ => _.EntityGuid != Constants.Module.RecipientGuid.ToString()).Any() : false;
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Status = Common.ObjType.Status.Active;
      _obj.TypeString = string.Empty;
    }

    public override void BeforeDelete(Sungero.Domain.BeforeDeleteEventArgs e)
    {
      bool canDelete;
      if (!e.Params.TryGetValue(Constants.ObjType.CanDeleteParamName, out canDelete) || !canDelete)
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