﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TestCSharpAst
{
	class Program
	{

		
		static void Main(string[] args)
		{
            TestPreprocessor();
		}

		static void TestNormal()
		{
			string filePath = "D:\\houhai.duan_rpg\\ProjRPG\\Assets\\GameCore\\TurnBasedGameCoreSource\\Static\\AdventureStatic.cs";
			//string filePath = "D:\\Projects\\python\\TestCSharpAst2\\AllInOneNoPreprocessor.cs";
			String input = System.IO.File.ReadAllText(filePath);
			ICharStream stream = CharStreams.fromString(input);
			ITokenSource lexer = new CSharpLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);
			CSharpParser parser = new CSharpParser(tokens);
			parser.BuildParseTree = true;
			IParseTree tree = parser.compilation_unit();
			Console.WriteLine(tree.ChildCount);
		}

		static void TestPreprocessor()
		{
            string filePath = "D:\\Projects\\Ast\\grammars-v4\\csharp\\not-ready-examples\\AllInOne2.cs";//"D:\\Projects\\python\\TestCSharpAst2\\TestCSharpAst\\Tests\\TestClass.cs";// "D:\\houhai.duan_rpg\\ProjRPG\\Assets\\GameCore\\TurnBasedGameCoreSource\\Static\\AdventureStatic.cs";//          
			String sourceCode = System.IO.File.ReadAllText(filePath);

            List<IToken> codeTokens = new List<IToken>();
            List<IToken> commentTokens = new List<IToken>();

            Lexer preprocessorLexer = new CSharpLexer(new AntlrInputStream(sourceCode));
            // Collect all tokens with lexer (CSharpLexer.g4).
            var tokens = preprocessorLexer.GetAllTokens();
            var directiveTokens = new List<IToken>();
            var directiveTokenSource = new ListTokenSource(directiveTokens);
            var directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
            CSharpPreprocessorParser preprocessorParser = new CSharpPreprocessorParser(directiveTokenStream);

            preprocessorParser.ConditionalSymbols.Add("GameCoreClient");

            int index = 0;
            bool compiliedTokens = true;
            while (index < tokens.Count)
            {
                var token = tokens[index];
                if (token.Type == CSharpLexer.SHARP)
                {
                    directiveTokens.Clear();
                    int directiveTokenIndex = index + 1;
                    // Collect all preprocessor directive tokens.
                    while (directiveTokenIndex < tokens.Count &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.Eof &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.DIRECTIVE_NEW_LINE &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.SHARP)
                    {
                        if (tokens[directiveTokenIndex].Channel == CSharpLexer.COMMENTS_CHANNEL)
                        {
                            commentTokens.Add(tokens[directiveTokenIndex]);
                        }
                        else if (tokens[directiveTokenIndex].Channel != Lexer.Hidden)
                        {
                            directiveTokens.Add(tokens[directiveTokenIndex]);
                        }
                        directiveTokenIndex++;
                    }

                    directiveTokenSource = new ListTokenSource(directiveTokens);
                    directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
                    preprocessorParser.TokenStream=directiveTokenStream;
                    //preprocessorParser.Reset();
                    // Parse condition in preprocessor directive (based on CSharpPreprocessorParser.g4 grammar).
                    CSharpPreprocessorParser.Preprocessor_directiveContext directive = preprocessorParser.preprocessor_directive();
                    // if true than next code is valid and not ignored.
                    compiliedTokens = directive.value;
                    //String directiveStr = tokens[index + 1].Text.Trim();
                    //if ("line".Equals(directiveStr) || "error".Equals(directiveStr) || "warning".Equals(directiveStr) || "define".Equals(directiveStr) || "endregion".Equals(directiveStr) || "endif".Equals(directiveStr) || "pragma".Equals(directiveStr))
                    //{
                    //    compiliedTokens = true;
                    //}
                    //String conditionalSymbol = null;
                    //if ("define".Equals(tokens[index + 1].Text))
                    //{
                    //    // add to the conditional symbols 
                    //    conditionalSymbol = tokens[index + 2].Text;
                    //    preprocessorParser.ConditionalSymbols.Add(conditionalSymbol);
                    //}
                    //if ("undef".Equals(tokens[index + 1].Text))
                    //{
                    //    conditionalSymbol = tokens[index + 2].Text;
                    //    preprocessorParser.ConditionalSymbols.Remove(conditionalSymbol);
                    //}
                    index = directiveTokenIndex - 1;
                    //compiliedTokens = directive.value;
                    //index = directiveTokenIndex - 1;
                }
                else if (token.Channel ==  CSharpLexer.COMMENTS_CHANNEL)
                {
                    commentTokens.Add(token); // Colect comment tokens (if required).
                }
                else if (token.Channel != Lexer.Hidden && token.Type != CSharpLexer.DIRECTIVE_NEW_LINE && compiliedTokens)
                {
                    codeTokens.Add(token); // Collect code tokens.
                }
                index++;
            }

            // At second stage tokens parsed in usual way.
            var codeTokenSource = new ListTokenSource(codeTokens);
            var codeTokenStream = new CommonTokenStream(codeTokenSource);
            CSharpParser parser = new CSharpParser(codeTokenStream);
            parser.BuildParseTree = true;
            // Parse syntax tree (CSharpParser.g4)
            var compilationUnit = parser.compilation_unit();
            Console.WriteLine(compilationUnit.ChildCount);
        }
	}
}
