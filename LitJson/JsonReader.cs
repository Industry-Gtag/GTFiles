using System;
using System.Collections.Generic;
using System.IO;

namespace LitJson
{
	// Token: 0x02000EFA RID: 3834
	public class JsonReader
	{
		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06005DC6 RID: 24006 RVA: 0x001DF1C8 File Offset: 0x001DD3C8
		// (set) Token: 0x06005DC7 RID: 24007 RVA: 0x001DF1D5 File Offset: 0x001DD3D5
		public bool AllowComments
		{
			get
			{
				return this.lexer.AllowComments;
			}
			set
			{
				this.lexer.AllowComments = value;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06005DC8 RID: 24008 RVA: 0x001DF1E3 File Offset: 0x001DD3E3
		// (set) Token: 0x06005DC9 RID: 24009 RVA: 0x001DF1F0 File Offset: 0x001DD3F0
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.lexer.AllowSingleQuotedStrings;
			}
			set
			{
				this.lexer.AllowSingleQuotedStrings = value;
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06005DCA RID: 24010 RVA: 0x001DF1FE File Offset: 0x001DD3FE
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06005DCB RID: 24011 RVA: 0x001DF206 File Offset: 0x001DD406
		public bool EndOfJson
		{
			get
			{
				return this.end_of_json;
			}
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06005DCC RID: 24012 RVA: 0x001DF20E File Offset: 0x001DD40E
		public JsonToken Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06005DCD RID: 24013 RVA: 0x001DF216 File Offset: 0x001DD416
		public object Value
		{
			get
			{
				return this.token_value;
			}
		}

		// Token: 0x06005DCE RID: 24014 RVA: 0x001DF21E File Offset: 0x001DD41E
		static JsonReader()
		{
			JsonReader.PopulateParseTable();
		}

		// Token: 0x06005DCF RID: 24015 RVA: 0x001DF225 File Offset: 0x001DD425
		public JsonReader(string json_text)
			: this(new StringReader(json_text), true)
		{
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x001DF234 File Offset: 0x001DD434
		public JsonReader(TextReader reader)
			: this(reader, false)
		{
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x001DF240 File Offset: 0x001DD440
		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.read_started = false;
			this.automaton_stack = new Stack<int>();
			this.automaton_stack.Push(65553);
			this.automaton_stack.Push(65543);
			this.lexer = new Lexer(reader);
			this.end_of_input = false;
			this.end_of_json = false;
			this.reader = reader;
			this.reader_is_owned = owned;
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x001DF2CC File Offset: 0x001DD4CC
		private static void PopulateParseTable()
		{
			JsonReader.parse_table = new Dictionary<int, IDictionary<int, int[]>>();
			JsonReader.TableAddRow(ParserToken.Array);
			JsonReader.TableAddCol(ParserToken.Array, 91, new int[] { 91, 65549 });
			JsonReader.TableAddRow(ParserToken.ArrayPrime);
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 34, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 91, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 93, new int[] { 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 123, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65537, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65538, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65539, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65540, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddRow(ParserToken.Object);
			JsonReader.TableAddCol(ParserToken.Object, 123, new int[] { 123, 65545 });
			JsonReader.TableAddRow(ParserToken.ObjectPrime);
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 34, new int[] { 65546, 65547, 125 });
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 125, new int[] { 125 });
			JsonReader.TableAddRow(ParserToken.Pair);
			JsonReader.TableAddCol(ParserToken.Pair, 34, new int[] { 65552, 58, 65550 });
			JsonReader.TableAddRow(ParserToken.PairRest);
			JsonReader.TableAddCol(ParserToken.PairRest, 44, new int[] { 44, 65546, 65547 });
			JsonReader.TableAddCol(ParserToken.PairRest, 125, new int[] { 65554 });
			JsonReader.TableAddRow(ParserToken.String);
			JsonReader.TableAddCol(ParserToken.String, 34, new int[] { 34, 65541, 34 });
			JsonReader.TableAddRow(ParserToken.Text);
			JsonReader.TableAddCol(ParserToken.Text, 91, new int[] { 65548 });
			JsonReader.TableAddCol(ParserToken.Text, 123, new int[] { 65544 });
			JsonReader.TableAddRow(ParserToken.Value);
			JsonReader.TableAddCol(ParserToken.Value, 34, new int[] { 65552 });
			JsonReader.TableAddCol(ParserToken.Value, 91, new int[] { 65548 });
			JsonReader.TableAddCol(ParserToken.Value, 123, new int[] { 65544 });
			JsonReader.TableAddCol(ParserToken.Value, 65537, new int[] { 65537 });
			JsonReader.TableAddCol(ParserToken.Value, 65538, new int[] { 65538 });
			JsonReader.TableAddCol(ParserToken.Value, 65539, new int[] { 65539 });
			JsonReader.TableAddCol(ParserToken.Value, 65540, new int[] { 65540 });
			JsonReader.TableAddRow(ParserToken.ValueRest);
			JsonReader.TableAddCol(ParserToken.ValueRest, 44, new int[] { 44, 65550, 65551 });
			JsonReader.TableAddCol(ParserToken.ValueRest, 93, new int[] { 65554 });
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x001DF645 File Offset: 0x001DD845
		private static void TableAddCol(ParserToken row, int col, params int[] symbols)
		{
			JsonReader.parse_table[(int)row].Add(col, symbols);
		}

		// Token: 0x06005DD4 RID: 24020 RVA: 0x001DF659 File Offset: 0x001DD859
		private static void TableAddRow(ParserToken rule)
		{
			JsonReader.parse_table.Add((int)rule, new Dictionary<int, int[]>());
		}

		// Token: 0x06005DD5 RID: 24021 RVA: 0x001DF66C File Offset: 0x001DD86C
		private void ProcessNumber(string number)
		{
			double num;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, out num))
			{
				this.token = JsonToken.Double;
				this.token_value = num;
				return;
			}
			int num2;
			if (int.TryParse(number, out num2))
			{
				this.token = JsonToken.Int;
				this.token_value = num2;
				return;
			}
			long num3;
			if (long.TryParse(number, out num3))
			{
				this.token = JsonToken.Long;
				this.token_value = num3;
				return;
			}
			this.token = JsonToken.Int;
			this.token_value = 0;
		}

		// Token: 0x06005DD6 RID: 24022 RVA: 0x001DF708 File Offset: 0x001DD908
		private void ProcessSymbol()
		{
			if (this.current_symbol == 91)
			{
				this.token = JsonToken.ArrayStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 93)
			{
				this.token = JsonToken.ArrayEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 123)
			{
				this.token = JsonToken.ObjectStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 125)
			{
				this.token = JsonToken.ObjectEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 34)
			{
				if (this.parser_in_string)
				{
					this.parser_in_string = false;
					this.parser_return = true;
					return;
				}
				if (this.token == JsonToken.None)
				{
					this.token = JsonToken.String;
				}
				this.parser_in_string = true;
				return;
			}
			else
			{
				if (this.current_symbol == 65541)
				{
					this.token_value = this.lexer.StringValue;
					return;
				}
				if (this.current_symbol == 65539)
				{
					this.token = JsonToken.Boolean;
					this.token_value = false;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65540)
				{
					this.token = JsonToken.Null;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65537)
				{
					this.ProcessNumber(this.lexer.StringValue);
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65546)
				{
					this.token = JsonToken.PropertyName;
					return;
				}
				if (this.current_symbol == 65538)
				{
					this.token = JsonToken.Boolean;
					this.token_value = true;
					this.parser_return = true;
				}
				return;
			}
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x001DF87A File Offset: 0x001DDA7A
		private bool ReadToken()
		{
			if (this.end_of_input)
			{
				return false;
			}
			this.lexer.NextToken();
			if (this.lexer.EndOfInput)
			{
				this.Close();
				return false;
			}
			this.current_input = this.lexer.Token;
			return true;
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001DF8B9 File Offset: 0x001DDAB9
		public void Close()
		{
			if (this.end_of_input)
			{
				return;
			}
			this.end_of_input = true;
			this.end_of_json = true;
			if (this.reader_is_owned)
			{
				this.reader.Close();
			}
			this.reader = null;
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x001DF8EC File Offset: 0x001DDAEC
		public bool Read()
		{
			if (this.end_of_input)
			{
				return false;
			}
			if (this.end_of_json)
			{
				this.end_of_json = false;
				this.automaton_stack.Clear();
				this.automaton_stack.Push(65553);
				this.automaton_stack.Push(65543);
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.token = JsonToken.None;
			this.token_value = null;
			if (!this.read_started)
			{
				this.read_started = true;
				if (!this.ReadToken())
				{
					return false;
				}
			}
			while (!this.parser_return)
			{
				this.current_symbol = this.automaton_stack.Pop();
				this.ProcessSymbol();
				if (this.current_symbol == this.current_input)
				{
					if (!this.ReadToken())
					{
						if (this.automaton_stack.Peek() != 65553)
						{
							throw new JsonException("Input doesn't evaluate to proper JSON text");
						}
						return this.parser_return;
					}
				}
				else
				{
					int[] array;
					try
					{
						array = JsonReader.parse_table[this.current_symbol][this.current_input];
					}
					catch (KeyNotFoundException ex)
					{
						throw new JsonException((ParserToken)this.current_input, ex);
					}
					if (array[0] != 65554)
					{
						for (int i = array.Length - 1; i >= 0; i--)
						{
							this.automaton_stack.Push(array[i]);
						}
					}
				}
			}
			if (this.automaton_stack.Peek() == 65553)
			{
				this.end_of_json = true;
			}
			return true;
		}

		// Token: 0x04006D06 RID: 27910
		private static IDictionary<int, IDictionary<int, int[]>> parse_table;

		// Token: 0x04006D07 RID: 27911
		private Stack<int> automaton_stack;

		// Token: 0x04006D08 RID: 27912
		private int current_input;

		// Token: 0x04006D09 RID: 27913
		private int current_symbol;

		// Token: 0x04006D0A RID: 27914
		private bool end_of_json;

		// Token: 0x04006D0B RID: 27915
		private bool end_of_input;

		// Token: 0x04006D0C RID: 27916
		private Lexer lexer;

		// Token: 0x04006D0D RID: 27917
		private bool parser_in_string;

		// Token: 0x04006D0E RID: 27918
		private bool parser_return;

		// Token: 0x04006D0F RID: 27919
		private bool read_started;

		// Token: 0x04006D10 RID: 27920
		private TextReader reader;

		// Token: 0x04006D11 RID: 27921
		private bool reader_is_owned;

		// Token: 0x04006D12 RID: 27922
		private object token_value;

		// Token: 0x04006D13 RID: 27923
		private JsonToken token;
	}
}
