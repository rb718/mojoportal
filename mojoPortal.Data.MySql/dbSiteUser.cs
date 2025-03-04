/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.
/// 
/// Note moved into separate class file from dbPortal 2007-11-03

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace mojoPortal.Data
{

	public static class DBSiteUser
	{

		public static IDataReader GetUserCountByYearMonth(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT ");
			sqlCommand.Append("YEAR(DateCreated) As Y,  ");
			sqlCommand.Append("MONTH(DateCreated) As M, ");
			sqlCommand.Append("CONCAT(YEAR(DateCreated), '-', MONTH(DateCreated)) As Label, ");
			sqlCommand.Append("COUNT(*) As Users ");

			sqlCommand.Append("FROM ");
			sqlCommand.Append("mp_Users ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("SiteID = ?SiteID ");
			sqlCommand.Append("GROUP BY YEAR(DateCreated), MONTH(DateCreated) ");
			sqlCommand.Append("ORDER BY YEAR(DateCreated), MONTH(DateCreated) ");
			sqlCommand.Append("; ");


			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);
		}


		public static IDataReader GetUserList(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT UserID, ");
			sqlCommand.Append("Name, ");
			sqlCommand.Append("PasswordSalt, ");
			sqlCommand.Append("Pwd, ");
			sqlCommand.Append("Email ");
			sqlCommand.Append("FROM mp_Users ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("SiteID = ?SiteID ");
			sqlCommand.Append("ORDER BY ");
			sqlCommand.Append("Email");
			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);
		}

		public static IDataReader GetSmartDropDownData(int siteId, string query, int rowsToGet)
		{

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT ");
			sqlCommand.Append("UserID, ");
			sqlCommand.Append("UserGuid, ");
			sqlCommand.Append("Email, ");
			sqlCommand.Append("FirstName, ");
			sqlCommand.Append("LastName, ");
			sqlCommand.Append("Name As SiteUser ");

			sqlCommand.Append("FROM mp_Users ");

			sqlCommand.Append("WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND (");
			sqlCommand.Append("(LOWER(Name) LIKE LOWER(?Query)) ");
			sqlCommand.Append("OR (LOWER(FirstName) LIKE LOWER(?Query)) ");
			sqlCommand.Append("OR (LOWER(LastName) LIKE LOWER(?Query)) ");
			sqlCommand.Append(") ");

			sqlCommand.Append("UNION ");

			sqlCommand.Append("SELECT ");
			sqlCommand.Append("UserID, ");
			sqlCommand.Append("UserGuid, ");
			sqlCommand.Append("Email, ");
			sqlCommand.Append("FirstName, ");
			sqlCommand.Append("LastName, ");
			sqlCommand.Append("Email As SiteUser ");

			sqlCommand.Append("FROM mp_Users ");
			sqlCommand.Append("WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND LOWER(Email) LIKE LOWER(?Query) ");

			sqlCommand.Append("ORDER BY SiteUser ");
			sqlCommand.Append("LIMIT " + rowsToGet.ToString());
			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?Query", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = query + "%";


			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);


		}

		public static IDataReader EmailLookup(int siteId, string query, int rowsToGet)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT ");
			sqlCommand.Append("UserID, ");
			sqlCommand.Append("UserGuid, ");
			sqlCommand.Append("Email ");

			sqlCommand.Append("FROM mp_Users ");

			sqlCommand.Append("WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND (");
			sqlCommand.Append("(LOWER(Email) LIKE LOWER(?Query)) ");
			sqlCommand.Append("OR (LOWER(Name) LIKE LOWER(?Query)) ");
			sqlCommand.Append("OR (LOWER(FirstName) LIKE LOWER(?Query)) ");
			sqlCommand.Append("OR (LOWER(LastName) LIKE LOWER(?Query)) ");
			sqlCommand.Append(") ");

			sqlCommand.Append("ORDER BY Email ");
			sqlCommand.Append("LIMIT " + rowsToGet.ToString());
			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?Query", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = query + "%";


			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);
		}

		public static int UserCount(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT COUNT(*) FROM mp_Users WHERE SiteID = ?SiteID;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;

		}

		public static int CountLockedOutUsers(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT COUNT(*) FROM mp_Users WHERE SiteID = ?SiteID AND IsLockedOut = 1;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;

		}

		public static int CountNotApprovedUsers(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT COUNT(*) FROM mp_Users WHERE SiteID = ?SiteID AND ApprovedForForums = 0;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;
		}

		public static int UserCount(int siteId, String nameBeginsWith, string nameFilterMode)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append(@"SELECT COUNT(*) FROM mp_Users 
				WHERE SiteID = ?SiteID 
				AND IsDeleted = 0 
				AND ProfileApproved = 1 ");

			switch (nameFilterMode)
			{
				case "display":
				default:
					sqlCommand.Append("AND Lower(Name) LIKE LOWER(?BeginsWith) ");
					break;
				case "lastname":
					sqlCommand.Append("AND Lower(LastName) LIKE LOWER(?BeginsWith) ");
					break;
				case "":
					break;
			}

			sqlCommand.Append("; ");

			List<MySqlParameter> arParams = new List<MySqlParameter>()
			{
				new MySqlParameter("?SiteID", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = siteId
				},
				new MySqlParameter("?BeginsWith", MySqlDbType.VarChar, 50)
				{
					Direction = ParameterDirection.Input,
					Value = nameBeginsWith + "%"
				}
			};

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams.ToArray()).ToString());

			return count;

		}

		public static int CountUsersByRegistrationDateRange(
			int siteId,
			DateTime beginDate,
			DateTime endDate)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT COUNT(*) FROM mp_Users WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND DateCreated >= ?BeginDate ");
			sqlCommand.Append("AND DateCreated < ?EndDate; ");

			MySqlParameter[] arParams = new MySqlParameter[3];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?BeginDate", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = beginDate;

			arParams[2] = new MySqlParameter("?EndDate", MySqlDbType.DateTime);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = endDate;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;

		}

		public static int CountOnlineSince(int siteId, DateTime sinceTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT COUNT(*) FROM mp_Users WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND LastActivityDate > ?SinceTime ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?SinceTime", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = sinceTime;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;

		}

		public static IDataReader GetUsersOnlineSince(int siteId, DateTime sinceTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * FROM mp_Users WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND LastActivityDate >= ?SinceTime   ");
			sqlCommand.Append("AND DisplayInMemberList = 1 ");
			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?SinceTime", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = sinceTime;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);


		}

		public static IDataReader GetTop50UsersOnlineSince(int siteId, DateTime sinceTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * FROM mp_Users WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND LastActivityDate >= ?SinceTime   ");
			sqlCommand.Append("ORDER BY LastActivityDate desc   ");
			sqlCommand.Append("LIMIT 50 ;   ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?SinceTime", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = sinceTime;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);


		}

		public static int GetNewestUserId(int siteId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT MAX(UserID) FROM mp_Users WHERE SiteID = ?SiteID;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return count;
		}



		public static int Count(int siteId, string userNameBeginsWith)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT Count(*) FROM mp_Users WHERE SiteID = ?SiteID ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND ProfileApproved = 1 ");

			if (userNameBeginsWith.Length > 0)
			{
				sqlCommand.Append(" AND Name  LIKE ?UserNameBeginsWith ");
			}
			sqlCommand.Append(" ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?UserNameBeginsWith", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = userNameBeginsWith + "%";

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams));

			return count;

		}


		public static IDataReader GetUserListPage(
			int siteId,
			int pageNumber,
			int pageSize,
			string beginsWith,
			int sortMode,
			string nameFilterMode,
			out int totalPages
		)
		{
			string commandText;

			// Create temporary table
			if (string.IsNullOrWhiteSpace(beginsWith))
			{
				commandText = @"
CREATE TEMPORARY TABLE IF NOT EXISTS PageIndexForUsers AS (
	SELECT UserID
	FROM mp_Users
	WHERE ProfileApproved = 1
	AND DisplayInMemberList = 1
	AND SiteID = ?SiteId
	AND IsDeleted = 0
	ORDER BY Name
)";
			}
			else
			{
				commandText = @"
CREATE TEMPORARY TABLE IF NOT EXISTS PageIndexForUsers AS (
	SELECT UserID
	FROM mp_Users
	WHERE ProfileApproved = 1
	AND DisplayInMemberList = 1
	AND SiteID = ?SiteID
	AND IsDeleted = 0
	AND (
		(?NameFilterMode = 'display' AND LOWER(Name) LIKE LOWER(?BeginsWith) + '%')
		OR (
			(?NameFilterMode = 'lastname' AND LOWER(LastName) LIKE LOWER(?BeginsWith) + '%')
			OR
			(?NameFilterMode = 'lastname' AND LOWER(Name) LIKE LOWER(?BeginsWith) + '%')
		)
		OR (?NameFilterMode <> 'display' AND ?NameFilterMode <> 'lastname' AND LOWER(Name) LIKE LOWER(?BeginsWith) + '%')
	)
	ORDER BY
		(CASE ?SortMode WHEN 1 THEN DateCreated END ) DESC,
		(CASE ?SortMode WHEN 2 THEN LastName END),
		(CASE ?SortMode WHEN 2 THEN FirstName END),
		Name
)
";
			}

			// Query from temporary table and then drop it
			commandText += @"
SELECT * FROM mp_Users u
JOIN #PageIndexForUsers p
ON u.UserID = p.UserID
WHERE u.ProfileApproved = 1
AND u.SiteID = ?SiteID
AND u.IsDeleted = 0
AND p.IndexID > ?PageLowerBound
AND p.IndexID < ?PageUpperBound
ORDER BY p.IndexID

DROP TABLE PageIndexForUsers";

			var pageLowerBound = (pageSize * pageNumber) - pageSize;
			var pageLowerUpper = pageLowerBound + pageSize + 1;
			var totalRows = UserCount(siteId, beginsWith, nameFilterMode);
			
			// VS says that one of the casts are redundant, but I remember it being an issue in the past so we'll just leave it
			totalPages = (int)Math.Ceiling((decimal)totalRows / (decimal)pageSize);

			var commandParameters = new MySqlParameter[]
			{
				new MySqlParameter("?SiteID", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = siteId
				},
				new MySqlParameter("?BeginsWith", MySqlDbType.VarChar, 50)
				{
					Direction = ParameterDirection.Input,
					Value = beginsWith + "%"
				},
				new MySqlParameter("?PageLowerBound", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = pageLowerBound
				},
				new MySqlParameter("?PageUpperBound", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = pageLowerUpper
				},
				new MySqlParameter("?NameFilterMode", MySqlDbType.VarChar, 10)
				{
					Direction = ParameterDirection.Input,
					Value = nameFilterMode
				},
				new MySqlParameter("?SortMode", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = sortMode
				},
			};

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				commandText,
				commandParameters
			);
		}


		private static int CountForSearch(int siteId, string searchInput)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append(@"SELECT Count(*) FROM mp_Users WHERE SiteID = ?SiteID
				AND ProfileApproved = 1 
				AND DisplayInMemberList = 1 
				AND IsDeleted = 0 ");

			if (searchInput.Length > 0)
			{
				sqlCommand.Append(@" AND (
					(Lower(Name) LIKE LOWER(?SearchInput)) 
					OR 
					(Lower(LoginName) LIKE LOWER(?SearchInput))
					OR
					(Lower(LastName) LIKE LOWER(?SearchInput)) 
					OR
					(Lower(FirstName) LIKE LOWER(?SearchInput)) 
					)");
			}
			sqlCommand.Append(" ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?SearchInput", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = "%" + searchInput + "%";

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams));

			return count;

		}

		public static IDataReader GetUserSearchPage(
			int siteId,
			int pageNumber,
			int pageSize,
			string searchInput,
			int sortMode,
			out int totalPages)
		{
			StringBuilder sqlCommand = new StringBuilder();
			int pageLowerBound = (pageSize * pageNumber) - pageSize;

			int totalRows
				= CountForSearch(siteId, searchInput);

			totalPages = 1;
			if (pageSize > 0)
				totalPages = totalRows / pageSize;

			if (totalRows <= pageSize)
			{
				totalPages = 1;
			}
			else
			{
				int remainder;
				Math.DivRem(totalRows, pageSize, out remainder);
				if (remainder > 0)
				{
					totalPages += 1;
				}
			}


			sqlCommand.Append("SELECT *  ");
			sqlCommand.Append("FROM	mp_Users  ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID    ");
			sqlCommand.Append("AND ProfileApproved = 1 ");
			sqlCommand.Append("AND DisplayInMemberList = 1 ");
			sqlCommand.Append("AND IsDeleted = 0 ");

			if (searchInput.Length > 0)
			{
				sqlCommand.Append(@" AND (
					(Lower(Name) LIKE LOWER(?SearchInput)) 
					OR 
					(Lower(LoginName) LIKE LOWER(?SearchInput))
					OR
					(Lower(LastName) LIKE LOWER(?SearchInput)) 
					OR
					(Lower(FirstName) LIKE LOWER(?SearchInput)) 
					)");
			}

			switch (sortMode)
			{
				case 1:
					sqlCommand.Append(" ORDER BY DateCreated DESC ");
					break;

				case 2:
					sqlCommand.Append(" ORDER BY LastName, FirstName,  Name ");
					break;

				case 0:
				default:
					sqlCommand.Append(" ORDER BY Name ");
					break;
			}

			sqlCommand.Append("LIMIT "
				+ pageLowerBound.ToString(CultureInfo.InvariantCulture)
				+ ", ?PageSize  ; ");

			MySqlParameter[] arParams = new MySqlParameter[3];


			arParams[0] = new MySqlParameter("?PageSize", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = pageSize;

			arParams[1] = new MySqlParameter("?SearchInput", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = "%" + searchInput + "%";

			arParams[2] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = siteId;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		private static int CountForAdminSearch(int siteId, string searchInput)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT Count(*) FROM mp_Users WHERE SiteID = ?SiteID ");
			if (searchInput.Length > 0)
			{
				sqlCommand.Append(" AND ");
				sqlCommand.Append("(");

				sqlCommand.Append(" (Name LIKE ?SearchInput) ");
				sqlCommand.Append(" OR ");
				sqlCommand.Append(" (LoginName LIKE ?SearchInput) ");
				sqlCommand.Append(" OR ");
				sqlCommand.Append(" (Email LIKE ?SearchInput) ");

				sqlCommand.Append(")");
			}
			sqlCommand.Append(" ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?SearchInput", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = "%" + searchInput + "%";

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams));

			return count;

		}

		public static IDataReader GetUserAdminSearchPage(
			int siteId,
			int pageNumber,
			int pageSize,
			string searchInput,
			int sortMode,
			out int totalPages)
		{

			int pageLowerBound = (pageSize * pageNumber) - pageSize;

			int totalRows = CountForAdminSearch(siteId, searchInput);

			totalPages = 1;
			if (pageSize > 0)
				totalPages = totalRows / pageSize;

			if (totalRows <= pageSize)
			{
				totalPages = 1;
			}
			else
			{
				int remainder;
				Math.DivRem(totalRows, pageSize, out remainder);
				if (remainder > 0)
				{
					totalPages += 1;
				}
			}

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT *  ");
			sqlCommand.Append("FROM	mp_Users  ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID    ");

			if (searchInput.Length > 0)
			{
				sqlCommand.Append(@" AND (
					(Lower(Name) LIKE LOWER(?SearchInput)) 
					OR 
					(Lower(LoginName) LIKE LOWER(?SearchInput))
					OR
					(Lower(LastName) LIKE LOWER(?SearchInput)) 
					OR
					(Lower(FirstName) LIKE LOWER(?SearchInput)) 
					OR
					(Lower(Email) LIKE LOWER(?SearchInput)) 
					)");
			}

			switch (sortMode)
			{
				case 1:
					sqlCommand.Append(" ORDER BY DateCreated DESC ");
					break;

				case 2:
					sqlCommand.Append(" ORDER BY LastName, FirstName,  Name ");
					break;

				case 0:
				default:
					sqlCommand.Append(" ORDER BY Name ");
					break;
			}

			sqlCommand.Append("LIMIT "
				+ pageLowerBound.ToString(CultureInfo.InvariantCulture)
				+ ", ?PageSize  ; ");

			MySqlParameter[] arParams = new MySqlParameter[3];


			arParams[0] = new MySqlParameter("?PageSize", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = pageSize;

			arParams[1] = new MySqlParameter("?SearchInput", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = "%" + searchInput + "%";

			arParams[2] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = siteId;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static IDataReader GetPageLockedUsers(
			int siteId,
			int pageNumber,
			int pageSize,
			out int totalPages)
		{
			totalPages = 1;
			int totalRows = CountLockedOutUsers(siteId);
			int pageLowerBound = (pageSize * pageNumber) - pageSize;

			if (pageSize > 0)
				totalPages = totalRows / pageSize;

			if (totalRows <= pageSize)
			{
				totalPages = 1;
			}
			else
			{
				int remainder;
				Math.DivRem(totalRows, pageSize, out remainder);
				if (remainder > 0)
				{
					totalPages += 1;
				}
			}

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT *  ");
			sqlCommand.Append("FROM	mp_Users  ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID    ");
			sqlCommand.Append("AND ");
			sqlCommand.Append("IsLockedOut = 1 ");

			sqlCommand.Append(" ORDER BY Name ");
			sqlCommand.Append("LIMIT "
				+ pageLowerBound.ToString(CultureInfo.InvariantCulture)
				+ ", ?PageSize  ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?PageSize", MySqlDbType.Int32);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = pageSize;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static IDataReader GetPageNotApprovedUsers(
			int siteId,
			int pageNumber,
			int pageSize,
			out int totalPages)
		{
			totalPages = 1;
			int totalRows = CountNotApprovedUsers(siteId);
			int pageLowerBound = (pageSize * pageNumber) - pageSize;

			if (pageSize > 0)
				totalPages = totalRows / pageSize;

			if (totalRows <= pageSize)
			{
				totalPages = 1;
			}
			else
			{
				int remainder;
				Math.DivRem(totalRows, pageSize, out remainder);
				if (remainder > 0)
				{
					totalPages += 1;
				}
			}

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT *  ");
			sqlCommand.Append("FROM	mp_Users  ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID    ");
			sqlCommand.Append("AND ");
			sqlCommand.Append("ApprovedForForums = 0 ");

			sqlCommand.Append(" ORDER BY Name ");
			sqlCommand.Append("LIMIT "
				+ pageLowerBound.ToString(CultureInfo.InvariantCulture)
				+ ", ?PageSize  ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?PageSize", MySqlDbType.Int32);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = pageSize;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}




		public static int AddUser(
			Guid siteGuid,
			int siteId,
			string fullName,
			String loginName,
			string email,
			string password,
			string passwordSalt,
			Guid userGuid,
			DateTime dateCreated,
			bool mustChangePwd,
			string firstName,
			string lastName,
			string timeZoneId,
			DateTime dateOfBirth,
			bool emailConfirmed,
			int pwdFormat,
			string passwordHash,
			string securityStamp,
			string phoneNumber,
			bool phoneNumberConfirmed,
			bool twoFactorEnabled,
			DateTime? lockoutEndDateUtc)
		{
			#region bit conversion

			int intmustChangePwd = 0;
			if (mustChangePwd)
			{ intmustChangePwd = 1; }
			int intEmailConfirmed = 0;
			if (emailConfirmed)
			{ intEmailConfirmed = 1; }
			int intPhoneNumberConfirmed = 0;
			if (phoneNumberConfirmed)
			{ intPhoneNumberConfirmed = 1; }
			int intTwoFactorEnabled = 0;
			if (twoFactorEnabled)
			{ intTwoFactorEnabled = 1; }

			#endregion

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("INSERT INTO mp_Users (");
			sqlCommand.Append("SiteGuid, ");
			sqlCommand.Append("SiteID, ");
			sqlCommand.Append("Name, ");
			sqlCommand.Append("LoginName, ");
			sqlCommand.Append("Email, ");

			sqlCommand.Append("FirstName, ");
			sqlCommand.Append("LastName, ");
			sqlCommand.Append("TimeZoneId, ");
			sqlCommand.Append("EmailChangeGuid, ");
			sqlCommand.Append("PasswordResetGuid, ");

			sqlCommand.Append("Pwd, ");
			sqlCommand.Append("PasswordSalt, ");
			sqlCommand.Append("MustChangePwd, ");
			sqlCommand.Append("RolesChanged, ");
			sqlCommand.Append("DateCreated, ");
			sqlCommand.Append("TotalPosts, ");
			sqlCommand.Append("TotalRevenue, ");
			sqlCommand.Append("DateOfBirth, ");

			sqlCommand.Append("EmailConfirmed, ");
			sqlCommand.Append("PwdFormat, ");
			sqlCommand.Append("PasswordHash, ");
			sqlCommand.Append("SecurityStamp, ");
			sqlCommand.Append("PhoneNumber, ");
			sqlCommand.Append("PhoneNumberConfirmed, ");
			sqlCommand.Append("TwoFactorEnabled, ");
			sqlCommand.Append("LockoutEndDateUtc, ");

			sqlCommand.Append("UserGuid");
			sqlCommand.Append(")");

			sqlCommand.Append("VALUES (");
			sqlCommand.Append(" ?SiteGuid , ");
			sqlCommand.Append(" ?SiteID , ");
			sqlCommand.Append(" ?FullName , ");
			sqlCommand.Append(" ?LoginName , ");
			sqlCommand.Append(" ?Email , ");

			sqlCommand.Append("?FirstName, ");
			sqlCommand.Append("?LastName, ");
			sqlCommand.Append("?TimeZoneId, ");
			sqlCommand.Append("?EmailChangeGuid, ");
			sqlCommand.Append("'00000000-0000-0000-0000-000000000000', ");

			sqlCommand.Append(" ?Password, ");
			sqlCommand.Append("?PasswordSalt, ");
			sqlCommand.Append("?MustChangePwd, ");
			sqlCommand.Append(" 0, ");
			sqlCommand.Append(" ?DateCreated, ");
			sqlCommand.Append(" 0, ");
			sqlCommand.Append(" 0, ");
			sqlCommand.Append("?DateOfBirth, ");

			sqlCommand.Append("?EmailConfirmed, ");
			sqlCommand.Append("?PwdFormat, ");
			sqlCommand.Append("?PasswordHash, ");
			sqlCommand.Append("?SecurityStamp, ");
			sqlCommand.Append("?PhoneNumber, ");
			sqlCommand.Append("?PhoneNumberConfirmed, ");
			sqlCommand.Append("?TwoFactorEnabled, ");
			sqlCommand.Append("?LockoutEndDateUtc, ");

			sqlCommand.Append(" ?UserGuid ");

			sqlCommand.Append(");");
			sqlCommand.Append("SELECT LAST_INSERT_ID();");

			MySqlParameter[] arParams = new MySqlParameter[23];

			arParams[0] = new MySqlParameter("?FullName", MySqlDbType.VarChar, 100);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = fullName;

			arParams[1] = new MySqlParameter("?LoginName", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = loginName;

			arParams[2] = new MySqlParameter("?Email", MySqlDbType.VarChar, 100);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = email;

			arParams[3] = new MySqlParameter("?Password", MySqlDbType.Text);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = password;

			arParams[4] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = siteId;

			arParams[5] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = userGuid.ToString();

			arParams[6] = new MySqlParameter("?DateCreated", MySqlDbType.DateTime);
			arParams[6].Direction = ParameterDirection.Input;
			arParams[6].Value = dateCreated;

			arParams[7] = new MySqlParameter("?SiteGuid", MySqlDbType.VarChar, 36);
			arParams[7].Direction = ParameterDirection.Input;
			arParams[7].Value = siteGuid.ToString();

			arParams[8] = new MySqlParameter("?MustChangePwd", MySqlDbType.Int32);
			arParams[8].Direction = ParameterDirection.Input;
			arParams[8].Value = intmustChangePwd;

			arParams[9] = new MySqlParameter("?FirstName", MySqlDbType.VarChar, 100);
			arParams[9].Direction = ParameterDirection.Input;
			arParams[9].Value = firstName;

			arParams[10] = new MySqlParameter("?LastName", MySqlDbType.VarChar, 100);
			arParams[10].Direction = ParameterDirection.Input;
			arParams[10].Value = lastName;

			arParams[11] = new MySqlParameter("?TimeZoneId", MySqlDbType.VarChar, 100);
			arParams[11].Direction = ParameterDirection.Input;
			arParams[11].Value = timeZoneId;

			arParams[12] = new MySqlParameter("?EmailChangeGuid", MySqlDbType.VarChar, 36);
			arParams[12].Direction = ParameterDirection.Input;
			arParams[12].Value = Guid.Empty.ToString();

			arParams[13] = new MySqlParameter("?PasswordSalt", MySqlDbType.VarChar, 128);
			arParams[13].Direction = ParameterDirection.Input;
			arParams[13].Value = passwordSalt;

			arParams[14] = new MySqlParameter("?DateOfBirth", MySqlDbType.DateTime);
			arParams[14].Direction = ParameterDirection.Input;
			if (dateOfBirth == DateTime.MinValue)
			{
				arParams[14].Value = DBNull.Value;
			}
			else
			{
				arParams[14].Value = dateOfBirth;
			}

			arParams[15] = new MySqlParameter("?EmailConfirmed", MySqlDbType.Int32);
			arParams[15].Direction = ParameterDirection.Input;
			arParams[15].Value = intEmailConfirmed;

			arParams[16] = new MySqlParameter("?PwdFormat", MySqlDbType.Int32);
			arParams[16].Direction = ParameterDirection.Input;
			arParams[16].Value = pwdFormat;

			arParams[17] = new MySqlParameter("?PasswordHash", MySqlDbType.Text);
			arParams[17].Direction = ParameterDirection.Input;
			arParams[17].Value = passwordHash;

			arParams[18] = new MySqlParameter("?SecurityStamp", MySqlDbType.Text);
			arParams[18].Direction = ParameterDirection.Input;
			arParams[18].Value = securityStamp;

			arParams[19] = new MySqlParameter("?PhoneNumber", MySqlDbType.VarChar, 50);
			arParams[19].Direction = ParameterDirection.Input;
			arParams[19].Value = phoneNumber;

			arParams[20] = new MySqlParameter("?PhoneNumberConfirmed", MySqlDbType.Int32);
			arParams[20].Direction = ParameterDirection.Input;
			arParams[20].Value = intPhoneNumberConfirmed;

			arParams[21] = new MySqlParameter("?TwoFactorEnabled", MySqlDbType.Int32);
			arParams[21].Direction = ParameterDirection.Input;
			arParams[21].Value = intTwoFactorEnabled;

			arParams[22] = new MySqlParameter("?LockoutEndDateUtc", MySqlDbType.DateTime);
			arParams[22].Direction = ParameterDirection.Input;
			if (lockoutEndDateUtc == null)
			{
				arParams[22].Value = DBNull.Value;
			}
			else
			{
				arParams[22].Value = lockoutEndDateUtc;
			}

			int newID = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams).ToString());

			return newID;

		}

		public static bool UpdateUser(
			int userId,
			String fullName,
			String loginName,
			String email,
			String password,
			string passwordSalt,
			String gender,
			bool profileApproved,
			bool approvedForForums,
			bool trusted,
			bool displayInMemberList,
			String webSiteUrl,
			String country,
			String state,
			String occupation,
			String interests,
			String msn,
			String yahoo,
			String aim,
			String icq,
			String avatarUrl,
			String signature,
			String skin,
			String loweredEmail,
			String passwordQuestion,
			String passwordAnswer,
			String comment,
			int timeOffsetHours,
			string openIdUri,
			string windowsLiveId,
			bool mustChangePwd,
			string firstName,
			string lastName,
			string timeZoneId,
			string editorPreference,
			string newEmail,
			Guid emailChangeGuid,
			Guid passwordResetGuid,
			bool rolesChanged,
			string authorBio,
			DateTime dateOfBirth,
			bool emailConfirmed,
			int pwdFormat,
			string passwordHash,
			string securityStamp,
			string phoneNumber,
			bool phoneNumberConfirmed,
			bool twoFactorEnabled,
			DateTime? lockoutEndDateUtc)
		{
			#region bit conversion

			byte approved = 1;
			if (!profileApproved)
			{
				approved = 0;
			}

			byte canPost = 1;
			if (!approvedForForums)
			{
				canPost = 0;
			}

			byte trust = 1;
			if (!trusted)
			{
				trust = 0;
			}

			byte displayInList = 1;
			if (!displayInMemberList)
			{
				displayInList = 0;
			}
			int intmustChangePwd = 0;
			if (mustChangePwd)
			{ intmustChangePwd = 1; }

			int introlesChanged = 0;
			if (rolesChanged)
			{ introlesChanged = 1; }
			int intEmailConfirmed = 0;
			if (emailConfirmed)
			{ intEmailConfirmed = 1; }

			int intPhoneNumberConfirmed = 0;
			if (phoneNumberConfirmed)
			{ intPhoneNumberConfirmed = 1; }

			int intTwoFactorEnabled = 0;
			if (twoFactorEnabled)
			{ intTwoFactorEnabled = 1; }

			#endregion


			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET Email = ?Email ,   ");
			sqlCommand.Append("Name = ?FullName,    ");
			sqlCommand.Append("LoginName = ?LoginName,    ");

			sqlCommand.Append("FirstName = ?FirstName,    ");
			sqlCommand.Append("LastName = ?LastName,    ");
			sqlCommand.Append("TimeZoneId = ?TimeZoneId,    ");
			sqlCommand.Append("EditorPreference = ?EditorPreference,    ");
			sqlCommand.Append("NewEmail = ?NewEmail,    ");
			sqlCommand.Append("EmailChangeGuid = ?EmailChangeGuid,    ");
			sqlCommand.Append("PasswordResetGuid = ?PasswordResetGuid,    ");

			sqlCommand.Append("Pwd = ?Password,    ");
			sqlCommand.Append("PasswordSalt = ?PasswordSalt,    ");
			sqlCommand.Append("MustChangePwd = ?MustChangePwd,    ");
			sqlCommand.Append("RolesChanged = ?RolesChanged,    ");
			sqlCommand.Append("Gender = ?Gender,    ");
			sqlCommand.Append("ProfileApproved = ?ProfileApproved,    ");
			sqlCommand.Append("ApprovedForForums = ?ApprovedForForums,    ");
			sqlCommand.Append("Trusted = ?Trusted,    ");
			sqlCommand.Append("DisplayInMemberList = ?DisplayInMemberList,    ");
			sqlCommand.Append("WebSiteURL = ?WebSiteURL,    ");
			sqlCommand.Append("Country = ?Country,    ");
			sqlCommand.Append("State = ?State,    ");
			sqlCommand.Append("Occupation = ?Occupation,    ");
			sqlCommand.Append("Interests = ?Interests,    ");
			sqlCommand.Append("MSN = ?MSN,    ");
			sqlCommand.Append("Yahoo = ?Yahoo,   ");
			sqlCommand.Append("AIM = ?AIM,   ");
			sqlCommand.Append("ICQ = ?ICQ,    ");
			sqlCommand.Append("AvatarUrl = ?AvatarUrl,    ");
			sqlCommand.Append("Signature = ?Signature,    ");
			sqlCommand.Append("Skin = ?Skin,    ");

			sqlCommand.Append("LoweredEmail = ?LoweredEmail, ");
			sqlCommand.Append("PasswordQuestion = ?PasswordQuestion, ");
			sqlCommand.Append("PasswordAnswer = ?PasswordAnswer, ");
			sqlCommand.Append("Comment = ?Comment, ");
			sqlCommand.Append("OpenIDURI = ?OpenIDURI, ");
			sqlCommand.Append("WindowsLiveID = ?WindowsLiveID, ");
			sqlCommand.Append("AuthorBio = ?AuthorBio, ");
			sqlCommand.Append("DateOfBirth = ?DateOfBirth, ");

			sqlCommand.Append("EmailConfirmed = ?EmailConfirmed, ");
			sqlCommand.Append("PwdFormat = ?PwdFormat, ");
			sqlCommand.Append("PasswordHash = ?PasswordHash, ");
			sqlCommand.Append("SecurityStamp = ?SecurityStamp, ");
			sqlCommand.Append("PhoneNumber = ?PhoneNumber, ");
			sqlCommand.Append("PhoneNumberConfirmed = ?PhoneNumberConfirmed, ");
			sqlCommand.Append("TwoFactorEnabled = ?TwoFactorEnabled, ");
			sqlCommand.Append("LockoutEndDateUtc = ?LockoutEndDateUtc, ");

			sqlCommand.Append("TimeOffsetHours = ?TimeOffsetHours    ");

			sqlCommand.Append("WHERE UserID = ?UserID ;");

			MySqlParameter[] arParams = new MySqlParameter[49];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			arParams[1] = new MySqlParameter("?Email", MySqlDbType.VarChar, 100);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = email;

			arParams[2] = new MySqlParameter("?Password", MySqlDbType.Text);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = password;

			arParams[3] = new MySqlParameter("?Gender", MySqlDbType.VarChar, 1);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = gender;

			arParams[4] = new MySqlParameter("?ProfileApproved", MySqlDbType.Int32);
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = approved;

			arParams[5] = new MySqlParameter("?ApprovedForForums", MySqlDbType.Int32);
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = canPost;

			arParams[6] = new MySqlParameter("?Trusted", MySqlDbType.Int32);
			arParams[6].Direction = ParameterDirection.Input;
			arParams[6].Value = trust;

			arParams[7] = new MySqlParameter("?DisplayInMemberList", MySqlDbType.Int32);
			arParams[7].Direction = ParameterDirection.Input;
			arParams[7].Value = displayInList;

			arParams[8] = new MySqlParameter("?WebSiteURL", MySqlDbType.VarChar, 100);
			arParams[8].Direction = ParameterDirection.Input;
			arParams[8].Value = webSiteUrl;

			arParams[9] = new MySqlParameter("?Country", MySqlDbType.VarChar, 100);
			arParams[9].Direction = ParameterDirection.Input;
			arParams[9].Value = country;

			arParams[10] = new MySqlParameter("?State", MySqlDbType.VarChar, 100);
			arParams[10].Direction = ParameterDirection.Input;
			arParams[10].Value = state;

			arParams[11] = new MySqlParameter("?Occupation", MySqlDbType.VarChar, 100);
			arParams[11].Direction = ParameterDirection.Input;
			arParams[11].Value = occupation;

			arParams[12] = new MySqlParameter("?Interests", MySqlDbType.VarChar, 100);
			arParams[12].Direction = ParameterDirection.Input;
			arParams[12].Value = interests;

			arParams[13] = new MySqlParameter("?MSN", MySqlDbType.VarChar, 100);
			arParams[13].Direction = ParameterDirection.Input;
			arParams[13].Value = msn;

			arParams[14] = new MySqlParameter("?Yahoo", MySqlDbType.VarChar, 100);
			arParams[14].Direction = ParameterDirection.Input;
			arParams[14].Value = yahoo;

			arParams[15] = new MySqlParameter("?AIM", MySqlDbType.VarChar, 100);
			arParams[15].Direction = ParameterDirection.Input;
			arParams[15].Value = aim;

			arParams[16] = new MySqlParameter("?ICQ", MySqlDbType.VarChar, 100);
			arParams[16].Direction = ParameterDirection.Input;
			arParams[16].Value = icq;

			arParams[17] = new MySqlParameter("?AvatarUrl", MySqlDbType.VarChar, 100);
			arParams[17].Direction = ParameterDirection.Input;
			arParams[17].Value = avatarUrl;

			arParams[18] = new MySqlParameter("?Signature", MySqlDbType.Text);
			arParams[18].Direction = ParameterDirection.Input;
			arParams[18].Value = signature;

			arParams[19] = new MySqlParameter("?Skin", MySqlDbType.VarChar, 100);
			arParams[19].Direction = ParameterDirection.Input;
			arParams[19].Value = skin;

			arParams[20] = new MySqlParameter("?FullName", MySqlDbType.VarChar, 100);
			arParams[20].Direction = ParameterDirection.Input;
			arParams[20].Value = fullName;

			arParams[21] = new MySqlParameter("?LoginName", MySqlDbType.VarChar, 50);
			arParams[21].Direction = ParameterDirection.Input;
			arParams[21].Value = loginName;

			arParams[22] = new MySqlParameter("?LoweredEmail", MySqlDbType.VarChar, 100);
			arParams[22].Direction = ParameterDirection.Input;
			arParams[22].Value = loweredEmail;

			arParams[23] = new MySqlParameter("?PasswordQuestion", MySqlDbType.VarChar, 255);
			arParams[23].Direction = ParameterDirection.Input;
			arParams[23].Value = passwordQuestion;

			arParams[24] = new MySqlParameter("?PasswordAnswer", MySqlDbType.VarChar, 255);
			arParams[24].Direction = ParameterDirection.Input;
			arParams[24].Value = passwordAnswer;

			arParams[25] = new MySqlParameter("?Comment", MySqlDbType.Text);
			arParams[25].Direction = ParameterDirection.Input;
			arParams[25].Value = comment;

			arParams[26] = new MySqlParameter("?TimeOffsetHours", MySqlDbType.Int32);
			arParams[26].Direction = ParameterDirection.Input;
			arParams[26].Value = timeOffsetHours;

			arParams[27] = new MySqlParameter("?OpenIDURI", MySqlDbType.VarChar, 255);
			arParams[27].Direction = ParameterDirection.Input;
			arParams[27].Value = openIdUri;

			arParams[28] = new MySqlParameter("?WindowsLiveID", MySqlDbType.VarChar, 36);
			arParams[28].Direction = ParameterDirection.Input;
			arParams[28].Value = windowsLiveId;

			arParams[29] = new MySqlParameter("?MustChangePwd", MySqlDbType.Int32);
			arParams[29].Direction = ParameterDirection.Input;
			arParams[29].Value = intmustChangePwd;

			arParams[30] = new MySqlParameter("?FirstName", MySqlDbType.VarChar, 100);
			arParams[30].Direction = ParameterDirection.Input;
			arParams[30].Value = firstName;

			arParams[31] = new MySqlParameter("?LastName", MySqlDbType.VarChar, 100);
			arParams[31].Direction = ParameterDirection.Input;
			arParams[31].Value = lastName;

			arParams[32] = new MySqlParameter("?TimeZoneId", MySqlDbType.VarChar, 100);
			arParams[32].Direction = ParameterDirection.Input;
			arParams[32].Value = timeZoneId;

			arParams[33] = new MySqlParameter("?EditorPreference", MySqlDbType.VarChar, 100);
			arParams[33].Direction = ParameterDirection.Input;
			arParams[33].Value = editorPreference;

			arParams[34] = new MySqlParameter("?NewEmail", MySqlDbType.VarChar, 100);
			arParams[34].Direction = ParameterDirection.Input;
			arParams[34].Value = newEmail;

			arParams[35] = new MySqlParameter("?EmailChangeGuid", MySqlDbType.VarChar, 36);
			arParams[35].Direction = ParameterDirection.Input;
			arParams[35].Value = emailChangeGuid.ToString();

			arParams[36] = new MySqlParameter("?PasswordResetGuid", MySqlDbType.VarChar, 36);
			arParams[36].Direction = ParameterDirection.Input;
			arParams[36].Value = passwordResetGuid.ToString();

			arParams[37] = new MySqlParameter("?PasswordSalt", MySqlDbType.VarChar, 128);
			arParams[37].Direction = ParameterDirection.Input;
			arParams[37].Value = passwordSalt;

			arParams[38] = new MySqlParameter("?RolesChanged", MySqlDbType.Int32);
			arParams[38].Direction = ParameterDirection.Input;
			arParams[38].Value = introlesChanged;

			arParams[39] = new MySqlParameter("?AuthorBio", MySqlDbType.Text);
			arParams[39].Direction = ParameterDirection.Input;
			arParams[39].Value = authorBio;

			arParams[40] = new MySqlParameter("?DateOfBirth", MySqlDbType.DateTime);
			arParams[40].Direction = ParameterDirection.Input;
			if (dateOfBirth == DateTime.MinValue)
			{
				arParams[40].Value = DBNull.Value;
			}
			else
			{
				arParams[40].Value = dateOfBirth;
			}

			arParams[41] = new MySqlParameter("?EmailConfirmed", MySqlDbType.Int32);
			arParams[41].Direction = ParameterDirection.Input;
			arParams[41].Value = intEmailConfirmed;

			arParams[42] = new MySqlParameter("?PwdFormat", MySqlDbType.Int32);
			arParams[42].Direction = ParameterDirection.Input;
			arParams[42].Value = pwdFormat;

			arParams[43] = new MySqlParameter("?PasswordHash", MySqlDbType.Text);
			arParams[43].Direction = ParameterDirection.Input;
			arParams[43].Value = passwordHash;

			arParams[44] = new MySqlParameter("?SecurityStamp", MySqlDbType.Text);
			arParams[44].Direction = ParameterDirection.Input;
			arParams[44].Value = securityStamp;

			arParams[45] = new MySqlParameter("?PhoneNumber", MySqlDbType.VarChar, 50);
			arParams[45].Direction = ParameterDirection.Input;
			arParams[45].Value = phoneNumber;

			arParams[46] = new MySqlParameter("?PhoneNumberConfirmed", MySqlDbType.Int32);
			arParams[46].Direction = ParameterDirection.Input;
			arParams[46].Value = intPhoneNumberConfirmed;

			arParams[47] = new MySqlParameter("?TwoFactorEnabled", MySqlDbType.Int32);
			arParams[47].Direction = ParameterDirection.Input;
			arParams[47].Value = intTwoFactorEnabled;

			arParams[48] = new MySqlParameter("?LockoutEndDateUtc", MySqlDbType.DateTime);
			arParams[48].Direction = ParameterDirection.Input;
			if (lockoutEndDateUtc == null)
			{
				arParams[48].Value = DBNull.Value;
			}
			else
			{
				arParams[48].Value = lockoutEndDateUtc;
			}

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}


		public static bool DeleteUser(int userId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("DELETE FROM mp_Users ");
			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);


			return (rowsAffected > 0);
		}

		public static bool UpdateLastActivityTime(Guid userGuid, DateTime lastUpdate)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET LastActivityDate = ?LastUpdate  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?LastUpdate", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = lastUpdate;

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateLastLoginTime(Guid userGuid, DateTime lastLoginTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET LastLoginDate = ?LastLoginTime,  ");
			sqlCommand.Append("FailedPasswordAttemptCount = 0, ");
			sqlCommand.Append("FailedPwdAnswerAttemptCount = 0 ");

			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?LastLoginTime", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = lastLoginTime;

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool AccountLockout(Guid userGuid, DateTime lockoutTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET IsLockedOut = 1,  ");
			sqlCommand.Append("LastLockoutDate = ?LockoutTime  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?LockoutTime", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = lockoutTime;

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateLastPasswordChangeTime(Guid userGuid, DateTime lastPasswordChangeTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("LastPasswordChangedDate = ?LastPasswordChangedDate  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?LastPasswordChangedDate", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = lastPasswordChangeTime;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateFailedPasswordAttemptStartWindow(
			Guid userGuid,
			DateTime windowStartTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("FailedPwdAttemptWindowStart = ?FailedPasswordAttemptWindowStart  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?FailedPasswordAttemptWindowStart", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = windowStartTime;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateFailedPasswordAttemptCount(
			Guid userGuid,
			int attemptCount)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("FailedPasswordAttemptCount = ?AttemptCount  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?AttemptCount", MySqlDbType.Int32);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = attemptCount;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateFailedPasswordAnswerAttemptStartWindow(
			Guid userGuid,
			DateTime windowStartTime)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("FailedPwdAnswerWindowStart = ?FailedPasswordAnswerAttemptWindowStart  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?FailedPasswordAnswerAttemptWindowStart", MySqlDbType.DateTime);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = windowStartTime;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdateFailedPasswordAnswerAttemptCount(
			Guid userGuid,
			int attemptCount)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("FailedPwdAnswerAttemptCount = ?AttemptCount  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?AttemptCount", MySqlDbType.Int32);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = attemptCount;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool SetRegistrationConfirmationGuid(Guid userGuid, Guid registrationConfirmationGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("IsLockedOut = 1,  ");
			sqlCommand.Append("RegisterConfirmGuid = ?RegisterConfirmGuid  ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?RegisterConfirmGuid", MySqlDbType.VarChar, 36);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = registrationConfirmationGuid.ToString();

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool ConfirmRegistration(Guid emptyGuid, Guid registrationConfirmationGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET   ");
			sqlCommand.Append("IsLockedOut = 0,  ");
			sqlCommand.Append("RegisterConfirmGuid = ?EmptyGuid  ");
			sqlCommand.Append("WHERE RegisterConfirmGuid = ?RegisterConfirmGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?EmptyGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = emptyGuid.ToString();

			arParams[1] = new MySqlParameter("?RegisterConfirmGuid", MySqlDbType.VarChar, 36);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = registrationConfirmationGuid.ToString();

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static bool AccountClearLockout(Guid userGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET IsLockedOut = 0,  ");
			sqlCommand.Append("FailedPasswordAttemptCount = 0, ");
			sqlCommand.Append("FailedPwdAnswerAttemptCount = 0 ");

			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static bool UpdatePasswordAndSalt(
			int userId,
			int pwdFormat,
			string password,
			string passwordSalt)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET Pwd = ?Password,  ");
			sqlCommand.Append("PasswordSalt = ?PasswordSalt,  ");
			sqlCommand.Append("PwdFormat = ?PwdFormat  ");

			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[4];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			arParams[1] = new MySqlParameter("?Password", MySqlDbType.Text);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = password;

			arParams[2] = new MySqlParameter("?PasswordSalt", MySqlDbType.VarChar, 128);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = passwordSalt;

			arParams[3] = new MySqlParameter("?PwdFormat", MySqlDbType.Int32);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = pwdFormat;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);

		}

		public static bool UpdatePasswordQuestionAndAnswer(
			Guid userGuid,
			String passwordQuestion,
			String passwordAnswer)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET PasswordQuestion = ?PasswordQuestion,  ");
			sqlCommand.Append("PasswordAnswer = ?PasswordAnswer  ");

			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[3];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?PasswordQuestion", MySqlDbType.VarChar, 255);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = passwordQuestion;

			arParams[2] = new MySqlParameter("?PasswordAnswer", MySqlDbType.VarChar, 255);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = passwordAnswer;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static void UpdateTotalRevenue(Guid userGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET TotalRevenue = COALESCE((  ");
			sqlCommand.Append("SELECT SUM(SubTotal) FROM mp_CommerceReport WHERE UserGuid = ?UserGuid)  ");
			sqlCommand.Append(", 0) ");

			sqlCommand.Append("WHERE UserGuid = ?UserGuid  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static void UpdateTotalRevenue()
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET TotalRevenue = COALESCE((  ");
			sqlCommand.Append("SELECT SUM(SubTotal) FROM mp_CommerceReport WHERE UserGuid = mp_Users.UserGuid)  ");
			sqlCommand.Append(", 0) ");

			sqlCommand.Append("  ;");

			MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				null);
		}



		public static bool FlagAsDeleted(int userId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET IsDeleted = 1 ");
			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static bool FlagAsNotDeleted(int userId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET IsDeleted = 0 ");
			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static bool IncrementTotalPosts(int userId)
		{

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET	TotalPosts = TotalPosts + 1 ");
			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);


			return (rowsAffected > 0);
		}

		public static bool DecrementTotalPosts(int userId)
		{

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_Users ");
			sqlCommand.Append("SET	TotalPosts = TotalPosts - 1 ");
			sqlCommand.Append("WHERE UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userId;

			int rowsAffected = 0;

			rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

			return (rowsAffected > 0);
		}

		public static IDataReader GetRolesByUser(int siteId, int userId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT ");
			sqlCommand.Append("mp_Roles.RoleID, ");
			sqlCommand.Append("mp_Roles.DisplayName, ");
			sqlCommand.Append("mp_Roles.RoleName ");

			sqlCommand.Append("FROM	 mp_UserRoles ");

			sqlCommand.Append("INNER JOIN mp_Users ");
			sqlCommand.Append("ON mp_UserRoles.UserID = mp_Users.UserID ");

			sqlCommand.Append("INNER JOIN mp_Roles ");
			sqlCommand.Append("ON  mp_UserRoles.RoleID = mp_Roles.RoleID ");

			sqlCommand.Append("WHERE mp_Users.SiteID = ?SiteID AND mp_Users.UserID = ?UserID  ;");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?UserID", MySqlDbType.Int32);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = userId;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static IDataReader GetUserByRegistrationGuid(int siteId, Guid registerConfirmGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * ");

			sqlCommand.Append("FROM	mp_Users ");

			sqlCommand.Append("WHERE SiteID = ?SiteID AND RegisterConfirmGuid = ?RegisterConfirmGuid  ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?RegisterConfirmGuid", MySqlDbType.VarChar, 36);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = registerConfirmGuid;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}


		public static IDataReader GetSingleUser(int siteId, string email)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * ");

			sqlCommand.Append("FROM	mp_Users ");

			sqlCommand.Append("WHERE SiteID = ?SiteID AND LoweredEmail = ?Email  ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?Email", MySqlDbType.VarChar, 100);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = email.ToLower();

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static IDataReader GetSingleUserByLoginName(int siteId, string loginName, bool allowEmailFallback)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * ");

			sqlCommand.Append("FROM	mp_Users ");

			sqlCommand.Append("WHERE SiteID = ?SiteID  ");

			if (allowEmailFallback)
			{
				sqlCommand.Append("AND ");
				sqlCommand.Append("(");
				sqlCommand.Append("LoginName = ?LoginName ");
				sqlCommand.Append("OR Email = ?LoginName ");
				sqlCommand.Append(")");
			}
			else
			{
				sqlCommand.Append("AND LoginName = ?LoginName ");
			}

			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?LoginName", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = loginName;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}


		public static IDataReader GetSingleUser(int userId, int siteId)
		{
			const string sqlCommand = @"
				SELECT
					*
				FROM
					mp_Users
				WHERE
					UserID = ?UserID
				AND
					SiteID = ?SiteID;";

			var arParams = new List<MySqlParameter>
			{
				new MySqlParameter("?UserID", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = userId
				},
				new MySqlParameter("?SiteID", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = siteId
				}
			};

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand,
				arParams.ToArray()
			);
		}


		public static IDataReader GetSingleUser(Guid userGuid, int siteId)
		{
			const string sqlCommand = @"
				SELECT
					*
				FROM
					mp_Users
				WHERE
					UserGuid = ?UserGuid
				AND
					SiteID = ?SiteID;";

			var arParams = new List<MySqlParameter>
			{
				new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36)
				{
					Direction = ParameterDirection.Input,
					Value = userGuid.ToString()
				},
				new MySqlParameter("?SiteID", MySqlDbType.Int32)
				{
					Direction = ParameterDirection.Input,
					Value = siteId
				}
			};

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand,
				arParams.ToArray()
			);
		}


		public static Guid GetUserGuidFromOpenId(
			int siteId,
			string openIdUri)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT UserGuid ");
			sqlCommand.Append("FROM	mp_Users ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID  ");
			sqlCommand.Append("AND OpenIDURI = ?OpenIDURI   ");
			sqlCommand.Append(" ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?OpenIDURI", MySqlDbType.VarChar, 255);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = openIdUri;

			Guid userGuid = Guid.Empty;

			using (IDataReader reader = MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams))
			{

				if (reader.Read())
				{
					userGuid = new Guid(reader["UserGuid"].ToString());
				}
			}

			return userGuid;

		}

		public static Guid GetUserGuidFromWindowsLiveId(
			int siteId,
			string windowsLiveId)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT UserGuid ");
			sqlCommand.Append("FROM	mp_Users ");
			sqlCommand.Append("WHERE   ");
			sqlCommand.Append("SiteID = ?SiteID  ");
			sqlCommand.Append("AND WindowsLiveID = ?WindowsLiveID   ");
			sqlCommand.Append(" ;  ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?WindowsLiveID", MySqlDbType.VarChar, 36);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = windowsLiveId;

			Guid userGuid = Guid.Empty;

			using (IDataReader reader = MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams))
			{

				if (reader.Read())
				{
					userGuid = new Guid(reader["UserGuid"].ToString());
				}
			}

			return userGuid;

		}

		public static string LoginByEmail(int siteId, string email, string password)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT Name ");

			sqlCommand.Append("FROM  mp_Users ");

			sqlCommand.Append("WHERE Email = ?Email  ");
			sqlCommand.Append("AND SiteID = ?SiteID  ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND Pwd = ?Password ;  ");

			MySqlParameter[] arParams = new MySqlParameter[3];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?Email", MySqlDbType.VarChar, 100);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = email;

			arParams[2] = new MySqlParameter("?Password", MySqlDbType.Text);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = password;

			string userName = string.Empty;

			using (IDataReader reader = MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams))
			{
				if (reader.Read())
				{
					userName = reader["Name"].ToString();
				}

			}

			return userName;
		}

		public static string Login(int siteId, string loginName, string password)
		{

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT Name ");

			sqlCommand.Append("FROM  mp_Users ");

			sqlCommand.Append("WHERE LoginName = ?LoginName  ");
			sqlCommand.Append("AND SiteID = ?SiteID  ");
			sqlCommand.Append("AND IsDeleted = 0 ");
			sqlCommand.Append("AND Pwd = ?Password ;  ");

			MySqlParameter[] arParams = new MySqlParameter[3];

			arParams[0] = new MySqlParameter("?SiteID", MySqlDbType.Int32);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = siteId;

			arParams[1] = new MySqlParameter("?LoginName", MySqlDbType.VarChar, 50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = loginName;

			arParams[2] = new MySqlParameter("?Password", MySqlDbType.Text);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = password;

			string userName = string.Empty;

			using (IDataReader reader = MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams))
			{
				if (reader.Read())
				{
					userName = reader["Name"].ToString();
				}

			}
			return userName;
		}

		public static DataTable GetNonLazyLoadedPropertiesForUser(Guid userGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();

			sqlCommand.Append("SELECT  * ");
			sqlCommand.Append("FROM	mp_UserProperties ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("UserGuid = ?UserGuid ;");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("UserGuid", typeof(String));
			dataTable.Columns.Add("PropertyName", typeof(String));
			dataTable.Columns.Add("PropertyValueString", typeof(String));
			dataTable.Columns.Add("PropertyValueBinary", typeof(object));

			using (IDataReader reader = MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams))
			{
				while (reader.Read())
				{
					DataRow row = dataTable.NewRow();
					row["UserGuid"] = reader["UserGuid"].ToString();
					row["PropertyName"] = reader["PropertyName"].ToString();
					row["PropertyValueString"] = reader["PropertyValueString"].ToString();
					row["PropertyValueBinary"] = reader["PropertyValueBinary"];
					dataTable.Rows.Add(row);
				}

			}

			return dataTable;
		}

		public static IDataReader GetLazyLoadedProperty(Guid userGuid, String propertyName)
		{
			StringBuilder sqlCommand = new StringBuilder();

			sqlCommand.Append("SELECT  * ");
			sqlCommand.Append("FROM	mp_UserProperties ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("UserGuid = ?UserGuid AND PropertyName = ?PropertyName  ");
			sqlCommand.Append("LIMIT 1 ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?PropertyName", MySqlDbType.VarChar, 255);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = propertyName;

			return MySqlHelper.ExecuteReader(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static bool PropertyExists(Guid userGuid, string propertyName)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT Count(*) ");
			sqlCommand.Append("FROM	mp_UserProperties ");
			sqlCommand.Append("WHERE UserGuid = ?UserGuid AND PropertyName = ?PropertyName ; ");

			MySqlParameter[] arParams = new MySqlParameter[2];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?PropertyName", MySqlDbType.VarChar, 255);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = propertyName;

			int count = Convert.ToInt32(MySqlHelper.ExecuteScalar(
				ConnectionString.GetReadConnectionString(),
				sqlCommand.ToString(),
				arParams));

			return (count > 0);

		}

		public static void CreateProperty(
			Guid propertyId,
			Guid userGuid,
			String propertyName,
			String propertyValues,
			byte[] propertyValueb,
			DateTime lastUpdatedDate,
			bool isLazyLoaded)
		{
			StringBuilder sqlCommand = new StringBuilder();

			sqlCommand.Append("INSERT INTO mp_UserProperties (");
			sqlCommand.Append("PropertyID, ");
			sqlCommand.Append("UserGuid, ");
			sqlCommand.Append("PropertyName, ");
			sqlCommand.Append("PropertyValueString, ");
			sqlCommand.Append("PropertyValueBinary, ");
			sqlCommand.Append("LastUpdatedDate, ");
			sqlCommand.Append("IsLazyLoaded )");

			sqlCommand.Append(" VALUES (");
			sqlCommand.Append("?PropertyID, ");
			sqlCommand.Append("?UserGuid, ");
			sqlCommand.Append("?PropertyName, ");
			sqlCommand.Append("?PropertyValueString, ");
			sqlCommand.Append("?PropertyValueBinary, ");
			sqlCommand.Append("?LastUpdatedDate, ");
			sqlCommand.Append("?IsLazyLoaded );");


			MySqlParameter[] arParams = new MySqlParameter[7];

			arParams[0] = new MySqlParameter("?PropertyID", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = propertyId.ToString();

			arParams[1] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = userGuid.ToString();

			arParams[2] = new MySqlParameter("?PropertyName", MySqlDbType.VarChar, 255);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = propertyName;

			arParams[3] = new MySqlParameter("?PropertyValueString", MySqlDbType.Text);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = propertyValues;

			arParams[4] = new MySqlParameter("?PropertyValueBinary", MySqlDbType.LongBlob);
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = propertyValueb;

			arParams[5] = new MySqlParameter("?LastUpdatedDate", MySqlDbType.DateTime);
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = lastUpdatedDate;

			arParams[6] = new MySqlParameter("?IsLazyLoaded", MySqlDbType.Bit);
			arParams[6].Direction = ParameterDirection.Input;
			arParams[6].Value = isLazyLoaded;

			MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);

		}

		public static void UpdateProperty(
			Guid userGuid,
			String propertyName,
			String propertyValues,
			byte[] propertyValueb,
			DateTime lastUpdatedDate,
			bool isLazyLoaded)
		{
			StringBuilder sqlCommand = new StringBuilder();

			sqlCommand.Append("UPDATE mp_UserProperties ");
			sqlCommand.Append("SET  ");
			//sqlCommand.Append("UserGuid = ?UserGuid, ");
			//sqlCommand.Append("PropertyName = ?PropertyName, ");
			sqlCommand.Append("PropertyValueString = ?PropertyValueString, ");
			sqlCommand.Append("PropertyValueBinary = ?PropertyValueBinary, ");
			sqlCommand.Append("LastUpdatedDate = ?LastUpdatedDate, ");
			sqlCommand.Append("IsLazyLoaded = ?IsLazyLoaded ");

			sqlCommand.Append("WHERE  ");
			sqlCommand.Append("UserGuid = ?UserGuid AND PropertyName = ?PropertyName ;");

			MySqlParameter[] arParams = new MySqlParameter[6];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			arParams[1] = new MySqlParameter("?PropertyName", MySqlDbType.VarChar, 255);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = propertyName;

			arParams[2] = new MySqlParameter("?PropertyValueString", MySqlDbType.Text);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = propertyValues;

			arParams[3] = new MySqlParameter("?PropertyValueBinary", MySqlDbType.LongBlob);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = propertyValueb;

			arParams[4] = new MySqlParameter("?LastUpdatedDate", MySqlDbType.DateTime);
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = lastUpdatedDate;

			arParams[5] = new MySqlParameter("?IsLazyLoaded", MySqlDbType.Bit);
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = isLazyLoaded;

			MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);


		}

		public static bool DeletePropertiesByUser(Guid userGuid)
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("DELETE FROM mp_UserProperties ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("UserGuid = ?UserGuid ");
			sqlCommand.Append(";");

			MySqlParameter[] arParams = new MySqlParameter[1];

			arParams[0] = new MySqlParameter("?UserGuid", MySqlDbType.VarChar, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = userGuid.ToString();

			int rowsAffected = MySqlHelper.ExecuteNonQuery(
				ConnectionString.GetWriteConnectionString(),
				sqlCommand.ToString(),
				arParams);
			return (rowsAffected > 0);

		}



	}
}
