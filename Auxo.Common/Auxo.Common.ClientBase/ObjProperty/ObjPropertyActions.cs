using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjProperty;

namespace Auxo.Common.Client
{
  partial class ObjPropertyActions
  {
    
    public virtual void OpenTypeLink(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var type = Functions.ObjType.Remote.GetTypeByGuid(_obj.NavigationGuid);
      if (type == null)
      {
        Dialogs.ShowMessage(ObjProperties.Resources.NoTypeErrorFormat(_obj.NavigationGuid));
        return;
      }
      
      type.ShowModal();
    }

    public virtual bool CanOpenTypeLink(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !string.IsNullOrEmpty(_obj.NavigationGuid);
    }

    
    public virtual void ChildProperties(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.ObjProperty.Remote.GetChildProperties(_obj).ShowModal();
    }

    public virtual bool CanChildProperties(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.PropertyType == ObjProperty.PropertyType.AxCollection;
    }

  }
}