using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using System;
using System.Collections.Generic;
using System.Text;

using static Sharp_LR35902_Compiler.BuiltIn.DataTypes;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class BuiltInDataTypesTests
	{
		[TestMethod]
		public void Exists_Finds()
		{
			Assert.IsTrue(Exists("byte"));
		}

		[TestMethod]
		public void Exists_DoesntFind()
		{
			Assert.IsFalse(Exists("madeupdatatype"));
		}

		[TestMethod]
		public void Get_Finds()
		{
			Assert.IsNotNull(Get("byte"));
		}

		[TestMethod]
		public void Get_DoesntFind()
		{
			Assert.IsNull(Get("madeupdatatype"));
		}

		[TestMethod]
		public void CanConvertTo()
		{
			Assert.IsTrue(BuiltIn.DataTypes.CanConvertTo(BuiltIn.DataTypes.Byte, BuiltIn.DataTypes.Int));
			Assert.IsTrue(BuiltIn.DataTypes.CanConvertTo(BuiltIn.DataTypes.Byte, BuiltIn.DataTypes.Byte));
			Assert.IsTrue(BuiltIn.DataTypes.CanConvertTo(BuiltIn.DataTypes.Int, BuiltIn.DataTypes.Int));
			Assert.IsFalse(BuiltIn.DataTypes.CanConvertTo(BuiltIn.DataTypes.Int, BuiltIn.DataTypes.Byte));
		}
	}
}
