using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000EFD RID: 3837
	public class JsonWriter
	{
		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06005DDB RID: 24027 RVA: 0x001DFA58 File Offset: 0x001DDC58
		// (set) Token: 0x06005DDC RID: 24028 RVA: 0x001DFA60 File Offset: 0x001DDC60
		public int IndentValue
		{
			get
			{
				return this.indent_value;
			}
			set
			{
				this.indentation = this.indentation / this.indent_value * value;
				this.indent_value = value;
			}
		}

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06005DDD RID: 24029 RVA: 0x001DFA7E File Offset: 0x001DDC7E
		// (set) Token: 0x06005DDE RID: 24030 RVA: 0x001DFA86 File Offset: 0x001DDC86
		public bool PrettyPrint
		{
			get
			{
				return this.pretty_print;
			}
			set
			{
				this.pretty_print = value;
			}
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06005DDF RID: 24031 RVA: 0x001DFA8F File Offset: 0x001DDC8F
		public TextWriter TextWriter
		{
			get
			{
				return this.writer;
			}
		}

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06005DE0 RID: 24032 RVA: 0x001DFA97 File Offset: 0x001DDC97
		// (set) Token: 0x06005DE1 RID: 24033 RVA: 0x001DFA9F File Offset: 0x001DDC9F
		public bool Validate
		{
			get
			{
				return this.validate;
			}
			set
			{
				this.validate = value;
			}
		}

		// Token: 0x06005DE3 RID: 24035 RVA: 0x001DFAB4 File Offset: 0x001DDCB4
		public JsonWriter()
		{
			this.inst_string_builder = new StringBuilder();
			this.writer = new StringWriter(this.inst_string_builder);
			this.Init();
		}

		// Token: 0x06005DE4 RID: 24036 RVA: 0x001DFADE File Offset: 0x001DDCDE
		public JsonWriter(StringBuilder sb)
			: this(new StringWriter(sb))
		{
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x001DFAEC File Offset: 0x001DDCEC
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.Init();
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x001DFB10 File Offset: 0x001DDD10
		private void DoValidation(Condition cond)
		{
			if (!this.context.ExpectingValue)
			{
				this.context.Count++;
			}
			if (!this.validate)
			{
				return;
			}
			if (this.has_reached_end)
			{
				throw new JsonException("A complete JSON symbol has already been written");
			}
			switch (cond)
			{
			case Condition.InArray:
				if (!this.context.InArray)
				{
					throw new JsonException("Can't close an array here");
				}
				break;
			case Condition.InObject:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't close an object here");
				}
				break;
			case Condition.NotAProperty:
				if (this.context.InObject && !this.context.ExpectingValue)
				{
					throw new JsonException("Expected a property");
				}
				break;
			case Condition.Property:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't add a property here");
				}
				break;
			case Condition.Value:
				if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
				{
					throw new JsonException("Can't add a value here");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001DFC34 File Offset: 0x001DDE34
		private void Init()
		{
			this.has_reached_end = false;
			this.hex_seq = new char[4];
			this.indentation = 0;
			this.indent_value = 4;
			this.pretty_print = false;
			this.validate = true;
			this.ctx_stack = new Stack<WriterContext>();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x001DFC98 File Offset: 0x001DDE98
		private static void IntToHex(int n, char[] hex)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = n % 16;
				if (num < 10)
				{
					hex[3 - i] = (char)(48 + num);
				}
				else
				{
					hex[3 - i] = (char)(65 + (num - 10));
				}
				n >>= 4;
			}
		}

		// Token: 0x06005DE9 RID: 24041 RVA: 0x001DFCD9 File Offset: 0x001DDED9
		private void Indent()
		{
			if (this.pretty_print)
			{
				this.indentation += this.indent_value;
			}
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x001DFCF8 File Offset: 0x001DDEF8
		private void Put(string str)
		{
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				for (int i = 0; i < this.indentation; i++)
				{
					this.writer.Write(' ');
				}
			}
			this.writer.Write(str);
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x001DFD44 File Offset: 0x001DDF44
		private void PutNewline()
		{
			this.PutNewline(true);
		}

		// Token: 0x06005DEC RID: 24044 RVA: 0x001DFD50 File Offset: 0x001DDF50
		private void PutNewline(bool add_comma)
		{
			if (add_comma && !this.context.ExpectingValue && this.context.Count > 1)
			{
				this.writer.Write(',');
			}
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				this.writer.Write('\n');
			}
		}

		// Token: 0x06005DED RID: 24045 RVA: 0x001DFDAC File Offset: 0x001DDFAC
		private void PutString(string str)
		{
			this.Put(string.Empty);
			this.writer.Write('"');
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str[i];
				switch (c)
				{
				case '\b':
					this.writer.Write("\\b");
					break;
				case '\t':
					this.writer.Write("\\t");
					break;
				case '\n':
					this.writer.Write("\\n");
					break;
				case '\v':
					goto IL_00E4;
				case '\f':
					this.writer.Write("\\f");
					break;
				case '\r':
					this.writer.Write("\\r");
					break;
				default:
					if (c != '"' && c != '\\')
					{
						goto IL_00E4;
					}
					this.writer.Write('\\');
					this.writer.Write(str[i]);
					break;
				}
				IL_0141:
				i++;
				continue;
				IL_00E4:
				if (str[i] >= ' ' && str[i] <= '~')
				{
					this.writer.Write(str[i]);
					goto IL_0141;
				}
				JsonWriter.IntToHex((int)str[i], this.hex_seq);
				this.writer.Write("\\u");
				this.writer.Write(this.hex_seq);
				goto IL_0141;
			}
			this.writer.Write('"');
		}

		// Token: 0x06005DEE RID: 24046 RVA: 0x001DFF12 File Offset: 0x001DE112
		private void Unindent()
		{
			if (this.pretty_print)
			{
				this.indentation -= this.indent_value;
			}
		}

		// Token: 0x06005DEF RID: 24047 RVA: 0x001DFF2F File Offset: 0x001DE12F
		public override string ToString()
		{
			if (this.inst_string_builder == null)
			{
				return string.Empty;
			}
			return this.inst_string_builder.ToString();
		}

		// Token: 0x06005DF0 RID: 24048 RVA: 0x001DFF4C File Offset: 0x001DE14C
		public void Reset()
		{
			this.has_reached_end = false;
			this.ctx_stack.Clear();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
			if (this.inst_string_builder != null)
			{
				this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
			}
		}

		// Token: 0x06005DF1 RID: 24049 RVA: 0x001DFFA7 File Offset: 0x001DE1A7
		public void Write(bool boolean)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(boolean ? "true" : "false");
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF2 RID: 24050 RVA: 0x001DFFD7 File Offset: 0x001DE1D7
		public void Write(decimal number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF3 RID: 24051 RVA: 0x001E0004 File Offset: 0x001DE204
		public void Write(double number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			string text = Convert.ToString(number, JsonWriter.number_format);
			this.Put(text);
			if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
			{
				this.writer.Write(".0");
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF4 RID: 24052 RVA: 0x001E0063 File Offset: 0x001DE263
		public void Write(int number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF5 RID: 24053 RVA: 0x001E008F File Offset: 0x001DE28F
		public void Write(long number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF6 RID: 24054 RVA: 0x001E00BB File Offset: 0x001DE2BB
		public void Write(string str)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			if (str == null)
			{
				this.Put("null");
			}
			else
			{
				this.PutString(str);
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF7 RID: 24055 RVA: 0x001E00ED File Offset: 0x001DE2ED
		public void Write(ulong number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005DF8 RID: 24056 RVA: 0x001E011C File Offset: 0x001DE31C
		public void WriteArrayEnd()
		{
			this.DoValidation(Condition.InArray);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("]");
		}

		// Token: 0x06005DF9 RID: 24057 RVA: 0x001E0188 File Offset: 0x001DE388
		public void WriteArrayStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("[");
			this.context = new WriterContext();
			this.context.InArray = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x06005DFA RID: 24058 RVA: 0x001E01DC File Offset: 0x001DE3DC
		public void WriteObjectEnd()
		{
			this.DoValidation(Condition.InObject);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("}");
		}

		// Token: 0x06005DFB RID: 24059 RVA: 0x001E0248 File Offset: 0x001DE448
		public void WriteObjectStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("{");
			this.context = new WriterContext();
			this.context.InObject = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x06005DFC RID: 24060 RVA: 0x001E029C File Offset: 0x001DE49C
		public void WritePropertyName(string property_name)
		{
			this.DoValidation(Condition.Property);
			this.PutNewline();
			this.PutString(property_name);
			if (this.pretty_print)
			{
				if (property_name.Length > this.context.Padding)
				{
					this.context.Padding = property_name.Length;
				}
				for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
				{
					this.writer.Write(' ');
				}
				this.writer.Write(": ");
			}
			else
			{
				this.writer.Write(':');
			}
			this.context.ExpectingValue = true;
		}

		// Token: 0x04006D1F RID: 27935
		private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;

		// Token: 0x04006D20 RID: 27936
		private WriterContext context;

		// Token: 0x04006D21 RID: 27937
		private Stack<WriterContext> ctx_stack;

		// Token: 0x04006D22 RID: 27938
		private bool has_reached_end;

		// Token: 0x04006D23 RID: 27939
		private char[] hex_seq;

		// Token: 0x04006D24 RID: 27940
		private int indentation;

		// Token: 0x04006D25 RID: 27941
		private int indent_value;

		// Token: 0x04006D26 RID: 27942
		private StringBuilder inst_string_builder;

		// Token: 0x04006D27 RID: 27943
		private bool pretty_print;

		// Token: 0x04006D28 RID: 27944
		private bool validate;

		// Token: 0x04006D29 RID: 27945
		private TextWriter writer;
	}
}
