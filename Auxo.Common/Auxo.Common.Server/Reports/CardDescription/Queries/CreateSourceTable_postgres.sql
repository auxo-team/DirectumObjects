CREATE Table {0}
(ReportSessionId varchar(256) NOT NULL,
 GroupNum int,
 GroupName citext,
 PropertyName citext,
 TypeInfo citext,
 IsRequired varchar(10) NOT NULL,
 IsEnabled varchar(10) NOT NULL,
 IsVisibility varchar(10) NOT NULL,
 CanBeSearch varchar(10) NOT NULL,
 ValuesInfo citext)