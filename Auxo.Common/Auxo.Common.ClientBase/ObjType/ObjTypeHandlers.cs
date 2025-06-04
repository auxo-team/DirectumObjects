using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjType;

namespace Auxo.Common
{
  partial class ObjTypeClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var properties = _obj.State.Properties;
      
      if (_obj.Status == ObjType.Status.Active)
        e.HideAction(_obj.Info.Actions.DeleteEntity);
      
      var current = Users.Current;
      properties.Status.IsEnabled = current.IncludedIn(Roles.Administrators);
      
      properties.Parents.IsVisible = _obj.Parents.Any();
    }

  }
}