// See https://aka.ms/new-console-template for more information


using DataExchange;

InfoLoad _infoLoad = new InfoLoad();

dbService _dbService = new dbService();

_dbService.ODBC_SQL_Open(_infoLoad.SQLInfoLoad());

_dbService.doaction(_infoLoad.QueryInfoLoad());

_dbService.ODBC_SQL_Close();