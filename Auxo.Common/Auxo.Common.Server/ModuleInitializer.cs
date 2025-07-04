using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace Auxo.Common.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      ClosedNullTypes();
      
      FillEntitiesTypes();
      FillEntitiesProperties();

      GrantRightsOnDefault();
      CreateDefaultTables();
    }
    
    /// <summary>
    /// Выдать права по-умолчанию.
    /// </summary>
    public static void GrantRightsOnDefault()
    {
      InitializationLogger.Debug("Init: Grant rights on default to all users.");
      
      var allUsers = Roles.AllUsers;
      
      if (!Common.ObjTypes.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, allUsers))
      {
        Common.ObjTypes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
        Common.ObjTypes.AccessRights.Save();
      }
      
      if (!Common.ObjProperties.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, allUsers))
      {
        Common.ObjProperties.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
        Common.ObjProperties.AccessRights.Save();
      }
    }
    
    /// <summary>
    /// Создать таблицы по умолчанию.
    /// </summary>
    public static void CreateDefaultTables()
    {
      CreateTable(Constants.ObjectChanges.SourceTableName, Queries.ObjectChanges.CreateSourceTable, true);
      CreateTable(Constants.CardDescription.SourceTableName, Queries.CardDescription.CreateSourceTable, true);
    }

    
    #region Заполнение справочника "Типы сущностей"
    
    /// <summary>
    /// Закрытие удаленных типов.
    /// </summary>
    public virtual void ClosedNullTypes()
    {
      InitializationLogger.Debug("Init: Cleaning the directory from old entities.");
      
      Jobs.ClosedNullTypes.Enqueue();
    }
    
    /// <summary>
    /// Заполнить типы.
    /// </summary>
    public virtual void FillEntitiesTypes()
    {
      InitializationLogger.Debug("Init: Fill in the directory Type of entities.");
      
      using (Sungero.Domain.Session session = new Sungero.Domain.Session(true, false))
      {
        FillTypes(typeof(Sungero.CoreEntities.IDatabookEntry), Common.ObjType.Type.Databook);
        HandleRecipientType();
        FillTypes(typeof(Sungero.Content.IElectronicDocument), Common.ObjType.Type.Document);
        FillTypes(typeof(Sungero.Workflow.ITask), Common.ObjType.Type.Task);
        FillTypes(typeof(Sungero.Workflow.IAssignment), Common.ObjType.Type.Assignement);
      }
    }
    
    /// <summary>
    /// Заполнить типы сущностей.
    /// </summary>
    /// <param name="type">Тип сущности.</param>
    /// <param name="objectType">Тип сущности из справочника.</param>
    public virtual void FillTypes(System.Type type, Enumeration objectType)
    {
      InitializationLogger.DebugFormat("Init: Fill type {0}.", objectType);
      if (objectType == null)
        return;

      //Метаданные базового типа (IElectronicDocument, IDatabookEntry и т.д.)
      var baseMetadata = type.GetEntityMetadata();
      if (baseMetadata == null)
        return;
      
      var currentTypesIds = ObjTypes.GetAll().Select(_ => _.Id).ToList();
      foreach (var typeGuid in GetFillGuids(baseMetadata.GetOriginal().NameGuid.ToString(), objectType))
      {
        var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(typeGuid);
        if (entityMetadata == null)
          continue;
        
        //Если baseMetadata является предком для entityMetadata
        if (typeGuid != Constants.Module.UserGuid && !baseMetadata.IsAncestorFor(entityMetadata))
          continue;

        var record = Functions.ObjType.CreateType(objectType, entityMetadata);
        if (record == null)
          continue;
        
        if (!currentTypesIds.Contains(record.Id))
        {
          currentTypesIds.Add(record.Id);
          continue;
        }
        
        Functions.ObjType.CheckAndUpdateTypeChanges(record, entityMetadata);
      }
    }
    
    /// <summary>
    /// Получить список Guid-ов для заполнения.
    /// </summary>
    /// <param name="currentGuid">Guid сущности.</param>
    /// <param name="objectType">Тип сущности из справочника.</param>
    /// <returns>Список Guid-ов для заполнения.</returns>
    public List<Guid> GetFillGuids(string currentGuid, Enumeration objectType)
    {
      var typesList = new List<Guid>();
      using (var command = SQL.GetCurrentConnection().CreateCommand())
      {
        command.CommandText = string.Format(Queries.ObjType.SelectTypeGuids, currentGuid.ToUpper());
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
            typesList.Add(reader.GetGuid(0));
        }
      }
      
      return typesList.Except(this.GetNotSelectGuids()).ToList();
    }
    
    /// <summary>
    /// Получить список Guid-ов исключаемых из заполнения.
    /// </summary>
    /// <returns>Список Guid-ов исключаемых из заполнения.</returns>
    public virtual System.Collections.Generic.HashSet<Guid> GetNotSelectGuids()
    {
      return new HashSet<Guid>
      {
        Guid.Parse("a6de7bb6-8c2d-425a-b35a-1d60119ecfa8"), //Активный пользователь
        Guid.Parse("668418c4-bd08-4aeb-94d7-d0c30869c1a0"), //Бинарный образ документа
        Guid.Parse("f5061291-4ac6-428f-b091-d53acdbe9ae5"), //Группа документов
        Guid.Parse("218ac7df-9b29-400a-b3fc-c2e22927f638"), //Действие по отправке документа
        Guid.Parse("1e9415ec-6ba8-46b5-b864-94b4385ffb52"), //Пакет бинарных образов документов
        Guid.Parse("91fcb864-f2ee-43d7-854b-23a3dcce65cb"), //Приложение-обработчик
        Guid.Parse("32ea0857-adf7-41c2-bc0c-188320e40786"), //Результат распознавания сущности
        Guid.Parse("0896aa80-e1da-4a1e-9485-d172f8e242bc"), //Тип документа
        Guid.Parse("34d8054f-43c0-4c6a-a1a6-7ee427d65dc8"), //Тип прав
        Guid.Parse("effcb90e-8151-43f3-8139-661aa5bc6885"), //Тип файлов
        Guid.Parse("56c16259-fa13-4474-98fc-3f517cc4ed02"), //Фоновый процесс
        Guid.Parse("1cafe357-1b39-4fcc-a703-9607b958f5ef"), //Хранилище
        Guid.Parse("96d026d3-0224-4cc8-b633-67a30679b1e5"), //Часовой пояс
        Guid.Parse("de2707a2-2a1f-41cb-98ef-e6e17449bea8"), //Частный календарь рабочего времени
        Guid.Parse("de73a02c-c1bf-4edf-bee4-bf2705d282b8"), //Элемент очереди выдачи прав на документы
        Guid.Parse("5ec13d1f-de94-43a3-a51a-1bef325d9dad"), //Элемент очереди выдачи прав на проект и папки проекта
        Guid.Parse("aa042ddf-a9fb-4dea-883c-d0024b9574da"), //Элемент очереди выдачи прав на проектные документы
        Guid.Parse("b7edf323-816d-4213-abca-6ee7da1c03bd"), //Элемент очереди выдачи прав участникам проекта
        Guid.Parse("2d30e2aa-1d0b-45f0-8e4d-00318b3a5cfd"), //Элемент очереди конвертации тел документов
        Guid.Parse("50a0d7aa-1f04-4e4a-8f0c-044e0ba99949"), //Элемент очереди синхронизации контрагентов
        Guid.Parse("f9a3ec37-0fd4-4343-a295-9394ba830a0e"), //Элемент очереди синхронизации сообщений
        Guid.Parse("9abcf1b7-f630-4a82-9912-7f79378ab199"), //Шаблоны документов (Sungero.Content.ElectronicDocumentTemplate)
        Guid.Parse("89c6dc08-dd81-48e2-a327-707dd86a64cd"), //Тип сущности
        Guid.Parse("cf6c5541-a733-4d30-9810-453454a03811"), //Свойство сущности
        Guid.Parse("f7cba906-f4c9-4f02-97e8-a6c6155a8ee6"), //Представления модулей
        Guid.Parse("f1bc9488-3907-4e85-86aa-a37232d5a558"), //Представления проводника
        Guid.Parse("000005ef-064d-4cbc-bc6c-d2c8cebedc9e"), //Представления форм
        Guid.Parse("4cbdfb03-5462-43c4-a033-034e3d3aa095"), //Модули проводника
        Guid.Parse("4eee364b-a766-4a5f-8fbd-91e45c5f95ca"), //Вычисляемые роли
        Guid.Parse("d7c7a65b-e62b-41dd-a6d9-7df4b4273763"), //Варианты процессов
        Guid.Parse("9d6f9dd6-e9d2-495b-b628-dd7082d201e6")  //Связи
      };
    }
    
    /// <summary>
    /// Обработать тип Recipient.
    /// </summary>
    public void HandleRecipientType()
    {
      var entityNames = GetHandleNames();
      
      var entityTypes = ObjTypes.GetAll()
        .Where(_ => entityNames.Contains(_.Name))
        .Where(_ => !_.Parents.Any(p => p.EntityGuid == Constants.Module.RecipientGuid.ToString()));
      
      if (!entityTypes.Any())
        return;
      
      //Метаданные
      var recipientMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(Constants.Module.RecipientGuid);
      
      foreach (var entityType in entityTypes)
      {
        var newParent = entityType.Parents.AddNew();
        newParent.EntityType = recipientMetadata.FullName;
        newParent.EntityGuid = Constants.Module.RecipientGuid.ToString();
        entityType.Save();
      }
    }
    
    /// <summary>
    /// Получить имена справочников для обработки.
    /// </summary>
    /// <returns>Имена справочников для обработки.</returns>
    public virtual List<string> GetHandleNames()
    {
      return new List<string> { "Employee", "RegistrationGroup", "Department", "BusinessUnit", "Role" };
    }
    
    #endregion

    
    #region Заполнение справочника "Свойства сущностей"
    
    public virtual void FillEntitiesProperties()
    {
      InitializationLogger.Debug("Init: Fill in the directory Entities properties.");
      
      var types = this.GetUpdateEntityPropertiesTypes();
      var objTypes = Functions.ObjType.GetAllActiveTypes().Where(_ => _.Type.HasValue && types.Contains(_.Type.Value));      
      foreach (var objType in objTypes)
      {
        var handler = AsyncHandlers.UpdateEntityProperties.Create();
        handler.ObjTypeId = objType.Id;
        handler.ExecuteAsync();
      }
    }
    
    /// <summary>
    /// Получить типы объектов для обновления свойств.
    /// </summary>
    /// <returns>Список типов объектов.</returns>
    public virtual List<Enumeration> GetUpdateEntityPropertiesTypes()
    {
      return new List<Enumeration>
      {
        Common.ObjType.Type.Document,
        Common.ObjType.Type.Assignement,
        Common.ObjType.Type.Task,
        Common.ObjType.Type.Databook
      };
    }
    
    #endregion
    
    
    #region Работа с БД
    
    /// <summary>
    /// Создать таблицу в БД.
    /// </summary>
    /// <param name="sourceTableName">Имя таблицы в БД.</param>
    /// <param name="query">Insert запрос.</param>
    /// <param name="isDrop">Удалить таблицу перед созданием, если она есть.</param>
    [Public]
    public static void CreateTable(string sourceTableName, string query, bool isDrop)
    {
      if (string.IsNullOrEmpty(sourceTableName) || string.IsNullOrEmpty(query))
        return;
      
      bool isCreate = false;
      
      if (isDrop)
      {
        Sungero.Docflow.PublicFunctions.Module.DropReportTempTable(sourceTableName);
        isCreate = true;
      }
      else
      {
        var result = ExecuteScalarSQLCommand(string.Format(Queries.Module.CheckTable, sourceTableName));
        if (result == "0")
          isCreate = true;
      }
      
      if (isCreate)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(query, new[] { sourceTableName });
    }
    
    /// <summary>
    /// Выполнить SQL-запрос.
    /// </summary>
    /// <param name="commandText">Форматируемая строка запроса.</param>
    /// <returns>Возвращает первый столбец первой строки.</returns>
    [Public]
    public static string ExecuteScalarSQLCommand(string commandText)
    {
      string result = string.Empty;
      
      using (var connection = Sungero.Core.SQL.CreateConnection())
      {
        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          var executionResult = command.ExecuteScalar();
          if (!(executionResult is DBNull) && executionResult != null)
            result = executionResult.ToString();
        }
      }
      
      return result;
    }
    
    #endregion
    
  }
}
