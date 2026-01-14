Select 
  GroupName, ObjectName, EventName, ActionName, CompanyCode
FROM 
  Auxo_ReportChanges
Where
  ReportSessionId = @ReportSessionId