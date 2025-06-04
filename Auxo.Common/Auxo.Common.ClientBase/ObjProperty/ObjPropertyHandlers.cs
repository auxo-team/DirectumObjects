using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjProperty;

namespace Auxo.Common
{
  partial class ObjPropertyClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var properties = _obj.State.Properties;
      
      if (_obj.Status == ObjProperty.Status.Active)
        e.HideAction(_obj.Info.Actions.DeleteEntity);
      
      properties.Parent.IsVisible = _obj.Parent != null;
      properties.NavigationGuid.IsVisible = _obj.PropertyType == ObjProperty.PropertyType.AxNavigation;
      properties.EnumValues.IsVisible = _obj.PropertyType == ObjProperty.PropertyType.AxEnumeration;
      properties.Length.IsVisible = _obj.PropertyType == ObjProperty.PropertyType.AxString;
    }

  }
}