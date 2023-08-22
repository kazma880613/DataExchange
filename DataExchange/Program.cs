// See https://aka.ms/new-console-template for more information


using DataExchange;

InfoLoad _infoLoad = new InfoLoad();

dbService _dbService = new dbService();

_dbService.ODBC_SQL_Open(_infoLoad.SQLInfoLoad());

_infoLoad.requestInfoLoad();

_dbService.Collecting_data(InfoLoad._request.QueryString);

_dbService.ODBC_SQL_Close();