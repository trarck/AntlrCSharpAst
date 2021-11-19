using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast
{
	public class AstNode
	{
		public struct Position
		{
			public int line;
			public int column;
			public int offset;
		}

		public AstNode parent;
		public List<AstNode> children;

		public Position start;
		public Position end;
	}

	public class DeclationNode:AstNode
	{
				
	}

	public class FunctionNode : AstNode
	{
	
	}

	public class FieldNode : AstNode
	{
		
	}
}
