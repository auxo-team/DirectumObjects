using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Auxo.Common.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Закрытие удаленных типов объектов.
    /// </summary>
    public virtual void ClosedNullTypes()
    {
      var postfix = "ClosedNullTypes";
      Logger.WithLogger(postfix).Debug("Job starting. Cleaning the directory from old entities.");
      
      var closeTypes = new List<long>();
      using (var command = SQL.GetCurrentConnection().CreateCommand())
      {
        command.CommandText = Queries.ObjType.SelectCleanEntityTypes;
        using (var typesIds = command.ExecuteReader())
        {
          while (typesIds.Read())
            closeTypes.Add(typesIds.GetInt64(0));
        }
      }
      
      if (!closeTypes.Any())
      {
        Logger.WithLogger(postfix).Debug("Job complite. There are no records to close.");
        return;
      }
      
      foreach (var closeType in Common.ObjTypes.GetAll().Where(_ => closeTypes.Contains(_.Id) && _.Status != Common.ObjType.Status.Closed))
      {
        closeType.Status = Common.ObjType.Status.Closed;
        closeType.Save();
        Logger.WithLogger(postfix).Debug(string.Format("Entry {0} successfully closed", closeType.Id));
      }
      
      Logger.WithLogger(postfix).Debug("Job complite");
    }

  }
}