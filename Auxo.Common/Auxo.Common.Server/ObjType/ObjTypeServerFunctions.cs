using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Auxo.Common.ObjType;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace Auxo.Common.Server
{
  partial class ObjTypeFunctions
  {

    /// <summary>
    /// Получить имя типа в родительном падеже.
    /// </summary>
    /// <returns>Имя типа в родительнос падеже.</returns>
    [Remote(IsPure = true)]
    public virtual string GetGenitiveName()
    {
      return Sungero.Content.Server.ElectronicDocumentFunctions.Genitive(_obj.LocalizedName);
    }

    /// <summary>
    /// Получить все действующие записи справочника "Тип сущности".
    /// </summary>
    /// <returns>Список записей справочника.</returns>
    [Public, Remote(IsPure = true, PackResultEntityEagerly = true)]
    public static List<IObjType> GetAllActiveTypes()
    {
      return ObjTypes.GetAll().Where(_ => _.Status == Common.ObjType.Status.Active).ToList();
    }

    /// <summary>
    /// Получить действующие записи справочника "Тип сущности" по GUID.
    /// </summary>
    /// <param name="entityGuid">GUID сущности.</param>
    /// <returns>Список записей "Тип сущности".</returns>
    [Public, Remote(IsPure = true, PackResultEntityEagerly = true)]
    public static IQueryable<IObjType> GetTypesByGuid(string entityGuid)
    {
      if (string.IsNullOrWhiteSpace(entityGuid))
        return null;
      
      entityGuid = entityGuid.ToLower();
      return ObjTypes.GetAll()
        .Where(_ => _.Status == Common.ObjType.Status.Active)
        .Where(_ => _.EntityGuid == entityGuid || _.Parents.Any(p => p.EntityGuid == entityGuid));
    }
   
    /// <summary>
    /// Создать запись "Тип сущности".
    /// </summary>
    /// <param name="objectType">GUID сущности.</param>
    /// <param name="metadata">Метаданные типа.</param>
    /// <returns>Запись "Тип сущности".</returns>
    public static IObjType CreateType(Enumeration objectType, Sungero.Metadata.EntityMetadata metadata)
    {
      if (metadata == null)
        return null;
      
      var recordFindName = metadata.Name;
      if (recordFindName == Constants.Module.ObjTypesNames.User)
        recordFindName = Constants.Module.ObjTypesNames.Employee;
      
      var originalEntityGuid = metadata.GetOriginal().NameGuid.ToString().ToLower();
      var record = ObjTypes.GetAll().Where(_ => _.EntityGuid == originalEntityGuid || _.Parents.Any(p => p.EntityGuid == originalEntityGuid)).FirstOrDefault();
      if (record != null)
        return record;
      
      record = ObjTypes.Create();
      record.Type = objectType;
      record.Name = recordFindName;
      record.LocalizedName = metadata.GetCollectionDisplayName();
      record.EntityType = metadata.FullName;
      record.EntityGuid = metadata.NameGuid.ToString().ToLower();
      record.TableNameDB = metadata.DBTableName.ToLower();
      record.IsAbstract = metadata.IsAbstract;            
      Functions.ObjType.FillAdditionalInfo(record, metadata);
      record.Save();
      
      return record;
    }
    
    /// <summary>
    /// Проверить и обновить изменения типа.
    /// </summary>
    /// <param name="metadata">Метаданные типа.</param>
    public virtual void CheckAndUpdateTypeChanges(Sungero.Metadata.EntityMetadata metadata)
    {
      if (metadata == null)
        return;
      
      var recordFindName = metadata.Name;
      if (recordFindName == Constants.Module.ObjTypesNames.User)
        recordFindName = Constants.Module.ObjTypesNames.Employee;
      
      var entityGuid = metadata.NameGuid.ToString().ToLower();
      if (_obj.Name != recordFindName && _obj.EntityGuid != entityGuid)
        return;

      var curentMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(Guid.Parse(_obj.EntityGuid));
      
      #region Актуализация изменяемых свойств типа

      if (_obj.Name != recordFindName)
        _obj.Name = recordFindName;
      
      var localizedName = metadata.GetCollectionDisplayName();
      if (_obj.LocalizedName != localizedName)
        _obj.LocalizedName = localizedName;
      
      if (_obj.Status != Common.ObjType.Status.Active)
        _obj.Status = Common.ObjType.Status.Active;
      
      #endregion
      
      #region Проверка наследования

      if (curentMetadata != null && _obj.EntityGuid != entityGuid && !_obj.Parents.Any(p => p.EntityGuid == entityGuid))
      {
        //Если metadata является родителем для curentMetadata
        if (curentMetadata.IsAncestorFor(metadata))
        {
          var newParent = _obj.Parents.AddNew();
          newParent.EntityType = _obj.EntityType;
          newParent.EntityGuid = _obj.EntityGuid;
          
          _obj.EntityType = metadata.FullName;
          _obj.EntityGuid = entityGuid;
        }
        else
        {
          var newParent = _obj.Parents.AddNew();
          newParent.EntityType = metadata.FullName;
          newParent.EntityGuid = entityGuid;
        }
      }
      
      #endregion
      
      #region Актуализация Guid-ов
      
      // Удалим устаревшие строки
      foreach (var line in _obj.Parents)
      {
        var checkMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(Guid.Parse(line.EntityGuid));
        if (checkMetadata != null)
          continue;
        
        _obj.Parents.Remove(line);
      }
      
      // Проверим и заменим конечный Guid типа
      if (curentMetadata == null)
      {
        if (!_obj.Parents.Any())
        {
          _obj.EntityGuid = entityGuid;
          _obj.EntityType = metadata.FullName;
        }
        else
        {
          var checkGuid = Guid.Parse(_obj.Parents.FirstOrDefault().EntityGuid);
          var typeMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(checkGuid).GetFinal();
          var finalGuid = typeMetadata.NameGuid.ToString().ToLower();
          var removedLine = _obj.Parents.Where(_ => _.EntityGuid == finalGuid).FirstOrDefault();
          
          _obj.EntityGuid = removedLine != null ? removedLine.EntityGuid : finalGuid;
          _obj.EntityType = removedLine != null ? removedLine.EntityType : typeMetadata.FullName;
          
          if (removedLine != null)
            _obj.Parents.Remove(removedLine);
        }
      }

      #endregion
      
      this.FillAdditionalInfo(metadata);
      
      if (_obj.State.IsChanged)
        _obj.Save();
    }
    
    /// <summary>
    /// Заполнить дополнительную информацию.
    /// </summary>
    /// <param name="metadata">Метаданные типа.</param>
    public virtual void FillAdditionalInfo(Sungero.Metadata.EntityMetadata metadata)
    {
      //TODO: Метод расширения функционала.
    }
    
  }
}