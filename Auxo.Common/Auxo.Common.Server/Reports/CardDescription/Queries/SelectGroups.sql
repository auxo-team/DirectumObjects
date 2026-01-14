Select DISTINCT
  GroupNum, GroupName
FROM 
  Auxo_ReportCardDescription
Where
  ReportSessionId = @ReportSessionId