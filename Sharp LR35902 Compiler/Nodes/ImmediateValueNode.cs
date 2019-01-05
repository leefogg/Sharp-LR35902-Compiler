﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class ImmediateValueNode : ValueNode
    {
		public byte Value { get; }

		public ImmediateValueNode(byte value)
		{
			Value = value;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            return NoRegisters;
        }

		public override bool Equals(object obj)
		{
			if (obj is ImmediateValueNode)
			{
				var other = obj as ImmediateValueNode;
				return other.Value == Value;
			}

			return false;
		}
	}
}
