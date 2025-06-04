using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjProperty;

namespace Auxo.Common
{
  partial class ObjPropertySharedHandlers
  {

    public virtual void ObjTypeChanged(Auxo.Common.Shared.ObjPropertyObjTypeChangedEventArgs e)
    {
      _obj.Length = null;
      
      if (e.NewValue != null)
        _obj.ObjTypeName = e.NewValue.Name;
    }

    public virtual void PropertyTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.PropertyTypeString = string.Empty;
      if (e.NewValue != null)
        _obj.PropertyTypeString = ObjProperties.Info.Properties.PropertyType.GetLocalizedValue(e.NewValue);
    }

  }
}