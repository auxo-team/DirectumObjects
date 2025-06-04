using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjProperty;

namespace Auxo.Common
{
  partial class ObjPropertyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Status = ObjProperty.Status.Active;
      _obj.PropertyTypeString = string.Empty;
    }
    
    public override void BeforeDelete(Sungero.Domain.BeforeDeleteEventArgs e)
    {
      bool canDelete;
      if (!e.Params.Contains(Constants.ObjProperty.CanDeleteParamName) || !(e.Params.TryGetValue(Constants.ObjProperty.CanDeleteParamName, out canDelete) && canDelete))
      {
        e.AddError(ObjProperties.Resources.DeleteErrorMessage);
        return;
      }
    }

  }
}