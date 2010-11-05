using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using libMatt.Formatters;
using System.Reflection;

namespace libMattTests {
	[TestFixture]
	class AttributeTest {

		[Test]
		public void FindAttributeOnEnum() {
			Assert.IsNotNull(MyEnum.value1.GetAttribute<MyAttribute>());
		}

		[Test]
		public void FindAttributeOnObjectField() {
			Assert.IsNotNull(new MyObject().aField.GetAttribute<MyAttribute>());
		}

		[Test]
		public void FindAttributeOnClass() {
			Assert.IsNotNull(new MyObject().GetAttribute<MyOtherAttribute>());
		}

		[Test]
		public void FindAttributeOnType() {
			Assert.IsNotNull(typeof(MyObject).GetAttribute<MyOtherAttribute>());
		}

		[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
		private sealed class MyAttribute : Attribute {

			public MyAttribute(string str, int i) {
				this.str = str;
				this.i = i;
			}

			public string str { get; private set; }
			public int i { get; private set; }
		}

		[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
		private sealed class MyOtherAttribute : Attribute {

			public MyOtherAttribute(string str) {
				this.x = str;
			}

			public string x { get; private set; }
		}

		private enum MyEnum {
			[My("value 1", 1)]
			value1,
			value2
		}

		[MyOther("class attr")]
		private class MyObject {

			public MyObject() {
				aField = "xyzzy";
			}

			[My("something", 1)]
			public string aField;

		}

	}


}
