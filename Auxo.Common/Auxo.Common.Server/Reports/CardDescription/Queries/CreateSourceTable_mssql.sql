CREATE Table {0}
([ReportSessionId] [varchar](256) NOT NULL,
 [GroupNum] int,
 [GroupName] [varchar](max) NULL,
 [PropertyName] [varchar](max) NULL,
 [TypeInfo] [varchar](max) NULL,
 [IsRequired] [varchar](10) NOT NULL,
 [IsEnabled] [varchar](10) NOT NULL,
 [IsVisibility] [varchar](10) NOT NULL,
 [CanBeSearch] [varchar](10) NOT NULL,
 [ValuesInfo] [varchar](max) NULL)