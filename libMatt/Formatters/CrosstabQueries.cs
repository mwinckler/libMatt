using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace libMatt.Formatters {

	/// <summary>
	/// Extension methods to make Access-style "crosstab" queries possible.
	/// </summary>
	public static class CrosstabQueries {

		#region DataTable Manipulation

		public static DataTable CrossTab(this DataRow[] source, string rowHeaderField, string colHeaderField, string dataField) {
			// Transform data into crosstab using LINQ
			// rowHeaderField (a.k.a. "yDataField") down the left
			// colHeaderField (a.k.a. "xDataField") across the top
			// dataField (a.k.a. "zExpr") in the cells

			var crossTab = (from yRow in source
							group yRow by yRow[rowHeaderField].ToString() into g
							select new {
								YHeader = g.Key,
								Datapoints = (from xRow in g
											  select new {
												  XHeader = xRow[colHeaderField].ToString(),
												  YHeader = xRow[rowHeaderField].ToString(),
												  Data = xRow[dataField].ToString()
											  }).ToDictionary(
												itm => itm.XHeader,
												itm => itm
												)
							});


			DataTable crossTabResults = new DataTable();
			crossTabResults.Columns.Add(rowHeaderField);
			foreach (string colName in (from item in crossTab
										select (from xItem in item.Datapoints
												select xItem.Key).ToList()
										).Aggregate((colNames, names) => { colNames.AddRange(names); return colNames; })
										.Distinct()) {
				crossTabResults.Columns.Add(colName);
			}

			DataRow newRow;
			foreach (var row in crossTab) {
				newRow = crossTabResults.NewRow();
				newRow[rowHeaderField] = row.YHeader;
				foreach (var xDatapoint in row.Datapoints) {
					newRow[xDatapoint.Key] = xDatapoint.Value.Data;
				}
				crossTabResults.Rows.Add(newRow);
			}



			return crossTabResults;
		}

		public static DataTable CrossTab(this DataTable source, string rowHeaderField, string colHeaderField, string dataField) {
			return CrossTab(source.Select(), rowHeaderField, colHeaderField, dataField);
		}

		#endregion


	}
}
