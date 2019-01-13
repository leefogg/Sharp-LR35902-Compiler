﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class PrimitiveDataTypeTests
	{
		[TestMethod]
		public void MaxValue()
		{
			var datatype = new PrimitiveDataType("byte", 8);

			Assert.AreEqual(255, datatype.MaxValue);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Constructor_NumBitsNotPowerOfTwoThrows()
		{
			new PrimitiveDataType("byte", 7);
		}
	}
}
