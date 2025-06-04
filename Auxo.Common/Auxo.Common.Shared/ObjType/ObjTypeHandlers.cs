using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjType;

namespace Auxo.Common
{
  partial class ObjTypeSharedHandlers
  {

    public virtual void TypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.TypeString = string.Empty;
      if (e.NewValue != null)
        _obj.TypeString = ObjTypes.Info.Properties.Type.GetLocalizedValue(e.NewValue);
    }

  }
}