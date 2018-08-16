using Common.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Sharp_LR35902_Assembler.Assembler;
using static Test_Common.Utils;
namespace Sharp_LR35902_Assembler_Tests
{
	[TestClass]
	public class Instructions
	{
		[TestMethod]
		public void NOP()
		{
			var result = CompileInstruction("NOP");
			Is(result, 0x00);
		}


		[TestMethod]
		public void LD_BC_nn()
		{
			var result = CompileInstruction("LD BC,62689");
			Is(result, 0x01, 225, 244);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_WrongNumberOfOprands()
		{
			CompileInstruction("LD"); // No oprands
		}


		[TestMethod]
		public void LD_BC_A()
		{
			var result = CompileInstruction("LD (BC),A");
			Is(result, 0x02);
		}


		[TestMethod]
		public void INC_BC()
		{
			var result = CompileInstruction("INC BC");
			Is(result, 0x03);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void INC_WrongNumberOfOprands()
		{
			CompileInstruction("INC"); // No oprands
		}


		[TestMethod]
		public void INC_B()
		{
			var result = CompileInstruction("INC B");
			Is(result, 0x04);
		}


		[TestMethod]
		public void DEC_B()
		{
			var result = CompileInstruction("DEC B");
			Is(result, 0x05);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void DEC_WrongNumberOfOprands()
		{
			CompileInstruction("DEC"); // No oprands
		}


		[TestMethod]
		public void LD_B_n()
		{
			var result = CompileInstruction("LD B,225");
			Is(result, 0x06, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_B_n_ImmediateTooBig()
		{
			CompileInstruction("LD B,62689");
		}


		[TestMethod]
		public void RLC_A()
		{
			var result = CompileInstruction("RLC A");
			Is(result, 0x07);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RLC_WrongNumberOfOprands()
		{
			CompileInstruction("RLC"); // No oprands
		}


		[TestMethod]
		public void LD_nn_SP()
		{
			var result = CompileInstruction("LD (62689),SP");
			Is(result, 0x08, 225, 244);
		}


		[TestMethod]
		public void ADD_HL_BC()
		{
			var result = CompileInstruction("ADD HL,BC");
			Is(result, 0x09);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void ADD_WrongNumberOfOprands()
		{
			CompileInstruction("ADD"); // No oprands
		}


		[TestMethod]
		public void LD_A_BC()
		{
			var result = CompileInstruction("LD A,(BC)");
			Is(result, 0x0A);
		}


		[TestMethod]
		public void DEC_BC()
		{
			var result = CompileInstruction("DEC BC");
			Is(result, 0x0B);
		}


		[TestMethod]
		public void INC_C()
		{
			var result = CompileInstruction("INC C");
			Is(result, 0x0C);
		}


		[TestMethod]
		public void DEC_C()
		{
			var result = CompileInstruction("DEC C");
			Is(result, 0x0D);
		}


		[TestMethod]
		public void LD_C_n()
		{
			var result = CompileInstruction("LD C,225");
			Is(result, 0x0E, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_C_n_ImmediateTooBig()
		{
			CompileInstruction("LD C,62689");
		}


		[TestMethod]
		public void RRC_A()
		{
			var result = CompileInstruction("RRC A");
			Is(result, 0x0F);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RRC_WrongNumberOfOprands()
		{
			CompileInstruction("RRC"); // No oprands
		}


		[TestMethod]
		public void STOP()
		{
			var result = CompileInstruction("STOP");
			Is(result, 0x10);
		}


		[TestMethod]
		public void LD_DE_nn()
		{
			var result = CompileInstruction("LD DE,62689");
			Is(result, 0x11, 225, 244);
		}


		[TestMethod]
		public void LD_DE_A()
		{
			var result = CompileInstruction("LD (DE),A");
			Is(result, 0x12);
		}


		[TestMethod]
		public void INC_DE()
		{
			var result = CompileInstruction("INC DE");
			Is(result, 0x13);
		}


		[TestMethod]
		public void INC_D()
		{
			var result = CompileInstruction("INC D");
			Is(result, 0x14);
		}


		[TestMethod]
		public void DEC_D()
		{
			var result = CompileInstruction("DEC D");
			Is(result, 0x15);
		}


		[TestMethod]
		public void LD_D_n()
		{
			var result = CompileInstruction("LD D,225");
			Is(result, 0x16, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_D_n_ImmediateTooBig()
		{
			CompileInstruction("LD D,62689");
		}


		[TestMethod]
		public void RL_A()
		{
			var result = CompileInstruction("RL A");
			Is(result, 0x17);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RL_WrongNumberOfOprands()
		{
			CompileInstruction("RL"); // No oprands
		}


		[TestMethod]
		public void JR_n()
		{
			var result = CompileInstruction("JR 225");
			Is(result, 0x18, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_WrongNumberOfOprands()
		{
			CompileInstruction("JR"); // No oprands
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_n_ImmediateTooBig()
		{
			CompileInstruction("JR 62689");
		}


		[TestMethod]
		public void ADD_HL_DE()
		{
			var result = CompileInstruction("ADD HL,DE");
			Is(result, 0x19);
		}


		[TestMethod]
		public void LD_A_DE()
		{
			var result = CompileInstruction("LD A,(DE)");
			Is(result, 0x1A);
		}


		[TestMethod]
		public void DEC_DE()
		{
			var result = CompileInstruction("DEC DE");
			Is(result, 0x1B);
		}


		[TestMethod]
		public void INC_E()
		{
			var result = CompileInstruction("INC E");
			Is(result, 0x1C);
		}


		[TestMethod]
		public void DEC_E()
		{
			var result = CompileInstruction("DEC E");
			Is(result, 0x1D);
		}


		[TestMethod]
		public void LD_E_n()
		{
			var result = CompileInstruction("LD E,225");
			Is(result, 0x1E, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_E_n_ImmediateTooBig()
		{
			CompileInstruction("LD E,62689");
		}


		[TestMethod]
		public void RR_A()
		{
			var result = CompileInstruction("RR A");
			Is(result, 0x1F);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RR_WrongNumberOfOprands()
		{
			CompileInstruction("RR"); // No oprands
		}


		[TestMethod]
		public void JR_NZ_n()
		{
			var result = CompileInstruction("JR NZ,225");
			Is(result, 0x20, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_NZ_n_ImmediateTooBig()
		{
			CompileInstruction("JR NZ,62689");
		}


		[TestMethod]
		public void LD_HL_nn()
		{
			var result = CompileInstruction("LD HL,62689");
			Is(result, 0x21, 225, 244);
		}


		[TestMethod]
		public void LDI_HL_A()
		{
			var result = CompileInstruction("LDI (HL),A");
			Is(result, 0x22);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDI_WrongNumberOfOprands()
		{
			CompileInstruction("LDI"); // No oprands
		}


		[TestMethod]
		public void INC_HL()
		{
			var result = CompileInstruction("INC HL");
			Is(result, 0x23);
		}


		[TestMethod]
		public void INC_H()
		{
			var result = CompileInstruction("INC H");
			Is(result, 0x24);
		}


		[TestMethod]
		public void DEC_H()
		{
			var result = CompileInstruction("DEC H");
			Is(result, 0x25);
		}


		[TestMethod]
		public void LD_H_n()
		{
			var result = CompileInstruction("LD H,225");
			Is(result, 0x26, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_H_n_ImmediateTooBig()
		{
			CompileInstruction("LD H,62689");
		}


		[TestMethod]
		public void DAA()
		{
			var result = CompileInstruction("DAA");
			Is(result, 0x27);
		}


		[TestMethod]
		public void JR_Z_n()
		{
			var result = CompileInstruction("JR Z,225");
			Is(result, 0x28, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_Z_n_ImmediateTooBig()
		{
			CompileInstruction("JR Z,62689");
		}


		[TestMethod]
		public void ADD_HL_HL()
		{
			var result = CompileInstruction("ADD HL,HL");
			Is(result, 0x29);
		}


		[TestMethod]
		public void LDI_A_HL()
		{
			var result = CompileInstruction("LDI A,(HL)");
			Is(result, 0x2A);
		}


		[TestMethod]
		public void DEC_HL()
		{
			var result = CompileInstruction("DEC HL");
			Is(result, 0x2B);
		}


		[TestMethod]
		public void INC_L()
		{
			var result = CompileInstruction("INC L");
			Is(result, 0x2C);
		}


		[TestMethod]
		public void DEC_L()
		{
			var result = CompileInstruction("DEC L");
			Is(result, 0x2D);
		}


		[TestMethod]
		public void LD_L_n()
		{
			var result = CompileInstruction("LD L,225");
			Is(result, 0x2E, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_L_n_ImmediateTooBig()
		{
			CompileInstruction("LD L,62689");
		}


		[TestMethod]
		public void CPL()
		{
			var result = CompileInstruction("CPL");
			Is(result, 0x2F);
		}


		[TestMethod]
		public void JR_NC_n()
		{
			var result = CompileInstruction("JR NC,225");
			Is(result, 0x30, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_NC_n_ImmediateTooBig()
		{
			CompileInstruction("JR NC,62689");
		}


		[TestMethod]
		public void LD_SP_nn()
		{
			var result = CompileInstruction("LD SP,62689");
			Is(result, 0x31, 225, 244);
		}


		[TestMethod]
		public void LDD_HL_A()
		{
			var result = CompileInstruction("LDD (HL),A");
			Is(result, 0x32);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDD_WrongNumberOfOprands()
		{
			CompileInstruction("LDD"); // No oprands
		}


		[TestMethod]
		public void INC_SP()
		{
			var result = CompileInstruction("INC SP");
			Is(result, 0x33);
		}


		[TestMethod]
		public void INC_At_HL()
		{
			var result = CompileInstruction("INC (HL)");
			Is(result, 0x34);
		}


		[TestMethod]
		public void DEC_At_HL()
		{
			var result = CompileInstruction("DEC (HL)");
			Is(result, 0x35);
		}


		[TestMethod]
		public void LD_HL_n()
		{
			var result = CompileInstruction("LD (HL),225");
			Is(result, 0x36, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_HL_n_ImmediateTooBig()
		{
			CompileInstruction("LD (HL),62689");
		}


		[TestMethod]
		public void SCF()
		{
			var result = CompileInstruction("SCF");
			Is(result, 0x37);
		}


		[TestMethod]
		public void JR_C_n()
		{
			var result = CompileInstruction("JR C,225");
			Is(result, 0x38, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JR_C_n_ImmediateTooBig()
		{
			CompileInstruction("JR C,62689");
		}


		[TestMethod]
		public void ADD_HL_SP()
		{
			var result = CompileInstruction("ADD HL,SP");
			Is(result, 0x39);
		}


		[TestMethod]
		public void LDD_A_HL()
		{
			var result = CompileInstruction("LDD A,(HL)");
			Is(result, 0x3A);
		}


		[TestMethod]
		public void DEC_SP()
		{
			var result = CompileInstruction("DEC SP");
			Is(result, 0x3B);
		}


		[TestMethod]
		public void INC_A()
		{
			var result = CompileInstruction("INC A");
			Is(result, 0x3C);
		}


		[TestMethod]
		public void DEC_A()
		{
			var result = CompileInstruction("DEC A");
			Is(result, 0x3D);
		}


		[TestMethod]
		public void LD_A_n()
		{
			var result = CompileInstruction("LD A,225");
			Is(result, 0x3E, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LD_A_n_ImmediateTooBig()
		{
			CompileInstruction("LD A,62689");
		}


		[TestMethod]
		public void CCF()
		{
			var result = CompileInstruction("CCF");
			Is(result, 0x3F);
		}


		[TestMethod]
		public void LD_B_B()
		{
			var result = CompileInstruction("LD B,B");
			Is(result, 0x40);
		}


		[TestMethod]
		public void LD_B_C()
		{
			var result = CompileInstruction("LD B,C");
			Is(result, 0x41);
		}


		[TestMethod]
		public void LD_B_D()
		{
			var result = CompileInstruction("LD B,D");
			Is(result, 0x42);
		}


		[TestMethod]
		public void LD_B_E()
		{
			var result = CompileInstruction("LD B,E");
			Is(result, 0x43);
		}


		[TestMethod]
		public void LD_B_H()
		{
			var result = CompileInstruction("LD B,H");
			Is(result, 0x44);
		}


		[TestMethod]
		public void LD_B_L()
		{
			var result = CompileInstruction("LD B,L");
			Is(result, 0x45);
		}


		[TestMethod]
		public void LD_B_HL()
		{
			var result = CompileInstruction("LD B,(HL)");
			Is(result, 0x46);
		}


		[TestMethod]
		public void LD_B_A()
		{
			var result = CompileInstruction("LD B,A");
			Is(result, 0x47);
		}


		[TestMethod]
		public void LD_C_B()
		{
			var result = CompileInstruction("LD C,B");
			Is(result, 0x48);
		}


		[TestMethod]
		public void LD_C_C()
		{
			var result = CompileInstruction("LD C,C");
			Is(result, 0x49);
		}


		[TestMethod]
		public void LD_C_D()
		{
			var result = CompileInstruction("LD C,D");
			Is(result, 0x4A);
		}


		[TestMethod]
		public void LD_C_E()
		{
			var result = CompileInstruction("LD C,E");
			Is(result, 0x4B);
		}


		[TestMethod]
		public void LD_C_H()
		{
			var result = CompileInstruction("LD C,H");
			Is(result, 0x4C);
		}


		[TestMethod]
		public void LD_C_L()
		{
			var result = CompileInstruction("LD C,L");
			Is(result, 0x4D);
		}


		[TestMethod]
		public void LD_C_HL()
		{
			var result = CompileInstruction("LD C,(HL)");
			Is(result, 0x4E);
		}


		[TestMethod]
		public void LD_C_A()
		{
			var result = CompileInstruction("LD C,A");
			Is(result, 0x4F);
		}


		[TestMethod]
		public void LD_D_B()
		{
			var result = CompileInstruction("LD D,B");
			Is(result, 0x50);
		}


		[TestMethod]
		public void LD_D_C()
		{
			var result = CompileInstruction("LD D,C");
			Is(result, 0x51);
		}


		[TestMethod]
		public void LD_D_D()
		{
			var result = CompileInstruction("LD D,D");
			Is(result, 0x52);
		}


		[TestMethod]
		public void LD_D_E()
		{
			var result = CompileInstruction("LD D,E");
			Is(result, 0x53);
		}


		[TestMethod]
		public void LD_D_H()
		{
			var result = CompileInstruction("LD D,H");
			Is(result, 0x54);
		}


		[TestMethod]
		public void LD_D_L()
		{
			var result = CompileInstruction("LD D,L");
			Is(result, 0x55);
		}


		[TestMethod]
		public void LD_D_HL()
		{
			var result = CompileInstruction("LD D,(HL)");
			Is(result, 0x56);
		}


		[TestMethod]
		public void LD_D_A()
		{
			var result = CompileInstruction("LD D,A");
			Is(result, 0x57);
		}


		[TestMethod]
		public void LD_E_B()
		{
			var result = CompileInstruction("LD E,B");
			Is(result, 0x58);
		}


		[TestMethod]
		public void LD_E_C()
		{
			var result = CompileInstruction("LD E,C");
			Is(result, 0x59);
		}


		[TestMethod]
		public void LD_E_D()
		{
			var result = CompileInstruction("LD E,D");
			Is(result, 0x5A);
		}


		[TestMethod]
		public void LD_E_E()
		{
			var result = CompileInstruction("LD E,E");
			Is(result, 0x5B);
		}


		[TestMethod]
		public void LD_E_H()
		{
			var result = CompileInstruction("LD E,H");
			Is(result, 0x5C);
		}


		[TestMethod]
		public void LD_E_L()
		{
			var result = CompileInstruction("LD E,L");
			Is(result, 0x5D);
		}


		[TestMethod]
		public void LD_E_HL()
		{
			var result = CompileInstruction("LD E,(HL)");
			Is(result, 0x5E);
		}


		[TestMethod]
		public void LD_E_A()
		{
			var result = CompileInstruction("LD E,A");
			Is(result, 0x5F);
		}


		[TestMethod]
		public void LD_H_B()
		{
			var result = CompileInstruction("LD H,B");
			Is(result, 0x60);
		}


		[TestMethod]
		public void LD_H_C()
		{
			var result = CompileInstruction("LD H,C");
			Is(result, 0x61);
		}


		[TestMethod]
		public void LD_H_D()
		{
			var result = CompileInstruction("LD H,D");
			Is(result, 0x62);
		}


		[TestMethod]
		public void LD_H_E()
		{
			var result = CompileInstruction("LD H,E");
			Is(result, 0x63);
		}


		[TestMethod]
		public void LD_H_H()
		{
			var result = CompileInstruction("LD H,H");
			Is(result, 0x64);
		}


		[TestMethod]
		public void LD_H_L()
		{
			var result = CompileInstruction("LD H,L");
			Is(result, 0x65);
		}


		[TestMethod]
		public void LD_H_HL()
		{
			var result = CompileInstruction("LD H,(HL)");
			Is(result, 0x66);
		}


		[TestMethod]
		public void LD_H_A()
		{
			var result = CompileInstruction("LD H,A");
			Is(result, 0x67);
		}


		[TestMethod]
		public void LD_L_B()
		{
			var result = CompileInstruction("LD L,B");
			Is(result, 0x68);
		}


		[TestMethod]
		public void LD_L_C()
		{
			var result = CompileInstruction("LD L,C");
			Is(result, 0x69);
		}


		[TestMethod]
		public void LD_L_D()
		{
			var result = CompileInstruction("LD L,D");
			Is(result, 0x6A);
		}


		[TestMethod]
		public void LD_L_E()
		{
			var result = CompileInstruction("LD L,E");
			Is(result, 0x6B);
		}


		[TestMethod]
		public void LD_L_H()
		{
			var result = CompileInstruction("LD L,H");
			Is(result, 0x6C);
		}


		[TestMethod]
		public void LD_L_L()
		{
			var result = CompileInstruction("LD L,L");
			Is(result, 0x6D);
		}


		[TestMethod]
		public void LD_L_HL()
		{
			var result = CompileInstruction("LD L,(HL)");
			Is(result, 0x6E);
		}


		[TestMethod]
		public void LD_L_A()
		{
			var result = CompileInstruction("LD L,A");
			Is(result, 0x6F);
		}


		[TestMethod]
		public void LD_HL_B()
		{
			var result = CompileInstruction("LD (HL),B");
			Is(result, 0x70);
		}


		[TestMethod]
		public void LD_HL_C()
		{
			var result = CompileInstruction("LD (HL),C");
			Is(result, 0x71);
		}


		[TestMethod]
		public void LD_HL_D()
		{
			var result = CompileInstruction("LD (HL),D");
			Is(result, 0x72);
		}


		[TestMethod]
		public void LD_HL_E()
		{
			var result = CompileInstruction("LD (HL),E");
			Is(result, 0x73);
		}


		[TestMethod]
		public void LD_HL_H()
		{
			var result = CompileInstruction("LD (HL),H");
			Is(result, 0x74);
		}


		[TestMethod]
		public void LD_HL_L()
		{
			var result = CompileInstruction("LD (HL),L");
			Is(result, 0x75);
		}


		[TestMethod]
		public void HALT()
		{
			var result = CompileInstruction("HALT");
			Is(result, 0x76);
		}


		[TestMethod]
		public void LD_HL_A()
		{
			var result = CompileInstruction("LD (HL),A");
			Is(result, 0x77);
		}


		[TestMethod]
		public void LD_A_B()
		{
			var result = CompileInstruction("LD A,B");
			Is(result, 0x78);
		}


		[TestMethod]
		public void LD_A_C()
		{
			var result = CompileInstruction("LD A,C");
			Is(result, 0x79);
		}


		[TestMethod]
		public void LD_A_D()
		{
			var result = CompileInstruction("LD A,D");
			Is(result, 0x7A);
		}


		[TestMethod]
		public void LD_A_E()
		{
			var result = CompileInstruction("LD A,E");
			Is(result, 0x7B);
		}


		[TestMethod]
		public void LD_A_H()
		{
			var result = CompileInstruction("LD A,H");
			Is(result, 0x7C);
		}


		[TestMethod]
		public void LD_A_L()
		{
			var result = CompileInstruction("LD A,L");
			Is(result, 0x7D);
		}


		[TestMethod]
		public void LD_A_HL()
		{
			var result = CompileInstruction("LD A,(HL)");
			Is(result, 0x7E);
		}


		[TestMethod]
		public void LD_A_A()
		{
			var result = CompileInstruction("LD A,A");
			Is(result, 0x7F);
		}


		[TestMethod]
		public void ADD_A_B()
		{
			var result = CompileInstruction("ADD A,B");
			Is(result, 0x80);
		}


		[TestMethod]
		public void ADD_A_C()
		{
			var result = CompileInstruction("ADD A,C");
			Is(result, 0x81);
		}


		[TestMethod]
		public void ADD_A_D()
		{
			var result = CompileInstruction("ADD A,D");
			Is(result, 0x82);
		}


		[TestMethod]
		public void ADD_A_E()
		{
			var result = CompileInstruction("ADD A,E");
			Is(result, 0x83);
		}


		[TestMethod]
		public void ADD_A_H()
		{
			var result = CompileInstruction("ADD A,H");
			Is(result, 0x84);
		}


		[TestMethod]
		public void ADD_A_L()
		{
			var result = CompileInstruction("ADD A,L");
			Is(result, 0x85);
		}


		[TestMethod]
		public void ADD_A_HL()
		{
			var result = CompileInstruction("ADD A,(HL)");
			Is(result, 0x86);
		}


		[TestMethod]
		public void ADD_A_A()
		{
			var result = CompileInstruction("ADD A,A");
			Is(result, 0x87);
		}


		[TestMethod]
		public void ADC_A_B()
		{
			var result = CompileInstruction("ADC A,B");
			Is(result, 0x88);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void ADC_WrongNumberOfOprands()
		{
			CompileInstruction("ADC"); // No oprands
		}


		[TestMethod]
		public void ADC_A_C()
		{
			var result = CompileInstruction("ADC A,C");
			Is(result, 0x89);
		}


		[TestMethod]
		public void ADC_A_D()
		{
			var result = CompileInstruction("ADC A,D");
			Is(result, 0x8A);
		}


		[TestMethod]
		public void ADC_A_E()
		{
			var result = CompileInstruction("ADC A,E");
			Is(result, 0x8B);
		}


		[TestMethod]
		public void ADC_A_H()
		{
			var result = CompileInstruction("ADC A,H");
			Is(result, 0x8C);
		}


		[TestMethod]
		public void ADC_A_L()
		{
			var result = CompileInstruction("ADC A,L");
			Is(result, 0x8D);
		}


		[TestMethod]
		public void ADC_A_HL()
		{
			var result = CompileInstruction("ADC A,(HL)");
			Is(result, 0x8E);
		}


		[TestMethod]
		public void ADC_A_A()
		{
			var result = CompileInstruction("ADC A,A");
			Is(result, 0x8F);
		}


		[TestMethod]
		public void SUB_A_B()
		{
			var result = CompileInstruction("SUB A,B");
			Is(result, 0x90);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SUB_WrongNumberOfOprands()
		{
			CompileInstruction("SUB"); // No oprands
		}


		[TestMethod]
		public void SUB_A_C()
		{
			var result = CompileInstruction("SUB A,C");
			Is(result, 0x91);
		}


		[TestMethod]
		public void SUB_A_D()
		{
			var result = CompileInstruction("SUB A,D");
			Is(result, 0x92);
		}


		[TestMethod]
		public void SUB_A_E()
		{
			var result = CompileInstruction("SUB A,E");
			Is(result, 0x93);
		}


		[TestMethod]
		public void SUB_A_H()
		{
			var result = CompileInstruction("SUB A,H");
			Is(result, 0x94);
		}


		[TestMethod]
		public void SUB_A_L()
		{
			var result = CompileInstruction("SUB A,L");
			Is(result, 0x95);
		}


		[TestMethod]
		public void SUB_A_HL()
		{
			var result = CompileInstruction("SUB A,(HL)");
			Is(result, 0x96);
		}


		[TestMethod]
		public void SUB_A_A()
		{
			var result = CompileInstruction("SUB A,A");
			Is(result, 0x97);
		}


		[TestMethod]
		public void SBC_A_B()
		{
			var result = CompileInstruction("SBC A,B");
			Is(result, 0x98);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SBC_WrongNumberOfOprands()
		{
			CompileInstruction("SBC"); // No oprands
		}


		[TestMethod]
		public void SBC_A_C()
		{
			var result = CompileInstruction("SBC A,C");
			Is(result, 0x99);
		}


		[TestMethod]
		public void SBC_A_D()
		{
			var result = CompileInstruction("SBC A,D");
			Is(result, 0x9A);
		}


		[TestMethod]
		public void SBC_A_E()
		{
			var result = CompileInstruction("SBC A,E");
			Is(result, 0x9B);
		}


		[TestMethod]
		public void SBC_A_H()
		{
			var result = CompileInstruction("SBC A,H");
			Is(result, 0x9C);
		}


		[TestMethod]
		public void SBC_A_L()
		{
			var result = CompileInstruction("SBC A,L");
			Is(result, 0x9D);
		}


		[TestMethod]
		public void SBC_A_HL()
		{
			var result = CompileInstruction("SBC A,(HL)");
			Is(result, 0x9E);
		}


		[TestMethod]
		public void SBC_A_A()
		{
			var result = CompileInstruction("SBC A,A");
			Is(result, 0x9F);
		}


		[TestMethod]
		public void AND_B()
		{
			var result = CompileInstruction("AND B");
			Is(result, 0xA0);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void AND_WrongNumberOfOprands()
		{
			CompileInstruction("AND"); // No oprands
		}


		[TestMethod]
		public void AND_C()
		{
			var result = CompileInstruction("AND C");
			Is(result, 0xA1);
		}


		[TestMethod]
		public void AND_D()
		{
			var result = CompileInstruction("AND D");
			Is(result, 0xA2);
		}


		[TestMethod]
		public void AND_E()
		{
			var result = CompileInstruction("AND E");
			Is(result, 0xA3);
		}


		[TestMethod]
		public void AND_H()
		{
			var result = CompileInstruction("AND H");
			Is(result, 0xA4);
		}


		[TestMethod]
		public void AND_L()
		{
			var result = CompileInstruction("AND L");
			Is(result, 0xA5);
		}


		[TestMethod]
		public void AND_HL()
		{
			var result = CompileInstruction("AND (HL)");
			Is(result, 0xA6);
		}


		[TestMethod]
		public void AND_A()
		{
			var result = CompileInstruction("AND A");
			Is(result, 0xA7);
		}


		[TestMethod]
		public void XOR_B()
		{
			var result = CompileInstruction("XOR B");
			Is(result, 0xA8);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void XOR_WrongNumberOfOprands()
		{
			CompileInstruction("XOR"); // No oprands
		}


		[TestMethod]
		public void XOR_C()
		{
			var result = CompileInstruction("XOR C");
			Is(result, 0xA9);
		}


		[TestMethod]
		public void XOR_D()
		{
			var result = CompileInstruction("XOR D");
			Is(result, 0xAA);
		}


		[TestMethod]
		public void XOR_E()
		{
			var result = CompileInstruction("XOR E");
			Is(result, 0xAB);
		}


		[TestMethod]
		public void XOR_H()
		{
			var result = CompileInstruction("XOR H");
			Is(result, 0xAC);
		}


		[TestMethod]
		public void XOR_L()
		{
			var result = CompileInstruction("XOR L");
			Is(result, 0xAD);
		}


		[TestMethod]
		public void XOR_HL()
		{
			var result = CompileInstruction("XOR (HL)");
			Is(result, 0xAE);
		}


		[TestMethod]
		public void XOR_A()
		{
			var result = CompileInstruction("XOR A");
			Is(result, 0xAF);
		}


		[TestMethod]
		public void OR_B()
		{
			var result = CompileInstruction("OR B");
			Is(result, 0xB0);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void OR_WrongNumberOfOprands()
		{
			CompileInstruction("OR"); // No oprands
		}


		[TestMethod]
		public void OR_C()
		{
			var result = CompileInstruction("OR C");
			Is(result, 0xB1);
		}


		[TestMethod]
		public void OR_D()
		{
			var result = CompileInstruction("OR D");
			Is(result, 0xB2);
		}


		[TestMethod]
		public void OR_E()
		{
			var result = CompileInstruction("OR E");
			Is(result, 0xB3);
		}


		[TestMethod]
		public void OR_H()
		{
			var result = CompileInstruction("OR H");
			Is(result, 0xB4);
		}


		[TestMethod]
		public void OR_L()
		{
			var result = CompileInstruction("OR L");
			Is(result, 0xB5);
		}


		[TestMethod]
		public void OR_HL()
		{
			var result = CompileInstruction("OR (HL)");
			Is(result, 0xB6);
		}


		[TestMethod]
		public void OR_A()
		{
			var result = CompileInstruction("OR A");
			Is(result, 0xB7);
		}


		[TestMethod]
		public void CP_B()
		{
			var result = CompileInstruction("CP B");
			Is(result, 0xB8);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CP_WrongNumberOfOprands()
		{
			CompileInstruction("CP"); // No oprands
		}


		[TestMethod]
		public void CP_C()
		{
			var result = CompileInstruction("CP C");
			Is(result, 0xB9);
		}


		[TestMethod]
		public void CP_D()
		{
			var result = CompileInstruction("CP D");
			Is(result, 0xBA);
		}


		[TestMethod]
		public void CP_E()
		{
			var result = CompileInstruction("CP E");
			Is(result, 0xBB);
		}


		[TestMethod]
		public void CP_H()
		{
			var result = CompileInstruction("CP H");
			Is(result, 0xBC);
		}


		[TestMethod]
		public void CP_L()
		{
			var result = CompileInstruction("CP L");
			Is(result, 0xBD);
		}


		[TestMethod]
		public void CP_HL()
		{
			var result = CompileInstruction("CP (HL)");
			Is(result, 0xBE);
		}


		[TestMethod]
		public void CP_A()
		{
			var result = CompileInstruction("CP A");
			Is(result, 0xBF);
		}


		[TestMethod]
		public void RET_NZ()
		{
			var result = CompileInstruction("RET NZ");
			Is(result, 0xC0);
		}


		[TestMethod]
		public void POP_BC()
		{
			var result = CompileInstruction("POP BC");
			Is(result, 0xC1);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void POP_WrongNumberOfOprands()
		{
			CompileInstruction("POP"); // No oprands
		}


		[TestMethod]
		public void JP_NZ_nn()
		{
			var result = CompileInstruction("JP NZ,62689");
			Is(result, 0xC2, 225, 244);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void JP_WrongNumberOfOprands()
		{
			CompileInstruction("JP"); // No oprands
		}


		[TestMethod]
		public void JP_nn()
		{
			var result = CompileInstruction("JP 62689");
			Is(result, 0xC3, 225, 244);
		}


		[TestMethod]
		public void CALL_NZ_nn()
		{
			var result = CompileInstruction("CALL NZ,62689");
			Is(result, 0xC4, 225, 244);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CALL_WrongNumberOfOprands()
		{
			CompileInstruction("CALL"); // No oprands
		}


		[TestMethod]
		public void PUSH_BC()
		{
			var result = CompileInstruction("PUSH BC");
			Is(result, 0xC5);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void PUSH_WrongNumberOfOprands()
		{
			CompileInstruction("PUSH"); // No oprands
		}


		[TestMethod]
		public void ADD_A_n()
		{
			var result = CompileInstruction("ADD A,225");
			Is(result, 0xC6, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void ADD_A_n_ImmediateTooBig()
		{
			CompileInstruction("ADD A,62689");
		}


		[TestMethod]
		public void RST_0()
		{
			var result = CompileInstruction("RST 0");
			Is(result, 0xC7);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RST_WrongNumberOfOprands()
		{
			CompileInstruction("RST"); // No oprands
		}


		[TestMethod]
		public void RET_Z()
		{
			var result = CompileInstruction("RET Z");
			Is(result, 0xC8);
		}


		[TestMethod]
		public void RET()
		{
			var result = CompileInstruction("RET");
			Is(result, 0xC9);
		}


		[TestMethod]
		public void JP_Z_nn()
		{
			var result = CompileInstruction("JP Z,62689");
			Is(result, 0xCA, 225, 244);
		}


		[TestMethod]
		public void CALL_Z_nn()
		{
			var result = CompileInstruction("CALL Z,62689");
			Is(result, 0xCC, 225, 244);
		}


		[TestMethod]
		public void CALL_nn()
		{
			var result = CompileInstruction("CALL 62689");
			Is(result, 0xCD, 225, 244);
		}


		[TestMethod]
		public void ADC_A_n()
		{
			var result = CompileInstruction("ADC A,225");
			Is(result, 0xCE, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void ADC_A_n_ImmediateTooBig()
		{
			CompileInstruction("ADC A,62689");
		}


		[TestMethod]
		public void RST_8()
		{
			var result = CompileInstruction("RST 8");
			Is(result, 0xCF);
		}


		[TestMethod]
		public void RET_NC()
		{
			var result = CompileInstruction("RET NC");
			Is(result, 0xD0);
		}


		[TestMethod]
		public void POP_DE()
		{
			var result = CompileInstruction("POP DE");
			Is(result, 0xD1);
		}


		[TestMethod]
		public void JP_NC_nn()
		{
			var result = CompileInstruction("JP NC,62689");
			Is(result, 0xD2, 225, 244);
		}


		[TestMethod]
		public void CALL_NC_nn()
		{
			var result = CompileInstruction("CALL NC,62689");
			Is(result, 0xD4, 225, 244);
		}


		[TestMethod]
		public void PUSH_DE()
		{
			var result = CompileInstruction("PUSH DE");
			Is(result, 0xD5);
		}


		[TestMethod]
		public void SUB_A_n()
		{
			var result = CompileInstruction("SUB A,225");
			Is(result, 0xD6, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SUB_A_n_ImmediateTooBig()
		{
			CompileInstruction("SUB A,62689");
		}


		[TestMethod]
		public void RST_10()
		{
			var result = CompileInstruction("RST 10");
			Is(result, 0xD7);
		}


		[TestMethod]
		public void RET_C()
		{
			var result = CompileInstruction("RET C");
			Is(result, 0xD8);
		}


		[TestMethod]
		public void RETI()
		{
			var result = CompileInstruction("RETI");
			Is(result, 0xD9);
		}


		[TestMethod]
		public void JP_C_nn()
		{
			var result = CompileInstruction("JP C,62689");
			Is(result, 0xDA, 225, 244);
		}


		[TestMethod]
		public void CALL_C_nn()
		{
			var result = CompileInstruction("CALL C,62689");
			Is(result, 0xDC, 225, 244);
		}


		[TestMethod]
		public void SBC_A_n()
		{
			var result = CompileInstruction("SBC A,225");
			Is(result, 0xDE, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SBC_A_n_ImmediateTooBig()
		{
			CompileInstruction("SBC A,62689");
		}


		[TestMethod]
		public void RST_18()
		{
			var result = CompileInstruction("RST 18");
			Is(result, 0xDF);
		}


		[TestMethod]
		public void LDH_n_A()
		{
			var result = CompileInstruction("LDH (225),A");
			Is(result, 0xE0, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDH_WrongNumberOfOprands()
		{
			CompileInstruction("LDH"); // No oprands
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDH_n_A_ImmediateTooBig()
		{
			CompileInstruction("LDH (62689),A");
		}


		[TestMethod]
		public void POP_HL()
		{
			var result = CompileInstruction("POP HL");
			Is(result, 0xE1);
		}


		[TestMethod]
		public void LDH_C_A()
		{
			var result = CompileInstruction("LDH (C),A");
			Is(result, 0xE2);
		}


		[TestMethod]
		public void PUSH_HL()
		{
			var result = CompileInstruction("PUSH HL");
			Is(result, 0xE5);
		}


		[TestMethod]
		public void AND_n()
		{
			var result = CompileInstruction("AND 225");
			Is(result, 0xE6, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void AND_n_ImmediateTooBig()
		{
			CompileInstruction("AND 62689");
		}


		[TestMethod]
		public void RST_20()
		{
			var result = CompileInstruction("RST 20");
			Is(result, 0xE7);
		}


		[TestMethod]
		public void ADD_SP_n()
		{
			var result = CompileInstruction("ADD SP,225");
			Is(result, 0xE8, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void ADD_SP_n_ImmediateTooBig()
		{
			CompileInstruction("ADD SP,62689");
		}


		[TestMethod]
		public void JP_HL()
		{
			var result = CompileInstruction("JP (HL)");
			Is(result, 0xE9);
		}


		[TestMethod]
		public void LD_nn_A()
		{
			var result = CompileInstruction("LD (62689),A");
			Is(result, 0xEA, 225, 244);
		}


		[TestMethod]
		public void XOR_n()
		{
			var result = CompileInstruction("XOR 225");
			Is(result, 0xEE, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void XOR_n_ImmediateTooBig()
		{
			CompileInstruction("XOR 62689");
		}


		[TestMethod]
		public void RST_28()
		{
			var result = CompileInstruction("RST 28");
			Is(result, 0xEF);
		}


		[TestMethod]
		public void LDH_A_n()
		{
			var result = CompileInstruction("LDH A,(225)");
			Is(result, 0xF0, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDH_A_n_ImmediateTooBig()
		{
			CompileInstruction("LDH A,(62689)");
		}


		[TestMethod]
		public void POP_AF()
		{
			var result = CompileInstruction("POP AF");
			Is(result, 0xF1);
		}


		[TestMethod]
		public void DI()
		{
			var result = CompileInstruction("DI");
			Is(result, 0xF3);
		}


		[TestMethod]
		public void PUSH_AF()
		{
			var result = CompileInstruction("PUSH AF");
			Is(result, 0xF5);
		}


		[TestMethod]
		public void OR_n()
		{
			var result = CompileInstruction("OR 225");
			Is(result, 0xF6, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void OR_n_ImmediateTooBig()
		{
			CompileInstruction("OR 62689");
		}


		[TestMethod]
		public void RST_30()
		{
			var result = CompileInstruction("RST 30");
			Is(result, 0xF7);
		}


		[TestMethod]
		public void LDHL_SP_n()
		{
			var result = CompileInstruction("LDHL SP,225");
			Is(result, 0xF8, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDHL_WrongNumberOfOprands()
		{
			CompileInstruction("LDHL"); // No oprands
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void LDHL_SP_n_ImmediateTooBig()
		{
			CompileInstruction("LDHL SP,62689");
		}


		[TestMethod]
		public void LD_SP_HL()
		{
			var result = CompileInstruction("LD SP,HL");
			Is(result, 0xF9);
		}


		[TestMethod]
		public void LD_A_nn()
		{
			var result = CompileInstruction("LD A,(62689)");
			Is(result, 0xFA, 225, 244);
		}


		[TestMethod]
		public void EI()
		{
			var result = CompileInstruction("EI");
			Is(result, 0xFB);
		}


		[TestMethod]
		public void CP_n()
		{
			var result = CompileInstruction("CP 225");
			Is(result, 0xFE, 225);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CP_n_ImmediateTooBig()
		{
			CompileInstruction("CP 62689");
		}


		[TestMethod]
		public void CB_RLC_B()
		{
			var result = CompileInstruction("RLC B");
			Is(result, 0xCB, 0x00);
		}


		[TestMethod]
		public void CB_RLC_C()
		{
			var result = CompileInstruction("RLC C");
			Is(result, 0xCB, 0x01);
		}


		[TestMethod]
		public void CB_RLC_D()
		{
			var result = CompileInstruction("RLC D");
			Is(result, 0xCB, 0x02);
		}


		[TestMethod]
		public void CB_RLC_E()
		{
			var result = CompileInstruction("RLC E");
			Is(result, 0xCB, 0x03);
		}


		[TestMethod]
		public void CB_RLC_H()
		{
			var result = CompileInstruction("RLC H");
			Is(result, 0xCB, 0x04);
		}


		[TestMethod]
		public void CB_RLC_L()
		{
			var result = CompileInstruction("RLC L");
			Is(result, 0xCB, 0x05);
		}


		[TestMethod]
		public void CB_RLC_HL()
		{
			var result = CompileInstruction("RLC (HL)");
			Is(result, 0xCB, 0x06);
		}


		[TestMethod]
		public void CB_RLC_A()
		{
			var result = CompileInstruction("RLC A");
			Is(result, 0x07);
		}


		[TestMethod]
		public void CB_RRC_B()
		{
			var result = CompileInstruction("RRC B");
			Is(result, 0xCB, 0x08);
		}


		[TestMethod]
		public void CB_RRC_C()
		{
			var result = CompileInstruction("RRC C");
			Is(result, 0xCB, 0x09);
		}


		[TestMethod]
		public void CB_RRC_D()
		{
			var result = CompileInstruction("RRC D");
			Is(result, 0xCB, 0x0A);
		}


		[TestMethod]
		public void CB_RRC_E()
		{
			var result = CompileInstruction("RRC E");
			Is(result, 0xCB, 0x0B);
		}


		[TestMethod]
		public void CB_RRC_H()
		{
			var result = CompileInstruction("RRC H");
			Is(result, 0xCB, 0x0C);
		}


		[TestMethod]
		public void CB_RRC_L()
		{
			var result = CompileInstruction("RRC L");
			Is(result, 0xCB, 0x0D);
		}


		[TestMethod]
		public void CB_RRC_HL()
		{
			var result = CompileInstruction("RRC (HL)");
			Is(result, 0xCB, 0x0E);
		}


		[TestMethod]
		public void CB_RRC_A()
		{
			var result = CompileInstruction("RRC A");
			Is(result, 0x0F);
		}


		[TestMethod]
		public void CB_RL_B()
		{
			var result = CompileInstruction("RL B");
			Is(result, 0xCB, 0x10);
		}


		[TestMethod]
		public void CB_RL_C()
		{
			var result = CompileInstruction("RL C");
			Is(result, 0xCB, 0x11);
		}


		[TestMethod]
		public void CB_RL_D()
		{
			var result = CompileInstruction("RL D");
			Is(result, 0xCB, 0x12);
		}


		[TestMethod]
		public void CB_RL_E()
		{
			var result = CompileInstruction("RL E");
			Is(result, 0xCB, 0x13);
		}


		[TestMethod]
		public void CB_RL_H()
		{
			var result = CompileInstruction("RL H");
			Is(result, 0xCB, 0x14);
		}


		[TestMethod]
		public void CB_RL_L()
		{
			var result = CompileInstruction("RL L");
			Is(result, 0xCB, 0x15);
		}


		[TestMethod]
		public void CB_RL_HL()
		{
			var result = CompileInstruction("RL (HL)");
			Is(result, 0xCB, 0x16);
		}


		[TestMethod]
		public void CB_RL_A()
		{
			var result = CompileInstruction("RL A");
			Is(result, 0x17);
		}


		[TestMethod]
		public void CB_RR_B()
		{
			var result = CompileInstruction("RR B");
			Is(result, 0xCB, 0x18);
		}


		[TestMethod]
		public void CB_RR_C()
		{
			var result = CompileInstruction("RR C");
			Is(result, 0xCB, 0x19);
		}


		[TestMethod]
		public void CB_RR_D()
		{
			var result = CompileInstruction("RR D");
			Is(result, 0xCB, 0x1A);
		}


		[TestMethod]
		public void CB_RR_E()
		{
			var result = CompileInstruction("RR E");
			Is(result, 0xCB, 0x1B);
		}


		[TestMethod]
		public void CB_RR_H()
		{
			var result = CompileInstruction("RR H");
			Is(result, 0xCB, 0x1C);
		}


		[TestMethod]
		public void CB_RR_L()
		{
			var result = CompileInstruction("RR L");
			Is(result, 0xCB, 0x1D);
		}


		[TestMethod]
		public void CB_RR_HL()
		{
			var result = CompileInstruction("RR (HL)");
			Is(result, 0xCB, 0x1E);
		}


		[TestMethod]
		public void CB_RR_A()
		{
			var result = CompileInstruction("RR A");
			Is(result, 0x1F);
		}


		[TestMethod]
		public void CB_SLA_B()
		{
			var result = CompileInstruction("SLA B");
			Is(result, 0xCB, 0x20);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SLA_WrongNumberOfOprands()
		{
			CompileInstruction("SLA"); // No oprands
		}


		[TestMethod]
		public void CB_SLA_C()
		{
			var result = CompileInstruction("SLA C");
			Is(result, 0xCB, 0x21);
		}


		[TestMethod]
		public void CB_SLA_D()
		{
			var result = CompileInstruction("SLA D");
			Is(result, 0xCB, 0x22);
		}


		[TestMethod]
		public void CB_SLA_E()
		{
			var result = CompileInstruction("SLA E");
			Is(result, 0xCB, 0x23);
		}


		[TestMethod]
		public void CB_SLA_H()
		{
			var result = CompileInstruction("SLA H");
			Is(result, 0xCB, 0x24);
		}


		[TestMethod]
		public void CB_SLA_L()
		{
			var result = CompileInstruction("SLA L");
			Is(result, 0xCB, 0x25);
		}


		[TestMethod]
		public void CB_SLA_HL()
		{
			var result = CompileInstruction("SLA (HL)");
			Is(result, 0xCB, 0x26);
		}


		[TestMethod]
		public void CB_SLA_A()
		{
			var result = CompileInstruction("SLA A");
			Is(result, 0xCB, 0x27);
		}


		[TestMethod]
		public void CB_SRA_B()
		{
			var result = CompileInstruction("SRA B");
			Is(result, 0xCB, 0x28);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SRA_WrongNumberOfOprands()
		{
			CompileInstruction("SRA"); // No oprands
		}


		[TestMethod]
		public void CB_SRA_C()
		{
			var result = CompileInstruction("SRA C");
			Is(result, 0xCB, 0x29);
		}


		[TestMethod]
		public void CB_SRA_D()
		{
			var result = CompileInstruction("SRA D");
			Is(result, 0xCB, 0x2A);
		}


		[TestMethod]
		public void CB_SRA_E()
		{
			var result = CompileInstruction("SRA E");
			Is(result, 0xCB, 0x2B);
		}


		[TestMethod]
		public void CB_SRA_H()
		{
			var result = CompileInstruction("SRA H");
			Is(result, 0xCB, 0x2C);
		}


		[TestMethod]
		public void CB_SRA_L()
		{
			var result = CompileInstruction("SRA L");
			Is(result, 0xCB, 0x2D);
		}


		[TestMethod]
		public void CB_SRA_HL()
		{
			var result = CompileInstruction("SRA (HL)");
			Is(result, 0xCB, 0x2E);
		}


		[TestMethod]
		public void CB_SRA_A()
		{
			var result = CompileInstruction("SRA A");
			Is(result, 0xCB, 0x2F);
		}


		[TestMethod]
		public void CB_SWAP_B()
		{
			var result = CompileInstruction("SWAP B");
			Is(result, 0xCB, 0x30);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SWAP_WrongNumberOfOprands()
		{
			CompileInstruction("SWAP"); // No oprands
		}


		[TestMethod]
		public void CB_SWAP_C()
		{
			var result = CompileInstruction("SWAP C");
			Is(result, 0xCB, 0x31);
		}


		[TestMethod]
		public void CB_SWAP_D()
		{
			var result = CompileInstruction("SWAP D");
			Is(result, 0xCB, 0x32);
		}


		[TestMethod]
		public void CB_SWAP_E()
		{
			var result = CompileInstruction("SWAP E");
			Is(result, 0xCB, 0x33);
		}


		[TestMethod]
		public void CB_SWAP_H()
		{
			var result = CompileInstruction("SWAP H");
			Is(result, 0xCB, 0x34);
		}


		[TestMethod]
		public void CB_SWAP_L()
		{
			var result = CompileInstruction("SWAP L");
			Is(result, 0xCB, 0x35);
		}


		[TestMethod]
		public void CB_SWAP_HL()
		{
			var result = CompileInstruction("SWAP (HL)");
			Is(result, 0xCB, 0x36);
		}


		[TestMethod]
		public void CB_SWAP_A()
		{
			var result = CompileInstruction("SWAP A");
			Is(result, 0xCB, 0x37);
		}


		[TestMethod]
		public void CB_SRL_B()
		{
			var result = CompileInstruction("SRL B");
			Is(result, 0xCB, 0x38);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SRL_WrongNumberOfOprands()
		{
			CompileInstruction("SRL"); // No oprands
		}


		[TestMethod]
		public void CB_SRL_C()
		{
			var result = CompileInstruction("SRL C");
			Is(result, 0xCB, 0x39);
		}


		[TestMethod]
		public void CB_SRL_D()
		{
			var result = CompileInstruction("SRL D");
			Is(result, 0xCB, 0x3A);
		}


		[TestMethod]
		public void CB_SRL_E()
		{
			var result = CompileInstruction("SRL E");
			Is(result, 0xCB, 0x3B);
		}


		[TestMethod]
		public void CB_SRL_H()
		{
			var result = CompileInstruction("SRL H");
			Is(result, 0xCB, 0x3C);
		}


		[TestMethod]
		public void CB_SRL_L()
		{
			var result = CompileInstruction("SRL L");
			Is(result, 0xCB, 0x3D);
		}


		[TestMethod]
		public void CB_SRL_HL()
		{
			var result = CompileInstruction("SRL (HL)");
			Is(result, 0xCB, 0x3E);
		}


		[TestMethod]
		public void CB_SRL_A()
		{
			var result = CompileInstruction("SRL A");
			Is(result, 0xCB, 0x3F);
		}


		[TestMethod]
		public void CB_BIT_0_B()
		{
			var result = CompileInstruction("BIT 0,B");
			Is(result, 0xCB, 0x40);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void BIT_WrongNumberOfOprands()
		{
			CompileInstruction("BIT"); // No oprands
		}


		[TestMethod]
		public void CB_BIT_0_C()
		{
			var result = CompileInstruction("BIT 0,C");
			Is(result, 0xCB, 0x41);
		}


		[TestMethod]
		public void CB_BIT_0_D()
		{
			var result = CompileInstruction("BIT 0,D");
			Is(result, 0xCB, 0x42);
		}


		[TestMethod]
		public void CB_BIT_0_E()
		{
			var result = CompileInstruction("BIT 0,E");
			Is(result, 0xCB, 0x43);
		}


		[TestMethod]
		public void CB_BIT_0_H()
		{
			var result = CompileInstruction("BIT 0,H");
			Is(result, 0xCB, 0x44);
		}


		[TestMethod]
		public void CB_BIT_0_L()
		{
			var result = CompileInstruction("BIT 0,L");
			Is(result, 0xCB, 0x45);
		}


		[TestMethod]
		public void CB_BIT_0_HL()
		{
			var result = CompileInstruction("BIT 0,(HL)");
			Is(result, 0xCB, 0x46);
		}


		[TestMethod]
		public void CB_BIT_0_A()
		{
			var result = CompileInstruction("BIT 0,A");
			Is(result, 0xCB, 0x47);
		}


		[TestMethod]
		public void CB_BIT_1_B()
		{
			var result = CompileInstruction("BIT 1,B");
			Is(result, 0xCB, 0x48);
		}


		[TestMethod]
		public void CB_BIT_1_C()
		{
			var result = CompileInstruction("BIT 1,C");
			Is(result, 0xCB, 0x49);
		}


		[TestMethod]
		public void CB_BIT_1_D()
		{
			var result = CompileInstruction("BIT 1,D");
			Is(result, 0xCB, 0x4A);
		}


		[TestMethod]
		public void CB_BIT_1_E()
		{
			var result = CompileInstruction("BIT 1,E");
			Is(result, 0xCB, 0x4B);
		}


		[TestMethod]
		public void CB_BIT_1_H()
		{
			var result = CompileInstruction("BIT 1,H");
			Is(result, 0xCB, 0x4C);
		}


		[TestMethod]
		public void CB_BIT_1_L()
		{
			var result = CompileInstruction("BIT 1,L");
			Is(result, 0xCB, 0x4D);
		}


		[TestMethod]
		public void CB_BIT_1_HL()
		{
			var result = CompileInstruction("BIT 1,(HL)");
			Is(result, 0xCB, 0x4E);
		}


		[TestMethod]
		public void CB_BIT_1_A()
		{
			var result = CompileInstruction("BIT 1,A");
			Is(result, 0xCB, 0x4F);
		}


		[TestMethod]
		public void CB_BIT_2_B()
		{
			var result = CompileInstruction("BIT 2,B");
			Is(result, 0xCB, 0x50);
		}


		[TestMethod]
		public void CB_BIT_2_C()
		{
			var result = CompileInstruction("BIT 2,C");
			Is(result, 0xCB, 0x51);
		}


		[TestMethod]
		public void CB_BIT_2_D()
		{
			var result = CompileInstruction("BIT 2,D");
			Is(result, 0xCB, 0x52);
		}


		[TestMethod]
		public void CB_BIT_2_E()
		{
			var result = CompileInstruction("BIT 2,E");
			Is(result, 0xCB, 0x53);
		}


		[TestMethod]
		public void CB_BIT_2_H()
		{
			var result = CompileInstruction("BIT 2,H");
			Is(result, 0xCB, 0x54);
		}


		[TestMethod]
		public void CB_BIT_2_L()
		{
			var result = CompileInstruction("BIT 2,L");
			Is(result, 0xCB, 0x55);
		}


		[TestMethod]
		public void CB_BIT_2_HL()
		{
			var result = CompileInstruction("BIT 2,(HL)");
			Is(result, 0xCB, 0x56);
		}


		[TestMethod]
		public void CB_BIT_2_A()
		{
			var result = CompileInstruction("BIT 2,A");
			Is(result, 0xCB, 0x57);
		}


		[TestMethod]
		public void CB_BIT_3_B()
		{
			var result = CompileInstruction("BIT 3,B");
			Is(result, 0xCB, 0x58);
		}


		[TestMethod]
		public void CB_BIT_3_C()
		{
			var result = CompileInstruction("BIT 3,C");
			Is(result, 0xCB, 0x59);
		}


		[TestMethod]
		public void CB_BIT_3_D()
		{
			var result = CompileInstruction("BIT 3,D");
			Is(result, 0xCB, 0x5A);
		}


		[TestMethod]
		public void CB_BIT_3_E()
		{
			var result = CompileInstruction("BIT 3,E");
			Is(result, 0xCB, 0x5B);
		}


		[TestMethod]
		public void CB_BIT_3_H()
		{
			var result = CompileInstruction("BIT 3,H");
			Is(result, 0xCB, 0x5C);
		}


		[TestMethod]
		public void CB_BIT_3_L()
		{
			var result = CompileInstruction("BIT 3,L");
			Is(result, 0xCB, 0x5D);
		}


		[TestMethod]
		public void CB_BIT_3_HL()
		{
			var result = CompileInstruction("BIT 3,(HL)");
			Is(result, 0xCB, 0x5E);
		}


		[TestMethod]
		public void CB_BIT_3_A()
		{
			var result = CompileInstruction("BIT 3,A");
			Is(result, 0xCB, 0x5F);
		}


		[TestMethod]
		public void CB_BIT_4_B()
		{
			var result = CompileInstruction("BIT 4,B");
			Is(result, 0xCB, 0x60);
		}


		[TestMethod]
		public void CB_BIT_4_C()
		{
			var result = CompileInstruction("BIT 4,C");
			Is(result, 0xCB, 0x61);
		}


		[TestMethod]
		public void CB_BIT_4_D()
		{
			var result = CompileInstruction("BIT 4,D");
			Is(result, 0xCB, 0x62);
		}


		[TestMethod]
		public void CB_BIT_4_E()
		{
			var result = CompileInstruction("BIT 4,E");
			Is(result, 0xCB, 0x63);
		}


		[TestMethod]
		public void CB_BIT_4_H()
		{
			var result = CompileInstruction("BIT 4,H");
			Is(result, 0xCB, 0x64);
		}


		[TestMethod]
		public void CB_BIT_4_L()
		{
			var result = CompileInstruction("BIT 4,L");
			Is(result, 0xCB, 0x65);
		}


		[TestMethod]
		public void CB_BIT_4_HL()
		{
			var result = CompileInstruction("BIT 4,(HL)");
			Is(result, 0xCB, 0x66);
		}


		[TestMethod]
		public void CB_BIT_4_A()
		{
			var result = CompileInstruction("BIT 4,A");
			Is(result, 0xCB, 0x67);
		}


		[TestMethod]
		public void CB_BIT_5_B()
		{
			var result = CompileInstruction("BIT 5,B");
			Is(result, 0xCB, 0x68);
		}


		[TestMethod]
		public void CB_BIT_5_C()
		{
			var result = CompileInstruction("BIT 5,C");
			Is(result, 0xCB, 0x69);
		}


		[TestMethod]
		public void CB_BIT_5_D()
		{
			var result = CompileInstruction("BIT 5,D");
			Is(result, 0xCB, 0x6A);
		}


		[TestMethod]
		public void CB_BIT_5_E()
		{
			var result = CompileInstruction("BIT 5,E");
			Is(result, 0xCB, 0x6B);
		}


		[TestMethod]
		public void CB_BIT_5_H()
		{
			var result = CompileInstruction("BIT 5,H");
			Is(result, 0xCB, 0x6C);
		}


		[TestMethod]
		public void CB_BIT_5_L()
		{
			var result = CompileInstruction("BIT 5,L");
			Is(result, 0xCB, 0x6D);
		}


		[TestMethod]
		public void CB_BIT_5_HL()
		{
			var result = CompileInstruction("BIT 5,(HL)");
			Is(result, 0xCB, 0x6E);
		}


		[TestMethod]
		public void CB_BIT_5_A()
		{
			var result = CompileInstruction("BIT 5,A");
			Is(result, 0xCB, 0x6F);
		}


		[TestMethod]
		public void CB_BIT_6_B()
		{
			var result = CompileInstruction("BIT 6,B");
			Is(result, 0xCB, 0x70);
		}


		[TestMethod]
		public void CB_BIT_6_C()
		{
			var result = CompileInstruction("BIT 6,C");
			Is(result, 0xCB, 0x71);
		}


		[TestMethod]
		public void CB_BIT_6_D()
		{
			var result = CompileInstruction("BIT 6,D");
			Is(result, 0xCB, 0x72);
		}


		[TestMethod]
		public void CB_BIT_6_E()
		{
			var result = CompileInstruction("BIT 6,E");
			Is(result, 0xCB, 0x73);
		}


		[TestMethod]
		public void CB_BIT_6_H()
		{
			var result = CompileInstruction("BIT 6,H");
			Is(result, 0xCB, 0x74);
		}


		[TestMethod]
		public void CB_BIT_6_L()
		{
			var result = CompileInstruction("BIT 6,L");
			Is(result, 0xCB, 0x75);
		}


		[TestMethod]
		public void CB_BIT_6_HL()
		{
			var result = CompileInstruction("BIT 6,(HL)");
			Is(result, 0xCB, 0x76);
		}


		[TestMethod]
		public void CB_BIT_6_A()
		{
			var result = CompileInstruction("BIT 6,A");
			Is(result, 0xCB, 0x77);
		}


		[TestMethod]
		public void CB_BIT_7_B()
		{
			var result = CompileInstruction("BIT 7,B");
			Is(result, 0xCB, 0x78);
		}


		[TestMethod]
		public void CB_BIT_7_C()
		{
			var result = CompileInstruction("BIT 7,C");
			Is(result, 0xCB, 0x79);
		}


		[TestMethod]
		public void CB_BIT_7_D()
		{
			var result = CompileInstruction("BIT 7,D");
			Is(result, 0xCB, 0x7A);
		}


		[TestMethod]
		public void CB_BIT_7_E()
		{
			var result = CompileInstruction("BIT 7,E");
			Is(result, 0xCB, 0x7B);
		}


		[TestMethod]
		public void CB_BIT_7_H()
		{
			var result = CompileInstruction("BIT 7,H");
			Is(result, 0xCB, 0x7C);
		}


		[TestMethod]
		public void CB_BIT_7_L()
		{
			var result = CompileInstruction("BIT 7,L");
			Is(result, 0xCB, 0x7D);
		}


		[TestMethod]
		public void CB_BIT_7_HL()
		{
			var result = CompileInstruction("BIT 7,(HL)");
			Is(result, 0xCB, 0x7E);
		}


		[TestMethod]
		public void CB_BIT_7_A()
		{
			var result = CompileInstruction("BIT 7,A");
			Is(result, 0xCB, 0x7F);
		}


		[TestMethod]
		public void CB_RES_0_B()
		{
			var result = CompileInstruction("RES 0,B");
			Is(result, 0xCB, 0x80);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void RES_WrongNumberOfOprands()
		{
			CompileInstruction("RES"); // No oprands
		}


		[TestMethod]
		public void CB_RES_0_C()
		{
			var result = CompileInstruction("RES 0,C");
			Is(result, 0xCB, 0x81);
		}


		[TestMethod]
		public void CB_RES_0_D()
		{
			var result = CompileInstruction("RES 0,D");
			Is(result, 0xCB, 0x82);
		}


		[TestMethod]
		public void CB_RES_0_E()
		{
			var result = CompileInstruction("RES 0,E");
			Is(result, 0xCB, 0x83);
		}


		[TestMethod]
		public void CB_RES_0_H()
		{
			var result = CompileInstruction("RES 0,H");
			Is(result, 0xCB, 0x84);
		}


		[TestMethod]
		public void CB_RES_0_L()
		{
			var result = CompileInstruction("RES 0,L");
			Is(result, 0xCB, 0x85);
		}


		[TestMethod]
		public void CB_RES_0_HL()
		{
			var result = CompileInstruction("RES 0,(HL)");
			Is(result, 0xCB, 0x86);
		}


		[TestMethod]
		public void CB_RES_0_A()
		{
			var result = CompileInstruction("RES 0,A");
			Is(result, 0xCB, 0x87);
		}


		[TestMethod]
		public void CB_RES_1_B()
		{
			var result = CompileInstruction("RES 1,B");
			Is(result, 0xCB, 0x88);
		}


		[TestMethod]
		public void CB_RES_1_C()
		{
			var result = CompileInstruction("RES 1,C");
			Is(result, 0xCB, 0x89);
		}


		[TestMethod]
		public void CB_RES_1_D()
		{
			var result = CompileInstruction("RES 1,D");
			Is(result, 0xCB, 0x8A);
		}


		[TestMethod]
		public void CB_RES_1_E()
		{
			var result = CompileInstruction("RES 1,E");
			Is(result, 0xCB, 0x8B);
		}


		[TestMethod]
		public void CB_RES_1_H()
		{
			var result = CompileInstruction("RES 1,H");
			Is(result, 0xCB, 0x8C);
		}


		[TestMethod]
		public void CB_RES_1_L()
		{
			var result = CompileInstruction("RES 1,L");
			Is(result, 0xCB, 0x8D);
		}


		[TestMethod]
		public void CB_RES_1_HL()
		{
			var result = CompileInstruction("RES 1,(HL)");
			Is(result, 0xCB, 0x8E);
		}


		[TestMethod]
		public void CB_RES_1_A()
		{
			var result = CompileInstruction("RES 1,A");
			Is(result, 0xCB, 0x8F);
		}


		[TestMethod]
		public void CB_RES_2_B()
		{
			var result = CompileInstruction("RES 2,B");
			Is(result, 0xCB, 0x90);
		}


		[TestMethod]
		public void CB_RES_2_C()
		{
			var result = CompileInstruction("RES 2,C");
			Is(result, 0xCB, 0x91);
		}


		[TestMethod]
		public void CB_RES_2_D()
		{
			var result = CompileInstruction("RES 2,D");
			Is(result, 0xCB, 0x92);
		}


		[TestMethod]
		public void CB_RES_2_E()
		{
			var result = CompileInstruction("RES 2,E");
			Is(result, 0xCB, 0x93);
		}


		[TestMethod]
		public void CB_RES_2_H()
		{
			var result = CompileInstruction("RES 2,H");
			Is(result, 0xCB, 0x94);
		}


		[TestMethod]
		public void CB_RES_2_L()
		{
			var result = CompileInstruction("RES 2,L");
			Is(result, 0xCB, 0x95);
		}


		[TestMethod]
		public void CB_RES_2_HL()
		{
			var result = CompileInstruction("RES 2,(HL)");
			Is(result, 0xCB, 0x96);
		}


		[TestMethod]
		public void CB_RES_2_A()
		{
			var result = CompileInstruction("RES 2,A");
			Is(result, 0xCB, 0x97);
		}


		[TestMethod]
		public void CB_RES_3_B()
		{
			var result = CompileInstruction("RES 3,B");
			Is(result, 0xCB, 0x98);
		}


		[TestMethod]
		public void CB_RES_3_C()
		{
			var result = CompileInstruction("RES 3,C");
			Is(result, 0xCB, 0x99);
		}


		[TestMethod]
		public void CB_RES_3_D()
		{
			var result = CompileInstruction("RES 3,D");
			Is(result, 0xCB, 0x9A);
		}


		[TestMethod]
		public void CB_RES_3_E()
		{
			var result = CompileInstruction("RES 3,E");
			Is(result, 0xCB, 0x9B);
		}


		[TestMethod]
		public void CB_RES_3_H()
		{
			var result = CompileInstruction("RES 3,H");
			Is(result, 0xCB, 0x9C);
		}


		[TestMethod]
		public void CB_RES_3_L()
		{
			var result = CompileInstruction("RES 3,L");
			Is(result, 0xCB, 0x9D);
		}


		[TestMethod]
		public void CB_RES_3_HL()
		{
			var result = CompileInstruction("RES 3,(HL)");
			Is(result, 0xCB, 0x9E);
		}


		[TestMethod]
		public void CB_RES_3_A()
		{
			var result = CompileInstruction("RES 3,A");
			Is(result, 0xCB, 0x9F);
		}


		[TestMethod]
		public void CB_RES_4_B()
		{
			var result = CompileInstruction("RES 4,B");
			Is(result, 0xCB, 0xA0);
		}


		[TestMethod]
		public void CB_RES_4_C()
		{
			var result = CompileInstruction("RES 4,C");
			Is(result, 0xCB, 0xA1);
		}


		[TestMethod]
		public void CB_RES_4_D()
		{
			var result = CompileInstruction("RES 4,D");
			Is(result, 0xCB, 0xA2);
		}


		[TestMethod]
		public void CB_RES_4_E()
		{
			var result = CompileInstruction("RES 4,E");
			Is(result, 0xCB, 0xA3);
		}


		[TestMethod]
		public void CB_RES_4_H()
		{
			var result = CompileInstruction("RES 4,H");
			Is(result, 0xCB, 0xA4);
		}


		[TestMethod]
		public void CB_RES_4_L()
		{
			var result = CompileInstruction("RES 4,L");
			Is(result, 0xCB, 0xA5);
		}


		[TestMethod]
		public void CB_RES_4_HL()
		{
			var result = CompileInstruction("RES 4,(HL)");
			Is(result, 0xCB, 0xA6);
		}


		[TestMethod]
		public void CB_RES_4_A()
		{
			var result = CompileInstruction("RES 4,A");
			Is(result, 0xCB, 0xA7);
		}


		[TestMethod]
		public void CB_RES_5_B()
		{
			var result = CompileInstruction("RES 5,B");
			Is(result, 0xCB, 0xA8);
		}


		[TestMethod]
		public void CB_RES_5_C()
		{
			var result = CompileInstruction("RES 5,C");
			Is(result, 0xCB, 0xA9);
		}


		[TestMethod]
		public void CB_RES_5_D()
		{
			var result = CompileInstruction("RES 5,D");
			Is(result, 0xCB, 0xAA);
		}


		[TestMethod]
		public void CB_RES_5_E()
		{
			var result = CompileInstruction("RES 5,E");
			Is(result, 0xCB, 0xAB);
		}


		[TestMethod]
		public void CB_RES_5_H()
		{
			var result = CompileInstruction("RES 5,H");
			Is(result, 0xCB, 0xAC);
		}


		[TestMethod]
		public void CB_RES_5_L()
		{
			var result = CompileInstruction("RES 5,L");
			Is(result, 0xCB, 0xAD);
		}


		[TestMethod]
		public void CB_RES_5_HL()
		{
			var result = CompileInstruction("RES 5,(HL)");
			Is(result, 0xCB, 0xAE);
		}


		[TestMethod]
		public void CB_RES_5_A()
		{
			var result = CompileInstruction("RES 5,A");
			Is(result, 0xCB, 0xAF);
		}


		[TestMethod]
		public void CB_RES_6_B()
		{
			var result = CompileInstruction("RES 6,B");
			Is(result, 0xCB, 0xB0);
		}


		[TestMethod]
		public void CB_RES_6_C()
		{
			var result = CompileInstruction("RES 6,C");
			Is(result, 0xCB, 0xB1);
		}


		[TestMethod]
		public void CB_RES_6_D()
		{
			var result = CompileInstruction("RES 6,D");
			Is(result, 0xCB, 0xB2);
		}


		[TestMethod]
		public void CB_RES_6_E()
		{
			var result = CompileInstruction("RES 6,E");
			Is(result, 0xCB, 0xB3);
		}


		[TestMethod]
		public void CB_RES_6_H()
		{
			var result = CompileInstruction("RES 6,H");
			Is(result, 0xCB, 0xB4);
		}


		[TestMethod]
		public void CB_RES_6_L()
		{
			var result = CompileInstruction("RES 6,L");
			Is(result, 0xCB, 0xB5);
		}


		[TestMethod]
		public void CB_RES_6_HL()
		{
			var result = CompileInstruction("RES 6,(HL)");
			Is(result, 0xCB, 0xB6);
		}


		[TestMethod]
		public void CB_RES_6_A()
		{
			var result = CompileInstruction("RES 6,A");
			Is(result, 0xCB, 0xB7);
		}


		[TestMethod]
		public void CB_RES_7_B()
		{
			var result = CompileInstruction("RES 7,B");
			Is(result, 0xCB, 0xB8);
		}


		[TestMethod]
		public void CB_RES_7_C()
		{
			var result = CompileInstruction("RES 7,C");
			Is(result, 0xCB, 0xB9);
		}


		[TestMethod]
		public void CB_RES_7_D()
		{
			var result = CompileInstruction("RES 7,D");
			Is(result, 0xCB, 0xBA);
		}


		[TestMethod]
		public void CB_RES_7_E()
		{
			var result = CompileInstruction("RES 7,E");
			Is(result, 0xCB, 0xBB);
		}


		[TestMethod]
		public void CB_RES_7_H()
		{
			var result = CompileInstruction("RES 7,H");
			Is(result, 0xCB, 0xBC);
		}


		[TestMethod]
		public void CB_RES_7_L()
		{
			var result = CompileInstruction("RES 7,L");
			Is(result, 0xCB, 0xBD);
		}


		[TestMethod]
		public void CB_RES_7_HL()
		{
			var result = CompileInstruction("RES 7,(HL)");
			Is(result, 0xCB, 0xBE);
		}


		[TestMethod]
		public void CB_RES_7_A()
		{
			var result = CompileInstruction("RES 7,A");
			Is(result, 0xCB, 0xBF);
		}


		[TestMethod]
		public void CB_SET_0_B()
		{
			var result = CompileInstruction("SET 0,B");
			Is(result, 0xCB, 0xC0);
		}
		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void SET_WrongNumberOfOprands()
		{
			CompileInstruction("SET"); // No oprands
		}


		[TestMethod]
		public void CB_SET_0_C()
		{
			var result = CompileInstruction("SET 0,C");
			Is(result, 0xCB, 0xC1);
		}


		[TestMethod]
		public void CB_SET_0_D()
		{
			var result = CompileInstruction("SET 0,D");
			Is(result, 0xCB, 0xC2);
		}


		[TestMethod]
		public void CB_SET_0_E()
		{
			var result = CompileInstruction("SET 0,E");
			Is(result, 0xCB, 0xC3);
		}


		[TestMethod]
		public void CB_SET_0_H()
		{
			var result = CompileInstruction("SET 0,H");
			Is(result, 0xCB, 0xC4);
		}


		[TestMethod]
		public void CB_SET_0_L()
		{
			var result = CompileInstruction("SET 0,L");
			Is(result, 0xCB, 0xC5);
		}


		[TestMethod]
		public void CB_SET_0_HL()
		{
			var result = CompileInstruction("SET 0,(HL)");
			Is(result, 0xCB, 0xC6);
		}


		[TestMethod]
		public void CB_SET_0_A()
		{
			var result = CompileInstruction("SET 0,A");
			Is(result, 0xCB, 0xC7);
		}


		[TestMethod]
		public void CB_SET_1_B()
		{
			var result = CompileInstruction("SET 1,B");
			Is(result, 0xCB, 0xC8);
		}


		[TestMethod]
		public void CB_SET_1_C()
		{
			var result = CompileInstruction("SET 1,C");
			Is(result, 0xCB, 0xC9);
		}


		[TestMethod]
		public void CB_SET_1_D()
		{
			var result = CompileInstruction("SET 1,D");
			Is(result, 0xCB, 0xCA);
		}


		[TestMethod]
		public void CB_SET_1_E()
		{
			var result = CompileInstruction("SET 1,E");
			Is(result, 0xCB, 0xCB);
		}


		[TestMethod]
		public void CB_SET_1_H()
		{
			var result = CompileInstruction("SET 1,H");
			Is(result, 0xCB, 0xCC);
		}


		[TestMethod]
		public void CB_SET_1_L()
		{
			var result = CompileInstruction("SET 1,L");
			Is(result, 0xCB, 0xCD);
		}


		[TestMethod]
		public void CB_SET_1_HL()
		{
			var result = CompileInstruction("SET 1,(HL)");
			Is(result, 0xCB, 0xCE);
		}


		[TestMethod]
		public void CB_SET_1_A()
		{
			var result = CompileInstruction("SET 1,A");
			Is(result, 0xCB, 0xCF);
		}


		[TestMethod]
		public void CB_SET_2_B()
		{
			var result = CompileInstruction("SET 2,B");
			Is(result, 0xCB, 0xD0);
		}


		[TestMethod]
		public void CB_SET_2_C()
		{
			var result = CompileInstruction("SET 2,C");
			Is(result, 0xCB, 0xD1);
		}


		[TestMethod]
		public void CB_SET_2_D()
		{
			var result = CompileInstruction("SET 2,D");
			Is(result, 0xCB, 0xD2);
		}


		[TestMethod]
		public void CB_SET_2_E()
		{
			var result = CompileInstruction("SET 2,E");
			Is(result, 0xCB, 0xD3);
		}


		[TestMethod]
		public void CB_SET_2_H()
		{
			var result = CompileInstruction("SET 2,H");
			Is(result, 0xCB, 0xD4);
		}


		[TestMethod]
		public void CB_SET_2_L()
		{
			var result = CompileInstruction("SET 2,L");
			Is(result, 0xCB, 0xD5);
		}


		[TestMethod]
		public void CB_SET_2_HL()
		{
			var result = CompileInstruction("SET 2,(HL)");
			Is(result, 0xCB, 0xD6);
		}


		[TestMethod]
		public void CB_SET_2_A()
		{
			var result = CompileInstruction("SET 2,A");
			Is(result, 0xCB, 0xD7);
		}


		[TestMethod]
		public void CB_SET_3_B()
		{
			var result = CompileInstruction("SET 3,B");
			Is(result, 0xCB, 0xD8);
		}


		[TestMethod]
		public void CB_SET_3_C()
		{
			var result = CompileInstruction("SET 3,C");
			Is(result, 0xCB, 0xD9);
		}


		[TestMethod]
		public void CB_SET_3_D()
		{
			var result = CompileInstruction("SET 3,D");
			Is(result, 0xCB, 0xDA);
		}


		[TestMethod]
		public void CB_SET_3_E()
		{
			var result = CompileInstruction("SET 3,E");
			Is(result, 0xCB, 0xDB);
		}


		[TestMethod]
		public void CB_SET_3_H()
		{
			var result = CompileInstruction("SET 3,H");
			Is(result, 0xCB, 0xDC);
		}


		[TestMethod]
		public void CB_SET_3_L()
		{
			var result = CompileInstruction("SET 3,L");
			Is(result, 0xCB, 0xDD);
		}


		[TestMethod]
		public void CB_SET_3_HL()
		{
			var result = CompileInstruction("SET 3,(HL)");
			Is(result, 0xCB, 0xDE);
		}


		[TestMethod]
		public void CB_SET_3_A()
		{
			var result = CompileInstruction("SET 3,A");
			Is(result, 0xCB, 0xDF);
		}


		[TestMethod]
		public void CB_SET_4_B()
		{
			var result = CompileInstruction("SET 4,B");
			Is(result, 0xCB, 0xE0);
		}


		[TestMethod]
		public void CB_SET_4_C()
		{
			var result = CompileInstruction("SET 4,C");
			Is(result, 0xCB, 0xE1);
		}


		[TestMethod]
		public void CB_SET_4_D()
		{
			var result = CompileInstruction("SET 4,D");
			Is(result, 0xCB, 0xE2);
		}


		[TestMethod]
		public void CB_SET_4_E()
		{
			var result = CompileInstruction("SET 4,E");
			Is(result, 0xCB, 0xE3);
		}


		[TestMethod]
		public void CB_SET_4_H()
		{
			var result = CompileInstruction("SET 4,H");
			Is(result, 0xCB, 0xE4);
		}


		[TestMethod]
		public void CB_SET_4_L()
		{
			var result = CompileInstruction("SET 4,L");
			Is(result, 0xCB, 0xE5);
		}


		[TestMethod]
		public void CB_SET_4_HL()
		{
			var result = CompileInstruction("SET 4,(HL)");
			Is(result, 0xCB, 0xE6);
		}


		[TestMethod]
		public void CB_SET_4_A()
		{
			var result = CompileInstruction("SET 4,A");
			Is(result, 0xCB, 0xE7);
		}


		[TestMethod]
		public void CB_SET_5_B()
		{
			var result = CompileInstruction("SET 5,B");
			Is(result, 0xCB, 0xE8);
		}


		[TestMethod]
		public void CB_SET_5_C()
		{
			var result = CompileInstruction("SET 5,C");
			Is(result, 0xCB, 0xE9);
		}


		[TestMethod]
		public void CB_SET_5_D()
		{
			var result = CompileInstruction("SET 5,D");
			Is(result, 0xCB, 0xEA);
		}


		[TestMethod]
		public void CB_SET_5_E()
		{
			var result = CompileInstruction("SET 5,E");
			Is(result, 0xCB, 0xEB);
		}


		[TestMethod]
		public void CB_SET_5_H()
		{
			var result = CompileInstruction("SET 5,H");
			Is(result, 0xCB, 0xEC);
		}


		[TestMethod]
		public void CB_SET_5_L()
		{
			var result = CompileInstruction("SET 5,L");
			Is(result, 0xCB, 0xED);
		}


		[TestMethod]
		public void CB_SET_5_HL()
		{
			var result = CompileInstruction("SET 5,(HL)");
			Is(result, 0xCB, 0xEE);
		}


		[TestMethod]
		public void CB_SET_5_A()
		{
			var result = CompileInstruction("SET 5,A");
			Is(result, 0xCB, 0xEF);
		}


		[TestMethod]
		public void CB_SET_6_B()
		{
			var result = CompileInstruction("SET 6,B");
			Is(result, 0xCB, 0xF0);
		}


		[TestMethod]
		public void CB_SET_6_C()
		{
			var result = CompileInstruction("SET 6,C");
			Is(result, 0xCB, 0xF1);
		}


		[TestMethod]
		public void CB_SET_6_D()
		{
			var result = CompileInstruction("SET 6,D");
			Is(result, 0xCB, 0xF2);
		}


		[TestMethod]
		public void CB_SET_6_E()
		{
			var result = CompileInstruction("SET 6,E");
			Is(result, 0xCB, 0xF3);
		}


		[TestMethod]
		public void CB_SET_6_H()
		{
			var result = CompileInstruction("SET 6,H");
			Is(result, 0xCB, 0xF4);
		}


		[TestMethod]
		public void CB_SET_6_L()
		{
			var result = CompileInstruction("SET 6,L");
			Is(result, 0xCB, 0xF5);
		}


		[TestMethod]
		public void CB_SET_6_HL()
		{
			var result = CompileInstruction("SET 6,(HL)");
			Is(result, 0xCB, 0xF6);
		}


		[TestMethod]
		public void CB_SET_6_A()
		{
			var result = CompileInstruction("SET 6,A");
			Is(result, 0xCB, 0xF7);
		}


		[TestMethod]
		public void CB_SET_7_B()
		{
			var result = CompileInstruction("SET 7,B");
			Is(result, 0xCB, 0xF8);
		}


		[TestMethod]
		public void CB_SET_7_C()
		{
			var result = CompileInstruction("SET 7,C");
			Is(result, 0xCB, 0xF9);
		}


		[TestMethod]
		public void CB_SET_7_D()
		{
			var result = CompileInstruction("SET 7,D");
			Is(result, 0xCB, 0xFA);
		}


		[TestMethod]
		public void CB_SET_7_E()
		{
			var result = CompileInstruction("SET 7,E");
			Is(result, 0xCB, 0xFB);
		}


		[TestMethod]
		public void CB_SET_7_H()
		{
			var result = CompileInstruction("SET 7,H");
			Is(result, 0xCB, 0xFC);
		}


		[TestMethod]
		public void CB_SET_7_L()
		{
			var result = CompileInstruction("SET 7,L");
			Is(result, 0xCB, 0xFD);
		}


		[TestMethod]
		public void CB_SET_7_HL()
		{
			var result = CompileInstruction("SET 7,(HL)");
			Is(result, 0xCB, 0xFE);
		}


	}
}
