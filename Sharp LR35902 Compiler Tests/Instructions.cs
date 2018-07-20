using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using static Sharp_LR35902_Compiler_Tests.Utils;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Instructions
	{


		[TestMethod]
		public void NOP()
		{
			var result = Assembler.CompileInstruction("NOP");
			Is(result, 0x00);
		}


		[TestMethod]
		public void LD_BC_nn()
		{
			var result = Assembler.CompileInstruction("LD BC,62689");
			Is(result, 0x01, 225, 244);
		}


		[TestMethod]
		public void LD_BC_A()
		{
			var result = Assembler.CompileInstruction("LD (BC),A");
			Is(result, 0x02);
		}


		[TestMethod]
		public void INC_BC()
		{
			var result = Assembler.CompileInstruction("INC BC");
			Is(result, 0x03);
		}


		[TestMethod]
		public void INC_B()
		{
			var result = Assembler.CompileInstruction("INC B");
			Is(result, 0x04);
		}


		[TestMethod]
		public void DEC_B()
		{
			var result = Assembler.CompileInstruction("DEC B");
			Is(result, 0x05);
		}


		[TestMethod]
		public void LD_B_n()
		{
			var result = Assembler.CompileInstruction("LD B,225");
			Is(result, 0x06, 225);
		}


		[TestMethod]
		public void RLC_A()
		{
			var result = Assembler.CompileInstruction("RLC A");
			Is(result, 0x07);
		}


		[TestMethod]
		public void LD_nn_SP()
		{
			var result = Assembler.CompileInstruction("LD (62689),SP");
			Is(result, 0x08, 225, 244);
		}


		[TestMethod]
		public void ADD_HL_BC()
		{
			var result = Assembler.CompileInstruction("ADD HL,BC");
			Is(result, 0x09);
		}


		[TestMethod]
		public void LD_A_BC()
		{
			var result = Assembler.CompileInstruction("LD A,(BC)");
			Is(result, 0x0A);
		}


		[TestMethod]
		public void DEC_BC()
		{
			var result = Assembler.CompileInstruction("DEC BC");
			Is(result, 0x0B);
		}


		[TestMethod]
		public void INC_C()
		{
			var result = Assembler.CompileInstruction("INC C");
			Is(result, 0x0C);
		}


		[TestMethod]
		public void DEC_C()
		{
			var result = Assembler.CompileInstruction("DEC C");
			Is(result, 0x0D);
		}


		[TestMethod]
		public void LD_C_n()
		{
			var result = Assembler.CompileInstruction("LD C,225");
			Is(result, 0x0E, 225);
		}


		[TestMethod]
		public void RRC_A()
		{
			var result = Assembler.CompileInstruction("RRC A");
			Is(result, 0x0F);
		}


		[TestMethod]
		public void STOP()
		{
			var result = Assembler.CompileInstruction("STOP");
			Is(result, 0x10);
		}


		[TestMethod]
		public void LD_DE_nn()
		{
			var result = Assembler.CompileInstruction("LD DE,62689");
			Is(result, 0x11, 225, 244);
		}


		[TestMethod]
		public void LD_DE_A()
		{
			var result = Assembler.CompileInstruction("LD (DE),A");
			Is(result, 0x12);
		}


		[TestMethod]
		public void INC_DE()
		{
			var result = Assembler.CompileInstruction("INC DE");
			Is(result, 0x13);
		}


		[TestMethod]
		public void INC_D()
		{
			var result = Assembler.CompileInstruction("INC D");
			Is(result, 0x14);
		}


		[TestMethod]
		public void DEC_D()
		{
			var result = Assembler.CompileInstruction("DEC D");
			Is(result, 0x15);
		}


		[TestMethod]
		public void LD_D_n()
		{
			var result = Assembler.CompileInstruction("LD D,225");
			Is(result, 0x16, 225);
		}


		[TestMethod]
		public void RL_A()
		{
			var result = Assembler.CompileInstruction("RL A");
			Is(result, 0x17);
		}


		[TestMethod]
		public void JR_n()
		{
			var result = Assembler.CompileInstruction("JR 225");
			Is(result, 0x18, 225);
		}


		[TestMethod]
		public void ADD_HL_DE()
		{
			var result = Assembler.CompileInstruction("ADD HL,DE");
			Is(result, 0x19);
		}


		[TestMethod]
		public void LD_A_DE()
		{
			var result = Assembler.CompileInstruction("LD A,(DE)");
			Is(result, 0x1A);
		}


		[TestMethod]
		public void DEC_DE()
		{
			var result = Assembler.CompileInstruction("DEC DE");
			Is(result, 0x1B);
		}


		[TestMethod]
		public void INC_E()
		{
			var result = Assembler.CompileInstruction("INC E");
			Is(result, 0x1C);
		}


		[TestMethod]
		public void DEC_E()
		{
			var result = Assembler.CompileInstruction("DEC E");
			Is(result, 0x1D);
		}


		[TestMethod]
		public void LD_E_n()
		{
			var result = Assembler.CompileInstruction("LD E,225");
			Is(result, 0x1E, 225);
		}


		[TestMethod]
		public void RR_A()
		{
			var result = Assembler.CompileInstruction("RR A");
			Is(result, 0x1F);
		}


		[TestMethod]
		public void JR_NZ_n()
		{
			var result = Assembler.CompileInstruction("JR NZ,225");
			Is(result, 0x20, 225);
		}


		[TestMethod]
		public void LD_HL_nn()
		{
			var result = Assembler.CompileInstruction("LD HL,62689");
			Is(result, 0x21, 225, 244);
		}


		[TestMethod]
		public void LDI_HL_A()
		{
			var result = Assembler.CompileInstruction("LDI (HL),A");
			Is(result, 0x22);
		}


		[TestMethod]
		public void INC_HL()
		{
			var result = Assembler.CompileInstruction("INC HL");
			Is(result, 0x23);
		}


		[TestMethod]
		public void INC_H()
		{
			var result = Assembler.CompileInstruction("INC H");
			Is(result, 0x24);
		}


		[TestMethod]
		public void DEC_H()
		{
			var result = Assembler.CompileInstruction("DEC H");
			Is(result, 0x25);
		}


		[TestMethod]
		public void LD_H_n()
		{
			var result = Assembler.CompileInstruction("LD H,225");
			Is(result, 0x26, 225);
		}


		[TestMethod]
		public void DAA()
		{
			var result = Assembler.CompileInstruction("DAA");
			Is(result, 0x27);
		}


		[TestMethod]
		public void JR_Z_n()
		{
			var result = Assembler.CompileInstruction("JR Z,225");
			Is(result, 0x28, 225);
		}


		[TestMethod]
		public void ADD_HL_HL()
		{
			var result = Assembler.CompileInstruction("ADD HL,HL");
			Is(result, 0x29);
		}


		[TestMethod]
		public void LDI_A_HL()
		{
			var result = Assembler.CompileInstruction("LDI A,(HL)");
			Is(result, 0x2A);
		}


		[TestMethod]
		public void DEC_HL()
		{
			var result = Assembler.CompileInstruction("DEC HL");
			Is(result, 0x2B);
		}


		[TestMethod]
		public void INC_L()
		{
			var result = Assembler.CompileInstruction("INC L");
			Is(result, 0x2C);
		}


		[TestMethod]
		public void DEC_L()
		{
			var result = Assembler.CompileInstruction("DEC L");
			Is(result, 0x2D);
		}


		[TestMethod]
		public void LD_L_n()
		{
			var result = Assembler.CompileInstruction("LD L,225");
			Is(result, 0x2E, 225);
		}


		[TestMethod]
		public void CPL()
		{
			var result = Assembler.CompileInstruction("CPL");
			Is(result, 0x2F);
		}


		[TestMethod]
		public void JR_NC_n()
		{
			var result = Assembler.CompileInstruction("JR NC,225");
			Is(result, 0x30, 225);
		}


		[TestMethod]
		public void LD_SP_nn()
		{
			var result = Assembler.CompileInstruction("LD SP,62689");
			Is(result, 0x31, 225, 244);
		}


		[TestMethod]
		public void LDD_HL_A()
		{
			var result = Assembler.CompileInstruction("LDD (HL),A");
			Is(result, 0x32);
		}


		[TestMethod]
		public void INC_SP()
		{
			var result = Assembler.CompileInstruction("INC SP");
			Is(result, 0x33);
		}


		[TestMethod]
		public void INC_At_HL()
		{
			var result = Assembler.CompileInstruction("INC (HL)");
			Is(result, 0x34);
		}


		[TestMethod]
		public void DEC_At_HL()
		{
			var result = Assembler.CompileInstruction("DEC (HL)");
			Is(result, 0x35);
		}


		[TestMethod]
		public void LD_HL_n()
		{
			var result = Assembler.CompileInstruction("LD (HL),225");
			Is(result, 0x36, 225);
		}


		[TestMethod]
		public void SCF()
		{
			var result = Assembler.CompileInstruction("SCF");
			Is(result, 0x37);
		}


		[TestMethod]
		public void JR_C_n()
		{
			var result = Assembler.CompileInstruction("JR C,225");
			Is(result, 0x38, 225);
		}


		[TestMethod]
		public void ADD_HL_SP()
		{
			var result = Assembler.CompileInstruction("ADD HL,SP");
			Is(result, 0x39);
		}


		[TestMethod]
		public void LDD_A_HL()
		{
			var result = Assembler.CompileInstruction("LDD A,(HL)");
			Is(result, 0x3A);
		}


		[TestMethod]
		public void DEC_SP()
		{
			var result = Assembler.CompileInstruction("DEC SP");
			Is(result, 0x3B);
		}


		[TestMethod]
		public void INC_A()
		{
			var result = Assembler.CompileInstruction("INC A");
			Is(result, 0x3C);
		}


		[TestMethod]
		public void DEC_A()
		{
			var result = Assembler.CompileInstruction("DEC A");
			Is(result, 0x3D);
		}


		[TestMethod]
		public void LD_A_n()
		{
			var result = Assembler.CompileInstruction("LD A,225");
			Is(result, 0x3E, 225);
		}


		[TestMethod]
		public void CCF()
		{
			var result = Assembler.CompileInstruction("CCF");
			Is(result, 0x3F);
		}


		[TestMethod]
		public void LD_B_B()
		{
			var result = Assembler.CompileInstruction("LD B,B");
			Is(result, 0x40);
		}


		[TestMethod]
		public void LD_B_C()
		{
			var result = Assembler.CompileInstruction("LD B,C");
			Is(result, 0x41);
		}


		[TestMethod]
		public void LD_B_D()
		{
			var result = Assembler.CompileInstruction("LD B,D");
			Is(result, 0x42);
		}


		[TestMethod]
		public void LD_B_E()
		{
			var result = Assembler.CompileInstruction("LD B,E");
			Is(result, 0x43);
		}


		[TestMethod]
		public void LD_B_H()
		{
			var result = Assembler.CompileInstruction("LD B,H");
			Is(result, 0x44);
		}


		[TestMethod]
		public void LD_B_L()
		{
			var result = Assembler.CompileInstruction("LD B,L");
			Is(result, 0x45);
		}


		[TestMethod]
		public void LD_B_HL()
		{
			var result = Assembler.CompileInstruction("LD B,(HL)");
			Is(result, 0x46);
		}


		[TestMethod]
		public void LD_B_A()
		{
			var result = Assembler.CompileInstruction("LD B,A");
			Is(result, 0x47);
		}


		[TestMethod]
		public void LD_C_B()
		{
			var result = Assembler.CompileInstruction("LD C,B");
			Is(result, 0x48);
		}


		[TestMethod]
		public void LD_C_C()
		{
			var result = Assembler.CompileInstruction("LD C,C");
			Is(result, 0x49);
		}


		[TestMethod]
		public void LD_C_D()
		{
			var result = Assembler.CompileInstruction("LD C,D");
			Is(result, 0x4A);
		}


		[TestMethod]
		public void LD_C_E()
		{
			var result = Assembler.CompileInstruction("LD C,E");
			Is(result, 0x4B);
		}


		[TestMethod]
		public void LD_C_H()
		{
			var result = Assembler.CompileInstruction("LD C,H");
			Is(result, 0x4C);
		}


		[TestMethod]
		public void LD_C_L()
		{
			var result = Assembler.CompileInstruction("LD C,L");
			Is(result, 0x4D);
		}


		[TestMethod]
		public void LD_C_HL()
		{
			var result = Assembler.CompileInstruction("LD C,(HL)");
			Is(result, 0x4E);
		}


		[TestMethod]
		public void LD_C_A()
		{
			var result = Assembler.CompileInstruction("LD C,A");
			Is(result, 0x4F);
		}


		[TestMethod]
		public void LD_D_B()
		{
			var result = Assembler.CompileInstruction("LD D,B");
			Is(result, 0x50);
		}


		[TestMethod]
		public void LD_D_C()
		{
			var result = Assembler.CompileInstruction("LD D,C");
			Is(result, 0x51);
		}


		[TestMethod]
		public void LD_D_D()
		{
			var result = Assembler.CompileInstruction("LD D,D");
			Is(result, 0x52);
		}


		[TestMethod]
		public void LD_D_E()
		{
			var result = Assembler.CompileInstruction("LD D,E");
			Is(result, 0x53);
		}


		[TestMethod]
		public void LD_D_H()
		{
			var result = Assembler.CompileInstruction("LD D,H");
			Is(result, 0x54);
		}


		[TestMethod]
		public void LD_D_L()
		{
			var result = Assembler.CompileInstruction("LD D,L");
			Is(result, 0x55);
		}


		[TestMethod]
		public void LD_D_HL()
		{
			var result = Assembler.CompileInstruction("LD D,(HL)");
			Is(result, 0x56);
		}


		[TestMethod]
		public void LD_D_A()
		{
			var result = Assembler.CompileInstruction("LD D,A");
			Is(result, 0x57);
		}


		[TestMethod]
		public void LD_E_B()
		{
			var result = Assembler.CompileInstruction("LD E,B");
			Is(result, 0x58);
		}


		[TestMethod]
		public void LD_E_C()
		{
			var result = Assembler.CompileInstruction("LD E,C");
			Is(result, 0x59);
		}


		[TestMethod]
		public void LD_E_D()
		{
			var result = Assembler.CompileInstruction("LD E,D");
			Is(result, 0x5A);
		}


		[TestMethod]
		public void LD_E_E()
		{
			var result = Assembler.CompileInstruction("LD E,E");
			Is(result, 0x5B);
		}


		[TestMethod]
		public void LD_E_H()
		{
			var result = Assembler.CompileInstruction("LD E,H");
			Is(result, 0x5C);
		}


		[TestMethod]
		public void LD_E_L()
		{
			var result = Assembler.CompileInstruction("LD E,L");
			Is(result, 0x5D);
		}


		[TestMethod]
		public void LD_E_HL()
		{
			var result = Assembler.CompileInstruction("LD E,(HL)");
			Is(result, 0x5E);
		}


		[TestMethod]
		public void LD_E_A()
		{
			var result = Assembler.CompileInstruction("LD E,A");
			Is(result, 0x5F);
		}


		[TestMethod]
		public void LD_H_B()
		{
			var result = Assembler.CompileInstruction("LD H,B");
			Is(result, 0x60);
		}


		[TestMethod]
		public void LD_H_C()
		{
			var result = Assembler.CompileInstruction("LD H,C");
			Is(result, 0x61);
		}


		[TestMethod]
		public void LD_H_D()
		{
			var result = Assembler.CompileInstruction("LD H,D");
			Is(result, 0x62);
		}


		[TestMethod]
		public void LD_H_E()
		{
			var result = Assembler.CompileInstruction("LD H,E");
			Is(result, 0x63);
		}


		[TestMethod]
		public void LD_H_H()
		{
			var result = Assembler.CompileInstruction("LD H,H");
			Is(result, 0x64);
		}


		[TestMethod]
		public void LD_H_L()
		{
			var result = Assembler.CompileInstruction("LD H,L");
			Is(result, 0x65);
		}


		[TestMethod]
		public void LD_H_HL()
		{
			var result = Assembler.CompileInstruction("LD H,(HL)");
			Is(result, 0x66);
		}


		[TestMethod]
		public void LD_H_A()
		{
			var result = Assembler.CompileInstruction("LD H,A");
			Is(result, 0x67);
		}


		[TestMethod]
		public void LD_L_B()
		{
			var result = Assembler.CompileInstruction("LD L,B");
			Is(result, 0x68);
		}


		[TestMethod]
		public void LD_L_C()
		{
			var result = Assembler.CompileInstruction("LD L,C");
			Is(result, 0x69);
		}


		[TestMethod]
		public void LD_L_D()
		{
			var result = Assembler.CompileInstruction("LD L,D");
			Is(result, 0x6A);
		}


		[TestMethod]
		public void LD_L_E()
		{
			var result = Assembler.CompileInstruction("LD L,E");
			Is(result, 0x6B);
		}


		[TestMethod]
		public void LD_L_H()
		{
			var result = Assembler.CompileInstruction("LD L,H");
			Is(result, 0x6C);
		}


		[TestMethod]
		public void LD_L_L()
		{
			var result = Assembler.CompileInstruction("LD L,L");
			Is(result, 0x6D);
		}


		[TestMethod]
		public void LD_L_HL()
		{
			var result = Assembler.CompileInstruction("LD L,(HL)");
			Is(result, 0x6E);
		}


		[TestMethod]
		public void LD_L_A()
		{
			var result = Assembler.CompileInstruction("LD L,A");
			Is(result, 0x6F);
		}


		[TestMethod]
		public void LD_HL_B()
		{
			var result = Assembler.CompileInstruction("LD (HL),B");
			Is(result, 0x70);
		}


		[TestMethod]
		public void LD_HL_C()
		{
			var result = Assembler.CompileInstruction("LD (HL),C");
			Is(result, 0x71);
		}


		[TestMethod]
		public void LD_HL_D()
		{
			var result = Assembler.CompileInstruction("LD (HL),D");
			Is(result, 0x72);
		}


		[TestMethod]
		public void LD_HL_E()
		{
			var result = Assembler.CompileInstruction("LD (HL),E");
			Is(result, 0x73);
		}


		[TestMethod]
		public void LD_HL_H()
		{
			var result = Assembler.CompileInstruction("LD (HL),H");
			Is(result, 0x74);
		}


		[TestMethod]
		public void LD_HL_L()
		{
			var result = Assembler.CompileInstruction("LD (HL),L");
			Is(result, 0x75);
		}


		[TestMethod]
		public void HALT()
		{
			var result = Assembler.CompileInstruction("HALT");
			Is(result, 0x76);
		}


		[TestMethod]
		public void LD_HL_A()
		{
			var result = Assembler.CompileInstruction("LD (HL),A");
			Is(result, 0x77);
		}


		[TestMethod]
		public void LD_A_B()
		{
			var result = Assembler.CompileInstruction("LD A,B");
			Is(result, 0x78);
		}


		[TestMethod]
		public void LD_A_C()
		{
			var result = Assembler.CompileInstruction("LD A,C");
			Is(result, 0x79);
		}


		[TestMethod]
		public void LD_A_D()
		{
			var result = Assembler.CompileInstruction("LD A,D");
			Is(result, 0x7A);
		}


		[TestMethod]
		public void LD_A_E()
		{
			var result = Assembler.CompileInstruction("LD A,E");
			Is(result, 0x7B);
		}


		[TestMethod]
		public void LD_A_H()
		{
			var result = Assembler.CompileInstruction("LD A,H");
			Is(result, 0x7C);
		}


		[TestMethod]
		public void LD_A_L()
		{
			var result = Assembler.CompileInstruction("LD A,L");
			Is(result, 0x7D);
		}


		[TestMethod]
		public void LD_A_HL()
		{
			var result = Assembler.CompileInstruction("LD A,(HL)");
			Is(result, 0x7E);
		}


		[TestMethod]
		public void LD_A_A()
		{
			var result = Assembler.CompileInstruction("LD A,A");
			Is(result, 0x7F);
		}


		[TestMethod]
		public void ADD_A_B()
		{
			var result = Assembler.CompileInstruction("ADD A,B");
			Is(result, 0x80);
		}


		[TestMethod]
		public void ADD_A_C()
		{
			var result = Assembler.CompileInstruction("ADD A,C");
			Is(result, 0x81);
		}


		[TestMethod]
		public void ADD_A_D()
		{
			var result = Assembler.CompileInstruction("ADD A,D");
			Is(result, 0x82);
		}


		[TestMethod]
		public void ADD_A_E()
		{
			var result = Assembler.CompileInstruction("ADD A,E");
			Is(result, 0x83);
		}


		[TestMethod]
		public void ADD_A_H()
		{
			var result = Assembler.CompileInstruction("ADD A,H");
			Is(result, 0x84);
		}


		[TestMethod]
		public void ADD_A_L()
		{
			var result = Assembler.CompileInstruction("ADD A,L");
			Is(result, 0x85);
		}


		[TestMethod]
		public void ADD_A_HL()
		{
			var result = Assembler.CompileInstruction("ADD A,(HL)");
			Is(result, 0x86);
		}


		[TestMethod]
		public void ADD_A_A()
		{
			var result = Assembler.CompileInstruction("ADD A,A");
			Is(result, 0x87);
		}


		[TestMethod]
		public void ADC_A_B()
		{
			var result = Assembler.CompileInstruction("ADC A,B");
			Is(result, 0x88);
		}


		[TestMethod]
		public void ADC_A_C()
		{
			var result = Assembler.CompileInstruction("ADC A,C");
			Is(result, 0x89);
		}


		[TestMethod]
		public void ADC_A_D()
		{
			var result = Assembler.CompileInstruction("ADC A,D");
			Is(result, 0x8A);
		}


		[TestMethod]
		public void ADC_A_E()
		{
			var result = Assembler.CompileInstruction("ADC A,E");
			Is(result, 0x8B);
		}


		[TestMethod]
		public void ADC_A_H()
		{
			var result = Assembler.CompileInstruction("ADC A,H");
			Is(result, 0x8C);
		}


		[TestMethod]
		public void ADC_A_L()
		{
			var result = Assembler.CompileInstruction("ADC A,L");
			Is(result, 0x8D);
		}


		[TestMethod]
		public void ADC_A_HL()
		{
			var result = Assembler.CompileInstruction("ADC A,(HL)");
			Is(result, 0x8E);
		}


		[TestMethod]
		public void ADC_A_A()
		{
			var result = Assembler.CompileInstruction("ADC A,A");
			Is(result, 0x8F);
		}


		[TestMethod]
		public void SUB_A_B()
		{
			var result = Assembler.CompileInstruction("SUB A,B");
			Is(result, 0x90);
		}


		[TestMethod]
		public void SUB_A_C()
		{
			var result = Assembler.CompileInstruction("SUB A,C");
			Is(result, 0x91);
		}


		[TestMethod]
		public void SUB_A_D()
		{
			var result = Assembler.CompileInstruction("SUB A,D");
			Is(result, 0x92);
		}


		[TestMethod]
		public void SUB_A_E()
		{
			var result = Assembler.CompileInstruction("SUB A,E");
			Is(result, 0x93);
		}


		[TestMethod]
		public void SUB_A_H()
		{
			var result = Assembler.CompileInstruction("SUB A,H");
			Is(result, 0x94);
		}


		[TestMethod]
		public void SUB_A_L()
		{
			var result = Assembler.CompileInstruction("SUB A,L");
			Is(result, 0x95);
		}


		[TestMethod]
		public void SUB_A_HL()
		{
			var result = Assembler.CompileInstruction("SUB A,(HL)");
			Is(result, 0x96);
		}


		[TestMethod]
		public void SUB_A_A()
		{
			var result = Assembler.CompileInstruction("SUB A,A");
			Is(result, 0x97);
		}


		[TestMethod]
		public void SBC_A_B()
		{
			var result = Assembler.CompileInstruction("SBC A,B");
			Is(result, 0x98);
		}


		[TestMethod]
		public void SBC_A_C()
		{
			var result = Assembler.CompileInstruction("SBC A,C");
			Is(result, 0x99);
		}


		[TestMethod]
		public void SBC_A_D()
		{
			var result = Assembler.CompileInstruction("SBC A,D");
			Is(result, 0x9A);
		}


		[TestMethod]
		public void SBC_A_E()
		{
			var result = Assembler.CompileInstruction("SBC A,E");
			Is(result, 0x9B);
		}


		[TestMethod]
		public void SBC_A_H()
		{
			var result = Assembler.CompileInstruction("SBC A,H");
			Is(result, 0x9C);
		}


		[TestMethod]
		public void SBC_A_L()
		{
			var result = Assembler.CompileInstruction("SBC A,L");
			Is(result, 0x9D);
		}


		[TestMethod]
		public void SBC_A_HL()
		{
			var result = Assembler.CompileInstruction("SBC A,(HL)");
			Is(result, 0x9E);
		}


		[TestMethod]
		public void SBC_A_A()
		{
			var result = Assembler.CompileInstruction("SBC A,A");
			Is(result, 0x9F);
		}


		[TestMethod]
		public void AND_B()
		{
			var result = Assembler.CompileInstruction("AND B");
			Is(result, 0xA0);
		}


		[TestMethod]
		public void AND_C()
		{
			var result = Assembler.CompileInstruction("AND C");
			Is(result, 0xA1);
		}


		[TestMethod]
		public void AND_D()
		{
			var result = Assembler.CompileInstruction("AND D");
			Is(result, 0xA2);
		}


		[TestMethod]
		public void AND_E()
		{
			var result = Assembler.CompileInstruction("AND E");
			Is(result, 0xA3);
		}


		[TestMethod]
		public void AND_H()
		{
			var result = Assembler.CompileInstruction("AND H");
			Is(result, 0xA4);
		}


		[TestMethod]
		public void AND_L()
		{
			var result = Assembler.CompileInstruction("AND L");
			Is(result, 0xA5);
		}


		[TestMethod]
		public void AND_HL()
		{
			var result = Assembler.CompileInstruction("AND (HL)");
			Is(result, 0xA6);
		}


		[TestMethod]
		public void AND_A()
		{
			var result = Assembler.CompileInstruction("AND A");
			Is(result, 0xA7);
		}


		[TestMethod]
		public void XOR_B()
		{
			var result = Assembler.CompileInstruction("XOR B");
			Is(result, 0xA8);
		}


		[TestMethod]
		public void XOR_C()
		{
			var result = Assembler.CompileInstruction("XOR C");
			Is(result, 0xA9);
		}


		[TestMethod]
		public void XOR_D()
		{
			var result = Assembler.CompileInstruction("XOR D");
			Is(result, 0xAA);
		}


		[TestMethod]
		public void XOR_E()
		{
			var result = Assembler.CompileInstruction("XOR E");
			Is(result, 0xAB);
		}


		[TestMethod]
		public void XOR_H()
		{
			var result = Assembler.CompileInstruction("XOR H");
			Is(result, 0xAC);
		}


		[TestMethod]
		public void XOR_L()
		{
			var result = Assembler.CompileInstruction("XOR L");
			Is(result, 0xAD);
		}


		[TestMethod]
		public void XOR_HL()
		{
			var result = Assembler.CompileInstruction("XOR (HL)");
			Is(result, 0xAE);
		}


		[TestMethod]
		public void XOR_A()
		{
			var result = Assembler.CompileInstruction("XOR A");
			Is(result, 0xAF);
		}


		[TestMethod]
		public void OR_B()
		{
			var result = Assembler.CompileInstruction("OR B");
			Is(result, 0xB0);
		}


		[TestMethod]
		public void OR_C()
		{
			var result = Assembler.CompileInstruction("OR C");
			Is(result, 0xB1);
		}


		[TestMethod]
		public void OR_D()
		{
			var result = Assembler.CompileInstruction("OR D");
			Is(result, 0xB2);
		}


		[TestMethod]
		public void OR_E()
		{
			var result = Assembler.CompileInstruction("OR E");
			Is(result, 0xB3);
		}


		[TestMethod]
		public void OR_H()
		{
			var result = Assembler.CompileInstruction("OR H");
			Is(result, 0xB4);
		}


		[TestMethod]
		public void OR_L()
		{
			var result = Assembler.CompileInstruction("OR L");
			Is(result, 0xB5);
		}


		[TestMethod]
		public void OR_HL()
		{
			var result = Assembler.CompileInstruction("OR (HL)");
			Is(result, 0xB6);
		}


		[TestMethod]
		public void OR_A()
		{
			var result = Assembler.CompileInstruction("OR A");
			Is(result, 0xB7);
		}


		[TestMethod]
		public void CP_B()
		{
			var result = Assembler.CompileInstruction("CP B");
			Is(result, 0xB8);
		}


		[TestMethod]
		public void CP_C()
		{
			var result = Assembler.CompileInstruction("CP C");
			Is(result, 0xB9);
		}


		[TestMethod]
		public void CP_D()
		{
			var result = Assembler.CompileInstruction("CP D");
			Is(result, 0xBA);
		}


		[TestMethod]
		public void CP_E()
		{
			var result = Assembler.CompileInstruction("CP E");
			Is(result, 0xBB);
		}


		[TestMethod]
		public void CP_H()
		{
			var result = Assembler.CompileInstruction("CP H");
			Is(result, 0xBC);
		}


		[TestMethod]
		public void CP_L()
		{
			var result = Assembler.CompileInstruction("CP L");
			Is(result, 0xBD);
		}


		[TestMethod]
		public void CP_HL()
		{
			var result = Assembler.CompileInstruction("CP (HL)");
			Is(result, 0xBE);
		}


		[TestMethod]
		public void CP_A()
		{
			var result = Assembler.CompileInstruction("CP A");
			Is(result, 0xBF);
		}


		[TestMethod]
		public void RET_NZ()
		{
			var result = Assembler.CompileInstruction("RET NZ");
			Is(result, 0xC0);
		}


		[TestMethod]
		public void POP_BC()
		{
			var result = Assembler.CompileInstruction("POP BC");
			Is(result, 0xC1);
		}


		[TestMethod]
		public void JP_NZ_nn()
		{
			var result = Assembler.CompileInstruction("JP NZ,62689");
			Is(result, 0xC2, 225, 244);
		}


		[TestMethod]
		public void JP_nn()
		{
			var result = Assembler.CompileInstruction("JP 62689");
			Is(result, 0xC3, 225, 244);
		}


		[TestMethod]
		public void CALL_NZ_nn()
		{
			var result = Assembler.CompileInstruction("CALL NZ,62689");
			Is(result, 0xC4, 225, 244);
		}


		[TestMethod]
		public void PUSH_BC()
		{
			var result = Assembler.CompileInstruction("PUSH BC");
			Is(result, 0xC5);
		}


		[TestMethod]
		public void ADD_A_n()
		{
			var result = Assembler.CompileInstruction("ADD A,225");
			Is(result, 0xC6, 225);
		}


		[TestMethod]
		public void RST_0()
		{
			var result = Assembler.CompileInstruction("RST 0");
			Is(result, 0xC7);
		}


		[TestMethod]
		public void RET_Z()
		{
			var result = Assembler.CompileInstruction("RET Z");
			Is(result, 0xC8);
		}


		[TestMethod]
		public void RET()
		{
			var result = Assembler.CompileInstruction("RET");
			Is(result, 0xC9);
		}


		[TestMethod]
		public void JP_Z_nn()
		{
			var result = Assembler.CompileInstruction("JP Z,62689");
			Is(result, 0xCA, 225, 244);
		}


		[TestMethod]
		public void Ext_ops()
		{
			var result = Assembler.CompileInstruction("Ext ops");
			Is(result, 0xCB);
		}


		[TestMethod]
		public void CALL_Z_nn()
		{
			var result = Assembler.CompileInstruction("CALL Z,62689");
			Is(result, 0xCC, 225, 244);
		}


		[TestMethod]
		public void CALL_nn()
		{
			var result = Assembler.CompileInstruction("CALL 62689");
			Is(result, 0xCD, 225, 244);
		}


		[TestMethod]
		public void ADC_A_n()
		{
			var result = Assembler.CompileInstruction("ADC A,225");
			Is(result, 0xCE, 225);
		}


		[TestMethod]
		public void RST_8()
		{
			var result = Assembler.CompileInstruction("RST 8");
			Is(result, 0xCF);
		}


		[TestMethod]
		public void RET_NC()
		{
			var result = Assembler.CompileInstruction("RET NC");
			Is(result, 0xD0);
		}


		[TestMethod]
		public void POP_DE()
		{
			var result = Assembler.CompileInstruction("POP DE");
			Is(result, 0xD1);
		}


		[TestMethod]
		public void JP_NC_nn()
		{
			var result = Assembler.CompileInstruction("JP NC,62689");
			Is(result, 0xD2, 225, 244);
		}


		[TestMethod]
		public void CALL_NC_nn()
		{
			var result = Assembler.CompileInstruction("CALL NC,62689");
			Is(result, 0xD4, 225, 244);
		}


		[TestMethod]
		public void PUSH_DE()
		{
			var result = Assembler.CompileInstruction("PUSH DE");
			Is(result, 0xD5);
		}


		[TestMethod]
		public void SUB_A_n()
		{
			var result = Assembler.CompileInstruction("SUB A,225");
			Is(result, 0xD6, 225);
		}


		[TestMethod]
		public void RST_10()
		{
			var result = Assembler.CompileInstruction("RST 10");
			Is(result, 0xD7);
		}


		[TestMethod]
		public void RET_C()
		{
			var result = Assembler.CompileInstruction("RET C");
			Is(result, 0xD8);
		}


		[TestMethod]
		public void RETI()
		{
			var result = Assembler.CompileInstruction("RETI");
			Is(result, 0xD9);
		}


		[TestMethod]
		public void JP_C_nn()
		{
			var result = Assembler.CompileInstruction("JP C,62689");
			Is(result, 0xDA, 225, 244);
		}


		[TestMethod]
		public void CALL_C_nn()
		{
			var result = Assembler.CompileInstruction("CALL C,62689");
			Is(result, 0xDC, 225, 244);
		}


		[TestMethod]
		public void SBC_A_n()
		{
			var result = Assembler.CompileInstruction("SBC A,225");
			Is(result, 0xDE, 225);
		}


		[TestMethod]
		public void RST_18()
		{
			var result = Assembler.CompileInstruction("RST 18");
			Is(result, 0xDF);
		}


		[TestMethod]
		public void LDH_n_A()
		{
			var result = Assembler.CompileInstruction("LDH (225),A");
			Is(result, 0xE0, 225);
		}


		[TestMethod]
		public void POP_HL()
		{
			var result = Assembler.CompileInstruction("POP HL");
			Is(result, 0xE1);
		}


		[TestMethod]
		public void LDH_C_A()
		{
			var result = Assembler.CompileInstruction("LDH (C),A");
			Is(result, 0xE2);
		}


		[TestMethod]
		public void PUSH_HL()
		{
			var result = Assembler.CompileInstruction("PUSH HL");
			Is(result, 0xE5);
		}


		[TestMethod]
		public void AND_n()
		{
			var result = Assembler.CompileInstruction("AND 225");
			Is(result, 0xE6, 225);
		}


		[TestMethod]
		public void RST_20()
		{
			var result = Assembler.CompileInstruction("RST 20");
			Is(result, 0xE7);
		}


		[TestMethod]
		public void ADD_SP_n()
		{
			var result = Assembler.CompileInstruction("ADD SP, 225");
			Is(result, 0xE8, 225);
		}


		[TestMethod]
		public void JP_HL()
		{
			var result = Assembler.CompileInstruction("JP (HL)");
			Is(result, 0xE9);
		}


		[TestMethod]
		public void LD_nn_A()
		{
			var result = Assembler.CompileInstruction("LD (62689),A");
			Is(result, 0xEA, 225, 244);
		}


		[TestMethod]
		public void XOR_n()
		{
			var result = Assembler.CompileInstruction("XOR 225");
			Is(result, 0xEE, 225);
		}


		[TestMethod]
		public void RST_28()
		{
			var result = Assembler.CompileInstruction("RST 28");
			Is(result, 0xEF);
		}


		[TestMethod]
		public void LDH_A_n()
		{
			var result = Assembler.CompileInstruction("LDH A,(225)");
			Is(result, 0xF0, 225);
		}


		[TestMethod]
		public void POP_AF()
		{
			var result = Assembler.CompileInstruction("POP AF");
			Is(result, 0xF1);
		}


		[TestMethod]
		public void DI()
		{
			var result = Assembler.CompileInstruction("DI");
			Is(result, 0xF3);
		}


		[TestMethod]
		public void PUSH_AF()
		{
			var result = Assembler.CompileInstruction("PUSH AF");
			Is(result, 0xF5);
		}


		[TestMethod]
		public void OR_n()
		{
			var result = Assembler.CompileInstruction("OR 225");
			Is(result, 0xF6, 225);
		}


		[TestMethod]
		public void RST_30()
		{
			var result = Assembler.CompileInstruction("RST 30");
			Is(result, 0xF7);
		}


		[TestMethod]
		public void LDHL_SP_d()
		{
			var result = Assembler.CompileInstruction("LDHL SP,d");
			Is(result, 0xF8);
		}


		[TestMethod]
		public void LD_SP_HL()
		{
			var result = Assembler.CompileInstruction("LD SP,HL");
			Is(result, 0xF9);
		}


		[TestMethod]
		public void LD_A_nn()
		{
			var result = Assembler.CompileInstruction("LD A,(62689)");
			Is(result, 0xFA, 225, 244);
		}


		[TestMethod]
		public void EI()
		{
			var result = Assembler.CompileInstruction("EI");
			Is(result, 0xFB);
		}


		[TestMethod]
		public void CP_n()
		{
			var result = Assembler.CompileInstruction("CP 225");
			Is(result, 0xFE, 225);
		}
	}
}
