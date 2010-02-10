using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace libMatt.Data {

	public class FieldDefinition {
		private string _field_name;
		private object _value;
		private bool _is_key;

		public string FieldName {
			get { return _field_name; }
			set { _field_name = value; }
		}
		public object Value {
			get { return _value; }
			set { _value = value; }
		}
		public bool IsKey {
			get { return _is_key; }
			set { _is_key = value; }
		}

		internal FieldDefinition(string fieldName, object value, bool isKey) {
			FieldName = fieldName;
			Value = value;
			IsKey = isKey;
		}
	}

	public class FieldCollection {
		private List<T> ToList<T>(ICollection<T> coll) {
			List<T> ret = new List<T>();
			foreach (T val in coll) {
				ret.Add(val);
			}
			return ret;
		}

		private Dictionary<string, FieldDefinition> _fields = new Dictionary<string, FieldDefinition>();

		public List<FieldDefinition> Fields {
			get { return ToList(_fields.Values); }
		}

		public List<FieldDefinition> Keys {
			get {
				List<FieldDefinition> ret = new List<FieldDefinition>();
				foreach (FieldDefinition field in _fields.Values) {
					if (field.IsKey)
						ret.Add(field);
				}
				return ret;
			}
		}

		public void AddParameter(string fieldName, object value, bool isKey) {
			_fields.Add(fieldName, new FieldDefinition(fieldName, value, isKey));
		}

		public void AddParameter(string fieldName, object value) {
			AddParameter(fieldName, value, false);
		}

		public void AddParameter(string fieldName) {
			AddParameter(fieldName, null, false);
		}

		public FieldDefinition this[string name] {
			get { return _fields[name]; }
			set { _fields[name] = value; }
		}

		public void Clear() {
			_fields.Clear();
		}
	}

	internal class CommandPackage {
		private string _ctsave;
		private Dictionary<string, object> _parms;
		public string CommandTextSave {
			get { return _ctsave; }
			set { _ctsave = value; }
		}
		public Dictionary<string, object> Parameters {
			get { return _parms; }
			set { _parms = value; }
		}

	}

}